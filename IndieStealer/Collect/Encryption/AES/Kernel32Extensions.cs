using System;
using IndieStealer.Collect.Browsers;

namespace IndieStealer.Collect.AesGcm.AES
{
	public static class Kernel32Extensions
	{
		public static void ThrowOnError(this NTSTATUS status)
		{
			if (status.Severity == (NTSTATUS.SeverityCode)3221225472U)
			{
				throw new Exception();
			}
		}
	}
}
