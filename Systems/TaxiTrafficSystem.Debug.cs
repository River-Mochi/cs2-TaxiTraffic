// <copyright file="TaxiTrafficSystem.Debug.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Debug.cs
// Optional debug logging helpers (controlled by settings).

namespace TaxiTraffic
{
    using System;           // Math
    using Game.Citizens;    // Citizen, CitizenFlags, HouseholdMember, TravelPurpose
    using Game.City;        // StatisticType, PassengerType
    using Game.Companies;   // ResourceBuyer
    using Game.Creatures;   // ResidentFlags, GroupMember, GroupCreature
    using Game.Vehicles;    // CurrentVehicle, Taxi, TaxiFlags
    using Unity.Entities;

    public partial class TaxiTrafficSystem
    {
        private const int kDebugPassengerDetailMax = 8;

        // Verbose logging is intentionally slow; logs get huge very fast.
        private const float kDebugMinSummaryIntervalSeconds = 60f;

        // Keeps TaxiSummary counters reasonably fresh without tying UI display to an age row.
        private const double kDebugForceStatusRefreshMaxAgeSeconds = 30.0;

        private float m_DebugTimerSeconds;

        private void ResetDebugOnCityLoaded()
        {
            m_DebugTimerSeconds = 0f;
        }

        private void TickDebugLogging(Setting setting, float intervalSeconds)
        {
            float effectiveIntervalSeconds = Math.Max(intervalSeconds, kDebugMinSummaryIntervalSeconds);

            m_DebugTimerSeconds += UnityEngine.Time.unscaledDeltaTime;
            if (m_DebugTimerSeconds < effectiveIntervalSeconds)
                return;

            m_DebugTimerSeconds = 0f;

            if (IsStatusSnapshotStale(kDebugForceStatusRefreshMaxAgeSeconds))
                RequestStatusRefresh(force: true);

            int dailyTaxiCitizen = 0;
            int dailyTaxiTourist = 0;

            try
            {
                if (m_CityStatisticsSystem != null)
                {
                    dailyTaxiCitizen = m_CityStatisticsSystem.GetStatisticValue(
                        StatisticType.PassengerCountTaxi, (int)PassengerType.Citizen);

                    dailyTaxiTourist = m_CityStatisticsSystem.GetStatisticValue(
                        StatisticType.PassengerCountTaxi, (int)PassengerType.Tourist);
                }
            }
            catch
            {
            }

            CS2Shared.RiverMochi.LogUtils.Info(
                Mod.s_Log,
                () =>
                    $"{Mod.ModTag} TaxiSummary: " +
                    $"residentTaxiAllowedPercent={setting.ResidentsAllowedToUseTaxis}, blockCommuters={setting.BlockCommuters}, blockTourists={setting.BlockTourists}, " +
                    $"taxis={s_StatusTaxisTotal}, transporting={s_StatusTaxiTransporting}, boarding={s_StatusTaxiBoarding}, returning={s_StatusTaxiReturning}, dispatched={s_StatusTaxiDispatched}, enRoute={s_StatusTaxiEnRoute}, parked={s_StatusTaxiParked}, accident={s_StatusTaxiAccident}, " +
                    $"fromOutside={s_StatusTaxiFromOutside}, disabled={s_StatusTaxiDisabled}, withServiceDispatch={s_StatusTaxiWithDispatchBuffer}, " +
                    $"requests[customer={s_StatusReqCustomer}, outside={s_StatusReqOutside}, none={s_StatusReqNone}], " +
                    $"taxiStandDebug(waiting={s_StatusWaitingTaxiStandTotal}, taxisRequestedToParkAtStands={s_StatusReqStand}), " +
                    $"custSeekers(ignoreTaxi={s_StatusReqCustomerSeekerIgnoreTaxi}/{s_StatusReqCustomerSeekerHasResident}), " +
                    $"outSeekers(ignoreTaxi={s_StatusReqOutsideSeekerIgnoreTaxi}/{s_StatusReqOutsideSeekerHasResident}), " +
                    $"passengers(ignoreTaxi={s_StatusPassengerIgnoreTaxi}/{s_StatusPassengerHasResident}, totalPassengers={s_StatusPassengerTotal}), " +
                    $"residents(ignoreTaxi={s_StatusResidentsIgnoreTaxi}/{s_StatusResidentsTotal}, blockedMark={s_StatusResidentsForcedMarker}, allowedMark={s_StatusResidentsAllowedMarker}), " +
                    $"commuters(ignoreTaxi={s_StatusCommutersIgnoreTaxi}/{s_StatusCommutersTotal}), " +
                    $"tourists(ignoreTaxi={s_StatusTouristsIgnoreTaxi}/{s_StatusTouristsTotal}), " +
                    $"groups(skipped={s_StatusLastSkippedGroupTravelers}, cleared={s_StatusLastClearedGroupTravelers}, linkedNow={s_StatusResidentsGroupLinked}, linkedIgnoreTaxi={s_StatusResidentsGroupLinkedIgnoreTaxi}), " +
                    $"waitingTransport(total={s_StatusWaitingTransportTotal}, taxiStand={s_StatusWaitingTaxiStandTotal}), " +
                    $"statsDailyTaxi(citizen={dailyTaxiCitizen}, tourist={dailyTaxiTourist}, approxPerMonth={30 * (dailyTaxiCitizen + dailyTaxiTourist)})");

            LogActiveTaxiPassengers();
        }

