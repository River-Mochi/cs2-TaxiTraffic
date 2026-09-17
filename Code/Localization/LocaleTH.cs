// <copyright file="LocaleTH.cs" company="River-Mochi">
// Copyright (c) 2026 River-Mochi. All rights reserved.
// Licensed under the GNU General Public License v3.0 or later,
// with the Cities: Skylines II Linking Exception.
// See LICENSE and LICENSE-EXCEPTION in the project root.
// This notice MUST be kept with copies or substantial portions of this code.
// ================= </copyright> ======================

// File: Localization/LocaleTH.cs
// Purpose: Thai (th-TH) Options UI text and status format strings.

namespace TaxiTraffic
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleTH : IDictionarySource
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

        public LocaleTH(TaxiSettings setting)
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
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.ActionsTab), "การทำงาน" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.StatusTab), "สถานะ" },
                { m_Setting.GetOptionTabLocaleID(TaxiSettings.AboutTab), "เกี่ยวกับ" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.BehaviorGroup), "ตัวเลือกแท็กซี่" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.TaxiScanGroup), "สแกนแท็กซี่" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.CityScanGroup), "ขนส่งในเมือง (ต่อเดือน)" },

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.LastUpdateGroup), "อัปเดตล่าสุด" },

#if DEBUG
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AdvancedDebugGroup), "ดีบักขั้นสูง (DEV)" },
#endif

                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.StatusActionsGroup), "คำสั่งสถานะ" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutInfoGroup), "ข้อมูล" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.AboutLinksGroup), "ลิงก์ช่วยเหลือ" },
                { m_Setting.GetOptionGroupLocaleID(TaxiSettings.DebugGroup), "ดีบัก / บันทึก" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)), "ผู้อยู่อาศัยเลี่ยงแท็กซี่" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResidentsAvoidTaxis)),
                    "<0%> = ใช้แท็กซี่ตามปกติ\n" +
                    "<25–75%> = เปอร์เซ็นต์ครัวเรือนในเมืองที่เลี่ยงแท็กซี่\n" +
                    "<100%> = ผู้อยู่อาศัยในเมืองที่เข้าเกณฑ์ทั้งหมดเลี่ยงแท็กซี่\n" +
                    "**อาจยังมีแท็กซี่อยู่บ้าง เที่ยวที่กำลังวิ่งและแท็กซี่ที่รอคิวตามปกติจะจบเอง และระบบเกมบางส่วนอาจเรียกแท็กซี่แยกต่างหาก**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockCommuters)), "ผู้เดินทางเข้าเมืองเลี่ยงแท็กซี่" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockCommuters)),
                    "**เปิด** = ผู้เดินทางเข้าเมืองเลี่ยงแท็กซี่\n" +
                    "**ปิด** = ใช้แท็กซี่ตามปกติสำหรับผู้เดินทางเข้าเมือง\n"+
                    "รอสักครู่ให้ระบบปรับตัว"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.BlockTourists)), "นักท่องเที่ยวเลี่ยงแท็กซี่" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.BlockTourists)),
                    "**เปิด** = นักท่องเที่ยวเลี่ยงแท็กซี่\n" +
                    "**ปิด** = นักท่องเที่ยวใช้แท็กซี่ตามปกติ\n" +
                    "รอสักครู่ให้ระบบปรับตัว"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ResetToGameDefaults)), "ค่าเริ่มต้นเกม" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ResetToGameDefaults)),
                    "ตั้งผู้อยู่อาศัยเลี่ยงแท็กซี่เป็น 0% และปิดการเลี่ยงสำหรับผู้เดินทางเข้าเมืองกับนักท่องเที่ยว"
                },

                // Status display
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)), "แสดงข้อมูลอัปเดตล่าสุด" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.ShowLastUpdateInfo)),
                    "แสดงการบล็อกปัจจุบัน การเปลี่ยนแปลงล่าสุด และเวลาสถานะ"
                },

                // Debug / logging
#if DEBUG
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.EnableDebugLogging)), "เปิดบันทึกแบบละเอียด" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.EnableDebugLogging)),
                    "เขียนบรรทัด TaxiSummary เป็นระยะเพื่อทดสอบ\n" +
                    "**ปิด** = ใช้สำหรับการเล่นปกติ"
                },
