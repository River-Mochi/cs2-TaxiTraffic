// <copyright file="LocaleES.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleES.cs
// Spanish (es-ES) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleES : IDictionarySource
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

        public LocaleES(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Acciones" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Estado" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Opciones de taxi" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "ESCANEO DE TAXIS" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "TRANSPORTE URBANO (por mes)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "ÚLTIMA ACTUALIZACIÓN" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "DEPURACIÓN AVANZADA (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "ACCIONES DE ESTADO" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Info" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Enlaces de ayuda" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Depuración / Registro" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "Los residentes evitan taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = uso normal del taxi.\n" +
                    "<25–75%> = porcentaje de hogares locales que evitan taxis.\n" +
                    "<100%> = todos los residentes locales elegibles evitan taxis.\n" +
                    "**Puede que aún queden algunos taxis. Taxi Traffic deja terminar los viajes activos y la espera normal en paradas de taxi para mantener estable el juego.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Los viajeros diarios evitan taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**ON** = los viajeros diarios evitan taxis.\n" +
                    "**OFF** = uso normal del taxi para viajeros diarios.\n"+
                    "Dale un poco de tiempo para ajustarse."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Los turistas evitan taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**ON** = los turistas evitan taxis.\n" +
                    "**OFF** = uso normal del taxi para turistas.\n" +
                    "Dale un poco de tiempo para ajustarse."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Valores del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Pone Residentes en 0% y desactiva evitar taxis para viajeros diarios y turistas."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Mostrar última actualización" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Muestra bloqueos actuales, cambios recientes y la hora de la captura de Estado."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Activar registro detallado" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Escribe líneas TaxiSummary periódicas para pruebas.\n" +
                    "**OFF** = úsalo para jugar normalmente."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Escribir informe" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Ejecuta el escaneo de diagnóstico completo y escribe el informe de Estado en el registro del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Abrir registro" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Abre el registro del mod. Si no está disponible, abre la carpeta Logs."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Escaneo de ciudad aún no disponible." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Abre una ciudad, deja correr la simulación y vuelve a abrir Opciones → Estado."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Escaneo de taxis aún no disponible." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Abre una ciudad, deja correr la simulación y vuelve a abrir Opciones → Estado."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Aún no hay actividad registrada." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Los detalles de la última actualización aparecen cuando el Estado está listo."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Pasajeros actuales" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Pasajeros actuales en taxi.\n" +
                    "<Bloqueados> = pasajeros a los que Taxi Traffic indicó evitar taxis.\n" +
                    "<Locales> = pasajeros que viven en tu ciudad."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Oferta de taxis" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Aparcados> = taxis aparcados ahora.\n" +
                    "<Activos> = taxis no aparcados, incluidos los que esperan en paradas de taxi.\n" +
                    "<Depósitos locales> = depósitos de taxis construidos por el jugador.\n" +
                    "<Paradas> = zonas designadas para recoger o esperar taxis."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Taxis de fuera" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<Desde OC> = taxis que llegan desde conexiones exteriores.\n" +
                    "<Fuentes OC> = conexiones exteriores que pueden enviar taxis (como depósitos invisibles).\n" +
                    "**Si no hay depósitos locales, el juego puede enviar taxis OC para solicitudes locales.**\n" +
                    "**En pruebas, con el control y las opciones [x] de evitar taxis al máximo, entraron pocos o ningún taxi OC a la ciudad.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Motivo del taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Motivo de las solicitudes de taxi actuales.\n" +
                    "<Ocio> | <Casa> | <Trabajo> | <Escuela> | <Compras> | <Otro>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Ciudadanos" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Taxi> | <Bus> | <Tranvía> | <Tren> | <Metro> | <Avión>\n" +
                    "**Viajes de ciudadanos por mes según la vista de Transporte del juego.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Turistas" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Taxi> | <Bus> | <Tranvía> | <Tren> | <Metro> | <Avión>\n" +
                    "**Viajes de turistas por mes según la vista de Transporte del juego.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Totales" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<Esperando> = cims que esperan transporte público ahora.\n" +
                    "<Turistas/mes> y <Ciudadanos/mes> = total de pasajeros de transporte público por mes."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Bloqueados ahora" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Local> | <Viajero> | <Turista>\n" +
                    "**Cims activos marcados ahora por Taxi Traffic. No es la población total de la ciudad.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Cambios recientes" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Bloqueados> = recién configurados para evitar taxis.\n" +
                    "<Desbloqueados> = vuelven a la elección normal de taxi.\n" +
                    "<Solicitudes de taxi detenidas> = llamadas de taxi detenidas por Taxi Traffic."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Actualizado" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Actualizado> = cuándo se revisó esta información de Estado."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Escribir Estado en el registro" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Escribe un informe de Estado más detallado en el registro de Taxi Traffic.**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Marcas de bloqueo (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "Comprobación DEV.\n" +
                    "Cims activos = agentes físicos de cims actualmente en la simulación.\n" +
                    "TT bloqueado = marcador de propiedad de Taxi Traffic.\n" +
                    "IgnoreTaxi ahora = valor vanilla real en este instante."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Marcas de taxi (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "Comprobación DEV.\n" +
                    "Orden: Con búfer de despacho | Desde fuera | Desactivado."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taxi | {1} bus | {2} tranvía |\n{3} tren | {4} metro | {5} avión" },
                { KeyStatusTouristsLine, "{0} taxi | {1} bus | {2} tranvía |\n{3} tren | {4} metro | {5} avión" },
                { KeyStatusTotalsLine, "{0} esperando | {1} turistas/mes | {2} ciudadanos/mes" },
                { KeyStatusPassengersLine, "{0} total | {1} bloqueados | {2} locales" },
                { KeyStatusTaxiSupplyLine, "{0} aparcados, {1} activos | {2} depósitos locales | {3} paradas" },
                { KeyStatusOutsideTaxisLine, "{0} desde OC | {1} fuentes OC" },
                { KeyStatusTaxiPurposeLine,
                    "{0} ocio | {1} casa | {2} trabajo |\n" +
                    "{3} escuela | {4} compras | {5} otro"
                },
                { KeyStatusRequestsLine,
                    "{0} pasajero ciudad ({1} bloqueado) | {2} pasajero OC ({3} bloqueado) |\n" +
                    "{4} oferta local | {5} oferta OC | {6} parada"
                },
                { KeyStatusTaxiStandsLine, "{0} esperando" },
                { KeyStatusTaxiFleetLine,
                    "{0} viaje | {1} espera | {2} regreso |\n" +
                    "{3} despacho | {4} en ruta | {5} aparcados"
                },
                { KeyStatusCoverageLine, "{0} local | {1} viajero | {2} turista" },
                { KeyStatusWorkDoneLine, "{0} bloqueados | {1} desbloqueados | {2} solicitudes detenidas" },
                { KeyStatusSnapshotLine, "Actualizado {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} cims activos | {1} TT bloqueados | {2} IgnoreTaxi ahora"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} búfer despacho | {1} exterior | {2} desactivado"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Nombre mostrado de este mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Versión actual del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Abre la página del autor en Paradox Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Abre el soporte de la comunidad en Discord desde el navegador."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
