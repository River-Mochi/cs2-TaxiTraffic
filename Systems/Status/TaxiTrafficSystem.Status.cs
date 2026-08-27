// <copyright file="TaxiTrafficSystem.Status.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Status/TaxiTrafficSystem.Status.cs
// Lightweight player Status plus deeper diagnostics only when requested.

namespace TaxiTraffic
{
    using System;               // DateTime, Math
    using System.Globalization; // CultureInfo
    using Game;                 // GameMode extensions
    using Game.Citizens;        // HouseholdMember, commuter/tourist households
    using Game.Common;          // Deleted
    using Game.Creatures;       // ResidentFlags, Passenger, HumanCurrentLane
    using Game.Events;          // InvolvedInAccident
    using Game.SceneFlow;       // GameManager
    // Game.Simulation types stay fully qualified because Entities source-gen can misresolve partial-system usings.
    using Game.Tools;           // Temp
    using Unity.Entities;       // SystemAPI, EntityQuery, DynamicBuffer

    public partial class TaxiTrafficSystem
    {
        private const int kNewOptionsVisitFrameGap = 30;
        private const string kNotReadyValue = "n/a";

        private static int s_LastStatusOptionsUiFrame = -100000;
        private static bool s_HasStatusSnapshotThisOptionsVisit;
        private static bool s_StatusSnapshotDirty = true;
        private static bool s_StatusForceRefresh;
        private static bool s_StatusSnapshotHasDetails;
        private static bool s_WasInGame;
        private static uint s_StatusLastSnapshotSimulationFrame = uint.MaxValue;

        internal static double s_StatusLastSnapshotRealtime;
        internal static string s_StatusLastSnapshotClock = kNotReadyValue;

        // Active resident creatures.
        internal static int s_StatusResidentsTotal;
        internal static int s_StatusResidentsIgnoreTaxi;
        internal static int s_StatusResidentsForcedMarker;
        internal static int s_StatusResidentsAllowedMarker;
        internal static int s_StatusResidentsGroupAllowedMarker;
        internal static int s_StatusResidentsGroupLinked;
        internal static int s_StatusResidentsGroupLinkedIgnoreTaxi;

        internal static int s_StatusCommutersTotal;
        internal static int s_StatusCommutersIgnoreTaxi;
        internal static int s_StatusCommutersBlockedMark;

        internal static int s_StatusTouristsTotal;
        internal static int s_StatusTouristsIgnoreTaxi;
        internal static int s_StatusTouristsBlockedMark;

        internal static int s_StatusWaitingTransportTotal;
        internal static int s_StatusWaitingTaxiStandTotal;

        // InfoView monthly passengers.
        internal static int s_InfoTaxiTourist;
        internal static int s_InfoTaxiCitizen;
        internal static int s_InfoBusTourist;
        internal static int s_InfoBusCitizen;
        internal static int s_InfoTramTourist;
        internal static int s_InfoTramCitizen;
        internal static int s_InfoTrainTourist;
        internal static int s_InfoTrainCitizen;
        internal static int s_InfoSubwayTourist;
        internal static int s_InfoSubwayCitizen;
        internal static int s_InfoShipTourist;
        internal static int s_InfoFerryTourist;
        internal static int s_InfoAirTourist;
        internal static int s_InfoShipCitizen;
        internal static int s_InfoFerryCitizen;
        internal static int s_InfoAirCitizen;
        internal static int s_InfoTotalTourist;
        internal static int s_InfoTotalCitizen;

        // Taxi requests. Detailed report / DEBUG only.
        internal static int s_StatusReqStand;
        internal static int s_StatusReqCustomer;
        internal static int s_StatusReqOutside;
        internal static int s_StatusReqOutsideRider;
        internal static int s_StatusReqOutsideSupply;
        internal static int s_StatusReqNone;

