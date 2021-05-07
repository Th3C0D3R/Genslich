﻿using System;
using GenslichCore.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Management.Automation;
using System.Security.Cryptography.Xml;

namespace GenslichCore.Extensions
{
   public static class DLLInjection
   {
      [DllImport("kernel32.dll")]
      public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

      [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
      public static extern IntPtr GetModuleHandle(string lpModuleName);

      [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
      static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

      [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
      static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
          uint dwSize, uint flAllocationType, uint flProtect);

      [DllImport("kernel32.dll", SetLastError = true)]
      static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

      [DllImport("kernel32.dll")]
      static extern IntPtr CreateRemoteThread(IntPtr hProcess,
          IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

      [DllImport("kernel32.dll", SetLastError = true)]
      static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);


      // privileges
      const int PROCESS_CREATE_THREAD = 0x0002;
      const int PROCESS_QUERY_INFORMATION = 0x0400;
      const int PROCESS_VM_OPERATION = 0x0008;
      const int PROCESS_VM_WRITE = 0x0020;
      const int PROCESS_VM_READ = 0x0010;

      // 
      const uint INFINITE = 0xFFFFFFFF;
      const uint WAIT_ABANDONED = 0x00000080;
      const uint WAIT_OBJECT_0 = 0x00000000;
      const uint WAIT_TIMEOUT = 0x00000102;

      // used for memory allocation
      const uint MEM_COMMIT = 0x00001000;
      const uint MEM_RESERVE = 0x00002000;
      const uint PAGE_READWRITE = 4;

      public static IntPtr Inject(Process ProcessName, string DLLPath)
      {
         // the target process - I'm using a dummy process for this
         // if you don't have one, open Task Manager and choose wisely
         Process targetProcess = ProcessName;

         // geting the handle of the process - with required privileges
         IntPtr procHandle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);

         // searching for the address of LoadLibraryA and storing it in a pointer
         IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

         // alocating some memory on the target process - enough to store the name of the dll
         // and storing its address in a pointer
         IntPtr allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((DLLPath.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

         // writing the name of the dll there
         WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(DLLPath), (uint)((DLLPath.Length + 1) * Marshal.SizeOf(typeof(char))), out _);

         // creating a thread that will call LoadLibraryA with allocMemAddress as argument
         IntPtr hThread = CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
         if (WaitForSingleObject(hThread, INFINITE) == WAIT_OBJECT_0)
            return hThread;
         else
            return IntPtr.Zero;
      }
      public static IntPtr OpenProcess(Process name)
      {
         return OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, name.Id);
      }
   }
}
