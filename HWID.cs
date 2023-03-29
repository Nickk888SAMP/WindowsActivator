﻿using System;
using System.IO;


namespace WindowsActivator
{
    internal class HWID
    {
        ProductData productData = new ProductData(
            Misc.GetWindowsBuild(),
            Misc.GetWindowsArch(),
            Misc.GetProductName(),
            Misc.GetProductPfn(),
            Misc.GetEditionID());

        public HWID()
        {
            Console.Title = "HWID Windows Activation";


            if (!Directory.Exists(Paths.applicationWorkDirectory))
            {
                Directory.CreateDirectory(Paths.applicationWorkDirectory);
            }
            if (Misc.IsOSSupported(productData.Build))
            {
                if (Misc.IsWindowsServer())
                {
                    Console.WriteLine("HWID Activation is not supported for Windows Server.");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("Searching OS Apps");
                    Console.ResetColor();

                    if (Misc.IsOSAppAvailable("cmd.exe", out Paths.cmdPath))
                    {
                        Console.WriteLine($"CMD\t\t\t\t\t[Found]");

                        if (Misc.IsOSAppAvailable("powershell.exe", out Paths.powerShellPath))
                        {
                            Console.WriteLine($"Powershell\t\t\t\t[Found]");

                            if (Misc.IsOSAppAvailable("slmgr.vbs", out Paths.slmgrPath))
                            {
                                Console.WriteLine($"SLMGR\t\t\t\t\t[Found]");

                                if (Misc.IsOSAppAvailable("cscript.exe", out Paths.cScriptPath))
                                {
                                    Console.WriteLine($"CScript\t\t\t\t\t[Found]");

                                    if (Misc.IsOSAppAvailable("ClipUp.exe", out Paths.clipUpPath))
                                    {
                                        Console.WriteLine($"ClipUp\t\t\t\t\t[Found]");

                                        if (Misc.IsOSAppAvailable("wmic.exe", out Paths.wmicPath))
                                        {
                                            Console.WriteLine($"WMIC\t\t\t\t\t[Found]");

                                            if (Misc.IsElevated())
                                            {
                                                Console.WriteLine();
                                                Console.BackgroundColor = ConsoleColor.White;
                                                Console.ForegroundColor = ConsoleColor.Black;
                                                Console.WriteLine("Diagnostic Tests");
                                                Console.ResetColor();
                                                Console.WriteLine($"Admin Priveleges\t\t\t[Yes]");
                                                if (Misc.IsInternetConnected())
                                                {
                                                    Console.WriteLine("Internet Connection\t\t\t[Connected]");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("No Internet Connection. Please ensure a stable internet connection to activate windows.");
                                                }

                                                Initialize();
                                            }
                                            else
                                            {
                                                Console.WriteLine("This application require administrator privileges.");
                                                Console.WriteLine("To do so, right click on this application and select 'Run as administrator'.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Unable to find wmic.exe in the system.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Unable to find clipup.exe in the system.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Unable to find cscript.exe in the system.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Unable to find slmgr.vbs in the system.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Unable to find powershell.exe in the system.");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Unable to find cmd.exe in the system.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Unsupported OS version detected.");
                Console.WriteLine("HWID Activation is supported only for Windows 10/11.");
                Console.WriteLine("Use Online KMS Activation option.");
            }

            Console.WriteLine();
            Console.WriteLine("Cleaning Up...");
            Misc.CleanUpWorkSpace();
            IsDone();
        }

        private void Initialize()
        {
            Console.WriteLine($"OS Info\t\t\t\t\t[{productData.Name} | {productData.Build} | {productData.Architecture}]");

            if (Misc.IsSystemPermanentActivated())
            {
                Console.WriteLine();
                Console.WriteLine($"{productData.Name} is Permanently Activated.");
                Console.WriteLine($"Activation is not required.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"Proceeding with activation...");
                Console.WriteLine();

                if (GenericWindowsKeys.TryGetProductKey(productData.EditionID, out string productKey))
                {
                    string keyOutput = Misc.RunCommand(Misc.CommandHandler.PowerShell, $"{Paths.cScriptPath} {Paths.slmgrPath} /ipk {productKey}");
                    if (keyOutput.Contains(productKey))
                    {
                        Console.WriteLine("Generic Product Key\t\t\t[Installed]");
                        if (TicketGenerator.GenerateGenuineTicket(productData.PFN))
                        {
                            Console.WriteLine("Genuine Ticket\t\t\t\t[Generated]");

                            string clipUpOutput = Misc.RunCommand(Misc.CommandHandler.PowerShell, $"{Paths.clipUpPath} -v -o -altto {Paths.applicationWorkDirectory}");
                            if(clipUpOutput.Contains("Done."))
                            {
                                Console.WriteLine("Genuine Ticket Conversion\t\t[Success]");
                                Console.WriteLine();
                                Console.WriteLine("Activating...");
                                Console.WriteLine();
                                Misc.RunCommand(Misc.CommandHandler.PowerShell, $"{Paths.cScriptPath} {Paths.slmgrPath} /ato");
                                    
                                if(Misc.IsSystemPermanentActivated())
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
                        }
                        else
                        {
                            Console.WriteLine("Genuine ticket couldn't be generated.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Generic product key couldn't be installed. Wrong product key?");
                    }

                }
                else
                {
                    Console.WriteLine("No product key Found!");
                }
            }
        }

        private void IsDone()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nPress any key to Exit");
            Console.ResetColor();
            Console.ReadKey();
        }


    }
}