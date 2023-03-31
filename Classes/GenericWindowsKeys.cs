using System.Collections.Generic;

namespace WindowsActivator
{
    static class GenericWindowsKeys
    {
        static private readonly Dictionary<string, string> editionIdToKey = new Dictionary<string, string>
        {
            // Home
            { "Core", "YTMG3-N6DKC-DKB77-7M9GH-8HVX7" }, // Check
            { "CoreN", "4CPRK-NM3K3-X6XXQ-RXX86-WXCHW" },
            { "CoreSingleLanguage", "BT79Q-G7N6G-PGBYW-4YWX6-6F4BT" }, // Check
            { "CoreCountrySpecific", "N2434-X9D7W-8PF6X-8DV9T-8TYMD" },

            // Professional
            { "Professional", "VK7JG-NPHTM-C97JM-9MPGT-3V66T" }, //Check
            { "ProfessionalN", "2B87N-8KFHP-DKV6R-Y2C8J-PKCKT" },

            // Professional Education
            { "ProfessionalEducation", "8PTT6-RNW4C-6V7J2-C2D3X-MHBPB" },
            { "ProfessionalEducationN", "GJTYN-HDMQY-FRR76-HVGC7-QPF8P" },

            // Enterprise
            { "Enterprise", "XGVPP-NMH47-7TTHJ-W3FW7-8HV2C" },
            { "EnterpriseN", "3V6Q6-NQXCX-V8YXR-9QCYV-QPFCT" },

            // Education
            { "Education", "YNMGQ-8RYV3-4PGQ3-C8XTP-7CFBY" }, // Check
            { "EducationN", "84NGF-MHBT6-FXBX8-QWJK7-DRR8H" },

            // IoT Enterprise
            { "IoTEnterprise", "XQQYW-NFFMW-XJPBH-K8732-CKFFD" },
            { "IoTEnterpriseS", "QPM6N-7J2WJ-P88HH-P3YRH-YY74H" },

            // Professional Workstation
            { "ProfessionalWorkstation", "DXG7C-N36C4-C4HTG-X4T3X-2YV77" },
            { "ProfessionalWorkstationN", "WYPNQ-8C467-V2W6J-TX4WX-WT2RQ" },

            // Cloud
            { "Cloud", "V3WVW-N2PV2-CGWC3-34QGF-VMJ2C" },
            { "CloudN", "NH9J3-68WK7-6FB93-4K3DF-DJ4F6" },

            // SE
            { "SE", "K9VKN-3BGWV-Y624W-MCRMQ-BHDCD" },
            { "SEN", "KY7PN-VR6RX-83W6Y-6DDYQ-T6R4W" },

            // Team
            { "Team", "XKCNC-J26Q9-KFHD2-FKTHY-KD72Y" }
        };

        static public bool TryGetProductKey(string editionId, out string productKey)
        {
            if(editionIdToKey.TryGetValue(editionId, out productKey))
            {
                return true;
            }
            return false;
        }

    }
}
