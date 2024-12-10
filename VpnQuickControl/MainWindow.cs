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

            InitializeWindow();
            RegisterGlobalHotKey();
            CheckVpnStatus();
        }

        /// <summary>
        /// ウィンドウの初期化
        /// </summary>
        private void InitializeWindow()
        {
            Icon = new Icon(vpnDisconnectIconPath);
            // タスクバーに表示
            ShowInTaskbar = true;
            // 最小化状態で起動
            WindowState = FormWindowState.Minimized;
            // ウィンドウを最小化状態で表示
            Visible = true;
        }

        /// <summary>
        /// グローバルホットキーを登録
        /// </summary>
        private void RegisterGlobalHotKey()
        {
            // グローバルホットキーを登録 (Ctrl + Shift + V)
            bool hotKeyRegistered = RegisterHotKey(Handle, 0, KeyModifiers.Control | KeyModifiers.Shift, Keys.V);

            if (!hotKeyRegistered)
                MessageBox.Show("ホットキーの登録に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// VPN接続状態を確認
        /// </summary>
        private void CheckVpnStatus()
        {
            switch (ExecuteVpnCommand(""))
            {
                case VpnStatus.VPN未接続:
                    isVpnConnected = false;
                    break;
                case VpnStatus.VPN接続済み:
                    isVpnConnected = true;
                    break;
                case VpnStatus.エラー:
                default:
                    break;
            }
            UpdateStatus(isVpnConnected ? VpnStatus.VPN接続済み.ToString() : VpnStatus.VPN未接続.ToString());
            UpdateTaskbarIcon();
        }

        /// <summary>
        /// VPN接続状態を切り替え
        /// </summary>
        private void ToggleVpnState()
        {
            if (isVpnConnected)
                DisconnectVpn();
            else
                ConnectVpn();
        }

        private void btnConnect_Click(object sender, EventArgs e) => ConnectVpn();

        private void btnDisconnect_Click(object sender, EventArgs e) => DisconnectVpn();

        /// <summary>
        /// VPN接続
        /// </summary>
        private void ConnectVpn()
        {
            string vpnCommand = $"{Config.VpnName} {Config.UserName} {GetDecryptedPassword()}";
            
            switch (ExecuteVpnCommand(vpnCommand))
            {
                case VpnStatus.VPN未接続:
                    isVpnConnected = false;
                    break;
                case VpnStatus.VPN接続済み:
                    isVpnConnected = true;
                    break;
                case VpnStatus.エラー:
                default:
                    break;
            }

            UpdateStatus(isVpnConnected ? VpnStatus.VPN接続済み.ToString() : "VPN接続失敗");
            UpdateTaskbarIcon();
        }

        /// <summary>
        /// VPN切断
        /// </summary>
        private void DisconnectVpn()
        {
            string vpnCommand = $"{Config.VpnName} /disconnect";

            switch (ExecuteVpnCommand(vpnCommand))
            {
                case VpnStatus.VPN未接続:
                    isVpnConnected = false;
                    break;
                case VpnStatus.VPN接続済み:
                    isVpnConnected = true;
                    break;
                case VpnStatus.エラー:
                default:
                    break;
            }
            UpdateStatus(!isVpnConnected ? VpnStatus.VPN未接続.ToString() : "VPN切断失敗");
            UpdateTaskbarIcon();
        }

        /// <summary>
        /// VPNコマンドを実行
        /// </summary>
        /// <param name="arguments"></param>
        /// 
        /// <returns></returns>
        private static VpnStatus ExecuteVpnCommand(string arguments)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "rasdial",            // VPN接続コマンド
                        Arguments = arguments,           // VPNパラメータ
                        RedirectStandardOutput = true,   // 標準出力をリダイレクト(接続成功やメッセージなどを取得可能にする)
                        RedirectStandardError = true,    // 標準エラー出力をリダイレクト(エラーメッセージなどを取得可能にする)
                        UseShellExecute = false,         // シェル機能を使用しない
                        CreateNoWindow = true            // コンソールウィンドウを表示しない
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // VPNコマンド正常終了時はExitCodeが0、それ以外はエラー
                if (process.ExitCode != 0)
                    throw new Exception($"VPNコマンドの実行に失敗しました。ExitCode: {process.ExitCode}");

                // 接続済みだと"次のサーバーに接続しました。"
                // 未接続だと"接続なし"
                // 接続時は "{}に正常に接続しました。"
                // 切断時には"コマンドは正常に終了しました。"のみ。これは処理が成功したすべての場合に出力される
                if (output.Contains("接続しました"))
                    return VpnStatus.VPN接続済み;
                else
                    return VpnStatus.VPN未接続;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"VPNコマンドの実行中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return VpnStatus.エラー;
            }
        }

        /// <summary>
        /// ステータスを更新
        /// </summary>
        /// <param name="status"></param>
        private void UpdateStatus(string status) => Invoke((Action)(() => lblStatus.Text = status));

        /// <summary>
        /// タスクバーアイコンを更新
        /// </summary>
        private void UpdateTaskbarIcon() => Icon = new Icon(isVpnConnected ? vpnConnectIconPath : vpnDisconnectIconPath);

        /// <summary>
        /// 暗号化されたパスワードを復号
        /// </summary>
        private static string GetDecryptedPassword()
        {
            try
            {
                string encryptedPassword = File.ReadAllText(Config.PasswordFilePath);
                byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
                byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"パスワードの復号中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;

            if (m.Msg == WM_HOTKEY)
                ToggleVpnState();

            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                UnregisterHotKey(Handle, 0);
            base.Dispose(disposing);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private enum VpnStatus
        {
            VPN未接続 = 0,
            VPN接続試行中 = 1,
            VPN接続済み = 2,
            VPN切断試行中 = 3,
            エラー = -1
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
