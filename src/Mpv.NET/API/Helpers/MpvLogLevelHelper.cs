namespace Mpv.NET.API
{
	public static class MpvLogLevelHelper
	{
		public static string GetStringForLogLevel(MpvLogLevel logLevel)
		{
			switch (logLevel)
			{
				case MpvLogLevel.None:
					return "no";
				case MpvLogLevel.Fatal:
					return "fatal";
				case MpvLogLevel.Error:
					return "error";
				case MpvLogLevel.Warning:
					return "warn";
				case MpvLogLevel.Info:
					return "info";
				case MpvLogLevel.V:
					return "v";
				case MpvLogLevel.Debug:
					return "debug";
				case MpvLogLevel.Trace:
					return "trace";
				default:
					return null;
			}
		}
	}
}