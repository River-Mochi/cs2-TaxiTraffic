// <copyright file="LocaleKO.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleKO.cs
// Korean (ko-KR) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleKO : IDictionarySource
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

        public LocaleKO(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "동작" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "상태" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "정보" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "택시 선택" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "택시 현황" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "도시 교통 (월간)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "최근 업데이트" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "고급 디버그 (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "상태 작업" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "정보" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "지원 링크" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "디버그 / 로그" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "주민이 택시를 피함" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = 일반 택시 이용.\n" +
                    "<25–75%> = 택시를 피하는 지역 가구 비율.\n" +
                    "<100%> = 대상 지역 주민 모두 택시를 피함.\n" +
                    "**일부 택시는 여전히 남아 있을 수 있습니다. 진행 중인 탑승과 택시 승강장의 일반 대기는 자연스럽게 끝날 수 있으며, 일부 게임 시스템이 독립적으로 택시를 호출할 수도 있습니다.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "통근자가 택시를 피함" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**켜기** = 통근자가 택시를 피함.\n" +
                    "**끄기** = 통근자는 일반적으로 택시 이용.\n"+
                    "반영될 때까지 잠시 기다려 주세요."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "관광객이 택시를 피함" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**켜기** = 관광객이 택시를 피함.\n" +
                    "**끄기** = 관광객은 일반적으로 택시 이용.\n" +
                    "반영될 때까지 잠시 기다려 주세요."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "게임 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "주민을 0%로 설정하고 통근자와 관광객의 택시 회피를 끕니다."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "최근 업데이트 정보 표시" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "현재 차단, 최근 변경, 상태 갱신 시간을 표시합니다."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "상세 로그 켜기" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "테스트용 TaxiSummary 줄을 주기적으로 기록합니다.\n" +
                    "**끄기** = 일반 플레이용."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "보고서 기록" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "상세 진단 스캔을 실행하고 전체 상태 보고서를 모드 로그에 기록합니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "로그 열기" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "모드 로그를 엽니다. 사용할 수 없으면 Logs 폴더를 엽니다."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "도시 스캔을 아직 사용할 수 없습니다." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "도시를 열고 시뮬레이션을 잠시 실행한 뒤 옵션 → 상태를 다시 여세요."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "택시 스캔을 아직 사용할 수 없습니다." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "도시를 열고 시뮬레이션을 잠시 실행한 뒤 옵션 → 상태를 다시 여세요."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "아직 기록된 활동이 없습니다." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "상태가 준비되면 최근 업데이트 정보가 표시됩니다."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "현재 승객" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "현재 택시 승객.\n" +
                    "<지역> = 내 도시에 사는 승객.\n" +
                    "<OC> = 외부 연결에서 온 통근자와 관광객."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "택시 공급" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<주차> = 현재 주차된 택시.\n" +
                    "<활성> = 주차되지 않은 택시. 택시 승강장에서 대기 중인 택시도 포함.\n" +
                    "<지역 차고지> = 플레이어가 지은 택시 차고지.\n" +
                    "<승강장> = 택시 승차/대기 구역."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "외부 택시" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<OC에서> = 외부 연결에서 오는 택시.\n" +
                    "<OC 공급원> = 택시를 보낼 수 있는 외부 연결(보이지 않는 차고지처럼 생각하면 됩니다).\n" +
                    "**지역 차고지가 없으면 게임이 지역 요청에 OC 택시를 보낼 수 있습니다.**\n" +
                    "**테스트에서는 택시 회피를 최대로 설정했을 때 도시로 들어오는 OC 택시가 거의 또는 전혀 없었습니다.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "택시 목적" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "현재 택시 요청의 목적.\n" +
                    "<여가> | <귀가> | <직장> | <학교> | <쇼핑> | <기타>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "시민" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<택시> | <버스> | <트램> | <기차> | <지하철> | <항공>\n" +
                    "**게임 교통 정보 보기의 시민 월간 이용 수.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "관광객" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<택시> | <버스> | <트램> | <기차> | <지하철> | <항공>\n" +
                    "**게임 교통 정보 보기의 관광객 월간 이용 수.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "합계" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<대기> = 현재 대중교통을 기다리는 시민.\n" +
                    "<관광객/월> 및 <시민/월> = 월간 대중교통 총 승객 수."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "현재 차단" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<지역> | <통근자> | <관광객>\n" +
                    "**Taxi Traffic이 현재 표시한 활성 시민입니다. 도시 전체 인구가 아닙니다.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "최근 변경" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<차단> = 새로 택시를 피하도록 설정됨.\n" +
                    "<해제> = 일반 택시 선택으로 돌아감.\n" +
                    "<중단된 택시 요청> = Taxi Traffic이 중단한 택시 호출."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "업데이트" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<업데이트> = 이 상태 정보를 확인한 시간."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "상태를 로그에 기록" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Taxi Traffic 로그에 더 자세한 상태 보고서를 기록합니다.**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "차단 플래그 (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV 확인용.\n" +
                    "활성 시민 = 현재 시뮬레이션에 실제로 존재하는 시민 에이전트.\n" +
                    "TT 차단 = Taxi Traffic 소유 마커.\n" +
                    "현재 IgnoreTaxi = 이 순간의 실제 기본 게임 플래그."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "택시 플래그 (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV 확인용.\n" +
                    "순서: dispatch buffer 있음 | 외부에서 | 비활성."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} 택시 | {1} 버스 | {2} 트램 |\n{3} 기차 | {4} 지하철 | {5} 항공" },
                { KeyStatusTouristsLine, "{0} 택시 | {1} 버스 | {2} 트램 |\n{3} 기차 | {4} 지하철 | {5} 항공" },
                { KeyStatusTotalsLine, "{0} 대기 | {1} 관광객/월 | {2} 시민/월" },
                { KeyStatusPassengersLine, "{0} 합계 | {1} 지역 | {2} OC" },
                { KeyStatusTaxiSupplyLine, "{0} 주차, {1} 활성 | {2} 지역 차고지 | {3} 승강장" },
                { KeyStatusOutsideTaxisLine, "{0} OC에서 | {1} OC 공급원" },
                { KeyStatusTaxiPurposeLine,
                    "{0} 여가 | {1} 귀가 | {2} 직장 |\n" +
                    "{3} 학교 | {4} 쇼핑 | {5} 기타"
                },
                { KeyStatusRequestsLine,
                    "{0} 지역 승객 ({1} 차단) | {2} OC 승객 ({3} 차단) |\n" +
                    "{4} 지역 공급 | {5} OC 공급 | {6} 승강장"
                },
                { KeyStatusTaxiStandsLine, "{0} 대기" },
                { KeyStatusTaxiFleetLine,
                    "{0} 운행 | {1} 대기 | {2} 복귀 |\n" +
                    "{3} 배차 | {4} 이동 중 | {5} 주차"
                },
                { KeyStatusCoverageLine, "{0} 지역 | {1} 통근자 | {2} 관광객" },
                { KeyStatusWorkDoneLine, "{0} 차단 | {1} 해제 | {2} 택시 요청 중단" },
                { KeyStatusSnapshotLine, "업데이트 {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} 활성 시민 | {1} TT 차단 | {2} IgnoreTaxi"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} dispatch buf | {1} 외부 | {2} 비활성"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "이 모드의 표시 이름."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "현재 모드 버전."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "브라우저에서 Paradox Mods의 제작자 페이지를 엽니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "브라우저에서 Discord 커뮤니티 지원을 엽니다."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
