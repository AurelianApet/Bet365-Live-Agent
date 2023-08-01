namespace Bet365LiveAgent
{
    partial class FrmMain
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
            this.txtInDataLog = new System.Windows.Forms.TextBox();
            this.txtOutDataLog = new System.Windows.Forms.TextBox();
            this.grpBoxDataLog = new System.Windows.Forms.GroupBox();
            this.lbOutDataLog = new System.Windows.Forms.Label();
            this.lbInDataLog = new System.Windows.Forms.Label();
            this.chkBoxOutDataAutoScrll = new System.Windows.Forms.CheckBox();
            this.chkBoxInDataAutoScrll = new System.Windows.Forms.CheckBox();
            this.grpBoxSetting = new System.Windows.Forms.GroupBox();
            this.txtWebSockServerPort = new System.Windows.Forms.TextBox();
            this.lbWebSockServerPort = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.grpBoxDataLog.SuspendLayout();
            this.grpBoxSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtInDataLog
            // 
            this.txtInDataLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtInDataLog.Location = new System.Drawing.Point(6, 38);
            this.txtInDataLog.Multiline = true;
            this.txtInDataLog.Name = "txtInDataLog";
            this.txtInDataLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInDataLog.Size = new System.Drawing.Size(575, 524);
            this.txtInDataLog.TabIndex = 6;
            // 
            // txtOutDataLog
            // 
            this.txtOutDataLog.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtOutDataLog.Location = new System.Drawing.Point(587, 38);
            this.txtOutDataLog.Multiline = true;
            this.txtOutDataLog.Name = "txtOutDataLog";
            this.txtOutDataLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutDataLog.Size = new System.Drawing.Size(575, 524);
            this.txtOutDataLog.TabIndex = 7;
            // 
            // grpBoxDataLog
            // 
            this.grpBoxDataLog.Controls.Add(this.lbOutDataLog);
            this.grpBoxDataLog.Controls.Add(this.lbInDataLog);
            this.grpBoxDataLog.Controls.Add(this.chkBoxOutDataAutoScrll);
            this.grpBoxDataLog.Controls.Add(this.chkBoxInDataAutoScrll);
            this.grpBoxDataLog.Controls.Add(this.txtInDataLog);
            this.grpBoxDataLog.Controls.Add(this.txtOutDataLog);
            this.grpBoxDataLog.Location = new System.Drawing.Point(12, 103);
            this.grpBoxDataLog.Name = "grpBoxDataLog";
            this.grpBoxDataLog.Size = new System.Drawing.Size(1169, 570);
            this.grpBoxDataLog.TabIndex = 0;
            this.grpBoxDataLog.TabStop = false;
            this.grpBoxDataLog.Text = "Data Log";
            // 
            // lbOutDataLog
            // 
            this.lbOutDataLog.AutoSize = true;
            this.lbOutDataLog.Location = new System.Drawing.Point(587, 20);
            this.lbOutDataLog.Name = "lbOutDataLog";
            this.lbOutDataLog.Size = new System.Drawing.Size(71, 13);
            this.lbOutDataLog.TabIndex = 0;
            this.lbOutDataLog.Text = "Out-Data Log";
            // 
            // lbInDataLog
            // 
            this.lbInDataLog.AutoSize = true;
            this.lbInDataLog.Location = new System.Drawing.Point(7, 20);
            this.lbInDataLog.Name = "lbInDataLog";
            this.lbInDataLog.Size = new System.Drawing.Size(63, 13);
            this.lbInDataLog.TabIndex = 0;
            this.lbInDataLog.Text = "In-Data Log";
            // 
            // chkBoxOutDataAutoScrll
            // 
            this.chkBoxOutDataAutoScrll.Checked = true;
            this.chkBoxOutDataAutoScrll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxOutDataAutoScrll.Location = new System.Drawing.Point(1085, 19);
            this.chkBoxOutDataAutoScrll.Name = "chkBoxOutDataAutoScrll";
            this.chkBoxOutDataAutoScrll.Size = new System.Drawing.Size(77, 17);
            this.chkBoxOutDataAutoScrll.TabIndex = 5;
            this.chkBoxOutDataAutoScrll.Text = "Auto Scroll";
            this.chkBoxOutDataAutoScrll.UseVisualStyleBackColor = true;
            // 
            // chkBoxInDataAutoScrll
            // 
            this.chkBoxInDataAutoScrll.Checked = true;
            this.chkBoxInDataAutoScrll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxInDataAutoScrll.Location = new System.Drawing.Point(504, 19);
            this.chkBoxInDataAutoScrll.Name = "chkBoxInDataAutoScrll";
            this.chkBoxInDataAutoScrll.Size = new System.Drawing.Size(77, 17);
            this.chkBoxInDataAutoScrll.TabIndex = 4;
            this.chkBoxInDataAutoScrll.Text = "Auto Scroll";
            this.chkBoxInDataAutoScrll.UseVisualStyleBackColor = true;
            // 
            // grpBoxSetting
            // 
            this.grpBoxSetting.Controls.Add(this.txtWebSockServerPort);
            this.grpBoxSetting.Controls.Add(this.lbWebSockServerPort);
            this.grpBoxSetting.Location = new System.Drawing.Point(12, 12);
            this.grpBoxSetting.Name = "grpBoxSetting";
            this.grpBoxSetting.Size = new System.Drawing.Size(281, 85);
            this.grpBoxSetting.TabIndex = 0;
            this.grpBoxSetting.TabStop = false;
            this.grpBoxSetting.Text = "Setting";
            // 
            // txtWebSockServerPort
            // 
            this.txtWebSockServerPort.Location = new System.Drawing.Point(154, 37);
            this.txtWebSockServerPort.Name = "txtWebSockServerPort";
            this.txtWebSockServerPort.Size = new System.Drawing.Size(68, 20);
            this.txtWebSockServerPort.TabIndex = 0;
            // 
            // lbWebSockServerPort
            // 
            this.lbWebSockServerPort.AutoSize = true;
            this.lbWebSockServerPort.Location = new System.Drawing.Point(28, 40);
            this.lbWebSockServerPort.Name = "lbWebSockServerPort";
            this.lbWebSockServerPort.Size = new System.Drawing.Size(120, 13);
            this.lbWebSockServerPort.TabIndex = 0;
            this.lbWebSockServerPort.Text = "WebSocket Server Port";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(851, 30);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(147, 56);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(851, 30);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(147, 56);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1027, 30);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(147, 56);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1193, 690);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.grpBoxSetting);
            this.Controls.Add(this.grpBoxDataLog);
            this.Name = "FrmMain";
            this.Text = "Bet365 Live Agent";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.grpBoxDataLog.ResumeLayout(false);
            this.grpBoxDataLog.PerformLayout();
            this.grpBoxSetting.ResumeLayout(false);
            this.grpBoxSetting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtInDataLog;
        private System.Windows.Forms.TextBox txtOutDataLog;
        private System.Windows.Forms.GroupBox grpBoxDataLog;
        private System.Windows.Forms.CheckBox chkBoxInDataAutoScrll;
        private System.Windows.Forms.CheckBox chkBoxOutDataAutoScrll;
        private System.Windows.Forms.GroupBox grpBoxSetting;
        private System.Windows.Forms.Label lbWebSockServerPort;
        private System.Windows.Forms.TextBox txtWebSockServerPort;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lbInDataLog;
        private System.Windows.Forms.Label lbOutDataLog;
    }
}

