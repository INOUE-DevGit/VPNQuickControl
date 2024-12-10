namespace VpnQuickControl
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // ThreadExceptionイベント・ハンドラを登録する
            Application.ThreadException += new
              ThreadExceptionEventHandler(Application_ThreadException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 設定ファイルの読み込み
            Config.LoadConfig();

            // パスワードファイルの存在確認
            CheckAndCreatePasswordFile();

            Application.Run(new MainWindow());
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
                    MessageBox.Show("パスワードが保存されました。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("パスワードが入力されていません。アプリケーションを終了します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0); // アプリケーションを終了
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
            ShowErrorMessage(e.Exception, "Application_ThreadExceptionによる例外通知です。");
        }

        public static void ShowErrorMessage(Exception ex, string extraMessage)
        {
            MessageBox.Show(extraMessage + " \n――――――――\n\n" +
              "エラーが発生しました。\n\n" +
              "【エラー内容】\n" + ex.Message + "\n\n" +
              "【スタックトレース】\n" + ex.StackTrace);
        }
    }
}
