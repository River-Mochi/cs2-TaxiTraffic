// <copyright file="TaxiTrafficSystem.Core.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Core.cs
// System lifecycle and update coordinator.


namespace TaxiTraffic
{
    using Game;             // GameSystemBase, GameMode
    using Game.Citizens;
    using Game.Creatures;   // prevent gs errors
    using Game.Pathfind;    // prevent gs errors

    using Game.Vehicles;    // prevent gs errors
    using Unity.Collections; // prevent gs errors
    using Unity.Entities;   // prevent gs errors

    public partial class TaxiTrafficSystem : GameSystemBase
    {
        private const int kMarkBatchPerUpdate = 2000;
        private const int kUpdateIntervalFrames = 16;

        // Broad queue fallback: burst after setting changes, then stay cheap.
        private const float kTaxiQueueSweepFastIntervalSeconds = 1.0f;
        private const float kTaxiQueueSweepMaintenanceIntervalSeconds = 30.0f;
        private const int kTaxiQueueSweepFastPasses = 3;

        private const float kDebugSummaryIntervalSeconds = 120.0f;
        private const uint kTaxiEligibilityHashSalt = 0x54415849u; // 'TAXI'

        private double m_LastUnstickRealtime;
        private int m_TaxiQueueSweepFastPassesRemaining;

        private int m_LastResidentsAllowedToUseTaxis = int.MinValue;
        private bool m_LastBlockCommuters;
        private bool m_LastBlockTourists;
        private bool m_TaxiEligibilityResetInProgress;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return kUpdateIntervalFrames;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            InitStatusSystemsOnCreate();

            // Only run after a real city is loaded.
            Enabled = false;
        }

