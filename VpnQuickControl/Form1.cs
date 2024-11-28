namespace VpnQuickControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeNotifyIcon(); // 通知アイコンの初期化
        }

        private bool isVpnConnected = false; // VPN接続状態

        private void InitializeNotifyIcon()
        {
            notifyIcon.Icon = new Icon("disconnected.ico"); // 最初のアイコン
            notifyIcon.Visible = true;
            notifyIcon.Text = "VPN未接続";

            // 通知アイコンをダブルクリックでVPN接続/切断を切り替える
            notifyIcon.DoubleClick += (sender, e) => ToggleVpnConnection();
        }

        private void ToggleVpnConnection()
        {
            if (isVpnConnected)
            {
                DisconnectVpn();
            }
            else
            {
                ConnectVpn();
            }
        }

        private void ConnectVpn()
        {
            isVpnConnected = true;
            lblStatus.Text = "VPN接続済み";
            notifyIcon.Icon = new Icon("connected.ico"); // 接続アイコンに変更
            notifyIcon.Text = "VPN接続済み";
            MessageBox.Show("VPNに接続しました。");
        }

        private void DisconnectVpn()
        {
            isVpnConnected = false;
            lblStatus.Text = "VPN未接続";
            notifyIcon.Icon = new Icon("disconnected.ico"); // 切断アイコンに変更
            notifyIcon.Text = "VPN未接続";
            MessageBox.Show("VPNを切断しました。");
        }

        // 接続ボタン
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectVpn();
        }

        // 切断ボタン
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectVpn();
        }
    }
}
