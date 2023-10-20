using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    public static class Program
    {
        // Preferences
        public static bool ignoreDisclaimer = false; // Arg: -id
        public static bool ignoreExistingActivation = false; // Arg: -iea
        public static bool autoCloseOnIsDone = false; // Arg: -ac

        // Starting point
        static void Main(string[] arguments)
        {
            // Set working directory path
            Paths.SetPath(Paths.Path.AppDirectory, Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Windows Activator");

            // Arguments
            HandleArguments(arguments);

            // Gets the version of the application.
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Changes the title of the application.
            Console.Title = $"Windows Activator v. {version} [Pre-Release]";

            // Disclaimer
            if (!ignoreDisclaimer)
            {
                // Showing the disclaimer.
                Printer.Print("Disclaimer:\n", ConsoleColor.Black, ConsoleColor.Red);
                Printer.Print("By using this Windows Activation application, you acknowledge and agree that the author of this software is\nnot responsible for any damages, including but not limited to data loss, system corruption, or any other issue\nthat may arise as a result of using this software.\n\nFurthermore, you agree to use this software at your own risk, and the author shall not be held liable for any\nconsequences that may result from its use.\n\nIf your system has been activated with a license key, this activator will replace it.\n\nThe author does not guarantee success or safety and cannot be held liable for any\npotential risks. The user assumes full responsibility for any risks associated with the use of this application.\n\nBy proceeding with the activation process, you confirm that you have read, understood,\nand agreed to the terms of this disclaimer.");
                Printer.Print("\nPress the 'Enter' key to proceed with the activation process.", ConsoleColor.Black, ConsoleColor.Green);

                // Checks for "Enter" Key
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    Activator.Run();
                    return;
                }

                // Exit application when "Enter" key was not pressed.
                Printer.Print("\nNot agreed to disclaimer. Exiting...", ConsoleColor.Black, ConsoleColor.DarkYellow);
                Thread.Sleep(3000);
            }
            else
            {
                Activator.Run();
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
