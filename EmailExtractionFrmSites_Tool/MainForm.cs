using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmailExtractionFrmSites_Tool
{
    public partial class MainForm : Form
    {
        string filePath = string.Empty;
        private bool isProcessing = false;
        private CancellationTokenSource cts;
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("You cannot browse a new file while the process is running.", "Browse Disabled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx|CSV Files|*.csv";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    txtFilePath.Text = filePath;
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("You cannot reset while the process is running.", "Reset Disabled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            txtFilePath.Clear();
            txtLog.Clear();
            btnStartProc.Text = "Start Process";
            filePath = string.Empty;
            lblTotalSites.Text = "00 / 00";
        }
 
        private void UpdateLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(UpdateLog), message);
            }
            else
            {
                txtLog.AppendText(message + Environment.NewLine);
            }
        }

        private async void btnStartProc_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                var result = MessageBox.Show("A process is already running. Do you want to stop it and save current results?",
                                             "Stop Process?",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    cts?.Cancel(); // trigger cancellation
                }
                return;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Please select a file first.");
                return;
            }

            try
            {

                isProcessing = true;
                cts = new CancellationTokenSource();
                btnStartProc.Text = "Stop Processing...";
                txtLog.Clear();
                //lblTotalSites.Text = "0 / 0";

                await ExtractEmails.ProcessFile(filePath, UpdateLog, UpdateProgressCount, cts.Token);
            }
            catch (OperationCanceledException)
            {
                UpdateLog("⚠️ Process was cancelled by the user.");
            }
            catch (Exception ex)
            {
                UpdateLog("❌ Error: " + ex.Message);
            }

            btnStartProc.Text = "Completed";
            isProcessing = false;
        }

        private void UpdateProgressCount(int done, int total)
        {
            if (lblTotalSites.InvokeRequired)
            {
                lblTotalSites.Invoke(new Action<int, int>(UpdateProgressCount), done, total);
            }
            else
            {
                lblTotalSites.Text = $"{done} / {total}";
            }
        }
    }
}
