using System.Runtime.InteropServices;

namespace Mpv.NET.API
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MpvEventStartFile
    {
        public ulong PlaylistEntryId;
    }
}