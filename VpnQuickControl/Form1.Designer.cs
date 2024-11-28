namespace VpnQuickControl
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
            components = new System.ComponentModel.Container();
            btnConnect = new Button();
            btnDisconnect = new Button();
            lblStatus = new Label();
            notifyIcon = new NotifyIcon(components);
            SuspendLayout();
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(131, 102);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "接続";
            btnConnect.UseVisualStyleBackColor = true;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(364, 102);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(75, 23);
            btnDisconnect.TabIndex = 1;
            btnDisconnect.Text = "切断";
            btnDisconnect.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(265, 179);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(66, 15);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "VPN未接続";
            // 
            // notifyIcon
            // 
            notifyIcon.Text = "VPN未接続";
            notifyIcon.Visible = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblStatus);
            Controls.Add(btnDisconnect);
            Controls.Add(btnConnect);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnConnect;
        private Button btnDisconnect;
        private Label lblStatus;
        private NotifyIcon notifyIcon;
    }
}
