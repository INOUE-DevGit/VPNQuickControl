using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace VpnQuickControl
{
    public partial class MainWindow : Form
    {
        // VPN�ڑ���
        private readonly string vpnName = "HostRocky";
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
            bool hotKeyRegistered = RegisterHotKey(Handle, 0, KeyModifiers.Control | KeyModifiers.Shift, Keys.V);
            if (!hotKeyRegistered)
            {
                MessageBox.Show("�z�b�g�L�[�̓o�^�Ɏ��s���܂����B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
                ToggleVpnState();
            base.WndProc(ref m);
        }

        private void ToggleVpnState()
        {
            // VPN�̐ڑ���Ԃ��m�F
            if (isVpnConnected)
                DisconnectVpn();
            else
                ConnectVpn();
            UpdateTaskbarIcon();
        }

        private void ConnectVpn()
        {
            int retryCount = 0;
            const int MaxRetryCount = 3;
            string vpnUserName = "user";
            string vpnPassword = GetDecryptedPassword();

            while (retryCount < MaxRetryCount && !isVpnConnected)
            {
                // �ڑ��R�}���h�̎��s
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "rasdial",                                      // VPN�ڑ��R�}���h
                        Arguments = $"{vpnName} {vpnUserName} {vpnPassword}",      // VPN�ڑ���
                        RedirectStandardOutput = true,                             // �W���o�͂����_�C���N�g(�ڑ������⃁�b�Z�[�W�Ȃǂ��擾�\�ɂ���)
                        RedirectStandardError = true,                              // �W���G���[�o�͂����_�C���N�g(�G���[���b�Z�[�W�Ȃǂ��擾�\�ɂ���)
                        UseShellExecute = false,                                   // �V�F���@�\���g�p���Ȃ�
                        CreateNoWindow = true                                      // �R���\�[���E�B���h�E��\�����Ȃ�
                    }
                };

                process.Start();
                this.Invoke((Action)(() => lblStatus.Text = "VPN�ڑ����s��..."));
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    isVpnConnected = true;
                    Invoke((Action)(() => lblStatus.Text = "VPN�ڑ��ς�"));
                }
                else
                {
                    retryCount++;
                    // TODO ���x���ɕ\������Ȃ�
                    Invoke((Action)(() => lblStatus.Text = $"VPN�ڑ����s�B���g���C: {retryCount}/{MaxRetryCount}"));
                }
            }

            if (!isVpnConnected)
            {
                Invoke((Action)(() => lblStatus.Text = "VPN���ڑ�"));
                Invoke((Action)(() => MessageBox.Show("VPN�ڑ��Ɏ��s���܂����B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private static string GetDecryptedPassword()
        {
            // �Í������ꂽ�p�X���[�h��Base64������Ƃ��ēǂݍ���
            string encryptedPassword = File.ReadAllText("password.dat");

            // Base64��������o�C�g�z��ɕϊ�
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);

            // ������
            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

            // �o�C�g�z��𕶎���ɕϊ����ĕԂ�
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private void DisconnectVpn()
        {
            // �ؒf�R�}���h�̎��s
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "rasdial",
                    Arguments = $"{vpnName} /disconnect",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            lblStatus.Text = "VPN���ڑ�";
            isVpnConnected = false;
        }

        private void UpdateTaskbarIcon()
        {
            if (isVpnConnected)
                Icon = new Icon(vpnConnectIconPath);
            else
                Icon = new Icon(vpnDisconnectIconPath);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                UnregisterHotKey(Handle, 0);
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
