﻿using System;
using System.Runtime.InteropServices;

namespace Mpv.NET.API.Interop
{
    internal class MpvStringMarshaler : ICustomMarshaler
    {
        private MarshalerCleanUpMethod CleanUpMethod { get; set; }

        public MpvStringMarshaler(MarshalerCleanUpMethod cleanUpMethod)
        {
            CleanUpMethod = cleanUpMethod;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            var cleanUpMethod = MarshalerCleanUpMethod.None;
            switch (cookie)
            {
                case "free-global":
                    cleanUpMethod = MarshalerCleanUpMethod.Global;
                    break;
                case "free-com":
                    cleanUpMethod = MarshalerCleanUpMethod.Com;
                    break;
            }

            return new MpvStringMarshaler(cleanUpMethod);
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            switch (CleanUpMethod)
            {
                case MarshalerCleanUpMethod.Com:
                    Marshal.FreeCoTaskMem(pNativeData);
                    break;
                case MarshalerCleanUpMethod.Global:
                    Marshal.FreeHGlobal(pNativeData);
                    break;
            }
        }

        public int GetNativeDataSize()
        {
            throw new NotImplementedException();
        }

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (!(managedObj is string @string))
                return IntPtr.Zero;

            var stringPtr = MpvMarshal.GetComPtrFromManagedUTF8String(@string);

            return stringPtr;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
                return null;

            var @string = MpvMarshal.GetManagedUTF8StringFromPtr(pNativeData);

            return @string;
        }
    }

    internal enum MarshalerCleanUpMethod
    {
        None,
        Com,
        Global
    }
}