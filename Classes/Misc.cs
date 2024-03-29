﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Windows.Forms;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    public static class Misc
    {
        /// <summary>
        /// Checks if an URL is active by pinging.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsURLActive(string url)
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
        public static bool IsInternetConnected() => IsURLActive("http://www.google.com");

        /// <summary>
        /// Checks if the application has Administration priveleges
        /// </summary>
        /// <returns></returns>
        public static bool HasAdminPriveleges()
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
        public static bool GetSystemApplication(string appName, out string pathToApp)
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

        /// <summary>
        /// Reads a text file from a URL and outputs a string.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool ReadFileFromURL(string url, out string content)
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
        /// <summary>
        /// Checks if the system is activated.
        /// </summary>
        /// <returns></returns>
        public static bool IsSystemActivated()
        {
            string output = CommandHandler.RunCommand($"{Paths.GetPath(Paths.Path.WMIC)} path SoftwareLicensingProduct where(LicenseStatus= '1' and PartialProductKey is not NULL) get Name /value 2> nul");
            return output.Contains("Windows");
        }

        /// <summary>
        /// Checks for important System applications.
        /// </summary>
        public static void CheckOSApps()
        {
            GetSystemApplication("cmd.exe", Paths.Path.CMD);
            GetSystemApplication("slmgr.vbs", Paths.Path.SLMGR);
            GetSystemApplication("cscript.exe", Paths.Path.CScript);
            GetSystemApplication("wmic.exe", Paths.Path.WMIC);
        }

        /// <summary>
        /// Tries to get a system application and does all the printing, canceling or continuing.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="path"></param>
        public static void GetSystemApplication(string appName, Paths.Path path)
        {
            if (!GetSystemApplication(appName, out string pathToApp))
            {
                Printer.Print();
                Printer.Print($"Unable to find {appName} in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(path, pathToApp);
            Printer.Print($"{appName}".PadRight(40) + "[ Found ]");
        }

        /// <summary>
        /// Cleans up work space and waits for a key press to exit.
        /// </summary>
        public static void IsDone()
        {
            if (!Program.autoCloseOnIsDone)
            {
                Printer.Print("\nPress any key to continue.", new ConsoleColor(), ConsoleColor.DarkYellow);
                Console.ReadKey();
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// Gets the current windows Build.
        /// </summary>
        /// <returns></returns>
        public static int GetWindowsBuild() => Environment.OSVersion.Version.Build;

        /// <summary>
        /// Checks if the OS is supported.
        /// </summary>
        /// <param name="buildNumber"></param>
        /// <returns></returns>
        public static bool IsOSSupported(int buildNumber) => buildNumber > 10240;

        /// <summary>
        /// Gets the systems root directory. 
        /// </summary>
        /// <returns></returns>
        public static string GetSystemRootDirectory() => Environment.GetEnvironmentVariable("SystemRoot");

        /// <summary>
        /// Gets the architecture of the operating system.
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsArch() => RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "PROCESSOR_ARCHITECTURE");

        /// <summary>
        /// Gets the OS product name.
        /// </summary>
        /// <returns></returns>
        public static string GetProductName() => RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");

        /// <summary>
        /// Gets the OS edition id.
        /// </summary>
        /// <returns></returns>
        public static string GetEditionID() => RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "EditionID");

        /// <summary>
        /// Gets the OS product Pfn
        /// </summary>
        /// <returns></returns>
        public static string GetProductPfn() => RegistryHandler.GetRegistryStringValue(RegistryHandler.RegistryRootKey.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\ProductOptions", "OSProductPfn");


    }
}
