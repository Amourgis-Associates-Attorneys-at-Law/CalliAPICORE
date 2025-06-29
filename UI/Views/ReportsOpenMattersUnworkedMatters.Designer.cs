﻿namespace CalliAPI.UI.Views
{
    partial class ReportsOpenMattersUnworkedMatters
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
            components = new System.ComponentModel.Container();
            btnLaunch = new Button();
            label1 = new Label();
            richTextBox1 = new RichTextBox();
            toolTip1 = new ToolTip(components);
            SuspendLayout();
            // 
            // btnLaunch
            // 
            btnLaunch.Location = new Point(175, 211);
            btnLaunch.Name = "btnLaunch";
            btnLaunch.Size = new Size(220, 23);
            btnLaunch.TabIndex = 0;
            btnLaunch.Text = "Launch Unworked Matters Report";
            btnLaunch.UseVisualStyleBackColor = true;
            btnLaunch.Click += BtnLaunch_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(22, 18);
            label1.Name = "label1";
            label1.Size = new Size(294, 32);
            label1.TabIndex = 9;
            label1.Text = "Open Unworked Matters";
            // 
            // richTextBox1
            // 
            richTextBox1.DetectUrls = false;
            richTextBox1.Location = new Point(22, 65);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(519, 140);
            richTextBox1.TabIndex = 11;
            richTextBox1.Text = "Obtain all of the open 713 matters in Clio that are in a prefile stage and unworked (so no tasks are marked as not complete).";
            // 
            // ReportsOpenMattersUnworkedMatters
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(richTextBox1);
            Controls.Add(label1);
            Controls.Add(btnLaunch);
            Name = "ReportsOpenMattersUnworkedMatters";
            Size = new Size(544, 237);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLaunch;
        private Label label1;
        private RichTextBox richTextBox1;
        private ToolTip toolTip1;
    }
}
