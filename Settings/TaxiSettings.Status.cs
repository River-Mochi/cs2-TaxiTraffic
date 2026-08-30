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

        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(HideLastUpdateNotReady))]
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

        // TAXI SCAN
        // Keep the short player-facing order: passengers, supply, outside taxis, purpose.

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
                    TaxiTrafficSystem.s_StatusPassengerLocal);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusTaxiSupply
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusTaxiSupply),
                    TaxiTrafficSystem.s_StatusTaxiParkedNow,
                    TaxiTrafficSystem.s_StatusTaxiActiveNow,
                    TaxiTrafficSystem.s_StatusTaxiDepotsLocal,
                    TaxiTrafficSystem.s_StatusTaxiStandsTotal);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusOutsideTaxis
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusOutsideTaxis),
                    TaxiTrafficSystem.s_StatusTaxiFromOutside,
                    TaxiTrafficSystem.s_StatusTaxiDepotsOutside);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusTaxiPurpose
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusTaxiPurpose),
                    TaxiTrafficSystem.s_StatusReqPurposeLeisure,
                    TaxiTrafficSystem.s_StatusReqPurposeHome,
                    TaxiTrafficSystem.s_StatusReqPurposeWork,
                    TaxiTrafficSystem.s_StatusReqPurposeSchool,
                    TaxiTrafficSystem.s_StatusReqPurposeShopping,
                    TaxiTrafficSystem.s_StatusReqPurposeOther);
            }
        }

        // LAST UPDATE — hidden by default behind Show last update info.

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(HideLastUpdateRows))]
        public string StatusCoverage1
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusCoverage1),
                    TaxiTrafficSystem.s_StatusLocalBlockedMark,
                    TaxiTrafficSystem.s_StatusCommutersBlockedMark,
                    TaxiTrafficSystem.s_StatusTouristsBlockedMark);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(HideLastUpdateRows))]
        public string StatusWorkDone1
        {
            get
            {
                TaxiTrafficSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusWorkDone1),
                    TaxiTrafficSystem.s_StatusLastAppliedIgnoreTaxi,
                    TaxiTrafficSystem.s_StatusLastRemovedIgnoreTaxi,
                    TaxiTrafficSystem.s_StatusLastTaxiRequestsStopped);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(HideLastUpdateRows))]
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

        // Detailed-only rows used by the report.

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

