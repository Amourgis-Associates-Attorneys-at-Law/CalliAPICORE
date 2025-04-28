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
            programToolStripMenuItem = new ToolStripMenuItem();
            mailerProgramToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            lblApiStatus = new ToolStripStatusLabel();
            lblClioAPIStatus = new ToolStripStatusLabel();
            toolStripSplitButton1 = new ToolStripSplitButton();
            connectToClioToolStripMenuItem = new ToolStripMenuItem();
            allMattersToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnOpenMailerForm
            // 
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
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, reportsToolStripMenuItem, programToolStripMenuItem });
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
            reportsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { unworkedMattersToolStripMenuItem, allMattersToolStripMenuItem });
            reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            reportsToolStripMenuItem.Size = new Size(59, 20);
            reportsToolStripMenuItem.Text = "Reports";
            // 
            // unworkedMattersToolStripMenuItem
            // 
            unworkedMattersToolStripMenuItem.Name = "unworkedMattersToolStripMenuItem";
            unworkedMattersToolStripMenuItem.Size = new Size(180, 22);
            unworkedMattersToolStripMenuItem.Text = "Unworked Matters";
            unworkedMattersToolStripMenuItem.Click += unworkedMattersToolStripMenuItem_Click;
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
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(103, 22);
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
            // allMattersToolStripMenuItem
            // 
            allMattersToolStripMenuItem.Name = "allMattersToolStripMenuItem";
            allMattersToolStripMenuItem.Size = new Size(180, 22);
            allMattersToolStripMenuItem.Text = "All Matters";
            allMattersToolStripMenuItem.Click += allMattersToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1343, 662);
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
    }
}