        internal static int s_StatusReqCustomerSeekerHasResident;
        internal static int s_StatusReqCustomerSeekerIgnoreTaxi;
        internal static int s_StatusReqCustomerSeekerBlockedMark;
        internal static int s_StatusReqOutsideSeekerHasResident;
        internal static int s_StatusReqOutsideSeekerIgnoreTaxi;
        internal static int s_StatusReqOutsideSeekerBlockedMark;

        // Taxi fleet.
        internal static int s_StatusTaxisTotal;
        internal static int s_StatusTaxiTransporting;
        internal static int s_StatusTaxiBoarding;
        internal static int s_StatusTaxiReturning;
        internal static int s_StatusTaxiDispatched;
        internal static int s_StatusTaxiEnRoute;
        internal static int s_StatusTaxiParked;
        internal static int s_StatusTaxiAccident;
        internal static int s_StatusTaxiFromOutside;
        internal static int s_StatusTaxiDisabled;
        internal static int s_StatusTaxiWithDispatchBuffer;

        // Passengers in taxis.
        internal static int s_StatusPassengerTotal;
        internal static int s_StatusPassengerHasResident;
        internal static int s_StatusPassengerIgnoreTaxi;
        internal static int s_StatusPassengerBlockedMark;

        // Cumulative behavior counters.
        internal static int s_StatusOutsideTaxiBlockedTotal;
        internal static int s_StatusGroupRepairsTotal;

        // Stands and taxi supply nodes.
        internal static int s_StatusTaxiStandsTotal;
        internal static int s_StatusTaxiDepotsTotal;
        internal static int s_StatusTaxiDepotsLocal;
        internal static int s_StatusTaxiDepotsOutside;
        internal static int s_StatusTaxiDepotsWithDispatchCenter;

        // Last-update counters written by Core.
        internal static int s_StatusLastAppliedIgnoreTaxi;
        internal static int s_StatusLastSkippedCommuters;
        internal static int s_StatusLastSkippedTourists;
        internal static int s_StatusLastSkippedGroupTravelers;
        internal static int s_StatusLastClearedGroupTravelers;
        internal static int s_StatusLastClearedTaxiLaneWaiting;
        internal static int s_StatusLastClearedTaxiStandWaiting;
        internal static int s_StatusLastRemovedRideNeeder;

        private Game.Simulation.CityStatisticsSystem? m_CityStatisticsSystem;
        private Game.Simulation.SimulationSystem? m_SimulationSystem;
        private Game.Prefabs.PrefabSystem? m_PrefabSystem;
        private EntityQuery m_TransportConfigQuery;
        private Game.Prefabs.UITransportConfigurationPrefab? m_TransportConfig;

        private void InitStatusSystemsOnCreate()
        {
            m_CityStatisticsSystem =
                World.GetOrCreateSystemManaged<Game.Simulation.CityStatisticsSystem>();

            m_SimulationSystem =
                World.GetOrCreateSystemManaged<Game.Simulation.SimulationSystem>();

            m_PrefabSystem = World.GetOrCreateSystemManaged<Game.Prefabs.PrefabSystem>();
            m_TransportConfigQuery =
                GetEntityQuery(ComponentType.ReadOnly<Game.Prefabs.UITransportConfigurationData>());
        }

        private void ResetStatusOnCityLoaded()
        {
            s_StatusLastSnapshotRealtime = 0.0;
            s_StatusLastSnapshotClock = kNotReadyValue;

            s_LastStatusOptionsUiFrame = -100000;
            s_HasStatusSnapshotThisOptionsVisit = false;
            s_StatusSnapshotDirty = true;
            s_StatusForceRefresh = false;
            s_StatusSnapshotHasDetails = false;
            s_WasInGame = true;
            s_StatusLastSnapshotSimulationFrame = uint.MaxValue;

            s_StatusOutsideTaxiBlockedTotal = 0;
            s_StatusGroupRepairsTotal = 0;

            ClearSnapshotValues();
            ClearLastUpdateValues();

            try
            {
                if (m_PrefabSystem != null)
                {
                    m_TransportConfig =
                        m_PrefabSystem.GetSingletonPrefab<Game.Prefabs.UITransportConfigurationPrefab>(
                            m_TransportConfigQuery);
                }
            }
            catch
            {
                m_TransportConfig = null;
            }
        }

