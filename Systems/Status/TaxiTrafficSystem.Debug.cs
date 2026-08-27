// <copyright file="TaxiTrafficSystem.Debug.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Status/TaxiTrafficSystem.Debug.cs
// Optional debug logging and DEBUG-only queue performance sampling.

namespace TaxiTraffic
{
    using System;             // Math
#if DEBUG
    using System.Diagnostics; // Stopwatch
#endif
    using Game.City;          // StatisticType, PassengerType

    public partial class TaxiTrafficSystem
    {
        private const float kDebugMinSummaryIntervalSeconds = 60f;
        private const double kDebugForceStatusRefreshMaxAgeSeconds = 30.0;

#if DEBUG
        private const double kDebugPerfLogIntervalSeconds = 120.0;

        private long m_DebugUnstickSamples;
        private double m_DebugUnstickTotalMs;
        private double m_DebugUnstickLastMs;
        private double m_DebugUnstickMaxMs;
        private int m_DebugUnstickLastScanned;
        private int m_DebugUnstickMaxScanned;
        private int m_DebugUnstickLastWaitingTransport;
        private int m_DebugUnstickLastTaxiQueue;
        private int m_DebugUnstickLastCleared;
        private int m_DebugUnstickClearedTotal;

        private int m_DebugTripSourceRepairsTotal;
        private double m_DebugLastPerfLogRealtime;
#endif

        private float m_DebugTimerSeconds;

        private void ResetDebugOnCityLoaded()
        {
            m_DebugTimerSeconds = 0f;

#if DEBUG
            m_DebugUnstickSamples = 0;
            m_DebugUnstickTotalMs = 0.0;
            m_DebugUnstickLastMs = 0.0;
            m_DebugUnstickMaxMs = 0.0;
            m_DebugUnstickLastScanned = 0;
            m_DebugUnstickMaxScanned = 0;
            m_DebugUnstickLastWaitingTransport = 0;
            m_DebugUnstickLastTaxiQueue = 0;
            m_DebugUnstickLastCleared = 0;
            m_DebugUnstickClearedTotal = 0;

            m_DebugTripSourceRepairsTotal = 0;
            m_DebugLastPerfLogRealtime =
                UnityEngine.Time.realtimeSinceStartupAsDouble;
#endif
        }

#if DEBUG
        private static long DebugGetTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }

        private void DebugRecordTripSourceRepairs(int repaired)
        {
            if (repaired > 0)
                m_DebugTripSourceRepairsTotal += repaired;
        }

        private void DebugRecordUnstickTaxiQueues(
            long startTicks,
            int scanned,
            int waitingTransport,
            int taxiQueue,
            int cleared)
        {
            long elapsedTicks = Stopwatch.GetTimestamp() - startTicks;
            double elapsedMs =
                elapsedTicks * 1000.0 / Stopwatch.Frequency;

            m_DebugUnstickSamples++;
            m_DebugUnstickTotalMs += elapsedMs;
            m_DebugUnstickLastMs = elapsedMs;
            m_DebugUnstickMaxMs =
                Math.Max(m_DebugUnstickMaxMs, elapsedMs);

            m_DebugUnstickLastScanned = scanned;
            m_DebugUnstickMaxScanned =
                Math.Max(m_DebugUnstickMaxScanned, scanned);

            m_DebugUnstickLastWaitingTransport = waitingTransport;
            m_DebugUnstickLastTaxiQueue = taxiQueue;
            m_DebugUnstickLastCleared = cleared;
            m_DebugUnstickClearedTotal += cleared;

            TaxiSettings? setting = Mod.Setting;
            if (setting is null || !setting.EnableDebugLogging)
                return;

            double now =
                UnityEngine.Time.realtimeSinceStartupAsDouble;

            if (now - m_DebugLastPerfLogRealtime <
                kDebugPerfLogIntervalSeconds)
            {
                return;
            }

            m_DebugLastPerfLogRealtime = now;

            double averageMs =
                m_DebugUnstickSamples > 0
                    ? m_DebugUnstickTotalMs / m_DebugUnstickSamples
                    : 0.0;

            CS2Shared.RiverMochi.LogUtils.Info(
                Mod.s_Log,
                () =>
                    $"{Mod.ModTag} TaxiPerf: queueSweep " +
                    $"lastMs={m_DebugUnstickLastMs:F3}, " +
                    $"avgMs={averageMs:F3}, " +
                    $"maxMs={m_DebugUnstickMaxMs:F3}, " +
                    $"sweeps={m_DebugUnstickSamples}, " +
                    $"scanned={m_DebugUnstickLastScanned}, " +
                    $"waiting={m_DebugUnstickLastWaitingTransport}, " +
                    $"taxiQueue={m_DebugUnstickLastTaxiQueue}, " +
                    $"clearedLast={m_DebugUnstickLastCleared}, " +
                    $"clearedTotal={m_DebugUnstickClearedTotal}, " +
                    $"tripRepairsTotal={m_DebugTripSourceRepairsTotal}");
        }
#endif

