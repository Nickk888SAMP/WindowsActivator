using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace WindowsActivator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new HWID().Run();
        }
    }
}
