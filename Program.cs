using System;
using System.Reflection;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    public static class Program
    {
        public static string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Preferences
        public static bool ignoreDisclaimer = false; // Arg: -id
        public static bool ignoreExistingActivation = false; // Arg: -iea
        public static bool autoCloseOnIsDone = false; // Arg: -ac

        // Starting point
        static void Main(string[] arguments)
        {
            Console.SetWindowSize(100, 30);
            Console.Clear();

            // Set working directory path
            Paths.SetPath(Paths.Path.AppDirectory, Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Windows Activator");

            // Arguments
            HandleArguments(arguments);

            // Changes the title of the application.
            Console.Title = $"Windows Activator v. {version}";

            // Get Operation System Apps
            Misc.CheckOSApps();

            // Menu
            while (true)
            {
                Console.Clear();

                // Init
                Printer.Print("Please wait...", true);
                bool systemActivated = Misc.IsSystemActivated();
                bool schedulerInstalled = ScheduledReactivator.IsInstalled();
                Console.Clear();

                // Activation
                Printer.Print("\n\t\t\tActivation\t\t\t\t", true, ConsoleColor.White, ConsoleColor.Black);
                Printer.Print(systemActivated ? "Windows is activated" : "Windows is not activated", true, systemActivated ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed, ConsoleColor.White);
                Printer.Print("\t\t\t\n", false, ConsoleColor.White, ConsoleColor.Black);

                Printer.Print("\t\t[1]\tActivate Windows with Auto-Renewal");
                Printer.Print("\t\t[2]\tActivate Windows");
                Printer.Print("\t\t[3]\tUninstall Completely");

                // Auto Renewal
                Printer.Print("\n\t\t\tAuto-Renewal\t\t\t\t", true, ConsoleColor.White, ConsoleColor.Black);
                Printer.Print(schedulerInstalled ? "Installed" : "Not Installed", true, schedulerInstalled ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed, ConsoleColor.White);
                Printer.Print("\t\t\t\t\t\n", false, ConsoleColor.White, ConsoleColor.Black);

                Printer.Print("\t\t[4]\tInstall Auto-Renewal");
                Printer.Print("\t\t[5]\tUninstall Auto-Renewal");

                // Manage Options
                Printer.Print("\n\tOption:  ", true);
                string key = Console.ReadLine();
                switch(key)
                {
                    case "1":
                        if(Activator.Install())
                        {
                            ScheduledReactivator.Install();
                        }
                        break;
                    case "2":
                        Activator.Install();
                        break;
                    case "3":
                        Activator.Uninstall();
                        break;
                    case "4":
                        ScheduledReactivator.Install();
                        break;
                    case "5":
                        ScheduledReactivator.Uninstall();
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the arguments
        /// </summary>
        /// <param name="arguments"></param>
        private static void HandleArguments(string[] arguments)
        {
            foreach (string argument in arguments)
            {
                switch (argument)
                {
                    case "-id": ignoreDisclaimer = true; break;
                    case "-iea": ignoreExistingActivation = true; break;
                    case "-ac": autoCloseOnIsDone = true; break;
                }
            }
        }
    }
}
