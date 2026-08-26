// <copyright file="TaxiTrafficSystem.Status.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Status.cs
// Status snapshots and InfoView-matching passenger statistics.

namespace TaxiTraffic
{
    using System;               // DateTime, Math
    using System.Globalization; // CultureInfo
    using Game;                 // GameMode extensions
    using Game.Citizens;        // HouseholdMember, Household
    using Game.Common;          // Deleted
    using Game.Creatures;       // ResidentFlags, Passenger, HumanCurrentLane
    using Game.Events;          // InvolvedInAccident
    using Game.Prefabs;         // PrefabSystem, PrefabRef
    using Game.Routes;          // TaxiStand, WaitingPassengers
    using Game.SceneFlow;       // GameManager
    // Game.Simulation types stay fully qualified here because Entities source-gen can misresolve partial-system usings.
    using Game.Tools;           // Temp
    using Game.Vehicles;        // Taxi, TaxiFlags, ParkedCar
    using Unity.Entities;       // SystemAPI, EntityQuery

    public partial class TaxiTrafficSystem
    {
        private const int kNewOptionsVisitFrameGap = 30;
        private const string kNotReadyValue = "n/a";

        private static int s_LastStatusOptionsUiFrame = -100000;
        private static bool s_HasStatusSnapshotThisOptionsVisit;
        private static bool s_StatusSnapshotDirty = true;
        private static bool s_StatusForceRefresh;
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

        internal static int s_StatusHouseholdsTotal;
        internal static int s_StatusHouseholdsCommuter;
        internal static int s_StatusHouseholdsTourist;
        internal static int s_StatusHouseholdsHomeless;
        internal static int s_StatusHouseholdsMovingInLocal;

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

        // Taxi requests.
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

        // Resident passengers currently riding FromOutside taxis.
        internal static int s_StatusOutsideTaxiResidentPassengers;
        internal static int s_StatusOutsideTaxiNotMovedInPassengers;
        internal static int s_StatusOutsideTaxiMoveInFromOcPassengers;
        internal static int s_StatusOutsideTaxiMoveInFromOcSeenTotal;
        internal static int s_StatusGroupRepairsTotal;

        // Stands and taxi supply nodes.
        internal static int s_StatusTaxiStandsTotal;
        internal static int s_StatusTaxiDepotsTotal;
        internal static int s_StatusTaxiDepotsLocal;
        internal static int s_StatusTaxiDepotsOutside;
        internal static int s_StatusTaxiDepotsWithDispatchCenter;

        // Cumulative outside supply requests removed since this city loaded.
        internal static int s_StatusOutsideSupplySuppressedTotal;

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
        private PrefabSystem? m_PrefabSystem;
        private EntityQuery m_TransportConfigQuery;
        private UITransportConfigurationPrefab? m_TransportConfig;

        private void InitStatusSystemsOnCreate()
        {
            m_CityStatisticsSystem = World.GetOrCreateSystemManaged<Game.Simulation.CityStatisticsSystem>();
            m_SimulationSystem = World.GetOrCreateSystemManaged<Game.Simulation.SimulationSystem>();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_TransportConfigQuery = GetEntityQuery(ComponentType.ReadOnly<UITransportConfigurationData>());
        }