        private void TickDebugLogging(
            TaxiSettings setting,
            float intervalSeconds)
        {
            float effectiveIntervalSeconds =
                Math.Max(intervalSeconds, kDebugMinSummaryIntervalSeconds);

            m_DebugTimerSeconds += UnityEngine.Time.unscaledDeltaTime;
            if (m_DebugTimerSeconds < effectiveIntervalSeconds)
                return;

            m_DebugTimerSeconds = 0f;

#if DEBUG
            if (IsStatusSnapshotStale(
                kDebugForceStatusRefreshMaxAgeSeconds))
            {
                RefreshStatusSnapshotForDebug();
            }
#else
            if (IsStatusSnapshotStale(
                kDebugForceStatusRefreshMaxAgeSeconds))
            {
                RefreshStatusSnapshotForOptionsUi(force: true);
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

            CS2Shared.RiverMochi.LogUtils.Info(
                Mod.s_Log,
                () =>
                    $"{Mod.ModTag} TaxiSummary: " +

                    $"settings[households={setting.ResidentsAllowedToUseTaxis}%, " +
                    $"commutersBlocked={setting.BlockCommuters}, " +
                    $"touristsBlocked={setting.BlockTourists}, " +
                    $"outsideBlocked={setting.BlockOutsideTaxis}], " +

                    $"fleet[total={s_StatusTaxisTotal}, " +
                    $"fromOC={s_StatusTaxiFromOutside}, " +
                    $"transporting={s_StatusTaxiTransporting}, " +
                    $"dispatched={s_StatusTaxiDispatched}, " +
                    $"returning={s_StatusTaxiReturning}, " +
                    $"parked={s_StatusTaxiParked}], " +

                    $"sources[localDepots={s_StatusTaxiDepotsLocal}, " +
                    $"ocSources={s_StatusTaxiDepotsOutside}], " +

                    $"requests[customer={s_StatusReqCustomer}, " +
                    $"outsideRider={s_StatusReqOutsideRider}, " +
                    $"outsideSupply={s_StatusReqOutsideSupply}, " +
                    $"stand={s_StatusReqStand}], " +

                    $"outside[blockedSinceLoad={s_StatusOutsideTaxiBlockedTotal}], " +

                    $"eligibility[blocked={s_StatusResidentsForcedMarker}/{s_StatusResidentsTotal}, " +
                    $"ignoreTaxi={s_StatusResidentsIgnoreTaxi}/{s_StatusResidentsTotal}, " +
                    $"normalAllowed={s_StatusResidentsAllowedMarker}, " +
                    $"groupExempt={s_StatusResidentsGroupAllowedMarker}/{s_StatusResidentsGroupLinked}, " +
                    $"groupIgnore={s_StatusResidentsGroupLinkedIgnoreTaxi}, " +
                    $"groupRepairs={s_StatusGroupRepairsTotal}, " +
                    $"commuterBlocked={s_StatusCommutersBlockedMark}/{s_StatusCommutersTotal}, " +
                    $"touristBlocked={s_StatusTouristsBlockedMark}/{s_StatusTouristsTotal}], " +

                    $"waiting[transport={s_StatusWaitingTransportTotal}, " +
                    $"taxiStand={s_StatusWaitingTaxiStandTotal}], " +

                    $"dailyTaxi[citizen={dailyTaxiCitizen}, " +
                    $"tourist={dailyTaxiTourist}]");
        }
    }
}
