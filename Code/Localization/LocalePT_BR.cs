// <copyright file="LocalePT_BR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocalePT_BR.cs
// Portuguese (pt-BR) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocalePT_BR : IDictionarySource
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

        public LocalePT_BR(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Status" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Opções de táxi" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "ANÁLISE DE TÁXIS" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "TRANSPORTE DA CIDADE (por mês)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "ÚLTIMA ATUALIZAÇÃO" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "DEBUG AVANÇADO (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "AÇÕES DE STATUS" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Links de suporte" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Debug / Logs" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "Moradores evitam táxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = uso normal de táxis.\n" +
                    "<25–75%> = porcentagem de famílias locais que evitam táxis.\n" +
                    "<100%> = todos os moradores locais elegíveis evitam táxis.\n" +
                    "**Alguns táxis ainda podem continuar circulando. Viagens ativas e a espera normal nos pontos de táxi podem terminar naturalmente, e alguns sistemas do jogo podem chamar táxis de forma independente.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Quem vem de fora evita táxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**ON** = quem vem de fora evita táxis.\n" +
                    "**OFF** = uso normal de táxis para quem vem de fora.\n"+
                    "Dê um tempinho para o jogo se ajustar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Turistas evitam táxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**ON** = turistas evitam táxis.\n" +
                    "**OFF** = uso normal de táxis para turistas.\n" +
                    "Dê um tempinho para o jogo se ajustar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Padrões do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Coloca Moradores em 0% e desliga a restrição para quem vem de fora e turistas."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Mostrar última atualização" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Mostra bloqueios atuais, mudanças recentes e a hora do Status."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Ativar logs detalhados" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Grava linhas TaxiSummary periodicamente para testes.\n" +
                    "**OFF** = use no jogo normal."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Gravar relatório" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Faz uma análise mais detalhada e grava o relatório completo de Status no log do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Abrir log" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Abre o log do mod. Se não estiver disponível, abre a pasta Logs."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Análise da cidade ainda não disponível." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Abra uma cidade, deixe a simulação rodar e depois reabra Opções → Status."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Análise de táxis ainda não disponível." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Abra uma cidade, deixe a simulação rodar e depois reabra Opções → Status."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Nenhuma atividade registrada ainda." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Os detalhes da última atualização aparecem quando o Status estiver pronto."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Passageiros agora" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Passageiros atuais em táxis.\n" +
                    "<Locais> = passageiros que moram na sua cidade.\n" +
                    "<OC> = trabalhadores pendulares e turistas de conexão externa.\n" +
                    "O total pode ser maior devido a animais nos táxis."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Oferta de táxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Estacionados> = táxis estacionados agora.\n" +
                    "<Ativos> = táxis não estacionados, incluindo os aguardando em pontos de táxi.\n" +
                    "<Garagens locais> = garagens de táxi construídas pelo jogador.\n" +
                    "<Pontos> = áreas marcadas para embarque e espera de táxis."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Táxis de fora" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<De OC> = táxis vindos de conexões externas.\n" +
                    "<Fontes OC> = conexões externas que podem mandar táxis (como garagens invisíveis).\n" +
                    "**Sem garagens locais, o jogo pode mandar táxis OC para pedidos locais.**\n" +
                    "**Nos testes, com a prevenção de táxis no máximo, poucos ou nenhum táxi OC entrou na cidade.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Motivo do táxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Motivo dos pedidos de táxi atuais.\n" +
                    "<Lazer> | <Casa> | <Trabalho> | <Escola> | <Compras> | <Outro>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Cidadãos" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Táxi> | <Ônibus> | <Bonde> | <Trem> | <Metrô> | <Avião>\n" +
                    "**Viagens de cidadãos por mês na visão de Transporte do jogo.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Turistas" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Táxi> | <Ônibus> | <Bonde> | <Trem> | <Metrô> | <Avião>\n" +
                    "**Viagens de turistas por mês na visão de Transporte do jogo.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Totais" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<Esperando> = cims esperando transporte público agora.\n" +
                    "<Turistas/mês> e <Cidadãos/mês> = total de passageiros do transporte público por mês."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Bloqueados agora" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Local> | <De fora> | <Turista>\n" +
                    "**Cims ativos marcados agora pelo Taxi Traffic. Isso não é a população total da cidade.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Mudanças recentes" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Bloqueados> = acabaram de ser configurados para evitar táxis.\n" +
                    "<Liberados> = voltaram à escolha normal de táxi.\n" +
                    "<Pedidos de táxi parados> = chamadas de táxi interrompidas pelo Taxi Traffic."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Atualizado" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Atualizado> = quando estas informações de Status foram verificadas."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Gravar Status no log" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Grava um relatório de Status mais detalhado no log do Taxi Traffic.**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Flags de bloqueio (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "Checagem DEV.\n" +
                    "Cims ativos = agentes físicos de cims atualmente na simulação.\n" +
                    "TT bloqueado = marcador de propriedade do Taxi Traffic.\n" +
                    "IgnoreTaxi agora = flag vanilla real neste instante."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Flags de táxi (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "Checagem DEV.\n" +
                    "Ordem: Com buffer de dispatch | De fora | Desativado."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} táxi | {1} ônibus | {2} bonde |\n{3} trem | {4} metrô | {5} avião" },
                { KeyStatusTouristsLine, "{0} táxi | {1} ônibus | {2} bonde |\n{3} trem | {4} metrô | {5} avião" },
                { KeyStatusTotalsLine, "{0} esperando | {1} turistas/mês | {2} cidadãos/mês" },
                { KeyStatusPassengersLine, "{0} total | {1} locais | {2} OC" },
                { KeyStatusTaxiSupplyLine, "{0} estacionados, {1} ativos | {2} garagens locais | {3} pontos" },
                { KeyStatusOutsideTaxisLine, "{0} de OC | {1} fontes OC" },
                { KeyStatusTaxiPurposeLine,
                    "{0} lazer | {1} casa | {2} trabalho |\n" +
                    "{3} escola | {4} compras | {5} outro"
                },
                { KeyStatusRequestsLine,
                    "{0} passageiro local ({1} bloqueado) | {2} passageiro OC ({3} bloqueado) |\n" +
                    "{4} oferta local | {5} oferta OC | {6} ponto"
                },
                { KeyStatusTaxiStandsLine, "{0} esperando" },
                { KeyStatusTaxiFleetLine,
                    "{0} corrida | {1} espera | {2} retorno |\n" +
                    "{3} dispatch | {4} a caminho | {5} estacionados"
                },
                { KeyStatusCoverageLine, "{0} local | {1} de fora | {2} turista" },
                { KeyStatusWorkDoneLine, "{0} bloqueados | {1} liberados | {2} pedidos parados" },
                { KeyStatusSnapshotLine, "Atualizado {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} cims ativos | {1} TT bloqueados | {2} IgnoreTaxi agora"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} buffer dispatch | {1} de fora | {2} desativados"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Nome exibido deste mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Versão atual do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Abre a página do autor no Paradox Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Abre o suporte da comunidade no Discord pelo navegador."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
