// File: Resources/LocaleEN.cs
// English (en-US) for Options UI.

namespace RiderControl
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleEN : IDictionarySource
    {
        public const string KeyStatusCitizensLine = "CimBeSmart.Status.CitizensLine";
        public const string KeyStatusTouristsLine = "CimBeSmart.Status.TouristsLine";
        public const string KeyStatusTotalsLine = "CimBeSmart.Status.TotalsLine";
        public const string KeyStatusTaxiSupplyLine = "CimBeSmart.Status.TaxiSupplyLine";
        public const string KeyStatusPassengersLine = "CimBeSmart.Status.PassengersLine";
        public const string KeyStatusRequestsLine = "CimBeSmart.Status.RequestsLine";
        public const string KeyStatusTaxiFleetLine = "CimBeSmart.Status.TaxiFleetLine";
        public const string KeyStatusTaxiStandsLine = "CimBeSmart.Status.TaxiStandsLine";
        public const string KeyStatusCoverageLine = "CimBeSmart.Status.CoverageLine";
        public const string KeyStatusCoverageGroupsLine = "CimBeSmart.Status.CoverageGroupsLine";
        public const string KeyStatusWorkDoneLine = "CimBeSmart.Status.WorkDoneLine";
        public const string KeyStatusWorkDone2Line = "CimBeSmart.Status.WorkDone2Line";
        public const string KeyStatusSnapshotLine = "CimBeSmart.Status.SnapshotLine";

        public const string KeyStatusGroupSafetyLine = "CimBeSmart.Status.GroupSafetyLine";

#if DEBUG
        public const string KeyStatusMarkedDevLine = "CimBeSmart.Status.MarkedDevLine";
        public const string KeyStatusTaxiFlagsDevLine = "CimBeSmart.Status.TaxiFlagsDevLine";
#endif

        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.StatusTab),  "Status" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Taxi Choices" },
                { m_Setting.GetOptionGroupLocaleID(Setting.DebugGroup),    "Debug / Logging" },

                { m_Setting.GetOptionGroupLocaleID(Setting.CityScanGroup), "CITY TRANSIT (per month)" },
                { m_Setting.GetOptionGroupLocaleID(Setting.TaxiScanGroup), "TAXI SCAN" },
                { m_Setting.GetOptionGroupLocaleID(Setting.LastUpdateGroup), "LAST UPDATE" },
                { m_Setting.GetOptionGroupLocaleID(Setting.StatusActionsGroup), "STATUS ACTIONS" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(Setting.AdvancedDebugGroup), "ADVANCED DEBUG (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup),  "Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Support Links" },

                // Behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResidentsAllowedToUseTaxis)), "Residents allowed to use taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResidentsAllowedToUseTaxis)),
                    "Controls normal resident taxi eligibility.\n" +
                    "0% = set all residents to ignore Taxis.\n" +
                    "25% = ~1 in 4 residents may use taxis.\n" +
                    "50% = ~half may use taxis.\n" +
                    "75% = ~3 in 4 may use taxis\n" +
                    "100% = residents use taxis like vanilla (heavy).\n" +
                    "Notes:\n" +
                    "- commuters and tourists toggles are separate behavior.\n" +
                    "- a few vanilla systems (e.g. Leisure) can can directly call taxis for cims and doesn't care if they have IgnoreTaxi,\n" +
                    " so there is some small usage at 0% setting; majority of cims will ignore taxis."
                    },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BlockCommuters)), "Commuters avoid taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BlockCommuters)),
                    "OFF = commuters are left alone, like vanilla.\n" +
                    "ON = all commuter households get Ignore Taxi flag." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BlockTourists)), "Tourists avoid taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BlockTourists)),
                    "OFF = tourists are left alone, like vanilla.\n" +
                    "ON = all tourist households get Ignore Taxi flag." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToGameDefaults)), "Reset to Game Defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToGameDefaults)),
                    "Sets normal residents to 100% taxi eligibility and turns commuter/tourist blocking OFF." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDebugLogging)), "Enable verbose taxi logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDebugLogging)),
                    "When enabled, logs periodic TaxiSummary lines to help debug remaining taxi activity.\n" +
                    "Disable for normal gameplay or it can hurt performance.\n" +
                    "Do not enable this for normal gameplay." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLogFile)), "Open Log File" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLogFile)),
                    "Opens this mod's log file. If the file is not available yet, opens the Logs/ folder." },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusNotReadyCityScan)),
                  "City scan not available yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusNotReadyCityScan)),
                  "Open a city and let the simulation run a few minutes, then reopen Options → Status.\n" +
                  "The value will show '-' until stats are ready." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusNotReadyTaxiScan)),
                  "Taxi scan not available yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusNotReadyTaxiScan)),
                  "Open a city and let the simulation run a few minutes.\n" +
                  "The value will show '-' until stats are ready." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusNotReadyLastUpdate)),
                  "No activity recorded yet." },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusNotReadyLastUpdate)),
                  "Once a scan completes, this section shows what changed in the last update.\n" +
                  "The value will show '-' until activity exists." },

                // CITY SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusMonthlyPassengers1)), "Citizens" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusMonthlyPassengers1)),
                    "InfoView passenger table (per month).\n" +
                    "Order: Taxi | Bus | Tram | Train | Subway | Air." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusMonthlyTourists)), "Tourists" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusMonthlyTourists)),
                    "InfoView passenger table (per month).\n" +
                    "Order: Taxi | Bus | Tram | Train | Subway | Air." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusMonthlyTotal)), "Totals" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusMonthlyTotal)),
                    "Waiting = all cims currently waiting for public transport, not just taxi.\n" +
                    "Tourists/mo and Citizens/mo come from the city Transportation InfoView monthly totals." },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTaxiSupply)), "Taxi supply" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTaxiSupply)),
                    "DispatchCtr = taxi depots with a dispatch center.\n" +
                    "Order: Taxis | Depots | DispatchCtr | Stands." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusRequests)), "Taxi requests" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusRequests)),
                    "TaxiRequest counts by type.\n" +
                    "Customer = city pickup request.\n" +
                    "Outside = outside-connection taxi request.\n" +
                    "None = request with no normal taxi request type."
               },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusPassengers)), "Passengers" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusPassengers)),
                    "Taxi passenger sanity check.\n" +
                    "Resident means the passenger has a Resident component, so IgnoreTaxi can be checked." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTaxiFleet)), "Taxi states" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTaxiFleet)),
                    "What taxis are doing now.\n" +
                    "Order: Ride | Board | Return | Dispatch | EnRoute | Parked." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTaxiStands)), "Taxi stands" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTaxiStands)),
                    "Waiting = total cims waiting at a taxi stand.\n"
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusCoverage1)), "IgnoreTaxi coverage" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusCoverage1)),
                    "Residents with IgnoreTaxi now / total residents." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusCoverage2)), "IgnoreTaxi coverage" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusCoverage2)),
                    "Commuter and tourist IgnoreTaxi coverage.\n" +
                    "Order: Commuter IgnoreTaxi/Total | Tourist IgnoreTaxi/Total." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusWorkDone1)), "Work done" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusWorkDone1)),
                    "What changed in the last update.\n" +
                    "Applied = residents newly marked IgnoreTaxi.\n" +
                    "RideClear = taxi ride request links cleared.\n" +
                    "LaneClear = taxi lane-waiting states cleared." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusWorkDone2)), "Work done (2)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusWorkDone2)),
                    "More counters from the last update.\n" +
                    "QueueClear = blocked residents released from taxi queue entities.\n" +
                    "SkipComm/SkipTour = commuters/tourists skipped because their toggles are OFF." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSnapshotMeta)), "Updated" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSnapshotMeta)),
                    "Updated time shows when this status snapshot was taken; usually after entering Options menu." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.WriteStatusReportToLog)), "Write Status Report to Log" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.WriteStatusReportToLog)),
                    "Writes the latest completed Status snapshot to this mod's log file and requests a fresh refresh." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusGroupSafety)), "Cims in groups stay vanilla" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusGroupSafety)),
                    "Residents traveling as part of a group are left alone (vanilla) to prevent mishaps.\n" +
                    "Mod only adjusts solo travelers (majority)." },

