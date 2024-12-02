namespace VpnQuickControl
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // ThreadException�C�x���g�E�n���h����o�^����
            Application.ThreadException += new
              ThreadExceptionEventHandler(Application_ThreadException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // �ݒ�t�@�C���̓ǂݍ���
            Config.LoadConfig();

            // �p�X���[�h�t�@�C���̑��݊m�F
            CheckAndCreatePasswordFile();

            Application.Run(new MainWindow());
        }

        private static void CheckAndCreatePasswordFile()
        {
            // �t�@�C�������݂��Ȃ��ꍇ�A�p�X���[�h���쐬
            if (!File.Exists("password.dat"))
            {
                MessageBox.Show("����N���̂��߁AVPN�p�X���[�h�̐ݒ肪�K�v�ł��B", "�p�X���[�h�ݒ�", MessageBoxButtons.OK, MessageBoxIcon.Information);

                string inputPassword = Microsoft.VisualBasic.Interaction.InputBox("VPN�̃p�X���[�h����͂��Ă�������:", "�p�X���[�h�ݒ�", "");
                if (!string.IsNullOrWhiteSpace(inputPassword))
                {
                    SaveEncryptedPassword(inputPassword);
                    MessageBox.Show("�p�X���[�h���ۑ�����܂����B", "����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("�p�X���[�h�����͂���Ă��܂���B�A�v���P�[�V�������I�����܂��B", "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0); // �A�v���P�[�V�������I��
                }
            }
        }

        private static void SaveEncryptedPassword(string plainTextPassword)
        {
            // �p�X���[�h���Í������ĕۑ�
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(plainTextPassword);
            byte[] encryptedBytes = System.Security.Cryptography.ProtectedData.Protect(passwordBytes, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            File.WriteAllText("password.dat", Convert.ToBase64String(encryptedBytes));
        }

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowErrorMessage(e.Exception, "Application_ThreadException�ɂ���O�ʒm�ł��B");
        }

        public static void ShowErrorMessage(Exception ex, string extraMessage)
        {
            MessageBox.Show(extraMessage + " \n�\�\�\�\�\�\�\�\\n\n" +
              "�G���[���������܂����B\n\n" +
              "�y�G���[���e�z\n" + ex.Message + "\n\n" +
              "�y�X�^�b�N�g���[�X�z\n" + ex.StackTrace);
        }
    }
}
