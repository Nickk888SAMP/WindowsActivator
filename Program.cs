using System;
using System.Reflection;
using System.Threading;

namespace WindowsActivator
{
    public static class Program
    {
        public static bool ignoreDisclaimer = false; // Arg: -id
        public static bool ignoreExistingActivation = false; // Arg: -iea
        public static bool autoCloseOnIsDone = false; // Arg: -ac
        public static bool noOutput = false; // Arg: -no

        static void Main(string[] args)
        {
            // Arguments
            foreach (string arg in args)
            {
                if(arg == "-id") ignoreDisclaimer = true;
                else if(arg == "-iea") ignoreExistingActivation = true;
                else if (arg == "-ac") autoCloseOnIsDone = true;
                else if (arg == "-no") noOutput = true;
            }

            /*TaskDefinition td = TaskService.Instance.NewTask();

            td.RegistrationInfo.Author = "Nickk888";
            td.RegistrationInfo.Description = "Activates windows";
            td.Actions.Add(new ExecAction(@"C:\Users\Test\Desktop\Windows Activator.exe"));
            Trigger trigger = new Trigger();

            td.Triggers.Add()

            TaskService.Instance.RootFolder.CreateFolder("Windows Activator", null, false);
            var folder = TaskService.Instance.GetFolder("Windows Activator");
            folder.RegisterTaskDefinition("Activator Windows", td);*/

            // Gets the version of the application.
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Changes the title of the application.
            Console.Title = $"Windows Activator v. {version}";

            // Activator 
            Activator activator = new Activator();

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
                    activator.Run();
                    return;
                }

                // Exit application when "Enter" key was not pressed.
                Printer.Print("\nNot agreed to disclaimer. Exiting...", ConsoleColor.Black, ConsoleColor.DarkYellow);
                Thread.Sleep(3000);
            }
            else
            {
                activator.Run();
            }
        }
    }
}
