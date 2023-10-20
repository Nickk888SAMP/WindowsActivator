namespace WindowsActivator
{
    public static class Paths
    {
        public enum Path
        {
            CMD,
            SLMGR,
            CScript,
            WMIC,
            AppDirectory
        }

        private static string cmdPath = null;
        private static string slmgrPath = null;
        private static string cScriptPath = null;
        private static string wmicPath = null;
        private static string applicationWorkDirectory = null;

        /// <summary>
        /// Sets the path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fullPath"></param>
        public static void SetPath(Path path, string fullPath)
        {
            switch (path)
            {
                case Path.CMD: 
                    cmdPath = fullPath;
                    break;
                case Path.SLMGR: 
                    slmgrPath = fullPath;
                    break;
                case Path.CScript: 
                    cScriptPath = fullPath;
                    break;
                case Path.WMIC: 
                    wmicPath = fullPath;
                    break;
                case Path.AppDirectory: 
                    applicationWorkDirectory = fullPath;
                    break;
            }
        }

        /// <summary>
        /// Gets the Path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPath(Path path)
        {
            switch(path)
            {
                case Path.CMD: return cmdPath;
                case Path.SLMGR: return slmgrPath;
                case Path.CScript: return cScriptPath;
                case Path.WMIC: return wmicPath;
                case Path.AppDirectory: return applicationWorkDirectory;
                default: return null;
            }
        }
    }
}
