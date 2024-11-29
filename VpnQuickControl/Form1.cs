using System;
using System.Drawing;
using System.Windows.Forms;

namespace VpnQuickControl
{
    public partial class Form1 : Form
    {
        private bool isVpnConnected = false; // VPN接続状態

        public Form1()
        {
            InitializeComponent();
            UpdateStatus(); // 初期状態の更新
        }

        private void UpdateStatus()
        {
            // タスクバーのアイコンとウィンドウタイトルを状態に応じて変更
            if (isVpnConnected)
            {
                this.Text = "VPN接続済み";
                this.Icon = new Icon(@"Image\VPNConnected.ico"); // 接続中のアイコン
            }
            else
            {
                this.Text = "VPN未接続";
                this.Icon = new Icon(@"Image\VPNDisconnected.ico"); // 未接続のアイコン
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            isVpnConnected = true;
            UpdateStatus();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            isVpnConnected = false;
            UpdateStatus();
        }
    }
}
