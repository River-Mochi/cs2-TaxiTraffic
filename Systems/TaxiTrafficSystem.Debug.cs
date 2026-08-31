// <copyright file="TaxiTrafficSystem.Debug.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Status/TaxiTrafficSystem.Debug.cs
// Optional low-frequency DEBUG summary logging.

namespace TaxiTraffic
{
    using System;
    using Game.City;

    public partial class TaxiTrafficSystem
    {
        private const float kDebugMinSummaryIntervalSeconds = 60f;
        private const double kDebugForceStatusRefreshMaxAgeSeconds = 30.0;

        private float m_DebugTimerSeconds;

#if DEBUG
        private long m_DebugEligibilityTotalTicks;
        private long m_DebugEligibilityMaxTicks;
        private int m_DebugEligibilitySamples;

        private long m_DebugReapplyTotalTicks;
        private long m_DebugReapplyMaxTicks;
        private int m_DebugReapplySamples;

        private long m_DebugEnforcementTotalTicks;
        private long m_DebugEnforcementMaxTicks;
        private int m_DebugEnforcementSamples;
#endif

        private void ResetDebugOnCityLoaded()
        {
            m_DebugTimerSeconds = 0f;

#if DEBUG
            ResetDebugPerformanceTimings();
#endif
        }

#if DEBUG
        private void RecordDebugEligibilityTiming(long elapsedTicks)
        {
            RecordDebugTiming(
                elapsedTicks,
                ref m_DebugEligibilityTotalTicks,
                ref m_DebugEligibilityMaxTicks,
                ref m_DebugEligibilitySamples);
        }

        private void RecordDebugReapplyTiming(long elapsedTicks)
        {
            RecordDebugTiming(
                elapsedTicks,
                ref m_DebugReapplyTotalTicks,
                ref m_DebugReapplyMaxTicks,
                ref m_DebugReapplySamples);
        }

        private void RecordDebugEnforcementTiming(long elapsedTicks)
        {
            RecordDebugTiming(
                elapsedTicks,
                ref m_DebugEnforcementTotalTicks,
                ref m_DebugEnforcementMaxTicks,
                ref m_DebugEnforcementSamples);
        }

        private static void RecordDebugTiming(
            long elapsedTicks,
            ref long totalTicks,
            ref long maxTicks,
            ref int samples)
        {
            totalTicks += elapsedTicks;
            maxTicks = Math.Max(maxTicks, elapsedTicks);
            samples++;
        }

        private static double DebugAverageMilliseconds(
            long totalTicks,
            int samples)
        {
            if (samples <= 0)
                return 0.0;

            return DebugMilliseconds(totalTicks) / samples;
        }

        private static double DebugMilliseconds(long ticks)
        {
            return ticks * 1000.0 /
                   System.Diagnostics.Stopwatch.Frequency;
        }

        private void ResetDebugPerformanceTimings()
        {
            m_DebugEligibilityTotalTicks = 0;
            m_DebugEligibilityMaxTicks = 0;
            m_DebugEligibilitySamples = 0;

            m_DebugReapplyTotalTicks = 0;
            m_DebugReapplyMaxTicks = 0;
            m_DebugReapplySamples = 0;

            m_DebugEnforcementTotalTicks = 0;
            m_DebugEnforcementMaxTicks = 0;
            m_DebugEnforcementSamples = 0;
        }
#endif

        private void TickDebugLogging(
            TaxiSettings setting,
            float intervalSeconds)
        {
            float effectiveIntervalSeconds =
                Math.Max(
                    intervalSeconds,
                    kDebugMinSummaryIntervalSeconds);

            m_DebugTimerSeconds +=
                UnityEngine.Time.unscaledDeltaTime;

            if (m_DebugTimerSeconds < effectiveIntervalSeconds)
                return;

            m_DebugTimerSeconds = 0f;

#if DEBUG
            if (IsStatusSnapshotStale(
                    kDebugForceStatusRefreshMaxAgeSeconds))
            {
                RefreshStatusSnapshotForDebug();
            }
#endif

            int dailyTaxiCitizen = 0;
            int dailyTaxiTourist = 0;

            try
            {
                if (m_CityStatisticsSystem != null)
                {
                    dailyTaxiCitizen =
                        m_CityStatisticsSystem.GetStatisticValue(
                            StatisticType.PassengerCountTaxi,
                            (int)PassengerType.Citizen);

                    dailyTaxiTourist =
                        m_CityStatisticsSystem.GetStatisticValue(
                            StatisticType.PassengerCountTaxi,
                            (int)PassengerType.Tourist);
                }
            }
            catch
            {
            }

#if DEBUG
            double eligibilityAverageMs =
                DebugAverageMilliseconds(
                    m_DebugEligibilityTotalTicks,
                    m_DebugEligibilitySamples);

            double eligibilityMaxMs =
                DebugMilliseconds(m_DebugEligibilityMaxTicks);

            int eligibilitySamples =
                m_DebugEligibilitySamples;

            double reapplyAverageMs =
                DebugAverageMilliseconds(
                    m_DebugReapplyTotalTicks,
                    m_DebugReapplySamples);

            double reapplyMaxMs =
                DebugMilliseconds(m_DebugReapplyMaxTicks);

            int reapplySamples =
                m_DebugReapplySamples;

            double enforcementAverageMs =
                DebugAverageMilliseconds(
                    m_DebugEnforcementTotalTicks,
                    m_DebugEnforcementSamples);

            double enforcementMaxMs =
                DebugMilliseconds(m_DebugEnforcementMaxTicks);

            int enforcementSamples =
                m_DebugEnforcementSamples;

            string performanceText =
                $", perfMs[" +
                $"eligibility={eligibilityAverageMs:F3} avg/{eligibilityMaxMs:F3} max n={eligibilitySamples}, " +
                $"reapply={reapplyAverageMs:F3} avg/{reapplyMaxMs:F3} max n={reapplySamples}, " +
                $"rideNeeder={enforcementAverageMs:F3} avg/{enforcementMaxMs:F3} max n={enforcementSamples}]";
#else
            const string performanceText = "";
#endif

            CS2Shared.RiverMochi.LogUtils.Info(
                Mod.s_Log,
                () =>
                    $"{Mod.ModTag} TaxiSummary: " +

                    $"settings[residentsAvoid={setting.ResidentsAvoidTaxis}%, " +
                    $"commutersAvoid={setting.BlockCommuters}, " +
                    $"touristsAvoid={setting.BlockTourists}], " +

                    $"fleet[total={s_StatusTaxisTotal}, " +
                    $"fromOC={s_StatusTaxiFromOutside}, " +
                    $"transporting={s_StatusTaxiTransporting}, " +
                    $"dispatched={s_StatusTaxiDispatched}, " +
                    $"returning={s_StatusTaxiReturning}], " +

                    $"blocking[activeCims={s_StatusActiveCimsTotal}, " +
                    $"owned={s_StatusOwnedBlocksTotal}, " +
                    $"ignoreTaxi={s_StatusResidentsIgnoreTaxi}], " +

                    $"control[rideNeedersStopped={s_StatusRideNeedersStoppedTotal}, " +
                    $"requestsIntercepted={s_StatusTaxiRequestsStoppedTotal}, " +
                    $"repathed={s_StatusTaxiWaitersRepathedTotal}], " +

                    $"dailyTaxi[citizen={dailyTaxiCitizen}, " +
                    $"tourist={dailyTaxiTourist}]" +
                    performanceText);

#if DEBUG
            ResetDebugPerformanceTimings();
#endif
        }
    }
}
