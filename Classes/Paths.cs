namespace WindowsActivator
{
    static class Paths
    {
        public enum Path
        {
            CMD,
            PowerShell,
            SLMGR,
            CScript,
            ClipUp,
            WMIC,
            WorkDirectory
        }

        private static string cmdPath;
        private static string powerShellPath;
        private static string slmgrPath;
        private static string cScriptPath;
        private static string clipUpPath;
        private static string wmicPath;
        private static string applicationWorkDirectory;

        public static void SetPath(Path path, string fullPath)
        {
            switch (path)
            {
                case Path.CMD: 
                    cmdPath = fullPath;
                    break;
                case Path.PowerShell: 
                    powerShellPath = fullPath;
                    break;
                case Path.SLMGR: 
                    slmgrPath = fullPath;
                    break;
                case Path.CScript: 
                    cScriptPath = fullPath;
                    break;
                case Path.ClipUp: 
                    clipUpPath = fullPath;
                    break;
                case Path.WMIC: 
                    wmicPath = fullPath;
                    break;
                case Path.WorkDirectory: 
                    applicationWorkDirectory = fullPath;
                    break;
            }
        }

        public static string GetPath(Path path)
        {
            switch(path)
            {
                case Path.CMD: return cmdPath;
                case Path.PowerShell: return powerShellPath;
                case Path.SLMGR: return slmgrPath;
                case Path.CScript: return cScriptPath;
                case Path.ClipUp: return clipUpPath;
                case Path.WMIC: return wmicPath;
                case Path.WorkDirectory: return applicationWorkDirectory;
                default: return null;
            }
        }
    }
}
