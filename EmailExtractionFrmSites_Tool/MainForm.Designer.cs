namespace EmailExtractionFrmSites_Tool
{
    partial class MainForm
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
            label1 = new Label();
            openFileDialog1 = new OpenFileDialog();
            btnBrowser = new Button();
            txtFilePath = new TextBox();
            label2 = new Label();
            btnStartProc = new Button();
            txtLog = new TextBox();
            btnReset = new Button();
            label3 = new Label();
            label4 = new Label();
            lblTotalSites = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Blue;
            label1.Location = new Point(333, 25);
            label1.Margin = new Padding(7, 0, 7, 0);
            label1.Name = "label1";
            label1.Size = new Size(1066, 91);
            label1.TabIndex = 0;
            label1.Text = "Email Extraction Tool From Sites";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnBrowser
            // 
            btnBrowser.Cursor = Cursors.Hand;
            btnBrowser.FlatStyle = FlatStyle.Flat;
            btnBrowser.Location = new Point(1161, 167);
            btnBrowser.Margin = new Padding(7, 8, 7, 8);
            btnBrowser.Name = "btnBrowser";
            btnBrowser.Size = new Size(444, 87);
            btnBrowser.TabIndex = 1;
            btnBrowser.Text = "Browser";
            btnBrowser.UseVisualStyleBackColor = true;
            btnBrowser.Click += btnBrowser_Click;
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new Point(289, 183);
            txtFilePath.Margin = new Padding(7, 8, 7, 8);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new Size(827, 47);
            txtFilePath.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(121, 191);
            label2.Margin = new Padding(7, 0, 7, 0);
            label2.Name = "label2";
            label2.Size = new Size(157, 41);
            label2.TabIndex = 3;
            label2.Text = "File Name:";
            // 
            // btnStartProc
            // 
            btnStartProc.Cursor = Cursors.Hand;
            btnStartProc.FlatStyle = FlatStyle.Flat;
            btnStartProc.Location = new Point(121, 262);
            btnStartProc.Margin = new Padding(7, 8, 7, 8);
            btnStartProc.Name = "btnStartProc";
            btnStartProc.Size = new Size(1001, 109);
            btnStartProc.TabIndex = 4;
            btnStartProc.Text = "Start Process";
            btnStartProc.UseVisualStyleBackColor = true;
            btnStartProc.Click += btnStartProc_Click;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(121, 388);
            txtLog.Margin = new Padding(7, 8, 7, 8);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.PlaceholderText = "Script Logs";
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(1478, 532);
            txtLog.TabIndex = 5;
            // 
            // btnReset
            // 
            btnReset.Cursor = Cursors.Hand;
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Location = new Point(1161, 271);
            btnReset.Margin = new Padding(7, 8, 7, 8);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(444, 101);
            btnReset.TabIndex = 4;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Blue;
            label3.Font = new Font("Calibri", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(1400, 972);
            label3.Margin = new Padding(7, 0, 7, 0);
            label3.Name = "label3";
            label3.Size = new Size(320, 49);
            label3.TabIndex = 6;
            label3.Text = "Sindhi Developers";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(126, 938);
            label4.Margin = new Padding(7, 0, 7, 0);
            label4.Name = "label4";
            label4.Size = new Size(151, 41);
            label4.TabIndex = 7;
            label4.Text = "Total Sites";
            // 
            // lblTotalSites
            // 
            lblTotalSites.AutoSize = true;
            lblTotalSites.Location = new Point(287, 938);
            lblTotalSites.Margin = new Padding(7, 0, 7, 0);
            lblTotalSites.Name = "lblTotalSites";
            lblTotalSites.Size = new Size(118, 41);
            lblTotalSites.TabIndex = 7;
            lblTotalSites.Text = "00  / 00";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1736, 1030);
            Controls.Add(lblTotalSites);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtLog);
            Controls.Add(btnReset);
            Controls.Add(btnStartProc);
            Controls.Add(label2);
            Controls.Add(txtFilePath);
            Controls.Add(btnBrowser);
            Controls.Add(label1);
            Margin = new Padding(7, 8, 7, 8);
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private OpenFileDialog openFileDialog1;
        private Button btnBrowser;
        private TextBox txtFilePath;
        private Label label2;
        private Button btnStartProc;
        private TextBox txtLog;
        private Button btnReset;
        private Label label3;
        private Label label4;
        private Label lblTotalSites;
    }
}