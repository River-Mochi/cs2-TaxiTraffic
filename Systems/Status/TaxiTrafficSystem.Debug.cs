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
    using System;    // Math
    using Game.City; // StatisticType, PassengerType

    public partial class TaxiTrafficSystem
    {
        private const float kDebugMinSummaryIntervalSeconds = 60f;
        private const double kDebugForceStatusRefreshMaxAgeSeconds = 30.0;

        private float m_DebugTimerSeconds;

        private void ResetDebugOnCityLoaded()
        {
            m_DebugTimerSeconds = 0f;
        }

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
            if (IsStatusSnapshotStale(kDebugForceStatusRefreshMaxAgeSeconds))
                RefreshStatusSnapshotForDebug();
#else
            if (IsStatusSnapshotStale(kDebugForceStatusRefreshMaxAgeSeconds))
                RefreshStatusSnapshotForOptionsUi(force: true);
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

                    $"settings[residentsAvoid={setting.ResidentsAvoidTaxis}%, " +
                    $"commutersAvoid={setting.BlockCommuters}, " +
                    $"touristsAvoid={setting.BlockTourists}, " +
                    $"outsideBlocked={setting.BlockOutsideTaxis}], " +

                    $"fleet[total={s_StatusTaxisTotal}, " +
                    $"fromOC={s_StatusTaxiFromOutside}, " +
                    $"transporting={s_StatusTaxiTransporting}, " +
                    $"dispatched={s_StatusTaxiDispatched}, " +
                    $"returning={s_StatusTaxiReturning}, " +
                    $"parked={s_StatusTaxiParked}], " +

                    $"eligibility[ownedBlocked={s_StatusResidentsForcedMarker}/{s_StatusResidentsTotal}, " +
                    $"ignoreTaxi={s_StatusResidentsIgnoreTaxi}/{s_StatusResidentsTotal}, " +
                    $"commuterBlocked={s_StatusCommutersBlockedMark}/{s_StatusCommutersTotal}, " +
                    $"touristBlocked={s_StatusTouristsBlockedMark}/{s_StatusTouristsTotal}], " +

                    $"outside[blockedSinceLoad={s_StatusOutsideTaxiBlockedTotal}], " +

                    $"waiting[transport={s_StatusWaitingTransportTotal}, " +
                    $"taxiStand={s_StatusWaitingTaxiStandTotal}], " +

                    $"dailyTaxi[citizen={dailyTaxiCitizen}, " +
                    $"tourist={dailyTaxiTourist}]");
        }
    }
}
