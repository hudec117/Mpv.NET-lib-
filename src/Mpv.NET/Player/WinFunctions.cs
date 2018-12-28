using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.Player
{
	internal static class WinFunctions
	{
		// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-createwindowexa
		[DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
		internal static extern IntPtr CreateWindowEx(int dwExStyle,
													 string lpClassName,
													 string lpWindowName,
													 int dwStyle,
													 int x,
													 int y,
													 int nWidth,
													 int nHeight,
													 IntPtr hWndParent,
													 IntPtr hMenu,
													 IntPtr hInstance,
													 [MarshalAs(UnmanagedType.AsAny)]
													 object lpParam);

		// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-destroywindow
		[DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
		internal static extern bool DestroyWindow(IntPtr hwnd);
	}
}