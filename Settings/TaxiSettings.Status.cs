// <copyright file="TaxiSettings.Status.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Settings/TaxiSettings.Status.cs
// Compact player Status; detailed diagnostics go to Write Status Report.

namespace TaxiTraffic
{
    using System.Text;
    using CS2Shared.RiverMochi; // LocaleUtils, LogUtils
    using Game.Settings;

    public sealed partial class TaxiSettings
    {
        // ---- Status ----

        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusReady))]
        [SettingsUISection(StatusTab, CityScanGroup)]
        public string StatusNotReadyCityScan
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();
                return string.Empty;
            }
        }

        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusReady))]
        [SettingsUISection(StatusTab, TaxiScanGroup)]
        public string StatusNotReadyTaxiScan
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();
                return string.Empty;
            }
        }

        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusReady))]
        [SettingsUISection(StatusTab, LastUpdateGroup)]
        public string StatusNotReadyLastUpdate
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();
                return string.Empty;
            }
        }

        // CITY TRANSIT

        [SettingsUISection(StatusTab, CityScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusMonthlyPassengers1
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusMonthlyPassengers1),
                    TaxiTrafficSystem.s_InfoTaxiCitizen,
                    TaxiTrafficSystem.s_InfoBusCitizen,
                    TaxiTrafficSystem.s_InfoTramCitizen,
                    TaxiTrafficSystem.s_InfoTrainCitizen,
                    TaxiTrafficSystem.s_InfoSubwayCitizen,
                    TaxiTrafficSystem.s_InfoAirCitizen);
            }
        }

        [SettingsUISection(StatusTab, CityScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusMonthlyTourists
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusMonthlyTourists),
                    TaxiTrafficSystem.s_InfoTaxiTourist,
                    TaxiTrafficSystem.s_InfoBusTourist,
                    TaxiTrafficSystem.s_InfoTramTourist,
                    TaxiTrafficSystem.s_InfoTrainTourist,
                    TaxiTrafficSystem.s_InfoSubwayTourist,
                    TaxiTrafficSystem.s_InfoAirTourist);
            }
        }

        [SettingsUISection(StatusTab, CityScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusMonthlyTotal
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusMonthlyTotal),
                    TaxiTrafficSystem.s_StatusWaitingTransportTotal,
                    TaxiTrafficSystem.s_InfoTotalTourist,
                    TaxiTrafficSystem.s_InfoTotalCitizen);
            }
        }

        // TAXI SCAN — only player-useful rows stay visible.

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusTaxiSupply
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusTaxiSupply),
                    TaxiTrafficSystem.s_StatusTaxisTotal,
                    TaxiTrafficSystem.s_StatusTaxiFromOutside,
                    TaxiTrafficSystem.s_StatusTaxiDepotsLocal,
                    TaxiTrafficSystem.s_StatusTaxiDepotsOutside,
                    TaxiTrafficSystem.s_StatusTaxiStandsTotal);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusPassengers
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusPassengers),
                    TaxiTrafficSystem.s_StatusPassengerTotal,
                    TaxiTrafficSystem.s_StatusPassengerBlockedMark,
                    TaxiTrafficSystem.s_StatusPassengerHasResident);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusOutsideControl
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusOutsideControl),
                    TaxiTrafficSystem.s_StatusTaxiFromOutside,
                    TaxiTrafficSystem.s_StatusOutsideTaxiBlockedTotal);
            }
        }

        // LAST UPDATE — combine related information to keep the page short.

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusCoverage1
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusCoverage1),
                    TaxiTrafficSystem.s_StatusResidentsForcedMarker,
                    TaxiTrafficSystem.s_StatusResidentsTotal,
                    TaxiTrafficSystem.s_StatusCommutersBlockedMark,
                    TaxiTrafficSystem.s_StatusCommutersTotal,
                    TaxiTrafficSystem.s_StatusTouristsBlockedMark,
                    TaxiTrafficSystem.s_StatusTouristsTotal);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusWorkDone1
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusWorkDone1),
                    TaxiTrafficSystem.s_StatusLastAppliedIgnoreTaxi,
                    TaxiTrafficSystem.s_StatusLastRemovedRideNeeder,
                    TaxiTrafficSystem.s_StatusLastClearedTaxiLaneWaiting,
                    TaxiTrafficSystem.s_StatusLastClearedTaxiStandWaiting);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusSnapshotMeta
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusSnapshotMeta),
                    TaxiTrafficSystem.GetStatusLastStampText());
            }
        }

        // Detailed-only rows. They remain available to BuildStatusReportText()
        // without making the normal Status page scan or display them.

        [SettingsUIHidden]
        public string StatusRequests =>
            StatusValue(
                nameof(StatusRequests),
                TaxiTrafficSystem.s_StatusReqCustomer,
                TaxiTrafficSystem.s_StatusReqCustomerSeekerBlockedMark,
                TaxiTrafficSystem.s_StatusReqOutsideRider,
                TaxiTrafficSystem.s_StatusReqOutsideSeekerBlockedMark,
                TaxiTrafficSystem.s_StatusReqNone,
                TaxiTrafficSystem.s_StatusReqOutsideSupply,
                TaxiTrafficSystem.s_StatusReqStand);

        [SettingsUIHidden]
        public string StatusTaxiFleet =>
            StatusValue(
                nameof(StatusTaxiFleet),
                TaxiTrafficSystem.s_StatusTaxiTransporting,
                TaxiTrafficSystem.s_StatusTaxiBoarding,
                TaxiTrafficSystem.s_StatusTaxiReturning,
                TaxiTrafficSystem.s_StatusTaxiDispatched,
                TaxiTrafficSystem.s_StatusTaxiEnRoute,
                TaxiTrafficSystem.s_StatusTaxiParked);

        [SettingsUIHidden]
        public string StatusTaxiStands =>
            StatusValue(
                nameof(StatusTaxiStands),
                TaxiTrafficSystem.s_StatusWaitingTaxiStandTotal);

        [SettingsUIHidden]
        public string StatusCoverage2 =>
            StatusValue(
                nameof(StatusCoverage2),
                TaxiTrafficSystem.s_StatusCommutersBlockedMark,
                TaxiTrafficSystem.s_StatusCommutersTotal,
                TaxiTrafficSystem.s_StatusTouristsBlockedMark,
                TaxiTrafficSystem.s_StatusTouristsTotal);

        [SettingsUIHidden]
        public string StatusWorkDone2 =>
            StatusValue(
                nameof(StatusWorkDone2),
                TaxiTrafficSystem.s_StatusLastClearedTaxiStandWaiting,
                TaxiTrafficSystem.s_StatusLastSkippedCommuters,
                TaxiTrafficSystem.s_StatusLastSkippedTourists);

        [SettingsUIHidden]
        public string StatusGroupSafety =>
            StatusValue(
                nameof(StatusGroupSafety),
                TaxiTrafficSystem.s_StatusResidentsGroupLinked,
                TaxiTrafficSystem.s_StatusResidentsGroupAllowedMarker,
                TaxiTrafficSystem.s_StatusGroupRepairsTotal);

