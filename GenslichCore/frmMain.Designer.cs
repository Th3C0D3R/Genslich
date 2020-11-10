namespace Genslich
{
    partial class frmMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
         System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "F2", System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "NoClip", System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point))}, -1);
         System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "F3", System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "E-Skill NoCooldown", System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point))}, -1);
         System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "F4",
            "Save Current Coords"}, -1, System.Drawing.Color.White, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point));
         System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "F5", System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "Teleport", System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point))}, -1);
         System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new System.Windows.Forms.ListViewItem.ListViewSubItem[] {
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "F6", System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)),
            new System.Windows.Forms.ListViewItem.ListViewSubItem(null, "Toggle FPS (30/200)", System.Drawing.Color.White, System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64))))), new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point))}, -1);
         System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "F7",
            "ESP (Enemy,Box)"}, -1, System.Drawing.Color.White, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point));
         System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "F9",
            "Exit"}, -1, System.Drawing.Color.White, System.Drawing.Color.Empty, new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point));
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
         this.btnStart = new System.Windows.Forms.Button();
         this.ofdGenshinexe = new System.Windows.Forms.OpenFileDialog();
         this.txtPathToExe = new System.Windows.Forms.TextBox();
         this.button1 = new System.Windows.Forms.Button();
         this.listView1 = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
         this.button2 = new System.Windows.Forms.Button();
         this.ftbcBar = new TitleBarControl.FormTitleBarControl();
         this.formControlBox1 = new window_control_box.FormControlBox();
         this.SuspendLayout();
         // 
         // btnStart
         // 
         this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnStart.Enabled = false;
         this.btnStart.Location = new System.Drawing.Point(221, 249);
         this.btnStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.btnStart.Name = "btnStart";
         this.btnStart.Size = new System.Drawing.Size(174, 30);
         this.btnStart.TabIndex = 0;
         this.btnStart.Text = "SELECT PATH FIRST";
         this.btnStart.UseVisualStyleBackColor = true;
         this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);
         // 
         // ofdGenshinexe
         // 
         this.ofdGenshinexe.DefaultExt = "exe";
         this.ofdGenshinexe.FileName = "GenshinImpact.exe";
         this.ofdGenshinexe.Filter = "GenshinImpact.exe|GenshinImpact.exe";
         this.ofdGenshinexe.InitialDirectory = "C:\\Program Files\\Genshin Impact\\Genshin Impact Game";
         this.ofdGenshinexe.FileOk += new System.ComponentModel.CancelEventHandler(this.OfdGenshinexe_FileOk);
         // 
         // txtPathToExe
         // 
         this.txtPathToExe.Location = new System.Drawing.Point(10, 49);
         this.txtPathToExe.Name = "txtPathToExe";
         this.txtPathToExe.ReadOnly = true;
         this.txtPathToExe.Size = new System.Drawing.Size(309, 23);
         this.txtPathToExe.TabIndex = 2;
         this.txtPathToExe.Text = "PATH TO GENSHIN IMPACT EXE";
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(325, 44);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(37, 31);
         this.button1.TabIndex = 3;
         this.button1.Text = "...";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.Button1_Click);
         // 
         // listView1
         // 
         this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
         this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
         this.listView1.HideSelection = false;
         listViewItem5.StateImageIndex = 0;
         this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7});
         this.listView1.Location = new System.Drawing.Point(10, 75);
         this.listView1.Name = "listView1";
         this.listView1.Size = new System.Drawing.Size(386, 170);
         this.listView1.TabIndex = 4;
         this.listView1.UseCompatibleStateImageBehavior = false;
         this.listView1.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "Hotkey";
         this.columnHeader1.Width = 75;
         // 
         // columnHeader2
         // 
         this.columnHeader2.Text = "Description";
         this.columnHeader2.Width = 210;
         // 
         // button2
         // 
         this.button2.Location = new System.Drawing.Point(10, 255);
         this.button2.Name = "button2";
         this.button2.Size = new System.Drawing.Size(20, 22);
         this.button2.TabIndex = 5;
         this.button2.Text = "?";
         this.button2.UseVisualStyleBackColor = true;
         this.button2.Click += new System.EventHandler(this.Button2_Click);
         // 
         // ftbcBar
         // 
         this.ftbcBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ftbcBar.BackgroundImage")));
         this.ftbcBar.Dock = System.Windows.Forms.DockStyle.Top;
         this.ftbcBar.Location = new System.Drawing.Point(0, 0);
         this.ftbcBar.Margin = new System.Windows.Forms.Padding(4);
         this.ftbcBar.Name = "ftbcBar";
         this.ftbcBar.Size = new System.Drawing.Size(411, 23);
         this.ftbcBar.TabIndex = 1;
         this.ftbcBar.Title = "Genslich Impact";
         this.ftbcBar.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
         this.ftbcBar.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
         this.ftbcBar.TitleForeColor = System.Drawing.Color.White;
         // 
         // formControlBox1
         // 
         this.formControlBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("formControlBox1.BackgroundImage")));
         this.formControlBox1.Close = true;
         this.formControlBox1.Location = new System.Drawing.Point(332, -2);
         this.formControlBox1.Margin = new System.Windows.Forms.Padding(4);
         this.formControlBox1.Maximize = false;
         this.formControlBox1.Minimize = true;
         this.formControlBox1.Name = "formControlBox1";
         this.formControlBox1.Size = new System.Drawing.Size(79, 25);
         this.formControlBox1.TabIndex = 6;
         // 
         // frmMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
         this.ClientSize = new System.Drawing.Size(411, 290);
         this.ControlBox = false;
         this.Controls.Add(this.formControlBox1);
         this.Controls.Add(this.button2);
         this.Controls.Add(this.listView1);
         this.Controls.Add(this.button1);
         this.Controls.Add(this.txtPathToExe);
         this.Controls.Add(this.ftbcBar);
         this.Controls.Add(this.btnStart);
         this.DoubleBuffered = true;
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
         this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.MaximizeBox = false;
         this.Name = "frmMain";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
         this.Text = "Genslich";
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.OpenFileDialog ofdGenshinexe;
        private TitleBarControl.FormTitleBarControl ftbcBar;
        private System.Windows.Forms.TextBox txtPathToExe;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button button2;
        private window_control_box.FormControlBox formControlBox1;
    }
}

