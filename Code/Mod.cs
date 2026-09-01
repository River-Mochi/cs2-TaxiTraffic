// <copyright file="Mod.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Mod.cs
// Entry point for "Taxi Traffic".
// Registers settings, localization, logging, and the Taxi Traffic system.

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
    using Game.Simulation;           // ResidentAISystem

    public sealed class Mod : IMod
    {
        public const string ModName = "Taxi Traffic";
        public const string ModId = "TaxiTraffic";
        public const string ModTag = "[TAXI]";
        public const string ShortName = "Taxi Traffic";

#if DEBUG
        private const string kBuildType = "DEBUG";
#else
        private const string kBuildType = "RELEASE";
#endif

        public static string BuildDisplayName =>
            kBuildType == "RELEASE" ? "Release" : kBuildType;

        private static bool s_BannerLogged;

        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        public static readonly ILog s_Log =
            LogManager.GetLogger(ModId).SetShowsErrorsInUI(false);

        public static TaxiSettings? Setting
        {
            get; private set;
        }

        public void OnLoad(UpdateSystem updateSystem)
        {
            LogUtils.Configure(ModId, s_Log);
            ShellOpen.Configure(s_Log, ModId, ModTag);

            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                LogUtils.Info(
                    $"{ModName} {ModTag} v{ModVersion} [{kBuildType}] OnLoad");
            }

            GameManager? gameManager = GameManager.instance;
            if (gameManager == null)
            {
                LogUtils.Warn(
                    $"{ModTag} GameManager.instance is null; {ModName} cannot initialize.");
                return;
            }

            TaxiSettings setting = new(this);
            Setting = setting;

            try
            {
                LocalizationManager? localizationManager =
                    gameManager.localizationManager;

                if (localizationManager == null)
                {
                    LogUtils.Warn(
                        $"{ModTag} LocalizationManager is null; locale sources were not registered.");
                }
                else
                {
                    // Current Options UI translations.
                    localizationManager.AddSource("en-US", new LocaleEN(setting));
                    localizationManager.AddSource("fr-FR", new LocaleFR(setting));
                    localizationManager.AddSource("de-DE", new LocaleDE(setting));
                    localizationManager.AddSource("es-ES", new LocaleES(setting));
                    localizationManager.AddSource("it-IT", new LocaleIT(setting));
                    localizationManager.AddSource("pl-PL", new LocalePL(setting));
                    localizationManager.AddSource("pt-BR", new LocalePT_BR(setting));
                    localizationManager.AddSource("ja-JP", new LocaleJA(setting));
                    localizationManager.AddSource("ko-KR", new LocaleKO(setting));
                    localizationManager.AddSource("zh-HANS", new LocaleZH_HANS(setting));
                    localizationManager.AddSource("zh-HANT", new LocaleZH_HANT(setting));

                    // Future translations.
                    // Some require a localization mod because CS2 does not
                    // officially expose every language in the normal language menu.
                    // localizationManager.AddSource("pt-PT", new LocalePT_PT(setting));
                    // localizationManager.AddSource("th-TH", new LocaleTH(setting));
                    // localizationManager.AddSource("tr-TR", new LocaleTR(setting));
                    // localizationManager.AddSource("uk-UA", new LocaleUK(setting));
                    // localizationManager.AddSource("vi-VN", new LocaleVI(setting));
                }
            }
            catch (Exception ex)
            {
                LogUtils.Warn(
                    $"{ModTag} Localization registration failed: " +
                    $"{ex.GetType().Name}: {ex.Message}",
                    ex);
            }

            try
            {
                TaxiSettings defaults = new(this);

                AssetDatabase.global.LoadSettings(
                    ModId,
                    setting,
                    defaults,
                    userSetting: true);
            }
            catch (Exception ex)
            {
                LogUtils.Warn(
                    $"{ModTag} LoadSettings failed; using defaults.",
                    ex);
            }

            try
            {
                setting.RegisterInOptionsUI();
            }
            catch (Exception ex)
            {
                LogUtils.Warn(
                    $"{ModTag} RegisterInOptionsUI failed; mod options may be missing.",
                    ex);
            }

            // IMPORTANT ORDERING: Register this only ONE time.
            // Keep TaxiTraffic AFTER ResidentAISystem. Do Not Move.
            //
            // Running it before ResidentAI caused repeatable native CTDs in testing.
            // Preserve this order: it is stable and still updates taxi choices
            // in time for future trips.
            updateSystem.UpdateAfter<TaxiTrafficSystem, ResidentAISystem>(
                SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            Setting?.UnregisterInOptionsUI();
            Setting = null;
        }
    }
}
