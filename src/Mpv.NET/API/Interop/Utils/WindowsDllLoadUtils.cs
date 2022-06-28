using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API.Interop
{
    class WindowsDllLoadUtils : IDllLoadUtils {
        void IDllLoadUtils.FreeLibrary(IntPtr handle) {
            FreeLibrary(handle);
        }

        IntPtr IDllLoadUtils.GetProcAddress(IntPtr dllHandle, string name) {
            return GetProcAddress(dllHandle, name);
        }

        IntPtr IDllLoadUtils.LoadLibrary(string fileName) {
            return LoadLibrary(fileName);
        }

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll")]
        private static extern int FreeLibrary(IntPtr handle);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress (IntPtr handle, string procedureName);
    }
}