        private void ResetStatusOnCityLoaded()
        {
            s_StatusLastSnapshotRealtime = 0.0;
            s_StatusLastSnapshotClock = kNotReadyValue;

            s_LastStatusOptionsUiFrame = -100000;
            s_HasStatusSnapshotThisOptionsVisit = false;
            s_StatusSnapshotDirty = true;
            s_StatusForceRefresh = false;
            s_WasInGame = true;
            s_StatusLastSnapshotSimulationFrame = uint.MaxValue;
            s_StatusOutsideSupplySuppressedTotal = 0;
            s_StatusOutsideTaxiMoveInFromOcSeenTotal = 0;
            s_StatusGroupRepairsTotal = 0;

            ClearSnapshotValues();
            ClearLastUpdateValues();

            try
            {
                if (m_PrefabSystem != null)
                {
                    m_TransportConfig =
                        m_PrefabSystem.GetSingletonPrefab<UITransportConfigurationPrefab>(m_TransportConfigQuery);
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

        private void BuildStatusSnapshotForOptionsUi(uint simulationFrame)
        {
            CompleteDependency();
            UpdateStatusSnapshot();

            s_StatusLastSnapshotRealtime = UnityEngine.Time.realtimeSinceStartupAsDouble;
            try
            {
                s_StatusLastSnapshotClock = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch
            {
                s_StatusLastSnapshotClock = kNotReadyValue;
            }

            s_StatusLastSnapshotSimulationFrame = simulationFrame;
            s_StatusSnapshotDirty = false;
            s_StatusForceRefresh = false;
            s_HasStatusSnapshotThisOptionsVisit = true;
        }

        private void UpdateStatusSnapshot()
        {
            ClearSnapshotValues();

            UpdateStatusMonthlyPassengers();
            UpdateStatusTaxiDepotAndStandCounts();

            foreach ((RefRO<Household> householdRef, Entity h) in SystemAPI
                         .Query<RefRO<Household>>()
                         .WithEntityAccess()
                         .WithNone<Deleted, Temp>())
            {
                s_StatusHouseholdsTotal++;

                if (SystemAPI.HasComponent<CommuterHousehold>(h))
                    s_StatusHouseholdsCommuter++;

                if (SystemAPI.HasComponent<TouristHousehold>(h))
                    s_StatusHouseholdsTourist++;

                if (SystemAPI.HasComponent<HomelessHousehold>(h))
                    s_StatusHouseholdsHomeless++;

                bool touristHousehold = SystemAPI.HasComponent<TouristHousehold>(h);
                bool commuterHousehold = SystemAPI.HasComponent<CommuterHousehold>(h);
                if (!touristHousehold && !commuterHousehold &&
                    (householdRef.ValueRO.m_Flags & HouseholdFlags.MovedIn) == 0)
                {
                    s_StatusHouseholdsMovingInLocal++;
                }
            }

            foreach ((RefRO<Game.Creatures.Resident> residentRef, Entity e) in SystemAPI
                         .Query<RefRO<Game.Creatures.Resident>>()
                         .WithEntityAccess()
                         .WithNone<Deleted, Temp>())
            {
                s_StatusResidentsTotal++;

                ResidentFlags flags = residentRef.ValueRO.m_Flags;
                bool ignoreTaxi = (flags & ResidentFlags.IgnoreTaxi) != 0;
                bool blockedMark = SystemAPI.HasComponent<IgnoreTaxiMark>(e);

                if (ignoreTaxi)
                    s_StatusResidentsIgnoreTaxi++;

                if (blockedMark)
                    s_StatusResidentsForcedMarker++;

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

                Entity citizenEntity = residentRef.ValueRO.m_Citizen;
                if (citizenEntity == Entity.Null || !SystemAPI.HasComponent<HouseholdMember>(citizenEntity))
                    continue;

                Entity household = SystemAPI.GetComponentRO<HouseholdMember>(citizenEntity).ValueRO.m_Household;
                if (household == Entity.Null)
                    continue;

                if (SystemAPI.HasComponent<CommuterHousehold>(household))
                {
                    s_StatusCommutersTotal++;
                    if (ignoreTaxi)
                        s_StatusCommutersIgnoreTaxi++;
                    if (blockedMark)
                        s_StatusCommutersBlockedMark++;
                }

                if (SystemAPI.HasComponent<TouristHousehold>(household))
                {
                    s_StatusTouristsTotal++;
                    if (ignoreTaxi)
                        s_StatusTouristsIgnoreTaxi++;
                    if (blockedMark)
                        s_StatusTouristsBlockedMark++;
                }
            }

            foreach ((RefRO<Game.Creatures.Resident> residentRef, RefRO<HumanCurrentLane> _) in SystemAPI
                         .Query<RefRO<Game.Creatures.Resident>, RefRO<HumanCurrentLane>>()
                         .WithNone<Deleted, Temp>())
            {
                if ((residentRef.ValueRO.m_Flags & ResidentFlags.WaitingTransport) != 0)
                    s_StatusWaitingTransportTotal++;
            }

            foreach (RefRO<WaitingPassengers> waiting in SystemAPI
                         .Query<RefRO<WaitingPassengers>>()
                         .WithAll<TaxiStand>()
                         .WithNone<Deleted, Temp>())
            {
                int count = waiting.ValueRO.m_Count;
                if (count > 0)
                    s_StatusWaitingTaxiStandTotal += count;
            }

            foreach ((RefRO<Game.Simulation.TaxiRequest> reqRef, Entity requestEntity) in SystemAPI
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
                                SystemAPI.GetComponentRO<Game.Simulation.ServiceRequest>(requestEntity).ValueRO;
                            reversed = (service.m_Flags & Game.Simulation.ServiceRequestFlags.Reversed) != 0;
                        }

                        if (reversed)
                        {
                            // Outside connections and active FromOutside taxis both advertise supply this way.
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

            foreach ((RefRO<Game.Vehicles.Taxi> taxiRef, Entity taxiEntity) in SystemAPI
                         .Query<RefRO<Game.Vehicles.Taxi>>()
                         .WithEntityAccess()
                         .WithNone<Deleted, Temp>())
            {
                s_StatusTaxisTotal++;

                TaxiFlags flags = taxiRef.ValueRO.m_State;

                if (SystemAPI.HasComponent<InvolvedInAccident>(taxiEntity))
                    s_StatusTaxiAccident++;
                else if (SystemAPI.HasComponent<ParkedCar>(taxiEntity))
                    s_StatusTaxiParked++;
                else if ((flags & TaxiFlags.Returning) != 0)
                    s_StatusTaxiReturning++;
                else if ((flags & TaxiFlags.Dispatched) != 0)
                    s_StatusTaxiDispatched++;
                else if ((flags & TaxiFlags.Boarding) != 0)
                    s_StatusTaxiBoarding++;
                else if ((flags & TaxiFlags.Transporting) != 0)
                    s_StatusTaxiTransporting++;
                else
                    s_StatusTaxiEnRoute++;

                bool fromOutsideTaxi = (flags & TaxiFlags.FromOutside) != 0;
                if (fromOutsideTaxi)
                    s_StatusTaxiFromOutside++;

                if ((flags & TaxiFlags.Disabled) != 0)
                    s_StatusTaxiDisabled++;

                if (SystemAPI.HasBuffer<Game.Simulation.ServiceDispatch>(taxiEntity))
                {
                    DynamicBuffer<Game.Simulation.ServiceDispatch> buf =
                        SystemAPI.GetBuffer<Game.Simulation.ServiceDispatch>(taxiEntity);

                    if (buf.IsCreated && buf.Length > 0)
                        s_StatusTaxiWithDispatchBuffer++;
                }

                if (!SystemAPI.HasBuffer<Passenger>(taxiEntity))
                    continue;

                DynamicBuffer<Passenger> passengers = SystemAPI.GetBuffer<Passenger>(taxiEntity);
                for (int i = 0; i < passengers.Length; i++)
                {
                    Entity passenger = passengers[i].m_Passenger;
                    s_StatusPassengerTotal++;

                    if (!SystemAPI.HasComponent<Game.Creatures.Resident>(passenger))
                        continue;

                    s_StatusPassengerHasResident++;

                    Game.Creatures.Resident passengerResident =
                        SystemAPI.GetComponentRO<Game.Creatures.Resident>(passenger).ValueRO;
                    ResidentFlags passengerFlags = passengerResident.m_Flags;

                    if ((passengerFlags & ResidentFlags.IgnoreTaxi) != 0)
                        s_StatusPassengerIgnoreTaxi++;

                    if (SystemAPI.HasComponent<IgnoreTaxiMark>(passenger))
                        s_StatusPassengerBlockedMark++;

                    if (fromOutsideTaxi)
                        CountOutsideTaxiResidentPassenger(passenger, passengerResident);
                }
            }
        }

        private void CountOutsideTaxiResidentPassenger(Entity passenger, Game.Creatures.Resident resident)
        {
            s_StatusOutsideTaxiResidentPassengers++;

            Entity citizen = resident.m_Citizen;
            if (citizen == Entity.Null || !SystemAPI.Exists(citizen) ||
                !SystemAPI.HasComponent<HouseholdMember>(citizen))
            {
                return;
            }

            Entity household = SystemAPI.GetComponentRO<HouseholdMember>(citizen).ValueRO.m_Household;
            if (household == Entity.Null || !SystemAPI.Exists(household) ||
                !SystemAPI.HasComponent<Household>(household))
            {
                return;
            }

            // Tourist and commuter households are not city move-ins.
            if (SystemAPI.HasComponent<TouristHousehold>(household) ||
                SystemAPI.HasComponent<CommuterHousehold>(household))
            {
                return;
            }

            Household householdData = SystemAPI.GetComponentRO<Household>(household).ValueRO;
            if ((householdData.m_Flags & HouseholdFlags.MovedIn) != 0)
                return;

            s_StatusOutsideTaxiNotMovedInPassengers++;

            // Strong Brucey-style signal: an unmoved-in local household member whose active trip
            // started at an outside connection while riding a FromOutside taxi.
            if (IsLocalMoveInFromOutsideConnection(passenger, resident))
                s_StatusOutsideTaxiMoveInFromOcPassengers++;
        }

        private void CountRequestSeekerResident(
            Entity seeker,
            ref int hasResident,
            ref int ignoreTaxi,
            ref int blockedMark)
        {
            if (seeker == Entity.Null || !SystemAPI.Exists(seeker) ||
                !SystemAPI.HasComponent<Game.Creatures.Resident>(seeker))
            {
                return;
            }

            hasResident++;

            ResidentFlags flags = SystemAPI.GetComponentRO<Game.Creatures.Resident>(seeker).ValueRO.m_Flags;
            if ((flags & ResidentFlags.IgnoreTaxi) != 0)
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
                        m_PrefabSystem.GetSingletonPrefab<UITransportConfigurationPrefab>(m_TransportConfigQuery);
                }
                catch
                {
                    m_TransportConfig = null;
                }
            }

            if (m_TransportConfig == null)
                return;

            UITransportSummaryItem[] items = m_TransportConfig.m_PassengerSummaryItems;
            for (int i = 0; i < items.Length; i++)
            {
                UITransportSummaryItem item = items[i];

                int citizen;
                int tourist;

                try
                {
                    citizen = m_CityStatisticsSystem.GetStatisticValue(item.m_Statistic);
                    tourist = m_CityStatisticsSystem.GetStatisticValue(item.m_Statistic, 1);
                }
                catch
                {
                    continue;
                }

                s_InfoTotalCitizen += citizen;
                s_InfoTotalTourist += tourist;

                switch (item.m_Type)
                {
                    case TransportType.Taxi: s_InfoTaxiCitizen = citizen; s_InfoTaxiTourist = tourist; break;
                    case TransportType.Bus: s_InfoBusCitizen = citizen; s_InfoBusTourist = tourist; break;
                    case TransportType.Tram: s_InfoTramCitizen = citizen; s_InfoTramTourist = tourist; break;
                    case TransportType.Train: s_InfoTrainCitizen = citizen; s_InfoTrainTourist = tourist; break;
                    case TransportType.Subway: s_InfoSubwayCitizen = citizen; s_InfoSubwayTourist = tourist; break;
                    case TransportType.Ship: s_InfoShipCitizen = citizen; s_InfoShipTourist = tourist; break;
                    case TransportType.Ferry: s_InfoFerryCitizen = citizen; s_InfoFerryTourist = tourist; break;
                    case TransportType.Airplane: s_InfoAirCitizen = citizen; s_InfoAirTourist = tourist; break;
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

            foreach (RefRO<TaxiStand> _ in SystemAPI
                         .Query<RefRO<TaxiStand>>()
                         .WithNone<Deleted, Temp>())
            {
                s_StatusTaxiStandsTotal++;
            }

            foreach ((RefRO<Game.Buildings.TransportDepot> depot,
                      RefRO<PrefabRef> prefabRef,
                      Entity depotEntity) in SystemAPI
                         .Query<RefRO<Game.Buildings.TransportDepot>, RefRO<PrefabRef>>()
                         .WithNone<Deleted, Temp, Game.Buildings.ServiceUpgrade>()
                         .WithEntityAccess())
            {
                Entity prefab = prefabRef.ValueRO.m_Prefab;
                if (prefab == Entity.Null || !SystemAPI.HasComponent<Game.Prefabs.TransportDepotData>(prefab))
                    continue;

                Game.Prefabs.TransportDepotData data =
                    SystemAPI.GetComponentRO<Game.Prefabs.TransportDepotData>(prefab).ValueRO;

                if (data.m_TransportType != Game.Prefabs.TransportType.Taxi)
                    continue;

                s_StatusTaxiDepotsTotal++;

                if (SystemAPI.HasComponent<Game.Objects.OutsideConnection>(depotEntity))
                {
                    // Vanilla represents each taxi-capable outside connection as a transport depot supply node.
                    s_StatusTaxiDepotsOutside++;
                    continue;
                }

                s_StatusTaxiDepotsLocal++;

                if ((depot.ValueRO.m_Flags & Game.Buildings.TransportDepotFlags.HasDispatchCenter) != 0)
                    s_StatusTaxiDepotsWithDispatchCenter++;
            }
        }

        private static void ClearSnapshotValues()
        {
            s_StatusHouseholdsTotal = 0;
            s_StatusHouseholdsCommuter = 0;
            s_StatusHouseholdsTourist = 0;
            s_StatusHouseholdsHomeless = 0;
            s_StatusHouseholdsMovingInLocal = 0;

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
            s_StatusOutsideTaxiResidentPassengers = 0;
            s_StatusOutsideTaxiNotMovedInPassengers = 0;
            s_StatusOutsideTaxiMoveInFromOcPassengers = 0;

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
            RefreshStatusSnapshotForOptionsUi(force: false);
        }

        internal static void RefreshStatusSnapshotForOptionsUi(bool force)
        {
            int frame = UnityEngine.Time.frameCount;
            bool newOptionsVisit =
                s_LastStatusOptionsUiFrame < 0 ||
                frame - s_LastStatusOptionsUiFrame > kNewOptionsVisitFrameGap;

            s_LastStatusOptionsUiFrame = frame;

            bool forceRefresh = force || s_StatusForceRefresh;
            if (!forceRefresh && !newOptionsVisit && s_HasStatusSnapshotThisOptionsVisit && !s_StatusSnapshotDirty)
                return;

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
                s_StatusLastSnapshotSimulationFrame = uint.MaxValue;
            }

            if (!isGame)
            {
                s_HasStatusSnapshotThisOptionsVisit = true;
                return;
            }

            try
            {
                TaxiTrafficSystem system = world.GetOrCreateSystemManaged<TaxiTrafficSystem>();
                uint simulationFrame = system.GetStatusSimulationFrame();
                bool simulationAdvanced = simulationFrame != s_StatusLastSnapshotSimulationFrame;

                bool shouldBuild =
                    forceRefresh ||
                    !HasSnapshot() ||
                    s_StatusSnapshotDirty ||
                    (newOptionsVisit && simulationAdvanced);

                if (!shouldBuild)
                {
                    s_HasStatusSnapshotThisOptionsVisit = true;
                    return;
                }

                system.BuildStatusSnapshotForOptionsUi(simulationFrame);
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

        internal static bool HasSnapshot() => s_StatusLastSnapshotRealtime > 0.0;

        internal static string GetStatusLastStampText() =>
            string.IsNullOrEmpty(s_StatusLastSnapshotClock) ? kNotReadyValue : s_StatusLastSnapshotClock;

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
            return s_StatusLastAppliedIgnoreTaxi != 0
                || s_StatusLastSkippedCommuters != 0
                || s_StatusLastSkippedTourists != 0
                || s_StatusLastSkippedGroupTravelers != 0
                || s_StatusLastClearedGroupTravelers != 0
                || s_StatusLastClearedTaxiLaneWaiting != 0
                || s_StatusLastClearedTaxiStandWaiting != 0
                || s_StatusLastRemovedRideNeeder != 0
                || s_StatusOutsideSupplySuppressedTotal != 0;
        }

        internal static string GetActivityNotReadyText() => kNotReadyValue;
    }
}
