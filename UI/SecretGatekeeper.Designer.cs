namespace CalliAPI.UI
{
    partial class SecretGatekeeper
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
            label2 = new Label();
            txtSecret = new TextBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 19);
            label1.Name = "label1";
            label1.Size = new Size(203, 15);
            label1.TabIndex = 0;
            label1.Text = "Enter your CLIO_CLIENT_SECRET key:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            label2.Location = new Point(12, 56);
            label2.Name = "label2";
            label2.Size = new Size(410, 15);
            label2.TabIndex = 1;
            label2.Text = "If you don't know what that means, ask your software support! Do not guess!";
            // 
            // txtSecret
            // 
            txtSecret.Location = new Point(221, 16);
            txtSecret.Name = "txtSecret";
            txtSecret.Size = new Size(422, 23);
            txtSecret.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 32F);
            label3.Location = new Point(12, 94);
            label3.Name = "label3";
            label3.Size = new Size(367, 59);
            label3.TabIndex = 3;
            label3.Text = "What's Going On?";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 169);
            label4.Name = "label4";
            label4.Size = new Size(767, 15);
            label4.TabIndex = 4;
            label4.Text = "You're seeing this because you don't have the proper client secret for the CalliAPI Clio App assigned to your registry. If you know it, enter it now. ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 194);
            label5.Name = "label5";
            label5.Size = new Size(234, 15);
            label5.TabIndex = 5;
            label5.Text = "Otherwise, reach out to your support team.";
            // 
            // button1
            // 
            button1.DialogResult = DialogResult.OK;
            button1.Location = new Point(649, 16);
            button1.Name = "button1";
            button1.Size = new Size(75, 26);
            button1.TabIndex = 6;
            button1.Text = "Submit";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.DialogResult = DialogResult.Cancel;
            button2.Location = new Point(435, 415);
            button2.Name = "button2";
            button2.Size = new Size(344, 23);
            button2.TabIndex = 7;
            button2.Text = "I don't have a CLIO_CLIENT_SECRET key; exit the program.";
            button2.UseVisualStyleBackColor = true;
            // 
            // SecretGatekeeper
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtSecret);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "SecretGatekeeper";
            Text = "SecretGatekeeper";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtSecret;
        private Label label3;
        private Label label4;
        private Label label5;
        private Button button1;
        private Button button2;
    }
}