namespace EmailExtractionFrmSites_Tool
{
    partial class LeadScrapePro
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
            lblClose = new Label();
            groupBox1 = new GroupBox();
            SuspendLayout();
            // 
            // lblClose
            // 
            lblClose.AutoSize = true;
            lblClose.Cursor = Cursors.Hand;
            lblClose.FlatStyle = FlatStyle.Flat;
            lblClose.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblClose.ForeColor = Color.IndianRed;
            lblClose.Location = new Point(2580, 9);
            lblClose.Name = "lblClose";
            lblClose.Size = new Size(70, 81);
            lblClose.TabIndex = 0;
            lblClose.Text = "X";
            lblClose.Click += lblClose_Click;
            // 
            // groupBox1
            // 
            groupBox1.Location = new Point(12, 9);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(434, 1325);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // LeadScrapePro
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.WindowFrame;
            ClientSize = new Size(2662, 1346);
            Controls.Add(groupBox1);
            Controls.Add(lblClose);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LeadScrapePro";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LeadScrapePro";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblClose;
        private GroupBox groupBox1;
    }
}