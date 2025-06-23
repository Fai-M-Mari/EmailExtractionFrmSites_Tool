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
    public partial class LicenseForm : Form
    {
        private Form mainForm;

        public LicenseForm(Form adminForm, Form mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string inputKey = txtProductKey.Text?.Trim();

            if (string.IsNullOrWhiteSpace(inputKey))
            {
                MessageBox.Show("⚠️ Please enter a license key.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(inputKey));

                if (!DateTime.TryParseExact(decoded, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime newExpiry))
                {
                    MessageBox.Show("❌ Invalid date format in license key. Expected format: dd/MM/yyyy", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (newExpiry <= DateTime.Now)
                {
                    MessageBox.Show("❌ License date must be a future date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (TrialHelper.UpdateExpiryWithKey(inputKey))
                {
                    MessageBox.Show("✅ License updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // closes LicenseForm
                    mainForm?.Show();   // shows MainForm again
                }
                else
                {
                    MessageBox.Show("❌ Failed to update license. Key may be invalid or not for this device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("❌ License key is not a valid base64 string.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public LicenseForm()
        {
            InitializeComponent();
        }

        

    }
}
