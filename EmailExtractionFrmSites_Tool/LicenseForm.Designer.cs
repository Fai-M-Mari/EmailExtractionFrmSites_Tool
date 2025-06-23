namespace EmailExtractionFrmSites_Tool
{
    partial class LicenseForm
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
            btnSubmit = new Button();
            txtProductKey = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // btnSubmit
            // 
            btnSubmit.Location = new Point(993, 340);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(188, 58);
            btnSubmit.TabIndex = 0;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // txtProductKey
            // 
            txtProductKey.Location = new Point(311, 346);
            txtProductKey.Name = "txtProductKey";
            txtProductKey.Size = new Size(655, 47);
            txtProductKey.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Blue;
            label1.Location = new Point(165, 209);
            label1.Margin = new Padding(7, 0, 7, 0);
            label1.Name = "label1";
            label1.Size = new Size(1066, 91);
            label1.TabIndex = 2;
            label1.Text = "Email Extraction Tool From Sites";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(192, 352);
            label2.Name = "label2";
            label2.Size = new Size(66, 41);
            label2.TabIndex = 3;
            label2.Text = "Key";
            // 
            // LicenseForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1421, 698);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtProductKey);
            Controls.Add(btnSubmit);
            Name = "LicenseForm";
            Text = "LicenseForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSubmit;
        private TextBox txtProductKey;
        private Label label1;
        private Label label2;
    }
}