#endif

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)), "เขียนรายงาน" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportAbout)),
                    "สแกนวิเคราะห์แบบละเอียดและเขียนรายงานสถานะเต็มลงบันทึกของม็อด"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenLogFile)), "เปิดบันทึก" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenLogFile)),
                    "เปิดไฟล์บันทึกของม็อด หากเปิดไม่ได้จะเปิดโฟลเดอร์ Logs"
                },

                // ----- STATUS TAB -----

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)), "ยังไม่มีข้อมูลสแกนเมือง" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyCityScan)),
                    "เปิดเมือง ปล่อยให้จำลองทำงาน แล้วเปิด ตัวเลือก → สถานะ อีกครั้ง"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)), "ยังไม่มีข้อมูลสแกนแท็กซี่" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyTaxiScan)),
                    "เปิดเมือง ปล่อยให้จำลองทำงาน แล้วเปิด ตัวเลือก → สถานะ อีกครั้ง"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)), "ยังไม่มีกิจกรรมที่บันทึก" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusNotReadyLastUpdate)),
                    "รายละเอียดอัปเดตล่าสุดจะแสดงเมื่อสถานะพร้อม"
                },

                // TAXI SCAN
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusPassengers)), "ผู้โดยสารตอนนี้" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusPassengers)),
                    "ผู้โดยสารแท็กซี่ตอนนี้\n" +
                    "<ในเมือง> = ผู้โดยสารที่อาศัยในเมืองของคุณ\n" +
                    "<OC> = ผู้เดินทางเข้าเมืองและนักท่องเที่ยวจากจุดเชื่อมต่อภายนอก\n" +
                    "**ยอดรวมอาจสูงขึ้นเพราะสัตว์เลี้ยงในแท็กซี่**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiSupply)), "จำนวนแท็กซี่" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiSupply)),
                    "<จอด> = แท็กซี่ที่จอดอยู่ตอนนี้\n" +
                    "<ใช้งาน> = แท็กซี่ที่ไม่ได้จอด รวมถึงที่รอในจุดแท็กซี่\n" +
                    "<อู่ในเมือง> = อู่แท็กซี่ที่ผู้เล่นสร้าง\n" +
                    "<จุด> = พื้นที่รับ/รอแท็กซี่"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)), "แท็กซี่จากภายนอก" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusOutsideTaxis)),
                    "<จาก OC> = แท็กซี่จากจุดเชื่อมต่อภายนอก\n" +
                    "<แหล่ง OC> = จุดเชื่อมต่อภายนอกที่ส่งแท็กซี่ได้ (เหมือนอู่ที่มองไม่เห็น)\n" +
                    "**ถ้าไม่มีอู่ในเมือง เกมอาจส่งแท็กซี่ OC มารับคำขอในเมืองได้**\n" +
                    "**ในการทดสอบ เมื่อตั้งการเลี่ยงแท็กซี่สูงสุด พบแท็กซี่ OC เข้าเมืองน้อยมากหรือไม่มีเลย**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)), "จุดประสงค์แท็กซี่" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusTaxiPurpose)),
                    "จุดประสงค์ของคำขอแท็กซี่ตอนนี้\n" +
                    "<พักผ่อน> | <บ้าน> | <งาน> | <โรงเรียน> | <ซื้อของ> | <อื่น>"
                },

                // CITY TRANSIT
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)), "ประชาชน" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyPassengers1)),
                    "<แท็กซี่> | <รถบัส> | <รถราง> | <รถไฟ> | <รถไฟใต้ดิน> | <เครื่องบิน>\n" +
                    "**เที่ยวของประชาชนต่อเดือนจากมุมมองข้อมูลการขนส่งของเกม**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)), "นักท่องเที่ยว" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTourists)),
                    "<แท็กซี่> | <รถบัส> | <รถราง> | <รถไฟ> | <รถไฟใต้ดิน> | <เครื่องบิน>\n" +
                    "**เที่ยวของนักท่องเที่ยวต่อเดือนจากมุมมองข้อมูลการขนส่งของเกม**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)), "รวม" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusMonthlyTotal)),
                    "<รอ> = cim ที่กำลังรอขนส่งสาธารณะ\n" +
                    "<นักท่องเที่ยว/ด.> และ <ประชาชน/ด.> = ผู้โดยสารขนส่งสาธารณะรวมต่อเดือน"
                },

                // LAST UPDATE
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusCoverage1)), "บล็อกตอนนี้" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusCoverage1)),
                    "<ในเมือง> | <เดินทางเข้าเมือง> | <นักท่องเที่ยว>\n" +
                    "**cim ที่ใช้งานและถูก Taxi Traffic ทำเครื่องหมาย ไม่ใช่ประชากรรวมของเมือง**"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusWorkDone1)), "การเปลี่ยนแปลงล่าสุด" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusWorkDone1)),
                    "<บล็อก> = เพิ่งตั้งให้เลี่ยงแท็กซี่\n" +
                    "<ปลด> = กลับไปเลือกแท็กซี่ตามปกติ\n" +
                    "<หยุดคำขอ> = คำเรียกแท็กซี่ที่ Taxi Traffic หยุดไว้"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)), "อัปเดต" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusSnapshotMeta)),
                    "<อัปเดต> = เวลาที่ตรวจข้อมูลสถานะนี้"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)), "เขียนสถานะลงบันทึก" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.WriteStatusReportToLog)),
                    "**เขียนรายงานสถานะแบบละเอียดลงบันทึก Taxi Traffic**"
                },        


