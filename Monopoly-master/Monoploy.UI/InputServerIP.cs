using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Monoploy.UI
{
    public partial class InputServerIP : Form
    {
        private ServerGameForm prevForm;
        private Color playerColor;

        public InputServerIP(ServerGameForm serverGameForm)
        {
            InitializeComponent();
            connectBtn.Enabled = false;
            prevForm = serverGameForm;
            CenterToParent();
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            string ipAddress;

            if (ipAddressTxt.Text != string.Empty)
            {
                ipAddress = ipAddressTxt.Text;
                prevForm.setIpAddress(ipAddress);
                prevForm.setPlayerColor(playerColor);
                Close();
            }
            else
                MessageBox.Show("Please enter a ip address.");
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();

            connectBtn.Enabled = true;
            if (colorDialog.Color == null)
                playerColor = Color.MediumVioletRed;
            else
                playerColor = colorDialog.Color;
        }

        private void hostButton_Click(object sender, EventArgs e)
        {
            prevForm.StartServer();
        }
    }
}
