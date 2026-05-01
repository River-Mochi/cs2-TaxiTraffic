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

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(Setting.AdvancedDebugGroup), "ADVANCED DEBUG (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup),  "Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGroup), "Support Links" },

                // Behavior
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResidentsAllowedToUseTaxis)), "Residents allowed to use taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResidentsAllowedToUseTaxis)),
                    "Controls which residents are eligible for taxis.\n" +
                    "0% = strongest taxi reduction.\n" +
                    "25% = about 1 in 4 residents may use taxis.\n" +
                    "50% = about half.\n" +
                    "75% = about 3 in 4.\n" +
                    "100% = vanilla-style taxi use; the mod removes its IgnoreTaxi marks.\n" +
                    "Even at 0%, a few taxi trips may still happen because some vanilla trip systems (i.e. Leisure) can directly request taxi routes." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BlockCommuters)), "Include commuters" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BlockCommuters)),
                    "When enabled, the taxi slider also applies to commuter households.\n" +
                    "Hidden when Residents allowed to use taxis is 100%." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BlockTourists)), "Include tourists" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BlockTourists)),
                    "When enabled, the taxi slider also applies to tourist households.\n" +
                    "Hidden when Residents allowed to use taxis is 100%." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDebugLogging)), "Enable verbose taxi logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDebugLogging)),
                    "When enabled, logs periodic TaxiSummary lines to help debug remaining taxi activity.\n" +
                    "Disable for normal gameplay or it can hurt performance.\n" +
                    "Do not enable this for normal gameplay." },

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
                    "AllWait = all cims currently waiting for public transport, not just taxi.\n" +
                    "Tourists/mo and Citizens/mo come from the Transportation InfoView monthly totals." },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTaxiSupply)), "Taxi supply" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTaxiSupply)),
                    "DispatchCtr = taxi depots with a dispatch center.\n" +
                    "Order: Taxis | Depots | DispatchCtr | Stands." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusRequests)), "Taxi requests" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusRequests)),
                    "TaxiRequest counts by type.\n" +
                    "Customer = normal city pickup request.\n" +
                    "Outside = outside-connection taxi request.\n" +
                    "None = request with no normal taxi request type.\n" +
                    "Stand = taxi stand request." },

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
                    "Stand pressure only. The mod no longer directly changes TaxiStand state.\n" +
                    "Order: StandWait | StandReq." },

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
                    "SkipComm/SkipTour = commuters/tourists skipped because their include toggles are OFF." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSnapshotMeta)), "Snapshot" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSnapshotMeta)),
                    "Updated time shows when this status snapshot was taken, usually after entering Options menu." },

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
                { KeyStatusCitizensLine, "Taxi {0} | Bus {1} | Tram {2}\nTrain {3} | Subway {4} | Air {5}" },
                { KeyStatusTouristsLine, "Taxi {0} | Bus {1} | Tram {2}\nTrain {3} | Subway {4} | Air {5}" },
                { KeyStatusTotalsLine, "AllWait {0} | Tourists/mo {1} | Citizens/mo {2}" },
                { KeyStatusTaxiSupplyLine, "Taxis {0} | Depots {1} | DispatchCtr {2} | Stands {3}" },
                { KeyStatusPassengersLine, "Total {0} | IgnoreTaxi {1} | Resident {2}" },
                { KeyStatusRequestsLine, "Customer {0} | Outside {1} | None {2} | Stand {3}" },
                { KeyStatusTaxiFleetLine, "Ride {0} | Board {1} | Return {2}\nDispatch {3} | EnRoute {4} | Parked {5}" },
                { KeyStatusTaxiStandsLine, "StandWait {0} | StandReq {1}" },
                { KeyStatusCoverageLine, "IgnoreTaxi {0}/{1}" },
                { KeyStatusCoverageGroupsLine, "Commuter {0}/{1} | Tourist {2}/{3}" },
                { KeyStatusWorkDoneLine, "Applied {0} | RideClear {1} | LaneClear {2}" },
                { KeyStatusWorkDone2Line, "QueueClear {0} | SkipComm {1} | SkipTour {2}" },
                { KeyStatusSnapshotLine, "Updated {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine, "Marked {0}/{1} | IgnoreTaxi now {2}" },
                { KeyStatusTaxiFlagsDevLine, "DispatchBuf {0} | Outside {1} | Disabled {2}" },
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
