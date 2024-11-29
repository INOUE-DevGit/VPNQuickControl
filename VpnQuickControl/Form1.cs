using System;
using System.Drawing;
using System.Windows.Forms;

namespace VpnQuickControl
{
    public partial class Form1 : Form
    {
        private bool isVpnConnected = false; // VPN�ڑ����

        public Form1()
        {
            InitializeComponent();
            UpdateStatus(); // ������Ԃ̍X�V
        }

        private void UpdateStatus()
        {
            // �^�X�N�o�[�̃A�C�R���ƃE�B���h�E�^�C�g������Ԃɉ����ĕύX
            if (isVpnConnected)
            {
                this.Text = "VPN�ڑ��ς�";
                this.Icon = new Icon(@"Image\VPNConnected.ico"); // �ڑ����̃A�C�R��
            }
            else
            {
                this.Text = "VPN���ڑ�";
                this.Icon = new Icon(@"Image\VPNDisconnected.ico"); // ���ڑ��̃A�C�R��
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            isVpnConnected = true;
            UpdateStatus();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            isVpnConnected = false;
            UpdateStatus();
        }
    }
}
