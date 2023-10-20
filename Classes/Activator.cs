using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    public static class Activator
    {
        // Collects OS data.
        static ProductData productData = new ProductData(
            Misc.GetWindowsBuild(),
            Misc.GetWindowsArch(),
            Misc.GetProductName(),
            Misc.GetProductPfn(),
            Misc.GetEditionID());

        /// <summary>
        /// Runs the activation process.
        /// </summary>
        public static void Run()
        {
            // Checks if the OS is supported.
            if (!Misc.IsOSSupported(productData.Build))
            {
                Printer.Print("\nUnsupported OS version detected.", ConsoleColor.DarkRed, ConsoleColor.White);
                Printer.Print("Activation is supported only for Windows 10/11.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            // Checks if OS is a windows server
            if (Misc.IsWindowsServer())
            {
                Printer.Print("\nActivation is not supported for Windows Server.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            // Searches for important system apps
            Printer.Print("Searching system applications", ConsoleColor.White, ConsoleColor.Black);
            CheckOSApps();

            // Diagnoses the system before activate attempt
            Printer.Print("\nDiagnostic Tests", ConsoleColor.White, ConsoleColor.Black);
            Diagnose();

            // Tries to activate the system
            Printer.Print("\nActivating", ConsoleColor.White, ConsoleColor.Black);
            Activate();

            // Checks if the system has been activated
            Printer.Print("\nVerifying", ConsoleColor.White, ConsoleColor.Black);
            CheckActivation();

            // Ends
            IsDone();
        }

        /// <summary>
        /// Attemps to activate Windows.
        /// </summary>
        private static void Activate()
        {
            string output;
            string command;

            // Tries to get the generic product key from the provided Edition ID
            if (!GenericWindowsKeys.TryGetProductKey(productData.EditionID, out string productKey))
            {
                Printer.Print("No product key Found!", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            // Tries to install the generic product key into the system
            command = $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /ipk {productKey}";
            output = CommandHandler.RunCommand(command);
            if (!output.Contains(productKey))
            {
                Printer.Print("Generic product key couldn't be installed. Wrong product key?", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Printer.Print("Generic Product Key\t\t\t[ Installed ]");

            // Try activation via KMS
            KMS.Initialize();
            bool providerFound = false;
            while (!providerFound && KMS.urlsCount > 0)
            {
                if (KMS.GetKMSProvider(out string kmsUrl))
                {
                    // Set KMS
                    Printer.Print($"Trying KMS provider \t\t\t[ {kmsUrl} ] ", true);
                    command = $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /skms {kmsUrl}";
                    CommandHandler.RunCommand(command);

                    // Try activating using KMS
                    output = CommandHandler.RunCommand($"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /ato");

                    // Invalid product key
                    if (output.Contains("0x803F7001"))
                    {
                        Printer.Print("[ Failed ]");
                        Printer.Print("Product Key is not valid. OS not compatible. If you think this OS is compatible, please contact the apps creator on GitHub.", ConsoleColor.DarkRed, ConsoleColor.White);
                        break;
                    }

                    // no connection to KMS provider
                    if (output.Contains("0xC004F074"))
                    {
                        Printer.Print("[ Failed ]");
                        KMS.RemoveKMSUrl(kmsUrl);
                        continue;
                    }

                    // Success
                    Printer.Print("[ Success ]");
                    providerFound = true;
                }
            }
            if(!providerFound)
            {
                Printer.Print($"Found no valid KMS provider.", ConsoleColor.DarkRed, ConsoleColor.White);
            }
        }

        /// <summary>
        /// Verifies the activation.
        /// </summary>
        private static void CheckActivation()
        {
            if (Misc.IsSystemActivated())
            {
                Printer.Print($"{productData.Name} has been activated.", ConsoleColor.DarkGreen, ConsoleColor.White);
            }
            else
            {
                Printer.Print($"Activation Failed.", ConsoleColor.DarkRed, ConsoleColor.White);
            }
        }

        /// <summary>
        /// Diagnoses the system 
        /// </summary>
        private static void Diagnose()
        {
            if (!Misc.HasAdminPriveleges())
            {
                Printer.Print("\nThis application require administrator privileges.", ConsoleColor.DarkRed, ConsoleColor.White);
                Printer.Print("To do so, right click on this application and select 'Run as administrator'.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            Printer.Print($"Admin Priveleges\t\t\t[ Yes ]");

            if (!Misc.IsInternetConnected())
            {
                Printer.Print("\nNo Internet Connection. Please ensure a stable internet connection to activate windows.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            Printer.Print("Internet Connection\t\t\t[ Connected ]");
            Printer.Print($"OS Info\t\t\t\t\t[ {productData.Name} | {productData.Build} | {productData.Architecture} ]");

            if (!Program.ignoreExistingActivation && Misc.IsSystemActivated())
            {
                Printer.Print($"\n{productData.Name} is currently Activated", ConsoleColor.White, ConsoleColor.Black);
                Printer.Print($"Do you still want to proceed with the activation?", ConsoleColor.Black, ConsoleColor.DarkYellow);
                Printer.Print($"Press the 'Enter' button to continue.", ConsoleColor.Black, ConsoleColor.DarkYellow);
                if(Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    IsDone();
                }
            }
        }


        /// <summary>
        /// Checks for important System applications.
        /// </summary>
        private static void CheckOSApps()
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
        private static void GetSystemApplication(string appName, Paths.Path path)
        {
            if (!Misc.GetSystemApplication(appName, out string pathToApp))
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
        private static void IsDone()
        {
            if (!Program.autoCloseOnIsDone)
            {
                Printer.Print("\nPress any key to Exit", new ConsoleColor(), ConsoleColor.DarkYellow);
                Console.ReadKey();
            }
            Process.GetCurrentProcess().Kill();
        }
    }
}
