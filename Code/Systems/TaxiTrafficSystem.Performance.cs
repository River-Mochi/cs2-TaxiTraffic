// <copyright file="TaxiTrafficSystem.Performance.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// Only built with -p:TaxiPerfTest=true. Never included in normal Release.
#if TAXI_PERF_TEST
using System;
using CS2Shared.RiverMochi;

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem
    {
        private const double kTaxiPerfIntervalSeconds = 10.0;

        private double m_PerfLastSummaryRealtime;
        private long m_PerfFrames;
        private long m_PerfEligibilityVisited;
        private long m_PerfEligibilityApplied;
        private long m_PerfEligibilityRemoved;
        private long m_PerfEligibilityReapplied;
        private long m_PerfReapplyVisited;
        private long m_PerfRideNeederVisited;
        private long m_PerfRideNeederStopped;
        private long m_PerfTaxiRequestsIntercepted;

        private void ResetTaxiPerfOnCityLoaded()
        {
            m_PerfLastSummaryRealtime =
                UnityEngine.Time.realtimeSinceStartupAsDouble;
            ClearTaxiPerfCounters();
        }

        private void RecordTaxiPerfCounters(
            int eligibilityVisited,
            int eligibilityApplied,
            int eligibilityRemoved,
            int eligibilityReapplied,
            int reapplyVisited,
            int rideNeederVisited,
            int rideNeederStopped,
            int taxiRequestsIntercepted)
        {
            m_PerfFrames++;
            m_PerfEligibilityVisited += eligibilityVisited;
            m_PerfEligibilityApplied += eligibilityApplied;
            m_PerfEligibilityRemoved += eligibilityRemoved;
            m_PerfEligibilityReapplied += eligibilityReapplied;
            m_PerfReapplyVisited += reapplyVisited;
            m_PerfRideNeederVisited += rideNeederVisited;
            m_PerfRideNeederStopped += rideNeederStopped;
            m_PerfTaxiRequestsIntercepted += taxiRequestsIntercepted;
        }

        private void TickTaxiPerfLogging(
            TaxiSettings setting,
            bool residentControlActive)
        {
            // Sampling the clock every 64 sim frames keeps the hot path light.
            if ((m_ControlSimulationSystem.frameIndex & 63u) != 0u)
                return;

            double now = UnityEngine.Time.realtimeSinceStartupAsDouble;
            if (now - m_PerfLastSummaryRealtime < kTaxiPerfIntervalSeconds)
                return;

            m_PerfLastSummaryRealtime = now;

            LogUtils.Info(
                Mod.s_Log,
                () =>
                    "[TAXI-PERF] " +
                    $"control={(residentControlActive ? "ON" : "OFF")} " +
                    $"residentsAvoid={setting.ResidentsAvoidTaxis}% " +
                    $"commuters={setting.BlockCommuters} " +
                    $"tourists={setting.BlockTourists} " +
                    $"frames={m_PerfFrames} " +
                    $"eligibilityVisited={m_PerfEligibilityVisited} " +
                    $"eligibilityApplied={m_PerfEligibilityApplied} " +
                    $"eligibilityRemoved={m_PerfEligibilityRemoved} " +
                    $"eligibilityReapplied={m_PerfEligibilityReapplied} " +
                    $"reapplyVisited={m_PerfReapplyVisited} " +
                    $"rideNeedersVisited={m_PerfRideNeederVisited} " +
                    $"rideNeedersStopped={m_PerfRideNeederStopped} " +
                    $"taxiRequestsIntercepted={m_PerfTaxiRequestsIntercepted}");

            ClearTaxiPerfCounters();
        }

        private void ClearTaxiPerfCounters()
        {
            m_PerfFrames = 0;
            m_PerfEligibilityVisited = 0;
            m_PerfEligibilityApplied = 0;
            m_PerfEligibilityRemoved = 0;
            m_PerfEligibilityReapplied = 0;
            m_PerfReapplyVisited = 0;
            m_PerfRideNeederVisited = 0;
            m_PerfRideNeederStopped = 0;
            m_PerfTaxiRequestsIntercepted = 0;
        }
    }
}
#endif
