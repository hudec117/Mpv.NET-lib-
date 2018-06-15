using Mpv.NET.API.Interop;
using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MpvEventProperty
	{
		public string Name;

		public MpvFormat Format;

		public IntPtr Data;

		public string DataString
		{
			get
			{
				if (Format == MpvFormat.None || Data == IntPtr.Zero)
					return default(string);

				var innerPtr = MpvMarshal.GetInnerPtr(Data);

				return MpvMarshal.GetManagedUTF8StringFromPtr(innerPtr);
			}
		}

		public long DataLong
		{
			get
			{
				if (Format == MpvFormat.None || Data == IntPtr.Zero)
					return default(long);

				return Marshal.ReadInt64(Data);
			}
		}
	}
}