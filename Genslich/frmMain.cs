using Genslich.Extensions;
using Genslich.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using TH3_1NJ3CT0R;

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
            dynamic ProcessHandler = null;
            dynamic ProcessID = null;
            using (PowerShell PowerShellInst = PowerShell.Create())
            {
                string path = PathToPS1Script;
                if (!string.IsNullOrEmpty(path))
                {
                    string script = File.ReadAllText(path).Replace("###File###", PathToExe).Replace("###Path###", PathToExe.Replace("GenshinImpact.exe", ""));
                    PowerShellInst.AddScript(script);
                }

                Collection<PSObject> PSOutput = PowerShellInst.Invoke();

                ProcessHandler = PSOutput[2].Properties["hProcess"];
                ProcessID = PSOutput[2].Properties["dwProcessID"];

            }

            if (ProcessHandler != null)
            {
                DllInjectionResult result = D11_1NJ3CT0R.D11_1NJ3CT3("GenshinImpact", "Resources\\HelloWorldDLL.dll");
                if (result == DllInjectionResult.Success)
                {
                    Process p = Process.GetProcessById(ProcessID);
                    if (p.Id == ProcessID)
                        ProcessExtensions.Resume(p);
                }
                else
                {
                    MessageBox.Show("ERROR INJECTING: " + result.ToString());
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
