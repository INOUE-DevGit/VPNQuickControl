using System.Text.Json;

namespace VPNQuickControl
{
    public static class Config
    {
        /// <summary>
        /// VPN接続名
        /// </summary>
        public static string VPNName { get; private set; } = "VPNName";
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
        public const string VPNConnectIconPath = @"Image\VPNConnected.ico";

        /// <summary>
        /// VPN未接続アイコンのパス
        /// </summary>
        public const string VPNDisconnectIconPath = @"Image\VPNDisconnected.ico";

        /// <summary>
        /// 設定ファイルを読み込む。存在しない場合はデフォルト設定を作成。
        /// </summary>
        public static void LoadConfig()
        {
            // 設定ファイルが存在しない場合
            if (!File.Exists(ConfigFilePath))
            {
                CreateDefaultConfig();
                MessageBox.Show($"初回起動のため、設定ファイル {ConfigFilePath} を作成しました。{Environment.NewLine}" +
                                $"必要に応じて編集してください。{Environment.NewLine}{Environment.NewLine}" +
                                $"{Path.Combine(Directory.GetCurrentDirectory(), ConfigFilePath)}",
                                "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                // 設定ファイルを読み込む
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(ConfigFilePath));

                // 設定値をプロパティに割り当てる
                VPNName = config?.GetValueOrDefault(nameof(VPNName), VPNName) ?? VPNName;
                UserName = config?.GetValueOrDefault(nameof(UserName), UserName) ?? UserName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定ファイルの読み込みに失敗しました。{Environment.NewLine}エラー: {ex.Message}",
                                "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// デフォルトの設定ファイルを作成する
        /// </summary>
        private static void CreateDefaultConfig()
        {
            var defaultConfig = new
            {
                VPNName,
                UserName
            };

            // 設定ファイルを作成
            File.WriteAllText(ConfigFilePath, JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
