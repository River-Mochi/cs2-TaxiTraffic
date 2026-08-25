// <copyright file="LocaleEN.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleEN.cs
// Purpose: English (en-US) for Options UI.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleEN : IDictionarySource
    {
        public const string KeyStatusCitizensLine = "TaxiTraffic.Status.CitizensLine";
        public const string KeyStatusTouristsLine = "TaxiTraffic.Status.TouristsLine";
        public const string KeyStatusTotalsLine = "TaxiTraffic.Status.TotalsLine";
        public const string KeyStatusTaxiSupplyLine = "TaxiTraffic.Status.TaxiSupplyLine";
        public const string KeyStatusPassengersLine = "TaxiTraffic.Status.PassengersLine";
        public const string KeyStatusRequestsLine = "TaxiTraffic.Status.RequestsLine";
        public const string KeyStatusTaxiFleetLine = "TaxiTraffic.Status.TaxiFleetLine";
        public const string KeyStatusTaxiStandsLine = "TaxiTraffic.Status.TaxiStandsLine";
        public const string KeyStatusCoverageLine = "TaxiTraffic.Status.CoverageLine";
        public const string KeyStatusCoverageGroupsLine = "TaxiTraffic.Status.CoverageGroupsLine";
        public const string KeyStatusWorkDoneLine = "TaxiTraffic.Status.WorkDoneLine";
        public const string KeyStatusWorkDone2Line = "TaxiTraffic.Status.WorkDone2Line";
        public const string KeyStatusSnapshotLine = "TaxiTraffic.Status.SnapshotLine";

        public const string KeyStatusGroupSafetyLine = "TaxiTraffic.Status.GroupSafetyLine";

#if DEBUG
        public const string KeyStatusMarkedDevLine = "TaxiTraffic.Status.MarkedDevLine";
        public const string KeyStatusTaxiFlagsDevLine = "TaxiTraffic.Status.TaxiFlagsDevLine";
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
            {
                title = title + " (" + Mod.ModVersion + ")";
            }

            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab),  "Status" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab),   "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Taxi Choices" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup),    "Debug / Logging" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "CITY TRANSIT (per month)" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "TAXI SCAN" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "LAST UPDATE" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "STATUS ACTIONS" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "ADVANCED DEBUG (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup),  "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Support Links" },

                // Behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAllowedToUseTaxis)), "Residents allowed to use taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAllowedToUseTaxis)),
                    "Controls local citizen taxi eligibility.\n" +
                    "**0% = residents ignore taxis** as much as possible.\n" +
                    "**25% = ~1 in 4** residents are eligible to use taxis.\n" +
                    "**50% = ~half** are eligible to use taxis.\n" +
                    "**75% = ~3 in 4** are eligible to use taxis.\n" +
                    "**100% = vanilla taxi** levels (heavy usage).\n" +
                    "Notes:\n" +
                    "- commuters and tourists are separate groups, see [ ✓ ] toggles.\n" +
                    "- a few vanilla systems (e.g. Leisure) can directly call taxis and may ignore the IgnoreTaxi flag,\n" +
                    " so small taxi usage can be seen even at 0%."
                    },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Commuters avoid taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**ON** = commuter households get Ignore Taxi flag.\n" +
                    "**OFF** = commuters can use taxis (vanilla)."
                     },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Tourists avoid taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**ON** = tourist households get Ignore Taxi flag.\n" +
                    "**OFF** = tourists can use taxis (vanilla).\n"
                    },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Game Defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Resets to game settings: residents return to vanilla taxi usage levels;\n" +
                    "any commuter/tourist blocking is OFF." },

                // Debug
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Enable verbose logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "When enabled, writes periodic TaxiSummary lines to help debug taxi activity.\n" +
                    "**OFF** = disable for normal gameplay; heavy logging can hurt performance.\n" +
                    "<Do not enable this for normal gameplay.>" },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Open Log File" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Opens this mod's log file. If the file is not available yet, opens the Logs/ folder." },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                  "City scan not available yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                  "Open a city and let the simulation run a few minutes, then reopen Options → Status.\n" +
                  "The value will show '-' until stats are ready." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                  "Taxi scan not available yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                  "Open a city and let the simulation run a few minutes.\n" +
                  "The value will show '-' until stats are ready." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                  "No activity recorded yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                  "Once a scan completes, this section shows what changed in the last update.\n" +
                  "The value will show '-' until activity exists." },

                // CITY SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Citizens" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "InfoView passenger table (per month).\n" +
                    "Order: Taxi | Bus | Tram | Train | Subway | Air." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Tourists" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "InfoView passenger table (per month).\n" +
                    "Order: Taxi | Bus | Tram | Train | Subway | Air." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Totals" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "Waiting = all cims currently waiting for public transport, not just taxi.\n" +
                    "Tourists/mo and Citizens/mo come from the city Transportation InfoView monthly totals." },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Taxi supply" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "DispatchCtr = taxi depots with a dispatch center.\n" +
                    "Order: Taxis | Depots | DispatchCtr | Stands." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusRequests)), "Taxi requests" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusRequests)),
                    "Active taxi requests by type.\n" +
                    "**City** = requests inside the city.\n" +
                    "**Outside** = requests tied to outside-connections.\n" +
                    "**Other** = request with no normal taxi request type."
               },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Passengers" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Taxi passenger sanity check.\n" +
                    "Resident means the passenger has a Resident component, so IgnoreTaxi can be checked." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiFleet)), "Taxi states" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiFleet)),
                    "What taxis are doing now.\n" +
                    "Order: Ride | Board | Return | Dispatch | EnRoute | Parked." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiStands)), "Taxi stands" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiStands)),
                    "Waiting = total cims waiting at a taxi stand.\n"
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "IgnoreTaxi coverage" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "Residents with IgnoreTaxi now / total residents." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage2)), "IgnoreTaxi coverage" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage2)),
                    "Commuter and tourist IgnoreTaxi coverage.\n" +
                    "Order: Commuter IgnoreTaxi/Total | Tourist IgnoreTaxi/Total." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Work done" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "What changed in the last update.\n" +
                    "Applied = residents newly marked IgnoreTaxi.\n" +
                    "RideClear = taxi ride request links cleared.\n" +
                    "LaneClear = taxi lane-waiting states cleared." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone2)), "Work done (2)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone2)),
                    "More counters from the last update.\n" +
                    "QueueClear = blocked residents released from taxi queue entities.\n" +
                    "SkipComm/SkipTour = commuters/tourists skipped because their toggles are OFF." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Updated" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "Updated time shows when this status snapshot was taken; usually after entering Options menu." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Write Status Report to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "Writes the latest completed Status snapshot to this mod's log file and requests a fresh refresh." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusGroupSafety)), "Cims in groups" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusGroupSafety)),
                    "Residents traveling as part of a group are left alone (vanilla) to prevent any mishaps.\n" +
                    "Mod only adjusts solo cims (majority of travelers)." },

