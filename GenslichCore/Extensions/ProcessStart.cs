using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GenslichCore.Extensions
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(string lpApplicationName,
               string lpCommandLine, IntPtr lpProcessAttributes,
               IntPtr lpThreadAttributes,
               bool bInheritHandles, ProcessCreationFlags dwCreationFlags,
               IntPtr lpEnvironment, string lpCurrentDirectory,
               ref STARTUPINFO lpStartupInfo,
               out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);
    }
    public static class ProcessStart
    {
        public static bool StartProcess(string path, ProcessCreationFlags creationFlags)
        {
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            bool success = NativeMethods.CreateProcess(path, null,
                IntPtr.Zero, IntPtr.Zero, false,
                ProcessCreationFlags.CREATE_SUSPENDED,
                IntPtr.Zero, null, ref si, out pi);
            return success;
        }
    }
}
