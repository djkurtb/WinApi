using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WinApi.NtDll
{
    public static class NtDllMethods
    {
        public const string LibraryName = "ntdll.dll";

        [DllImport(LibraryName, ExactSpelling = true)]
        public static extern NtStatus NtQuerySystemInformation(SystemInformationClass systemInformationClass, IntPtr systemInformation, uint systemInformationLength, out uint returnLength);

        public static IntPtr NtQuerySystemInformation(SystemInformationClass systemInformationClass, uint infoLength = 0)
        {
            if (infoLength == 0)
                infoLength = 0x10000;

            var infoPtr = Marshal.AllocHGlobal((int)infoLength);

            var tries = 0;
            while (true)
            {
                var result = NtQuerySystemInformation(SystemInformationClass.SystemHandleInformation, infoPtr, infoLength, out infoLength);

                if (result == NtStatus.Success)
                    return infoPtr;

                Marshal.FreeHGlobal(infoPtr);  //free pointer when not Successful

                if (result != NtStatus.InfoLengthMismatch && result != NtStatus.BufferOverflow && result != NtStatus.BufferTooSmall)
                {
                    //throw new Exception("Unhandled NtStatus " + result);
                    return IntPtr.Zero;
                }

                if (++tries > 5)
                    return IntPtr.Zero;

                infoPtr = Marshal.AllocHGlobal((int)infoLength);
            }
        }
    }
}
