namespace CalliAPI_Mailer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            chkShowPrinted = new CheckBox();
            chkShowIgnored = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 136);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(1486, 745);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // chkShowPrinted
            // 
            chkShowPrinted.AutoSize = true;
            chkShowPrinted.Location = new Point(325, 111);
            chkShowPrinted.Name = "chkShowPrinted";
            chkShowPrinted.Size = new Size(139, 19);
            chkShowPrinted.TabIndex = 1;
            chkShowPrinted.Text = "Show Already Printed";
            chkShowPrinted.UseVisualStyleBackColor = true;
            // 
            // chkShowIgnored
            // 
            chkShowIgnored.AutoSize = true;
            chkShowIgnored.Checked = true;
            chkShowIgnored.CheckState = CheckState.Checked;
            chkShowIgnored.Location = new Point(487, 111);
            chkShowIgnored.Name = "chkShowIgnored";
            chkShowIgnored.Size = new Size(99, 19);
            chkShowIgnored.TabIndex = 2;
            chkShowIgnored.Text = "Show Ignored";
            chkShowIgnored.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1834, 918);
            Controls.Add(chkShowIgnored);
            Controls.Add(chkShowPrinted);
            Controls.Add(dataGridView1);
            Name = "Form1";
            Text = "CalliAPI Mailer Program";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private CheckBox chkShowPrinted;
        private CheckBox chkShowIgnored;
    }
}