        private uint GetStatusSimulationFrame()
        {
            return m_SimulationSystem?.frameIndex ?? uint.MaxValue;
        }

        private void BuildStatusSnapshotForOptionsUi(
            uint simulationFrame,
            bool detailed)
        {
            CompleteDependency();
            UpdateStatusSnapshot(detailed);

            s_StatusLastSnapshotRealtime =
                UnityEngine.Time.realtimeSinceStartupAsDouble;

            try
            {
                s_StatusLastSnapshotClock =
                    DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                s_StatusLastSnapshotClock = kNotReadyValue;
            }

            s_StatusLastSnapshotSimulationFrame = simulationFrame;
            s_StatusSnapshotDirty = false;
            s_StatusForceRefresh = false;
            s_StatusSnapshotHasDetails = detailed;
            s_HasStatusSnapshotThisOptionsVisit = true;
        }

        private void UpdateStatusSnapshot(bool detailed)
        {
            ClearSnapshotValues();

            UpdateStatusMonthlyPassengers();
            UpdateStatusTaxiDepotAndStandCounts();

            foreach ((RefRO<Game.Creatures.Resident> residentRef, Entity e) in SystemAPI
                         .Query<RefRO<Game.Creatures.Resident>>()
                         .WithEntityAccess()
                         .WithNone<Deleted, Temp>())
            {
                s_StatusResidentsTotal++;

                Game.Creatures.ResidentFlags flags = residentRef.ValueRO.m_Flags;
                bool ignoreTaxi = (flags & Game.Creatures.ResidentFlags.IgnoreTaxi) != 0;
                bool blockedMark = SystemAPI.HasComponent<IgnoreTaxiMark>(e);

                if (blockedMark)
                    s_StatusResidentsForcedMarker++;

                if (detailed)
                {
                    if (ignoreTaxi)
                        s_StatusResidentsIgnoreTaxi++;

                    if (SystemAPI.HasComponent<TaxiAllowedMark>(e))
                        s_StatusResidentsAllowedMarker++;

                    if (SystemAPI.HasComponent<GroupTaxiAllowedMark>(e))
                        s_StatusResidentsGroupAllowedMarker++;

                    bool groupLinked = IsGroupLinkedTraveler(e);
                    if (groupLinked)
                    {
                        s_StatusResidentsGroupLinked++;

                        if (ignoreTaxi)
                            s_StatusResidentsGroupLinkedIgnoreTaxi++;
                    }
                }

                Entity citizenEntity = residentRef.ValueRO.m_Citizen;
                if (citizenEntity == Entity.Null ||
                    !SystemAPI.HasComponent<Game.Citizens.HouseholdMember>(citizenEntity))
                {
                    continue;
                }

                Entity household =
                    SystemAPI.GetComponentRO<Game.Citizens.HouseholdMember>(citizenEntity).ValueRO.m_Household;

                if (household == Entity.Null)
                    continue;

                if (SystemAPI.HasComponent<Game.Citizens.CommuterHousehold>(household))
                {
                    s_StatusCommutersTotal++;

                    if (blockedMark)
                        s_StatusCommutersBlockedMark++;

                    if (detailed && ignoreTaxi)
                        s_StatusCommutersIgnoreTaxi++;
                }

                if (SystemAPI.HasComponent<Game.Citizens.TouristHousehold>(household))
                {
                    s_StatusTouristsTotal++;

                    if (blockedMark)
                        s_StatusTouristsBlockedMark++;

                    if (detailed && ignoreTaxi)
                        s_StatusTouristsIgnoreTaxi++;
                }
            }

            // Player-facing total uses all cims waiting for public transport.
            foreach ((RefRO<Game.Creatures.Resident> residentRef,
                      RefRO<Game.Creatures.HumanCurrentLane> _) in SystemAPI
                         .Query<RefRO<Game.Creatures.Resident>, RefRO<Game.Creatures.HumanCurrentLane>>()
                         .WithNone<Deleted, Temp>())
            {
                if ((residentRef.ValueRO.m_Flags & ResidentFlags.WaitingTransport) != 0)
                    s_StatusWaitingTransportTotal++;
            }

            if (detailed)
            {
                UpdateDetailedStatusRequests();
                UpdateDetailedTaxiStandWaiting();
            }

            UpdateStatusTaxiFleetAndPassengers(detailed);
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
                        if (SystemAPI.HasComponent<Game.Simulation.ServiceRequest>(requestEntity))
                        {
                            Game.Simulation.ServiceRequest service =
                                SystemAPI.GetComponentRO<Game.Simulation.ServiceRequest>(
                                    requestEntity).ValueRO;

                            reversed =
                                (service.m_Flags &
                                 Game.Simulation.ServiceRequestFlags.Reversed) != 0;
                        }

                        if (reversed)
                        {
                            // OC sources and active FromOutside taxis advertise supply this way.
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
            foreach ((RefRO<Game.Vehicles.Taxi> taxiRef, Entity taxiEntity) in SystemAPI
                         .Query<RefRO<Game.Vehicles.Taxi>>()
                         .WithEntityAccess()
                         .WithNone<Deleted, Temp>())
            {
                s_StatusTaxisTotal++;

                Game.Vehicles.TaxiFlags flags = taxiRef.ValueRO.m_State;
                bool fromOutsideTaxi = (flags & Game.Vehicles.TaxiFlags.FromOutside) != 0;

                if (fromOutsideTaxi)
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

                    if (SystemAPI.HasBuffer<Game.Simulation.ServiceDispatch>(taxiEntity))
                    {
                        DynamicBuffer<Game.Simulation.ServiceDispatch> dispatches =
                            SystemAPI.GetBuffer<Game.Simulation.ServiceDispatch>(taxiEntity);

                        if (dispatches.IsCreated && dispatches.Length > 0)
                            s_StatusTaxiWithDispatchBuffer++;
                    }
                }

                if (!SystemAPI.HasBuffer<Game.Creatures.Passenger>(taxiEntity))
                    continue;

                DynamicBuffer<Game.Creatures.Passenger> passengers =
                    SystemAPI.GetBuffer<Game.Creatures.Passenger>(taxiEntity);

                for (int i = 0; i < passengers.Length; i++)
                {
                    Entity passenger = passengers[i].m_Passenger;
                    s_StatusPassengerTotal++;

                    if (!SystemAPI.HasComponent<Game.Creatures.Resident>(passenger))
                        continue;

                    s_StatusPassengerHasResident++;

                    if (SystemAPI.HasComponent<IgnoreTaxiMark>(passenger))
                        s_StatusPassengerBlockedMark++;

                    if (!detailed)
                        continue;

                    ResidentFlags passengerFlags =
                        SystemAPI.GetComponentRO<Game.Creatures.Resident>(
                            passenger).ValueRO.m_Flags;

                    if ((passengerFlags & Game.Creatures.ResidentFlags.IgnoreTaxi) != 0)
                        s_StatusPassengerIgnoreTaxi++;
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
                !SystemAPI.HasComponent<Game.Creatures.Resident>(seeker))
            {
                return;
            }

            hasResident++;

            ResidentFlags flags =
                SystemAPI.GetComponentRO<Game.Creatures.Resident>(seeker).ValueRO.m_Flags;

            if ((flags & Game.Creatures.ResidentFlags.IgnoreTaxi) != 0)
                ignoreTaxi++;

            if (SystemAPI.HasComponent<IgnoreTaxiMark>(seeker))
                blockedMark++;
        }

        private void UpdateStatusMonthlyPassengers()
        {
            if (m_CityStatisticsSystem == null)
                return;

            s_InfoTaxiTourist = 0; s_InfoTaxiCitizen = 0;
            s_InfoBusTourist = 0; s_InfoBusCitizen = 0;
            s_InfoTramTourist = 0; s_InfoTramCitizen = 0;
            s_InfoTrainTourist = 0; s_InfoTrainCitizen = 0;
            s_InfoSubwayTourist = 0; s_InfoSubwayCitizen = 0;
            s_InfoShipTourist = 0; s_InfoShipCitizen = 0;
            s_InfoFerryTourist = 0; s_InfoFerryCitizen = 0;
            s_InfoAirTourist = 0; s_InfoAirCitizen = 0;
            s_InfoTotalTourist = 0; s_InfoTotalCitizen = 0;

            if (m_TransportConfig == null && m_PrefabSystem != null)
            {
                try
                {
                    m_TransportConfig =
                        m_PrefabSystem.GetSingletonPrefab<Game.Prefabs.UITransportConfigurationPrefab>(
                            m_TransportConfigQuery);
                }
                catch
                {
                    m_TransportConfig = null;
                }
            }

            if (m_TransportConfig == null)
                return;

            Game.Prefabs.UITransportSummaryItem[] items = m_TransportConfig.m_PassengerSummaryItems;
            for (int i = 0; i < items.Length; i++)
            {
                Game.Prefabs.UITransportSummaryItem item = items[i];

                int citizen;
                int tourist;

                try
                {
                    citizen =
                        m_CityStatisticsSystem.GetStatisticValue(item.m_Statistic);

                    tourist =
                        m_CityStatisticsSystem.GetStatisticValue(item.m_Statistic, 1);
                }
                catch
                {
                    continue;
                }

                s_InfoTotalCitizen += citizen;
                s_InfoTotalTourist += tourist;

                switch (item.m_Type)
                {
                    case Game.Prefabs.TransportType.Taxi:
                        s_InfoTaxiCitizen = citizen;
                        s_InfoTaxiTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Bus:
                        s_InfoBusCitizen = citizen;
                        s_InfoBusTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Tram:
                        s_InfoTramCitizen = citizen;
                        s_InfoTramTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Train:
                        s_InfoTrainCitizen = citizen;
                        s_InfoTrainTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Subway:
                        s_InfoSubwayCitizen = citizen;
                        s_InfoSubwayTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Ship:
                        s_InfoShipCitizen = citizen;
                        s_InfoShipTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Ferry:
                        s_InfoFerryCitizen = citizen;
                        s_InfoFerryTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Airplane:
                        s_InfoAirCitizen = citizen;
                        s_InfoAirTourist = tourist;
                        break;
                }
            }
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
                         .Query<RefRO<Game.Buildings.TransportDepot>, RefRO<Game.Prefabs.PrefabRef>>()
                         .WithNone<Deleted, Temp, Game.Buildings.ServiceUpgrade>()
                         .WithEntityAccess())
            {
                Entity prefab = prefabRef.ValueRO.m_Prefab;
                if (prefab == Entity.Null ||
                    !SystemAPI.HasComponent<Game.Prefabs.TransportDepotData>(prefab))
                {
                    continue;
                }

                Game.Prefabs.TransportDepotData data =
                    SystemAPI.GetComponentRO<Game.Prefabs.TransportDepotData>(prefab).ValueRO;

                if (data.m_TransportType != Game.Prefabs.TransportType.Taxi)
                    continue;

                s_StatusTaxiDepotsTotal++;

                if (SystemAPI.HasComponent<Game.Objects.OutsideConnection>(depotEntity))
                {
                    // Vanilla uses a taxi-capable OC as a taxi supply source.
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

        private static void ClearSnapshotValues()
        {
            s_StatusResidentsTotal = 0;
            s_StatusResidentsIgnoreTaxi = 0;
            s_StatusResidentsForcedMarker = 0;
            s_StatusResidentsAllowedMarker = 0;
            s_StatusResidentsGroupAllowedMarker = 0;
            s_StatusResidentsGroupLinked = 0;
            s_StatusResidentsGroupLinkedIgnoreTaxi = 0;

            s_StatusCommutersTotal = 0;
            s_StatusCommutersIgnoreTaxi = 0;
            s_StatusCommutersBlockedMark = 0;

            s_StatusTouristsTotal = 0;
            s_StatusTouristsIgnoreTaxi = 0;
            s_StatusTouristsBlockedMark = 0;

            s_StatusWaitingTransportTotal = 0;
            s_StatusWaitingTaxiStandTotal = 0;

            s_StatusReqStand = 0;
            s_StatusReqCustomer = 0;
            s_StatusReqOutside = 0;
            s_StatusReqOutsideRider = 0;
            s_StatusReqOutsideSupply = 0;
            s_StatusReqNone = 0;

            s_StatusReqCustomerSeekerHasResident = 0;
            s_StatusReqCustomerSeekerIgnoreTaxi = 0;
            s_StatusReqCustomerSeekerBlockedMark = 0;
            s_StatusReqOutsideSeekerHasResident = 0;
            s_StatusReqOutsideSeekerIgnoreTaxi = 0;
            s_StatusReqOutsideSeekerBlockedMark = 0;

            s_StatusTaxisTotal = 0;
            s_StatusTaxiTransporting = 0;
            s_StatusTaxiBoarding = 0;
            s_StatusTaxiReturning = 0;
            s_StatusTaxiDispatched = 0;
            s_StatusTaxiEnRoute = 0;
            s_StatusTaxiParked = 0;
            s_StatusTaxiAccident = 0;
            s_StatusTaxiFromOutside = 0;
            s_StatusTaxiDisabled = 0;
            s_StatusTaxiWithDispatchBuffer = 0;

            s_StatusPassengerTotal = 0;
            s_StatusPassengerHasResident = 0;
            s_StatusPassengerIgnoreTaxi = 0;
            s_StatusPassengerBlockedMark = 0;

            s_StatusTaxiStandsTotal = 0;
            s_StatusTaxiDepotsTotal = 0;
            s_StatusTaxiDepotsLocal = 0;
            s_StatusTaxiDepotsOutside = 0;
            s_StatusTaxiDepotsWithDispatchCenter = 0;

            s_InfoTaxiTourist = 0; s_InfoTaxiCitizen = 0;
            s_InfoBusTourist = 0; s_InfoBusCitizen = 0;
            s_InfoTramTourist = 0; s_InfoTramCitizen = 0;
            s_InfoTrainTourist = 0; s_InfoTrainCitizen = 0;
            s_InfoSubwayTourist = 0; s_InfoSubwayCitizen = 0;
            s_InfoShipTourist = 0; s_InfoShipCitizen = 0;
            s_InfoFerryTourist = 0; s_InfoFerryCitizen = 0;
            s_InfoAirTourist = 0; s_InfoAirCitizen = 0;
            s_InfoTotalTourist = 0; s_InfoTotalCitizen = 0;
        }

        private static void ClearLastUpdateValues()
        {
            s_StatusLastAppliedIgnoreTaxi = 0;
            s_StatusLastSkippedCommuters = 0;
            s_StatusLastSkippedTourists = 0;
            s_StatusLastSkippedGroupTravelers = 0;
            s_StatusLastClearedGroupTravelers = 0;
            s_StatusLastClearedTaxiLaneWaiting = 0;
            s_StatusLastClearedTaxiStandWaiting = 0;
            s_StatusLastRemovedRideNeeder = 0;
        }

        internal static void AutoRequestStatusRefreshOnRead()
        {
            RefreshStatusSnapshot(force: false, detailed: false);
        }

        internal static void RefreshStatusSnapshotForOptionsUi(bool force)
        {
            RefreshStatusSnapshot(force, detailed: false);
        }

        internal static void RefreshStatusSnapshotForReport()
        {
            RefreshStatusSnapshot(force: true, detailed: true);
        }

#if DEBUG
        internal static void EnsureDetailedStatusSnapshot()
        {
            RefreshStatusSnapshot(force: false, detailed: true);
        }

        internal static void RefreshStatusSnapshotForDebug()
        {
            RefreshStatusSnapshot(force: true, detailed: true);
        }
#endif

        private static void RefreshStatusSnapshot(bool force, bool detailed)
        {
            int frame = UnityEngine.Time.frameCount;
            bool newOptionsVisit =
                s_LastStatusOptionsUiFrame < 0 ||
                frame - s_LastStatusOptionsUiFrame > kNewOptionsVisitFrameGap;

            s_LastStatusOptionsUiFrame = frame;

            bool forceRefresh = force || s_StatusForceRefresh;
            bool needsDetails = detailed && !s_StatusSnapshotHasDetails;

            if (!forceRefresh &&
                !needsDetails &&
                !newOptionsVisit &&
                s_HasStatusSnapshotThisOptionsVisit &&
                !s_StatusSnapshotDirty)
            {
                return;
            }

            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
                return;

            GameManager gm = GameManager.instance;
            bool isGame = gm != null && gm.gameMode.IsGame();

            if (isGame != s_WasInGame)
            {
                s_WasInGame = isGame;
                s_HasStatusSnapshotThisOptionsVisit = false;
                s_StatusSnapshotDirty = true;
                s_StatusSnapshotHasDetails = false;
                s_StatusLastSnapshotSimulationFrame = uint.MaxValue;
            }

            if (!isGame)
            {
                s_HasStatusSnapshotThisOptionsVisit = true;
                return;
            }

            try
            {
                TaxiTrafficSystem system =
                    world.GetOrCreateSystemManaged<TaxiTrafficSystem>();

                uint simulationFrame = system.GetStatusSimulationFrame();
                bool simulationAdvanced =
                    simulationFrame != s_StatusLastSnapshotSimulationFrame;

                bool shouldBuild =
                    forceRefresh ||
                    needsDetails ||
                    !HasSnapshot() ||
                    s_StatusSnapshotDirty ||
                    (newOptionsVisit && simulationAdvanced);

                if (!shouldBuild)
                {
                    s_HasStatusSnapshotThisOptionsVisit = true;
                    return;
                }

                system.BuildStatusSnapshotForOptionsUi(simulationFrame, detailed);
            }
            catch (Exception)
            {
                s_StatusSnapshotDirty = true;
                s_StatusForceRefresh = false;
            }
        }

        internal static void RequestStatusRefresh(bool force)
        {
            s_StatusSnapshotDirty = true;

            if (force)
                s_StatusForceRefresh = true;
        }

        internal static bool HasSnapshot() =>
            s_StatusLastSnapshotRealtime > 0.0;

        internal static string GetStatusLastStampText() =>
            string.IsNullOrEmpty(s_StatusLastSnapshotClock)
                ? kNotReadyValue
                : s_StatusLastSnapshotClock;

        internal static bool IsStatusSnapshotStale(double maxAgeSeconds)
        {
            double age = GetStatusAgeSeconds();
            return age < 0.0 || age > maxAgeSeconds;
        }

        internal static double GetStatusAgeSeconds()
        {
            if (s_StatusLastSnapshotRealtime <= 0.0)
                return -1.0;

            double now = UnityEngine.Time.realtimeSinceStartupAsDouble;
            return Math.Max(0.0, now - s_StatusLastSnapshotRealtime);
        }

        internal static string GetCityScanNotReadyText() => kNotReadyValue;
        internal static string GetTaxiScanNotReadyText() => kNotReadyValue;

        internal static bool HasActivity()
        {
            return s_StatusLastAppliedIgnoreTaxi != 0 ||
                   s_StatusLastSkippedCommuters != 0 ||
                   s_StatusLastSkippedTourists != 0 ||
                   s_StatusLastSkippedGroupTravelers != 0 ||
                   s_StatusLastClearedGroupTravelers != 0 ||
                   s_StatusLastClearedTaxiLaneWaiting != 0 ||
                   s_StatusLastClearedTaxiStandWaiting != 0 ||
                   s_StatusLastRemovedRideNeeder != 0 ||
                   s_StatusOutsideTaxiBlockedTotal != 0;
        }

        internal static string GetActivityNotReadyText() => kNotReadyValue;
    }
}
