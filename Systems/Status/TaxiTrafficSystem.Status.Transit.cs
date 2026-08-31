// <copyright file="TaxiTrafficSystem.Status.Transit.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Systems/Status/TaxiTrafficSystem.Status.Transit.cs
// Game's monthly transportation InfoView passenger Status values.

namespace TaxiTraffic
{
    public partial class TaxiTrafficSystem
    {
        private void UpdateStatusMonthlyPassengers()
        {
            if (m_CityStatisticsSystem == null)
                return;

            s_InfoTaxiTourist = 0;
            s_InfoTaxiCitizen = 0;
            s_InfoBusTourist = 0;
            s_InfoBusCitizen = 0;
            s_InfoTramTourist = 0;
            s_InfoTramCitizen = 0;
            s_InfoTrainTourist = 0;
            s_InfoTrainCitizen = 0;
            s_InfoSubwayTourist = 0;
            s_InfoSubwayCitizen = 0;
            s_InfoShipTourist = 0;
            s_InfoShipCitizen = 0;
            s_InfoFerryTourist = 0;
            s_InfoFerryCitizen = 0;
            s_InfoAirTourist = 0;
            s_InfoAirCitizen = 0;
            s_InfoTotalTourist = 0;
            s_InfoTotalCitizen = 0;

            if (m_TransportConfig == null && m_PrefabSystem != null)
            {
                try
                {
                    m_TransportConfig =
                        m_PrefabSystem.GetSingletonPrefab<
                            Game.Prefabs.UITransportConfigurationPrefab>(
                            m_TransportConfigQuery);
                }
                catch
                {
                    m_TransportConfig = null;
                }
            }

            if (m_TransportConfig == null)
                return;

            Game.Prefabs.UITransportSummaryItem[] items =
                m_TransportConfig.m_PassengerSummaryItems;

            for (int i = 0; i < items.Length; i++)
            {
                Game.Prefabs.UITransportSummaryItem item = items[i];

                int citizen;
                int tourist;

                try
                {
                    citizen =
                        m_CityStatisticsSystem.GetStatisticValue(
                            item.m_Statistic);

                    tourist =
                        m_CityStatisticsSystem.GetStatisticValue(
                            item.m_Statistic,
                            1);
                }
                catch
                {
                    continue;
                }

                s_InfoTotalCitizen += citizen;
                s_InfoTotalTourist += tourist;

                switch (item.m_Type)
                {
                    case Game.Prefabs.TransportType.Taxi:
                        s_InfoTaxiCitizen = citizen;
                        s_InfoTaxiTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Bus:
                        s_InfoBusCitizen = citizen;
                        s_InfoBusTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Tram:
                        s_InfoTramCitizen = citizen;
                        s_InfoTramTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Train:
                        s_InfoTrainCitizen = citizen;
                        s_InfoTrainTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Subway:
                        s_InfoSubwayCitizen = citizen;
                        s_InfoSubwayTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Ship:
                        s_InfoShipCitizen = citizen;
                        s_InfoShipTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Ferry:
                        s_InfoFerryCitizen = citizen;
                        s_InfoFerryTourist = tourist;
                        break;

                    case Game.Prefabs.TransportType.Airplane:
                        s_InfoAirCitizen = citizen;
                        s_InfoAirTourist = tourist;
                        break;
                }
            }
        }

    }
}
