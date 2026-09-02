// <copyright file="LocaleZH_HANS.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleZH_HANS.cs
// Simplified Chinese (zh-HANS) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleZH_HANS : IDictionarySource
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

        public LocaleZH_HANS(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "状态" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "关于" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "出租车选择" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "出租车扫描" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "城市交通（每月）" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "最近更新" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "高级调试（DEV）" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "状态操作" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "信息" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "支持链接" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "调试 / 日志" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "居民避开出租车" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = 正常使用出租车。\n" +
                    "<25–75%> = 避开出租车的本地家庭比例。\n" +
                    "<100%> = 所有符合条件的本地居民都避开出租车。\n" +
                    "**仍可能看到少量出租车。进行中的行程和出租车站的正常待命可以自然结束，而且某些游戏系统也可能独立呼叫出租车。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "通勤者避开出租车" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**开** = 通勤者避开出租车。\n" +
                    "**关** = 通勤者正常使用出租车。\n"+
                    "给游戏一点时间调整。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "游客避开出租车" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**开** = 游客避开出租车。\n" +
                    "**关** = 游客正常使用出租车。\n" +
                    "给游戏一点时间调整。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "游戏默认值" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "将居民设为 0%，并关闭通勤者和游客的出租车规避。"
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "显示最近更新信息" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "显示当前屏蔽、最近变化和状态更新时间。"
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "启用详细日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "定期写入 TaxiSummary 行用于测试。\n" +
                    "**关** = 正常游玩时使用。"
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "写入报告" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "运行更详细的诊断扫描，并把完整状态报告写入本模组日志。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "打开日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "打开本模组日志。若不可用，则打开 Logs 文件夹。"
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "城市扫描暂不可用。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "打开城市并运行一会儿模拟，然后重新打开 选项 → 状态。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "出租车扫描暂不可用。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "打开城市并运行一会儿模拟，然后重新打开 选项 → 状态。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "还没有记录到活动。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "状态准备好后会显示最近更新详情。"
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "当前乘客" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "当前出租车乘客。\n" +
                    "<本地> = 住在你城市里的乘客。\n" +
                    "<OC> = 来自外部连接的通勤者和游客。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "出租车供应" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<已停放> = 当前停放的出租车。\n" +
                    "<活跃> = 未停放的出租车，包括在出租车站待命的车辆。\n" +
                    "<本地车库> = 玩家建造的出租车车库。\n" +
                    "<车站> = 指定的出租车上下客/等候区域。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "外来出租车" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<来自 OC> = 从外部连接来的出租车。\n" +
                    "<OC 来源> = 可以派出租车的外部连接（可以理解成看不见的车库）。\n" +
                    "**如果没有本地车库，游戏可以为本地请求派来 OC 出租车。**\n" +
                    "**测试中，将出租车规避设到最大后，进入城市的 OC 出租车很少或没有。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "出租车用途" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "当前出租车请求的用途。\n" +
                    "<休闲> | <回家> | <上班> | <上学> | <购物> | <其他>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "市民" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<出租车> | <公交> | <电车> | <火车> | <地铁> | <飞机>\n" +
                    "**游戏交通信息视图中的市民每月出行数。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "游客" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<出租车> | <公交> | <电车> | <火车> | <地铁> | <飞机>\n" +
                    "**游戏交通信息视图中的游客每月出行数。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "总计" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<等待中> = 当前正在等待公共交通的市民。\n" +
                    "<游客/月> 和 <市民/月> = 每月公共交通总乘客数。"
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "当前已屏蔽" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<本地> | <通勤者> | <游客>\n" +
                    "**Taxi Traffic 当前标记的活跃市民。这不是城市总人口。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "最近变化" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<已屏蔽> = 刚设为避开出租车。\n" +
                    "<已解除> = 恢复正常出租车选择。\n" +
                    "<已停止出租车请求> = Taxi Traffic 停止的叫车请求。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "已更新" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<已更新> = 这次状态信息的检查时间。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "将状态写入日志" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**把更详细的状态报告写入 Taxi Traffic 日志。**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "屏蔽标记（dev）" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV 检查。\n" +
                    "活跃市民 = 当前模拟中实际存在的市民实体。\n" +
                    "TT 屏蔽 = Taxi Traffic 的所有权标记。\n" +
                    "当前 IgnoreTaxi = 此刻实际的原版标志。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "出租车标志（dev）" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV 检查。\n" +
                    "顺序：有 dispatch buffer | 来自外部 | 已禁用。"
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} 出租车 | {1} 公交 | {2} 电车 |\n{3} 火车 | {4} 地铁 | {5} 飞机" },
                { KeyStatusTouristsLine, "{0} 出租车 | {1} 公交 | {2} 电车 |\n{3} 火车 | {4} 地铁 | {5} 飞机" },
                { KeyStatusTotalsLine, "{0} 等待中 | {1} 游客/月 | {2} 市民/月" },
                { KeyStatusPassengersLine, "{0} 总计 | {1} 本地 | {2} OC" },
                { KeyStatusTaxiSupplyLine, "{0} 已停放, {1} 活跃 | {2} 本地车库 | {3} 车站" },
                { KeyStatusOutsideTaxisLine, "{0} 来自 OC | {1} OC 来源" },
                { KeyStatusTaxiPurposeLine,
                    "{0} 休闲 | {1} 回家 | {2} 上班 |\n" +
                    "{3} 上学 | {4} 购物 | {5} 其他"
                },
                { KeyStatusRequestsLine,
                    "{0} 本地乘客 ({1} 屏蔽) | {2} OC 乘客 ({3} 屏蔽) |\n" +
                    "{4} 本地供应 | {5} OC 供应 | {6} 车站"
                },
                { KeyStatusTaxiStandsLine, "{0} 等待中" },
                { KeyStatusTaxiFleetLine,
                    "{0} 载客 | {1} 待命 | {2} 返回 |\n" +
                    "{3} 派车 | {4} 途中 | {5} 已停放"
                },
                { KeyStatusCoverageLine, "{0} 本地 | {1} 通勤者 | {2} 游客" },
                { KeyStatusWorkDoneLine, "{0} 已屏蔽 | {1} 已解除 | {2} 已停止叫车" },
                { KeyStatusSnapshotLine, "已更新 {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} 活跃市民 | {1} TT 屏蔽 | {2} 当前 IgnoreTaxi"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} dispatch buf | {1} 外部 | {2} 已禁用"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "模组" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "此模组的显示名称。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "当前模组版本。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "在浏览器中打开作者的 Paradox Mods 页面。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "在浏览器中打开 Discord 社区支持。"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
