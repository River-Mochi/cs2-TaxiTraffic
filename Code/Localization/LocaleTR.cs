// <copyright file="LocaleTR.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleTR.cs
// Purpose: Turkish (tr-TR) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleTR : IDictionarySource
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

        public LocaleTR(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "Eylemler" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "Durum" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "Hakkında" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "Taksi Seçimleri" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "TAKSİ TARAMASI" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "ŞEHİR ULAŞIMI (aylık)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "SON GÜNCELLEME" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "GELİŞMİŞ HATA AYIKLAMA (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "DURUM EYLEMLERİ" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "Bilgi" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "Destek Bağlantıları" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "Hata Ayıklama / Günlük" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "Sakinler taksiden kaçınır" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = normal taksi kullanımı.\n" +
                    "<25–75%> = taksiden kaçınan yerel hanelerin yüzdesi.\n" +
                    "<100%> = uygun tüm yerel sakinler taksiden kaçınır.\n" +
                    "**Bazı taksiler yine de kalabilir. Aktif yolculuklar ve duraktaki normal bekleyişler doğal olarak bitebilir; bazı oyun sistemleri ayrıca taksi çağırabilir.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "Gidip gelenler taksiden kaçınır" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**AÇIK** = gidip gelenler taksiden kaçınır.\n" +
                    "**KAPALI** = normal gidip gelen taksi kullanımı.\n"+
                    "Uyum sağlaması için biraz zaman verin."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "Turistler taksiden kaçınır" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**AÇIK** = turistler taksiden kaçınır.\n" +
                    "**KAPALI** = normal turist taksi kullanımı.\n" +
                    "Uyum sağlaması için biraz zaman verin."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "Oyun Varsayılanları" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "Sakinlerin taksiden kaçınmasını %0 yapar; gidip gelen ve turist kaçınmasını KAPALI yapar."
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "Son güncelleme bilgisini göster" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "Mevcut engellemeyi, son değişiklikleri ve Durum zamanını gösterir."
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "Ayrıntılı günlüğü aç" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "Test için düzenli TaxiSummary satırları yazar.\n" +
                    "**KAPALI** = normal oyun için kullanın."
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "Rapor Yaz" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "Ayrıntılı tanı taraması yapar ve tam Durum raporunu mod günlüğüne yazar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "Günlüğü Aç" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "Mod günlüğünü açar. Yoksa Logs klasörünü açar."
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "Şehir taraması henüz hazır değil." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "Bir şehir açın, simülasyonu çalıştırın, sonra Seçenekler → Durum'u yeniden açın."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "Taksi taraması henüz hazır değil." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "Bir şehir açın, simülasyonu çalıştırın, sonra Seçenekler → Durum'u yeniden açın."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "Henüz etkinlik kaydedilmedi." },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "Son güncelleme ayrıntıları Durum hazır olduğunda görünür."
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "Mevcut yolcular" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "Mevcut taksi yolcuları.\n" +
                    "<Yerel> = şehrinizde yaşayan yolcular.\n" +
                    "<DB> = dış bağlantıdan gelen gidip gelenler ve turistler.\n" +
                    "**Taksilerdeki evcil hayvanlar nedeniyle toplam daha yüksek olabilir.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "Taksi arzı" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<Park> = şu anda park etmiş taksiler.\n" +
                    "<Aktif> = parkta olmayan, durakta bekleyenler dahil taksiler.\n" +
                    "<Yerel depolar> = oyuncunun yaptığı taksi depoları.\n" +
                    "<Duraklar> = taksi alma/bekleme alanları."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "Dış taksiler" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<DB'den> = dış bağlantılardan gelen taksiler.\n" +
                    "<DB kaynakları> = taksi gönderebilen dış bağlantılar (görünmez depolar gibi).\n" +
                    "**Yerel depo yoksa oyun yerel istekler için DB taksileri gönderebilir.**\n" +
                    "**Testlerde tüm kaçınma seçenekleri en yüksekteyken şehre çok az veya hiç DB taksisi girmedi.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "Taksi amacı" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "Mevcut taksi isteklerinin amacı.\n" +
                    "<Eğlence> | <Ev> | <İş> | <Okul> | <Alışveriş> | <Diğer>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "Vatandaşlar" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<Taksi> | <Otobüs> | <Tramvay> | <Tren> | <Metro> | <Hava>\n" +
                    "**Oyunun Ulaşım Bilgi Görünümündeki aylık vatandaş yolculukları.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "Turistler" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<Taksi> | <Otobüs> | <Tramvay> | <Tren> | <Metro> | <Hava>\n" +
                    "**Oyunun Ulaşım Bilgi Görünümündeki aylık turist yolculukları.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "Toplamlar" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<Bekleyen> = toplu taşıma bekleyen cimler.\n" +
                    "<Turist/ay> ve <Vatandaş/ay> = aylık toplam toplu taşıma yolcusu."
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "Şimdi engelli" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<Yerel> | <Gidip gelen> | <Turist>\n" +
                    "**Taxi Traffic tarafından işaretlenmiş aktif cimler. Şehrin toplam nüfusu değildir.**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "Son değişiklikler" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<Engelli> = yeni taksiden kaçınanlar.\n" +
                    "<Serbest> = normal taksi seçimine dönenler.\n" +
                    "<Durdurulan istek> = Taxi Traffic'in engellediği taksi çağrıları."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "Güncellendi" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<Güncellendi> = bu Durum bilgisinin kontrol zamanı."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "Durumu Günlüğe Yaz" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**Daha ayrıntılı Durum raporunu Taxi Traffic günlüğüne yazar.**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "Engel bayrakları (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "DEV kontrolü.\n" +
                    "Aktif cimler = simülasyondaki fiziksel cim ajanları.\n" +
                    "TT engelli = Taxi Traffic sahiplik işareti.\n" +
                    "IgnoreTaxi şimdi = o andaki gerçek vanilla bayrağı."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "Taksi bayrakları (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "DEV kontrolü.\n" +
                    "Sıra: Dispatch tamponu | Dışarıdan | Devre dışı."
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} taksi | {1} otobüs | {2} tramvay |\n{3} tren | {4} metro | {5} hava" },
                { KeyStatusTouristsLine, "{0} taksi | {1} otobüs | {2} tramvay |\n{3} tren | {4} metro | {5} hava" },
                { KeyStatusTotalsLine, "{0} bekler | {1} turist/ay | {2} vatandaş/ay" },
                { KeyStatusPassengersLine, "{0} toplam | {1} yerel | {2} DB" },
                { KeyStatusTaxiSupplyLine, "{0} park, {1} aktif | {2} depo | {3} durak" },
                { KeyStatusOutsideTaxisLine, "{0} DB'den | {1} DB kaynağı" },
                { KeyStatusTaxiPurposeLine,
                    "{0} eğl. | {1} ev | {2} iş |\n" +
                    "{3} okul | {4} alışv. | {5} diğer"
                },
                { KeyStatusRequestsLine,
                    "{0} şehir ({1} eng.) | {2} DB ({3} eng.) |\n" +
                    "{4} yerel arz | {5} DB arz | {6} durak"
                },
                { KeyStatusTaxiStandsLine, "{0} bekler" },
                { KeyStatusTaxiFleetLine,
                    "{0} yolcu | {1} bekle | {2} dönüş |\n" +
                    "{3} sevk | {4} yolda | {5} park"
                },
                { KeyStatusCoverageLine, "{0} yerel | {1} gidip-gelen | {2} turist" },
                { KeyStatusWorkDoneLine, "{0} eng. | {1} serbest | {2} istek durdu" },
                { KeyStatusSnapshotLine, "Güncellendi {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} aktif cim | {1} TT eng. | {2} IgnoreTaxi"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} disp. buf | {1} dış | {2} kapalı"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "Bu modun görünen adı."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "Sürüm" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "Mevcut mod sürümü."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "Yazarın Paradox Mods sayfasını açar."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "Discord topluluk desteğini tarayıcıda açar."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
