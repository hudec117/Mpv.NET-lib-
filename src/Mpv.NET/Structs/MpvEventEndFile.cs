using System.Runtime.InteropServices;

namespace Mpv.NET
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MpvEventEndFile
	{
		public MpvEndFileReason Reason;

		public MpvError Error;
	}
}