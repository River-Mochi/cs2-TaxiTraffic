// File: lang/LocaleEN.cs
// English (en-US) for Options UI.

namespace RiderControl
{
    using Colossal;
    using System.Collections.Generic;

    public sealed class LocaleEN : IDictionarySource
    {
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
                { m_Setting.GetOptionGroupLocaleID(Setting.BehaviorGroup), "Behavior" },
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BlockTaxiUsage)), "Residents: ignore taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BlockTaxiUsage)),
                    "**Enabled [ ✓ ]** means **Residents** ignore taxis.\n" +
                    "Also clears cims currently waiting for a taxi so they re-route to other methods.\n" +
                    "Note: a very small number may still appear because some trip planners like Leisure System can randomly allow taxi routing.\n" +
                    "Disabled = vanilla taxi use." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BlockCommuters)), "Commuters: ignore taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BlockCommuters)),
                    "**Enabled [ ✓ ]** means **Commuters** ignore taxis.\n" +
                    "Hidden and <disabled> unless [Residents: ignore taxis] is Enabled [ ✓ ]\n" +
                    "Even if you left Commuters checked, it will be OFF when Residents is OFF." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BlockTourists)), "Tourists: ignore taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BlockTourists)),
                    "**Enabled [ ✓ ]** means **Tourists** ignore taxis.\n" +
                    "Hidden and <disabled> unless [Residents: ignore taxis] is Enabled [ ✓ ]\n" +
                    "Even if you left Tourists checked, it will be OFF when Residents toggle is OFF." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BlockTaxiStandDemand)), "Taxi stands: block demand (alpha)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BlockTaxiStandDemand)),
                    "This toggle is new (Alpha):\n" +
                    "Clears TaxiStand **Passengers waiting** so the stand stops requesting stand-by taxis.\n" +
                    "Hidden and disabled unless [Residents: ignore taxis] is enabled [ ✓ ]\n" +
                    "Even if you left taxi stands checked, it will be OFF when Residents is OFF." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FixMovingAwayHighwayWalkers)), "Moving-away fix (highway walkers)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.FixMovingAwayHighwayWalkers)),
                    "Fixes Moving-Away cims with flag **Ignore Transport** so they can take public transport instead of walking to the outside connection.\n" +
                    "Tip: add a direct bus connection to outside connection for best results." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableDebugLogging)), "Enable verbose taxi logging" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableDebugLogging)),
                    "When enabled, logs a periodic TaxiSummary line to help debug remaining taxi activity.\n" +
                    "Disable for normal gameplay (can hurt performance / spam logs)." },

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
                { "CBS.Status.Value." + nameof(Setting.StatusMonthlyPassengers1),
                    "Taxi {0:N0} | Bus {1:N0} | Tram {2:N0}\n" +
                    "Train {3:N0} | Subway {4:N0} | Air {5:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusMonthlyTourists)), "Tourists" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusMonthlyTourists)),
                    "InfoView passenger table (per month).\n" +
                    "Order: Taxi | Bus | Tram | Train | Subway | Air." },
                { "CBS.Status.Value." + nameof(Setting.StatusMonthlyTourists),
                    "Taxi {0:N0} | Bus {1:N0} | Tram {2:N0}\n" +
                    "Train {3:N0} | Subway {4:N0} | Air {5:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusMonthlyTotal)), "Totals" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusMonthlyTotal)),
                    "Wait now = cims currently flagged as WaitingTransport (any transport type, not taxi-only).\n" +
                    "Tourists/mo and Citizens/mo match the InfoView passenger summary totals." },
                { "CBS.Status.Value." + nameof(Setting.StatusMonthlyTotal),
                    "Wait now {0:N0} | Tourists/mo {1:N0} | Citizens/mo {2:N0}" },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTaxiSupply)), "Taxi supply" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTaxiSupply)),
                    "Taxis = taxi vehicles.\n" +
                    "Depots = taxi depots.\n" +
                    "Dispatch centers = taxi depots with the Dispatch Center upgrade.\n" +
                    "Stands = taxi stands." },
                { "CBS.Status.Value." + nameof(Setting.StatusTaxiSupply),
                    "Taxis {0:N0} | Depots {1:N0}\n" +
                    "Dispatch centers {2:N0} | Stands {3:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusRequests)), "Taxi requests" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusRequests)),
                    "TaxiRequest counts by type.\n" +
                    "Customer = normal cim taxi request.\n" +
                    "Outside = outside-connection taxi request.\n" +
                    "None = other/unknown taxi request type." },
                { "CBS.Status.Value." + nameof(Setting.StatusRequests),
                    "Customer {0:N0} | Outside {1:N0} | None {2:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusPassengers)), "Passengers" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusPassengers)),
                    "Taxi passenger sanity check.\n" +
                    "Resident = passengers that have a Resident component (cim passenger entities)." },
                { "CBS.Status.Value." + nameof(Setting.StatusPassengers),
                    "Total {0:N0} | IgnoreTaxi {1:N0} | Resident {2:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTaxiFleet)), "Taxi states" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTaxiFleet)),
                    "What taxis are doing now.\n" +
                    "Transport | Boarding | Return | Dispatched | EnRoute | Parked" },
                { "CBS.Status.Value." + nameof(Setting.StatusTaxiFleet),
                    "Transport {0:N0} | Boarding {1:N0} | Return {2:N0}\n" +
                    "Dispatched {3:N0} | EnRoute {4:N0} | Parked {5:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusTaxiStands)), "Taxi stands" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusTaxiStands)),
                    "Stand pressure.\n" +
                    "Waiting = WaitingPassengers count.\n" +
                    "StandReq = TaxiRequestType.Stand." },
                { "CBS.Status.Value." + nameof(Setting.StatusTaxiStands),
                    "Waiting {0:N0} | StandReq {1:N0}" },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusCoverage1)), "IgnoreTaxi coverage" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusCoverage1)),
                    "Residents with IgnoreTaxi / Total residents scanned." },
                { "CBS.Status.Value." + nameof(Setting.StatusCoverage1),
                    "IgnoreTaxi {0:N0}/{1:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusCoverage2)), "IgnoreTaxi coverage" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusCoverage2)),
                    "Commuters IgnoreTaxi / Total | Tourists IgnoreTaxi / Total." },
                { "CBS.Status.Value." + nameof(Setting.StatusCoverage2),
                    "Commuter {0:N0}/{1:N0} | Tourist {2:N0}/{3:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusWorkDone1)), "Work done" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusWorkDone1)),
                    "Last update counters.\n" +
                    "Applied | RideNeeder cleared | Taxi-lane waiting cleared." },
                { "CBS.Status.Value." + nameof(Setting.StatusWorkDone1),
                    "Applied {0:N0} | RideClear {1:N0} | LaneClear {2:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusWorkDone2)), "Work done (2)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusWorkDone2)),
                    "More last update counters.\n" +
                    "Taxi-stand waiting cleared | Commuters skipped | Tourists skipped." },
                { "CBS.Status.Value." + nameof(Setting.StatusWorkDone2),
                    "StandClear {0:N0} | SkipCommuter {1:N0} | SkipTourist {2:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSnapshotMeta)), "Snapshot" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSnapshotMeta)),
                    "Snapshot timestamp for the current Status scan.\n" +
                    "Status updates while Options UI is open (throttled)." },
                { "CBS.Status.Value." + nameof(Setting.StatusSnapshotMeta),
                    "At {0}" },

#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusDebugMarkedCoverage)), "Marked (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusDebugMarkedCoverage)),
                    "DEV sanity check.\n" +
                    "Marked by this mod / Total residents | IgnoreTaxi now." },
                { "CBS.Status.Value." + nameof(Setting.StatusDebugMarkedCoverage),
                    "Marked {0:N0}/{2:N0} | IgnoreTaxi now {1:N0}" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusDebugTaxiFlags)), "Taxi flags (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusDebugTaxiFlags)),
                    "DEV sanity check.\n" +
                    "With dispatch buffer | From outside | Disabled." },
                { "CBS.Status.Value." + nameof(Setting.StatusDebugTaxiFlags),
                    "DispatchBuf {0:N0} | Outside {1:N0} | Disabled {2:N0}" },
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