#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusDebugMarkedCoverage)), "Marked (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusDebugMarkedCoverage)),
                    "DEV sanity check.\n" +
                    "Marked = residents currently marked by this mod for IgnoreTaxi.\n" +
                    "IgnoreTaxi now = residents with the actual vanilla IgnoreTaxi flag." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusDebugTaxiFlags)), "Taxi flags (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusDebugTaxiFlags)),
                    "DEV sanity check.\n" +
                    "Order: With dispatch buffer | From outside | Disabled." },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taxi | {1} bus | {2} tram\n{3} train | {4} subway | {5} air" },
                { KeyStatusTouristsLine, "{0} taxi | {1} bus | {2} tram\n{3} train | {4} subway | {5} air" },
                { KeyStatusTotalsLine, "{0} waiting | {1} tourists/mo | {2} citizens/mo" },
                { KeyStatusTaxiSupplyLine, "{0} taxis | {1} depots | {2} dispatch ctr | {3} stands" },
                { KeyStatusPassengersLine, "{0} total | {1} IgnoreTaxi | {2} resident" },
                { KeyStatusRequestsLine, "{0} customer | {1} outside (OC) | {2} none " },
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameDisplay)), "Display name of this mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionDisplay)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionDisplay)), "Current mod version." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Opens Paradox Mods website for the author's mods." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenDiscord)), "Opens Discord community support in a browser." },
            };
        }

        public void Unload()
        {
        }
    }
}
