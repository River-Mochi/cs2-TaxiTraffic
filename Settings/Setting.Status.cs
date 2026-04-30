// File: Settings/Setting.Status.cs
// Status tab Options UI rows for "Cim Be Smart".
// Status row text is formatted from lang/LocaleEN.cs entries.

namespace RiderControl
{
    using CS2Shared.RiverMochi; // LogUtils
    using Game.Settings;

    public sealed partial class Setting
    {
        // ---- Status ----

        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusReady))]
        [SettingsUISection(StatusTab, CityScanGroup)]
        public string StatusNotReadyCityScan
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();
                return string.Empty;
            }
        }

        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusReady))]
        [SettingsUISection(StatusTab, TaxiScanGroup)]
        public string StatusNotReadyTaxiScan
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();
                return string.Empty;
            }
        }

        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusReady))]
        [SettingsUISection(StatusTab, LastUpdateGroup)]
        public string StatusNotReadyLastUpdate
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();
                return string.Empty;
            }
        }

        // CITY SCAN

        [SettingsUISection(StatusTab, CityScanGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusMonthlyPassengers1
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusMonthlyPassengers1),
                    RiderControlSystem.s_InfoTaxiCitizen,
                    RiderControlSystem.s_InfoBusCitizen,
                    RiderControlSystem.s_InfoTramCitizen,
                    RiderControlSystem.s_InfoTrainCitizen,
                    RiderControlSystem.s_InfoSubwayCitizen,
                    RiderControlSystem.s_InfoAirCitizen);
            }
        }

        [SettingsUISection(StatusTab, CityScanGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusMonthlyTourists
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusMonthlyTourists),
                    RiderControlSystem.s_InfoTaxiTourist,
                    RiderControlSystem.s_InfoBusTourist,
                    RiderControlSystem.s_InfoTramTourist,
                    RiderControlSystem.s_InfoTrainTourist,
                    RiderControlSystem.s_InfoSubwayTourist,
                    RiderControlSystem.s_InfoAirTourist);
            }
        }

        [SettingsUISection(StatusTab, CityScanGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusMonthlyTotal
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusMonthlyTotal),
                    RiderControlSystem.s_StatusWaitingTransportTotal,
                    RiderControlSystem.s_InfoTotalTourist,
                    RiderControlSystem.s_InfoTotalCitizen);
            }
        }

        // TAXI SCAN

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusTaxiSupply
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusTaxiSupply),
                    RiderControlSystem.s_StatusTaxisTotal,
                    RiderControlSystem.s_StatusTaxiDepotsTotal,
                    RiderControlSystem.s_StatusTaxiDepotsWithDispatchCenter,
                    RiderControlSystem.s_StatusTaxiStandsTotal);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusPassengers
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusPassengers),
                    RiderControlSystem.s_StatusPassengerTotal,
                    RiderControlSystem.s_StatusPassengerIgnoreTaxi,
                    RiderControlSystem.s_StatusPassengerHasResident);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusRequests
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusRequests),
                    RiderControlSystem.s_StatusReqCustomer,
                    RiderControlSystem.s_StatusReqOutside,
                    RiderControlSystem.s_StatusReqNone,
                    RiderControlSystem.s_StatusReqStand);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusTaxiFleet
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusTaxiFleet),
                    RiderControlSystem.s_StatusTaxiTransporting,
                    RiderControlSystem.s_StatusTaxiBoarding,
                    RiderControlSystem.s_StatusTaxiReturning,
                    RiderControlSystem.s_StatusTaxiDispatched,
                    RiderControlSystem.s_StatusTaxiEnRoute,
                    RiderControlSystem.s_StatusTaxiParked);
            }
        }

        [SettingsUISection(StatusTab, TaxiScanGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusTaxiStands
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusTaxiStands),
                    RiderControlSystem.s_StatusWaitingTaxiStandTotal,
                    RiderControlSystem.s_StatusReqStand);
            }
        }

        // LAST UPDATE

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusCoverage1
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusCoverage1),
                    RiderControlSystem.s_StatusResidentsIgnoreTaxi,
                    RiderControlSystem.s_StatusResidentsTotal);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusCoverage2
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusCoverage2),
                    RiderControlSystem.s_StatusCommutersIgnoreTaxi,
                    RiderControlSystem.s_StatusCommutersTotal,
                    RiderControlSystem.s_StatusTouristsIgnoreTaxi,
                    RiderControlSystem.s_StatusTouristsTotal);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusWorkDone1
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusWorkDone1),
                    RiderControlSystem.s_StatusLastAppliedIgnoreTaxi,
                    RiderControlSystem.s_StatusLastRemovedRideNeeder,
                    RiderControlSystem.s_StatusLastClearedTaxiLaneWaiting);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusWorkDone2
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusWorkDone2),
                    RiderControlSystem.s_StatusLastClearedTaxiStandWaiting,
                    RiderControlSystem.s_StatusLastSkippedCommuters,
                    RiderControlSystem.s_StatusLastSkippedTourists);
            }
        }

        [SettingsUISection(StatusTab, LastUpdateGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusSnapshotMeta
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusSnapshotMeta),
                    RiderControlSystem.GetStatusLastStampText());
            }
        }

#if DEBUG
        // ---- Status → Advanced Debug (DEV builds only) ----

        [SettingsUISection(StatusTab, AdvancedDebugGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusDebugMarkedCoverage
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusDebugMarkedCoverage),
                    RiderControlSystem.s_StatusResidentsForcedMarker,
                    RiderControlSystem.s_StatusResidentsTotal,
                    RiderControlSystem.s_StatusResidentsIgnoreTaxi);
            }
        }

        [SettingsUISection(StatusTab, AdvancedDebugGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsStatusNotReady))]
        public string StatusDebugTaxiFlags
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead();

                return StatusValue(
                    nameof(StatusDebugTaxiFlags),
                    RiderControlSystem.s_StatusTaxiWithDispatchBuffer,
                    RiderControlSystem.s_StatusTaxiFromOutside,
                    RiderControlSystem.s_StatusTaxiDisabled);
            }
        }
#endif

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
                nameof(StatusTaxiFleet) => LocaleEN.KeyStatusTaxiFleetLine,
                nameof(StatusTaxiStands) => LocaleEN.KeyStatusTaxiStandsLine,
                nameof(StatusCoverage1) => LocaleEN.KeyStatusCoverageLine,
                nameof(StatusCoverage2) => LocaleEN.KeyStatusCoverageGroupsLine,
                nameof(StatusWorkDone1) => LocaleEN.KeyStatusWorkDoneLine,
                nameof(StatusWorkDone2) => LocaleEN.KeyStatusWorkDone2Line,
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
