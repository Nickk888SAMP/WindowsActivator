using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WindowsActivator.Classes
{
    public static class KMS
    {
        public static int UrlsCount { get { return kmsUrls.Count; } }

        private static readonly string kmsProviderslistUrl = @"http://raw.githubusercontent.com/Nickk888SAMP/WindowsActivator/master/KMSProviders";
        private static List<string> kmsUrls = new List<string>();

        /// <summary>
        /// Initializes the KMS URL List.
        /// </summary>
        public static void Initialize()
        {
            kmsUrls.Clear();
            if (Misc.ReadFileFromURL(kmsProviderslistUrl, out string content))
            {
                StringReader reader = new StringReader(content);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    AddUrl(line);
                }
            }
        }

        /// <summary>
        /// Gets the first possible KMS provider.
        /// </summary>
        /// <param name="kmsUrl"></param>
        /// <returns></returns>
        public static bool GetKMSProvider(out string kmsUrl) => (kmsUrl = kmsUrls.FirstOrDefault()) != null;

        /// <summary>
        /// Adds a URL to the KMS list.
        /// </summary>
        /// <param name="url"></param>
        public static void AddUrl(string url) => kmsUrls.Add(url);

        /// <summary>
        /// Removes a URL from the KMS list.
        /// </summary>
        /// <param name="url"></param>
        public static void RemoveUrl(string url) => kmsUrls.Remove(url);
    }
}
