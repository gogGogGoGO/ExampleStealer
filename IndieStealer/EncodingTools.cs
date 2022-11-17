using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace IndieStealer
{
    class EncodingTools
    { 

        public static string Xor(string ln, string ky)
        {          
            var result = new StringBuilder();
            for (int c = 0; c < ln.Length; c++)
                result.Append((char)((uint)ln[c] ^ (uint)ky[(c + 1) % (ky.Length - 1)]));
            return result.ToString();
        }
        static Random random = new Random();
        public static string RandomStr(int minlength, int maxlength)
        {
            
            int length = random.Next(minlength, maxlength + 1);
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = string.Empty;
            for (int i = 0; i < length; i++)
            {
                result += letters[random.Next(0, letters.Length)];
            }
            return result;
        }
    }
}
