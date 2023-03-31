using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Principal;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    static class Misc
    {
        static public bool IsInternetConnected()
        {
            try
            {
                Ping myPing = new Ping();
                string host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }


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

        static public void CleanUpWorkSpace()
        {
            Directory.Delete(Paths.GetPath(Paths.Path.WorkDirectory), true);
        }

        static public string GetWindowsArch()
        {
            return RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "PROCESSOR_ARCHITECTURE");
        }

        static public bool IsSystemPermanentActivated()
        {
            string output = CommandHandler.RunCommand(CommandHandler.CommandType.CMD, $"{Paths.GetPath(Paths.Path.WMIC)} path SoftwareLicensingProduct where(LicenseStatus= '1' and GracePeriodRemaining = '0' and PartialProductKey is not NULL) get Name /value 2> nul");
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
