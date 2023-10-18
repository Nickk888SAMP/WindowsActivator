using System;
using System.Collections.Generic;

namespace WindowsActivator
{
    static class GenericWindowsKeys
    {
        static private readonly Dictionary<string, string> editionIdToKey = new Dictionary<string, string>
        {
            // Home
            { "Core", "TX9XD-98N7V-6WMQ6-BX7FG-H8Q99" },
            { "CoreN", "3KHY7-WNT83-DGQKR-F7HPR-844BM" },
            { "CoreSingleLanguage", "7HNRX-D7KGG-3K4RQ-4WPJ4-YTDFH" },
            { "CoreCountrySpecific", "PVMJN-6DFY6-9CCP6-7BKTT-D3WVR" },

            // Professional
            { "Professional", "W269N-WFGWX-YVC9B-4J6C9-T83GX" },
            { "ProfessionalN", "MH37W-N47XK-V7XM9-C7227-GCQG9" },

            // Enterprise
            { "Enterprise", "NPPR9-FWDCX-D2C8J-H872K-2YT43" },
            { "EnterpriseN", "DPH2V-TTNVB-4X9Q3-TJR4H-KHJW4" }, 

            // Education
            { "Education", "NW6C2-QMPVW-D7KKK-3GKT6-VCFB2" },
            { "EducationN", "2WH4N-8QGBV-H22JP-CT43Q-MDWWJ" }
        };
        
        /// <summary>
        /// Tries to get the product key of the operating systems edition ID.
        /// </summary>
        /// <param name="editionId"></param>
        /// <param name="productKey"></param>
        /// <returns></returns>
        static public bool TryGetProductKey(string editionId, out string productKey)
        {
            productKey = string.Empty;
            if (editionIdToKey.TryGetValue(editionId, out productKey))
            {
                return true;
            }
            return false;
        }

    }
}
