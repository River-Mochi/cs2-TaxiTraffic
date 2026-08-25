// <copyright file="Mod.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Mod.cs
// Entry point for "Taxi Traffic".

namespace TaxiTraffic
{
    using System;                    // Exception
    using System.Reflection;         // Assembly version number
    using Colossal.IO.AssetDatabase; // AssetDatabase
    using Colossal.Localization;     // LocalizationManager
    using Colossal.Logging;          // ILog, LogManager
    using CS2Shared.RiverMochi;      // LogUtils, ShellOpen
    using Game;                      // UpdateSystem
    using Game.Modding;              // IMod
    using Game.SceneFlow;            // GameManager
    using Game.Simulation;           // ResidentAISystem, taxi/ride systems

    public sealed class Mod : IMod
    {
        public const string ModName = "Taxi Traffic";
        public const string ModId = "TaxiTraffic";
        public const string ModTag = "[TAXI]";
        public const string ShortName = "Taxi Traffic";

        private static bool s_BannerLogged;

        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        public static readonly ILog s_Log =
            LogManager.GetLogger(ModId).SetShowsErrorsInUI(false);

        public static Setting? Setting
        {
            get; private set;
        }

        public void OnLoad(UpdateSystem updateSystem)
        {
            // Shared helpers need the mod ID so fallback logs still use TaxiTraffic.log.
            LogUtils.Configure(ModId);
            ShellOpen.Configure(s_Log, ModId, ModTag);

            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                LogUtils.Info(s_Log, () => $"{ModId} {ModTag} v{ModVersion} OnLoad");
            }

            Setting setting = new(this);
            Setting = setting;

            try
            {
                // Add locale text before Options UI registration; the UI looks up these keys when it builds the page.
                LocalizationManager? lm = GameManager.instance?.localizationManager;
                if (lm != null)
                {
                    lm.AddSource("en-US", new LocaleEN(setting));
                }
                else
                {
                    LogUtils.WarnOnce(
                        s_Log,
                        key: "LocalizationManagerNull",
                        messageFactory: () => $"{ModTag} LocalizationManager is null; skipping locale registration.");
                }
            }
            catch (Exception ex)
            {
                LogUtils.WarnOnce(
                    s_Log,
                    key: "LocaleRegistrationFailed",
                    messageFactory: () => $"{ModTag} Locale registration failed; Options UI text may be missing.",
                    exception: ex);
            }

            try
            {
                // Saved user settings replace defaults after locale keys exist.
                Setting defaults = new(this);
                AssetDatabase.global.LoadSettings(ModId, setting, defaults, userSetting: true);
            }
            catch (Exception ex)
            {
                LogUtils.WarnOnce(
                    s_Log,
                    key: "LoadSettingsFailed",
                    messageFactory: () => $"{ModTag} LoadSettings failed; using defaults.",
                    exception: ex);
            }

            try
            {
                // Locale setup before register so that Options UI can use localized text.
                setting.RegisterInOptionsUI();
            }
            catch (Exception ex)
            {
                LogUtils.WarnOnce(
                    s_Log,
                    key: "RegisterOptionsFailed",
                    messageFactory: () => $"{ModTag} RegisterInOptionsUI failed; mod options may be missing.",
                    exception: ex);
            }

            // Run main rider control after resident AI, then before taxi and ride-need systems can act on ride decisions.
            updateSystem.UpdateAfter<TaxiTrafficSystem, ResidentAISystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateBefore<TaxiTrafficSystem, TaxiDispatchSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateBefore<TaxiTrafficSystem, RideNeederSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            LogUtils.Info(s_Log, () => $"{ModTag} OnDispose");

            try
            {
                Setting?.UnregisterInOptionsUI();
            }
            catch (Exception ex)
            {
                LogUtils.WarnOnce(
                    s_Log,
                    key: "UnregisterOptionsFailed",
                    messageFactory: () => $"{ModTag} UnregisterInOptionsUI failed.",
                    exception: ex);
            }

            Setting = null;
        }
    }
}
