using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Principal;

namespace WindowsActivator
{
    static class Misc
    {

        public static string RunCommand(Enums.CommandHandler commandHandler, string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = commandHandler == Enums.CommandHandler.CMD ? Paths.cmdPath : Paths.powerShellPath,
                Arguments = commandHandler == Enums.CommandHandler.CMD ? ("/c " + command) : command,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process ps = new Process
            {
                StartInfo = psi,
            };

            ps.Start();

            string output = ps.StandardOutput.ReadToEnd();
            ps.WaitForExit();

            return output;
        }

        static public Stream CreateFile(string fileName, out string filePath)
        {
            filePath = $@"{Paths.applicationWorkDirectory}\{fileName}";
            using (Stream s = File.Create(filePath))
            {
                return s;
            }
        }

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


        static public bool IsElevated()
        {
            bool isElevated = false;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return isElevated;
        }

        static public bool IsOSAppAvailable(string appName, out string pathToApp)
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
            Directory.Delete(Paths.applicationWorkDirectory, true);
        }

        static public string GetWindowsArch()
        {
            return RegistryHandler.GetRegistryStringValue(Enums.RegistryRootKey.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "PROCESSOR_ARCHITECTURE");
        }

        static public bool IsSystemPermanentActivated()
        {
            string output = RunCommand(Enums.CommandHandler.CMD, $"{Paths.wmicPath} path SoftwareLicensingProduct where(LicenseStatus= '1' and GracePeriodRemaining = '0' and PartialProductKey is not NULL) get Name /value 2> nul");
            return output.Contains("Windows");
        }

        static public string GetProductName()
        {
            return RegistryHandler.GetRegistryStringValue(Enums.RegistryRootKey.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
        }

        static public string GetEditionID()
        {
            return RegistryHandler.GetRegistryStringValue(Enums.RegistryRootKey.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID");
        }

        static public string GetProductPfn()
        {
            return RegistryHandler.GetRegistryStringValue(Enums.RegistryRootKey.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\ProductOptions", "OSProductPfn");
        }
    }
}
