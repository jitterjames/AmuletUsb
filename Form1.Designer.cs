namespace AmuletJUSB
{
	partial class AmuletForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmuletForm));
            this.lblLinkStatus = new System.Windows.Forms.Label();
            this.lblMicStatus = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.numUdpPort = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.showHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnHide = new System.Windows.Forms.Button();
            this.timerAutoHide = new System.Windows.Forms.Timer(this.components);
            this.labelDelayOn = new System.Windows.Forms.Label();
            this.labelDelayOff = new System.Windows.Forms.Label();
            this.numDelayOn = new System.Windows.Forms.NumericUpDown();
            this.numDelayOff = new System.Windows.Forms.NumericUpDown();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtTargetIP = new System.Windows.Forms.TextBox();
            this.radTargetIP = new System.Windows.Forms.RadioButton();
            this.radBroadcast = new System.Windows.Forms.RadioButton();
            this.btnRestart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numUdpPort)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayOff)).BeginInit();
            this.groupBoxStatus.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLinkStatus
            // 
            this.lblLinkStatus.AutoSize = true;
            this.lblLinkStatus.Location = new System.Drawing.Point(12, 24);
            this.lblLinkStatus.Name = "lblLinkStatus";
            this.lblLinkStatus.Size = new System.Drawing.Size(63, 13);
            this.lblLinkStatus.TabIndex = 1;
            this.lblLinkStatus.Text = "Link Status:";
            // 
            // lblMicStatus
            // 
            this.lblMicStatus.AutoSize = true;
            this.lblMicStatus.Location = new System.Drawing.Point(12, 47);
            this.lblMicStatus.Name = "lblMicStatus";
            this.lblMicStatus.Size = new System.Drawing.Size(60, 13);
            this.lblMicStatus.TabIndex = 3;
            this.lblMicStatus.Text = "Mic Status:";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(182, 244);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(79, 32);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // numUdpPort
            // 
            this.numUdpPort.Location = new System.Drawing.Point(150, 25);
            this.numUdpPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numUdpPort.Name = "numUdpPort";
            this.numUdpPort.Size = new System.Drawing.Size(86, 20);
            this.numUdpPort.TabIndex = 8;
            this.numUdpPort.Value = new decimal(new int[] {
            33000,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "UDP Send Port:";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "loading";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.showHideToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(134, 48);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
            this.toolStripMenuItem1.Text = "Close";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // showHideToolStripMenuItem
            // 
            this.showHideToolStripMenuItem.Name = "showHideToolStripMenuItem";
            this.showHideToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.showHideToolStripMenuItem.Text = "Show/Hide";
            this.showHideToolStripMenuItem.Click += new System.EventHandler(this.showHideToolStripMenuItem_Click);
            // 
            // btnHide
            // 
            this.btnHide.Location = new System.Drawing.Point(8, 244);
            this.btnHide.Name = "btnHide";
            this.btnHide.Size = new System.Drawing.Size(79, 32);
            this.btnHide.TabIndex = 9;
            this.btnHide.Text = "Hide";
            this.btnHide.UseVisualStyleBackColor = true;
            this.btnHide.Click += new System.EventHandler(this.button1_Click);
            // 
            // timerAutoHide
            // 
            this.timerAutoHide.Enabled = true;
            this.timerAutoHide.Interval = 500;
            this.timerAutoHide.Tick += new System.EventHandler(this.timerAutoHide_Tick);
            // 
            // labelDelayOn
            // 
            this.labelDelayOn.AutoSize = true;
            this.labelDelayOn.Location = new System.Drawing.Point(12, 100);
            this.labelDelayOn.Name = "labelDelayOn";
            this.labelDelayOn.Size = new System.Drawing.Size(73, 13);
            this.labelDelayOn.TabIndex = 10;
            this.labelDelayOn.Text = "Delay On (ms)";
            // 
            // labelDelayOff
            // 
            this.labelDelayOff.AutoSize = true;
            this.labelDelayOff.Location = new System.Drawing.Point(12, 124);
            this.labelDelayOff.Name = "labelDelayOff";
            this.labelDelayOff.Size = new System.Drawing.Size(73, 13);
            this.labelDelayOff.TabIndex = 10;
            this.labelDelayOff.Text = "Delay Off (ms)";
            // 
            // numDelayOn
            // 
            this.numDelayOn.Location = new System.Drawing.Point(168, 98);
            this.numDelayOn.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDelayOn.Name = "numDelayOn";
            this.numDelayOn.Size = new System.Drawing.Size(68, 20);
            this.numDelayOn.TabIndex = 11;
            // 
            // numDelayOff
            // 
            this.numDelayOff.Location = new System.Drawing.Point(168, 122);
            this.numDelayOff.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numDelayOff.Name = "numDelayOff";
            this.numDelayOff.Size = new System.Drawing.Size(68, 20);
            this.numDelayOff.TabIndex = 11;
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Controls.Add(this.lblLinkStatus);
            this.groupBoxStatus.Controls.Add(this.lblMicStatus);
            this.groupBoxStatus.Location = new System.Drawing.Point(8, 11);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(254, 70);
            this.groupBoxStatus.TabIndex = 12;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Amulet USB Monitor";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtTargetIP);
            this.groupBox2.Controls.Add(this.radTargetIP);
            this.groupBox2.Controls.Add(this.radBroadcast);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.numUdpPort);
            this.groupBox2.Controls.Add(this.labelDelayOn);
            this.groupBox2.Controls.Add(this.numDelayOff);
            this.groupBox2.Controls.Add(this.labelDelayOff);
            this.groupBox2.Controls.Add(this.numDelayOn);
            this.groupBox2.Location = new System.Drawing.Point(8, 87);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(254, 152);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // txtTargetIP
            // 
            this.txtTargetIP.Enabled = false;
            this.txtTargetIP.Location = new System.Drawing.Point(121, 69);
            this.txtTargetIP.Name = "txtTargetIP";
            this.txtTargetIP.Size = new System.Drawing.Size(115, 20);
            this.txtTargetIP.TabIndex = 13;
            this.txtTargetIP.Text = "127.0.0.1";
            // 
            // radTargetIP
            // 
            this.radTargetIP.AutoSize = true;
            this.radTargetIP.Location = new System.Drawing.Point(15, 69);
            this.radTargetIP.Name = "radTargetIP";
            this.radTargetIP.Size = new System.Drawing.Size(72, 17);
            this.radTargetIP.TabIndex = 12;
            this.radTargetIP.Text = "Target IP:";
            this.radTargetIP.UseVisualStyleBackColor = true;
            this.radTargetIP.CheckedChanged += new System.EventHandler(this.radTargetIP_CheckedChanged);
            // 
            // radBroadcast
            // 
            this.radBroadcast.AutoSize = true;
            this.radBroadcast.Checked = true;
            this.radBroadcast.Location = new System.Drawing.Point(15, 49);
            this.radBroadcast.Name = "radBroadcast";
            this.radBroadcast.Size = new System.Drawing.Size(73, 17);
            this.radBroadcast.TabIndex = 12;
            this.radBroadcast.TabStop = true;
            this.radBroadcast.Text = "Broadcast";
            this.radBroadcast.UseVisualStyleBackColor = true;
            // 
            // btnRestart
            // 
            this.btnRestart.Location = new System.Drawing.Point(95, 244);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(79, 32);
            this.btnRestart.TabIndex = 13;
            this.btnRestart.Text = "Restart";
            this.btnRestart.UseVisualStyleBackColor = true;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // AmuletForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 282);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBoxStatus);
            this.Controls.Add(this.btnHide);
            this.Controls.Add(this.btnExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(230, 242);
            this.Name = "AmuletForm";
            this.ShowInTaskbar = false;
            this.Text = "Amulet USB Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AmuletForm_FormClosing);
            this.Load += new System.EventHandler(this.AmuletForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numUdpPort)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numDelayOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelayOff)).EndInit();
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxStatus.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.Label lblLinkStatus;
        private System.Windows.Forms.Label lblMicStatus;
		private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.NumericUpDown numUdpPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button btnHide;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showHideToolStripMenuItem;
        private System.Windows.Forms.Timer timerAutoHide;
        private System.Windows.Forms.Label labelDelayOn;
        private System.Windows.Forms.Label labelDelayOff;
        private System.Windows.Forms.NumericUpDown numDelayOn;
        private System.Windows.Forms.NumericUpDown numDelayOff;
        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtTargetIP;
        private System.Windows.Forms.RadioButton radTargetIP;
        private System.Windows.Forms.RadioButton radBroadcast;
        private System.Windows.Forms.Button btnRestart;
	}
}

