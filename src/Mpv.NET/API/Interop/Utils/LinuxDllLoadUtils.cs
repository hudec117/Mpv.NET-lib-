using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API.Interop
{
    public class LinuxDllLoadUtils : IDllLoadUtils
    {
        [DllImport("libdl")]
        private static extern IntPtr dlopen(string fileName, int flags);

        [DllImport("libdl")]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport("libdl")]
        private static extern int dlclose(IntPtr handle);

        [DllImport("libdl")]
        private static extern IntPtr dlerror();

        private const int RTLD_NOW = 2;

        public IntPtr LoadLibrary(string fileName)
        {
            return dlopen(fileName, RTLD_NOW);
        }

        public void FreeLibrary(IntPtr handle)
        {
            dlclose(handle);
        }

        public IntPtr GetProcAddress(IntPtr dllHandle, string name)
        {
            // Clear previous errors if any.
            dlerror();

            var res = dlsym(dllHandle, name);

            var errPtr = dlerror();
            if (errPtr != IntPtr.Zero)
                throw new MpvAPIException("dlsym: " + Marshal.PtrToStringAnsi(errPtr));

            return res;
        }
    }
}