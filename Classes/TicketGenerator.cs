using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using WindowsActivator.Classes;

namespace WindowsActivator
{
    static class TicketGenerator
    {
        private static readonly string password = "xS8ZxNHwqVz4rrznCqK7qFybbba2GdWQamU8WgkVZDAHJpxrX4UCr4zXdxcDCf63";

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
            string resourceName = "WindowsActivator.Resources.VGlja2V0R2VuZXJhdG9yLnhtbA==_encrypted";
            using (Stream streamEncrypted = assembly.GetManifestResourceStream(resourceName))
            {
                DecryptAndCreateGeneratorFile(streamEncrypted, password, out generatorPath);

                RegistryHandler.CreateRegistrySubKeyEntry(RegistryHandler.RegistryRootKey.CURRENT_USER,
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers",
                    generatorPath,
                    "WINXPSP3",
                    RegistryValueKind.String);
            }
        }

        static private void DecryptAndCreateGeneratorFile(Stream inputStream, string password, out string generatorPath)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;

                byte[] iv = new byte[aes.BlockSize / 8];
                inputStream.Read(iv, 0, iv.Length);

                Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(password, iv, 1000);
                aes.Key = keyGenerator.GetBytes(aes.KeySize / 8);
                aes.IV = iv;

                using (CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    generatorPath = $@"{Paths.GetPath(Paths.Path.WorkDirectory)}\TG.exe";
                    using (Stream s = File.Create(generatorPath))
                    {
                        cryptoStream.CopyTo(s);
                    }
                    

                }
            }
        }
    }
}
