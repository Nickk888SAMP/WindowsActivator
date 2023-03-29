using System.Collections.Generic;

namespace WindowsActivator
{
    static class GenericWindowsKeys
    {
        static private readonly Dictionary<string, string> editionIdToKey = new Dictionary<string, string>
        {
            { "Education", "YNMGQ-8RYV3-4PGQ3-C8XTP-7CFBY" },
            { "Education N", "84NGF-MHBT6-FXBX8-QWJK7-DRR8H" },
            { "Enterprise", "XGVPP-NMH47-7TTHJ-W3FW7-8HV2C" },
            { "Enterprise N", "3V6Q6-NQXCX-V8YXR-9QCYV-QPFCT" },
            { "Enterprise LTSB 2015", "FWN7H-PF93Q-4GGP8-M8RF3-MDWWW" },
            { "Enterprise LTSB 2016", "NK96Y-D9CD8-W44CQ-R8YTK-DYJWX" },
            { "Enterprise LTSC 2019", "43TBQ-NH92J-XKTM7-KT3KK-P39PB" },
            { "Enterprise N LTSB 2015", "NTX6B-BRYC2-K6786-F6MVQ-M7V2X" },
            { "Enterprise N LTSB 2016", "2DBW3-N2PJG-MVHW3-G7TDK-9HKR4" },
            { "Core", "YTMG3-N6DKC-DKB77-7M9GH-8HVX7" }, // Home
            { "Core N", "4CPRK-NM3K3-X6XXQ-RXX86-WXCHW" }, // Home N
            { "Core China", "N2434-X9D7W-8PF6X-8DV9T-8TYMD" }, // Home China
            { "Core Single Language", "BT79Q-G7N6G-PGBYW-4YWX6-6F4BT" }, // Home Single Language
            { "IoT Enterprise", "XQQYW-NFFMW-XJPBH-K8732-CKFFD" },
            { "IoT Enterprise LTSC 2021", "QPM6N-7J2WJ-P88HH-P3YRH-YY74H" },
            { "IoT Enterprise LTSC Subscription", "J7NJW-V6KBM-CC8RW-Y29Y4-HQ2MJ" },
            { "Professional", "VK7JG-NPHTM-C97JM-9MPGT-3V66T" },
            { "Professional N", "2B87N-8KFHP-DKV6R-Y2C8J-PKCKT" },
            { "Professional Education", "8PTT6-RNW4C-6V7J2-C2D3X-MHBPB" },
            { "Professional Education N", "GJTYN-HDMQY-FRR76-HVGC7-QPF8P" },
            { "Professional for Workstations", "DXG7C-N36C4-C4HTG-X4T3X-2YV77" },
            { "Professional N for Workstations", "WYPNQ-8C467-V2W6J-TX4WX-WT2RQ" },
            { "S", "V3WVW-N2PV2-CGWC3-34QGF-VMJ2C" },
            { "S N", "NH9J3-68WK7-6FB93-4K3DF-DJ4F6" },
            { "SE", "K9VKN-3BGWV-Y624W-MCRMQ-BHDCD" },
            { "SE N", "KY7PN-VR6RX-83W6Y-6DDYQ-T6R4W" },
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
