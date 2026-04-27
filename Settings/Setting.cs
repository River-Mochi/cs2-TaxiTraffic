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
        AboutInfoGroup,
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
        AboutInfoGroup,
        DebugGroup,
        AboutLinksGroup
    )]
#endif
    public sealed partial class Setting : ModSetting
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
                RiderControlSystem.AutoRequestStatusRefreshOnRead(); // Status update on Options-open.
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

        // Moved to About tab so normal gameplay options stay clean.
        [SettingsUISection(AboutTab, DebugGroup)]
        public bool EnableDebugLogging
        {
            get; set;
        }

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

        public bool IsStatusReady()
        {
            return RiderControlSystem.s_StatusLastSnapshotRealtime > 0.0;
        }

        public bool IsStatusNotReady()
        {
            return !IsStatusReady();
        }
    }
}
