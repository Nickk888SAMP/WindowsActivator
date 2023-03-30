using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
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
            // Gets the version of the application.
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Set working directory path
            Paths.SetPath(Paths.Path.WorkDirectory, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\WindowsActivator");

            // Changes the title of the application.
            Console.Title = $"HWID Windows Activation v. {version}";

            // Create working directory if not exists.
            if (!Directory.Exists(Paths.GetPath(Paths.Path.WorkDirectory)))
            {
                Directory.CreateDirectory(Paths.GetPath(Paths.Path.WorkDirectory));
            }

            // Cheks if the OS is supported.
            if (!Misc.IsOSSupported(productData.Build))
            {
                Print.Write("\nUnsupported OS version detected.", ConsoleColor.DarkRed, ConsoleColor.White);
                Print.Write("HWID Activation is supported only for Windows 10/11.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            // Checks if OS is a windows server
            if (Misc.IsWindowsServer())
            {
                Print.Write("\nHWID Activation is not supported for Windows Server.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            // Searches for important system apps
            Print.Write("Searching important system applications", ConsoleColor.White, ConsoleColor.Black);
            CheckOSApps();

            // Diagnoses the system before activate attempt
            Print.Write("\nDiagnostic Tests", ConsoleColor.White, ConsoleColor.Black);
            Diagnose();

            // Tries to activate the system
            Print.Write($"\nProceeding with activation...\n");
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
                Print.Write("No product key Found!", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }

            // Tries to install the generic product key into the system
            command = $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /ipk {productKey}";
            output = CommandHandler.RunCommand(CommandHandler.CommandType.PowerShell, command);
            if (!output.Contains(productKey))
            {
                Print.Write("Generic product key couldn't be installed. Wrong product key?", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Print.Write("Generic Product Key\t\t\t[Installed]");

            // Tries to generate a GenuineTicket.xml file
            if (!TicketGenerator.GenerateGenuineTicket(productData.PFN))
            {
                Print.Write("Genuine ticket couldn't be generated.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Print.Write("Genuine Ticket\t\t\t\t[Generated]");

            // Tries to convert the GenuineTicket.xml wih the product key and the HWID
            command = $"{Paths.GetPath(Paths.Path.ClipUp)} -v -o -altto {Paths.GetPath(Paths.Path.WorkDirectory)}";
            output = CommandHandler.RunCommand(CommandHandler.CommandType.PowerShell, command);
            if (output.Contains("Done."))
            {
                Print.Write("Genuine Ticket Conversion\t\t[Success]");
                Print.Write("\nActivating...\n");
                CommandHandler.RunCommand(CommandHandler.CommandType.PowerShell, $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /ato");

                // Checks if the system has been activated
                if (Misc.IsSystemPermanentActivated())
                {
                    Print.Write($"{productData.Name} is permanently activated with a digital license.", ConsoleColor.DarkGreen, ConsoleColor.White);
                }
                else
                {
                    Print.Write($"Activation Failed.", ConsoleColor.DarkRed, ConsoleColor.White);
                }
            }
        }

        /// <summary>
        /// Diagnoses the system 
        /// </summary>
        private void Diagnose()
        {
            if (!Misc.IsElevated())
            {
                Print.Write("\nThis application require administrator privileges.");
                Print.Write("To do so, right click on this application and select 'Run as administrator'.");
                IsDone();
            }

            Print.Write($"Admin Priveleges\t\t\t[Yes]");

            if (!Misc.IsInternetConnected())
            {
                Print.Write("\nNo Internet Connection. Please ensure a stable internet connection to activate windows.");
                IsDone();
            }

            Print.Write("Internet Connection\t\t\t[Connected]");
            Print.Write($"OS Info\t\t\t\t\t[{productData.Name} | {productData.Build} | {productData.Architecture}]");

            if (Misc.IsSystemPermanentActivated())
            {
                Print.Write($"\n{productData.Name} is currently Permanently Activated.", ConsoleColor.DarkYellow, ConsoleColor.White);
                Print.Write($"Activation is not required.", ConsoleColor.DarkYellow, ConsoleColor.White);
                IsDone();
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
                Print.Write();
                Print.Write("Unable to find cmd.exe in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.CMD, path);
            Print.Write($"CMD\t\t\t\t\t[Found]");

            if (!Misc.GetSystemApplication("powershell.exe", out path))
            {
                Print.Write();
                Print.Write("Unable to find powershell.exe in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.PowerShell, path);
            Print.Write($"Powershell\t\t\t\t[Found]");

            if (!Misc.GetSystemApplication("slmgr.vbs", out path))
            {
                Print.Write();
                Print.Write("Unable to find slmgr.vbs in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.SLMGR, path);
            Print.Write($"SLMGR\t\t\t\t\t[Found]");

            if (!Misc.GetSystemApplication("cscript.exe", out path))
            {
                Print.Write();
                Print.Write("Unable to find cscript.exe in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.CScript, path);
            Print.Write($"CScript\t\t\t\t\t[Found]");

            if (!Misc.GetSystemApplication("ClipUp.exe", out path))
            {
                Print.Write();
                Print.Write("Unable to find clipup.exe in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.ClipUp, path);
            Print.Write($"ClipUp\t\t\t\t\t[Found]");

            if (!Misc.GetSystemApplication("wmic.exe", out path))
            {
                Print.Write();
                Print.Write("Unable to find wmic.exe in the system.", ConsoleColor.DarkRed, ConsoleColor.White);
                IsDone();
            }
            Paths.SetPath(Paths.Path.WMIC, path);
            Print.Write($"WMIC\t\t\t\t\t[Found]");
        }

        /// <summary>
        /// Cleans up work space and waits for a key press to exit.
        /// </summary>
        private void IsDone()
        {
            Print.Write("\nCleaning Up...");

            Misc.CleanUpWorkSpace();

            Print.Write("\nPress any key to Exit", new ConsoleColor(), ConsoleColor.DarkYellow);
            Console.ReadKey();
            Process.GetCurrentProcess().Kill();
        }
    }
}
