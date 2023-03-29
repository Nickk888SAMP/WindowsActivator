using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace WindowsActivator
{
    static class TicketGenerator
    {
        static public bool GenerateGenuineTicket(string productPfn)
        {
            CreateTicketGenerator(out string generatorPath);

            string generateCommand = $@"{generatorPath} /c Pfn={productPfn}`;DownlevelGenuineState=1";
            Misc.RunCommand(Misc.CommandHandler.PowerShell, generateCommand);

            if (File.Exists(Paths.applicationWorkDirectory + @"\GenuineTicket.xml"))
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
                Debug.WriteLine(stream.ToString());
                Misc.CreateFile("gtg.exe", out generatorPath);
                Stream s = File.Open(generatorPath, FileMode.Open, FileAccess.Write);
                stream.CopyTo(s);
                s.Dispose();

                Misc.CreateRegistrySubKeyEntry(Misc.RegistryRootKey.CURRENT_USER,
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers",
                    generatorPath,
                    "WINXPSP3",
                    RegistryValueKind.String);
            }
        }
    }
}
