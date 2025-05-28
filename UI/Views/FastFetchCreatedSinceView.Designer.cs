namespace CalliAPI.UI.Views
{
    partial class FastFetchCreatedSinceView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            richTextBox1 = new RichTextBox();
            label1 = new Label();
            btnLaunch = new Button();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.DetectUrls = false;
            richTextBox1.Location = new Point(14, 56);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(519, 140);
            richTextBox1.TabIndex = 14;
            richTextBox1.Text = "Obtain all of the matters in Clio that have been created since a certain date.\n\nFastFetch is limited to 10,000 records.";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(14, 9);
            label1.Name = "label1";
            label1.Size = new Size(302, 32);
            label1.TabIndex = 13;
            label1.Text = "All Matters Created Since";
            // 
            // btnLaunch
            // 
            btnLaunch.Location = new Point(167, 202);
            btnLaunch.Name = "btnLaunch";
            btnLaunch.Size = new Size(220, 23);
            btnLaunch.TabIndex = 12;
            btnLaunch.Text = "Launch FastFetch Created Since Report";
            btnLaunch.UseVisualStyleBackColor = true;
            btnLaunch.Click += btnLaunch_Click;
            // 
            // FastFetchCreatedSinceView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(richTextBox1);
            Controls.Add(label1);
            Controls.Add(btnLaunch);
            Name = "FastFetchCreatedSinceView";
            Size = new Size(554, 248);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBox1;
        private Label label1;
        private Button btnLaunch;
    }
}
