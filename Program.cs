using System;
using System.Reflection;
using System.Threading;

namespace WindowsActivator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Gets the version of the application.
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Changes the title of the application.
            Console.Title = $"HWID Windows Activation v. {version}";

            // Showing the disclaimer.
            Printer.Print("Disclaimer:\n", ConsoleColor.Black, ConsoleColor.Red);
            Printer.Print("By using this Windows Activation application, you acknowledge and agree that the author of this software is\nnot responsible for any damages, including but not limited to data loss, system corruption, or any other issue\nthat may arise as a result of using this software.\n\nFurthermore, you agree to use this software at your own risk, and the author shall not be held liable for any\nconsequences that may result from its use.\n\nIf your system has been activated with a license key, this activator will replace it.\n\nThe author does not guarantee success or safety and cannot be held liable for any\npotential risks. The user assumes full responsibility for any risks associated with the use of this application.\n\nPlease note that due to the HWID activation approach, the activation will be permanent\nuntil the hardware has been changed.\n\nBy proceeding with the activation process, you confirm that you have read, understood,\nand agreed to the terms of this disclaimer.");
            Printer.Print("\nPress the 'Enter' key to proceed with the activation process.", ConsoleColor.Black, ConsoleColor.Green);

            // Checks for "Enter" Key
            if(Console.ReadKey().Key == ConsoleKey.Enter)
            {
                Console.Clear();
                new Activator().Run();
                return;
            }

            // Exit application when "Enter" key was not pressed.
            Printer.Print("\nNot agreed to disclaimer. Exiting...", ConsoleColor.Black, ConsoleColor.DarkYellow);
            Thread.Sleep(3000);
        }
    }
}