        protected override void OnGameLoadingComplete(Colossal.Serialization.Entities.Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            bool isRealGame =
                mode == GameMode.Game &&
                (purpose == Colossal.Serialization.Entities.Purpose.NewGame ||
                 purpose == Colossal.Serialization.Entities.Purpose.LoadGame);

            if (!isRealGame)
                return;

            m_LastUnstickRealtime = UnityEngine.Time.realtimeSinceStartupAsDouble;
            m_TaxiQueueSweepFastPassesRemaining = 0;
            m_OutsideBlockFallbackActive = false;

            m_LastResidentsAllowedToUseTaxis = int.MinValue;
            m_LastBlockCommuters = false;
            m_LastBlockTourists = false;
            m_TaxiEligibilityResetInProgress = false;

            ResetDebugOnCityLoaded();
            ResetStatusOnCityLoaded();

            Enabled = true;

#if DEBUG
            CS2Shared.RiverMochi.LogUtils.Info(Mod.s_Log, () => $"{Mod.ModTag} TaxiTrafficSystem enabled (city load complete).");
#endif
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
            int skippedCommuters = 0;
            int skippedTourists = 0;
            int skippedGroupTravelers = 0;

            int clearedTaxiLaneWaiting = 0;
            int clearedTaxiStandWaiting = 0;
            int removedRideNeeders = 0;

            // Keep outside control independent from the local-rider settings/reset cycle.
            SuppressOutsideTaxiSupplyRequests(setting);

            // Evidence for the old outside-taxi move-in issue; count each Resident trip only once.
            ObserveOutsideTaxiMoveInEvidence();

            // Do not sweep all marked residents every update; too expensive in large cities.
            int clearedGroupTravelers = 0;

            bool changed = DetectTaxiEligibilitySettingChange(setting);
            if (changed)
            {
                m_TaxiEligibilityResetInProgress = true;

                // Give stale taxi waits a few quick cleanup passes after a settings change.
                m_TaxiQueueSweepFastPassesRemaining = kTaxiQueueSweepFastPasses;
            }

            // Setting changes clear old buckets before applying the new stable bucket.
            if (m_TaxiEligibilityResetInProgress)
            {
                int resetCount = ResetTaxiEligibilityMarkersBatch();

                RecordLastUpdateCounters(
                    appliedIgnoreTaxi,
                    skippedCommuters,
                    skippedTourists,
                    skippedGroupTravelers,
                    clearedGroupTravelers,
                    clearedTaxiLaneWaiting,
                    clearedTaxiStandWaiting,
                    removedRideNeeders);

                if (resetCount > 0)
                {
                    if (setting.EnableDebugLogging)
                        TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

                    return;
                }

                m_TaxiEligibilityResetInProgress = false;
            }

            // Group membership is dynamic: entering a group gets a temporary exemption; leaving it must re-enter normal eligibility.
            clearedGroupTravelers = MaintainGroupTaxiExemptionsBatch();
            s_StatusGroupRepairsTotal += clearedGroupTravelers;

            bool vanillaResidents = setting.ResidentsAllowedToUseTaxis >= TaxiSettings.kTaxiAllowedPercentMax;
            bool vanillaGroups = !setting.BlockCommuters && !setting.BlockTourists;

            if (vanillaResidents && vanillaGroups)
            {
                UnmarkIgnoreTaxiBatch(out _);

                RecordLastUpdateCounters(
                    appliedIgnoreTaxi,
                    skippedCommuters,
                    skippedTourists,
                    skippedGroupTravelers,
                    clearedGroupTravelers,
                    clearedTaxiLaneWaiting,
                    clearedTaxiStandWaiting,
                    removedRideNeeders);

                if (setting.EnableDebugLogging)
                    TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

                return;
            }

            ApplyTaxiEligibilityBatch(
                setting,
                out appliedIgnoreTaxi,
                out skippedCommuters,
                out skippedTourists,
                out skippedGroupTravelers);

            // Vanilla clears IgnoreTaxi at arrival; reused creatures keep our blocked marker.
            RepairStaleIgnoreTaxiOnTripStart();

            // RideNeeder is already a narrow archetype, so stop invalid taxi requests every system update.
            UnstickTaxiLaneWaiters(setting, out clearedTaxiLaneWaiting, out removedRideNeeders);


            // Broad fallback scan: short burst after changes, then rare maintenance.
            double now = UnityEngine.Time.realtimeSinceStartupAsDouble;

            double queueSweepInterval =
                m_TaxiQueueSweepFastPassesRemaining > 0
                    ? kTaxiQueueSweepFastIntervalSeconds
                    : kTaxiQueueSweepMaintenanceIntervalSeconds;

            if (now - m_LastUnstickRealtime >= queueSweepInterval)
            {
                m_LastUnstickRealtime = now;

                UnstickTaxiQueues(setting, out clearedTaxiStandWaiting);

                if (clearedTaxiStandWaiting > 0)
                {
                    // Found a real stale queue; do a few follow-up passes.
                    m_TaxiQueueSweepFastPassesRemaining = kTaxiQueueSweepFastPasses;
                }
                else if (m_TaxiQueueSweepFastPassesRemaining > 0)
                {
                    m_TaxiQueueSweepFastPassesRemaining--;
                }
            }

            RecordLastUpdateCounters(
                appliedIgnoreTaxi,
                skippedCommuters,
                skippedTourists,
                skippedGroupTravelers,
                clearedGroupTravelers,
                clearedTaxiLaneWaiting,
                clearedTaxiStandWaiting,
                removedRideNeeders);

            if (setting.EnableDebugLogging)
                TickDebugLogging(setting, kDebugSummaryIntervalSeconds);
        }

        private static void RecordLastUpdateCounters(
            int appliedIgnoreTaxi,
            int skippedCommuters,
            int skippedTourists,
            int skippedGroupTravelers,
            int clearedGroupTravelers,
            int clearedTaxiLaneWaiting,
            int clearedTaxiStandWaiting,
            int removedRideNeeders)
        {
            s_StatusLastAppliedIgnoreTaxi = appliedIgnoreTaxi;
            s_StatusLastSkippedCommuters = skippedCommuters;
            s_StatusLastSkippedTourists = skippedTourists;
            s_StatusLastSkippedGroupTravelers = skippedGroupTravelers;
            s_StatusLastClearedGroupTravelers = clearedGroupTravelers;
            s_StatusLastClearedTaxiLaneWaiting = clearedTaxiLaneWaiting;
            s_StatusLastClearedTaxiStandWaiting = clearedTaxiStandWaiting;
            s_StatusLastRemovedRideNeeder = removedRideNeeders;
        }

    }
}
