// File: Mod.cs
// Entry point for "Cim Be Smart".

namespace RiderControl
{
    using Colossal.IO.AssetDatabase; // AssetDatabase
    using Colossal.Localization;     // LocalizationManager
    using Colossal.Logging;          // ILog, LogManager
    using CS2Shared.RiverMochi;      // LogUtils, ShellOpen
    using Game;                      // UpdateSystem
    using Game.Modding;              // IMod
    using Game.SceneFlow;            // GameManager
    using Game.Simulation;           // ResidentAISystem, taxi/ride systems
    using System;                    // Exception
    using System.Reflection;         // Assembly version number

    public sealed class Mod : IMod
    {
        public const string ModName = "Cim Be Smart";
        public const string ModId = "CimBeSmart";
        public const string ModTag = "[CBS]";
        public const string ShortName = "Cim Be Smart";

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
            // Shared helpers need the mod ID so fallback logs still use CimBeSmart.log.
            LogUtils.Configure(ModId);
            ShellOpen.Configure(s_Log, ModId, ModTag);

            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                LogUtils.Info(s_Log, () => $"{ModId} {ModTag} v{ModVersion} OnLoad");
            }

            Setting setting = new Setting(this);
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
                Setting defaults = new Setting(this);
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
            updateSystem.UpdateAfter<RiderControlSystem, ResidentAISystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateBefore<RiderControlSystem, TaxiDispatchSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateBefore<RiderControlSystem, RideNeederSystem>(SystemUpdatePhase.GameSimulation);
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
