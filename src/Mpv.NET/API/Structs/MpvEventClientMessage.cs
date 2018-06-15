using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MpvEventClientMessage
	{
		public int NumArgs;

		public IntPtr Args;
	}
}