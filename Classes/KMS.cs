using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WindowsActivator.Classes
{
    public static class KMS
    {
        private static readonly string kmsProviderslistUrl = @"http://raw.githubusercontent.com/Nickk888SAMP/WindowsActivator/master/KMSProviders";
        private static List<string> kmsUrls = new List<string>();

        public static void Initialize()
        {
            kmsUrls.Clear();
            if (Misc.ReadFileFromURL(kmsProviderslistUrl, out string content))
            {
                StringReader reader = new StringReader(content);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    AddKMSUrl(line);
                }
            }
        }

        public static int KMSUrlCount() => kmsUrls.Count;

        public static bool GetKMSProvider(out string kmsUrl)
        {
            kmsUrl = kmsUrls.FirstOrDefault();
            if(kmsUrl == null)
            {
                return false;
            }
            return true;
        }

        public static void AddKMSUrl(string url)
        {
            kmsUrls.Add(url);
        }

        public static void RemoveKMSUrl(string url)
        {
            kmsUrls.Remove(url);
        }
    }
}
