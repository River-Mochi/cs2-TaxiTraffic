// <copyright file="LocaleZH_HANT.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleZH_HANT.cs
// Traditional Chinese (zh-HANT) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleZH_HANT : IDictionarySource
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

        public LocaleZH_HANT(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "狀態" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "關於" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "計程車選擇" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "計程車掃描" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "城市交通（每月）" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "最近更新" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "進階除錯（DEV）" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "狀態操作" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "資訊" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "支援連結" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "除錯 / 日誌" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "居民避開計程車" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = 正常使用計程車。\n" +
                    "<25–75%> = 避開計程車的本地家庭比例。\n" +
                    "<100%> = 所有符合條件的本地居民都避開計程車。\n" +
                    "**仍可能看到少量計程車。為了讓遊戲保持穩定，Taxi Traffic 會讓進行中的行程和計程車站的正常待命自然結束。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "通勤者避開計程車" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**開** = 通勤者避開計程車。\n" +
                    "**關** = 通勤者正常使用計程車。\n"+
                    "給遊戲一點時間調整。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "遊客避開計程車" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**開** = 遊客避開計程車。\n" +
                    "**關** = 遊客正常使用計程車。\n" +
                    "給遊戲一點時間調整。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "遊戲預設值" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "將居民設為 0%，並關閉通勤者和遊客的計程車避用設定。"
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "顯示最近更新資訊" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "顯示目前封鎖、最近變更和狀態更新時間。"
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "啟用詳細日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "定期寫入 TaxiSummary 行供測試使用。\n" +
                    "**關** = 一般遊玩時使用。"
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "寫入報告" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "執行更詳細的診斷掃描，並把完整狀態報告寫入本模組日誌。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "開啟日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "開啟本模組日誌。若無法使用，則開啟 Logs 資料夾。"
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "城市掃描暫時不可用。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "開啟城市並讓模擬跑一下，然後重新開啟 選項 → 狀態。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "計程車掃描暫時不可用。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "開啟城市並讓模擬跑一下，然後重新開啟 選項 → 狀態。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "還沒有記錄到活動。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "狀態準備好後會顯示最近更新詳情。"
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "目前乘客" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "目前計程車乘客。\n" +
                    "<已封鎖> = Taxi Traffic 標記為避開計程車的乘客。\n" +
                    "<本地> = 住在你城市裡的乘客。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "計程車供應" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<已停放> = 目前停放的計程車。\n" +
                    "<活躍> = 未停放的計程車，包括在計程車站待命的車輛。\n" +
                    "<本地車庫> = 玩家建造的計程車車庫。\n" +
                    "<車站> = 指定的計程車上下客 / 等候區域。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "外來計程車" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<來自 OC> = 從外部連線來的計程車。\n" +
                    "<OC 來源> = 可以派計程車的外部連線（可以想成看不見的車庫）。\n" +
                    "**如果沒有本地車庫，遊戲可以為本地需求派來 OC 計程車。**\n" +
                    "**測試中，把滑桿 + 選項 [x] 的避開計程車設到最大後，進入城市的 OC 計程車很少或沒有。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "計程車用途" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "目前計程車需求的用途。\n" +
                    "<休閒> | <回家> | <上班> | <上學> | <購物> | <其他>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "市民" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<計程車> | <公車> | <電車> | <火車> | <捷運> | <飛機>\n" +
                    "**遊戲交通資訊檢視中的市民每月出行數。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "遊客" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<計程車> | <公車> | <電車> | <火車> | <捷運> | <飛機>\n" +
                    "**遊戲交通資訊檢視中的遊客每月出行數。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "總計" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<等待中> = 目前正在等待大眾運輸的市民。\n" +
                    "<遊客/月> 和 <市民/月> = 每月大眾運輸總乘客數。"
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "目前已封鎖" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<本地> | <通勤者> | <遊客>\n" +
                    "**Taxi Traffic 目前標記的活躍市民。這不是城市總人口。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "最近變更" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<已封鎖> = 剛設為避開計程車。\n" +
                    "<已解除> = 恢復正常計程車選擇。\n" +
                    "<已停止計程車需求> = Taxi Traffic 停止的叫車需求。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "已更新" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<已更新> = 這次狀態資訊的檢查時間。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "將狀態寫入日誌" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**把更詳細的狀態報告寫入 Taxi Traffic 日誌。**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "封鎖標記（dev）" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV 檢查。\n" +
                    "活躍市民 = 目前模擬中實際存在的市民實體。\n" +
                    "TT 封鎖 = Taxi Traffic 的所有權標記。\n" +
                    "目前 IgnoreTaxi = 此刻實際的原版標記。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "計程車標記（dev）" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV 檢查。\n" +
                    "順序：有 dispatch buffer | 來自外部 | 已停用。"
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} 計程車 | {1} 公車 | {2} 電車 |\n{3} 火車 | {4} 捷運 | {5} 飛機" },
                { KeyStatusTouristsLine, "{0} 計程車 | {1} 公車 | {2} 電車 |\n{3} 火車 | {4} 捷運 | {5} 飛機" },
                { KeyStatusTotalsLine, "{0} 等待中 | {1} 遊客/月 | {2} 市民/月" },
                { KeyStatusPassengersLine, "{0} 總計 | {1} 已封鎖 | {2} 本地" },
                { KeyStatusTaxiSupplyLine, "{0} 已停放, {1} 活躍 | {2} 本地車庫 | {3} 車站" },
                { KeyStatusOutsideTaxisLine, "{0} 來自 OC | {1} OC 來源" },
                { KeyStatusTaxiPurposeLine,
                    "{0} 休閒 | {1} 回家 | {2} 上班 |\n" +
                    "{3} 上學 | {4} 購物 | {5} 其他"
                },
                { KeyStatusRequestsLine,
                    "{0} 本地乘客 ({1} 封鎖) | {2} OC 乘客 ({3} 封鎖) |\n" +
                    "{4} 本地供應 | {5} OC 供應 | {6} 車站"
                },
                { KeyStatusTaxiStandsLine, "{0} 等待中" },
                { KeyStatusTaxiFleetLine,
                    "{0} 載客 | {1} 待命 | {2} 返回 |\n" +
                    "{3} 派車 | {4} 途中 | {5} 已停放"
                },
                { KeyStatusCoverageLine, "{0} 本地 | {1} 通勤者 | {2} 遊客" },
                { KeyStatusWorkDoneLine, "{0} 已封鎖 | {1} 已解除 | {2} 已停止叫車" },
                { KeyStatusSnapshotLine, "已更新 {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} 活躍市民 | {1} TT 封鎖 | {2} 目前 IgnoreTaxi"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} dispatch buf | {1} 外部 | {2} 已停用"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "模組" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "此模組的顯示名稱。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "目前模組版本。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "在瀏覽器中開啟作者的 Paradox Mods 頁面。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "在瀏覽器中開啟 Discord 社群支援。"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
