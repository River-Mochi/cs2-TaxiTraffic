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
using Unity.Entities;

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem : GameSystemBase
    {
        // Full household eligibility does not need to run every simulation tick.
        // The small "keep IgnoreTaxi applied" and RideNeeder protection passes do.
        private const uint kEligibilityScanMask = 15u; // every 16 simulation frames

        private const float kDebugSummaryIntervalSeconds = 120.0f;
        private const uint kTaxiEligibilityHashSalt = 0x54415849u; // 'TAXI'

        private static TaxiTrafficSystem? s_Instance;

        private Game.Simulation.SimulationSystem m_ControlSimulationSystem = null!;
        private EntityQuery m_OwnedBlockQuery;

        private bool m_ResidentCleanupPending;
        private bool m_EligibilityRefreshRequested;

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

            InitStatusSystemsOnCreate();

            // Only run after a real city is loaded.
            Enabled = false;
        }

        protected override void OnDestroy()
        {
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
            m_EligibilityRefreshRequested = true;

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

            s_Instance.m_EligibilityRefreshRequested = true;
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
                bool runFullEligibility =
                    m_EligibilityRefreshRequested ||
                    (simulationFrame & kEligibilityScanMask) == 0u;

                if (runFullEligibility)
                {
                    UpdateResidentTaxiEligibility(
                        setting,
                        out appliedIgnoreTaxi,
                        out removedIgnoreTaxi);

                    m_EligibilityRefreshRequested = false;
                }

                // Vanilla clears IgnoreTaxi during normal trip resets.
                // Keep our owned flag applied without repeating the household work.
                ReapplyOwnedTaxiBlocks(out reappliedIgnoreTaxi);

                // Catch blocked cims that already reached the on-demand taxi path.
                // This is targeted to RideNeeder only; taxi stands are left alone.
                StopBlockedRideNeeders(
                    setting,
                    out int lateAppliedIgnoreTaxi,
                    out stoppedRideNeeders,
                    out existingTaxiRequestsStopped,
                    out repathedTaxiWaiters);

                appliedIgnoreTaxi += lateAppliedIgnoreTaxi;
            }
            else if (m_ResidentCleanupPending)
            {
                // Game-default mode clears only IgnoreTaxi flags owned by Taxi Traffic.
                // In-vehicle residents are left alone until their current trip finishes.
                removedIgnoreTaxi = ClearOwnedResidentTaxiBlocks();

                m_ResidentCleanupPending =
                    !m_OwnedBlockQuery.IsEmptyIgnoreFilter;
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
