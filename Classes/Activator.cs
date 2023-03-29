using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
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

            Paths.SetPath(Paths.Path.WorkDirectory, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\WindowsActivator");

            // Changes the title of the application.
            Console.Title = $"HWID Windows Activation {version}";

            // Create working directory if not exists.
            if (!Directory.Exists(Paths.GetPath(Paths.Path.WorkDirectory)))
            {
                Directory.CreateDirectory(Paths.GetPath(Paths.Path.WorkDirectory));
            }

            // Cheks if the OS is supported.
            if (!Misc.IsOSSupported(productData.Build))
            {
                Console.WriteLine();
                Console.WriteLine("Unsupported OS version detected.");
                Console.WriteLine("HWID Activation is supported only for Windows 10/11.");
                Console.WriteLine("Use Online KMS Activation option.");
                IsDone();
                return;
            }

            // Checks if OS is a windows server
            if (Misc.IsWindowsServer())
            {
                Console.WriteLine();
                Console.WriteLine("HWID Activation is not supported for Windows Server.");
                IsDone();
                return;
            }

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Searching OS Apps");
            Console.ResetColor();

            string path;
            string output;
            string command;

            if (!Misc.IsOSAppAvailable("cmd.exe", out path))
            {
                Console.WriteLine();
                Console.WriteLine("Unable to find cmd.exe in the system.");
                IsDone();
                return;
            }
            Paths.SetPath(Paths.Path.CMD, path);
            Console.WriteLine($"CMD\t\t\t\t\t[Found]");

            if (!Misc.IsOSAppAvailable("powershell.exe", out path))
            {
                Console.WriteLine();
                Console.WriteLine("Unable to find powershell.exe in the system.");
                IsDone();
                return;
            }
            Paths.SetPath(Paths.Path.PowerShell, path);
            Console.WriteLine($"Powershell\t\t\t\t[Found]");

            if (!Misc.IsOSAppAvailable("slmgr.vbs", out path))
            {
                Console.WriteLine();
                Console.WriteLine("Unable to find slmgr.vbs in the system.");
                IsDone();
                return;
            }
            Paths.SetPath(Paths.Path.SLMGR, path);
            Console.WriteLine($"SLMGR\t\t\t\t\t[Found]");

            if (!Misc.IsOSAppAvailable("cscript.exe", out path))
            {
                Console.WriteLine();
                Console.WriteLine("Unable to find cscript.exe in the system.");
                IsDone();
                return;
            }
            Paths.SetPath(Paths.Path.CScript, path);
            Console.WriteLine($"CScript\t\t\t\t\t[Found]");

            if (!Misc.IsOSAppAvailable("ClipUp.exe", out path))
            {
                Console.WriteLine();
                Console.WriteLine("Unable to find clipup.exe in the system.");
                IsDone();
                return;
            }
            Paths.SetPath(Paths.Path.ClipUp, path);
            Console.WriteLine($"ClipUp\t\t\t\t\t[Found]");

            if (!Misc.IsOSAppAvailable("wmic.exe", out path))
            {
                Console.WriteLine();
                Console.WriteLine("Unable to find wmic.exe in the system.");
                IsDone();
                return;
            }
            Paths.SetPath(Paths.Path.WMIC, path);
            Console.WriteLine($"WMIC\t\t\t\t\t[Found]");

            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Diagnostic Tests");
            Console.ResetColor();

            if (!Misc.IsElevated())
            {
                Console.WriteLine();
                Console.WriteLine("This application require administrator privileges.");
                Console.WriteLine("To do so, right click on this application and select 'Run as administrator'.");
                IsDone();
                return;
            }
            Console.WriteLine($"Admin Priveleges\t\t\t[Yes]");

            if (!Misc.IsInternetConnected())
            {
                Console.WriteLine();
                Console.WriteLine("No Internet Connection. Please ensure a stable internet connection to activate windows.");
                IsDone();
                return;
            }
            Console.WriteLine("Internet Connection\t\t\t[Connected]");

            Console.WriteLine($"OS Info\t\t\t\t\t[{productData.Name} | {productData.Build} | {productData.Architecture}]");

            if (Misc.IsSystemPermanentActivated())
            {
                Console.WriteLine();
                Console.WriteLine($"{productData.Name} is currently Permanently Activated.");
                Console.WriteLine($"Activation is not required.");
                IsDone();
                return;
            }
            Console.WriteLine();
            Console.WriteLine($"Proceeding with activation...");
            Console.WriteLine();

            if (!GenericWindowsKeys.TryGetProductKey(productData.EditionID, out string productKey))
            {
                Console.WriteLine("No product key Found!");
                IsDone();
                return;
            }

            command = $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /ipk {productKey}";
            output = CommandHandler.RunCommand(CommandHandler.CommandType.PowerShell, command);
            if (!output.Contains(productKey))
            {
                Console.WriteLine("Generic product key couldn't be installed. Wrong product key?");
                IsDone();
                return;
            }
            Console.WriteLine("Generic Product Key\t\t\t[Installed]");

            if (!TicketGenerator.GenerateGenuineTicket(productData.PFN))
            {
                Console.WriteLine("Genuine ticket couldn't be generated.");
                IsDone();
                return;
            }
            Console.WriteLine("Genuine Ticket\t\t\t\t[Generated]");

            command = $"{Paths.GetPath(Paths.Path.ClipUp)} -v -o -altto {Paths.GetPath(Paths.Path.WorkDirectory)}";
            output = CommandHandler.RunCommand(CommandHandler.CommandType.PowerShell, command);
            if (output.Contains("Done."))
            {
                Console.WriteLine("Genuine Ticket Conversion\t\t[Success]");
                Console.WriteLine();
                Console.WriteLine("Activating...");
                Console.WriteLine();
                CommandHandler.RunCommand(CommandHandler.CommandType.PowerShell, $"{Paths.GetPath(Paths.Path.CScript)} {Paths.GetPath(Paths.Path.SLMGR)} /ato");

                if (Misc.IsSystemPermanentActivated())
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"{productData.Name} is permanently activated with a digital license.");
                    Console.ResetColor();
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Activation Failed.");
                    Console.ResetColor();
                }
            }

            IsDone();
        }

        /// <summary>
        /// Cleans up work space and waits for a key press to exit.
        /// </summary>
        private void IsDone()
        {
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Cleaning Up...");

            Misc.CleanUpWorkSpace();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nPress any key to Exit");
            Console.ResetColor();
            Console.ReadKey();
        }


    }
}
