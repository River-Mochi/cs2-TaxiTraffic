// <copyright file="TaxiTrafficSystem.Debug.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Debug.cs
// Optional debug logging and lightweight DEBUG-only performance sampling.

namespace TaxiTraffic
{
    using System;           // Math
#if DEBUG
    using System.Diagnostics; // Stopwatch
#endif
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

#if DEBUG
        private const double kDebugPerfLogIntervalSeconds = 30.0;

        private long m_DebugUnstickSamples;
        private double m_DebugUnstickTotalMs;
        private double m_DebugUnstickLastMs;
        private double m_DebugUnstickMaxMs;
        private int m_DebugUnstickLastScanned;
        private int m_DebugUnstickMaxScanned;
        private int m_DebugUnstickLastWaitingTransport;
        private int m_DebugUnstickLastTaxiQueue;
        private int m_DebugUnstickLastCleared;
        private int m_DebugUnstickClearedTotal;

        private int m_DebugTripSourceRepairsSincePerfLog;
        private int m_DebugTripSourceRepairsTotal;
        private double m_DebugLastPerfLogRealtime;
#endif

        private float m_DebugTimerSeconds;

        private void ResetDebugOnCityLoaded()
        {
            m_DebugTimerSeconds = 0f;

#if DEBUG
            m_DebugUnstickSamples = 0;
            m_DebugUnstickTotalMs = 0.0;
            m_DebugUnstickLastMs = 0.0;
            m_DebugUnstickMaxMs = 0.0;
            m_DebugUnstickLastScanned = 0;
            m_DebugUnstickMaxScanned = 0;
            m_DebugUnstickLastWaitingTransport = 0;
            m_DebugUnstickLastTaxiQueue = 0;
            m_DebugUnstickLastCleared = 0;
            m_DebugUnstickClearedTotal = 0;

            m_DebugTripSourceRepairsSincePerfLog = 0;
            m_DebugTripSourceRepairsTotal = 0;
            m_DebugLastPerfLogRealtime = UnityEngine.Time.realtimeSinceStartupAsDouble;
#endif
        }

#if DEBUG
        private static long DebugGetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }

        private void DebugRecordTripSourceRepairs(int repaired)
        {
            if (repaired <= 0)
                return;

            m_DebugTripSourceRepairsSincePerfLog += repaired;
            m_DebugTripSourceRepairsTotal += repaired;
        }

        private void DebugRecordUnstickTaxiQueues(
            long startTicks,
            int scanned,
            int waitingTransport,
            int taxiQueue,
            int cleared)
        {
            long elapsedTicks = Stopwatch.GetTimestamp() - startTicks;
            double elapsedMs = elapsedTicks * 1000.0 / Stopwatch.Frequency;

            m_DebugUnstickSamples++;
            m_DebugUnstickTotalMs += elapsedMs;
            m_DebugUnstickLastMs = elapsedMs;
            m_DebugUnstickMaxMs = Math.Max(m_DebugUnstickMaxMs, elapsedMs);
            m_DebugUnstickLastScanned = scanned;
            m_DebugUnstickMaxScanned = Math.Max(m_DebugUnstickMaxScanned, scanned);
            m_DebugUnstickLastWaitingTransport = waitingTransport;
            m_DebugUnstickLastTaxiQueue = taxiQueue;
            m_DebugUnstickLastCleared = cleared;
            m_DebugUnstickClearedTotal += cleared;

            Setting? setting = Mod.Setting;
            if (setting is null || !setting.EnableDebugLogging)
                return;

            double now = UnityEngine.Time.realtimeSinceStartupAsDouble;
            if (now - m_DebugLastPerfLogRealtime < kDebugPerfLogIntervalSeconds)
                return;

            m_DebugLastPerfLogRealtime = now;

            double averageMs =
                m_DebugUnstickSamples > 0
                    ? m_DebugUnstickTotalMs / m_DebugUnstickSamples
                    : 0.0;

            int repairsSincePerfLog = m_DebugTripSourceRepairsSincePerfLog;
            m_DebugTripSourceRepairsSincePerfLog = 0;

            CS2Shared.RiverMochi.LogUtils.Info(
                Mod.s_Log,
                () =>
                    $"{Mod.ModTag} TaxiPerf: UnstickTaxiQueues " +
                    $"lastMs={m_DebugUnstickLastMs:F3}, avgMs={averageMs:F3}, maxMs={m_DebugUnstickMaxMs:F3}, " +
                    $"samples={m_DebugUnstickSamples}, scannedLast={m_DebugUnstickLastScanned}, scannedMax={m_DebugUnstickMaxScanned}, " +
                    $"waitingTransportLast={m_DebugUnstickLastWaitingTransport}, taxiQueueLast={m_DebugUnstickLastTaxiQueue}, " +
                    $"clearedLast={m_DebugUnstickLastCleared}, clearedTotal={m_DebugUnstickClearedTotal}, " +
                    $"tripSourceRepairsSincePerfLog={repairsSincePerfLog}, tripSourceRepairsTotal={m_DebugTripSourceRepairsTotal}");
        }
#endif

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

                if (citizenEntity != Entity.Null && SystemAPI.Exists(citizenEntity))
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
