using Mpv.NET.Interop;
using System;
using System.Runtime.InteropServices;

namespace Mpv.NET
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
				CheckDataPointer();

				return MpvMarshal.GetManagedUTF8StringFromPtr(Data);
			}
		}

		public long DataLong
		{
			get
			{
				CheckDataPointer();

				return Marshal.ReadInt64(Data);
			}
		}

		public double DataDouble
		{
			get
			{
				CheckDataPointer();

				var dataBytes = new byte[sizeof(double)];

				Marshal.Copy(Data, dataBytes, 0, sizeof(double));

				return BitConverter.ToDouble(dataBytes, 0);
			}
		}

		private void CheckDataPointer()
		{
			if (Data == IntPtr.Zero)
				throw new MpvException("Invalid data pointer.");
		}
	}
}