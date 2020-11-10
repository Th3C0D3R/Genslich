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
using System.Threading;
using System.Windows.Forms;

namespace Genslich
{
    public partial class frmMain : Form
    {
        private string PathToExe = "";
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
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PathToExe))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                if (ProcessStart.StartProcess(PathToExe, ProcessCreationFlags.ZERO_FLAG))
                {
                    Process Genshin = Process.GetProcesses().ToList().Find((p) => p.ProcessName == "GenshinImpact");
                    if (Genshin != null)
                    {
                        try
                        {
                            Thread.Sleep(1000);
                            IntPtr threadAddress = DLLInjection.Inject(Genshin, "Resources\\HelloWorldDLL.dll");
                            sw.Stop();
                            lblTime.Text = $"{sw.Elapsed.Seconds}:{sw.Elapsed.Milliseconds}";
                            //if (threadAddress != null)
                            //    ProcessExtensions.Resume(Genshin, threadAddress);
                            //else throw new ApplicationException($"Code: {Marshal.GetLastWin32Error()}");
                        }
                        catch (ApplicationException ex)
                        {
                            MessageBox.Show("ERROR:\n" + ex.Message);
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
    }
}
