// <copyright file="LocaleVI.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleVI.cs
// Vietnamese (vi-VN) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleVI : IDictionarySource
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

        public LocaleVI(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Thao tác" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Trạng thái" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "Giới thiệu" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Tùy chọn taxi" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "QUÉT TAXI" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "GIAO THÔNG ĐÔ THỊ (mỗi tháng)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "CẬP NHẬT GẦN NHẤT" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "GỠ LỖI NÂNG CAO (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "THAO TÁC TRẠNG THÁI" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Thông tin" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Liên kết hỗ trợ" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Gỡ lỗi / Nhật ký" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "Cư dân tránh taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = sử dụng taxi bình thường.\n" +
                    "<25–75%> = tỷ lệ hộ gia đình địa phương tránh taxi.\n" +
                    "<100%> = mọi cư dân địa phương đủ điều kiện đều tránh taxi.\n" +
                    "**Vẫn có thể còn một số taxi. Chuyến đang chạy và taxi chờ bình thường ở điểm taxi có thể kết thúc tự nhiên; một số hệ thống game cũng có thể tự gọi taxi.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Người đi làm tránh taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**BẬT** = người đi làm tránh taxi.\n" +
                    "**TẮT** = dùng taxi bình thường.\n" +
                    "Chờ một chút để game điều chỉnh."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Du khách tránh taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**BẬT** = du khách tránh taxi.\n" +
                    "**TẮT** = du khách dùng taxi bình thường.\n" +
                    "Chờ một chút để game điều chỉnh."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Mặc định game" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Đặt cư dân về 0% và tắt tránh taxi cho người đi làm và du khách."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Hiện thông tin cập nhật" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Hiện trạng chặn hiện tại, thay đổi gần đây và thời điểm cập nhật Trạng thái."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Bật nhật ký chi tiết" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Ghi TaxiSummary định kỳ để kiểm tra.\n" +
                    "**TẮT** = chơi bình thường."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Ghi báo cáo" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Chạy quét chẩn đoán sâu và ghi toàn bộ báo cáo Trạng thái vào nhật ký mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Mở nhật ký" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Mở nhật ký của mod. Nếu không có, mở thư mục Logs."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Chưa có dữ liệu quét thành phố." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Mở thành phố, chạy mô phỏng một lúc rồi mở lại Tùy chọn → Trạng thái."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Chưa có dữ liệu quét taxi." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Mở thành phố, chạy mô phỏng một lúc rồi mở lại Tùy chọn → Trạng thái."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Chưa ghi nhận hoạt động." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Chi tiết cập nhật sẽ hiện khi Trạng thái sẵn sàng."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Hành khách hiện tại" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Người đang đi taxi.\n" +
                    "<Địa phương> = hành khách sống trong thành phố.\n" +
                    "<OC> = người đi làm và du khách từ kết nối bên ngoài.\n" +
                    "**Tổng có thể cao hơn do thú cưng đi taxi.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Nguồn taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Đỗ> = taxi đang đỗ.\n" +
                    "<Hoạt động> = taxi không đỗ, gồm cả taxi chờ ở điểm taxi.\n" +
                    "<Ga-ra địa phương> = ga-ra taxi do người chơi xây.\n" +
                    "<Điểm taxi> = khu vực đón và chờ taxi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Taxi từ bên ngoài" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<Từ OC> = taxi từ kết nối bên ngoài.\n" +
                    "<Nguồn OC> = kết nối bên ngoài có thể gửi taxi (như ga-ra vô hình).\n" +
                    "**Nếu không có ga-ra địa phương, game có thể gửi taxi OC cho yêu cầu trong thành phố.**\n" +
                    "**Khi thử nghiệm với tất cả tùy chọn tránh taxi ở mức tối đa, rất ít hoặc không có taxi OC vào thành phố.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Mục đích chuyến taxi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Mục đích của các yêu cầu taxi hiện tại.\n" +
                    "<Giải trí> | <Về nhà> | <Đi làm> | <Đi học> | <Mua sắm> | <Khác>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Công dân" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Taxi> | <Xe buýt> | <Xe điện> | <Tàu> | <Metro> | <Máy bay>\n" +
                    "**Lượt đi của công dân mỗi tháng từ chế độ xem Giao thông của game.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Du khách" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Taxi> | <Xe buýt> | <Xe điện> | <Tàu> | <Metro> | <Máy bay>\n" +
                    "**Lượt đi của du khách mỗi tháng từ chế độ xem Giao thông của game.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Tổng" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<Đang chờ> = cim đang chờ giao thông công cộng.\n" +
                    "<Du khách/tháng> và <Công dân/tháng> = tổng hành khách giao thông công cộng mỗi tháng."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Đang bị chặn" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Địa phương> | <Người đi làm> | <Du khách>\n" +
                    "**Cim đang hoạt động được Taxi Traffic đánh dấu. Không phải tổng dân số thành phố.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Thay đổi gần đây" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Đã chặn> = mới được đặt tránh taxi.\n" +
                    "<Đã bỏ chặn> = trở lại lựa chọn taxi bình thường.\n" +
                    "<Yêu cầu taxi bị dừng> = cuộc gọi taxi bị Taxi Traffic dừng."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Đã cập nhật" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Đã cập nhật> = thời điểm thông tin Trạng thái này được kiểm tra."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Ghi Trạng thái vào nhật ký" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Ghi báo cáo Trạng thái chi tiết hơn vào nhật ký Taxi Traffic.**"
                },

