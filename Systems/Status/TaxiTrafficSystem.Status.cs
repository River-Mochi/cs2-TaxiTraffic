// <copyright file="TaxiTrafficSystem.Status.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Status/TaxiTrafficSystem.Status.cs
// Cached Status state, snapshot lifecycle, and refresh helpers.

using System;
using System.Globalization;
using Game;
using Game.SceneFlow;
using Unity.Entities;

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem
    {
        private const string kNotReadyValue = "n/a";

        private static bool s_HasStatusSnapshot;
        private static bool s_StatusSnapshotDirty = true;
        private static bool s_StatusForceRefresh;
        private static bool s_StatusSnapshotHasDetails;
        private static bool s_WasInGame;
        private static uint s_StatusLastSnapshotSimulationFrame = uint.MaxValue;

        internal static double s_StatusLastSnapshotRealtime;
        internal static string s_StatusLastSnapshotClock = kNotReadyValue;

        // Active physical cim agents; not the city's total population.
        internal static int s_StatusActiveCimsTotal;
        internal static int s_StatusLocalCimsTotal;
        internal static int s_StatusLocalBlockedMark;
        internal static int s_StatusCommutersTotal;
        internal static int s_StatusCommutersBlockedMark;
        internal static int s_StatusTouristsTotal;
        internal static int s_StatusTouristsBlockedMark;
        internal static int s_StatusOwnedBlocksTotal;
        internal static int s_StatusResidentsIgnoreTaxi;

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

        // Current city-rider request purpose. Kept light for the player Status row, more detailed in report.
        internal static int s_StatusReqPurposeLeisure;
        internal static int s_StatusReqPurposeHome;
        internal static int s_StatusReqPurposeWork;
        internal static int s_StatusReqPurposeSchool;
        internal static int s_StatusReqPurposeShopping;
        internal static int s_StatusReqPurposeOther;

        // Taxi fleet.
        internal static int s_StatusTaxisTotal;
        internal static int s_StatusTaxiParkedNow;
        internal static int s_StatusTaxiActiveNow;
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
        internal static int s_StatusPassengerLocal;
        internal static int s_StatusPassengerIgnoreTaxi;
        internal static int s_StatusPassengerBlockedMark;

        // Stands and taxi supply nodes.
        internal static int s_StatusTaxiStandsTotal;
        internal static int s_StatusTaxiDepotsTotal;
        internal static int s_StatusTaxiDepotsLocal;
        internal static int s_StatusTaxiDepotsOutside;
        internal static int s_StatusTaxiDepotsWithDispatchCenter;

        // Last control pass.
        internal static int s_StatusLastAppliedIgnoreTaxi;
        internal static int s_StatusLastRemovedIgnoreTaxi;
        internal static int s_StatusLastReappliedIgnoreTaxi;
        internal static int s_StatusLastRideNeedersStopped;
        internal static int s_StatusLastTaxiRequestsStopped;
        internal static int s_StatusLastTaxiWaitersRepathed;

        // Cheap cumulative counters since city load.
        internal static int s_StatusRideNeedersStoppedTotal;
        internal static int s_StatusTaxiRequestsStoppedTotal;
        internal static int s_StatusTaxiWaitersRepathedTotal;

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

            m_PrefabSystem =
                World.GetOrCreateSystemManaged<Game.Prefabs.PrefabSystem>();

            m_TransportConfigQuery =
                GetEntityQuery(
                    ComponentType.ReadOnly<Game.Prefabs.UITransportConfigurationData>());
        }

        private void ResetStatusOnCityLoaded()
        {
            s_StatusLastSnapshotRealtime = 0.0;
            s_StatusLastSnapshotClock = kNotReadyValue;

            s_HasStatusSnapshot = false;
            s_StatusSnapshotDirty = true;
            s_StatusForceRefresh = false;
            s_StatusSnapshotHasDetails = false;
            s_WasInGame = true;
            s_StatusLastSnapshotSimulationFrame = uint.MaxValue;

            ClearSnapshotValues();
            ClearLastUpdateValues();

            s_StatusRideNeedersStoppedTotal = 0;
            s_StatusTaxiRequestsStoppedTotal = 0;
            s_StatusTaxiWaitersRepathedTotal = 0;

            try
            {
                if (m_PrefabSystem != null)
                {
                    m_TransportConfig =
                        m_PrefabSystem.GetSingletonPrefab<
                            Game.Prefabs.UITransportConfigurationPrefab>(
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

        private void BuildStatusSnapshot(
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
                    DateTime.Now.ToString(
                        "HH:mm:ss",
                        CultureInfo.InvariantCulture);
            }
            catch
            {
                s_StatusLastSnapshotClock = kNotReadyValue;
            }

            s_StatusLastSnapshotSimulationFrame = simulationFrame;
            s_StatusSnapshotDirty = false;
            s_StatusForceRefresh = false;
            s_StatusSnapshotHasDetails = detailed;
            s_HasStatusSnapshot = true;
        }


        private static void ClearSnapshotValues()
        {
            s_StatusActiveCimsTotal = 0;
            s_StatusLocalCimsTotal = 0;
            s_StatusLocalBlockedMark = 0;
            s_StatusCommutersTotal = 0;
            s_StatusCommutersBlockedMark = 0;
            s_StatusTouristsTotal = 0;
            s_StatusTouristsBlockedMark = 0;
            s_StatusOwnedBlocksTotal = 0;
            s_StatusResidentsIgnoreTaxi = 0;

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

            s_StatusReqPurposeLeisure = 0;
            s_StatusReqPurposeHome = 0;
            s_StatusReqPurposeWork = 0;
            s_StatusReqPurposeSchool = 0;
            s_StatusReqPurposeShopping = 0;
            s_StatusReqPurposeOther = 0;

            s_StatusTaxisTotal = 0;
            s_StatusTaxiParkedNow = 0;
            s_StatusTaxiActiveNow = 0;
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
            s_StatusPassengerLocal = 0;
            s_StatusPassengerIgnoreTaxi = 0;
            s_StatusPassengerBlockedMark = 0;

            s_StatusTaxiStandsTotal = 0;
            s_StatusTaxiDepotsTotal = 0;
            s_StatusTaxiDepotsLocal = 0;
            s_StatusTaxiDepotsOutside = 0;
            s_StatusTaxiDepotsWithDispatchCenter = 0;

            s_InfoTaxiTourist = 0;
            s_InfoTaxiCitizen = 0;
            s_InfoBusTourist = 0;
            s_InfoBusCitizen = 0;
            s_InfoTramTourist = 0;
            s_InfoTramCitizen = 0;
            s_InfoTrainTourist = 0;
            s_InfoTrainCitizen = 0;
            s_InfoSubwayTourist = 0;
            s_InfoSubwayCitizen = 0;
            s_InfoShipTourist = 0;
            s_InfoShipCitizen = 0;
            s_InfoFerryTourist = 0;
            s_InfoFerryCitizen = 0;
            s_InfoAirTourist = 0;
            s_InfoAirCitizen = 0;
            s_InfoTotalTourist = 0;
            s_InfoTotalCitizen = 0;
        }

        private static void ClearLastUpdateValues()
        {
            s_StatusLastAppliedIgnoreTaxi = 0;
            s_StatusLastRemovedIgnoreTaxi = 0;
            s_StatusLastReappliedIgnoreTaxi = 0;
            s_StatusLastRideNeedersStopped = 0;
            s_StatusLastTaxiRequestsStopped = 0;
            s_StatusLastTaxiWaitersRepathed = 0;
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

        private static void RefreshStatusSnapshot(
            bool force,
            bool detailed)
        {
            World world = World.DefaultGameObjectInjectionWorld;

            if (world == null || !world.IsCreated)
                return;

            GameManager gm = GameManager.instance;
            bool isGame = gm != null && gm.gameMode.IsGame();

            if (isGame != s_WasInGame)
            {
                s_WasInGame = isGame;
                s_HasStatusSnapshot = false;
                s_StatusSnapshotDirty = true;
                s_StatusSnapshotHasDetails = false;
                s_StatusLastSnapshotSimulationFrame = uint.MaxValue;
            }

            if (!isGame)
                return;

            try
            {
                TaxiTrafficSystem system =
                    world.GetOrCreateSystemManaged<TaxiTrafficSystem>();

                uint simulationFrame =
                    system.GetStatusSimulationFrame();

                bool forceRefresh =
                    force || s_StatusForceRefresh;

                bool needsDetails =
                    detailed && !s_StatusSnapshotHasDetails;

                // Options pauses the city. If the simulation frame has not
                // changed, every Status row reuses the same cached snapshot.
                if (!forceRefresh &&
                    !s_StatusSnapshotDirty &&
                    !needsDetails &&
                    s_HasStatusSnapshot &&
                    s_StatusLastSnapshotSimulationFrame == simulationFrame)
                {
                    return;
                }

                system.BuildStatusSnapshot(
                    simulationFrame,
                    detailed);
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

        internal static bool HasSnapshot()
        {
            return s_StatusLastSnapshotRealtime > 0.0;
        }

        internal static string GetStatusLastStampText()
        {
            return string.IsNullOrEmpty(s_StatusLastSnapshotClock)
                ? kNotReadyValue
                : s_StatusLastSnapshotClock;
        }

        internal static bool IsStatusSnapshotStale(double maxAgeSeconds)
        {
            double age = GetStatusAgeSeconds();
            return age < 0.0 || age > maxAgeSeconds;
        }

        internal static double GetStatusAgeSeconds()
        {
            if (s_StatusLastSnapshotRealtime <= 0.0)
                return -1.0;

            double now =
                UnityEngine.Time.realtimeSinceStartupAsDouble;

            return Math.Max(
                0.0,
                now - s_StatusLastSnapshotRealtime);
        }

        internal static string GetCityScanNotReadyText()
        {
            return kNotReadyValue;
        }

        internal static string GetTaxiScanNotReadyText()
        {
            return kNotReadyValue;
        }

        internal static bool HasActivity()
        {
            return s_StatusLastAppliedIgnoreTaxi != 0 ||
                   s_StatusLastRemovedIgnoreTaxi != 0 ||
                   s_StatusLastRideNeedersStopped != 0 ||
                   s_StatusRideNeedersStoppedTotal != 0;
        }

        internal static string GetActivityNotReadyText()
        {
            return kNotReadyValue;
        }
    }
}
