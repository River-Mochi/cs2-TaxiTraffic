// <copyright file="TaxiTrafficSystem.Core.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

    using Game;              // GameSystemBase, GameMode


namespace TaxiTraffic
{
    // File: Systems/TaxiTrafficSystem.Core.cs
    // System lifecycle and update coordinator.

    public partial class TaxiTrafficSystem : GameSystemBase
    {
        private const int kMarkBatchPerUpdate = 2000;
        private const int kUpdateIntervalFrames = 16;

        private const float kDebugSummaryIntervalSeconds = 120.0f;
        private const uint kTaxiEligibilityHashSalt = 0x54415849u; // 'TAXI'

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

            m_LastResidentsAllowedToUseTaxis = int.MinValue;
            m_LastBlockCommuters = false;
            m_LastBlockTourists = false;
            m_TaxiEligibilityResetInProgress = false;

            ResetDebugOnCityLoaded();
            ResetStatusOnCityLoaded();

            Enabled = true;

#if DEBUG
            CS2Shared.RiverMochi.LogUtils.Info(
                Mod.s_Log,
                () => $"{Mod.ModTag} TaxiTrafficSystem enabled (city load complete).");
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
            int clearedGroupTravelers = 0;

            // Soft-enforcement diagnostic build:
            // do not cancel taxi requests, clear taxi lanes/queues, invalidate paths,
            // or invoke outside-connection taxi cancellation.
            int clearedTaxiLaneWaiting = 0;
            int clearedTaxiStandWaiting = 0;
            int removedRideNeeders = 0;

            bool changed = DetectTaxiEligibilitySettingChange(setting);
            if (changed)
                m_TaxiEligibilityResetInProgress = true;

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

            // Travel-group exemption is no longer used; keep the compatibility/status hook.
            clearedGroupTravelers = MaintainGroupTaxiExemptionsBatch();
            s_StatusGroupRepairsTotal += clearedGroupTravelers;

            bool vanillaResidents =
                setting.ResidentsAllowedToUseTaxis >=
                TaxiSettings.kTaxiAllowedPercentMax;

            bool vanillaGroups =
                !setting.BlockCommuters && !setting.BlockTourists;

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
