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
        public const string KeyStatusCitizensLine = "TaxiTraffic.Status.CitizensLine";
        public const string KeyStatusTouristsLine = "TaxiTraffic.Status.TouristsLine";
        public const string KeyStatusTotalsLine = "TaxiTraffic.Status.TotalsLine";
        public const string KeyStatusTaxiSupplyLine = "TaxiTraffic.Status.TaxiSupplyLine";
        public const string KeyStatusPassengersLine = "TaxiTraffic.Status.PassengersLine";
        public const string KeyStatusRequestsLine = "TaxiTraffic.Status.RequestsLine";
        public const string KeyStatusOutsideControlLine = "TaxiTraffic.Status.OutsideControlLine";
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
                title = title + " (" + Mod.ModVersion + ")";

            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab),  "Status" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab),   "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Taxi Choices" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Debug / Logging" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "CITY TRANSIT (per month)" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "TAXI SCAN" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "LAST UPDATE" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "STATUS ACTIONS" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "ADVANCED DEBUG (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Support Links" },

                // Behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAllowedToUseTaxis)), "Residents allowed to use taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAllowedToUseTaxis)),
                    "Controls local resident taxi eligibility.\n" +
                    "<0% = residents avoid taxis> as much as possible.\n" +
                    "<25% = ~1 in 4> residents are eligible to use taxis.\n" +
                    "<50% = ~half> are eligible to use taxis.\n" +
                    "<75% = ~3 in 4> are eligible to use taxis.\n" +
                    "<100% = vanilla taxi> usage.\n" +
                    "0% greatly reduces usage. A small amount may remain because separate game systems, including Leisure, can independently allow taxis for some trips.\n" +
                    "Commuters and tourists are controlled separately by the other toggles [x]."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Commuters avoid taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**ON** = commuter households are blocked from normal taxi use.\n" +
                    "**OFF** = commuters can use taxis (vanilla)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Tourists avoid taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**ON** = tourist households are blocked from normal taxi use.\n" +
                    "**OFF** = tourists can use taxis (vanilla)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockOutsideTaxis)), "Block outside taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockOutsideTaxis)),
                    "**ON** = blocks new taxi supply from outside connections.\n" +
                    "Existing outside taxis are not deleted; they can finish current work and drain naturally.\n" +
                    "Local taxi depots and normal outside traffic are not changed.\n" +
                    "With no local taxi depots, regular taxi service may become unavailable.\n" +
                    "**OFF** = vanilla outside taxi supply is available to enter the city." +
                    "Blocking outside taxis lets you build a city around walking, cycling, transit and personal cars."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Game Defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Returns residents to vanilla taxi usage and turns commuter, tourist, and outside-taxi blocking OFF." },

                // Debug / log controls
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Enable verbose logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Writes periodic TaxiSummary and performance lines for testing.\n" +
                    "**OFF** = use for normal gameplay; heavy logging can hurt performance.\n" +
                    "<Do not enable this for normal gameplay.>" },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Open Log File" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Opens this mod's log file. If the file is not available yet, opens the Logs/ folder." },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "City scan not available yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Open a city and let the simulation run a few minutes, then reopen Options → Status.\n" +
                    "The value will show '-' until stats are ready." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Taxi scan not available yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Open a city and let the simulation run a few minutes.\n" +
                    "The value will show '-' until stats are ready." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "No activity recorded yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Once a scan completes, this section shows blocking coverage and recent cleanup work.\n" +
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
                    "Waiting = all cims currently waiting for public transport, not just taxis.\n" +
                    "Tourists/mo and Citizens/mo come from the Transportation InfoView monthly totals." },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Taxi supply" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "From OC = active taxis carrying the game's FromOutside flag.\n" +
                    "Local depot = placed taxi depot buildings; garage upgrades add capacity but do not count as another depot.\n" +
                    "OC depot = outside connections that vanilla treats as taxi supply depots; these are not placed buildings.\n" +
                    "Total depot = local + OC taxi supply depots.\n" +
                    "Dispatch ctr = local taxi depots with a dispatch center." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Passengers" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Current taxi passengers.\n" +
                    "Blocked = resident passengers carrying Taxi Traffic's block marker.\n" +
                    "When their rider type is blocked, this should fall toward 0 as existing rides finish." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusRequests)), "Taxi requests" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusRequests)),
                    "Active taxi requests by purpose.\n" +
                    "City rider = normal local pickup requests; blocked shows how many seekers carry Taxi Traffic's block marker.\n" +
                    "OC rider = pickup requests from a rider at an outside-connection lane; blocked is the same sanity check.\n" +
                    "Local supply = reversed supply requests advertised by local depots or local taxis.\n" +
                    "OC supply = reversed supply requests advertised by outside connections or active outside taxis.\n" +
                    "Stand = requests for taxis to wait at taxi stands." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideControl)), "Outside taxi control" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideControl)),
                    "From OC = active taxis carrying the game's FromOutside flag.\n" +
                    "OC supply = active outside taxi-supply requests right now.\n" +
                    "Blocked = outside supply requests Taxi Traffic has suppressed since this city loaded.\n" +
                    "OC taxi resident = resident passengers currently riding FromOutside taxis.\n" +
                    "Not moved-in = those riders whose local household has not completed its first move-in.\n" +
                    "Move-in now = strongest old-style outside-taxi signal: not-moved-in local rider whose trip started at an outside connection.\n" +
                    "Move-in seen = cumulative count of those trips since this city loaded.\n" +
                    "Moving-in HH = local households not yet marked MovedIn; if this is 0, a zero Move-in seen result is not a useful test." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiFleet)), "Taxi states" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiFleet)),
                    "What taxis are doing now.\n" +
                    "Standby uses the game's Boarding state: it includes taxis sitting at a taxi stand waiting for the next job,\n" +
                    "and can also include a taxi actively boarding a passenger. The vehicle panel may show **Boarding** while it waits." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiStands)), "Taxi stands" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiStands)),
                    "Waiting = total cims currently waiting at taxi stands." },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Taxi blocking coverage" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "Residents carrying Taxi Traffic's block marker / active resident creatures.\n" +
                    "The vanilla IgnoreTaxi flag can clear temporarily at a destination, so the marker is the better player-facing coverage check.\n" +
                    "Group-linked travelers are left vanilla for safety, so coverage can be slightly below 100%." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage2)), "Taxi blocking coverage" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage2)),
                    "Blocked commuter and tourist resident creatures.\n" +
                    "Order: Commuter blocked/total | Tourist blocked/total." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Work done" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "What changed in the most recent update.\n" +
                    "Applied = residents newly blocked from taxis.\n" +
                    "RideNeed removed = invalid taxi RideNeeder components removed.\n" +
                    "Lane clear = blocked residents released from taxi waiting lanes." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone2)), "Work done (2)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone2)),
                    "Queue clear = blocked residents released from taxi queue entities.\n" +
                    "Skip comm / skip tour = rider types left vanilla because their toggles are OFF." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusGroupSafety)), "Cims in groups" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusGroupSafety)),
                    "Group-linked residents are temporarily left vanilla for safety.\n" +
                    "Group exempt is a temporary marker used only while the resident is linked to a group.\n" +
                    "Repaired = group exemptions or blocks corrected since this city loaded as group membership changed." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Updated" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "Shows when this Status snapshot was taken; it usually refreshes after entering the Options menu." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Write Status Report to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "Writes the latest Status snapshot to this mod's log file and requests a fresh refresh." },

