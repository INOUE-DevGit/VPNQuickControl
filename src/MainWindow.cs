using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace VpnQuickControl
{
    public partial class MainWindow : Form
    {
        // VPN接続状態
        private bool _isVpnConnected = false;

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
            Icon = new Icon(Config.VpnDisconnectIconPath);
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
                case VpnStatus.Disconnected:
                    _isVpnConnected = false;
                    break;
                case VpnStatus.Connected:
                    _isVpnConnected = true;
                    break;
                case VpnStatus.Error:
                default:
                    break;
            }
            UpdateStatus(_isVpnConnected ? VpnStatus.Connected.GetDescription() : VpnStatus.Disconnected.GetDescription());
            UpdateTaskbarIcon();
        }

        /// <summary>
        /// VPN接続状態を切り替え
        /// </summary>
        private void ToggleVpnState()
        {
            if (_isVpnConnected)
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
            const int MaxRetryCount = 3;
            const int RetryDelayMs = 2000;
            int retryCount = 0;

            UpdateStatus(VpnStatus.Connecting.GetDescription());

            while (retryCount < MaxRetryCount && !_isVpnConnected)
            {
                retryCount++;
                string vpnCommand = $"{Config.VpnName} {Config.UserName} {GetDecryptedPassword()}";

                // VPN接続コマンドを実行
                switch (ExecuteVpnCommand(vpnCommand))
                {
                    case VpnStatus.Connected:
                        _isVpnConnected = true;
                        UpdateStatus(VpnStatus.Connected.GetDescription());
                        UpdateTaskbarIcon();
                        return;

                    case VpnStatus.Disconnected:
                    case VpnStatus.Error:
                    default:
                        _isVpnConnected = false;
                        UpdateStatus($"{VpnStatus.Connecting.GetDescription()}... ({retryCount}/{MaxRetryCount})");
                        break;
                }
                Thread.Sleep(RetryDelayMs);
            }

            // 最大再試行回数に達しても成功しなかった場合
            UpdateStatus(VpnStatus.ConnectionFailed.GetDescription());
            UpdateTaskbarIcon();
            MessageBox.Show("VPN接続に失敗しました。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// VPN切断
        /// </summary>
        private void DisconnectVpn()
        {
            string vpnCommand = $"{Config.VpnName} /disconnect";

            switch (ExecuteVpnCommand(vpnCommand))
            {
                case VpnStatus.Disconnected:
                    _isVpnConnected = false;
                    break;
                case VpnStatus.Connected:
                    _isVpnConnected = true;
                    break;
                case VpnStatus.Error:
                default:
                    break;
            }
            UpdateStatus(!_isVpnConnected ? VpnStatus.Disconnected.GetDescription() : VpnStatus.ConnectionFailed.GetDescription());
            UpdateTaskbarIcon();
        }

        /// <summary>
        /// VPNコマンドを実行
        /// </summary>
        /// <param name="arguments">VPNパラメータ</param>
        /// <remarks>argumentsで空文字を送るとステータスの確認が行えます。</remarks>
        private static VpnStatus ExecuteVpnCommand(string arguments)
        {
            try
            {
                Config.LoadConfig();

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
                    return VpnStatus.Connected;
                else
                    return VpnStatus.Disconnected;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"VPNコマンドの実行中にエラーが発生しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return VpnStatus.Error;
            }
        }

        /// <summary>
        /// ステータスを更新
        /// </summary>
        /// <param name="status">ラベルに表示させる文字列</param>
        private void UpdateStatus(string status) => Invoke((Action)(() => lblStatus.Text = status));

        /// <summary>
        /// タスクバーアイコンを更新
        /// </summary>
        private void UpdateTaskbarIcon() => Icon = new Icon(_isVpnConnected ? Config.VpnConnectIconPath : Config.VpnDisconnectIconPath);

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

    }

    public enum VpnStatus
    {
        [Description("VPN未接続")]
        Disconnected = 0,

        [Description("VPN接続試行中")]
        Connecting = 1,

        [Description("VPN接続済み")]
        Connected = 2,

        [Description("VPN接続失敗")]
        ConnectionFailed = 3,

        [Description("VPN切断試行中")]
        Disconnecting = 4,

        [Description("VPN切断失敗")]
        DisconnectionFailed = 5,

        [Description("エラー")]
        Error = -1
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
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
