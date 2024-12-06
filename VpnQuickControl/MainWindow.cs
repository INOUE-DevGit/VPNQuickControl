using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace VpnQuickControl
{
    public partial class MainWindow : Form
    {
        // VPN接続状態
        private bool isVpnConnected = false;
        // アイコンファイルパス
        private readonly string vpnConnectIconPath = @"Image\VPNConnected.ico";
        private readonly string vpnDisconnectIconPath = @"Image\VPNDisconnected.ico";

        public MainWindow()
        {
            InitializeComponent();

            // アイコンと初期状態の設定
            Icon = new Icon(vpnDisconnectIconPath);
            ShowInTaskbar = true; // タスクバーに表示
            WindowState = FormWindowState.Minimized; // 最小化状態で起動
            Visible = true; // ウィンドウを最小化状態で表示

            // グローバルホットキーを登録 (Ctrl + Shift + V)
            bool hotKeyRegistered = RegisterHotKey(Handle, 0, KeyModifiers.Control | KeyModifiers.Shift, Keys.V);
            if (!hotKeyRegistered)
                MessageBox.Show("ホットキーの登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);

            CheckVpnStatus();
        }

        private void CheckVpnStatus()
        {
            try
            {
                string vpnName = Config.VpnName;

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "rasdial",
                        Arguments = vpnName, // VPN名のみを指定して接続状況を確認
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // "接続されている" などの成功メッセージが含まれているかチェック
                if (process.ExitCode == 0)
                {
                    isVpnConnected = true;
                    Invoke((Action)(() => lblStatus.Text = "VPN接続済み"));
                }
                else
                {
                    isVpnConnected = false;
                    Invoke((Action)(() => lblStatus.Text = "VPN未接続"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"VPN接続状況の確認中にエラーが発生しました:\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isVpnConnected = false; // デフォルトは未接続
                lblStatus.Text = "VPN未接続";
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
            // VPNの接続状態を確認
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
            string vpnName = Config.VpnName;         // 設定ファイルからVPN名を取得
            string vpnUserName = Config.UserName;    // 設定ファイルからユーザー名を取得
            string vpnPassword = GetDecryptedPassword();

            while (retryCount < MaxRetryCount && !isVpnConnected)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "rasdial",                                      // VPN接続コマンド
                        Arguments = $"{vpnName} {vpnUserName} {vpnPassword}",      // VPN接続名
                        RedirectStandardOutput = true,                             // 標準出力をリダイレクト(接続成功やメッセージなどを取得可能にする)
                        RedirectStandardError = true,                              // 標準エラー出力をリダイレクト(エラーメッセージなどを取得可能にする)
                        UseShellExecute = false,                                   // シェル機能を使用しない
                        CreateNoWindow = true                                      // コンソールウィンドウを表示しない
                    }
                };

                process.Start();
                this.Invoke((Action)(() => lblStatus.Text = "VPN接続試行中..."));
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    isVpnConnected = true;
                    Invoke((Action)(() => lblStatus.Text = "VPN接続済み"));
                }
                else
                {
                    retryCount++;
                    // TODO ラベルに表示されない
                    Invoke((Action)(() => lblStatus.Text = $"VPN接続失敗。リトライ: {retryCount}/{MaxRetryCount}"));
                }
            }

            if (!isVpnConnected)
            {
                Invoke((Action)(() => lblStatus.Text = "VPN未接続"));
                Invoke((Action)(() => MessageBox.Show("VPN接続に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private static string GetDecryptedPassword()
        {
            // 暗号化されたパスワードをBase64文字列として読み込み
            string encryptedPassword = File.ReadAllText(Config.PasswordFilePath);

            // Base64文字列をバイト配列に変換
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);

            // 復号化
            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

            // バイト配列を文字列に変換して返す
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private void DisconnectVpn()
        {
            string vpnName = Config.VpnName; // 設定ファイルからVPN名を取得

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

            lblStatus.Text = "VPN未接続";
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

        // ホットキーの登録
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        // ホットキーの解除
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectVpn();
            UpdateTaskbarIcon();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectVpn();
            UpdateTaskbarIcon();
        }
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
