using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunningGame
{
    public partial class FormSpring : Form
    {

        Game game;

        const int CLIENT_WIDTH = 640;
        const int CLIENT_HEIGHT = 480;

        public FormSpring()
        {
            InitializeComponent();
        }

        private void FormRunningGame_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.ClientSize = new Size(CLIENT_WIDTH, CLIENT_HEIGHT);
            Graphics g = this.CreateGraphics();            
        }

        private void FormRunningGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            game.close();
        }

        private void FormRunningGame_KeyUp(object sender, KeyEventArgs e)
        {
            game.KeyUp(e);
        }

        private void FormRunningGame_KeyPress(object sender, KeyPressEventArgs e)
        {
            game.KeyPressed(e);
        }

        private void FormRunningGame_KeyDown(object sender, KeyEventArgs e)
        {
            game.KeyDown(e);
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            btnBegin.Enabled = false;
            btnBegin.Visible = false;
            lblLoading.Text = "Loading...";
            this.Refresh();
            //Use this.Width and this.Height instead of ClientSize to reduce streaching at edge
            game = new Game(this.CreateGraphics(), this.ClientSize.Width, this.ClientSize.Height);
            lblLoading.Visible = false;
        }

        private void FormSpring_MouseClick(object sender, MouseEventArgs e)
        {
            if(game != null)
                game.MouseClick(e);
        }

    }
}
