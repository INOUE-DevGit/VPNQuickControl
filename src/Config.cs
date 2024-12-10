using System.Text.Json;

namespace VpnQuickControl
{
    public static class Config
    {
        /// <summary>
        /// VPN接続名
        /// </summary>
        public static string VpnName { get; private set; } = "VPNName";
        /// <summary>
        /// ユーザー名
        /// </summary>
        public static string UserName { get; private set; } = "user";

        /// <summary>
        /// パスワードファイル名
        /// </summary>
        public static readonly string PasswordFilePath = "password.dat";

        /// <summary>
        /// 設定ファイルパス
        /// </summary>
        private const string ConfigFilePath = "config.json";

        /// <summary>
        /// VPN接続アイコンのパス
        /// </summary>
        public const string VpnConnectIconPath = @"Image\VPNConnected.ico";

        /// <summary>
        /// VPN未接続アイコンのパス
        /// </summary>
        public const string VpnDisconnectIconPath = @"Image\VPNDisconnected.ico";

        /// <summary>
        /// 設定ファイルを読み込む。存在しない場合はデフォルト設定を作成。
        /// </summary>
        public static void LoadConfig()
        {
            // 設定ファイルが存在しない場合
            if (!File.Exists(ConfigFilePath))
            {
                CreateDefaultConfig();
                MessageBox.Show($"初回起動のため、設定ファイル {ConfigFilePath} を作成しました。\n必要に応じて編集してください。",
                                "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                // 設定ファイルを読み込む
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(ConfigFilePath));

                // 設定値をプロパティに割り当てる
                VpnName = config?.GetValueOrDefault(nameof(VpnName), VpnName) ?? VpnName;
                UserName = config?.GetValueOrDefault(nameof(UserName), UserName) ?? UserName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定ファイルの読み込みに失敗しました。\nエラー: {ex.Message}",
                                "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1); // アプリケーションを終了
            }
        }

        /// <summary>
        /// デフォルトの設定ファイルを作成する
        /// </summary>
        private static void CreateDefaultConfig()
        {
            var defaultConfig = new
            {
                VpnName,
                UserName
            };

            // 設定ファイルを作成
            File.WriteAllText(ConfigFilePath, JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
