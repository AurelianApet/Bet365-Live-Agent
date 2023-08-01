using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Bet365LiveAgent.Logics;

namespace Bet365LiveAgent
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            Global.WriteLog = OnWriteLog;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Config.Instance.LoadConfig();
            txtWebSockServerPort.Text = Config.Instance.WebSockServerPort.ToString();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Config.Instance.WebSockServerPort = Convert.ToUInt16(txtWebSockServerPort.Text);
            Config.Instance.SaveConfig();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Global.LogFileName = $"log_{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
            RefreshControls(true);
            Bet365ClientManager.Intance.Start();
            Bet365AgentManager.Intance.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            RefreshControls(false);
            Bet365ClientManager.Intance.Stop();
            Bet365AgentManager.Intance.Stop();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Bet365ClientManager.Intance.Stop();
            this.Close();
        }

        private void RefreshControls(bool state)
        {
            btnStart.Enabled = !state;
            btnStart.Visible = !state;
            btnStop.Enabled = state;
            btnStop.Visible = state;

            Config.Instance.WebSockServerPort = Convert.ToUInt16(txtWebSockServerPort.Text);
        }

        private void OnWriteLog(LOGLEVEL logLevel, LOGTYPE logType, string strLog)
        {
            this.Invoke(new Action(() =>
            {
                string strLogPrefix = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]";
                if (logLevel == LOGLEVEL.FILE || logLevel == LOGLEVEL.FULL)
                {
                    if (logType == LOGTYPE.INDATA)
                        strLogPrefix = $"{strLogPrefix} [In-Data]";
                    else if(logType == LOGTYPE.OUTDATA)
                        strLogPrefix = $"{strLogPrefix} [Out-Data]";
                    WriteLogToFile($"{strLogPrefix} {strLog}");
                }
                if (logLevel == LOGLEVEL.NOTICE || logLevel == LOGLEVEL.FULL)
                {
                    if (logType == LOGTYPE.INDATA)
                    {
                        if (txtInDataLog.TextLength > 3 * 1024 * 1024)
                            txtInDataLog.Text = string.Empty;
                        int selectionStart = txtInDataLog.SelectionStart;
                        int selectionLength = txtInDataLog.SelectionLength;
                        txtInDataLog.AppendText($"{strLogPrefix} {strLog}{Environment.NewLine}");
                        if (!chkBoxInDataAutoScrll.Checked)
                        {
                            txtInDataLog.SelectionStart = selectionStart;
                            txtInDataLog.SelectionLength = selectionLength;
                            txtInDataLog.ScrollToCaret();
                        }
                    }
                    else if (logType == LOGTYPE.OUTDATA)
                    {
                        if (txtOutDataLog.Lines.Length > 10 * 1024 * 1024)
                            txtOutDataLog.Text = string.Empty;
                        int selectionStart = txtOutDataLog.SelectionStart;
                        int selectionLength = txtOutDataLog.SelectionLength;
                        txtOutDataLog.AppendText($"{strLogPrefix} {strLog}{Environment.NewLine}");
                        if (!chkBoxOutDataAutoScrll.Checked)
                        {
                            txtOutDataLog.SelectionStart = selectionStart;
                            txtOutDataLog.SelectionLength = selectionLength;
                            txtOutDataLog.ScrollToCaret();
                        }                            
                    }
                }
            }));
        }

        private void WriteLogToFile(string strLog)
        {
            try
            {
                Directory.CreateDirectory(Global.LogFilePath);
                FileStream fileStream = File.Open($"{Global.LogFilePath}{Global.LogFileName}", FileMode.Append, FileAccess.Write, FileShare.Read);
                if (fileStream.Length > 3 * 1024 * 1024)
                {                    
                    Global.LogFileName = $"log_{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
                    fileStream.Close();
                    fileStream = File.Open($"{Global.LogFilePath}{Global.LogFileName}", FileMode.Append, FileAccess.Write, FileShare.Read);
                }
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                if (!string.IsNullOrEmpty(strLog))
                    streamWriter.WriteLine(strLog);
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in WriteLogToFile() : {ex}");
            }
        }
    }
}
