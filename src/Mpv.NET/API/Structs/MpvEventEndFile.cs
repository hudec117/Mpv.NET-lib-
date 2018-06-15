using System.Runtime.InteropServices;

namespace Mpv.NET.API
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MpvEventEndFile
	{
		public MpvEndFileReason Reason;

		public MpvError Error;
	}
}