using System.Runtime.InteropServices;

namespace VpnQuickControl
{
    public partial class MainWindow : Form
    {
        private bool isVpnConnected = false; // VPN接続状態
        private readonly string vpnConnectIconPath = @"Image\VPNConnected.ico"; // VPN接続時のアイコン
        private readonly string vpnDisconnectIconPath = @"Image\VPNDisconnected.ico"; // VPN未接続時のアイコン

        public MainWindow()
        {
            InitializeComponent();
            Text = "VpnQuickControl";
            Icon = new Icon(vpnDisconnectIconPath);
            ShowInTaskbar = true; // タスクバーに表示
            WindowState = FormWindowState.Minimized; // 最小化状態で起動
            Visible = true; // ウィンドウを最小化状態で表示

            // グローバルホットキーを登録 (Ctrl + Shift + V)
            bool hotKeyRegistered = RegisterHotKey(this.Handle, 0, KeyModifiers.Control | KeyModifiers.Shift, Keys.V);
            if (!hotKeyRegistered)
            {
                MessageBox.Show("ホットキーの登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                ToggleVpnState();
            }
            base.WndProc(ref m);
        }

        private void ToggleVpnState()
        {
            isVpnConnected = !isVpnConnected;
            UpdateTaskbarIcon();
        }

        private void UpdateTaskbarIcon()
        {
            if (isVpnConnected)
            {
                Icon = new Icon(vpnConnectIconPath);
                Text = "VPN接続済み";
            }
            else
            {
                Icon = new Icon(vpnDisconnectIconPath);
                Text = "VPN未接続";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnregisterHotKey(this.Handle, 0);
            }
            base.Dispose(disposing);
        }

        // WinAPI関数のインポート
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }

    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
}
