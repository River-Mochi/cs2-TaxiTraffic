// <copyright file="TaxiTrafficSystem.Core.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/TaxiTrafficSystem.Core.cs
// Purpose: system lifecycle and update coordinator.

using Game;            // GameSystemBase, GameMode
using Game.Simulation; // EndFrameBarrier

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem : GameSystemBase
    {
        // Taxi eligibility is not time-critical. A slower interval keeps the
        // resident scan lightweight, especially at high simulation speed.
        private const int kUpdateIntervalFrames = 64;

        private const float kDebugSummaryIntervalSeconds = 120.0f;
        private const uint kTaxiEligibilityHashSalt = 0x54415849u; // 'TAXI'

        private static TaxiTrafficSystem? s_Instance;

        private EndFrameBarrier m_EndFrameBarrier = null!;

        // These only track whether one final cleanup pass may still be needed.
        // Normal setting changes do NOT reset or rebuild every resident.
        private bool m_ResidentCleanupPending;
        private bool m_OutsideCleanupPending;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return kUpdateIntervalFrames;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            s_Instance = this;
            m_EndFrameBarrier = World.GetOrCreateSystemManaged<EndFrameBarrier>();

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

            // Check once for an owned resident marker from an earlier session/build.
            // If none exists and all settings are vanilla, the system goes dormant.
            m_ResidentCleanupPending = true;
            m_OutsideCleanupPending = false;

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
            if (s_Instance != null)
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

            bool residentControlActive =
                setting.ResidentsAvoidTaxis > TaxiSettings.kTaxiAvoidPercentMin ||
                setting.BlockCommuters ||
                setting.BlockTourists;

            bool outsideControlActive = setting.BlockOutsideTaxis;

            if (residentControlActive)
            {
                m_ResidentCleanupPending = true;

                UpdateResidentTaxiEligibility(
                    setting,
                    out appliedIgnoreTaxi,
                    out removedIgnoreTaxi);
            }
            else if (m_ResidentCleanupPending)
            {
                // Game-default resident behavior: clear only IgnoreTaxi flags owned
                // by Taxi Traffic. Once none remain, no resident scan is needed.
                removedIgnoreTaxi = ClearOwnedResidentTaxiBlocks();

                if (removedIgnoreTaxi == 0)
                    m_ResidentCleanupPending = false;
            }

            // Outside-connection control remains isolated from normal resident
            // eligibility. With the option OFF, this code path does not run.
            if (outsideControlActive)
            {
                m_OutsideCleanupPending = true;

                UpdateOutsideTaxiBlocking(
                    setting,
                    out _,
                    out _);
            }
            else if (m_OutsideCleanupPending)
            {
                // One cleanup pass after the option is turned OFF.
                UpdateOutsideTaxiBlocking(
                    setting,
                    out _,
                    out _);

                m_OutsideCleanupPending = false;
            }

            RecordLastUpdateCounters(
                appliedIgnoreTaxi,
                removedIgnoreTaxi);

            if (setting.EnableDebugLogging)
                TickDebugLogging(setting, kDebugSummaryIntervalSeconds);

            // True vanilla/no-op state. After Taxi Traffic's own resident markers
            // are gone and outside control is OFF, stop scheduling this system.
            if (!residentControlActive &&
                !outsideControlActive &&
                !m_ResidentCleanupPending &&
                !m_OutsideCleanupPending)
            {
                Enabled = false;
            }
        }

        private static void RecordLastUpdateCounters(
            int appliedIgnoreTaxi,
            int removedIgnoreTaxi)
        {
            s_StatusLastAppliedIgnoreTaxi = appliedIgnoreTaxi;
            s_StatusLastRemovedIgnoreTaxi = removedIgnoreTaxi;
        }
    }
}
