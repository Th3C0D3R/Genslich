using Genslich.Extensions;
using GenslichCore.Extensions;
using GenslichCore.Properties;
using Lunar;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Genslich
{
    public partial class frmMain : Form
    {
        private string PathToPS1Script = "";
        private string PathToExe = "";
        public frmMain()
        {
            InitializeComponent();
            PathToPS1Script = SavePS1ScriptTmp();
            Settings.Default.Reload();
            if (!string.IsNullOrWhiteSpace(Settings.Default.GenshinFilePath))
            {
                PathToExe = Settings.Default.GenshinFilePath;
                txtPathToExe.Text = PathToExe;
                btnStart.Enabled = true;
                btnStart.Text = "START && INJECT";
            }
        }

        private string SavePS1ScriptTmp()
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = "startGenshin.ps1";
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!resourcePath.StartsWith(nameof(Genslich)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(resourcePath));
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                File.WriteAllText(Path.Combine(Path.GetTempPath(), "genslich.ps1"), reader.ReadToEnd());
                return Path.Combine(Path.GetTempPath(), "genslich.ps1");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PathToExe))
            {
                if (ProcessStart.StartProcess(PathToExe, ProcessCreationFlags.CREATE_SUSPENDED))
                {
                    Process Genshin = Process.GetProcesses().ToList().Find((p) => p.ProcessName == "GenshinImpact");
                    if (Genshin != null)
                    {
                        try
                        {
                            LibraryMapper lm = new LibraryMapper(Genshin, "Resources\\HelloWorldDLL.dll");
                            lm.MapLibrary();
                            ProcessExtensions.Resume(Genshin);
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

        private void ofdGenshinexe_FileOk(object sender, CancelEventArgs e)
        {
            PathToExe = ofdGenshinexe.FileName;
            txtPathToExe.Text = PathToExe;
            btnStart.Enabled = true;
            btnStart.Text = "START && INJECT";
            Settings.Default.GenshinFilePath = PathToExe;
            Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ofdGenshinexe.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("If you need help, just ask someone on Discord!\n\nIf you are not on the Discord Server already, just send TH3C0D3R#4338 a PM with your orderID", "NEEd HELP???");
        }
    }
}
