using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API.Interop
{
    public interface IDllLoadUtils {
        IntPtr LoadLibrary(string fileName);
        void FreeLibrary(IntPtr handle);
        IntPtr GetProcAddress(IntPtr dllHandle, string name);
    }
    
    public static class PlatformDllLoadUtils{
        private static IDllLoadUtils SelectDllLoadUtils() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsDllLoadUtils();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxDllLoadUtils();
            }

            throw new Exception("Platform is unknown");
            // else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            // {
            //     _mediaPlayer.NsObject = _platformHandle.Handle;
            // }
        }
        public static IDllLoadUtils Get { get; private set; } = SelectDllLoadUtils();
    }
}