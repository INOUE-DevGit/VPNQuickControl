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
            this.ShowInTaskbar = true; // �^�X�N�o�[�ɕ\��
            this.WindowState = FormWindowState.Minimized; // �ŏ�����ԂŋN��
            this.Visible = false; // �E�B���h�E��\�����邪�ŏ�������Ă���
            RegisterHotKey(this.Handle, 0, KeyModifiers.None, Keys.F12); // F12�L�[�Ő؂�ւ�
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.WindowState = FormWindowState.Minimized; // �ŏ�����ԂŋN��
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
                this.Text = "VPN�ڑ��ς�";
            }
            else
            {
                this.Icon = new Icon(@"Image\VPNDisconnected.ico");
                this.Text = "VPN���ڑ�";
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

        // �O���[�o���z�b�g�L�[�̓o�^/����
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
