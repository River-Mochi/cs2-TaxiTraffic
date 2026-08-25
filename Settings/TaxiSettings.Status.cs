// <copyright file="TaxiSettings.Status.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Settings/TaxiSettings.Status.cs
// Status-tab Options rows. Display text is formatted from Localization/LocaleEN.cs.

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

        // CITY SCAN

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

        // TAXI SCAN

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
                    TaxiTrafficSystem.s_StatusTaxiDepotsTotal,
                    TaxiTrafficSystem.s_StatusTaxiDepotsWithDispatchCenter,
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
        public string StatusRequests
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusRequests),
                    TaxiTrafficSystem.s_StatusReqCustomer,
                    TaxiTrafficSystem.s_StatusReqCustomerSeekerBlockedMark,
                    TaxiTrafficSystem.s_StatusReqOutsideRider,
                    TaxiTrafficSystem.s_StatusReqOutsideSeekerBlockedMark,
                    TaxiTrafficSystem.s_StatusReqNone,
                    TaxiTrafficSystem.s_StatusReqOutsideSupply,
                    TaxiTrafficSystem.s_StatusReqStand);
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
                    TaxiTrafficSystem.s_StatusReqOutsideSupply,
                    TaxiTrafficSystem.s_StatusOutsideSupplySuppressedTotal);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusTaxiFleet
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusTaxiFleet),
                    TaxiTrafficSystem.s_StatusTaxiTransporting,
                    TaxiTrafficSystem.s_StatusTaxiBoarding,
                    TaxiTrafficSystem.s_StatusTaxiReturning,
                    TaxiTrafficSystem.s_StatusTaxiDispatched,
                    TaxiTrafficSystem.s_StatusTaxiEnRoute,
                    TaxiTrafficSystem.s_StatusTaxiParked);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusTaxiStands
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusTaxiStands),
                    TaxiTrafficSystem.s_StatusWaitingTaxiStandTotal);
            }
        }

        // LAST UPDATE

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
                    TaxiTrafficSystem.s_StatusResidentsTotal);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusCoverage2
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusCoverage2),
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
                    TaxiTrafficSystem.s_StatusLastClearedTaxiLaneWaiting);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusWorkDone2
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusWorkDone2),
                    TaxiTrafficSystem.s_StatusLastClearedTaxiStandWaiting,
                    TaxiTrafficSystem.s_StatusLastSkippedCommuters,
                    TaxiTrafficSystem.s_StatusLastSkippedTourists);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusGroupSafety
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusGroupSafety),
                    TaxiTrafficSystem.s_StatusLastSkippedGroupTravelers);
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

#if DEBUG
        // ---- Status → Advanced Debug (DEV builds only) ----

        [SettingsUISection(StatusTab, AdvancedDebugGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusDebugMarkedCoverage
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusDebugMarkedCoverage),
                    TaxiTrafficSystem.s_StatusResidentsForcedMarker,
                    TaxiTrafficSystem.s_StatusResidentsTotal,
                    TaxiTrafficSystem.s_StatusResidentsIgnoreTaxi,
                    TaxiTrafficSystem.s_StatusResidentsAllowedMarker);
            }
        }

        [SettingsUISection(StatusTab, AdvancedDebugGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusDebugTaxiFlags
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

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

                TaxiTrafficSystem.RefreshStatusSnapshotForOptionsUi(force: true);
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
                TaxiTrafficSystem.s_StatusTaxiDepotsTotal,
                TaxiTrafficSystem.s_StatusTaxiDepotsWithDispatchCenter,
                TaxiTrafficSystem.s_StatusTaxiStandsTotal));

            sb.AppendLine("Passengers: " + StatusValue(
                nameof(StatusPassengers),
                TaxiTrafficSystem.s_StatusPassengerTotal,
                TaxiTrafficSystem.s_StatusPassengerBlockedMark,
                TaxiTrafficSystem.s_StatusPassengerHasResident));

            sb.AppendLine("Requests: " + StatusValue(
                nameof(StatusRequests),
                TaxiTrafficSystem.s_StatusReqCustomer,
                TaxiTrafficSystem.s_StatusReqCustomerSeekerBlockedMark,
                TaxiTrafficSystem.s_StatusReqOutsideRider,
                TaxiTrafficSystem.s_StatusReqOutsideSeekerBlockedMark,
                TaxiTrafficSystem.s_StatusReqNone,
                TaxiTrafficSystem.s_StatusReqOutsideSupply,
                TaxiTrafficSystem.s_StatusReqStand));

            sb.AppendLine("Outside control: " + StatusValue(
                nameof(StatusOutsideControl),
                TaxiTrafficSystem.s_StatusTaxiFromOutside,
                TaxiTrafficSystem.s_StatusReqOutsideSupply,
                TaxiTrafficSystem.s_StatusOutsideSupplySuppressedTotal));

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
                TaxiTrafficSystem.s_StatusResidentsTotal));

            sb.AppendLine("Commuter/tourist coverage: " + StatusValue(
                nameof(StatusCoverage2),
                TaxiTrafficSystem.s_StatusCommutersBlockedMark,
                TaxiTrafficSystem.s_StatusCommutersTotal,
                TaxiTrafficSystem.s_StatusTouristsBlockedMark,
                TaxiTrafficSystem.s_StatusTouristsTotal));

            sb.AppendLine("Groups: " + StatusValue(
                nameof(StatusGroupSafety),
                TaxiTrafficSystem.s_StatusLastSkippedGroupTravelers));

            sb.AppendLine(
                "Debug flags: " +
                TaxiTrafficSystem.s_StatusResidentsIgnoreTaxi + "/" + TaxiTrafficSystem.s_StatusResidentsTotal +
                " IgnoreTaxi now | " +
                TaxiTrafficSystem.s_StatusReqCustomerSeekerIgnoreTaxi + "/" + TaxiTrafficSystem.s_StatusReqCustomerSeekerHasResident +
                " city request seekers IgnoreTaxi | " +
                TaxiTrafficSystem.s_StatusPassengerIgnoreTaxi + "/" + TaxiTrafficSystem.s_StatusPassengerHasResident +
                " resident passengers IgnoreTaxi");

            sb.AppendLine("Work: " + StatusValue(
                nameof(StatusWorkDone1),
                TaxiTrafficSystem.s_StatusLastAppliedIgnoreTaxi,
                TaxiTrafficSystem.s_StatusLastRemovedRideNeeder,
                TaxiTrafficSystem.s_StatusLastClearedTaxiLaneWaiting));

            sb.AppendLine("Work 2: " + StatusValue(
                nameof(StatusWorkDone2),
                TaxiTrafficSystem.s_StatusLastClearedTaxiStandWaiting,
                TaxiTrafficSystem.s_StatusLastSkippedCommuters,
                TaxiTrafficSystem.s_StatusLastSkippedTourists));

            sb.Append("Snapshot: ");
            sb.Append(StatusValue(nameof(StatusSnapshotMeta), TaxiTrafficSystem.GetStatusLastStampText()));

            return sb.ToString();
        }

        private static string StatusValue(string propertyName, params object[] args)
        {
            string entryId = GetStatusValueEntryId(propertyName);

            // Fallback key makes missing LocaleEN entries obvious during testing.
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
