using Mpv.NET.API.Interop;
using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MpvEvent
	{
		public MpvEventID ID;

		public MpvError Error;

		public ulong ReplyUserData;

		public IntPtr Data;

		public TData? MarshalDataToStruct<TData>() where TData : struct
		{
			if (Data == IntPtr.Zero)
				return null;

			return MpvMarshal.PtrToStructure<TData>(Data);
		}
	}
}