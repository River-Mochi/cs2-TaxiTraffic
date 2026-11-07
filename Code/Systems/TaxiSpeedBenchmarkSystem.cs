// <copyright file="TaxiSpeedBenchmarkSystem.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Passive speed logger, independent of TaxiTrafficSystem.Enabled.
// Only built with -p:TaxiPerfTest=true.
#if TAXI_PERF_TEST
using System;
using System.Collections.Generic;
using System.Diagnostics;
using CS2Shared.RiverMochi;
using Game;
using Game.Simulation;

namespace TaxiTraffic
{
    public partial class TaxiSpeedBenchmarkSystem : GameSystemBase
    {
        private const int kSampleSeconds = 10;

        private readonly List<float> m_HourlySpeeds = new();
        private SimulationSystem m_SimulationSystem = null!;
        private TimeSystem m_TimeSystem = null!;
        private long m_NextSampleTick;
        private int m_SampledHour = -1;
        private bool m_SampledControlEnabled;

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            return 64;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            m_SimulationSystem =
                World.GetOrCreateSystemManaged<SimulationSystem>();
            m_TimeSystem =
                World.GetOrCreateSystemManaged<TimeSystem>();

            Enabled = false;
        }

        protected override void OnGameLoadingComplete(
            Colossal.Serialization.Entities.Purpose purpose,
            GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (mode != GameMode.Game ||
                (purpose != Colossal.Serialization.Entities.Purpose.NewGame &&
                 purpose != Colossal.Serialization.Entities.Purpose.LoadGame))
            {
                return;
            }

            WriteHourlySummary();
            m_HourlySpeeds.Clear();
            m_SampledHour = -1;
            m_NextSampleTick = 0;
            m_SampledControlEnabled = IsTaxiControlActive();
            Enabled = true;

            LogUtils.Info(
                Mod.s_Log,
                () => "[TAXI-BENCHMARK] loaded; " +
                      "ON/OFF comes from TaxiTraffic settings (not NCF).");
        }

        protected override void OnDestroy()
        {
            WriteHourlySummary();
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            long now = Stopwatch.GetTimestamp();
            if (m_NextSampleTick == 0)
            {
                m_NextSampleTick = now + kSampleSeconds * Stopwatch.Frequency;
                return;
            }

            if (now < m_NextSampleTick)
                return;

            m_NextSampleTick = now + kSampleSeconds * Stopwatch.Frequency;

            int minuteOfDay = (int)(m_TimeSystem.normalizedTime * 1440f);
            int hour = (minuteOfDay / 60) % 24;
            int minute = minuteOfDay % 60;

            float selected = m_SimulationSystem.selectedSpeed;
            float actual = m_SimulationSystem.smoothSpeed;
            bool controlEnabled = IsTaxiControlActive();

            if (m_SampledHour != hour ||
                m_SampledControlEnabled != controlEnabled)
            {
                WriteHourlySummary();
                m_SampledHour = hour;
                m_SampledControlEnabled = controlEnabled;
            }

            if (selected >= 7.99f && selected <= 8.01f)
                m_HourlySpeeds.Add(actual);

            LogUtils.Info(
                Mod.s_Log,
                () =>
                    "[TAXI-SPEED] " +
                    $"control={(controlEnabled ? "ON" : "OFF")} " +
                    $"cityTime={hour:D2}:{minute:D2} " +
                    $"selected={selected:F2} actual={actual:F3} " +
                    $"frame={m_SimulationSystem.frameIndex}");
        }

        private static bool IsTaxiControlActive()
        {
            TaxiSettings? setting = Mod.Setting;
            return setting != null &&
                   (setting.ResidentsAvoidTaxis >
                        TaxiSettings.kTaxiAvoidPercentMin ||
                    setting.BlockCommuters ||
                    setting.BlockTourists);
        }

        private void WriteHourlySummary()
        {
            if (m_SampledHour < 0 || m_HourlySpeeds.Count == 0)
                return;

            float[] speeds = m_HourlySpeeds.ToArray();
            Array.Sort(speeds);

            double total = 0;
            for (int i = 0; i < speeds.Length; i++)
                total += speeds[i];

            int count = speeds.Length;
            double median = count % 2 == 0
                ? (speeds[count / 2 - 1] + speeds[count / 2]) / 2.0
                : speeds[count / 2];

            LogUtils.Info(
                Mod.s_Log,
                () =>
                    "[TAXI-SPEED-HOUR] " +
                    $"control={(m_SampledControlEnabled ? "ON" : "OFF")} " +
                    $"hour={m_SampledHour:D2} selected=8.00 " +
                    $"samples={count} avg={total / count:F3} " +
                    $"median={median:F3} min={speeds[0]:F3} " +
                    $"max={speeds[count - 1]:F3}");

            m_HourlySpeeds.Clear();
        }
    }
}
#endif
