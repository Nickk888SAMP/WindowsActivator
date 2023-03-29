using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsActivator
{
    static class Enums
    {
        public enum RegistryRootKey
        {
            CLASSES_ROOT,
            CURRENT_USER,
            LOCAL_MACHINE,
            USERS,
            CURRENT_CONFIG
        }

        public enum CommandHandler
        {
            CMD,
            PowerShell
        }
    }
}
