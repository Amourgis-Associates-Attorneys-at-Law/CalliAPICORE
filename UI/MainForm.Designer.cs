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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            reportsToolStripMenuItem = new ToolStripMenuItem();
            unworkedMattersToolStripMenuItem = new ToolStripMenuItem();
            allMattersToolStripMenuItem = new ToolStripMenuItem();
            all713MattersToolStripMenuItem = new ToolStripMenuItem();
            allUnworked7And13MattersToolStripMenuItem = new ToolStripMenuItem();
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
            textBox1 = new TextBox();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, reportsToolStripMenuItem, fastFetchToolStripMenuItem1, programToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1043, 24);
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
            reportsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { unworkedMattersToolStripMenuItem, allMattersToolStripMenuItem, all713MattersToolStripMenuItem, allUnworked7And13MattersToolStripMenuItem });
            reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            reportsToolStripMenuItem.Size = new Size(59, 20);
            reportsToolStripMenuItem.Text = "Reports";
            // 
            // unworkedMattersToolStripMenuItem
            // 
            unworkedMattersToolStripMenuItem.Enabled = false;
            unworkedMattersToolStripMenuItem.Name = "unworkedMattersToolStripMenuItem";
            unworkedMattersToolStripMenuItem.Size = new Size(235, 22);
            unworkedMattersToolStripMenuItem.Text = "Unworked Matters";
            unworkedMattersToolStripMenuItem.Click += unworkedMattersToolStripMenuItem_Click;
            // 
            // allMattersToolStripMenuItem
            // 
            allMattersToolStripMenuItem.Name = "allMattersToolStripMenuItem";
            allMattersToolStripMenuItem.Size = new Size(235, 22);
            allMattersToolStripMenuItem.Text = "All Matters";
            allMattersToolStripMenuItem.Click += allMattersToolStripMenuItem_Click;
            // 
            // all713MattersToolStripMenuItem
            // 
            all713MattersToolStripMenuItem.Name = "all713MattersToolStripMenuItem";
            all713MattersToolStripMenuItem.Size = new Size(235, 22);
            all713MattersToolStripMenuItem.Text = "All Open 7 & 13 Matters";
            all713MattersToolStripMenuItem.Click += all713MattersToolStripMenuItem_Click;
            // 
            // allUnworked7And13MattersToolStripMenuItem
            // 
            allUnworked7And13MattersToolStripMenuItem.Name = "allUnworked7And13MattersToolStripMenuItem";
            allUnworked7And13MattersToolStripMenuItem.Size = new Size(235, 22);
            allUnworked7And13MattersToolStripMenuItem.Text = "All Unworked 7 and 13 Matters";
            allUnworked7And13MattersToolStripMenuItem.Click += allUnworked7And13MattersToolStripMenuItem_Click;
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
            mailerProgramToolStripMenuItem.Size = new Size(156, 22);
            mailerProgramToolStripMenuItem.Text = "Mailer Program";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Enabled = false;
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(103, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblApiStatus, lblClioAPIStatus, toolStripSplitButton1 });
            statusStrip1.Location = new Point(0, 438);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1043, 22);
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
            // textBox1
            // 
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            textBox1.ForeColor = SystemColors.ControlDark;
            textBox1.Location = new Point(12, 100);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(829, 122);
            textBox1.TabIndex = 7;
            textBox1.Text = "The FastFetch reports can capture up to 10,000 records, and are ideal for large datasets under 10,000 records. For small datasets or datasets over 10,000 records, use the Reports features instead.";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1043, 460);
            Controls.Add(textBox1);
            Controls.Add(lblReportName);
            Controls.Add(lblReportPageRetrieved);
            Controls.Add(progressBarPagesRetrieved);
            Controls.Add(statusStrip1);
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
        private ToolStripMenuItem fastFetchToolStripMenuItem1;
        private ToolStripMenuItem createdSinceToolStripMenuItem;
        private ToolStripMenuItem all713MattersToolStripMenuItem;
        private TextBox textBox1;
        private ToolStripMenuItem allUnworked7And13MattersToolStripMenuItem;
    }
}