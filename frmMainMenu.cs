using helicopter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace helicopter
{
    public partial class frmMainMenu : Form
    {
        public frmMainMenu()
        {
            InitializeComponent();
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmGame frmGame = new frmGame();
            frmGame.Show();
        }

        private void btnCredits_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Developed by Ben Davies \nhttps://github.com/bxdavies/helicopter");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
