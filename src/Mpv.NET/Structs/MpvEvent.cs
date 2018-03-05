using System;
using System.Runtime.InteropServices;

namespace Mpv.NET
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MpvEvent
	{
		public MpvEventID ID;

		public MpvError Error;

		public ulong ReplyUserData;

		public IntPtr Data;
	}
}