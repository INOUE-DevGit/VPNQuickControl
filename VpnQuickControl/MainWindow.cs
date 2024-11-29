using System.Runtime.InteropServices;

namespace VpnQuickControl
{
    public partial class MainWindow : Form
    {
        private bool isVpnConnected = false;

        public MainWindow()
        {
            this.Text = "VpnQuickControl";
            this.Icon = new Icon(@"Image\VPNDisconnected.ico");
            this.ShowInTaskbar = true; // タスクバーに表示
            this.WindowState = FormWindowState.Minimized; // 最小化状態で起動
            this.Visible = false; // ウィンドウを表示するが最小化されている
            RegisterHotKey(this.Handle, 0, KeyModifiers.None, Keys.F12); // F12キーで切り替え
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.WindowState = FormWindowState.Minimized; // 最小化状態で起動
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
                this.Icon = new Icon(@"Image\VPNConnected.ico");
                this.Text = "VPN接続済み";
            }
            else
            {
                this.Icon = new Icon(@"Image\VPNDisconnected.ico");
                this.Text = "VPN未接続";
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

        // グローバルホットキーの登録/解除
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll")]
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
