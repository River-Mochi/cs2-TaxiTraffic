// <copyright file="LocaleDE.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleDE.cs
// German (de-DE) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleDE : IDictionarySource
    {
        public const string KeyStatusCitizensLine = "TaxiTraffic.Status.CitizensLine";
        public const string KeyStatusTouristsLine = "TaxiTraffic.Status.TouristsLine";
        public const string KeyStatusTotalsLine = "TaxiTraffic.Status.TotalsLine";
        public const string KeyStatusPassengersLine = "TaxiTraffic.Status.PassengersLine";
        public const string KeyStatusTaxiSupplyLine = "TaxiTraffic.Status.TaxiSupplyLine";
        public const string KeyStatusOutsideTaxisLine = "TaxiTraffic.Status.OutsideTaxisLine";
        public const string KeyStatusTaxiPurposeLine = "TaxiTraffic.Status.TaxiPurposeLine";
        public const string KeyStatusRequestsLine = "TaxiTraffic.Status.RequestsLine";
        public const string KeyStatusTaxiFleetLine = "TaxiTraffic.Status.TaxiFleetLine";
        public const string KeyStatusTaxiStandsLine = "TaxiTraffic.Status.TaxiStandsLine";
        public const string KeyStatusCoverageLine = "TaxiTraffic.Status.CoverageLine";
        public const string KeyStatusWorkDoneLine = "TaxiTraffic.Status.WorkDoneLine";
        public const string KeyStatusSnapshotLine = "TaxiTraffic.Status.SnapshotLine";

#if DEBUG
        public const string KeyStatusMarkedDevLine = "TaxiTraffic.Status.MarkedDevLine";
        public const string KeyStatusTaxiFlagsDevLine = "TaxiTraffic.Status.TaxiFlagsDevLine";
#endif

        private readonly TaxiSettings m_Setting;

        public LocaleDE(TaxiSettings setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            string title = Mod.ShortName;

            if (!string.IsNullOrEmpty(Mod.ModVersion))
                title = title + " (" + Mod.ModVersion + ")";

            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Aktionen" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Status" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "Über" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Taxi-Auswahl" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "TAXI-SCAN" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "STADTVERKEHR (pro Monat)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "LETZTES UPDATE" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "ERWEITERTES DEBUG (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "STATUS-AKTIONEN" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Support-Links" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Debug / Logging" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "Einwohner meiden Taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = normale Taxinutzung.\n" +
                    "<25–75%> = Anteil lokaler Haushalte, die Taxis meiden.\n" +
                    "<100%> = alle passenden lokalen Einwohner meiden Taxis.\n" +
                    "**Einige Taxis können trotzdem unterwegs sein. Aktive Fahrten und normales Warten an Taxiständen können natürlich auslaufen, und einige Spielsysteme können unabhängig davon Taxis rufen.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Pendler meiden Taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**AN** = Pendler meiden Taxis.\n" +
                    "**AUS** = normale Taxinutzung für Pendler.\n"+
                    "Gib dem Spiel kurz Zeit, sich anzupassen."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Touristen meiden Taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**AN** = Touristen meiden Taxis.\n" +
                    "**AUS** = normale Taxinutzung für Touristen.\n" +
                    "Gib dem Spiel kurz Zeit, sich anzupassen."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Spiel-Standardwerte" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Setzt Einwohner auf 0 % und schaltet Taxi-Vermeidung für Pendler und Touristen AUS."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Letztes Update anzeigen" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Zeigt aktuelle Blockierungen, letzte Änderungen und die Zeit des Status-Snapshots."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Ausführliches Logging aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Schreibt regelmäßig TaxiSummary-Zeilen für Tests.\n" +
                    "**AUS** = für normales Spielen."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Bericht schreiben" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Führt den tieferen Diagnose-Scan aus und schreibt den vollständigen Statusbericht ins Mod-Log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Log öffnen" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Öffnet das Mod-Log. Falls nicht verfügbar, wird der Logs-Ordner geöffnet."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Stadt-Scan noch nicht verfügbar." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Öffne eine Stadt, lass die Simulation laufen und öffne dann Optionen → Status erneut."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Taxi-Scan noch nicht verfügbar." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Öffne eine Stadt, lass die Simulation laufen und öffne dann Optionen → Status erneut."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Noch keine Aktivität aufgezeichnet." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Details zum letzten Update erscheinen, sobald der Status bereit ist."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Aktuelle Fahrgäste" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Aktuelle Fahrgäste in Taxis.\n" +
                    "<Blockiert> = Fahrgäste, denen Taxi Traffic gesagt hat, Taxis zu meiden.\n" +
                    "<Lokal> = Fahrgäste, die in deiner Stadt wohnen."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Taxi-Angebot" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Geparkt> = aktuell geparkte Taxis.\n" +
                    "<Aktiv> = nicht geparkte Taxis, inklusive Bereitschaft an Taxiständen.\n" +
                    "<Lokale Depots> = vom Spieler gebaute Taxidepots.\n" +
                    "<Stände> = ausgewiesene Taxi-Abhol- und Wartebereiche."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Taxis von außerhalb" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<Von OC> = Taxis aus Außenverbindungen.\n" +
                    "<OC-Quellen> = Außenverbindungen, die Taxis schicken können (wie unsichtbare Depots).\n" +
                    "**Ohne lokale Depots kann das Spiel OC-Taxis für lokale Anfragen schicken.**\n" +
                    "**In Tests führte maximale Taxi-Vermeidung dazu, dass nur wenige oder keine OC-Taxis in die Stadt kamen.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Taxi-Zweck" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Zweck der aktuellen Taxianfragen.\n" +
                    "<Freizeit> | <Zuhause> | <Arbeit> | <Schule> | <Einkaufen> | <Sonstiges>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Bürger" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Taxi> | <Bus> | <Tram> | <Zug> | <U-Bahn> | <Flug>\n" +
                    "**Bürgerfahrten pro Monat aus der Verkehrs-Infoansicht des Spiels.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Touristen" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Taxi> | <Bus> | <Tram> | <Zug> | <U-Bahn> | <Flug>\n" +
                    "**Touristenfahrten pro Monat aus der Verkehrs-Infoansicht des Spiels.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Gesamt" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<Wartend> = Cims, die gerade auf öffentlichen Verkehr warten.\n" +
                    "<Touristen/Monat> und <Bürger/Monat> = gesamte ÖPNV-Fahrgäste pro Monat."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Jetzt blockiert" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Lokal> | <Pendler> | <Tourist>\n" +
                    "**Aktive Cims, die Taxi Traffic gerade markiert hat. Das ist nicht die gesamte Stadtbevölkerung.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Letzte Änderungen" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Blockiert> = neu auf Taxi vermeiden gesetzt.\n" +
                    "<Freigegeben> = wieder normale Taxi-Auswahl.\n" +
                    "<Taxianfragen gestoppt> = von Taxi Traffic gestoppte Taxirufe."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Aktualisiert" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Aktualisiert> = Zeitpunkt, zu dem dieser Status geprüft wurde."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Status ins Log schreiben" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Schreibt einen ausführlicheren Statusbericht ins Taxi-Traffic-Log.**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Blockier-Flags (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV-Prüfung.\n" +
                    "Aktive Cims = physische Cim-Agenten, die gerade simuliert werden.\n" +
                    "TT blockiert = Eigentumsmarker von Taxi Traffic.\n" +
                    "IgnoreTaxi jetzt = tatsächliches Vanilla-Flag in diesem Moment."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Taxi-Flags (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV-Prüfung.\n" +
                    "Reihenfolge: Mit Dispatch-Buffer | Von außerhalb | Deaktiviert."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} Taxi | {1} Bus | {2} Tram |\n{3} Zug | {4} U-Bahn | {5} Flug" },
                { KeyStatusTouristsLine, "{0} Taxi | {1} Bus | {2} Tram |\n{3} Zug | {4} U-Bahn | {5} Flug" },
                { KeyStatusTotalsLine, "{0} wartend | {1} Touristen/Monat | {2} Bürger/Monat" },
                { KeyStatusPassengersLine, "{0} gesamt | {1} blockiert | {2} lokal" },
                { KeyStatusTaxiSupplyLine, "{0} geparkt, {1} aktiv | {2} lokale Depots | {3} Stände" },
                { KeyStatusOutsideTaxisLine, "{0} von OC | {1} OC-Quellen" },
                { KeyStatusTaxiPurposeLine,
                    "{0} Freizeit | {1} Zuhause | {2} Arbeit |\n" +
                    "{3} Schule | {4} Einkaufen | {5} Sonstiges"
                },
                { KeyStatusRequestsLine,
                    "{0} Stadt-Fahrgast ({1} blockiert) | {2} OC-Fahrgast ({3} blockiert) |\n" +
                    "{4} lokale Versorgung | {5} OC-Versorgung | {6} Stand"
                },
                { KeyStatusTaxiStandsLine, "{0} wartend" },
                { KeyStatusTaxiFleetLine,
                    "{0} Fahrt | {1} Bereitschaft | {2} Rückfahrt |\n" +
                    "{3} Dispatch | {4} unterwegs | {5} geparkt"
                },
                { KeyStatusCoverageLine, "{0} lokal | {1} Pendler | {2} Tourist" },
                { KeyStatusWorkDoneLine, "{0} blockiert | {1} freigegeben | {2} Taxianfragen gestoppt" },
                { KeyStatusSnapshotLine, "Aktualisiert {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} aktive Cims | {1} TT blockiert | {2} IgnoreTaxi jetzt"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} Dispatch-Buffer | {1} außerhalb | {2} deaktiviert"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Anzeigename dieses Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Aktuelle Mod-Version."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Öffnet die Autorenseite auf Paradox Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Öffnet den Discord-Community-Support im Browser."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
