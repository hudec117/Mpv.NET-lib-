﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Mpv.NET.API.Interop
{
    public static class MpvMarshal
    {
        public static IntPtr GetComPtrFromManagedUTF8String(string @string)
        {
            Guard.AgainstNull(@string, nameof(@string));

            @string += '\0';

            var stringBytes = Encoding.UTF8.GetBytes(@string);
            var stringBytesCount = stringBytes.Length;

            var stringPtr = Marshal.AllocCoTaskMem(stringBytesCount);
            Marshal.Copy(stringBytes, 0, stringPtr, stringBytesCount);

            return stringPtr;
        }

        public static string GetManagedUTF8StringFromPtr(IntPtr stringPtr)
        {
            if (stringPtr == IntPtr.Zero)
                throw new ArgumentException("Cannot get string from invalid pointer.");

            var stringBytes = new List<byte>();
            var offset = 0;

            // Just to be safe!
            while (offset < short.MaxValue)
            {
                var @byte = Marshal.ReadByte(stringPtr, offset);
                if (@byte == '\0')
                    break;

                stringBytes.Add(@byte);

                offset++;
            }

            var stringBytesArray = stringBytes.ToArray();

            return Encoding.UTF8.GetString(stringBytesArray);
        }

        public static IntPtr GetComPtrForManagedUTF8StringArray(string[] inArray, out IntPtr[] outArray)
        {
            Guard.AgainstNull(inArray, nameof(inArray));

            var numberOfStrings = inArray.Length + 1;

            outArray = new IntPtr[numberOfStrings];

            // Allocate COM memory since this array will be passed to
            // a C function. This allocates space for the pointers that will point
            // to each string.
            var rootPointer = Marshal.AllocCoTaskMem(IntPtr.Size * numberOfStrings);

            for (var index = 0; index < inArray.Length; index++)
            {
                var currentString = inArray[index];
                var currentStringPtr = GetComPtrFromManagedUTF8String(currentString);

                outArray[index] = currentStringPtr;
            }

            Marshal.Copy(outArray, 0, rootPointer, numberOfStrings);

            return rootPointer;
        }

        public static void FreeComPtrArray(IntPtr[] ptrArray)
        {
            Guard.AgainstNull(ptrArray, nameof(ptrArray));

            foreach (var intPtr in ptrArray)
                Marshal.FreeCoTaskMem(intPtr);
        }

        public static TStruct PtrToStructure<TStruct>(IntPtr ptr) where TStruct : struct
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentException("Invalid pointer.");

            return (TStruct)Marshal.PtrToStructure(ptr, typeof(TStruct));
        }

        public static TDelegate LoadUnmanagedFunction<TDelegate>(IntPtr dllHandle, string functionName) where TDelegate : class
        {
            if (dllHandle == IntPtr.Zero)
                throw new ArgumentException("DLL handle is invalid.", nameof(dllHandle));

            Guard.AgainstNullOrEmptyOrWhiteSpaceString(functionName, nameof(functionName));

            var functionPtr = PlatformDll.Utils.GetProcAddress(dllHandle, functionName);
            if (functionPtr == IntPtr.Zero)
                return null;

            return (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(functionPtr, typeof(TDelegate));
        }
    }
}