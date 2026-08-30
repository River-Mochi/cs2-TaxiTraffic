// <copyright file="LocaleEN.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleEN.cs
// English (en-US) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleEN : IDictionarySource
    {
        public const string KeyStatusCitizensLine =
            "TaxiTraffic.Status.CitizensLine";

        public const string KeyStatusTouristsLine =
            "TaxiTraffic.Status.TouristsLine";

        public const string KeyStatusTotalsLine =
            "TaxiTraffic.Status.TotalsLine";

        public const string KeyStatusPassengersLine =
            "TaxiTraffic.Status.PassengersLine";

        public const string KeyStatusTaxiSupplyLine =
            "TaxiTraffic.Status.TaxiSupplyLine";

        public const string KeyStatusOutsideTaxisLine =
            "TaxiTraffic.Status.OutsideTaxisLine";

        public const string KeyStatusRequestsLine =
            "TaxiTraffic.Status.RequestsLine";

        public const string KeyStatusTaxiFleetLine =
            "TaxiTraffic.Status.TaxiFleetLine";

        public const string KeyStatusTaxiStandsLine =
            "TaxiTraffic.Status.TaxiStandsLine";

        public const string KeyStatusCoverageLine =
            "TaxiTraffic.Status.CoverageLine";

        public const string KeyStatusWorkDoneLine =
            "TaxiTraffic.Status.WorkDoneLine";

        public const string KeyStatusSnapshotLine =
            "TaxiTraffic.Status.SnapshotLine";

#if DEBUG
        public const string KeyStatusMarkedDevLine =
            "TaxiTraffic.Status.MarkedDevLine";

        public const string KeyStatusTaxiFlagsDevLine =
            "TaxiTraffic.Status.TaxiFlagsDevLine";
#endif

        private readonly TaxiSettings m_Setting;

        public LocaleEN(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Status" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Taxi Choices" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "CITY TRANSIT (per month)" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "TAXI SCAN" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "LAST UPDATE" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "ADVANCED DEBUG (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "STATUS ACTIONS" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Support Links" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Debug / Logging" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "Residents avoid taxis"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0% = vanilla taxi use>.\n" +
                    "25–75% = about that share of local households avoid taxis.\n" +
                    "<100% = all local households avoid taxis>.\n" +
                    "Everyone in the same household gets the same setting.\n" +
                    "**Some taxis may still remain. Taxi Traffic allows active trips and normal taxi-stand standby behavior to finish naturally for game stability.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "Commuters avoid taxis"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**ON** = commuters avoid taxis.\n" +
                    "**OFF** = vanilla commuter taxi use."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "Tourists avoid taxis"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**ON** = tourists avoid taxis.\n" +
                    "**OFF** = vanilla tourist taxi use."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Game Defaults"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Sets Residents avoid taxis to 0% and turns commuter and tourist avoidance OFF."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Show last update info"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Shows current blocking, recent changes, and the Status snapshot time."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Enable verbose logging"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Writes periodic TaxiSummary lines for testing.\n" +
                    "**OFF** = use for normal gameplay."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Write Report"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Runs the deeper diagnostic scan and writes the full Status report to this mod's log."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Open Log"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Opens this mod's log file. If unavailable, opens the Logs folder."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "City scan not available yet."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Open a city and let the simulation run, then reopen Options → Status."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Taxi scan not available yet."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Open a city and let the simulation run, then reopen Options → Status."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "No activity recorded yet."
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Last-update details appear after the Status snapshot is ready."
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "Citizens"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "Transportation InfoView passengers per month.\n" +
                    "Order: Taxi | Bus | Tram | Train | Subway | Air."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "Tourists"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "Transportation InfoView tourist passengers per month.\n" +
                    "Order: Taxi | Bus | Tram | Train | Subway | Air."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "Totals"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "Waiting = cims currently waiting for public transport.\n" +
                    "Tourists/mo and Citizens/mo are Transportation InfoView totals."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Current passengers"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Current taxi passengers.\n" +
                    "Blocked = passengers flagged by Taxi Traffic to ignore taxis.\n" +
                    "Local = passengers who are local city cims."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "Taxi supply"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "Taxi count seen right now.\n" +
                    "OC = taxis carrying the game's FromOutside flag.\n" +
                    "Sources = outside connections that can supply taxis.\n" +
                    "Stands = taxi stand buildings."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "Outside taxis"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "From OC = active taxis carrying the game's FromOutside flag."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "Blocked now"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "Active cims currently flagged by Taxi Traffic.\n" +
                    "This is not the city's total population."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "Recent changes"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "Most recent Taxi Traffic control pass."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "Updated"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "Shows when this cached Status snapshot was taken."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "Write Status to Log"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "Runs the deeper diagnostic scan and writes the full Status report to this mod's log."
                },

#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "Blocking flags (dev)"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV sanity check.\n" +
                    "Active cims = physical cim agents currently in the simulation.\n" +
                    "TT blocked = Taxi Traffic's ownership marker.\n" +
                    "IgnoreTaxi now = the actual vanilla flag at this instant."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "Taxi flags (dev)"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV sanity check.\n" +
                    "Order: With dispatch buffer | From outside | Disabled."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine,
                    "{0} taxi | {1} bus | {2} tram\n{3} train | {4} subway | {5} air"
                },
                { KeyStatusTouristsLine,
                    "{0} taxi | {1} bus | {2} tram\n{3} train | {4} subway | {5} air"
                },
                { KeyStatusTotalsLine,
                    "{0} waiting | {1} tourists/mo | {2} citizens/mo"
                },
                { KeyStatusPassengersLine,
                    "{0} total | {1} blocked | {2} local"
                },
                { KeyStatusTaxiSupplyLine,
                    "{0} taxis ({1} OC) | {2} local depots | {3} OC sources | {4} stands"
                },
                { KeyStatusOutsideTaxisLine,
                    "{0} from OC"
                },
                { KeyStatusRequestsLine,
                    "{0} city rider ({1} blocked) | {2} OC rider ({3} blocked)\n" +
                    "{4} local supply | {5} OC supply | {6} stand"
                },
                { KeyStatusTaxiStandsLine,
                    "{0} waiting"
                },
                { KeyStatusTaxiFleetLine,
                    "{0} ride | {1} standby | {2} return\n" +
                    "{3} dispatch | {4} en route | {5} parked"
                },
                { KeyStatusCoverageLine,
                    "{0} local | {1} commuter | {2} tourist"
                },
                { KeyStatusWorkDoneLine,
                    "{0} blocked | {1} unblocked | {2} taxi requests stopped"
                },
                { KeyStatusSnapshotLine,
                    "Updated {0}"
                },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} active cims | {1} TT blocked | {2} IgnoreTaxi now"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} dispatch buf | {1} outside | {2} disabled"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Mod"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Display name of this mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Version"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Current mod version."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Paradox Mods"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Opens Paradox Mods for the author's mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Discord"
                },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Opens Discord community support in a browser."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
