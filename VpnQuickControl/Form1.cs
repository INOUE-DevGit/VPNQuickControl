namespace VpnQuickControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeNotifyIcon(); // �ʒm�A�C�R���̏�����
        }

        private bool isVpnConnected = false; // VPN�ڑ����

        private void InitializeNotifyIcon()
        {
            notifyIcon.Icon = new Icon("disconnected.ico"); // �ŏ��̃A�C�R��
            notifyIcon.Visible = true;
            notifyIcon.Text = "VPN���ڑ�";

            // �ʒm�A�C�R�����_�u���N���b�N��VPN�ڑ�/�ؒf��؂�ւ���
            notifyIcon.DoubleClick += (sender, e) => ToggleVpnConnection();
        }

        private void ToggleVpnConnection()
        {
            if (isVpnConnected)
            {
                DisconnectVpn();
            }
            else
            {
                ConnectVpn();
            }
        }

        private void ConnectVpn()
        {
            isVpnConnected = true;
            lblStatus.Text = "VPN�ڑ��ς�";
            notifyIcon.Icon = new Icon("connected.ico"); // �ڑ��A�C�R���ɕύX
            notifyIcon.Text = "VPN�ڑ��ς�";
            MessageBox.Show("VPN�ɐڑ����܂����B");
        }

        private void DisconnectVpn()
        {
            isVpnConnected = false;
            lblStatus.Text = "VPN���ڑ�";
            notifyIcon.Icon = new Icon("disconnected.ico"); // �ؒf�A�C�R���ɕύX
            notifyIcon.Text = "VPN���ڑ�";
            MessageBox.Show("VPN��ؒf���܂����B");
        }

        // �ڑ��{�^��
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectVpn();
        }

        // �ؒf�{�^��
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectVpn();
        }
    }
}