#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Cờ chặn (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "Kiểm tra DEV.\n" +
                    "Cim hoạt động = tác nhân cim vật lý hiện có trong mô phỏng.\n" +
                    "TT chặn = dấu sở hữu của Taxi Traffic.\n" +
                    "IgnoreTaxi hiện tại = cờ gốc của game tại thời điểm này."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Cờ taxi (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "Kiểm tra DEV.\n" +
                    "Thứ tự: Có bộ đệm điều phối | Từ bên ngoài | Vô hiệu hóa."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taxi | {1} xe buýt | {2} xe điện |\n{3} tàu | {4} metro | {5} máy bay" },
                { KeyStatusTouristsLine, "{0} taxi | {1} xe buýt | {2} xe điện |\n{3} tàu | {4} metro | {5} máy bay" },
                { KeyStatusTotalsLine, "{0} đang chờ | {1} du khách/tháng | {2} công dân/tháng" },
                { KeyStatusPassengersLine, "{0} tổng | {1} địa phương | {2} OC" },
                { KeyStatusTaxiSupplyLine, "{0} đỗ, {1} hoạt động | {2} ga-ra địa phương | {3} điểm taxi" },
                { KeyStatusOutsideTaxisLine, "{0} từ OC | {1} nguồn OC" },
                { KeyStatusTaxiPurposeLine,
                    "{0} giải trí | {1} về nhà | {2} đi làm |\n" +
                    "{3} đi học | {4} mua sắm | {5} khác"
                },
                { KeyStatusRequestsLine,
                    "{0} khách nội thành ({1} chặn) | {2} khách OC ({3} chặn) |\n" +
                    "{4} nguồn địa phương | {5} nguồn OC | {6} điểm taxi"
                },
                { KeyStatusTaxiStandsLine, "{0} đang chờ" },
                { KeyStatusTaxiFleetLine,
                    "{0} chở khách | {1} chờ | {2} quay về |\n" +
                    "{3} điều phối | {4} đang tới | {5} đỗ"
                },
                { KeyStatusCoverageLine, "{0} địa phương | {1} người đi làm | {2} du khách" },
                { KeyStatusWorkDoneLine, "{0} chặn | {1} bỏ chặn | {2} yêu cầu taxi bị dừng" },
                { KeyStatusSnapshotLine, "Cập nhật {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} cim hoạt động | {1} TT chặn | {2} IgnoreTaxi hiện tại"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} bộ đệm điều phối | {1} bên ngoài | {2} vô hiệu"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Tên hiển thị của mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Phiên bản" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Phiên bản mod hiện tại."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Mở trang tác giả trên Paradox Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Mở hỗ trợ cộng đồng Discord trong trình duyệt."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
