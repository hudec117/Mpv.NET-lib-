using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API.Interop
{
    class LinuxDllLoadUtils : IDllLoadUtils {
        public IntPtr LoadLibrary(string fileName) {
            return dlopen(fileName, RTLD_NOW);
        }

        public void FreeLibrary(IntPtr handle) {
            dlclose(handle);
        }

        public IntPtr GetProcAddress(IntPtr dllHandle, string name) {
            // clear previous errors if any
            dlerror();
            var res = dlsym(dllHandle, name);
            var errPtr = dlerror();
            if (errPtr != IntPtr.Zero) {
                throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(errPtr));
            }
            return res;
        }

        const int RTLD_NOW = 2;

        [DllImport("libdl")]
        private static extern IntPtr dlopen(String fileName, int flags);
        
        [DllImport("libdl")]
        private static extern IntPtr dlsym(IntPtr handle, String symbol);

        [DllImport("libdl")]
        private static extern int dlclose(IntPtr handle);

        [DllImport("libdl")]
        private static extern IntPtr dlerror();
    }
}