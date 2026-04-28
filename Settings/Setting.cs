// File: Settings/Setting.cs
// Options UI for "Cim Be Smart".
// All user-facing strings are in lang/LocaleEN.cs.

namespace RiderControl
{
    using Colossal.IO.AssetDatabase;
    using Game.Modding;
    using Game.Settings;
    using Game.UI;
    using System;
    using UnityEngine;

#if DEBUG
    [FileLocation("ModsSettings/CimBeSmart/CimBeSmart")]
    [SettingsUITabOrder(ActionsTab, StatusTab, AboutTab)]
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
    [SettingsUITabOrder(ActionsTab, StatusTab, AboutTab)]
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

        internal const int TaxiAllowedPercentMin = 0;
        internal const int TaxiAllowedPercentMax = 100;
        internal const int TaxiAllowedPercentStep = 25;
        internal const int TaxiAllowedPercentDefault = 0;

        private const string UrlParadox =  
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";
 
        private const string UrlDiscord = "https://discord.gg/HTav7ARPs2";

        private int m_ResidentsAllowedToUseTaxis = TaxiAllowedPercentDefault;

        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        // ---- Actions ----

        [SettingsUISlider(
            min = TaxiAllowedPercentMin,
            max = TaxiAllowedPercentMax,
            step = TaxiAllowedPercentStep,
            scalarMultiplier = 1,
            unit = Unit.kPercentage)]
        [SettingsUISection(ActionsTab, BehaviorGroup)]
        [SettingsUISetter(typeof(Setting), nameof(OnTaxiEligibilitySliderChanged))]
        public int ResidentsAllowedToUseTaxis
        {
            get => m_ResidentsAllowedToUseTaxis;
            set => m_ResidentsAllowedToUseTaxis = SnapTaxiAllowedPercent(value);
        }

        [SettingsUISection(ActionsTab, BehaviorGroup)]
        [SettingsUISetter(typeof(Setting), nameof(OnTaxiEligibilityToggleChanged))]
        public bool BlockCommuters
        {
            get; set;
        }

        [SettingsUISection(ActionsTab, BehaviorGroup)]
        [SettingsUISetter(typeof(Setting), nameof(OnTaxiEligibilityToggleChanged))]
        public bool BlockTourists
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
            ResidentsAllowedToUseTaxis = TaxiAllowedPercentDefault;

            BlockCommuters = true;
            BlockTourists = true;

            FixMovingAwayHighwayWalkers = false;

            EnableDebugLogging = false;
        }

        public bool IsStatusReady()
        {
            return RiderControlSystem.s_StatusLastSnapshotRealtime > 0.0;
        }

        public bool IsStatusNotReady()
        {
            return !IsStatusReady();
        }

        // SettingsUISetter callback: refresh Status after slider changes.
        private void OnTaxiEligibilitySliderChanged(int _)
        {
            RiderControlSystem.RequestStatusRefresh(force: true);
        }

        // SettingsUISetter callback: refresh Status after commuter/tourist filter changes.
        private void OnTaxiEligibilityToggleChanged(bool _)
        {
            RiderControlSystem.RequestStatusRefresh(force: true);
        }

        private static int SnapTaxiAllowedPercent(int value)
        {
            int clamped = Math.Max(TaxiAllowedPercentMin, Math.Min(TaxiAllowedPercentMax, value));
            int snapped = ((clamped + (TaxiAllowedPercentStep / 2)) / TaxiAllowedPercentStep) * TaxiAllowedPercentStep;

            return Math.Max(TaxiAllowedPercentMin, Math.Min(TaxiAllowedPercentMax, snapped));
        }
    }
}
