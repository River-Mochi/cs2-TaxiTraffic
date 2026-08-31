// <copyright file="LocalePL.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocalePL.cs
// Polish (pl-PL) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocalePL : IDictionarySource
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

        public LocalePL(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Akcje" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Stan" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "O modzie" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Ustawienia taksówek" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "SKAN TAKSÓWEK" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "TRANSPORT MIEJSKI (miesięcznie)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "OSTATNIA AKTUALIZACJA" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "ZAAWANSOWANY DEBUG (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "AKCJE STANU" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Pomoc i linki" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Debug / Logi" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "Mieszkańcy unikają taksówek" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = normalne korzystanie z taksówek.\n" +
                    "<25–75%> = procent lokalnych gospodarstw domowych unikających taksówek.\n" +
                    "<100%> = wszyscy kwalifikujący się lokalni mieszkańcy unikają taksówek.\n" +
                    "**Niektóre taksówki mogą nadal zostać. Taxi Traffic pozwala dokończyć aktywne kursy i normalne oczekiwanie na postojach, żeby gra działała stabilnie.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Dojeżdżający unikają taksówek" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**WŁ.** = dojeżdżający unikają taksówek.\n" +
                    "**WYŁ.** = normalne korzystanie z taksówek przez dojeżdżających.\n"+
                    "Daj grze chwilę na dostosowanie."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Turyści unikają taksówek" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**WŁ.** = turyści unikają taksówek.\n" +
                    "**WYŁ.** = normalne korzystanie z taksówek przez turystów.\n" +
                    "Daj grze chwilę na dostosowanie."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Domyślne gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Ustawia Mieszkańców na 0% i wyłącza unikanie taksówek dla dojeżdżających i turystów."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Pokaż ostatnią aktualizację" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Pokazuje aktualne blokady, ostatnie zmiany i czas migawki Stanu."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Włącz szczegółowe logi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Okresowo zapisuje linie TaxiSummary do testów.\n" +
                    "**WYŁ.** = do normalnej gry."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Zapisz raport" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Uruchamia dokładniejszy skan diagnostyczny i zapisuje pełny raport Stanu do logu moda."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Otwórz log" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Otwiera log moda. Jeśli jest niedostępny, otwiera folder Logs."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Skan miasta jeszcze niedostępny." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Otwórz miasto, uruchom symulację, a potem ponownie otwórz Opcje → Stan."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Skan taksówek jeszcze niedostępny." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Otwórz miasto, uruchom symulację, a potem ponownie otwórz Opcje → Stan."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Brak zapisanej aktywności." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Szczegóły ostatniej aktualizacji pojawią się, gdy Stan będzie gotowy."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Aktualni pasażerowie" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Aktualni pasażerowie taksówek.\n" +
                    "<Zablokowani> = pasażerowie, którym Taxi Traffic kazał unikać taksówek.\n" +
                    "<Lokalni> = pasażerowie mieszkający w twoim mieście."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Dostępność taksówek" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Zaparkowane> = obecnie zaparkowane taksówki.\n" +
                    "<Aktywne> = taksówki nieparkujące, w tym czekające na postojach.\n" +
                    "<Lokalne zajezdnie> = zajezdnie taksówek zbudowane przez gracza.\n" +
                    "<Postoje> = wyznaczone miejsca odbioru i oczekiwania taksówek."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Taksówki z zewnątrz" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<Z OC> = taksówki z połączeń zewnętrznych.\n" +
                    "<Źródła OC> = połączenia zewnętrzne, które mogą wysyłać taksówki (jak niewidzialne zajezdnie).\n" +
                    "**Jeśli nie ma lokalnych zajezdni, gra może wysłać taksówki OC do lokalnych zgłoszeń.**\n" +
                    "**W testach, przy suwaku + opcjach [x] unikania taksówek na maksimum, do miasta przyjeżdżało mało albo zero taksówek OC.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Cel taksówki" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Cel aktualnych zgłoszeń taksówek.\n" +
                    "<Rozrywka> | <Dom> | <Praca> | <Szkoła> | <Zakupy> | <Inne>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Obywatele" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Taxi> | <Autobus> | <Tramwaj> | <Pociąg> | <Metro> | <Samolot>\n" +
                    "**Podróże obywateli na miesiąc z widoku Transportu w grze.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Turyści" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Taxi> | <Autobus> | <Tramwaj> | <Pociąg> | <Metro> | <Samolot>\n" +
                    "**Podróże turystów na miesiąc z widoku Transportu w grze.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Razem" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<Oczekujący> = cimy czekające obecnie na transport publiczny.\n" +
                    "<Turyści/mies.> i <Obywatele/mies.> = łączna liczba pasażerów transportu publicznego na miesiąc."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Zablokowani teraz" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Lokalni> | <Dojeżdżający> | <Turyści>\n" +
                    "**Aktywne cimy oznaczone obecnie przez Taxi Traffic. To nie jest cała populacja miasta.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Ostatnie zmiany" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Zablokowani> = właśnie ustawieni na unikanie taksówek.\n" +
                    "<Odblokowani> = wrócili do normalnego wyboru taksówki.\n" +
                    "<Zatrzymane zgłoszenia taxi> = zgłoszenia taksówek zatrzymane przez Taxi Traffic."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Zaktualizowano" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Zaktualizowano> = kiedy sprawdzono te dane Stanu."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Zapisz Stan do logu" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Zapisuje dokładniejszy raport Stanu do logu Taxi Traffic.**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Flagi blokowania (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "Kontrola DEV.\n" +
                    "Aktywne cimy = fizyczni agenci cimów obecni w symulacji.\n" +
                    "TT blocked = znacznik własności Taxi Traffic.\n" +
                    "IgnoreTaxi teraz = rzeczywista flaga vanilla w tej chwili."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Flagi taxi (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "Kontrola DEV.\n" +
                    "Kolejność: Z buforem dispatch | Z zewnątrz | Wyłączone."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taxi | {1} autobus | {2} tramwaj\n{3} pociąg | {4} metro | {5} samolot" },
                { KeyStatusTouristsLine, "{0} taxi | {1} autobus | {2} tramwaj\n{3} pociąg | {4} metro | {5} samolot" },
                { KeyStatusTotalsLine, "{0} oczekuje | {1} turyści/mies. | {2} obywatele/mies." },
                { KeyStatusPassengersLine, "{0} razem | {1} zablok. | {2} lokalni" },
                { KeyStatusTaxiSupplyLine, "{0} zapark., {1} aktywne | {2} lokalne zajezdnie | {3} postoje" },
                { KeyStatusOutsideTaxisLine, "{0} z OC | {1} źródła OC" },
                { KeyStatusTaxiPurposeLine,
                    "{0} rozrywka | {1} dom | {2} praca\n" +
                    "{3} szkoła | {4} zakupy | {5} inne"
                },
                { KeyStatusRequestsLine,
                    "{0} pasażer miasta ({1} blok.) | {2} pasażer OC ({3} blok.)\n" +
                    "{4} lokalna podaż | {5} podaż OC | {6} postój"
                },
                { KeyStatusTaxiStandsLine, "{0} oczekuje" },
                { KeyStatusTaxiFleetLine,
                    "{0} kurs | {1} postój | {2} powrót\n" +
                    "{3} dispatch | {4} w drodze | {5} zapark."
                },
                { KeyStatusCoverageLine, "{0} lokalni | {1} dojeżdżający | {2} turyści" },
                { KeyStatusWorkDoneLine, "{0} zablok. | {1} odblok. | {2} zatrzymane zgłoszenia" },
                { KeyStatusSnapshotLine, "Zaktualizowano {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} aktywne cimy | {1} TT blok. | {2} IgnoreTaxi teraz"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} bufor dispatch | {1} z zewnątrz | {2} wyłączone"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Nazwa wyświetlana tego moda."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Aktualna wersja moda."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Otwiera stronę autora w serwisie Paradox Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Otwiera pomoc społeczności Discord w przeglądarce."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
