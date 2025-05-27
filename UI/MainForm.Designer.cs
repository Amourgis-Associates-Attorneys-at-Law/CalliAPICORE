using CalliAPI.UI.Controls;

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
            TreeNode treeNode1 = new TreeNode("Unworked Matters");
            TreeNode treeNode2 = new TreeNode("Open Matters", new TreeNode[] { treeNode1 });
            TreeNode treeNode3 = new TreeNode("All Matters");
            TreeNode treeNode4 = new TreeNode("Reports", new TreeNode[] { treeNode2, treeNode3 });
            TreeNode treeNode5 = new TreeNode("FastFetch");
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            checkForUpdatesToolStripMenuItem = new ToolStripMenuItem();
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
            toolStripBtnConnectToClio = new ToolStripStatusLabel();
            progressBarPagesRetrieved = new ProgressBarWithText();
            lblReportPageRetrieved = new Label();
            lblReportName = new Label();
            textBox1 = new TextBox();
            splitContainer1 = new SplitContainer();
            treeViewReports = new TreeView();
            panelContent = new Panel();
            panel1 = new Panel();
            lblVersion = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            debugToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, reportsToolStripMenuItem, fastFetchToolStripMenuItem1, programToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1184, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { checkForUpdatesToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            checkForUpdatesToolStripMenuItem.Size = new Size(180, 22);
            checkForUpdatesToolStripMenuItem.Text = "Check for Updates";
            checkForUpdatesToolStripMenuItem.Click += checkForUpdatesToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(180, 22);
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
            programToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mailerProgramToolStripMenuItem, debugToolStripMenuItem });
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
            openToolStripMenuItem.Size = new Size(103, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblApiStatus, lblClioAPIStatus, toolStripBtnConnectToClio });
            statusStrip1.Location = new Point(0, 539);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1184, 22);
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
            // toolStripBtnConnectToClio
            // 
            toolStripBtnConnectToClio.BackColor = SystemColors.ButtonShadow;
            toolStripBtnConnectToClio.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            toolStripBtnConnectToClio.ForeColor = SystemColors.HotTrack;
            toolStripBtnConnectToClio.Image = (Image)resources.GetObject("toolStripBtnConnectToClio.Image");
            toolStripBtnConnectToClio.ImageTransparentColor = Color.Magenta;
            toolStripBtnConnectToClio.Name = "toolStripBtnConnectToClio";
            toolStripBtnConnectToClio.Size = new Size(107, 17);
            toolStripBtnConnectToClio.Text = "Connect to Clio";
            toolStripBtnConnectToClio.Click += toolStripBtnConnectToClio_Click;
            // 
            // progressBarPagesRetrieved
            // 
            progressBarPagesRetrieved.ForeColor = Color.FromArgb(162, 188, 214);
            progressBarPagesRetrieved.Location = new Point(13, 28);
            progressBarPagesRetrieved.Name = "progressBarPagesRetrieved";
            progressBarPagesRetrieved.Size = new Size(829, 23);
            progressBarPagesRetrieved.TabIndex = 3;
            // 
            // lblReportPageRetrieved
            // 
            lblReportPageRetrieved.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblReportPageRetrieved.AutoSize = true;
            lblReportPageRetrieved.Location = new Point(743, 10);
            lblReportPageRetrieved.Name = "lblReportPageRetrieved";
            lblReportPageRetrieved.Size = new Size(99, 15);
            lblReportPageRetrieved.TabIndex = 4;
            lblReportPageRetrieved.Text = "Report Progress...";
            lblReportPageRetrieved.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblReportName
            // 
            lblReportName.AutoSize = true;
            lblReportName.Location = new Point(13, 10);
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
            textBox1.Location = new Point(13, 57);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(829, 34);
            textBox1.TabIndex = 7;
            textBox1.Text = "The FastFetch reports can capture up to 10,000 records, and are ideal for large datasets under 10,000 records. For small datasets or datasets over 10,000 records, use the Reports features instead.";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 153);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(treeViewReports);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panelContent);
            splitContainer1.Size = new Size(1178, 359);
            splitContainer1.SplitterDistance = 390;
            splitContainer1.TabIndex = 8;
            // 
            // treeViewReports
            // 
            treeViewReports.Dock = DockStyle.Fill;
            treeViewReports.Location = new Point(0, 0);
            treeViewReports.Name = "treeViewReports";
            treeNode1.Name = "Unworked Matters";
            treeNode1.Tag = "ReportsOpenMattersUnworkedMatters";
            treeNode1.Text = "Unworked Matters";
            treeNode2.Name = "Open Matters";
            treeNode2.Tag = "ReportsOpenMatters";
            treeNode2.Text = "Open Matters";
            treeNode3.Name = "All Matters";
            treeNode3.Tag = "ReportsAllMatters";
            treeNode3.Text = "All Matters";
            treeNode4.Name = "Reports";
            treeNode4.Tag = "Reports";
            treeNode4.Text = "Reports";
            treeNode5.Name = "FastFetch";
            treeNode5.NodeFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            treeNode5.Tag = "FastFetch";
            treeNode5.Text = "FastFetch";
            treeViewReports.Nodes.AddRange(new TreeNode[] { treeNode4, treeNode5 });
            treeViewReports.Size = new Size(390, 359);
            treeViewReports.TabIndex = 0;
            treeViewReports.AfterSelect += treeViewReports_AfterSelect;
            // 
            // panelContent
            // 
            panelContent.AutoScroll = true;
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(0, 0);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(784, 359);
            panelContent.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblVersion);
            panel1.Controls.Add(progressBarPagesRetrieved);
            panel1.Controls.Add(lblReportPageRetrieved);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(lblReportName);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1178, 144);
            panel1.TabIndex = 9;
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Font = new Font("Sylfaen", 8.25F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblVersion.ForeColor = SystemColors.AppWorkspace;
            lblVersion.Location = new Point(377, 10);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(70, 14);
            lblVersion.TabIndex = 8;
            lblVersion.Text = "Version 0.0.0";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(splitContainer1, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 24);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1184, 515);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // debugToolStripMenuItem
            // 
            debugToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem2 });
            debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            debugToolStripMenuItem.Size = new Size(180, 22);
            debugToolStripMenuItem.Text = "Debug";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(180, 22);
            toolStripMenuItem2.Text = "100%";
            toolStripMenuItem2.Click += Debug100Percent;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 561);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "CalliAPI";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
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
        private ToolStripMenuItem allMattersToolStripMenuItem;
        private Label lblReportPageRetrieved;
        private Label lblReportName;
        private ToolStripMenuItem fastFetchToolStripMenuItem1;
        private ToolStripMenuItem createdSinceToolStripMenuItem;
        private ToolStripMenuItem all713MattersToolStripMenuItem;
        private TextBox textBox1;
        private ToolStripMenuItem allUnworked7And13MattersToolStripMenuItem;
        private SplitContainer splitContainer1;
        private TreeView treeViewReports;
        private Panel panelContent;
        private Panel panel1;
        private ToolStripStatusLabel toolStripBtnConnectToClio;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblVersion;
        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private ProgressBarWithText progressBarPagesRetrieved;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
    }
}