#if DEBUG
        // Advanced Debug remains DEV-only on the Options page.

        [SettingsUISection(StatusTab, AdvancedDebugGroup)]
        [SettingsUIHideByCondition(typeof(TaxiSettings), nameof(IsStatusNotReady))]
        public string StatusDebugMarkedCoverage
        {
            get
            {
                TaxiTrafficSystem.EnsureDetailedStatusSnapshot();

                return StatusValue(
                    nameof(StatusDebugMarkedCoverage),
                    TaxiTrafficSystem.s_StatusActiveCimsTotal,
                    TaxiTrafficSystem.s_StatusOwnedBlocksTotal,
                    TaxiTrafficSystem.s_StatusResidentsIgnoreTaxi);
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

        private bool HideLastUpdateNotReady()
        {
            return !ShowLastUpdateInfo || IsStatusReady();
        }

        private bool HideLastUpdateRows()
        {
            return !ShowLastUpdateInfo || IsStatusNotReady();
        }

        private static string BuildStatusReportText()
        {
            StringBuilder sb = new();

            sb.AppendLine($"{Mod.ModTag} StatusReport:");

            TaxiSettings? setting = Mod.Setting;
            if (setting != null)
            {
                sb.AppendLine(
                    $"Settings: residents avoid {setting.ResidentsAvoidTaxis}% | " +
                    $"commuters avoid {setting.BlockCommuters} | " +
                    $"tourists avoid {setting.BlockTourists}");
            }

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

            sb.AppendLine("Current passengers: " + StatusValue(
                nameof(StatusPassengers),
                TaxiTrafficSystem.s_StatusPassengerTotal,
                TaxiTrafficSystem.s_StatusPassengerBlockedMark,
                TaxiTrafficSystem.s_StatusPassengerLocal));

            sb.AppendLine("Taxi supply: " + StatusValue(
                nameof(StatusTaxiSupply),
                TaxiTrafficSystem.s_StatusTaxiParkedNow,
                TaxiTrafficSystem.s_StatusTaxiActiveNow,
                TaxiTrafficSystem.s_StatusTaxiDepotsLocal,
                TaxiTrafficSystem.s_StatusTaxiStandsTotal));

            sb.AppendLine("Outside taxis: " + StatusValue(
                nameof(StatusOutsideTaxis),
                TaxiTrafficSystem.s_StatusTaxiFromOutside,
                TaxiTrafficSystem.s_StatusTaxiDepotsOutside));

            sb.AppendLine("Requests: " + StatusValue(
                nameof(StatusRequests),
                TaxiTrafficSystem.s_StatusReqCustomer,
                TaxiTrafficSystem.s_StatusReqCustomerSeekerBlockedMark,
                TaxiTrafficSystem.s_StatusReqOutsideRider,
                TaxiTrafficSystem.s_StatusReqOutsideSeekerBlockedMark,
                TaxiTrafficSystem.s_StatusReqNone,
                TaxiTrafficSystem.s_StatusReqOutsideSupply,
                TaxiTrafficSystem.s_StatusReqStand));

            sb.AppendLine("City request purpose: " + StatusValue(
                nameof(StatusTaxiPurpose),
                TaxiTrafficSystem.s_StatusReqPurposeLeisure,
                TaxiTrafficSystem.s_StatusReqPurposeHome,
                TaxiTrafficSystem.s_StatusReqPurposeWork,
                TaxiTrafficSystem.s_StatusReqPurposeSchool,
                TaxiTrafficSystem.s_StatusReqPurposeShopping,
                TaxiTrafficSystem.s_StatusReqPurposeOther));

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

            sb.AppendLine("Blocked now: " + StatusValue(
                nameof(StatusCoverage1),
                TaxiTrafficSystem.s_StatusLocalBlockedMark,
                TaxiTrafficSystem.s_StatusCommutersBlockedMark,
                TaxiTrafficSystem.s_StatusTouristsBlockedMark));

            sb.AppendLine(
                "Active cims: " +
                $"{TaxiTrafficSystem.s_StatusActiveCimsTotal} total | " +
                $"{TaxiTrafficSystem.s_StatusLocalCimsTotal} local | " +
                $"{TaxiTrafficSystem.s_StatusCommutersTotal} commuter | " +
                $"{TaxiTrafficSystem.s_StatusTouristsTotal} tourist");

            sb.AppendLine(
                "Debug flags: " +
                $"{TaxiTrafficSystem.s_StatusOwnedBlocksTotal} TaxiTraffic-owned block | " +
                $"{TaxiTrafficSystem.s_StatusResidentsIgnoreTaxi} IgnoreTaxi now | " +
                $"{TaxiTrafficSystem.s_StatusReqCustomerSeekerIgnoreTaxi}/" +
                $"{TaxiTrafficSystem.s_StatusReqCustomerSeekerHasResident} " +
                "city request seekers IgnoreTaxi | " +
                $"{TaxiTrafficSystem.s_StatusPassengerIgnoreTaxi}/" +
                $"{TaxiTrafficSystem.s_StatusPassengerHasResident} " +
                "resident passengers IgnoreTaxi");

            sb.AppendLine(
                "Control since load: " +
                $"{TaxiTrafficSystem.s_StatusRideNeedersStoppedTotal} RideNeeders stopped | " +
                $"{TaxiTrafficSystem.s_StatusTaxiRequestsStoppedTotal} existing taxi requests intercepted | " +
                $"{TaxiTrafficSystem.s_StatusTaxiWaitersRepathedTotal} taxi paths repathed");

            sb.AppendLine(
                "Last control pass: " +
                $"{TaxiTrafficSystem.s_StatusLastAppliedIgnoreTaxi} blocked | " +
                $"{TaxiTrafficSystem.s_StatusLastRemovedIgnoreTaxi} unblocked | " +
                $"{TaxiTrafficSystem.s_StatusLastReappliedIgnoreTaxi} re-applied | " +
                $"{TaxiTrafficSystem.s_StatusLastRideNeedersStopped} RideNeeders stopped | " +
                $"{TaxiTrafficSystem.s_StatusLastTaxiRequestsStopped} requests intercepted | " +
                $"{TaxiTrafficSystem.s_StatusLastTaxiWaitersRepathed} repathed");

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
            return LocaleUtils.SafeFormat(entryId, entryId, args);
        }

        private static string GetStatusValueEntryId(string propertyName)
        {
            return propertyName switch
            {
                nameof(StatusMonthlyPassengers1) => LocaleEN.KeyStatusCitizensLine,
                nameof(StatusMonthlyTourists) => LocaleEN.KeyStatusTouristsLine,
                nameof(StatusMonthlyTotal) => LocaleEN.KeyStatusTotalsLine,
                nameof(StatusPassengers) => LocaleEN.KeyStatusPassengersLine,
                nameof(StatusTaxiSupply) => LocaleEN.KeyStatusTaxiSupplyLine,
                nameof(StatusOutsideTaxis) => LocaleEN.KeyStatusOutsideTaxisLine,
                nameof(StatusTaxiPurpose) => LocaleEN.KeyStatusTaxiPurposeLine,
                nameof(StatusRequests) => LocaleEN.KeyStatusRequestsLine,
                nameof(StatusTaxiFleet) => LocaleEN.KeyStatusTaxiFleetLine,
                nameof(StatusTaxiStands) => LocaleEN.KeyStatusTaxiStandsLine,
                nameof(StatusCoverage1) => LocaleEN.KeyStatusCoverageLine,
                nameof(StatusWorkDone1) => LocaleEN.KeyStatusWorkDoneLine,
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
