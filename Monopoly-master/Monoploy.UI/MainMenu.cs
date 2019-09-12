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
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void LocalBtn_Click(object sender, EventArgs e)
        {
            using (LocalGameForm localGame = new LocalGameForm())
            {
                localGame.ShowDialog();
                Hide();
            }

            Show();
        }

        private void ServerBtn_Click(object sender, EventArgs e)
        {
            Hide();

            ServerGameForm serverGame = new ServerGameForm(this);
        }


        private void MainMenu_Load(object sender, EventArgs e)
        {

        }
    }
}
