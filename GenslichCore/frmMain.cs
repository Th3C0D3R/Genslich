using Genslich.Extensions;
using GenslichCore.Extensions;
using GenslichCore.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace Genslich
{
   public enum HotKeys
   {
      Numpad1,

   }
   public partial class frmMain : Form
   {
      #region "Stuff"
      [DllImport("user32.dll")]
      public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
      [DllImport("user32.dll")]
      public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
      [DllImport("kernel32.dll", SetLastError = true)]
      private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
      [DllImport("kernel32.dll", SetLastError = true)]
      private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] buffer, int size, out int lpNumberOfBytesWritten);
      #endregion


      private string PathToExe = "";
      private IntPtr ProcHandle = IntPtr.Zero;
      private Process Genshin = null;
      private bool Patched = false;
      private BackgroundWorker bwKeyHandle = new BackgroundWorker();
      public frmMain()
      {
         InitializeComponent();
         Settings.Default.Reload();
         if (!string.IsNullOrWhiteSpace(Settings.Default.GenshinFilePath))
         {
            PathToExe = Settings.Default.GenshinFilePath;
            txtPathToExe.Text = PathToExe;
            btnStart.Enabled = true;
            btnStart.Text = "START && INJECT";
         }
         if (!RegisterHotKey(Handle, (int)HotKeys.Numpad1, 0, (int)Keys.NumPad1))
            MessageBox.Show($"FAILED TO REGISTER HOTKEY {HotKeys.Numpad1}");
      }

      private void BtnStart_Click(object sender, EventArgs e)
      {
         if (!string.IsNullOrEmpty(PathToExe))
         {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            (bool, PROCESS_INFORMATION) data = ProcessStart.StartProcess(PathToExe, ProcessCreationFlags.ZERO_FLAG);
            if (data.Item1)
            {
               Process genshin = Process.GetProcesses().ToList().Find((p) => p.ProcessName == "GenshinImpact");
               if (genshin != null)
               {
                  try
                  {
                     //string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "HelloWorldDLL.dll");
                    //if(!File.Exists(path)) throw new ApplicationException($"Code: {path} does not exists");
                     IntPtr procHandle = DLLInjection.OpenProcess(genshin);
                     if (IntPtr.Zero == procHandle) throw new ApplicationException($"Code: procHandle has value 0x{procHandle}");
                     ProcHandle = procHandle;
                     Genshin = genshin;
                     ProcessExtensions.Resume(Genshin);
                  }
                  catch (ApplicationException ex)
                  {
                     MessageBox.Show("ERROR:\n" + ex.Message);
                  }
                  finally
                  {
                     ProcessExtensions.NativeMethods.CloseHandle(data.Item2.hProcess);
                  }
               }
               else
               {
                  MessageBox.Show("ERROR: FAILED TO GET GENSHIN PROCESS");
               }
            }
         }
      }
      private void OfdGenshinexe_FileOk(object sender, CancelEventArgs e)
      {
         PathToExe = ofdGenshinexe.FileName;
         txtPathToExe.Text = PathToExe;
         btnStart.Enabled = true;
         btnStart.Text = "START && INJECT";
         Settings.Default.GenshinFilePath = PathToExe;
         Settings.Default.Save();
      }
      private void Button1_Click(object sender, EventArgs e)
      {
         ofdGenshinexe.ShowDialog();
      }
      private void Button2_Click(object sender, EventArgs e)
      {
         MessageBox.Show("If you need help, just ask someone on Discord!\n\nIf you are not on the Discord Server already, just send TH3C0D3R#4338 a PM with your orderID", "NEED HELP???");
      }
      protected override void WndProc(ref Message m)
      {
         if (m.Msg == 0x0312)
         {
            switch ((HotKeys)m.WParam.ToInt32())
            {
               case HotKeys.Numpad1:
                  PathUnityPlayer(ProcHandle);
                  break;
               default:
                  break;
            }
         }
         base.WndProc(ref m);
      }
      private void PathUnityPlayer(IntPtr procHandle)
      {
         try
         {
            /*
             * 14CDDB0 - C3
             * 14CE400 - C3
             * 1CA75C3 - 31 D2
             */
            IntPtr UnityPlayerBaseAddress = GetModuleBaseAddress(Genshin, "UnityPlayer.dll");
            if (procHandle == IntPtr.Zero || UnityPlayerBaseAddress == IntPtr.Zero) throw new ApplicationException($"Code: procHandle or unitymodule has value 0x{IntPtr.Zero}");
            bool written = WriteProcessMemory(procHandle, IntPtr.Add(UnityPlayerBaseAddress, 0x14CDDB0), StructureToByteArray(0xC3), 1, out int refer);
            if(!written) throw new ApplicationException($"Code: First Patch Failed");
            written = WriteProcessMemory(procHandle, IntPtr.Add(UnityPlayerBaseAddress, 0x14CE400), StructureToByteArray(0xC3), 1, out refer);
            if (!written) throw new ApplicationException($"Code: Second Patch Failed");
            written = WriteProcessMemory(procHandle, IntPtr.Add(UnityPlayerBaseAddress, 0x1CA75C3), StructureToByteArray(0x31), 1, out refer);
            if (!written) throw new ApplicationException($"Code: Third Patch Failed");
            written = WriteProcessMemory(procHandle, IntPtr.Add(UnityPlayerBaseAddress, 0x1CA75C4), StructureToByteArray(0xD2), 1, out refer);
            if (!written) throw new ApplicationException($"Code: Fourth Patch Failed");
            ServiceController sc = new ServiceController("mhyprot2");
            MessageBox.Show("The mhyprot2 service status is currently set to {0}", sc.Status.ToString());
            if (!sc.Status.Equals(ServiceControllerStatus.Stopped))
            {
               sc.Stop();
               sc.Refresh();
               MessageBox.Show("The mhyprot2 service status is now set to {0}", sc.Status.ToString());
               if (sc.Status.Equals(ServiceControllerStatus.Stopped))
               {
                  MessageBox.Show("PATCHED"); 
                  Patched = true;
               }
            }
         }
         catch (ApplicationException ex)
         {
            MessageBox.Show("ERROR:\n" + ex.Message);
         }
      }

      public IntPtr GetModuleBaseAddress(Process process, string moduleName)
      {
         var module = process.Modules.Cast<ProcessModule>().SingleOrDefault(m => string.Equals(m.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase));
         return module?.BaseAddress ?? IntPtr.Zero;
      }
      private static byte[] StructureToByteArray(object obj)
      {
         int len = Marshal.SizeOf(obj);
         byte[] arr = new byte[len];
         IntPtr ptr = Marshal.AllocHGlobal(len);
         Marshal.StructureToPtr(obj, ptr, true);
         Marshal.Copy(ptr, arr, 0, len);
         Marshal.FreeHGlobal(ptr);

         return arr;
      }
   }
}
