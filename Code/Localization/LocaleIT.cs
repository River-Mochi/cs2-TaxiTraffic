// <copyright file="LocaleIT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleIT.cs
// Italian (it-IT) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleIT : IDictionarySource
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

        public LocaleIT(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Azioni" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Stato" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Scelte taxi" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "SCANSIONE TAXI" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "TRASPORTO CITTADINO (al mese)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "ULTIMO AGGIORNAMENTO" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "DEBUG AVANZATO (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "AZIONI STATO" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Link di supporto" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Debug / Log" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "I residenti evitano i taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = uso normale dei taxi.\n" +
                    "<25–75%> = percentuale di famiglie locali che evitano i taxi.\n" +
                    "<100%> = tutti i residenti locali idonei evitano i taxi.\n" +
                    "**Qualche taxi può comunque rimanere in circolazione. I viaggi attivi e la normale attesa ai posteggi taxi possono terminare naturalmente, e alcuni sistemi del gioco possono chiamare taxi in modo indipendente.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "I pendolari evitano i taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**ON** = i pendolari evitano i taxi.\n" +
                    "**OFF** = uso normale dei taxi per i pendolari.\n"+
                    "Lascia un po' di tempo al gioco per adattarsi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "I turisti evitano i taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**ON** = i turisti evitano i taxi.\n" +
                    "**OFF** = uso normale dei taxi per i turisti.\n" +
                    "Lascia un po' di tempo al gioco per adattarsi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Valori del gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Imposta Residenti su 0% e disattiva l'evitamento taxi per pendolari e turisti."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Mostra ultimo aggiornamento" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Mostra blocchi attuali, modifiche recenti e ora dell'ultimo stato."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Attiva log dettagliato" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Scrive periodicamente righe TaxiSummary per i test.\n" +
                    "**OFF** = per il gioco normale."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Scrivi rapporto" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Esegue la scansione diagnostica completa e scrive il rapporto Stato nel log del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Apri log" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Apre il log del mod. Se non disponibile, apre la cartella Logs."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Scansione città non ancora disponibile." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Apri una città, lascia girare la simulazione, poi riapri Opzioni → Stato."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Scansione taxi non ancora disponibile." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Apri una città, lascia girare la simulazione, poi riapri Opzioni → Stato."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Nessuna attività registrata." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "I dettagli dell'ultimo aggiornamento appaiono quando lo Stato è pronto."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Passeggeri attuali" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Passeggeri attualmente nei taxi.\n" +
                    "<Bloccati> = passeggeri a cui Taxi Traffic ha detto di evitare i taxi.\n" +
                    "<Locali> = passeggeri che vivono nella tua città."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Disponibilità taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Parcheggiati> = taxi attualmente parcheggiati.\n" +
                    "<Attivi> = taxi non parcheggiati, inclusi quelli in attesa ai posteggi taxi.\n" +
                    "<Depositi locali> = depositi taxi costruiti dal giocatore.\n" +
                    "<Posteggi> = aree dedicate a salita e attesa dei taxi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Taxi esterni" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<Da OC> = taxi provenienti dalle connessioni esterne.\n" +
                    "<Fonti OC> = connessioni esterne che possono inviare taxi (come depositi invisibili).\n" +
                    "**Se non ci sono depositi locali, il gioco può inviare taxi OC per le richieste locali.**\n" +
                    "**Nei test, con il massimo evitamento dei taxi, sono entrati in città pochi o nessun taxi OC.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Motivo taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Motivo delle richieste taxi attuali.\n" +
                    "<Svago> | <Casa> | <Lavoro> | <Scuola> | <Shopping> | <Altro>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Cittadini" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Taxi> | <Bus> | <Tram> | <Treno> | <Metro> | <Aereo>\n" +
                    "**Viaggi dei cittadini al mese dalla vista Trasporti del gioco.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Turisti" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Taxi> | <Bus> | <Tram> | <Treno> | <Metro> | <Aereo>\n" +
                    "**Viaggi dei turisti al mese dalla vista Trasporti del gioco.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Totali" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<In attesa> = cims che stanno aspettando il trasporto pubblico.\n" +
                    "<Turisti/mese> e <Cittadini/mese> = passeggeri totali del trasporto pubblico al mese."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Bloccati ora" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Locale> | <Pendolare> | <Turista>\n" +
                    "**Cims attivi attualmente segnati da Taxi Traffic. Non è la popolazione totale della città.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Modifiche recenti" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Bloccati> = appena impostati per evitare i taxi.\n" +
                    "<Sbloccati> = tornati alla scelta taxi normale.\n" +
                    "<Richieste taxi fermate> = chiamate taxi fermate da Taxi Traffic."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Aggiornato" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Aggiornato> = quando queste info di Stato sono state controllate."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Scrivi Stato nel log" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Scrive un rapporto Stato più dettagliato nel log di Taxi Traffic.**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Flag di blocco (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "Controllo DEV.\n" +
                    "Cims attivi = agenti fisici dei cims attualmente nella simulazione.\n" +
                    "TT bloccati = marker di proprietà di Taxi Traffic.\n" +
                    "IgnoreTaxi ora = vero flag vanilla in questo istante."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Flag taxi (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "Controllo DEV.\n" +
                    "Ordine: Con buffer dispatch | Da fuori | Disattivato."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taxi | {1} bus | {2} tram |\n{3} treno | {4} metro | {5} aereo" },
                { KeyStatusTouristsLine, "{0} taxi | {1} bus | {2} tram |\n{3} treno | {4} metro | {5} aereo" },
                { KeyStatusTotalsLine, "{0} in attesa | {1} turisti/mese | {2} cittadini/mese" },
                { KeyStatusPassengersLine, "{0} totale | {1} bloccati | {2} locali" },
                { KeyStatusTaxiSupplyLine, "{0} parcheggiati, {1} attivi | {2} depositi locali | {3} posteggi" },
                { KeyStatusOutsideTaxisLine, "{0} da OC | {1} fonti OC" },
                { KeyStatusTaxiPurposeLine,
                    "{0} svago | {1} casa | {2} lavoro |\n" +
                    "{3} scuola | {4} shopping | {5} altro"
                },
                { KeyStatusRequestsLine,
                    "{0} passeggero città ({1} bloccato) | {2} passeggero OC ({3} bloccato) |\n" +
                    "{4} offerta locale | {5} offerta OC | {6} posteggio"
                },
                { KeyStatusTaxiStandsLine, "{0} in attesa" },
                { KeyStatusTaxiFleetLine,
                    "{0} corsa | {1} attesa | {2} ritorno |\n" +
                    "{3} dispatch | {4} in arrivo | {5} parcheggiati"
                },
                { KeyStatusCoverageLine, "{0} locale | {1} pendolare | {2} turista" },
                { KeyStatusWorkDoneLine, "{0} bloccati | {1} sbloccati | {2} richieste taxi fermate" },
                { KeyStatusSnapshotLine, "Aggiornato {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} cims attivi | {1} TT bloccati | {2} IgnoreTaxi ora"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} buffer dispatch | {1} esterni | {2} disattivati"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Nome visualizzato del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Versione attuale del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Apre la pagina dell'autore su Paradox Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Apre il supporto della community Discord nel browser."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
