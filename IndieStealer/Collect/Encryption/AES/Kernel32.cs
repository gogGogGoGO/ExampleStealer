using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IndieStealer.Collect.AesGcm.AES
{
    unsafe class Kernel32
    {
		[DllImport("Kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);
		public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		[DllImport("Kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeLibraryHandle LoadLibrary(string lpFileName);
		public unsafe static string FormatMessage(FormatMessageFlags dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, IntPtr[] Arguments, int maxAllowedBufferSize)
		{
			void* lpSource2 = lpSource.ToPointer();
			return FormatMessage(dwFlags, lpSource2, dwMessageId, dwLanguageId, Arguments, maxAllowedBufferSize);
		}
		[DllImport("Kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		public unsafe static extern int FormatMessage(FormatMessageFlags dwFlags, void* lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr[] Arguments);
		public unsafe static string FormatMessage(FormatMessageFlags dwFlags, void* lpSource, int dwMessageId, int dwLanguageId, IntPtr[] Arguments, int maxAllowedBufferSize)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			string result;
			while (!TryGetErrorMessage(dwFlags, lpSource, dwMessageId, dwLanguageId, stringBuilder, Arguments, out result))
			{
				if (GetLastError() != 122)
				{
					return null;
				}
				stringBuilder.Capacity *= 4;
				if (stringBuilder.Capacity >= maxAllowedBufferSize)
				{
					return null;
				}
			}
			return result;
		}
		private unsafe static bool TryGetErrorMessage(FormatMessageFlags flags, void* source, int messageId, int languageId, StringBuilder sb, IntPtr[] arguments, out string errorMsg)
		{
			errorMsg = string.Empty;
			if (FormatMessage(flags | FormatMessageFlags.FORMAT_MESSAGE_ARGUMENT_ARRAY, source, messageId, languageId, sb, sb.Capacity + 1, arguments) > 0)
			{
				int i;
				for (i = sb.Length; i > 0; i--)
				{
					char c = sb[i - 1];
					if (c > ' ' && c != '.')
					{
						break;
					}
				}
				errorMsg = sb.ToString(0, i);
				return true;
			}
			return false;
		}
		public static int GetLastError()
		{
			return Marshal.GetLastWin32Error();
		}
		[Flags]
		public enum FormatMessageFlags
		{
			FORMAT_MESSAGE_ALLOCATE_BUFFER = 256,
			FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192,
			FORMAT_MESSAGE_FROM_HMODULE = 2048,
			FORMAT_MESSAGE_FROM_STRING = 1024,
			FORMAT_MESSAGE_FROM_SYSTEM = 4096,
			FORMAT_MESSAGE_IGNORE_INSERTS = 512,
			FORMAT_MESSAGE_MAX_WIDTH_MASK = 255
		}
		public class SafeLibraryHandle : SafeHandle
		{
			public SafeLibraryHandle() : base(INVALID_HANDLE_VALUE, true)
			{
			}
			public SafeLibraryHandle(IntPtr preexistingHandle, bool ownsHandle = true) : base(INVALID_HANDLE_VALUE, ownsHandle)
			{
				SetHandle(preexistingHandle);
			}
			public override bool IsInvalid
			{
				get
				{
					return handle == INVALID_HANDLE_VALUE || handle == IntPtr.Zero;
				}
			}
			protected override bool ReleaseHandle()
			{
				return FreeLibrary(handle);
			}
			public static readonly SafeLibraryHandle Null = new SafeLibraryHandle(IntPtr.Zero);
			public static readonly SafeLibraryHandle Invalid = new SafeLibraryHandle();
		}

		
	}
}
