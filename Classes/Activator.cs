using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    internal class Activator
    {
        // Collects OS data.
        ProductData productData = new ProductData(
            Misc.GetWindowsBuild(),
            Misc.GetWindowsArch(),
            Misc.GetProductName(),
            Misc.GetProductPfn(),
            Misc.GetEditionID());

        /// <summary>
        /// Runs the activation process.
        /// </summary>
        public void Run()
        {
            // Set working directory path
            Paths.SetPath(Paths.Path.WorkDirectory, Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Windows Activator");

            // Checks if the OS is supported.
            if (!Misc.IsOSSupported(productData.Build))
            {
                Printer.Print("\nUnsupported OS version detected.", ConsoleColor.DarkRed, ConsoleColor.White);
                Printer.Print("HWID Activation is supported only for Windows 10/11.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            // Checks if OS is a windows server
            if (Misc.IsWindowsServer())
            {
                Printer.Print("\nHWID Activation is not supported for Windows Server.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            // Searches for important system apps
            Printer.Print("Searching important system applications", ConsoleColor.White, ConsoleColor.Black);
            CheckOSApps();

            // Diagnoses the system before activate attempt
            Printer.Print("\nDiagnostic Tests", ConsoleColor.White, ConsoleColor.Black);
            Diagnose();

            // Tries to activate the system
            Printer.Print($"\nProceeding with activation...\n");
            Activate();

            // Ends
            IsDone();
        }

        /// <summary>
        /// Attemps to activate Windows
        /// </summary>
        private void Activate()
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
            while (!providerFound && KMS.KMSUrlCount() > 0)
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
                        Printer.Print("Product Key is not valid. Please contact the apps creator on GitHub.", ConsoleColor.DarkRed, ConsoleColor.White);
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

            // Checks if the system has been activated
            if (Misc.IsSystemActivated())
            {
                Printer.Print($"\n{productData.Name} is activated with a digital license.", ConsoleColor.DarkGreen, ConsoleColor.White);
            }
            else
            {
                Printer.Print($"\nActivation Failed.", ConsoleColor.DarkRed, ConsoleColor.White);
            }

            IsDone();
        }

        /// <summary>
        /// Diagnoses the system 
        /// </summary>
        private void Diagnose()
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
        /// Checks for important System applications
        /// </summary>
        private void CheckOSApps()
        {
            string path;
            if (!Misc.GetSystemApplication("cmd.exe", out path))
            {
                Printer.Print();
                Printer.Print("Unable to find cmd.exe in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.CMD, path);
            Printer.Print($"CMD\t\t\t\t\t[ Found ]");

            if (!Misc.GetSystemApplication("slmgr.vbs", out path))
            {
                Printer.Print();
                Printer.Print("Unable to find slmgr.vbs in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.SLMGR, path);
            Printer.Print($"SLMGR\t\t\t\t\t[ Found ]");

            if (!Misc.GetSystemApplication("cscript.exe", out path))
            {
                Printer.Print();
                Printer.Print("Unable to find cscript.exe in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.CScript, path);
            Printer.Print($"CScript\t\t\t\t\t[ Found ]");

            if (!Misc.GetSystemApplication("wmic.exe", out path))
            {
                Printer.Print();
                Printer.Print("Unable to find wmic.exe in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.WMIC, path);
            Printer.Print($"WMIC\t\t\t\t\t[ Found ]");
        }

        /// <summary>
        /// Cleans up work space and waits for a key press to exit.
        /// </summary>
        private void IsDone()
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
