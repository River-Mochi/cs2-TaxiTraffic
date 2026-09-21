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
using Unity.Jobs;

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

        // Structural changes go through the same barrier vanilla resident systems
        // use. Playing an EntityCommandBuffer back directly against EntityManager
        // calls CompleteAllJobsAndInvalidateArrays, which drains every worker job
        // in the world; doing that once per simulation frame is what made Taxi
        // Traffic cost roughly a fifth of the simulation thread.
        private EndFrameBarrier m_EndFrameBarrier = null!;

        private EntityQuery m_OwnedBlockQuery;
        private EntityQuery m_EligibilityFullQuery;
        private EntityQuery m_EligibilityBucketQuery;
        private EntityQuery m_MaxAvoidanceEligibilityBucketQuery;
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
            m_EndFrameBarrier =
                World.GetOrCreateSystemManaged<EndFrameBarrier>();

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

            // At 100% with both groups enabled, residents already owned by Taxi
            // Traffic do not need another eligibility decision. Only new/unowned
            // residents stay in this bucket query; the tiny reapply pass maintains
            // IgnoreTaxi on residents we already own.
            m_MaxAvoidanceEligibilityBucketQuery =
                GetEntityQuery(
                    ComponentType.ReadWrite<Game.Creatures.Resident>(),
                    ComponentType.ReadOnly<Game.Simulation.UpdateFrame>(),
                    ComponentType.Exclude<IgnoreTaxiMark>(),
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
                    5,
                    Allocator.Persistent,
                    NativeArrayOptions.ClearMemory);

            InitStatusSystemsOnCreate();

            // Only run after a real city is loaded.
            Enabled = false;
        }

        protected override void OnDestroy()
        {
            // The counter arrays are handed to scheduled jobs, so a world teardown
            // while one is still in flight would free memory a worker is writing.
            // Wait for our own work before disposing anything.
            CompleteDependency();

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

            // Taxi Traffic's jobs are scheduled, not run inline, so their counters
            // are only valid once those jobs finish. SystemState.BeforeOnUpdate
            // completes this system's own handle before OnUpdate is entered, so the
            // previous frame's results are ready here and nowhere earlier. Status
            // and DEBUG numbers are therefore one simulation frame behind, which
            // costs nothing and keeps the hot path free of a Complete() call.
            PublishPreviousFrameCounters();
            ResetJobCounters();

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

                // DEBUG perfMs now measures what this system costs the simulation
                // thread, which is scheduling only. Worker execution time is not
                // included, and that is the point of the measurement.
#if DEBUG
                long eligibilityStartTicks =
                    System.Diagnostics.Stopwatch.GetTimestamp();
#endif

                JobHandle handle = Dependency;

                bool usedFullEligibilityRefresh =
                    m_FullEligibilityRefreshRequested;

                if (usedFullEligibilityRefresh)
                {
                    // City load gets one full reconciliation. Options changes are
                    // deliberately spread over the normal 16-frame bucket cycle.
                    handle = ScheduleResidentTaxiEligibility(
                        avoidanceData,
                        handle);

                    m_FullEligibilityRefreshRequested = false;
                }
                else
                {
                    handle = ScheduleResidentTaxiEligibilityBucket(
                        avoidanceData,
                        simulationFrame,
                        handle);
                }

#if DEBUG
                RecordDebugEligibilityTiming(
                    System.Diagnostics.Stopwatch.GetTimestamp() -
                    eligibilityStartTicks);
#endif

                // A separate reapply scan is only needed when the eligibility query
                // could not see the residents Taxi Traffic owns. That is true for
                // the maximum-avoidance bucket query alone, which excludes
                // IgnoreTaxiMark. The full query and the general bucket query both
                // include owned residents, and ResidentTaxiEligibilityJob already
                // restores IgnoreTaxi for them in the same pass, so scanning the
                // same bucket again would repeat work that is already done.
                if (!usedFullEligibilityRefresh &&
                    UsesMaximumAvoidanceQuery(avoidanceData))
                {
                    // ResidentAI only updates one of its 16 UpdateFrame buckets each
                    // frame. Reapply IgnoreTaxi only to owned residents in that bucket.
#if DEBUG
                    long reapplyStartTicks =
                        System.Diagnostics.Stopwatch.GetTimestamp();
#endif

                    handle = ScheduleReapplyOwnedTaxiBlocks(
                        simulationFrame,
                        handle);

#if DEBUG
                    RecordDebugReapplyTiming(
                        System.Diagnostics.Stopwatch.GetTimestamp() -
                        reapplyStartTicks);
#endif
                }

                // Catch blocked cims that already reached the on-demand taxi path.
                // This stays every simulation update so later taxi systems do not
                // get a chance to dispatch a newly created request.
#if DEBUG
                long enforcementStartTicks =
                    System.Diagnostics.Stopwatch.GetTimestamp();
#endif

                handle = ScheduleStopBlockedRideNeeders(
                    avoidanceData,
                    handle);

#if DEBUG
                RecordDebugEnforcementTiming(
                    System.Diagnostics.Stopwatch.GetTimestamp() -
                    enforcementStartTicks);
#endif

                // The passes are chained rather than run side by side: they all
                // write Game.Creatures.Resident, so they must not overlap.
                // Registering with the barrier makes it wait for this work before
                // playing back the structural changes the jobs recorded, and
                // assigning Dependency lets every later system that touches these
                // components wait on us instead of racing.
                m_EndFrameBarrier.AddJobHandleForProducer(handle);
                Dependency = handle;
            }
            else
            {
                // There is nothing to reconcile at city load when all controls are
                // vanilla. Any future Options change should use the bucketed path.
                m_FullEligibilityRefreshRequested = false;

                if (m_ResidentCleanupPending)
                {
                    // Game-default mode clears only IgnoreTaxi flags owned by Taxi
                    // Traffic. In-vehicle residents are left alone until their trip
                    // finishes. This runs while shutting down, so its main-thread
                    // pass and immediate playback are not on any hot path.
                    s_StatusLastRemovedIgnoreTaxi = ClearOwnedResidentTaxiBlocks();

                    m_ResidentCleanupPending =
                        !m_OwnedBlockQuery.IsEmptyIgnoreFilter;
                }
            }

            if (setting.EnableDebugLogging)
                TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

            // True vanilla/no-op state: once our marker is gone, stop running.
            if (!residentControlActive && !m_ResidentCleanupPending)
                Enabled = false;
        }

        /// <summary>
        /// Copies the counters last frame's jobs wrote into the Status and DEBUG
        /// statics. Only valid at the top of OnUpdate, where this system's previous
        /// job handle has already been completed on our behalf.
        /// </summary>
        private void PublishPreviousFrameCounters()
        {
            int stoppedRideNeeders = m_EnforcementCounters[1];
            int existingTaxiRequestsStopped = m_EnforcementCounters[2];
            int repathedTaxiWaiters = m_EnforcementCounters[3];

            RecordLastUpdateCounters(
                m_EligibilityCounters[0] + m_EnforcementCounters[0],
                m_EligibilityCounters[1],
                m_EligibilityCounters[2] + m_ReapplyCounter[0],
                stoppedRideNeeders,
                existingTaxiRequestsStopped,
                repathedTaxiWaiters,
                m_EnforcementCounters[4]);

            s_StatusRideNeedersStoppedTotal += stoppedRideNeeders;
            s_StatusTaxiRequestsStoppedTotal += existingTaxiRequestsStopped;
            s_StatusTaxiWaitersRepathedTotal += repathedTaxiWaiters;
        }

        private void ResetJobCounters()
        {
            for (int i = 0; i < m_EligibilityCounters.Length; i++)
                m_EligibilityCounters[i] = 0;

            m_ReapplyCounter[0] = 0;

            for (int i = 0; i < m_EnforcementCounters.Length; i++)
                m_EnforcementCounters[i] = 0;
        }

        private static void RecordLastUpdateCounters(
            int appliedIgnoreTaxi,
            int removedIgnoreTaxi,
            int reappliedIgnoreTaxi,
            int stoppedRideNeeders,
            int existingTaxiRequestsStopped,
            int repathedTaxiWaiters,
            int dispatchedSkipped)
        {
            s_StatusLastAppliedIgnoreTaxi = appliedIgnoreTaxi;
            s_StatusLastRemovedIgnoreTaxi = removedIgnoreTaxi;
            s_StatusLastReappliedIgnoreTaxi = reappliedIgnoreTaxi;
            s_StatusLastRideNeedersStopped = stoppedRideNeeders;
            s_StatusLastTaxiRequestsStopped = existingTaxiRequestsStopped;
            s_StatusLastTaxiWaitersRepathed = repathedTaxiWaiters;
            s_StatusLastDispatchedSkipped = dispatchedSkipped;
        }
    }
}
