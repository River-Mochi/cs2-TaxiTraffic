// <copyright file="LocaleFR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleFR.cs
// French (fr-FR) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleFR : IDictionarySource
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

        public LocaleFR(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "État" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Choix de taxi" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "ANALYSE DES TAXIS" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "TRANSPORTS URBAINS (par mois)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "DERNIÈRE MISE À JOUR" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "DÉBOGAGE AVANCÉ (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "ACTIONS D'ÉTAT" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Infos" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Liens d'aide" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Débogage / Journal" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "Les résidents évitent les taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = usage normal des taxis.\n" +
                    "<25–75%> = part des foyers locaux qui évitent les taxis.\n" +
                    "<100%> = tous les résidents locaux concernés évitent les taxis.\n" +
                    "**Quelques taxis peuvent rester. Les trajets en cours et l'attente aux stations se terminent normalement; certains systèmes du jeu peuvent aussi appeler des taxis.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Les navetteurs évitent les taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**ACTIVÉ** = les navetteurs évitent les taxis.\n" +
                    "**DÉSACTIVÉ** = usage normal des taxis.\n" +
                    "Laissez un peu de temps pour l'effet."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Les touristes évitent les taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**ACTIVÉ** = les touristes évitent les taxis.\n" +
                    "**DÉSACTIVÉ** = usage normal des taxis.\n" +
                    "Laissez un peu de temps pour l'effet."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Valeurs du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Met les résidents à 0 % et désactive l'évitement pour navetteurs et touristes."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Afficher la dernière mise à jour" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Affiche les blocages, les changements récents et l'heure de l'état."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Activer le journal détaillé" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Écrit régulièrement des lignes TaxiSummary pour les tests.\n" +
                    "**DÉSACTIVÉ** = pour jouer normalement."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Écrire le rapport" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Lance le diagnostic détaillé et écrit le rapport d'état complet dans le journal du mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Ouvrir le journal" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Ouvre le journal du mod. Sinon, ouvre le dossier Logs."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Analyse de la ville pas encore disponible." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Ouvrez une ville, laissez tourner la simulation, puis rouvrez Options → État."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Analyse des taxis pas encore disponible." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Ouvrez une ville, laissez tourner la simulation, puis rouvrez Options → État."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Aucune activité enregistrée." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Les détails apparaissent quand l'état est prêt."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Passagers actuels" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Passagers actuellement en taxi.\n" +
                    "<Locaux> = passagers qui vivent dans votre ville.\n" +
                    "<OC> = navetteurs et touristes des connexions extérieures.\n" +
                    "**Le total peut être plus élevé à cause des animaux dans les taxis.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Taxis disponibles" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Stationnés> = taxis actuellement garés.\n" +
                    "<Actifs> = taxis non garés, y compris ceux en attente aux stations.\n" +
                    "<Dépôts locaux> = dépôts construits par le joueur.\n" +
                    "<Stations> = zones de prise en charge et d'attente."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Taxis extérieurs" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<Depuis OC> = taxis venant des connexions extérieures.\n" +
                    "<Sources OC> = connexions pouvant envoyer des taxis (comme des dépôts invisibles).\n" +
                    "**Sans dépôt local, le jeu peut envoyer des taxis OC pour les demandes locales.**\n" +
                    "**En test, avec toutes les options d'évitement au maximum, peu ou aucun taxi OC n'est entré en ville.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Motif du taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Motif des demandes de taxi actuelles.\n" +
                    "<Loisirs> | <Domicile> | <Travail> | <École> | <Achats> | <Autre>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Citoyens" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Taxi> | <Bus> | <Tram> | <Train> | <Métro> | <Avion>\n" +
                    "**Trajets citoyens par mois dans la vue Transports.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Touristes" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Taxi> | <Bus> | <Tram> | <Train> | <Métro> | <Avion>\n" +
                    "**Trajets touristes par mois dans la vue Transports.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Totaux" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<En attente> = cims qui attendent les transports en commun.\n" +
                    "<Touristes/mois> et <Citoyens/mois> = passagers totaux par mois."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Actuellement bloqués" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Local> | <Navetteur> | <Touriste>\n" +
                    "**Cims actifs marqués par Taxi Traffic. Ce n'est pas la population totale.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Changements récents" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Bloqués> = viennent d'être configurés pour éviter les taxis.\n" +
                    "<Débloqués> = retour au choix normal.\n" +
                    "<Appels taxi bloqués> = appels arrêtés par Taxi Traffic."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Mis à jour" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Mis à jour> = heure de cette vérification."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Écrire l'état dans le journal" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Écrit un rapport d'état plus détaillé dans le journal de Taxi Traffic.**"
                },

#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Drapeaux de blocage (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "Vérification DEV.\n" +
                    "Cims actifs = agents physiques présents dans la simulation.\n" +
                    "TT bloqués = marqueur de Taxi Traffic.\n" +
                    "IgnoreTaxi actuel = drapeau vanilla réel à cet instant."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Drapeaux taxi (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "Vérification DEV.\n" +
                    "Ordre : Avec buffer d'affectation | Depuis l'extérieur | Désactivé."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taxi | {1} bus | {2} tram |\n{3} train | {4} métro | {5} avion" },
                { KeyStatusTouristsLine, "{0} taxi | {1} bus | {2} tram |\n{3} train | {4} métro | {5} avion" },
                { KeyStatusTotalsLine, "{0} en attente | {1} touristes/mois | {2} citoyens/mois" },
                { KeyStatusPassengersLine, "{0} total | {1} locaux | {2} OC" },
                { KeyStatusTaxiSupplyLine, "{0} garés, {1} actifs | {2} dépôts locaux | {3} stations" },
                { KeyStatusOutsideTaxisLine, "{0} depuis OC | {1} sources OC" },
                { KeyStatusTaxiPurposeLine,
                    "{0} loisirs | {1} domicile | {2} travail |\n" +
                    "{3} école | {4} achats | {5} autre"
                },
                { KeyStatusRequestsLine,
                    "{0} passager local ({1} bloqué) | {2} passager OC ({3} bloqué) |\n" +
                    "{4} offre locale | {5} offre OC | {6} station"
                },
                { KeyStatusTaxiStandsLine, "{0} en attente" },
                { KeyStatusTaxiFleetLine,
                    "{0} course | {1} attente | {2} retour |\n" +
                    "{3} affectation | {4} en route | {5} garés"
                },
                { KeyStatusCoverageLine, "{0} local | {1} navetteur | {2} touriste" },
                { KeyStatusWorkDoneLine, "{0} bloqués | {1} débloqués | {2} appels taxi bloqués" },
                { KeyStatusSnapshotLine, "Mis à jour {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} cims actifs | {1} bloqués TT | {2} IgnoreTaxi actuel"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} buffer affectation | {1} extérieur | {2} désactivé"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Nom affiché du mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Version actuelle du mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Ouvre la page de l'auteur sur Paradox Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Ouvre l'aide Discord dans le navigateur."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
