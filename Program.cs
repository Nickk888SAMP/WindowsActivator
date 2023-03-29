using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace WindowsActivator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HWID hWID = new HWID();

           
        }

        static public Stream CreateFile(string fileName, out string filePath)
        {
            filePath = $@"C:\Files\ticket.exe";
            using (Stream s = File.Create(filePath))
            {
                return s;
            }

        }
        static public void CreateTicket(out string generatorPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "WindowsLoader.Resources.gatherosstatemodified";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                Debug.WriteLine(stream);
                CreateFile("gtg.exe", out generatorPath);
                Stream s = File.Open(generatorPath, FileMode.Open, FileAccess.Write);
                stream.CopyTo(s);
                s.Dispose();

            }

        }
    }
}