#if DEBUG
                // Advanced Debug (DEV builds only)
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)), "แฟล็กบล็อก (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugMarkedCoverage)),
                    "ตรวจสอบ DEV\n" +
                    "cim ที่ใช้งาน = ตัวแทน cim ที่อยู่ในระบบจำลองตอนนี้\n" +
                    "TT บล็อก = เครื่องหมายความเป็นเจ้าของของ Taxi Traffic\n" +
                    "IgnoreTaxi ตอนนี้ = แฟล็ก vanilla จริงในขณะนี้"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)), "แฟล็กแท็กซี่ (dev)" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.StatusDebugTaxiFlags)),
                    "ตรวจสอบ DEV\n" +
                    "ลำดับ: บัฟเฟอร์ส่งงาน | จากภายนอก | ปิดใช้งาน"
                },
#endif

                // Status row format strings
                { KeyStatusCitizensLine, "{0} แท็กซี่ | {1} บัส | {2} ราง |\n{3} รถไฟ | {4} ใต้ดิน | {5} บิน" },
                { KeyStatusTouristsLine, "{0} แท็กซี่ | {1} บัส | {2} ราง |\n{3} รถไฟ | {4} ใต้ดิน | {5} บิน" },
                { KeyStatusTotalsLine, "{0} รอ | {1} นักท่องฯ/ด. | {2} ประชาชน/ด." },
                { KeyStatusPassengersLine, "{0} รวม | {1} ในเมือง | {2} OC" },
                { KeyStatusTaxiSupplyLine, "{0} จอด, {1} ใช้งาน | {2} อู่ | {3} จุด" },
                { KeyStatusOutsideTaxisLine, "{0} จาก OC | {1} แหล่ง OC" },
                { KeyStatusTaxiPurposeLine,
                    "{0} พัก | {1} บ้าน | {2} งาน |\n" +
                    "{3} เรียน | {4} ซื้อของ | {5} อื่น"
                },
                { KeyStatusRequestsLine,
                    "{0} เมือง ({1} บล็อก) | {2} OC ({3} บล็อก) |\n" +
                    "{4} รถในเมือง | {5} รถ OC | {6} จุด"
                },
                { KeyStatusTaxiStandsLine, "{0} รอ" },
                { KeyStatusTaxiFleetLine,
                    "{0} รับผู้โดยสาร | {1} รอ | {2} กลับ |\n" +
                    "{3} ส่งงาน | {4} ระหว่างทาง | {5} จอด"
                },
                { KeyStatusCoverageLine, "{0} ในเมือง | {1} เข้าเมือง | {2} ท่องเที่ยว" },
                { KeyStatusWorkDoneLine, "{0} บล็อก | {1} ปลด | {2} หยุดคำขอ" },
                { KeyStatusSnapshotLine, "อัปเดต {0}" },

#if DEBUG
                { KeyStatusMarkedDevLine,
                    "{0} cim ใช้งาน | {1} TT บล็อก | {2} IgnoreTaxi"
                },
                { KeyStatusTaxiFlagsDevLine,
                    "{0} บัฟ.ส่ง | {1} ภายนอก | {2} ปิด"
                },
#endif

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.NameDisplay)), "ม็อด" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.NameDisplay)),
                    "ชื่อที่แสดงของม็อดนี้"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.VersionDisplay)), "เวอร์ชัน" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.VersionDisplay)),
                    "เวอร์ชันปัจจุบันของม็อด"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenParadoxMods)),
                    "เปิดหน้า Paradox Mods ของผู้สร้าง"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(TaxiSettings.OpenDiscord)), "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(TaxiSettings.OpenDiscord)),
                    "เปิดชุมชนช่วยเหลือ Discord ในเบราว์เซอร์"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