#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Blocking flags (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV sanity check.\n" +
                    "Blocked mark = Taxi Traffic's stable block marker.\n" +
                    "IgnoreTaxi now = the actual vanilla flag at this instant.\n" +
                    "Allowed mark = normal residents intentionally left taxi-eligible.\n" +
                    "Group exempt = temporary group-linked exemption marker." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Taxi flags (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV sanity check.\n" +
                    "Order: With dispatch buffer | From outside | Disabled." },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taxi | {1} bus | {2} tram\n{3} train | {4} subway | {5} air" },
                { KeyStatusTouristsLine, "{0} taxi | {1} bus | {2} tram\n{3} train | {4} subway | {5} air" },
                { KeyStatusTotalsLine, "{0} waiting | {1} tourists/mo | {2} citizens/mo" },
                { KeyStatusTaxiSupplyLine, "{0} taxis ({1} from OC)\n{2} local depot | {3} OC depot | {4} total\n{5} dispatch ctr | {6} stands" },
                { KeyStatusPassengersLine, "{0} total | {1} blocked | {2} resident" },
                { KeyStatusRequestsLine, "{0} city rider ({1} blocked) | {2} OC rider ({3} blocked)\n{4} local supply | {5} OC supply | {6} stand" },
                { KeyStatusOutsideControlLine, "{0} from OC | {1} OC supply | {2} blocked since load\n{3} OC taxi resident | {4} not moved-in | {5} move-in now | {6} move-in seen\n{7} moving-in HH" },
                { KeyStatusTaxiStandsLine, "{0} waiting" },
                { KeyStatusTaxiFleetLine, "{0} ride | {1} standby | {2} return\n{3} dispatch | {4} en route | {5} parked" },
                { KeyStatusCoverageLine, "{0}/{1} blocked" },
                { KeyStatusCoverageGroupsLine, "{0}/{1} commuter | {2}/{3} tourist" },
                { KeyStatusWorkDoneLine, "{0} applied | {1} ride need removed | {2} lane clear" },
                { KeyStatusWorkDone2Line, "{0} queue clear | {1} skip comm | {2} skip tour" },
                { KeyStatusGroupSafetyLine, "{0} linked now | {1} group exempt | {2} repaired since load" },
                { KeyStatusSnapshotLine, "Updated {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine, "{0}/{1} blocked mark | {2} IgnoreTaxi now\n{3} allowed mark | {4} group exempt" },
                { KeyStatusTaxiFlagsDevLine, "{0} dispatch buf | {1} outside | {2} disabled" },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)), "Display name of this mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)), "Current mod version." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Opens Paradox Mods for the author's mods." },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)), "Opens Discord community support in a browser." },
            };
        }

        public void Unload()
        {
        }
    }
}
