using System.Runtime.InteropServices;

namespace Mpv.NET
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MpvLogMessage
	{
		public string Prefix;
		
		public string Level;
		
		public string Text;

		public MpvLogLevel LogLevel;
	}
}