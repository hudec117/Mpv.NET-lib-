using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API.Interop
{
	internal class WinFunctions
	{
		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms684175(v=vs.85).aspx
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms683152(v=vs.85).aspx
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
		public static extern int FreeLibrary(IntPtr hModule);

		// https://msdn.microsoft.com/en-us/library/windows/desktop/ms683212(v=vs.85).aspx
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string lProcName);
	}
}