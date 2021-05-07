﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Genslich.Extensions
{
   public static class ProcessExtensions
   {
      #region Private Enums

      [Flags]
      public enum ThreadAccess : int
      {
         TERMINATE = (0x0001),
         SUSPEND_RESUME = (0x0002),
         GET_CONTEXT = (0x0008),
         SET_CONTEXT = (0x0010),
         SET_INFORMATION = (0x0020),
         QUERY_INFORMATION = (0x0040),
         SET_THREAD_TOKEN = (0x0080),
         IMPERSONATE = (0x0100),
         DIRECT_IMPERSONATION = (0x0200)
      }

      [Flags]
      public enum AllocationType
      {
         Commit = 0x1000,
         Reserve = 0x2000,
         Decommit = 0x4000,
         Release = 0x8000,
         Reset = 0x80000,
         Physical = 0x400000,
         TopDown = 0x100000,
         WriteWatch = 0x200000,
         LargePages = 0x20000000
      }

      #endregion Private Enums

      #region Public Methods

      public static void Resume(this Process process)
      {
         static void resume(ProcessThread pt)
         {
            IntPtr threadHandle = NativeMethods.OpenThread(
                ThreadAccess.SUSPEND_RESUME, false, (uint)pt.Id);

            if (threadHandle != IntPtr.Zero)
            {
               try { NativeMethods.ResumeThread(threadHandle); }
               finally { NativeMethods.CloseHandle(threadHandle); }
            }
         }

         ProcessThread[] threads = process.Threads.Cast<ProcessThread>().ToArray();

         if (threads.Length > 1)
         {
            Parallel.ForEach(threads,
                new ParallelOptions { MaxDegreeOfParallelism = threads.Length },
                pt => resume(pt));
         }
         else resume(threads[0]);
      }

      public static void Suspend(this Process process)
      {
         static void suspend(ProcessThread pt)
         {
            IntPtr threadHandle = NativeMethods.OpenThread(
                ThreadAccess.SUSPEND_RESUME, false, (uint)pt.Id);

            if (threadHandle != IntPtr.Zero)
            {
               try { NativeMethods.SuspendThread(threadHandle); }
               finally { NativeMethods.CloseHandle(threadHandle); }
            };
         }

         ProcessThread[] threads = process.Threads.Cast<ProcessThread>().ToArray();

         if (threads.Length > 1)
         {
            Parallel.ForEach(threads,
                new ParallelOptions { MaxDegreeOfParallelism = threads.Length },
                pt => suspend(pt));
         }
         else suspend(threads[0]);
      }

      #endregion Public Methods

      #region Private Classes

      [SuppressUnmanagedCodeSecurity]
      public static class NativeMethods
      {
         #region Public Methods

         [DllImport("kernel32.dll")]
         [return: MarshalAs(UnmanagedType.Bool)]
         public static extern bool CloseHandle(IntPtr hObject);

         [DllImport("kernel32.dll")]
         public static extern IntPtr OpenThread(
             ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

         [DllImport("kernel32.dll")]
         public static extern uint ResumeThread(IntPtr hThread);

         [DllImport("kernel32.dll")]
         public static extern uint SuspendThread(IntPtr hThread);

         [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
         public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
               int dwSize, AllocationType dwFreeType);

         #endregion Public Methods
      }

      #endregion Private Classes
   }
}
