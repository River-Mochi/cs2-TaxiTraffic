// <copyright file="LocaleJA.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleJA.cs
// Japanese (ja-JP) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleJA : IDictionarySource
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

        public LocaleJA(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "ステータス" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "タクシー利用" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "タクシー状況" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "都市交通（月間）" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "最終更新" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "高度なデバッグ (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "ステータス操作" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "情報" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "サポート" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "デバッグ / ログ" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "住民はタクシーを避ける" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = 通常のタクシー利用。\n" +
                    "<25–75%> = タクシーを避ける市内世帯の割合。\n" +
                    "<100%> = 対象となる市内住民全員がタクシーを避ける。\n" +
                    "**一部のタクシーは残ることがあります。進行中の乗車やタクシー乗り場での通常待機は自然に完了し、ゲームの一部システムが独自にタクシーを呼ぶ場合もあります。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "通勤者はタクシーを避ける" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**オン** = 通勤者はタクシーを避ける。\n" +
                    "**オフ** = 通勤者は通常どおりタクシーを利用。\n"+
                    "反映まで少し待ってください。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "観光客はタクシーを避ける" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**オン** = 観光客はタクシーを避ける。\n" +
                    "**オフ** = 観光客は通常どおりタクシーを利用。\n" +
                    "反映まで少し待ってください。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "ゲーム標準" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "住民を0%にし、通勤者と観光客のタクシー回避をオフにします。"
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "最終更新情報を表示" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "現在のブロック、最近の変更、ステータス更新時刻を表示します。"
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "詳細ログを有効化" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "テスト用のTaxiSummaryを定期的にログへ出力します。\n" +
                    "**オフ** = 通常プレイ向け。"
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "レポートを書き出す" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "詳しい診断スキャンを実行し、完全なステータスレポートをModログへ出力します。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "ログを開く" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Modのログを開きます。利用できない場合はLogsフォルダーを開きます。"
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "都市スキャンはまだ利用できません。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "都市を開いてシミュレーションを少し進め、オプション → ステータスを開き直してください。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "タクシースキャンはまだ利用できません。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "都市を開いてシミュレーションを少し進め、オプション → ステータスを開き直してください。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "まだ記録された動きはありません。" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "ステータスの準備ができると最終更新の詳細が表示されます。"
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "現在の乗客" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "現在タクシーに乗っている乗客。\n" +
                    "<市内> = あなたの都市に住む乗客。\n" +
                    "<OC> = 都市外接続からの通勤者と観光客。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "タクシー供給" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<駐車中> = 現在駐車しているタクシー。\n" +
                    "<稼働中> = 駐車していないタクシー。乗り場で待機中も含みます。\n" +
                    "<市内営業所> = プレイヤーが建てたタクシー営業所。\n" +
                    "<乗り場> = タクシーの乗車・待機用エリア。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "外部タクシー" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<OCから> = 外部接続から来たタクシー。\n" +
                    "<OC供給元> = タクシーを送れる外部接続（見えない営業所のようなもの）。\n" +
                    "**市内営業所がない場合、ゲームは市内の依頼にOCタクシーを送ることがあります。**\n" +
                    "**テストでは、タクシー回避を最大にすると、OCタクシーの市内流入はほぼ、またはまったくありませんでした。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "タクシー利用目的" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "現在のタクシー依頼の目的。\n" +
                    "<レジャー> | <帰宅> | <仕事> | <学校> | <買い物> | <その他>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "市民" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<タクシー> | <バス> | <トラム> | <鉄道> | <地下鉄> | <航空>\n" +
                    "**ゲームの交通情報ビューにある市民の月間利用数。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "観光客" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<タクシー> | <バス> | <トラム> | <鉄道> | <地下鉄> | <航空>\n" +
                    "**ゲームの交通情報ビューにある観光客の月間利用数。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "合計" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<待機中> = 現在公共交通を待っている市民。\n" +
                    "<観光客/月> と <市民/月> = 公共交通の月間総乗客数。"
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "現在ブロック中" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<市内> | <通勤者> | <観光客>\n" +
                    "**Taxi Trafficが現在マークしているアクティブな市民。都市の総人口ではありません。**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "最近の変更" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<ブロック> = 新たにタクシー回避へ設定。\n" +
                    "<解除> = 通常のタクシー選択へ戻した。\n" +
                    "<停止したタクシー依頼> = Taxi Trafficが止めたタクシー呼び出し。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "更新" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<更新> = このステータス情報を確認した時刻。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "ステータスをログに出力" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Taxi Trafficのログへ詳しいステータスレポートを書き出します。**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "ブロックフラグ (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV確認用。\n" +
                    "アクティブ市民 = 現在シミュレーション内にいる実体の市民エージェント。\n" +
                    "TTブロック = Taxi Trafficの所有マーカー。\n" +
                    "現在のIgnoreTaxi = この時点の実際のバニラフラグ。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "タクシーフラグ (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV確認用。\n" +
                    "順番: dispatch bufferあり | 外部から | 無効。"
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} タクシー | {1} バス | {2} トラム |\n{3} 鉄道 | {4} 地下鉄 | {5} 航空" },
                { KeyStatusTouristsLine, "{0} タクシー | {1} バス | {2} トラム |\n{3} 鉄道 | {4} 地下鉄 | {5} 航空" },
                { KeyStatusTotalsLine, "{0} 待機中 | {1} 観光客/月 | {2} 市民/月" },
                { KeyStatusPassengersLine, "{0} 合計 | {1} 市内 | {2} OC" },
                { KeyStatusTaxiSupplyLine, "{0} 駐車中, {1} 稼働中 | {2} 市内営業所 | {3} 乗り場" },
                { KeyStatusOutsideTaxisLine, "{0} OCから | {1} OC供給元" },
                { KeyStatusTaxiPurposeLine,
                    "{0} レジャー | {1} 帰宅 | {2} 仕事 |\n" +
                    "{3} 学校 | {4} 買い物 | {5} その他"
                },
                { KeyStatusRequestsLine,
                    "{0} 市内客 ({1} ブロック) | {2} OC客 ({3} ブロック) |\n" +
                    "{4} 市内供給 | {5} OC供給 | {6} 乗り場"
                },
                { KeyStatusTaxiStandsLine, "{0} 待機中" },
                { KeyStatusTaxiFleetLine,
                    "{0} 乗車中 | {1} 待機 | {2} 帰還 |\n" +
                    "{3} 配車 | {4} 移動中 | {5} 駐車"
                },
                { KeyStatusCoverageLine, "{0} 市内 | {1} 通勤者 | {2} 観光客" },
                { KeyStatusWorkDoneLine, "{0} ブロック | {1} 解除 | {2} タクシー依頼停止" },
                { KeyStatusSnapshotLine, "更新 {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} アクティブ市民 | {1} TTブロック | {2} IgnoreTaxi"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} dispatch buf | {1} 外部 | {2} 無効"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "このModの表示名。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "現在のModバージョン。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Paradox Modsの作者ページをブラウザーで開きます。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Discordのコミュニティサポートをブラウザーで開きます。"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
