namespace VpnQuickControl
{
    public partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            btnConnect.Location = new Point(25, 41);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "接続";
            btnConnect.UseVisualStyleBackColor = true;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(160, 41);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(75, 23);
            btnDisconnect.TabIndex = 1;
            btnDisconnect.Text = "切断";
            btnDisconnect.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(103, 87);
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
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(276, 131);
            Controls.Add(lblStatus);
            Controls.Add(btnDisconnect);
            Controls.Add(btnConnect);
            Name = "MainWindow";
            Text = "VpnQuickControl";
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
