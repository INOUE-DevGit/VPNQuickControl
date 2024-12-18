namespace VPNQuickControl
{
    internal static class Program
    {
        private static Mutex? appMutex;

        [STAThread]
        static void Main()
        {
            // Mutexを使って二重起動を防ぐ
            appMutex = new Mutex(true, "VpnQuickControlAppMutex", out bool isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show("アプリケーションは既に起動しています。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // ThreadExceptionイベント・ハンドラを登録する
                Application.ThreadException += Application_ThreadException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // 設定ファイルの読み込み
                Config.LoadConfig();

                // パスワードファイルの存在確認
                CheckAndCreatePasswordFile();

                Application.Run(new MainWindow());
            }
            finally
            {
                // アプリケーション終了時にMutexを解放
                appMutex.ReleaseMutex();
            }
        }

        private static void CheckAndCreatePasswordFile()
        {
            // ファイルが存在しない場合、パスワードを作成
            if (!File.Exists(Config.PasswordFilePath))
            {
                MessageBox.Show("初回起動のため、VPNパスワードの設定が必要です。", "パスワード設定", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string inputPassword = Microsoft.VisualBasic.Interaction.InputBox("VPNのパスワードを入力してください:", "パスワード設定", "");
                if (!string.IsNullOrWhiteSpace(inputPassword))
                {
                    SaveEncryptedPassword(inputPassword);
                    MessageBox.Show($"パスワードが保存されました。{Environment.NewLine}{Environment.NewLine}" +
                                    $"{Path.Combine(Directory.GetCurrentDirectory(), Config.PasswordFilePath)}",
                                    "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("パスワードが入力されていません。アプリケーションを終了します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
        }

        private static void SaveEncryptedPassword(string plainTextPassword)
        {
            // パスワードを暗号化して保存
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(plainTextPassword);
            byte[] encryptedBytes = System.Security.Cryptography.ProtectedData.Protect(passwordBytes, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            File.WriteAllText(Config.PasswordFilePath, Convert.ToBase64String(encryptedBytes));
        }

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"Application_ThreadExceptionによる例外通知です。{Environment.NewLine}――――――――{Environment.NewLine}" +
              $"エラーが発生しました。{Environment.NewLine}" +
              $"【エラー内容】{Environment.NewLine}" + e.Exception.Message + Environment.NewLine +
              $"【スタックトレース】{Environment.NewLine}" + e.Exception.StackTrace);
        }
    }
}
