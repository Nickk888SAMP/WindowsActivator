using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    static class TicketGenerator
    {
        static public bool GenerateGenuineTicket(string productPfn)
        {
            CreateTicketGenerator(out string generatorPath);
            string generateCommand = $@"{generatorPath} /c Pfn={productPfn}`;DownlevelGenuineState=1";
            CommandHandler.RunCommand(CommandHandler.CommandType.PowerShell, generateCommand);

            if (File.Exists(Paths.GetPath(Paths.Path.WorkDirectory) + @"\GenuineTicket.xml"))
            {
                return true;
            }
            return false;
        }

        static public void CreateTicketGenerator(out string generatorPath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "WindowsActivator.Resources.TicketGeneratorBinary";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                Misc.CreateFile("gtg.exe", out generatorPath);
                Stream s = File.Open(generatorPath, FileMode.Open, FileAccess.Write);
                stream.CopyTo(s);
                s.Dispose();

                RegistryHandler.CreateRegistrySubKeyEntry(RegistryHandler.RegistryRootKey.CURRENT_USER,
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers",
                    generatorPath,
                    "WINXPSP3",
                    RegistryValueKind.String);
            }
        }
    }
}
