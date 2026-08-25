// <copyright file="TaxiSettings.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Settings/TaxiSettings.cs
// Purpose: Options UI for "Taxi Traffic".
// All user-facing strings are in Localization/LocaleEN.cs.

namespace TaxiTraffic
{
    using System;
    using Colossal.IO.AssetDatabase;
    using CS2Shared.RiverMochi; // LogUtils, ShellOpen
    using Game.Modding;
    using Game.Settings;
    using Game.UI;
    using UnityEngine;

    [FileLocation("ModsSettings/TaxiTraffic/TaxiTraffic")]
    [SettingsUITabOrder(ActionsTab, StatusTab, AboutTab)]
    [SettingsUIGroupOrder(
        BehaviorGroup,
        CityScanGroup,
        TaxiScanGroup,
        LastUpdateGroup,
    #if DEBUG
        AdvancedDebugGroup,
    #endif
        StatusActionsGroup,
        AboutInfoGroup,
        DebugGroup,
        AboutLinksGroup
    )]
    [SettingsUIShowGroupName(
        BehaviorGroup,
        CityScanGroup,
        TaxiScanGroup,
        LastUpdateGroup,
    #if DEBUG
        AdvancedDebugGroup,
    #endif
        StatusActionsGroup,
        AboutInfoGroup,
        DebugGroup,
        AboutLinksGroup
    )]

    public sealed partial class TaxiSettings : ModSetting
    {
        public const string ActionsTab = "Actions";
        public const string StatusTab = "Status";
        public const string AboutTab = "About";

        public const string BehaviorGroup = "Behavior";
        public const string DebugGroup = "Debug";
        public const string CityScanGroup = "CityScan";
        public const string TaxiScanGroup = "TaxiScan";
        public const string LastUpdateGroup = "LastUpdate";
        public const string StatusActionsGroup = "StatusActions";

        // Status (DEBUG builds only)
#if DEBUG
        public const string AdvancedDebugGroup = "AdvancedDebug";
#endif

        public const string AboutInfoGroup = "Info";
        public const string AboutLinksGroup = "Support Links";

        internal const int kTaxiAllowedPercentMin = 0;
        internal const int kTaxiAllowedPercentMax = 100;
        internal const int kTaxiAllowedPercentStep = 25;

        // First-install mod default: strongest normal-resident taxi reduction.
        internal const int kTaxiAllowedPercentDefault = 0;

        private const string kUrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        private const string kUrlDiscord = "https://discord.gg/HTav7ARPs2";

        private int m_ResidentsAllowedToUseTaxis = kTaxiAllowedPercentDefault;

        public TaxiSettings(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        // ---- Actions ----

        [SettingsUISlider(
            min = kTaxiAllowedPercentMin,
            max = kTaxiAllowedPercentMax,
            step = kTaxiAllowedPercentStep,
            scalarMultiplier = 1,
            unit = Unit.kPercentage)]
        [SettingsUISection(ActionsTab, BehaviorGroup)]
        [SettingsUISetter(typeof(TaxiSettings), nameof(OnTaxiEligibilitySliderChanged))]
        public int ResidentsAllowedToUseTaxis
        {
            get => m_ResidentsAllowedToUseTaxis;
            set => m_ResidentsAllowedToUseTaxis = SnapTaxiAllowedPercent(value);
        }

        [SettingsUISection(ActionsTab, BehaviorGroup)]
        [SettingsUISetter(typeof(TaxiSettings), nameof(OnTaxiEligibilityToggleChanged))]
        public bool BlockCommuters
        {
            get; set;
        }

        [SettingsUISection(ActionsTab, BehaviorGroup)]
        [SettingsUISetter(typeof(TaxiSettings), nameof(OnTaxiEligibilityToggleChanged))]
        public bool BlockTourists
        {
            get; set;
        }

        [SettingsUIButtonGroup(BehaviorGroup)]
        [SettingsUIButton]
        [SettingsUISection(ActionsTab, BehaviorGroup)]
        public bool ResetToGameDefaults
        {
            set
            {
                if (!value)
                    return;

                ApplyGameDefaults();
                TaxiTrafficSystem.RequestStatusRefresh(force: true);
                LogUtils.Info(Mod.s_Log, () => $"{Mod.ModTag} Reset to game defaults.");
            }
        }

        // Verbose logging is a DEBUG-only tool. In Release it is hidden and hard-wired to false so a
        // stale saved value (e.g. carried over from a DEBUG run, same settings file) can never leave
        // periodic logging running during normal play. Release users get the Status tab and the
        // one-shot "Write Report" button instead.
#if DEBUG
        // Moved to About tab so normal gameplay options stay clean.
        [SettingsUISection(AboutTab, DebugGroup)]
        public bool EnableDebugLogging
        {
            get; set;
        }
#else
        [SettingsUIHidden]
        public bool EnableDebugLogging
        {
            get => false;
            set { }
        }
#endif

        [SettingsUIButtonGroup(DebugGroup)]
        [SettingsUIButton]
        [SettingsUISection(AboutTab, DebugGroup)]
        public bool OpenLogFile
        {
            set
            {
                if (!value)
                    return;

                ShellOpen.OpenModLogOrLogsFolder();
            }
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
                    Application.OpenURL(kUrlParadox);
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
                    Application.OpenURL(kUrlDiscord);
                }
                catch (Exception)
                {
                }
            }
        }

        public override void SetDefaults()
        {
            // First install: normal residents avoid taxis; commuters/tourists stay vanilla unless enabled.
            ResidentsAllowedToUseTaxis = kTaxiAllowedPercentDefault;
            BlockCommuters = false;
            BlockTourists = false;
            EnableDebugLogging = false;
        }

        private void ApplyGameDefaults()
        {
            // Game Defaults: leave all rider types alone.
            ResidentsAllowedToUseTaxis = kTaxiAllowedPercentMax;
            BlockCommuters = false;
            BlockTourists = false;
            EnableDebugLogging = false;
        }

        public bool IsStatusReady()
        {
            return TaxiTrafficSystem.s_StatusLastSnapshotRealtime > 0.0;
        }

        public bool IsStatusNotReady()
        {
            return !IsStatusReady();
        }

        // SettingsUISetter callback: refresh Status after slider changes.
        private void OnTaxiEligibilitySliderChanged(int _)
        {
            TaxiTrafficSystem.RequestStatusRefresh(force: true);
        }

        // SettingsUISetter callback: refresh Status after commuter/tourist filter changes.
        private void OnTaxiEligibilityToggleChanged(bool _)
        {
            TaxiTrafficSystem.RequestStatusRefresh(force: true);
        }

        private static int SnapTaxiAllowedPercent(int value)
        {
            int clamped = Math.Max(kTaxiAllowedPercentMin, Math.Min(kTaxiAllowedPercentMax, value));
            int snapped = ((clamped + (kTaxiAllowedPercentStep / 2)) / kTaxiAllowedPercentStep) * kTaxiAllowedPercentStep;

            return Math.Max(kTaxiAllowedPercentMin, Math.Min(kTaxiAllowedPercentMax, snapped));
        }
    }
}
