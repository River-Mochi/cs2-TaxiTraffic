// File: Settings/Setting.cs
// Options UI for "Cim Be Smart".
// All user-facing strings are in lang/LocaleEN.cs.

namespace RiderControl
{
    using Colossal.IO.AssetDatabase;
    using Game.Modding;
    using Game.Settings;
    using System;
    using UnityEngine;

#if DEBUG
    [FileLocation("ModsSettings/CimBeSmart/CimBeSmart")]
    [SettingsUIGroupOrder(
        BehaviorGroup,
        CityScanGroup,
        TaxiScanGroup,
        LastUpdateGroup,
        AdvancedDebugGroup,
        AboutInfoGroup,
        DebugGroup,
        AboutLinksGroup
    )]
    [SettingsUIShowGroupName(
        BehaviorGroup,
        CityScanGroup,
        TaxiScanGroup,
        LastUpdateGroup,
        AdvancedDebugGroup,
        DebugGroup,
        AboutLinksGroup
    )]
#else
    [FileLocation("ModsSettings/CimBeSmart/CimBeSmart")]
    [SettingsUIGroupOrder(
        BehaviorGroup,
        CityScanGroup,
        TaxiScanGroup,
        LastUpdateGroup,
        AboutInfoGroup,
        DebugGroup,
        AboutLinksGroup
    )]
    [SettingsUIShowGroupName(
        BehaviorGroup,
        CityScanGroup,
        TaxiScanGroup,
        LastUpdateGroup,
        DebugGroup,
        AboutLinksGroup
    )]
#endif
    public sealed class Setting : ModSetting
    {
        public const string ActionsTab = "Actions";
        public const string StatusTab = "Status";
        public const string AboutTab = "About";

        public const string BehaviorGroup = "Behavior";
        public const string DebugGroup = "Debug";
        public const string CityScanGroup = "CityScan";
        public const string TaxiScanGroup = "TaxiScan";
        public const string LastUpdateGroup = "LastUpdate";

        // Status (DEBUG builds only)
        public const string AdvancedDebugGroup = "AdvancedDebug";

        public const string AboutInfoGroup = "Info";
        public const string AboutLinksGroup = "Support Links";

        private const string StatusValueLocalePrefix = "CBS.Status.Value.";

        private const string UrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        private const string UrlDiscord = "https://discord.gg/HTav7ARPs2";

        private bool m_BlockTaxiUsage = true;

        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        // ---- Actions ----

        [SettingsUISection(ActionsTab, BehaviorGroup)]
        public bool BlockTaxiUsage
        {
            get
            {
                RiderControlSystem.AutoRequestStatusRefreshOnRead(); // Status update on Options-open (throttled)
                return m_BlockTaxiUsage;
            }
            set
            {
                m_BlockTaxiUsage = value;

                if (!m_BlockTaxiUsage)
                {
                    BlockTaxiStandDemand = false;
                    BlockCommuters = false;
                    BlockTourists = false;
                }
            }
        }

        [SettingsUIHideByCondition(typeof(Setting), nameof(IsTaxiBlockingOff))]
        [SettingsUISection(ActionsTab, BehaviorGroup)]
        public bool BlockCommuters
        {
            get; set;
        }

        [SettingsUIHideByCondition(typeof(Setting), nameof(IsTaxiBlockingOff))]
        [SettingsUISection(ActionsTab, BehaviorGroup)]
        public bool BlockTourists
        {
            get; set;
        }

        // Alpha phase: disables TaxiStand-driven taxi demand by clearing TaxiStand WaitingPassengers.
        [SettingsUIHideByCondition(typeof(Setting), nameof(IsTaxiBlockingOff))]
        [SettingsUISection(ActionsTab, BehaviorGroup)]
        public bool BlockTaxiStandDemand
        {
            get; set;
        }

        // Optional toggle: prevent moving-away walking on highway to Outside connection.
        [SettingsUISection(ActionsTab, BehaviorGroup)]
        public bool FixMovingAwayHighwayWalkers
        {
            get; set;
        }

        public bool IsStatusReady()
        {
            return RiderControlSystem.s_StatusLastSnapshotRealtime > 0.0;
        }

        public bool IsStatusNotReady()
        {
            return !IsStatusReady();
        }

        // Moved to About tab (requested).
        [SettingsUISection(AboutTab, DebugGroup)]
        public bool EnableDebugLogging
        {
            get; set;
        }

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
                    RiderControlSystem.s_StatusReqNone);
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
                    RiderControlSystem.s_StatusResidentsIgnoreTaxi,
                    RiderControlSystem.s_StatusResidentsTotal);
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

        // ---- About ----

        [SettingsUISection(AboutTab, AboutInfoGroup)]
        public string NameDisplay => Mod.ModName;

        [SettingsUISection(AboutTab, AboutInfoGroup)]
        public string VersionDisplay => Mod.ModVersion;

        [SettingsUIButtonGroup(AboutLinksGroup)]
        [SettingsUIButton]
        [SettingsUISection(AboutTab, AboutLinksGroup)]
        public bool OpenParadoxMods
        {
            set
            {
                if (!value)
                    return;

                try
                {
                    Application.OpenURL(UrlParadox);
                }
                catch (Exception)
                {
                }
            }
        }

        [SettingsUIButtonGroup(AboutLinksGroup)]
        [SettingsUIButton]
        [SettingsUISection(AboutTab, AboutLinksGroup)]
        public bool OpenDiscord
        {
            set
            {
                if (!value)
                    return;

                try
                {
                    Application.OpenURL(UrlDiscord);
                }
                catch (Exception)
                {
                }
            }
        }

        public override void SetDefaults()
        {
            BlockTaxiUsage = true;
            BlockCommuters = true;
            BlockTourists = true;
            BlockTaxiStandDemand = true;

            FixMovingAwayHighwayWalkers = false;

            EnableDebugLogging = false;
        }

        // Used by SettingsUIHideByCondition.
        public bool IsTaxiBlockingOff()
        {
            return !BlockTaxiUsage;
        }

        private static string StatusValue(string propertyName, params object[] args)
        {
            string entryId = StatusValueLocalePrefix + propertyName;

            // Fallback is the locale key so missing locale entries are obvious during testing.
            return LocaleUtils.SafeFormat(entryId, entryId, args);
        }
    }
}
