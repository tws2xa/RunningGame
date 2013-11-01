using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace RunningGame
{

    public partial class FormSpring : Form
    {

        Game game;

        delegate void resetDelegate();

        const int CLIENT_WIDTH = 640;
        const int CLIENT_HEIGHT = 480;

        Image bkgImg;

        public ArrayList downKeys = new ArrayList();

        public FormSpring()
        {
            InitializeComponent();
        }

        private void FormRunningGame_Load(object sender, EventArgs e)
        {

            bkgImg = this.BackgroundImage;

            this.DoubleBuffered = true;
            //this.ClientSize = new Size(CLIENT_WIDTH, CLIENT_HEIGHT);
            Graphics g = this.CreateGraphics();

            showHideLevelButtons(false);
            showHideWorldButtons(false);

            //btnBegin.Visible = false;
            //btnBegin.Enabled = false;


        }

        private void FormRunningGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(game != null)
                game.close();
        }

        private void FormRunningGame_KeyUp(object sender, KeyEventArgs e)
        {
            if (game != null)
            {
                game.KeyUp(e);
                if (downKeys.Contains(e.KeyData))
                    downKeys.Remove(e.KeyData);
            }
        }

        private void FormRunningGame_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(game!=null)
                game.KeyPressed(e);
        }

        private void FormRunningGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (game != null)
            {
                if (!downKeys.Contains(e.KeyData))
                {
                    game.KeyDown(e);
                    downKeys.Add(e.KeyData);
                }
            }
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            loadLevel();
        }

        private void loadLevel()
        {
            //Hide all buttons!
            foreach (Control c in this.Controls)
            {
                if (c is Button)
                {
                    c.Enabled = false;
                    c.Visible = false;
                }
            }
            this.BackgroundImage = null;
            lblLoading.Text = "Loading...";
            this.Refresh();
            //Use this.Width and this.Height instead of ClientSize to reduce streaching at edge
            game = new Game(this.CreateGraphics(), this.ClientSize.Width, this.ClientSize.Height, "", this);
            lblLoading.Visible = false;
        }

        private void loadLevel(string str)
        {
            //Hide all buttons!
            foreach (Control c in this.Controls)
            {
                if (c is Button)
                {
                    c.Enabled = false;
                    c.Visible = false;
                }
            }
            this.BackgroundImage = null;
            lblLoading.Text = "Loading...";
            this.Refresh();
            //Use this.Width and this.Height instead of ClientSize to reduce streaching at edge
            game = new Game(this.CreateGraphics(), this.ClientSize.Width, this.ClientSize.Height, str, this);
            lblLoading.Visible = false;
        }

        private void FormSpring_MouseClick(object sender, MouseEventArgs e)
        {
            if(game != null)
                game.MouseClick(e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FormEditor frmEdit = new FormEditor();
            frmEdit.Show();
        }

        private void btnWorld1_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 1;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        private void btnWorld2_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 2;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        private void btnWorld3_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 3;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        private void btnWorld4_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 4;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        private void btnWorld5_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 5;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        private void showHideLevelButtons(bool show)
        {
            btnLvl1.Visible = show;
            btnLvl1.Enabled = show;

            btnLvl2.Visible = show;
            btnLvl2.Enabled = show;

            btnLvl3.Visible = show;
            btnLvl3.Enabled = show;

            btnLvlReturn.Visible = show;
            btnLvlReturn.Enabled = show;
        }

        private void showHideWorldButtons(bool show)
        {
            btnWorld1.Visible = show;
            btnWorld1.Enabled = show;

            btnWorld2.Visible = show;
            btnWorld2.Enabled = show;

            btnWorld3.Visible = show;
            btnWorld3.Enabled = show;

            btnWorld4.Visible = show;
            btnWorld4.Enabled = show;

            btnWorld5.Visible = show;
            btnWorld5.Enabled = show;
        }
            
        private void btnLevel1_Click(object sender, EventArgs e)
        {
            switch (GlobalVars.worldNum)
            {
                case(0):
                    Console.WriteLine("Error: Selecting level 1 in world 0");
                    break;
                case (1):
                    loadLevel("RunningGame.Resources.Levels.EnemyLevel4.png"); //World 1 Level 1
                    break;
                case (2):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevel.png"); //World 2 Level 1
                    break;
                case (3):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 3 Level 1
                    break;
                case (4):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 4 Level 1
                    break;
                case (5):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 5 Level 1
                    break;
            }
        }


        private void btnLvl2_Click(object sender, EventArgs e)
        {
            switch (GlobalVars.worldNum)
            {
                case (0):
                    Console.WriteLine("Error: Selecting level 2 in world 0");
                    break;
                case (1):
                    loadLevel("RunningGame.Resources.Levels.EnemyLevel4.png"); //World 1 Level 2
                    break;
                case (2):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevel.png"); //World 2 Level 2
                    break;
                case (3):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 3 Level 2
                    break;
                case (4):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 4 Level 2
                    break;
                case (5):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 5 Level 2
                    break;
            }
        }

        private void btnLvl3_Click(object sender, EventArgs e)
        {
            switch (GlobalVars.worldNum)
            {
                case (0):
                    Console.WriteLine("Error: Selecting level 1 in world 0");
                    break;
                case (1):
                    loadLevel("RunningGame.Resources.Levels.EnemyLevel4.png"); //World 1 Level 3
                    break;
                case (2):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevel.png"); //World 2 Level 3
                    break;
                case (3):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 3 Level 3
                    break;
                case (4):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 4 Level 3
                    break;
                case (5):
                    loadLevel("RunningGame.Resources.Levels.PresentationLevelExtended.png"); //World 5 Level 3
                    break;
            }
        }

        private void btnLvlReturn_Click(object sender, EventArgs e)
        {
            showHideLevelButtons(false);
            showHideWorldButtons(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnPlay.Visible = false;
            btnPlay.Enabled = false;
            showHideWorldButtons(true);
        }



        public void Reset()
        {
            
            //Get on the proper thread
            if (InvokeRequired)
            {
                Invoke(new resetDelegate(Reset));
            }
            else
            {
                showHideLevelButtons(false);
                showHideWorldButtons(false);
                this.BackgroundImage = bkgImg;
                this.btnPlay.Enabled = true;
                this.btnPlay.Visible = true;
                game.close();
                game = null;
            }
        }
    }
}
