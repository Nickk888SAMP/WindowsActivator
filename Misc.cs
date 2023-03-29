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
        public enum RegistryRootKey
        {
            CLASSES_ROOT,
            CURRENT_USER,
            LOCAL_MACHINE,
            USERS,
            CURRENT_CONFIG
        }

        public enum CommandHandler
        {
            CMD,
            PowerShell
        }

        public static string RunCommand(CommandHandler commandHandler, string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = commandHandler == CommandHandler.CMD ? Paths.cmdPath : Paths.powerShellPath,
                Arguments = commandHandler == CommandHandler.CMD ? ("/c " + command) : command,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process ps = new Process();
            ps.StartInfo = psi;
            ps.Start();

            string output = ps.StandardOutput.ReadToEnd();
            ps.WaitForExit();

            return output;
        }

        static public void CreateRegistrySubKeyEntry(Misc.RegistryRootKey rootKey, string subKey, string name, object value, RegistryValueKind valueKind)
        {
            RegistryKey key;

            switch (rootKey)
            {
                default:
                case Misc.RegistryRootKey.CLASSES_ROOT:
                    key = Registry.ClassesRoot.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    break;
                case Misc.RegistryRootKey.CURRENT_USER:
                    key = Registry.CurrentUser.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    break;
                case Misc.RegistryRootKey.LOCAL_MACHINE:
                    key = Registry.LocalMachine.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    break;
                case Misc.RegistryRootKey.USERS:
                    key = Registry.Users.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    break;
                case Misc.RegistryRootKey.CURRENT_CONFIG:
                    key = Registry.CurrentConfig.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    break;
            }

            key.SetValue(name, value, valueKind);
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

        static public string GetWindowsArch()
        {
            return Misc.HKLM_GetString(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "PROCESSOR_ARCHITECTURE");
        }


        static public bool IsSystemPermanentActivated()
        {
            string output = Misc.RunCommand(Misc.CommandHandler.CMD, $"{Paths.wmicPath} path SoftwareLicensingProduct where(LicenseStatus= '1' and GracePeriodRemaining = '0' and PartialProductKey is not NULL) get Name /value 2> nul");
            return output.Contains("Windows");
        }


        static public string GetProductName()
        {
            return HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
        }

        static public string GetEditionID()
        {
            return HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID");
        }

        static public string GetProductPfn()
        {
            return HKLM_GetString(@"SYSTEM\CurrentControlSet\Control\ProductOptions", "OSProductPfn");
        }

        static public string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
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
    }
}
