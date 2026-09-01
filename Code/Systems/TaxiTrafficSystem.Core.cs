// <copyright file="TaxiTrafficSystem.Core.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Core.cs
// Purpose: system lifecycle and update coordinator.

using Game;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem : GameSystemBase
    {
        // Vanilla ResidentAI divides resident work across 16 UpdateFrame buckets.
        // Taxi Traffic follows the same buckets so each resident is reevaluated
        // once per 16 simulation frames without one large periodic scan.
        private const uint kResidentUpdateFrameCount = 16u;

        private const float kDebugSummaryIntervalSeconds = 120.0f;
        private const uint kTaxiEligibilityHashSalt = 0x54415849u; // 'TAXI'

        private static TaxiTrafficSystem? s_Instance;

        private Game.Simulation.SimulationSystem m_ControlSimulationSystem = null!;
        private EntityQuery m_OwnedBlockQuery;
        private EntityQuery m_EligibilityFullQuery;
        private EntityQuery m_EligibilityBucketQuery;
        private EntityQuery m_ReapplyBlockQuery;
        private EntityQuery m_RideNeederQuery;

        private NativeArray<int> m_EligibilityCounters;
        private NativeArray<int> m_ReapplyCounter;
        private NativeArray<int> m_EnforcementCounters;

        private bool m_ResidentCleanupPending;
        private bool m_FullEligibilityRefreshRequested;

        protected override void OnCreate()
        {
            base.OnCreate();

            s_Instance = this;
            m_ControlSimulationSystem =
                World.GetOrCreateSystemManaged<Game.Simulation.SimulationSystem>();

            m_OwnedBlockQuery =
                GetEntityQuery(
                    ComponentType.ReadOnly<IgnoreTaxiMark>(),
                    ComponentType.Exclude<Deleted>(),
                    ComponentType.Exclude<Temp>());

            // City load gets one complete reconciliation so saved residents start
            // from a known state before normal bucketed updates take over.
            m_EligibilityFullQuery =
                GetEntityQuery(
                    ComponentType.ReadWrite<Game.Creatures.Resident>(),
                    ComponentType.Exclude<Game.Creatures.CurrentVehicle>(),
                    ComponentType.Exclude<Deleted>(),
                    ComponentType.Exclude<Temp>());

            // Use the same UpdateFrame bucket as ResidentAI for steady-state eligibility work.
            m_EligibilityBucketQuery =
                GetEntityQuery(
                    ComponentType.ReadWrite<Game.Creatures.Resident>(),
                    ComponentType.ReadOnly<Game.Simulation.UpdateFrame>(),
                    ComponentType.Exclude<Game.Creatures.CurrentVehicle>(),
                    ComponentType.Exclude<Deleted>(),
                    ComponentType.Exclude<Temp>());

            // ResidentAI processes one UpdateFrame bucket per simulation frame.
            // Reapply only that same bucket after ResidentAI had a chance to clear IgnoreTaxi.
            m_ReapplyBlockQuery =
                GetEntityQuery(
                    ComponentType.ReadWrite<Game.Creatures.Resident>(),
                    ComponentType.ReadOnly<IgnoreTaxiMark>(),
                    ComponentType.ReadOnly<Game.Simulation.UpdateFrame>(),
                    ComponentType.Exclude<Game.Creatures.CurrentVehicle>(),
                    ComponentType.Exclude<Deleted>(),
                    ComponentType.Exclude<Temp>());

            // Keep RideNeeder protection targeted. This query is small compared
            // with the full resident population and must remain available every
            // simulation update so taxi requests are stopped before dispatch.
            m_RideNeederQuery =
                GetEntityQuery(
                    ComponentType.ReadWrite<Game.Creatures.Resident>(),
                    ComponentType.ReadOnly<Game.Creatures.RideNeeder>(),
                    ComponentType.Exclude<Game.Creatures.CurrentVehicle>(),
                    ComponentType.Exclude<Deleted>(),
                    ComponentType.Exclude<Temp>());

            m_EligibilityCounters =
                new NativeArray<int>(
                    3,
                    Allocator.Persistent,
                    NativeArrayOptions.ClearMemory);

            m_ReapplyCounter =
                new NativeArray<int>(
                    1,
                    Allocator.Persistent,
                    NativeArrayOptions.ClearMemory);

            m_EnforcementCounters =
                new NativeArray<int>(
                    4,
                    Allocator.Persistent,
                    NativeArrayOptions.ClearMemory);

            InitStatusSystemsOnCreate();

            // Only run after a real city is loaded.
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            if (m_EligibilityCounters.IsCreated)
                m_EligibilityCounters.Dispose();

            if (m_ReapplyCounter.IsCreated)
                m_ReapplyCounter.Dispose();

            if (m_EnforcementCounters.IsCreated)
                m_EnforcementCounters.Dispose();

            if (ReferenceEquals(s_Instance, this))
                s_Instance = null;

            base.OnDestroy();
        }

        protected override void OnGameLoadingComplete(
            Colossal.Serialization.Entities.Purpose purpose,
            GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            bool isRealGame =
                mode == GameMode.Game &&
                (purpose == Colossal.Serialization.Entities.Purpose.NewGame ||
                 purpose == Colossal.Serialization.Entities.Purpose.LoadGame);

            if (!isRealGame)
                return;

            m_ResidentCleanupPending = true;
            m_FullEligibilityRefreshRequested = true;

            ResetDebugOnCityLoaded();
            ResetStatusOnCityLoaded();

            Enabled = true;

#if DEBUG
            CS2Shared.RiverMochi.LogUtils.Info(
                Mod.s_Log,
                () => $"{Mod.ModTag} TaxiTrafficSystem enabled (city load complete).");
#endif
        }

        internal static void WakeForSettingsChange()
        {
            if (s_Instance == null)
                return;

            // Do not turn an Options change into a full-city spike. The new
            // setting reaches every resident through the next 16 buckets.
            s_Instance.m_FullEligibilityRefreshRequested = false;
            s_Instance.Enabled = true;
        }

        protected override void OnUpdate()
        {
            TaxiSettings? setting = Mod.Setting;
            if (setting is null)
            {
                Enabled = false;
                return;
            }

            int appliedIgnoreTaxi = 0;
            int removedIgnoreTaxi = 0;
            int reappliedIgnoreTaxi = 0;
            int stoppedRideNeeders = 0;
            int existingTaxiRequestsStopped = 0;
            int repathedTaxiWaiters = 0;

            bool residentControlActive =
                setting.ResidentsAvoidTaxis > TaxiSettings.kTaxiAvoidPercentMin ||
                setting.BlockCommuters ||
                setting.BlockTourists;

            if (residentControlActive)
            {
                m_ResidentCleanupPending = true;

                uint simulationFrame = m_ControlSimulationSystem.frameIndex;
                TaxiAvoidanceData avoidanceData =
                    CreateTaxiAvoidanceData(setting);

#if DEBUG
                long eligibilityStartTicks =
                    System.Diagnostics.Stopwatch.GetTimestamp();
#endif

                if (m_FullEligibilityRefreshRequested)
                {
                    // City load gets one immediate reconciliation. Options changes
                    // are deliberately spread over the normal 16-frame bucket cycle.
                    UpdateResidentTaxiEligibility(
                        avoidanceData,
                        out appliedIgnoreTaxi,
                        out removedIgnoreTaxi,
                        out int fullScanReappliedIgnoreTaxi);

                    reappliedIgnoreTaxi += fullScanReappliedIgnoreTaxi;
                    m_FullEligibilityRefreshRequested = false;
                }
                else
                {
                    UpdateResidentTaxiEligibilityBucket(
                        avoidanceData,
                        simulationFrame,
                        out appliedIgnoreTaxi,
                        out removedIgnoreTaxi,
                        out int bucketEligibilityReappliedIgnoreTaxi);

                    reappliedIgnoreTaxi += bucketEligibilityReappliedIgnoreTaxi;
                }

#if DEBUG
                RecordDebugEligibilityTiming(
                    System.Diagnostics.Stopwatch.GetTimestamp() -
                    eligibilityStartTicks);
#endif

                // ResidentAI only updates one of its 16 UpdateFrame buckets each frame.
                // Reapply IgnoreTaxi only to Taxi Traffic-owned residents in that same bucket.
#if DEBUG
                long reapplyStartTicks =
                    System.Diagnostics.Stopwatch.GetTimestamp();
#endif

                ReapplyOwnedTaxiBlocks(
                    simulationFrame,
                    out int bucketReappliedIgnoreTaxi);

                reappliedIgnoreTaxi += bucketReappliedIgnoreTaxi;

#if DEBUG
                RecordDebugReapplyTiming(
                    System.Diagnostics.Stopwatch.GetTimestamp() -
                    reapplyStartTicks);
#endif

                // Catch blocked cims that already reached the on-demand taxi path.
                // This stays every simulation update so later taxi systems do not
                // get a chance to dispatch a newly created request.
#if DEBUG
                long enforcementStartTicks =
                    System.Diagnostics.Stopwatch.GetTimestamp();
#endif

                StopBlockedRideNeeders(
                    avoidanceData,
                    out int lateAppliedIgnoreTaxi,
                    out stoppedRideNeeders,
                    out existingTaxiRequestsStopped,
                    out repathedTaxiWaiters);

                appliedIgnoreTaxi += lateAppliedIgnoreTaxi;

#if DEBUG
                RecordDebugEnforcementTiming(
                    System.Diagnostics.Stopwatch.GetTimestamp() -
                    enforcementStartTicks);
#endif
            }
            else
            {
                // There is nothing to reconcile at city load when all controls are
                // vanilla. Any future Options change should use the bucketed path.
                m_FullEligibilityRefreshRequested = false;

                if (m_ResidentCleanupPending)
                {
                    // Game-default mode clears only IgnoreTaxi flags owned by Taxi Traffic.
                    // In-vehicle residents are left alone until their current trip finishes.
                    removedIgnoreTaxi = ClearOwnedResidentTaxiBlocks();

                    m_ResidentCleanupPending =
                        !m_OwnedBlockQuery.IsEmptyIgnoreFilter;
                }
            }

            RecordLastUpdateCounters(
                appliedIgnoreTaxi,
                removedIgnoreTaxi,
                reappliedIgnoreTaxi,
                stoppedRideNeeders,
                existingTaxiRequestsStopped,
                repathedTaxiWaiters);

            s_StatusRideNeedersStoppedTotal += stoppedRideNeeders;
            s_StatusTaxiRequestsStoppedTotal += existingTaxiRequestsStopped;
            s_StatusTaxiWaitersRepathedTotal += repathedTaxiWaiters;

            if (setting.EnableDebugLogging)
                TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

            // True vanilla/no-op state: once our marker is gone, stop running.
            if (!residentControlActive && !m_ResidentCleanupPending)
                Enabled = false;
        }

        private static void RecordLastUpdateCounters(
            int appliedIgnoreTaxi,
            int removedIgnoreTaxi,
            int reappliedIgnoreTaxi,
            int stoppedRideNeeders,
            int existingTaxiRequestsStopped,
            int repathedTaxiWaiters)
        {
            s_StatusLastAppliedIgnoreTaxi = appliedIgnoreTaxi;
            s_StatusLastRemovedIgnoreTaxi = removedIgnoreTaxi;
            s_StatusLastReappliedIgnoreTaxi = reappliedIgnoreTaxi;
            s_StatusLastRideNeedersStopped = stoppedRideNeeders;
            s_StatusLastTaxiRequestsStopped = existingTaxiRequestsStopped;
            s_StatusLastTaxiWaitersRepathed = repathedTaxiWaiters;
        }
    }
}
