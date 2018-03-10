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

				var innerPtrBytes = new byte[IntPtr.Size];
				Marshal.Copy(Data, innerPtrBytes, 0, IntPtr.Size);

				var innerPtrValue = BitConverter.ToInt32(innerPtrBytes, 0);
				var innerPtr = new IntPtr(innerPtrValue);

				return MpvMarshal.GetManagedUTF8StringFromPtr(innerPtr);
			}
		}

		private void CheckDataPointer()
		{
			if (Data == IntPtr.Zero)
				throw new MpvException("Invalid data pointer.");
		}
	}
}