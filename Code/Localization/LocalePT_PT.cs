// <copyright file="LocalePT_PT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocalePT_PT.cs
// Purpose: Portuguese (pt-PT) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocalePT_PT : IDictionarySource
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

        public LocalePT_PT(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Ações" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Estado" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Opções de táxi" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "ANÁLISE DE TÁXIS" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "TRANSPORTE DA CIDADE (por mês)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "ÚLTIMA ATUALIZAÇÃO" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "DEBUG AVANÇADO (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "AÇÕES DE ESTADO" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Links de suporte" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Debug / Registos" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "Residentes evitam táxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = uso normal de táxis.\n" +
                    "<25–75%> = percentagem de agregados locais que evitam táxis.\n" +
                    "<100%> = todos os residentes locais elegíveis evitam táxis.\n" +
                    "**Alguns táxis podem continuar ativos. Viagens em curso e táxis à espera nas praças podem terminar normalmente, e alguns sistemas do jogo podem chamar táxis de forma independente.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Pendulares evitam táxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**ON** = pendulares evitam táxis.\n" +
                    "**OFF** = uso normal de táxis por pendulares.\n"+
                    "Dê algum tempo para ajustar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Turistas evitam táxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**ON** = turistas evitam táxis.\n" +
                    "**OFF** = uso normal de táxis por turistas.\n" +
                    "Dê algum tempo para ajustar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Predefinições do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Define Residentes evitam táxis para 0% e desativa a opção para pendulares e turistas."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Mostrar última atualização" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Mostra bloqueios atuais, alterações recentes e a hora do estado."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Ativar registo detalhado" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Escreve linhas TaxiSummary periódicas para testes.\n" +
                    "**OFF** = usar no jogo normal."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Gravar relatório" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Executa uma análise detalhada e grava o relatório completo de estado no registo do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Abrir registo" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Abre o registo do mod. Se não estiver disponível, abre a pasta Logs."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Análise da cidade ainda indisponível." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Abra uma cidade, deixe a simulação correr e reabra Opções → Estado."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Análise de táxis ainda indisponível." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Abra uma cidade, deixe a simulação correr e reabra Opções → Estado."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Ainda sem atividade registada." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Os detalhes aparecem quando o estado estiver pronto."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Passageiros atuais" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Passageiros atuais em táxis.\n" +
                    "<Local> = passageiros que vivem na cidade.\n" +
                    "<LE> = pendulares e turistas de ligações externas.\n" +
                    "**O total pode ser maior devido a animais nos táxis.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Oferta de táxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Estacionados> = táxis estacionados.\n" +
                    "<Ativos> = táxis não estacionados, incluindo os à espera nas praças.\n" +
                    "<Depósitos locais> = depósitos de táxis construídos pelo jogador.\n" +
                    "<Praças> = áreas designadas para recolha/espera de táxis."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Táxis externos" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<De LE> = táxis vindos de ligações externas.\n" +
                    "<Fontes LE> = ligações externas que podem enviar táxis (como depósitos invisíveis).\n" +
                    "**Sem depósitos locais, o jogo pode enviar táxis de LE para pedidos locais.**\n" +
                    "**Nos testes, com todas as opções no máximo, entraram poucos ou nenhuns táxis de LE na cidade.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Motivo do táxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Motivo dos pedidos de táxi atuais.\n" +
                    "<Lazer> | <Casa> | <Trabalho> | <Escola> | <Compras> | <Outro>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Cidadãos" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Táxi> | <Autocarro> | <Elétrico> | <Comboio> | <Metro> | <Avião>\n" +
                    "**Viagens de cidadãos por mês na vista de Transportes do jogo.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Turistas" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Táxi> | <Autocarro> | <Elétrico> | <Comboio> | <Metro> | <Avião>\n" +
                    "**Viagens de turistas por mês na vista de Transportes do jogo.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Totais" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<Espera> = cims à espera de transporte público.\n" +
                    "<Turistas/mês> e <Cidadãos/mês> = total mensal de passageiros em transportes públicos."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Bloqueados agora" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Local> | <Pendular> | <Turista>\n" +
                    "**Cims ativos marcados pelo Taxi Traffic. Não é a população total da cidade.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Alterações recentes" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Bloq.> = passaram a evitar táxis.\n" +
                    "<Livre> = voltaram à escolha normal de táxi.\n" +
                    "<Pedidos parados> = chamadas de táxi impedidas pelo Taxi Traffic."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Atualizado" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Atualizado> = quando esta informação foi verificada."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Gravar estado no registo" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Grava um relatório de estado detalhado no registo do Taxi Traffic.**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Flags de bloqueio (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "Verificação DEV.\n" +
                    "Cims ativos = agentes físicos atualmente na simulação.\n" +
                    "TT bloqueado = marcador de propriedade do Taxi Traffic.\n" +
                    "IgnoreTaxi agora = flag vanilla real neste instante."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Flags de táxi (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "Verificação DEV.\n" +
                    "Ordem: Buffer dispatch | Exterior | Desativado."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} táxi | {1} autoc. | {2} elétr. |\n{3} comboio | {4} metro | {5} avião" },
                { KeyStatusTouristsLine, "{0} táxi | {1} autoc. | {2} elétr. |\n{3} comboio | {4} metro | {5} avião" },
                { KeyStatusTotalsLine, "{0} espera | {1} turistas/mês | {2} cidadãos/mês" },
                { KeyStatusPassengersLine, "{0} total | {1} local | {2} LE" },
                { KeyStatusTaxiSupplyLine, "{0} estac., {1} ativos | {2} depósitos | {3} praças" },
                { KeyStatusOutsideTaxisLine, "{0} de LE | {1} fontes LE" },
                { KeyStatusTaxiPurposeLine,
                    "{0} lazer | {1} casa | {2} trab. |\n" +
                    "{3} escola | {4} compras | {5} outro"
                },
                { KeyStatusRequestsLine,
                    "{0} cidade ({1} bloq.) | {2} LE ({3} bloq.) |\n" +
                    "{4} oferta local | {5} oferta LE | {6} praça"
                },
                { KeyStatusTaxiStandsLine, "{0} espera" },
                { KeyStatusTaxiFleetLine,
                    "{0} viagem | {1} espera | {2} regresso |\n" +
                    "{3} despacho | {4} a caminho | {5} estac."
                },
                { KeyStatusCoverageLine, "{0} local | {1} pendular | {2} turista" },
                { KeyStatusWorkDoneLine, "{0} bloq. | {1} livres | {2} pedidos parados" },
                { KeyStatusSnapshotLine, "Atualizado {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} cims ativos | {1} TT bloq. | {2} IgnoreTaxi"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} buf. despacho | {1} exterior | {2} desat."
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Nome apresentado deste mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Versão atual do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Abre a página deste autor no site Paradox Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Abre o suporte da comunidade no Discord no navegador."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
