using System.Runtime.InteropServices;

namespace VpnQuickControl
{
    public partial class MainWindow : Form
    {
        // VPN�z�X�g��
        private readonly string vpnHostName = "HostRocky";
        // VPN�ڑ����
        private bool isVpnConnected = false;
        // �A�C�R���t�@�C���p�X
        private readonly string vpnConnectIconPath = @"Image\VPNConnected.ico";
        private readonly string vpnDisconnectIconPath = @"Image\VPNDisconnected.ico";

        public MainWindow()
        {
            InitializeComponent();
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

        private void ConnectVpn()
        {

        }

        private void DisconnectVpn()
        {
            
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
                lblStatus.Text = "VPN�ڑ��ς�";
            }
            else
            {
                Icon = new Icon(vpnDisconnectIconPath);
                lblStatus.Text = "VPN���ڑ�";
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

        // �z�b�g�L�[�̓o�^
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);
        // �z�b�g�L�[�̉���
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
