using System.Runtime.InteropServices;

namespace VpnQuickControl
{
    public partial class MainWindow : Form
    {
        private bool isVpnConnected = false; // VPN�ڑ����
        private readonly string vpnConnectIconPath = @"Image\VPNConnected.ico"; // VPN�ڑ����̃A�C�R��
        private readonly string vpnDisconnectIconPath = @"Image\VPNDisconnected.ico"; // VPN���ڑ����̃A�C�R��

        public MainWindow()
        {
            InitializeComponent();
            Text = "VpnQuickControl";
            Icon = new Icon(vpnDisconnectIconPath);
            ShowInTaskbar = true; // �^�X�N�o�[�ɕ\��
            WindowState = FormWindowState.Minimized; // �ŏ�����ԂŋN��
            Visible = true; // �E�B���h�E���ŏ�����Ԃŕ\��

            // �O���[�o���z�b�g�L�[��o�^ (Ctrl + Shift + V)
            bool hotKeyRegistered = RegisterHotKey(this.Handle, 0, KeyModifiers.Control | KeyModifiers.Shift, Keys.V);
            if (!hotKeyRegistered)
            {
                MessageBox.Show("�z�b�g�L�[�̓o�^�Ɏ��s���܂����B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Text = "VPN�ڑ��ς�";
            }
            else
            {
                Icon = new Icon(vpnDisconnectIconPath);
                Text = "VPN���ڑ�";
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

        // WinAPI�֐��̃C���|�[�g
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
