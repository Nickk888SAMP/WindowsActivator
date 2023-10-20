using System;
using System.Threading;
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

        static private string output;
        static private string command;

        /// <summary>
        /// Runs the activation process.
        /// </summary>
        public static bool Install()
        {
            Console.Clear();

            // Checks if the OS is supported.
            if (!Misc.IsOSSupported(productData.Build))
            {
                Printer.Print("\nUnsupported OS version detected.", ConsoleColor.DarkRed, ConsoleColor.White);
                Printer.Print("Activation is supported only for Windows 10/11.", ConsoleColor.DarkRed, ConsoleColor.White);
                Misc.IsDone();
                return false;
            }

            // Diagnoses the system before activate attempt
            Printer.Print("Diagnostic Tests", ConsoleColor.White, ConsoleColor.Black);
            if(!Diagnose())
            {
                return false;
            }

            // Tries to activate the system
            Printer.Print("\nActivating", ConsoleColor.White, ConsoleColor.Black);
            if(!Activate())
            {
                return false;
            }

            // Checks if the system has been activated
            Printer.Print("\nVerifying", ConsoleColor.White, ConsoleColor.Black);
            if(!CheckActivation())
            {
                return false;
            }

            // Ends
            Misc.IsDone();
            return true;
        }
        public static bool Uninstall()
        {
            Console.Clear();

            // Activation
            command = $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /upk";
            CommandHandler.RunCommand(command);
            Printer.Print("Generic Product Key\t\t\t[ Removed ]");

            command = $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /cpky";
            CommandHandler.RunCommand(command);
            Printer.Print("Product key in registry\t\t\t[ Removed ]");

            command = $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /rearm";
            CommandHandler.RunCommand(command);
            Printer.Print("Licensing status\t\t\t[ Reset ]");

            // Task scheduler
            ScheduledReactivator.Uninstall();
            Printer.Print("Auto-Renewal\t\t\t\t[ Removed ]");

            Printer.Print($"\nActivation uninstalled! Please restart the system for the changes to take effect.", ConsoleColor.DarkGreen, ConsoleColor.White);
            Printer.Print($"\nNote: Can't be re-activated until the system has been restarted!", ConsoleColor.DarkYellow, ConsoleColor.White);

            // Ends
            Misc.IsDone();
            return true;
        }

        /// <summary>
        /// Attemps to activate Windows.
        /// </summary>
        private static bool Activate()
        {
            // Tries to get the generic product key from the provided Edition ID
            if (!GenericWindowsKeys.TryGetProductKey(productData.EditionID, out string productKey))
            {
                Printer.Print("No product key Found!", ConsoleColor.DarkRed, ConsoleColor.White);
                Misc.IsDone();
                return false;
            }

            // Tries to install the generic product key into the system
            command = $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /ipk {productKey}";
            output = CommandHandler.RunCommand(command);
            if (!output.Contains(productKey))
            {
                Printer.Print("Generic product key couldn't be installed. Wrong product key?", ConsoleColor.DarkRed, ConsoleColor.White);
                Misc.IsDone();
                return false;
            }
            Printer.Print("Generic Product Key\t\t\t[ Installed ]");

            // Try activation via KMS
            KMS.Initialize();
            bool providerFound = false;
            while (!providerFound && KMS.UrlsCount > 0)
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
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verifies the activation.
        /// </summary>
        private static bool CheckActivation()
        {
            if (!Misc.IsSystemActivated())
            {
                Printer.Print($"Activation Failed.", ConsoleColor.DarkRed, ConsoleColor.White);
                return false;
            }
            Printer.Print($"{productData.Name} has been activated.", ConsoleColor.DarkGreen, ConsoleColor.White);
            return true;
        }

        /// <summary>
        /// Diagnoses the system 
        /// </summary>
        private static bool Diagnose()
        {
            if (!Misc.HasAdminPriveleges())
            {
                Printer.Print("\nThis application require administrator privileges.", ConsoleColor.DarkRed, ConsoleColor.White);
                Printer.Print("To do so, right click on this application and select 'Run as administrator'.", ConsoleColor.DarkRed, ConsoleColor.White);
                Misc.IsDone();
                return false;
            }

            Printer.Print($"Admin Priveleges\t\t\t[ Yes ]");

            // Try internet connection
            int internetReconnectAttempt = 10;
            while (internetReconnectAttempt > 0)
            {
                if (!Misc.IsInternetConnected())
                {
                    Thread.Sleep(3000);
                    internetReconnectAttempt--;
                    Printer.Print("No Internet Connection. Retrying...");
                }
                else
                {
                    internetReconnectAttempt = -1;
                }
            }
            if(internetReconnectAttempt == 0)
            {
                Printer.Print("\nNo Internet Connection. Please ensure a stable internet connection to activate windows.", ConsoleColor.DarkRed, ConsoleColor.White);
                Misc.IsDone();
                return false;
            }
            Printer.Print("Internet Connection\t\t\t[ Connected ]");

            // OS Info
            Printer.Print($"OS Info\t\t\t\t\t[ {productData.Name} | {productData.Build} | {productData.Architecture} ]");
            if (!Program.ignoreExistingActivation && Misc.IsSystemActivated())
            {
                Printer.Print($"\n{productData.Name} is currently Activated", ConsoleColor.White, ConsoleColor.Black);
                Printer.Print($"Do you still want to proceed with the activation?", ConsoleColor.Black, ConsoleColor.DarkYellow);
                Printer.Print($"Press the 'Enter' button to continue.", ConsoleColor.Black, ConsoleColor.DarkYellow);
                if(Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    Misc.IsDone();
                    return false;
                }
            }
            return true;
        }


    }
}