#if DEBUG
        // Advanced Debug remains DEV-only.

        [SettingsUISection(StatusTab, AdvancedDebugGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusDebugMarkedCoverage
        {
            get
            {
                TaxiTrafficSystem.EnsureDetailedStatusSnapshot();

                return StatusValue(
                    nameof(StatusDebugMarkedCoverage),
                    TaxiTrafficSystem.s_StatusResidentsForcedMarker,
                    TaxiTrafficSystem.s_StatusResidentsTotal,
                    TaxiTrafficSystem.s_StatusResidentsIgnoreTaxi,
                    TaxiTrafficSystem.s_StatusResidentsAllowedMarker,
                    TaxiTrafficSystem.s_StatusResidentsGroupAllowedMarker);
            }
        }

        [SettingsUISection(StatusTab, AdvancedDebugGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusDebugTaxiFlags
        {
            get
            {
                TaxiTrafficSystem.EnsureDetailedStatusSnapshot();

                return StatusValue(
                    nameof(StatusDebugTaxiFlags),
                    TaxiTrafficSystem.s_StatusTaxiWithDispatchBuffer,
                    TaxiTrafficSystem.s_StatusTaxiFromOutside,
                    TaxiTrafficSystem.s_StatusTaxiDisabled);
            }
        }
#endif

        [SettingsUIButtonGroup(StatusActionsGroup)]
        [SettingsUIButton]
        [SettingsUISection(StatusTab, StatusActionsGroup)]
        public bool WriteStatusReportToLog
        {
            set
            {
                if (!value)
                    return;

                TaxiTrafficSystem.RefreshStatusSnapshotForReport();
                LogUtils.Info(Mod.s_Log, BuildStatusReportText);
            }
        }

        private static string BuildStatusReportText()
        {
            StringBuilder sb = new();

            sb.AppendLine($"{Mod.ModTag} StatusReport:");

            sb.AppendLine("Citizens: " + StatusValue(
                nameof(StatusMonthlyPassengers1),
                TaxiTrafficSystem.s_InfoTaxiCitizen,
                TaxiTrafficSystem.s_InfoBusCitizen,
                TaxiTrafficSystem.s_InfoTramCitizen,
                TaxiTrafficSystem.s_InfoTrainCitizen,
                TaxiTrafficSystem.s_InfoSubwayCitizen,
                TaxiTrafficSystem.s_InfoAirCitizen));

            sb.AppendLine("Tourists: " + StatusValue(
                nameof(StatusMonthlyTourists),
                TaxiTrafficSystem.s_InfoTaxiTourist,
                TaxiTrafficSystem.s_InfoBusTourist,
                TaxiTrafficSystem.s_InfoTramTourist,
                TaxiTrafficSystem.s_InfoTrainTourist,
                TaxiTrafficSystem.s_InfoSubwayTourist,
                TaxiTrafficSystem.s_InfoAirTourist));

            sb.AppendLine("Totals: " + StatusValue(
                nameof(StatusMonthlyTotal),
                TaxiTrafficSystem.s_StatusWaitingTransportTotal,
                TaxiTrafficSystem.s_InfoTotalTourist,
                TaxiTrafficSystem.s_InfoTotalCitizen));

            sb.AppendLine("Taxi supply: " + StatusValue(
                nameof(StatusTaxiSupply),
                TaxiTrafficSystem.s_StatusTaxisTotal,
                TaxiTrafficSystem.s_StatusTaxiFromOutside,
                TaxiTrafficSystem.s_StatusTaxiDepotsLocal,
                TaxiTrafficSystem.s_StatusTaxiDepotsOutside,
                TaxiTrafficSystem.s_StatusTaxiStandsTotal));

            sb.AppendLine(
                "Taxi source detail: " +
                $"{TaxiTrafficSystem.s_StatusTaxiDepotsLocal} local | " +
                $"{TaxiTrafficSystem.s_StatusTaxiDepotsOutside} OC taxi source | " +
                $"{TaxiTrafficSystem.s_StatusTaxiDepotsTotal} total | " +
                $"{TaxiTrafficSystem.s_StatusTaxiDepotsWithDispatchCenter} dispatch center");

            sb.AppendLine("Passengers: " + StatusValue(
                nameof(StatusPassengers),
                TaxiTrafficSystem.s_StatusPassengerTotal,
                TaxiTrafficSystem.s_StatusPassengerBlockedMark,
                TaxiTrafficSystem.s_StatusPassengerHasResident));

            sb.AppendLine("Outside control: " + StatusValue(
                nameof(StatusOutsideControl),
                TaxiTrafficSystem.s_StatusTaxiFromOutside,
                TaxiTrafficSystem.s_StatusOutsideTaxiBlockedTotal));

            sb.AppendLine("Requests: " + StatusValue(
                nameof(StatusRequests),
                TaxiTrafficSystem.s_StatusReqCustomer,
                TaxiTrafficSystem.s_StatusReqCustomerSeekerBlockedMark,
                TaxiTrafficSystem.s_StatusReqOutsideRider,
                TaxiTrafficSystem.s_StatusReqOutsideSeekerBlockedMark,
                TaxiTrafficSystem.s_StatusReqNone,
                TaxiTrafficSystem.s_StatusReqOutsideSupply,
                TaxiTrafficSystem.s_StatusReqStand));

            sb.AppendLine("Fleet: " + StatusValue(
                nameof(StatusTaxiFleet),
                TaxiTrafficSystem.s_StatusTaxiTransporting,
                TaxiTrafficSystem.s_StatusTaxiBoarding,
                TaxiTrafficSystem.s_StatusTaxiReturning,
                TaxiTrafficSystem.s_StatusTaxiDispatched,
                TaxiTrafficSystem.s_StatusTaxiEnRoute,
                TaxiTrafficSystem.s_StatusTaxiParked));

            sb.AppendLine("Stands: " + StatusValue(
                nameof(StatusTaxiStands),
                TaxiTrafficSystem.s_StatusWaitingTaxiStandTotal));

            sb.AppendLine("Coverage: " + StatusValue(
                nameof(StatusCoverage1),
                TaxiTrafficSystem.s_StatusResidentsForcedMarker,
                TaxiTrafficSystem.s_StatusResidentsTotal,
                TaxiTrafficSystem.s_StatusCommutersBlockedMark,
                TaxiTrafficSystem.s_StatusCommutersTotal,
                TaxiTrafficSystem.s_StatusTouristsBlockedMark,
                TaxiTrafficSystem.s_StatusTouristsTotal));

            sb.AppendLine("Groups: " + StatusValue(
                nameof(StatusGroupSafety),
                TaxiTrafficSystem.s_StatusResidentsGroupLinked,
                TaxiTrafficSystem.s_StatusResidentsGroupAllowedMarker,
                TaxiTrafficSystem.s_StatusGroupRepairsTotal));

            sb.AppendLine(
                "Debug flags: " +
                TaxiTrafficSystem.s_StatusResidentsIgnoreTaxi + "/" +
                TaxiTrafficSystem.s_StatusResidentsTotal +
                " IgnoreTaxi now | " +
                TaxiTrafficSystem.s_StatusResidentsAllowedMarker +
                " allowed mark | " +
                TaxiTrafficSystem.s_StatusResidentsGroupAllowedMarker +
                " group exempt mark | " +
                TaxiTrafficSystem.s_StatusReqCustomerSeekerIgnoreTaxi + "/" +
                TaxiTrafficSystem.s_StatusReqCustomerSeekerHasResident +
                " city request seekers IgnoreTaxi | " +
                TaxiTrafficSystem.s_StatusPassengerIgnoreTaxi + "/" +
                TaxiTrafficSystem.s_StatusPassengerHasResident +
                " resident passengers IgnoreTaxi");

            sb.AppendLine("Cleanup: " + StatusValue(
                nameof(StatusWorkDone1),
                TaxiTrafficSystem.s_StatusLastAppliedIgnoreTaxi,
                TaxiTrafficSystem.s_StatusLastRemovedRideNeeder,
                TaxiTrafficSystem.s_StatusLastClearedTaxiLaneWaiting,
                TaxiTrafficSystem.s_StatusLastClearedTaxiStandWaiting));

            sb.AppendLine(
                "Skipped: " +
                TaxiTrafficSystem.s_StatusLastSkippedCommuters +
                " commuter | " +
                TaxiTrafficSystem.s_StatusLastSkippedTourists +
                " tourist | " +
                TaxiTrafficSystem.s_StatusLastSkippedGroupTravelers +
                " group traveler");

            sb.Append("Snapshot: ");
            sb.Append(StatusValue(
                nameof(StatusSnapshotMeta),
                TaxiTrafficSystem.GetStatusLastStampText()));

            return sb.ToString();
        }

        private static string StatusValue(
            string propertyName,
            params object[] args)
        {
            string entryId = GetStatusValueEntryId(propertyName);

            // Missing LocaleEN keys stay obvious during testing.
            return LocaleUtils.SafeFormat(entryId, entryId, args);
        }

        private static string GetStatusValueEntryId(string propertyName)
        {
            return propertyName switch
            {
                nameof(StatusMonthlyPassengers1) => LocaleEN.KeyStatusCitizensLine,
                nameof(StatusMonthlyTourists) => LocaleEN.KeyStatusTouristsLine,
                nameof(StatusMonthlyTotal) => LocaleEN.KeyStatusTotalsLine,
                nameof(StatusTaxiSupply) => LocaleEN.KeyStatusTaxiSupplyLine,
                nameof(StatusPassengers) => LocaleEN.KeyStatusPassengersLine,
                nameof(StatusRequests) => LocaleEN.KeyStatusRequestsLine,
                nameof(StatusOutsideControl) => LocaleEN.KeyStatusOutsideControlLine,
                nameof(StatusTaxiFleet) => LocaleEN.KeyStatusTaxiFleetLine,
                nameof(StatusTaxiStands) => LocaleEN.KeyStatusTaxiStandsLine,
                nameof(StatusCoverage1) => LocaleEN.KeyStatusCoverageLine,
                nameof(StatusCoverage2) => LocaleEN.KeyStatusCoverageGroupsLine,
                nameof(StatusWorkDone1) => LocaleEN.KeyStatusWorkDoneLine,
                nameof(StatusWorkDone2) => LocaleEN.KeyStatusWorkDone2Line,
                nameof(StatusGroupSafety) => LocaleEN.KeyStatusGroupSafetyLine,
                nameof(StatusSnapshotMeta) => LocaleEN.KeyStatusSnapshotLine,
#if DEBUG
                nameof(StatusDebugMarkedCoverage) => LocaleEN.KeyStatusMarkedDevLine,
                nameof(StatusDebugTaxiFlags) => LocaleEN.KeyStatusTaxiFlagsDevLine,
#endif
                _ => propertyName
            };
        }
    }
}
