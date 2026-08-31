// <copyright file="TaxiTrafficSystem.Status.Scan.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Status/TaxiTrafficSystem.Status.Scan.cs
// Live taxi, passenger, resident, request, stand, and depot Status scans.

using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Events;
using Game.Tools;
using Unity.Entities;

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem
    {
        private void UpdateStatusSnapshot(bool detailed)
        {
            ClearSnapshotValues();

            UpdateStatusMonthlyPassengers();
            UpdateStatusTaxiDepotAndStandCounts();

            foreach ((RefRO<Resident> residentRef, Entity entity) in SystemAPI
                         .Query<RefRO<Resident>>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                s_StatusActiveCimsTotal++;

                Resident resident = residentRef.ValueRO;
                bool blockedMark =
                    SystemAPI.HasComponent<IgnoreTaxiMark>(entity);

                GetResidentGroup(
                    resident,
                    out bool isCommuter,
                    out bool isTourist);

                if (isCommuter)
                {
                    s_StatusCommutersTotal++;

                    if (blockedMark)
                        s_StatusCommutersBlockedMark++;
                }
                else if (isTourist)
                {
                    s_StatusTouristsTotal++;

                    if (blockedMark)
                        s_StatusTouristsBlockedMark++;
                }
                else
                {
                    s_StatusLocalCimsTotal++;

                    if (blockedMark)
                        s_StatusLocalBlockedMark++;
                }

                if (blockedMark)
                    s_StatusOwnedBlocksTotal++;

                if (detailed &&
                    (resident.m_Flags & ResidentFlags.IgnoreTaxi) != 0)
                {
                    s_StatusResidentsIgnoreTaxi++;
                }
            }

            // Lightweight player-facing waiting total.
            foreach ((RefRO<Resident> residentRef,
                      RefRO<HumanCurrentLane> _) in SystemAPI
                         .Query<RefRO<Resident>, RefRO<HumanCurrentLane>>()
                         .WithNone<Deleted, Temp>())
            {
                if ((residentRef.ValueRO.m_Flags &
                     ResidentFlags.WaitingTransport) != 0)
                {
                    s_StatusWaitingTransportTotal++;
                }
            }

            UpdateStatusTaxiFleetAndPassengers(detailed);
            UpdateStatusTaxiRequestPurposes();

            if (detailed)
            {
                UpdateDetailedStatusRequests();
                UpdateDetailedTaxiStandWaiting();
            }
        }

        private void GetResidentGroup(
            Resident resident,
            out bool isCommuter,
            out bool isTourist)
        {
            isCommuter = false;
            isTourist = false;

            Entity citizen = resident.m_Citizen;
            if (citizen == Entity.Null ||
                !SystemAPI.HasComponent<HouseholdMember>(citizen))
            {
                return;
            }

            Entity household =
                SystemAPI.GetComponentRO<HouseholdMember>(
                    citizen).ValueRO.m_Household;

            if (household == Entity.Null)
                return;

            isCommuter =
                SystemAPI.HasComponent<CommuterHousehold>(household);

            if (!isCommuter)
            {
                isTourist =
                    SystemAPI.HasComponent<TouristHousehold>(household);
            }
        }

        private void UpdateStatusTaxiRequestPurposes()
        {
            foreach (RefRO<Game.Simulation.TaxiRequest> request in SystemAPI
                         .Query<RefRO<Game.Simulation.TaxiRequest>>()
                         .WithNone<Deleted, Temp>())
            {
                if (request.ValueRO.m_Type !=
                    Game.Simulation.TaxiRequestType.Customer)
                {
                    continue;
                }

                CountRequestPurpose(request.ValueRO.m_Seeker);
            }
        }

        private void UpdateDetailedStatusRequests()
        {
            foreach ((RefRO<Game.Simulation.TaxiRequest> reqRef,
                      Entity requestEntity) in SystemAPI
                         .Query<RefRO<Game.Simulation.TaxiRequest>>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                Game.Simulation.TaxiRequest req = reqRef.ValueRO;

                switch (req.m_Type)
                {
                    case Game.Simulation.TaxiRequestType.Stand:
                        s_StatusReqStand++;
                        break;

                    case Game.Simulation.TaxiRequestType.Customer:
                        s_StatusReqCustomer++;

                        CountRequestSeekerResident(
                            req.m_Seeker,
                            ref s_StatusReqCustomerSeekerHasResident,
                            ref s_StatusReqCustomerSeekerIgnoreTaxi,
                            ref s_StatusReqCustomerSeekerBlockedMark);

                        break;

                    case Game.Simulation.TaxiRequestType.Outside:
                    {
                        s_StatusReqOutside++;

                        bool reversed = false;

                        if (SystemAPI.HasComponent<Game.Simulation.ServiceRequest>(
                                requestEntity))
                        {
                            Game.Simulation.ServiceRequest service =
                                SystemAPI.GetComponentRO<
                                    Game.Simulation.ServiceRequest>(
                                    requestEntity).ValueRO;

                            reversed =
                                (service.m_Flags &
                                 Game.Simulation.ServiceRequestFlags.Reversed) != 0;
                        }

                        if (reversed)
                        {
                            s_StatusReqOutsideSupply++;
                        }
                        else
                        {
                            s_StatusReqOutsideRider++;

                            CountRequestSeekerResident(
                                req.m_Seeker,
                                ref s_StatusReqOutsideSeekerHasResident,
                                ref s_StatusReqOutsideSeekerIgnoreTaxi,
                                ref s_StatusReqOutsideSeekerBlockedMark);
                        }

                        break;
                    }

                    default:
                        s_StatusReqNone++;
                        break;
                }
            }
        }

        private void CountRequestPurpose(Entity seeker)
        {
            if (seeker == Entity.Null ||
                !SystemAPI.Exists(seeker) ||
                !SystemAPI.HasComponent<Resident>(seeker))
            {
                s_StatusReqPurposeOther++;
                return;
            }

            Resident resident =
                SystemAPI.GetComponentRO<Resident>(seeker).ValueRO;

            Entity citizen = resident.m_Citizen;

            if (citizen == Entity.Null ||
                !SystemAPI.HasComponent<TravelPurpose>(citizen))
            {
                s_StatusReqPurposeOther++;
                return;
            }

            Purpose purpose =
                SystemAPI.GetComponentRO<TravelPurpose>(
                    citizen).ValueRO.m_Purpose;

            switch (purpose)
            {
                case Purpose.Leisure: s_StatusReqPurposeLeisure++;
                    break;

                case Purpose.GoingHome: s_StatusReqPurposeHome++;
                    break;

                case Purpose.GoingToWork: s_StatusReqPurposeWork++;
                    break;

                case Purpose.GoingToSchool: s_StatusReqPurposeSchool++;
                    break;

                case Purpose.Shopping: s_StatusReqPurposeShopping++;
                    break;

                default: s_StatusReqPurposeOther++;
                    break;
            }
        }

        private void UpdateDetailedTaxiStandWaiting()
        {
            foreach (RefRO<Game.Routes.WaitingPassengers> waiting in SystemAPI
                         .Query<RefRO<Game.Routes.WaitingPassengers>>()
                         .WithAll<Game.Routes.TaxiStand>()
                         .WithNone<Deleted, Temp>())
            {
                int count = waiting.ValueRO.m_Count;

                if (count > 0)
                    s_StatusWaitingTaxiStandTotal += count;
            }
        }

        private void UpdateStatusTaxiFleetAndPassengers(bool detailed)
        {
            foreach ((RefRO<Game.Vehicles.Taxi> taxiRef,
                      Entity taxiEntity) in SystemAPI
                         .Query<RefRO<Game.Vehicles.Taxi>>()
                         .WithNone<Deleted, Temp>()
                         .WithEntityAccess())
            {
                s_StatusTaxisTotal++;

                if (SystemAPI.HasComponent<Game.Vehicles.ParkedCar>(taxiEntity))
                    s_StatusTaxiParkedNow++;
                else
                    s_StatusTaxiActiveNow++;

                Game.Vehicles.TaxiFlags flags =
                    taxiRef.ValueRO.m_State;

                if ((flags & Game.Vehicles.TaxiFlags.FromOutside) != 0)
                    s_StatusTaxiFromOutside++;

                if (detailed)
                {
                    if (SystemAPI.HasComponent<InvolvedInAccident>(taxiEntity))
                        s_StatusTaxiAccident++;
                    else if (SystemAPI.HasComponent<Game.Vehicles.ParkedCar>(taxiEntity))
                        s_StatusTaxiParked++;
                    else if ((flags & Game.Vehicles.TaxiFlags.Returning) != 0)
                        s_StatusTaxiReturning++;
                    else if ((flags & Game.Vehicles.TaxiFlags.Dispatched) != 0)
                        s_StatusTaxiDispatched++;
                    else if ((flags & Game.Vehicles.TaxiFlags.Boarding) != 0)
                        s_StatusTaxiBoarding++;
                    else if ((flags & Game.Vehicles.TaxiFlags.Transporting) != 0)
                        s_StatusTaxiTransporting++;
                    else
                        s_StatusTaxiEnRoute++;

                    if ((flags & Game.Vehicles.TaxiFlags.Disabled) != 0)
                        s_StatusTaxiDisabled++;

                    if (SystemAPI.HasBuffer<Game.Simulation.ServiceDispatch>(
                            taxiEntity))
                    {
                        DynamicBuffer<Game.Simulation.ServiceDispatch> dispatches =
                            SystemAPI.GetBuffer<Game.Simulation.ServiceDispatch>(
                                taxiEntity);

                        if (dispatches.IsCreated && dispatches.Length > 0)
                            s_StatusTaxiWithDispatchBuffer++;
                    }
                }

                if (!SystemAPI.HasBuffer<Game.Vehicles.Passenger>(taxiEntity))
                    continue;

                DynamicBuffer<Game.Vehicles.Passenger> passengers =
                    SystemAPI.GetBuffer<Game.Vehicles.Passenger>(taxiEntity);

                for (int i = 0; i < passengers.Length; i++)
                {
                    Entity passenger = passengers[i].m_Passenger;
                    s_StatusPassengerTotal++;

                    if (!SystemAPI.HasComponent<Resident>(passenger))
                        continue;

                    s_StatusPassengerHasResident++;

                    if (SystemAPI.HasComponent<IgnoreTaxiMark>(passenger))
                        s_StatusPassengerBlockedMark++;

                    Resident passengerResident =
                        SystemAPI.GetComponentRO<Resident>(
                            passenger).ValueRO;

                    GetResidentGroup(
                        passengerResident,
                        out bool isCommuter,
                        out bool isTourist);

                    if (!isCommuter && !isTourist)
                        s_StatusPassengerLocal++;

                    if (detailed &&
                        (passengerResident.m_Flags &
                         ResidentFlags.IgnoreTaxi) != 0)
                    {
                        s_StatusPassengerIgnoreTaxi++;
                    }
                }
            }
        }

        private void CountRequestSeekerResident(
            Entity seeker,
            ref int hasResident,
            ref int ignoreTaxi,
            ref int blockedMark)
        {
            if (seeker == Entity.Null ||
                !SystemAPI.Exists(seeker) ||
                !SystemAPI.HasComponent<Resident>(seeker))
            {
                return;
            }

            hasResident++;

            ResidentFlags flags =
                SystemAPI.GetComponentRO<Resident>(
                    seeker).ValueRO.m_Flags;

            if ((flags & ResidentFlags.IgnoreTaxi) != 0)
                ignoreTaxi++;

            if (SystemAPI.HasComponent<IgnoreTaxiMark>(seeker))
                blockedMark++;
        }


        private void UpdateStatusTaxiDepotAndStandCounts()
        {
            s_StatusTaxiStandsTotal = 0;
            s_StatusTaxiDepotsTotal = 0;
            s_StatusTaxiDepotsLocal = 0;
            s_StatusTaxiDepotsOutside = 0;
            s_StatusTaxiDepotsWithDispatchCenter = 0;

            foreach (RefRO<Game.Routes.TaxiStand> _ in SystemAPI
                         .Query<RefRO<Game.Routes.TaxiStand>>()
                         .WithNone<Deleted, Temp>())
            {
                s_StatusTaxiStandsTotal++;
            }

            foreach ((RefRO<Game.Buildings.TransportDepot> depot,
                      RefRO<Game.Prefabs.PrefabRef> prefabRef,
                      Entity depotEntity) in SystemAPI
                         .Query<
                             RefRO<Game.Buildings.TransportDepot>,
                             RefRO<Game.Prefabs.PrefabRef>>()
                         .WithNone<
                             Deleted,
                             Temp,
                             Game.Buildings.ServiceUpgrade>()
                         .WithEntityAccess())
            {
                Entity prefab = prefabRef.ValueRO.m_Prefab;

                if (prefab == Entity.Null ||
                    !SystemAPI.HasComponent<
                        Game.Prefabs.TransportDepotData>(prefab))
                {
                    continue;
                }

                Game.Prefabs.TransportDepotData data =
                    SystemAPI.GetComponentRO<
                        Game.Prefabs.TransportDepotData>(
                        prefab).ValueRO;

                if (data.m_TransportType !=
                    Game.Prefabs.TransportType.Taxi)
                {
                    continue;
                }

                s_StatusTaxiDepotsTotal++;

                if (SystemAPI.HasComponent<
                        Game.Objects.OutsideConnection>(depotEntity))
                {
                    s_StatusTaxiDepotsOutside++;
                    continue;
                }

                s_StatusTaxiDepotsLocal++;

                if ((depot.ValueRO.m_Flags &
                     Game.Buildings.TransportDepotFlags.HasDispatchCenter) != 0)
                {
                    s_StatusTaxiDepotsWithDispatchCenter++;
                }
            }
        }

    }
}
