using System;

namespace WindowsActivator
{
    static class Paths
    {
        public static string cmdPath;
        public static string powerShellPath;
        public static string slmgrPath;
        public static string cScriptPath;
        public static string clipUpPath;
        public static string wmicPath;
        public static string applicationWorkDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\WindowsActivator";
    }
}
