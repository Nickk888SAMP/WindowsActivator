using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsActivator
{
    static class GenericWindowsKeys
    {
        static private readonly Dictionary<string, string> editionIdToKey_encoded = new Dictionary<string, string>
        {
            // Home
            { "Core", "WVRNRzMtTjZES0MtREtCNzctN005R0gtOEhWWDc=" }, // Check
            { "CoreN", "NENQUkstTk0zSzMtWDZYWFEtUlhYODYtV1hDSFc=" }, // Check
            { "CoreSingleLanguage", "QlQ3OVEtRzdONkctUEdCWVctNFlXWDYtNkY0QlQ=" }, // Check
            { "CoreCountrySpecific", "TjI0MzQtWDlEN1ctOFBGNlgtOERWOVQtOFRZTUQ=" },

            // Professional
            { "Professional", "Vks3SkctTlBIVE0tQzk3Sk0tOU1QR1QtM1Y2NlQ=" }, //Check
            { "ProfessionalN", "MkI4N04tOEtGSFAtREtWNlItWTJDOEotUEtDS1Q=" },

            // Professional Education
            { "ProfessionalEducation", "OFBUVDYtUk5XNEMtNlY3SjItQzJEM1gtTUhCUEI=" },
            { "ProfessionalEducationN", "R0pUWU4tSERNUVktRlJSNzYtSFZHQzctUVBGOFA=" },

            // Enterprise
            { "Enterprise", "WEdWUFAtTk1INDctN1RUSEotVzNGVzctOEhWMkM=" }, // Check
            { "EnterpriseN", "M1Y2UTYtTlFYQ1gtVjhZWFItOVFDWVYtUVBGQ1Q=" },

            // Education
            { "Education", "WU5NR1EtOFJZVjMtNFBHUTMtQzhYVFAtN0NGQlk=" }, // Check
            { "EducationN", "ODROR0YtTUhCVDYtRlhCWDgtUVdKSzctRFJSOEg=" },

            // IoT Enterprise
            { "IoTEnterprise", "WFFRWVctTkZGTVctWEpQQkgtSzg3MzItQ0tGRkQ=" },
            { "IoTEnterpriseS", "UVBNNk4tN0oyV0otUDg4SEgtUDNZUkgtWVk3NEg=" },

            // Professional Workstation
            { "ProfessionalWorkstation", "RFhHN0MtTjM2QzQtQzRIVEctWDRUM1gtMllWNzc=" },
            { "ProfessionalWorkstationN", "V1lQTlEtOEM0NjctVjJXNkotVFg0V1gtV1QyUlE=" },

            // Cloud
            { "Cloud", "VjNXVlctTjJQVjItQ0dXQzMtMzRRR0YtVk1KMkM=" },
            { "CloudN", "Tkg5SjMtNjhXSzctNkZCOTMtNEszREYtREo0RjY=" },

            // SE
            { "SE", "SzlWS04tM0JHV1YtWTYyNFctTUNSTVEtQkhEQ0Q=" },
            { "SEN", "S1k3UE4tVlI2UlgtODNXNlktNkREWVEtVDZSNFc=" },

            // Team
            { "Team", "WEtDTkMtSjI2UTktS0ZIRDItRktUSFktS0Q3Mlk=" }
        };
        
        static public bool TryGetProductKey(string editionId, out string decodedProductKey)
        {
            decodedProductKey = string.Empty;
            if (editionIdToKey_encoded.TryGetValue(editionId, out string encodedProductKey))
            {
                decodedProductKey = DecodeKey(encodedProductKey);
                return true;
            }
            return false;
        }

        static private string DecodeKey(string encodedKey)
        {
            byte[] utf8Bytes = Convert.FromBase64String(encodedKey);
            string decodedKey = Encoding.UTF8.GetString(utf8Bytes);
            return decodedKey;
        }

    }
}
