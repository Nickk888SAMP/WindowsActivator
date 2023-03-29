using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsActivator.Misc;

namespace WindowsActivator
{
    static class RegistryHandler
    {
        static public string GetRegistryStringValue(Enums.RegistryRootKey rootKey, string subKey, string key)
        {
            try
            {
                RegistryKey registryKey;

                switch (rootKey)
                {
                    default:
                    case Enums.RegistryRootKey.CLASSES_ROOT:
                        registryKey = Registry.ClassesRoot.OpenSubKey(subKey);
                        break;
                    case Enums.RegistryRootKey.CURRENT_USER:
                        registryKey = Registry.CurrentUser.OpenSubKey(subKey);
                        break;
                    case Enums.RegistryRootKey.LOCAL_MACHINE:
                        registryKey = Registry.LocalMachine.OpenSubKey(subKey);
                        break;
                    case Enums.RegistryRootKey.USERS:
                        registryKey = Registry.Users.OpenSubKey(subKey);
                        break;
                    case Enums.RegistryRootKey.CURRENT_CONFIG:
                        registryKey = Registry.CurrentConfig.OpenSubKey(subKey);
                        break;
                }

                return (string)registryKey.GetValue(key);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Exception");
                return "";
            }

        }

        static public void CreateRegistrySubKeyEntry(Enums.RegistryRootKey rootKey, string subKey, string name, object value, RegistryValueKind valueKind)
        {
            try
            {
                RegistryKey key;

                switch (rootKey)
                {
                    default:
                    case Enums.RegistryRootKey.CLASSES_ROOT:
                        key = Registry.ClassesRoot.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                    case Enums.RegistryRootKey.CURRENT_USER:
                        key = Registry.CurrentUser.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                    case Enums.RegistryRootKey.LOCAL_MACHINE:
                        key = Registry.LocalMachine.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                    case Enums.RegistryRootKey.USERS:
                        key = Registry.Users.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                    case Enums.RegistryRootKey.CURRENT_CONFIG:
                        key = Registry.CurrentConfig.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                }

                key.SetValue(name, value, valueKind);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error Exception");
            }
        }
    }
}
