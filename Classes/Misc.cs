using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Security.Principal;
using System.Threading;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    static class Misc
    {
        /// <summary>
        /// Checks if an URL is active by pinging.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public bool IsURLActive(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 15000;
            request.Method = "HEAD";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if there's an internet connection pinging google.com
        /// </summary>
        /// <returns></returns>
        static public bool IsInternetConnected()
        {
            return IsURLActive("http://www.google.com");
        }

        /// <summary>
        /// Checks if the application has Administration priveleges
        /// </summary>
        /// <returns></returns>
        static public bool HasAdminPriveleges()
        {
            bool hasAdmin = false;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                hasAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return hasAdmin;
        }

        /// <summary>
        /// Tries to get a system application
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="pathToApp"></param>
        /// <returns></returns>
        static public bool GetSystemApplication(string appName, out string pathToApp)
        {
            pathToApp = null;
            string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
            bool found = false;
            foreach (string path in paths)
            {
                string fullPath = Path.Combine(path, appName);
                if (File.Exists(fullPath))
                {
                    pathToApp = fullPath;
                    found = true;
                    break;
                }
            }
            return found;
        }

        static public bool ReadFileFromURL(string url, out string content)
        {
            content = String.Empty;
            if(IsURLActive(url))
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(url);
                StreamReader reader = new StreamReader(stream);
                content = reader.ReadToEnd();
                return true;
            }
            return false;
        }

        static public int GetWindowsBuild()
        {
            return Environment.OSVersion.Version.Build;
        }

        static public bool IsOSSupported(int buildNumber)
        {
            return buildNumber > 10240;
        }

        static public bool IsWindowsServer()
        {
            string serverEditionFilePath = $@"{GetSystemRootDirectory()}\Servicing\Packages\Microsoft-Windows-Server*Edition~*.mum";
            return File.Exists(serverEditionFilePath);
        }

        static public string GetSystemRootDirectory()
        {
            return Environment.GetEnvironmentVariable("SystemRoot");
        }

        static public string GetWindowsArch()
        {
            return RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "PROCESSOR_ARCHITECTURE");
        }

        static public bool IsSystemActivated()
        {
            string output = CommandHandler.RunCommand($"{Paths.GetPath(Paths.Path.WMIC)} path SoftwareLicensingProduct where(LicenseStatus= '1' and PartialProductKey is not NULL) get Name /value 2> nul");
            return output.Contains("Windows");
        }

        static public string GetProductName()
        {
            return RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
        }

        static public string GetEditionID()
        {
            return RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID");
        }

        static public string GetProductPfn()
        {
            return RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\ProductOptions", "OSProductPfn");
        }
    }
}
