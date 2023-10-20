using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace WindowsActivator
{
    public static class RegistryHandler
    {
        public enum RegistryRootKey
        {
            CLASSES_ROOT,
            CURRENT_USER,
            LOCAL_MACHINE,
            USERS,
            CURRENT_CONFIG
        }

        /// <summary>
        /// Gets a registry value.
        /// </summary>
        /// <param name="rootKey"></param>
        /// <param name="subKey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetRegistryStringValue(RegistryRootKey rootKey, string subKey, string key)
        {
            try
            {
                RegistryKey registryKey;

                switch (rootKey)
                {
                    default:
                    case RegistryRootKey.CLASSES_ROOT:
                        registryKey = Registry.ClassesRoot.OpenSubKey(subKey);
                        break;
                    case RegistryRootKey.CURRENT_USER:
                        registryKey = Registry.CurrentUser.OpenSubKey(subKey);
                        break;
                    case RegistryRootKey.LOCAL_MACHINE:
                        registryKey = Registry.LocalMachine.OpenSubKey(subKey);
                        break;
                    case RegistryRootKey.USERS:
                        registryKey = Registry.Users.OpenSubKey(subKey);
                        break;
                    case RegistryRootKey.CURRENT_CONFIG:
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

        /// <summary>
        /// Creates a registry entry.
        /// </summary>
        /// <param name="rootKey"></param>
        /// <param name="subKey"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="valueKind"></param>
        public static void CreateRegistrySubKeyEntry(RegistryRootKey rootKey, string subKey, string name, object value, RegistryValueKind valueKind)
        {
            try
            {
                RegistryKey key;

                switch (rootKey)
                {
                    default:
                    case RegistryRootKey.CLASSES_ROOT:
                        key = Registry.ClassesRoot.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                    case RegistryRootKey.CURRENT_USER:
                        key = Registry.CurrentUser.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                    case RegistryRootKey.LOCAL_MACHINE:
                        key = Registry.LocalMachine.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                    case RegistryRootKey.USERS:
                        key = Registry.Users.CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                        break;
                    case RegistryRootKey.CURRENT_CONFIG:
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
