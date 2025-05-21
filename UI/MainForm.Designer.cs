namespace CalliAPI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnOpenMailerForm = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            reportsToolStripMenuItem = new ToolStripMenuItem();
            unworkedMattersToolStripMenuItem = new ToolStripMenuItem();
            allMattersToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            fastFetchToolStripMenuItem1 = new ToolStripMenuItem();
            createdSinceToolStripMenuItem = new ToolStripMenuItem();
            programToolStripMenuItem = new ToolStripMenuItem();
            mailerProgramToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lblApiStatus = new ToolStripStatusLabel();
            lblClioAPIStatus = new ToolStripStatusLabel();
            toolStripSplitButton1 = new ToolStripSplitButton();
            connectToClioToolStripMenuItem = new ToolStripMenuItem();
            progressBarPagesRetrieved = new ProgressBar();
            lblReportPageRetrieved = new Label();
            lblReportName = new Label();
            label1 = new Label();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnOpenMailerForm
            // 
            btnOpenMailerForm.Enabled = false;
            btnOpenMailerForm.Location = new Point(1062, 566);
            btnOpenMailerForm.Name = "btnOpenMailerForm";
            btnOpenMailerForm.Size = new Size(226, 66);
            btnOpenMailerForm.TabIndex = 0;
            btnOpenMailerForm.Text = "Open Mailer";
            btnOpenMailerForm.UseVisualStyleBackColor = true;
            btnOpenMailerForm.Click += btnOpenMailerForm_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, reportsToolStripMenuItem, fastFetchToolStripMenuItem1, programToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1343, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(92, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // reportsToolStripMenuItem
            // 
            reportsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { unworkedMattersToolStripMenuItem, allMattersToolStripMenuItem, toolStripSeparator1 });
            reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            reportsToolStripMenuItem.Size = new Size(59, 20);
            reportsToolStripMenuItem.Text = "Reports";
            // 
            // unworkedMattersToolStripMenuItem
            // 
            unworkedMattersToolStripMenuItem.Enabled = false;
            unworkedMattersToolStripMenuItem.Name = "unworkedMattersToolStripMenuItem";
            unworkedMattersToolStripMenuItem.Size = new Size(171, 22);
            unworkedMattersToolStripMenuItem.Text = "Unworked Matters";
            unworkedMattersToolStripMenuItem.Click += unworkedMattersToolStripMenuItem_Click;
            // 
            // allMattersToolStripMenuItem
            // 
            allMattersToolStripMenuItem.Name = "allMattersToolStripMenuItem";
            allMattersToolStripMenuItem.Size = new Size(171, 22);
            allMattersToolStripMenuItem.Text = "All Matters";
            allMattersToolStripMenuItem.Click += allMattersToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(168, 6);
            // 
            // fastFetchToolStripMenuItem1
            // 
            fastFetchToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { createdSinceToolStripMenuItem });
            fastFetchToolStripMenuItem1.Name = "fastFetchToolStripMenuItem1";
            fastFetchToolStripMenuItem1.Size = new Size(69, 20);
            fastFetchToolStripMenuItem1.Text = "FastFetch";
            // 
            // createdSinceToolStripMenuItem
            // 
            createdSinceToolStripMenuItem.Name = "createdSinceToolStripMenuItem";
            createdSinceToolStripMenuItem.Size = new Size(146, 22);
            createdSinceToolStripMenuItem.Text = "Created Since";
            createdSinceToolStripMenuItem.Click += createdSinceToolStripMenuItem_Click;
            // 
            // programToolStripMenuItem
            // 
            programToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mailerProgramToolStripMenuItem });
            programToolStripMenuItem.Name = "programToolStripMenuItem";
            programToolStripMenuItem.Size = new Size(65, 20);
            programToolStripMenuItem.Text = "Program";
            // 
            // mailerProgramToolStripMenuItem
            // 
            mailerProgramToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem });
            mailerProgramToolStripMenuItem.Name = "mailerProgramToolStripMenuItem";
            mailerProgramToolStripMenuItem.Size = new Size(180, 22);
            mailerProgramToolStripMenuItem.Text = "Mailer Program";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Enabled = false;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(180, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblApiStatus, lblClioAPIStatus, toolStripSplitButton1 });
            statusStrip1.Location = new Point(0, 640);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1343, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblApiStatus
            // 
            lblApiStatus.Name = "lblApiStatus";
            lblApiStatus.Size = new Size(87, 17);
            lblApiStatus.Text = "Clio API Status:";
            // 
            // lblClioAPIStatus
            // 
            lblClioAPIStatus.Name = "lblClioAPIStatus";
            lblClioAPIStatus.Size = new Size(80, 17);
            lblClioAPIStatus.Text = "Not Initialized";
            lblClioAPIStatus.Click += lblClioAPIStatus_Click;
            // 
            // toolStripSplitButton1
            // 
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[] { connectToClioToolStripMenuItem });
            toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(32, 20);
            toolStripSplitButton1.Text = "toolStripSplitButton1";
            // 
            // connectToClioToolStripMenuItem
            // 
            connectToClioToolStripMenuItem.Name = "connectToClioToolStripMenuItem";
            connectToClioToolStripMenuItem.Size = new Size(157, 22);
            connectToClioToolStripMenuItem.Text = "Connect to Clio";
            connectToClioToolStripMenuItem.Click += connectToClioToolStripMenuItem_Click;
            // 
            // progressBarPagesRetrieved
            // 
            progressBarPagesRetrieved.Location = new Point(12, 71);
            progressBarPagesRetrieved.Name = "progressBarPagesRetrieved";
            progressBarPagesRetrieved.Size = new Size(829, 23);
            progressBarPagesRetrieved.TabIndex = 3;
            // 
            // lblReportPageRetrieved
            // 
            lblReportPageRetrieved.AutoSize = true;
            lblReportPageRetrieved.Location = new Point(742, 53);
            lblReportPageRetrieved.Name = "lblReportPageRetrieved";
            lblReportPageRetrieved.Size = new Size(99, 15);
            lblReportPageRetrieved.TabIndex = 4;
            lblReportPageRetrieved.Text = "Report Progress...";
            lblReportPageRetrieved.TextAlign = ContentAlignment.BottomRight;
            // 
            // lblReportName
            // 
            lblReportName.AutoSize = true;
            lblReportName.Location = new Point(12, 53);
            lblReportName.Name = "lblReportName";
            lblReportName.Size = new Size(77, 15);
            lblReportName.TabIndex = 5;
            lblReportName.Text = "Report Name";
            lblReportName.TextAlign = ContentAlignment.BottomLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 97);
            label1.Name = "label1";
            label1.Size = new Size(1012, 15);
            label1.TabIndex = 6;
            label1.Text = "The FastFetch reports can capture up to 10,000 records, and are ideal for large datasets under 10,000 records. For small datasets or datasets over 10,000 records, use the Reports features instead.";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1343, 662);
            Controls.Add(label1);
            Controls.Add(lblReportName);
            Controls.Add(lblReportPageRetrieved);
            Controls.Add(progressBarPagesRetrieved);
            Controls.Add(statusStrip1);
            Controls.Add(btnOpenMailerForm);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "CalliAPI";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnOpenMailerForm;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem reportsToolStripMenuItem;
        private ToolStripMenuItem programToolStripMenuItem;
        private ToolStripMenuItem mailerProgramToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblApiStatus;
        private ToolStripStatusLabel lblClioAPIStatus;
        private ToolStripMenuItem unworkedMattersToolStripMenuItem;
        private ToolStripSplitButton toolStripSplitButton1;
        private ToolStripMenuItem connectToClioToolStripMenuItem;
        private ToolStripMenuItem allMattersToolStripMenuItem;
        private ProgressBar progressBarPagesRetrieved;
        private Label lblReportPageRetrieved;
        private Label lblReportName;
        private ToolStripSeparator toolStripSeparator1;
        private Label label1;
        private ToolStripMenuItem fastFetchToolStripMenuItem1;
        private ToolStripMenuItem createdSinceToolStripMenuItem;
    }
}