        private void LogActiveTaxiPassengers()
        {
            if (!Mod.s_Log.isDebugEnabled)
                return;

            int inTaxi = 0;
            int examples = 0;

            foreach ((RefRO<Game.Creatures.Resident> resident, RefRO<CurrentVehicle> currentVehicle, Entity passengerEntity) in SystemAPI
                         .Query<RefRO<Game.Creatures.Resident>, RefRO<CurrentVehicle>>()
                         .WithEntityAccess())
            {
                Entity vehicle = currentVehicle.ValueRO.m_Vehicle;
                if (vehicle == Entity.Null || !SystemAPI.HasComponent<Taxi>(vehicle))
                    continue;

                inTaxi++;

                if (examples >= kDebugPassengerDetailMax)
                    continue;

                examples++;

                ResidentFlags rf = resident.ValueRO.m_Flags;
                bool ignoreTaxi = (rf & ResidentFlags.IgnoreTaxi) != 0;
                bool blockedMark = SystemAPI.HasComponent<IgnoreTaxiMark>(passengerEntity);
                bool allowedMark = SystemAPI.HasComponent<TaxiAllowedMark>(passengerEntity);
                bool groupMember = SystemAPI.HasComponent<GroupMember>(passengerEntity);
                bool groupCreature = SystemAPI.HasBuffer<GroupCreature>(passengerEntity);

                Entity citizenEntity = resident.ValueRO.m_Citizen;

                CitizenFlags citizenFlags = 0;
                bool hhCommuter = false;
                bool hhTourist = false;

                if (citizenEntity != Entity.Null && EntityManager.Exists(citizenEntity))
                {
                    if (SystemAPI.HasComponent<Citizen>(citizenEntity))
                        citizenFlags = SystemAPI.GetComponentRO<Citizen>(citizenEntity).ValueRO.m_State;

                    if (SystemAPI.HasComponent<HouseholdMember>(citizenEntity))
                    {
                        Entity household = SystemAPI.GetComponentRO<HouseholdMember>(citizenEntity).ValueRO.m_Household;
                        if (household != Entity.Null)
                        {
                            hhCommuter = SystemAPI.HasComponent<CommuterHousehold>(household);
                            hhTourist = SystemAPI.HasComponent<TouristHousehold>(household);
                        }
                    }
                }

                bool hasResourceBuyer = SystemAPI.HasComponent<ResourceBuyer>(passengerEntity);

                string purpose = "none";
                if (SystemAPI.HasComponent<TravelPurpose>(passengerEntity))
                {
                    TravelPurpose tp = SystemAPI.GetComponentRO<TravelPurpose>(passengerEntity).ValueRO;
                    purpose = tp.m_Purpose.ToString();
                }

                TaxiFlags taxiFlags = SystemAPI.GetComponentRO<Taxi>(vehicle).ValueRO.m_State;

                CS2Shared.RiverMochi.LogUtils.Debug(
                    Mod.s_Log,
                    () =>
                        $"{Mod.ModTag} TaxiPassengerNow: passenger={passengerEntity.Index}:{passengerEntity.Version} " +
                        $"vehicle={vehicle.Index}:{vehicle.Version} taxiFlags={taxiFlags} " +
                        $"ignoreTaxi={ignoreTaxi} blockedMark={blockedMark} allowedMark={allowedMark} " +
                        $"groupMember={groupMember} groupCreature={groupCreature} " +
                        $"citizenFlags={citizenFlags} hhCommuter={hhCommuter} hhTourist={hhTourist} " +
                        $"purpose={purpose} resourceBuyer={hasResourceBuyer}");
            }

            if (inTaxi > 0)
            {
                CS2Shared.RiverMochi.LogUtils.Debug(
                    Mod.s_Log,
                    () => $"{Mod.ModTag} TaxiPassengerNow: totalResidentsInTaxi={inTaxi} (examplesShown={examples}/{kDebugPassengerDetailMax})");
            }
        }
    }
}
