using System.Text.Json;
using System.Text.RegularExpressions;

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
            btnEmail.Text = "Send Email";
            txtEmailLimit.Text = "Send email limit";
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

            btnStartProc.Text = "Process Completed";
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

        //private void MainForm_Load(object sender, EventArgs e)
        //{
        //    if (TrialHelper.IsTrialExpiredOrDeviceMismatch())
        //    {
        //        MessageBox.Show("❌ Trial expired or not valid for this system.\n\n Contact with Developer at faizmuhammadmarri@gmail.com", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //        this.Close();
        //        return;
        //    }

        //}

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (TrialHelper.IsTrialExpiredOrDeviceMismatch())
            {
                var result = MessageBox.Show(
                    "❌ Trial expired or not valid for this system.\n\nWould you like to enter a license key?",
                    "Access Denied",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    // Show the LicenseForm and pass MainForm reference if needed
                    // licenseForm = new LicenseForm(this); // pass this if needed
                    LicenseForm licenseForm = new LicenseForm();
                    licenseForm.ShowDialog();

                    // Re-check after license form is closed
                    if (TrialHelper.IsTrialExpiredOrDeviceMismatch())
                    {
                        MessageBox.Show("❌ License is still invalid or not updated. Closing application. \n\n Contact with Developer at faizmuhammadmarri@gmail.com", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }

                return;
            }
        }

        private async void btnEmail_Click(object sender, EventArgs e)
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
                //btnEmail.Enabled = false;
                isProcessing = true;
                string input = txtEmailLimit.Text;
                int value = 0;
                // Extract only digits
                string numbersOnly = Regex.Replace(input, @"\D", "");

                if (!string.IsNullOrEmpty(numbersOnly))
                {
                    value = int.Parse(numbersOnly);

                }
                cts = new CancellationTokenSource();
                btnEmail.Text = "Stop Email Processing...";
                //string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
                EmailConfig config = LoadEmailConfig(configPath);

                await EmailService.SendEmail(UpdateLog, UpdateProgressCount, value, cts.Token, filePath, configPath, config.Email, config.Password, config.Subject, config.Body, config.Port, config.EnableSsl, config.SmtpClient);
                //  EmailService.SendEmail(UpdateLog, UpdateProgressCount, filePath, senderEmail, senderPassword, config.Subject, config.Body);
            }
            catch (OperationCanceledException)
            {
                UpdateLog("⚠️ Process was cancelled by the user.");
            }
            catch (Exception ex)
            {
                UpdateLog("❌ Error: " + ex.Message);
            }

            btnEmail.Text = "Process Completed";
            // btnEmail.Enabled = true;
            isProcessing = false;
        }

        public static EmailConfig LoadEmailConfig(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Email configuration file not found.", filePath);

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<EmailConfig>(json);
        }

        private void txtEmailLimit_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