#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Marked (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV sanity check.\n" +
                    "Marked = residents currently marked by this mod for IgnoreTaxi.\n" +
                    "IgnoreTaxi now = residents with the actual vanilla IgnoreTaxi flag." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Taxi flags (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV sanity check.\n" +
                    "Order: With dispatch buffer | From outside | Disabled." },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taxi | {1} bus | {2} tram\n{3} train | {4} subway | {5} air" },
                { KeyStatusTouristsLine, "{0} taxi | {1} bus | {2} tram\n{3} train | {4} subway | {5} air" },
                { KeyStatusTotalsLine, "{0} waiting | {1} tourists/mo | {2} citizens/mo" },
                { KeyStatusTaxiSupplyLine, "{0} taxis | {1} depots | {2} dispatch ctr | {3} stands" },
                { KeyStatusPassengersLine, "{0} total | {1} IgnoreTaxi | {2} resident" },
                { KeyStatusRequestsLine, "{0} city | {1} outside (OC) | {2} none " },
                { KeyStatusTaxiStandsLine, "{0} waiting" },
                { KeyStatusTaxiFleetLine, "{0} ride | {1} board | {2} return\n{3} dispatch | {4} en route | {5} parked" },
                { KeyStatusCoverageLine, "{0}/{1} IgnoreTaxi" },
                { KeyStatusCoverageGroupsLine, "{0}/{1} commuter | {2}/{3} tourist" },
                { KeyStatusWorkDoneLine, "{0} applied | {1} ride clear | {2} lane clear" },
                { KeyStatusWorkDone2Line, "{0} queue clear | {1} skip comm | {2} skip tour" },
                { KeyStatusGroupSafetyLine, "{0} skipped" },
                { KeyStatusSnapshotLine, "Updated {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine, "{0}/{1} marked | {2} IgnoreTaxi now" },
                { KeyStatusTaxiFlagsDevLine, "{0} dispatch buf | {1} outside | {2} disabled" },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)), "Display name of this mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)), "Current mod version." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Opens Paradox Mods website for the author's mods." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)), "Opens Discord community support in a browser." },
            };
        }

        public void Unload()
        {
        }
    }
}
