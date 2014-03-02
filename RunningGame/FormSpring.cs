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

        //The game
        Game game;

        //A delegate which is basically used to call the reset method (To do with cross threading issues)
        delegate void resetDelegate();
        delegate void resetDelegate2(int world, int num);

        //The width of the window
        const int CLIENT_WIDTH = 640;
        //The height of the window
        const int CLIENT_HEIGHT = 480;

        //The background image (Currently Laurel's Concept Art)
        Image bkgImg;

        //Sound Button Image
        Image soundOnImage = null;
        Image soundOffImage = null;

        //An array list of keys that have been pressed and not released (They're being held down)
        //This is used to prevent repeated calls of KeyPressed
        public List<Keys> downKeys = new List<Keys>();

        System.Media.SoundPlayer titleMusicPlayer;


        //When the form starts up, initialize it.
        public FormSpring()
        {
            InitializeComponent();
        }

        //Called when the form loads
        private void FormRunningGame_Load(object sender, EventArgs e)
        {
            //Set the background image
            bkgImg = this.BackgroundImage;

            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream myStreamOn = myAssembly.GetManifestResourceStream("RunningGame.Resources.Artwork.Other.SoundBtn1.png");
            System.IO.Stream myStreamOff = myAssembly.GetManifestResourceStream("RunningGame.Resources.Artwork.Other.SoundBtn2.png");

            soundOnImage = new Bitmap(myStreamOn);
            soundOffImage = new Bitmap(myStreamOff);

            myStreamOn.Close();
            myStreamOff.Close();

            //Set it to double buffered.
            this.DoubleBuffered = true;


            System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("RunningGame.Resources.Sounds.TitleTest.wav");
            titleMusicPlayer = new System.Media.SoundPlayer(stream);
            titleMusicPlayer.PlayLooping();

            addLevels();

            //Get the graphics
            Graphics g = this.CreateGraphics();

            //Hide the level and world selection buttons
            showHideLevelButtons(false);
            showHideWorldButtons(false);

            //Hide the debug begin buttons
            //btnBegin.Visible = false;
            //btnBegin.Enabled = false;

        }

        //Called when the form is closed
        private void FormRunningGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            //If the game is running, stop it.
            if (game != null)
                game.close();
        }

        //Called when the debug begin button is clicked
        private void btnBegin_Click(object sender, EventArgs e)
        {
            //Load the default level
            loadLevel();
        }

        //Load level (For default level)
        private void loadLevel()
        {
            showHideControlButtons(false);
            //Hide all buttons!
            foreach (Control c in this.Controls)
            {
                if (c is Button)
                {
                    c.Enabled = false;
                    c.Visible = false;
                }
            }

            sndToggle.Visible = false;
            sndToggle.Enabled = false;

            //Stop sound
            titleMusicPlayer.Stop();

            //Clear the background image
            this.BackgroundImage = null;

            //Write the loading text
            lblLoading.Text = "Loading...";

            //Refresh the form (So the loading text appears)
            this.Refresh();

            //Start the game
            //Use this.Width and this.Height instead of ClientSize to reduce streaching at edge
            game = new Game(this.CreateGraphics(), this.ClientSize.Width, this.ClientSize.Height, "", 1, 1, this, displayFontLbl.Font);

            //Once the game has been started - hide the loading text
            lblLoading.Visible = false;
        }

        //Called when the edit button is clicked.
        private void btnEdit_Click(object sender, EventArgs e)
        {
            //Show the editor form.
            FormEditor frmEdit = new FormEditor();
            frmEdit.Show();
        }

        //-------------------------WORLD BUTTONS-----------------------------
        //World 1
        private void btnWorld1_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 1;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        //World 2
        private void btnWorld2_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 2;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        //World 3
        private void btnWorld3_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 3;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        //World 4
        private void btnWorld4_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 4;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        //World 5
        private void btnWorld5_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 5;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        //World 6
        private void btnWorld6_Click(object sender, EventArgs e)
        {
            GlobalVars.worldNum = 6;
            showHideLevelButtons(true);
            showHideWorldButtons(false);
        }

        //----------------------------------------------------------------

        //---------------------------ADD LEVELS HERE----------------------

        private void addLevels()
        {
            string defaultLevel = "RunningGame.Resources.Levels.PresentationLevelExtended.png";
            for (int i = 0; i < GlobalVars.numWorlds; i++)
            {
                GlobalVars.levels.Add(new List<string>());
                for (int j = 0; j < GlobalVars.numLevelsPerWorld; j++)
                {
                    GlobalVars.levels[i].Add(defaultLevel);
                }
            }

            //Here is where you assign particular level images to their level in game.
            //Format: GlobalVars.levels[world # - 1][level # - 1] = "Image address"
            //Remember the indexes start at 0, so they're off by 1
            //i.e. levels[0][0] => World 1 Level 1
            GlobalVars.levels[0][0] = "RunningGame.Resources.Levels.World1Level1anna.png";
            GlobalVars.levels[0][1] = "RunningGame.Resources.Levels.World1Level2anna.png";
            GlobalVars.levels[0][2] = "RunningGame.Resources.Levels.World1Level3anna.png";

            GlobalVars.levels[1][0] = "RunningGame.Resources.Levels.World2Level1.png";
            GlobalVars.levels[1][1] = "RunningGame.Resources.Levels.World2Level2.png";
            GlobalVars.levels[1][2] = "RunningGame.Resources.Levels.World2Level31.png";

            GlobalVars.levels[2][0] = "RunningGame.Resources.Levels.World3Level1_new.png";
            GlobalVars.levels[2][1] = "RunningGame.Resources.Levels.World3Level2.png";
            GlobalVars.levels[2][2] = "RunningGame.Resources.Levels.World3Level3.png";

            GlobalVars.levels[3][0] = "RunningGame.Resources.Levels.World4Level1.png";
            GlobalVars.levels[3][1] = "RunningGame.Resources.Levels.World4Level2.png";
            GlobalVars.levels[3][2] = "RunningGame.Resources.Levels.World4Level31.png";

            GlobalVars.levels[4][0] = "RunningGame.Resources.Levels.World5Level1.png";
            GlobalVars.levels[4][1] = "RunningGame.Resources.Levels.World5Level2.png";
            GlobalVars.levels[4][2] = "RunningGame.Resources.Levels.World5level3.png";

            GlobalVars.levels[5][0] = "RunningGame.Resources.Levels.World6level1.png";
            GlobalVars.levels[5][1] = "RunningGame.Resources.Levels.World6level2.png";
            GlobalVars.levels[5][2] = "RunningGame.Resources.Levels.World6level3.png";

        }

        //----------------------------------------------------------------

        //---------------------------LEVEL BUTTONS------------------------
        //Level one
        private void btnLevel1_Click(object sender, EventArgs e)
        {
            switch (GlobalVars.worldNum)
            {
                case (0):
                    Console.WriteLine("Error: Selecting level 1 in world 0");
                    break;
                case (1):
                    loadLevel(1, 1); //World 1 Level 1
                    break;
                case (2):
                    loadLevel(2, 1); //World 2 Level 1
                    break;
                case (3):
                    loadLevel(3, 1); //World 3 Level 1
                    break;
                case (4):
                    loadLevel(4, 1); //World 4 Level 1
                    break;
                case (5):
                    loadLevel(5, 1); //World 5 Level 1
                    break;
                case (6):
                    loadLevel(6, 1); //World 6 Level 1
                    break;
            }
        }

        //Level 2
        private void btnLvl2_Click(object sender, EventArgs e)
        {
            switch (GlobalVars.worldNum)
            {
                case (0):
                    Console.WriteLine("Error: Selecting level 2 in world 0");
                    break;
                case (1):
                    loadLevel(1, 2); //World 1 Level 2
                    break;
                case (2):
                    loadLevel(2, 2); //World 2 Level 2
                    break;
                case (3):
                    loadLevel(3, 2); //World 3 Level 2
                    break;
                case (4):
                    loadLevel(4, 2); //World 4 Level 2
                    break;
                case (5):
                    loadLevel(5, 2); //World 5 Level 2
                    break;
                case (6):
                    loadLevel(6, 2); //World 6 Level 2
                    break;
            }
        }

        //Level 3
        private void btnLvl3_Click(object sender, EventArgs e)
        {
            switch (GlobalVars.worldNum)
            {
                case (0):
                    Console.WriteLine("Error: Selecting level 1 in world 0");
                    break;
                case (1):
                    loadLevel(1, 3); //World 1 Level 3
                    break;
                case (2):
                    loadLevel(2, 3); //World 2 Level 3
                    break;
                case (3):
                    loadLevel(3, 3); //World 3 Level 3
                    break;
                case (4):
                    loadLevel(4, 3); //World 4 Level 3
                    break;
                case (5):
                    loadLevel(5, 3); //World 5 Level 3
                    break;
                case (6):
                    loadLevel(6, 3); //World 6 Level 3
                    break;
            }
        }

        //Return to world select menu
        private void btnLvlReturn_Click(object sender, EventArgs e)
        {
            showHideLevelButtons(false);
            showHideWorldButtons(true);
        }
        //----------------------------------------------------------------

        //Called when the play button is clicked
        private void button1_Click(object sender, EventArgs e)
        {
            btnPlay.Visible = false;
            btnPlay.Enabled = false;
            showHideWorldButtons(true);
        }



        //Shows/hides the select level buttons
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

        //Show/hides the control buttons
        private void showHideControlButtons(bool show)
        {
            this.btnControlReturn.Visible = show;
            this.btnControlReturn.Enabled = show;
            this.lblJump.Visible = show;
            this.btnSetJump.Visible = show;
            this.btnSetJump.Enabled = show;
            this.btnSetLeft.Visible = show;
            this.btnSetLeft.Enabled = show;
            this.lblLeft.Visible = show;
            this.btnSetRight.Visible = show;
            this.btnSetRight.Enabled = show;
            this.lblRight.Visible = show; 
        }

        //Show/hides the select world buttons
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

            btnWorld6.Visible = show;
            btnWorld6.Enabled = show;
        }


        //Resets the menu
        public void Reset()
        {

            //Get on the proper thread
            if (InvokeRequired)
            {
                Invoke(new resetDelegate(Reset));
            }
            else
            {
                titleMusicPlayer.PlayLooping();

                sndToggle.Visible = true;
                sndToggle.Enabled = true;
                showHideLevelButtons(false);
                showHideWorldButtons(false);
                showHideControlButtons(false);
                this.BackgroundImage = bkgImg;
                this.btnPlay.Enabled = true;
                this.btnPlay.Visible = true;
                this.btnControls.Enabled = true;
                this.btnControls.Visible = true;
                game.close();
                game = null;
            }
        }
        public void Reset(int newWorld, int newNum)
        {

            //Get on the proper thread
            if (InvokeRequired)
            {
                resetDelegate2 reset = Reset;
                Invoke(reset, newWorld, newNum);
            }
            else
            {
                /*showHideLevelButtons(false);
                showHideWorldButtons(false);
                this.BackgroundImage = bkgImg;
                this.btnPlay.Enabled = true;
                this.btnPlay.Visible = true;*/
                game.close();
                game = null;
                loadLevel(newWorld, newNum);
            }
        }

        //Load a specific level
        private void loadLevel(int world, int level)
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

            sndToggle.Visible = false;
            sndToggle.Enabled = false;

            //Stop sound
            titleMusicPlayer.Stop();

            //Clear the background image
            this.BackgroundImage = null;

            //Show the loading text
            lblLoading.Text = "Loading...";
            lblLoading.Visible = true; //Loading sign

            //Refresh the form so you can see the loading text
            this.Refresh();

            //Start the game
            //Use this.Width and this.Height instead of ClientSize to reduce streaching at edge
            game = new Game(this.CreateGraphics(), this.ClientSize.Width, this.ClientSize.Height, GlobalVars.levels[world - 1][level - 1], world, level, this, displayFontLbl.Font);

            //Once the game is started, hide the loading text
            lblLoading.Visible = false;
        }


        //-----------------------------------------INPUT-----------------------------------------------

        //Called when a key is released
        private void FormRunningGame_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            //If the game is running...
            if (game != null)
            {
                //Let the game know that the key was released
                game.KeyUp(e);
                //If the key is contained in downKeys, remove it. (It is no longer down)
                if (downKeys.Contains(e.KeyData))
                    downKeys.Remove(e.KeyData);
            }
        }

        //Called when a key is held down
        private void FormRunningGame_KeyPress(object sender, KeyPressEventArgs e)
        {
            //If the game is running, tell it that the key was pressed
            if (game != null)
            {
                game.KeyPressed(e);

                e.Handled = true;
            }
        }

        //Called when a key is first pushed
        private void FormRunningGame_KeyDown(object sender, KeyEventArgs e)
        {

            e.Handled = true;
            e.SuppressKeyPress = true;

            //If the game is running...
            if (game != null)
            {
                //If it hasn;t already been registered as pressed
                if (!downKeys.Contains(e.KeyData))
                {
                    //Tell the game it was pressed
                    game.KeyDown(e);
                    //Add it to the list of pressed keys
                    downKeys.Add(e.KeyData);

                }
            }


        }

        //Called when the mouse is clicked in the form
        private void FormSpring_MouseClick(object sender, MouseEventArgs e)
        {
            //If the game is running, tell the game that the mouse was clicked
            if (game != null)
                game.MouseClick(e);
        }

        private void btnControls_Click(object sender, EventArgs e)
        {
            btnControls.Visible = false;
            btnControls.Enabled = false;
            showHideControlButtons(true);
        }

        private void btnControlReturn_Click(object sender, EventArgs e)
        {
            showHideControlButtons(false);
            btnControls.Visible = true;
            btnControls.Enabled = true;
        }

        private void displayFontLbl_Click(object sender, EventArgs e)
        {

        }

        private void lblJump_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            btnSetJump.Text = "";
            GlobalVars.reservedKeys.Remove(GlobalVars.KEY_JUMP);
            btnControlReturn.Enabled = false;
        }

        private void btnSetJump_KeyDown(object sender, KeyEventArgs e)
        {
            if (!GlobalVars.reservedKeys.Contains(e.KeyData))
            {
                GlobalVars.reservedKeys.Add(e.KeyData);
                btnSetJump.Text = Convert.ToString(e.KeyData);
                GlobalVars.KEY_JUMP = e.KeyData;
                btnControlReturn.Enabled = true;
            }
        }


        private void sndToggle_Click(object sender, EventArgs e)
        {

            if (game == null)
            {
                GlobalVars.soundOn = !GlobalVars.soundOn;

                if (GlobalVars.soundOn)
                {
                    sndToggle.Image = soundOnImage;
                    titleMusicPlayer.PlayLooping();
                }
                else
                {
                    sndToggle.Image = soundOffImage;
                    titleMusicPlayer.Stop();
                }
            }

        }

        private void btnSetLeft_Click(object sender, EventArgs e)
        {
            btnSetLeft.Text = "";
            GlobalVars.reservedKeys.Remove(GlobalVars.KEY_LEFT);
            btnControlReturn.Enabled = false;
        }

        private void btnSetLeft_KeyDown(object sender, KeyEventArgs e)
        {
      if (!GlobalVars.reservedKeys.Contains(e.KeyData))
            {
                GlobalVars.reservedKeys.Add(e.KeyData);
                btnSetLeft.Text = Convert.ToString(e.KeyData);
                GlobalVars.KEY_LEFT = e.KeyData;
                btnControlReturn.Enabled = true;
        }

        //---------------------------------------------------------------------------------------------

    }

        private void lblLeft_Click(object sender, EventArgs e)
        {

        }

        private void btnSetRight_Click(object sender, EventArgs e)
        {
            btnSetRight.Text = "";
            GlobalVars.reservedKeys.Remove(GlobalVars.KEY_RIGHT);
            btnControlReturn.Enabled = false;
        }

        private void btnSetRight_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.KeyData.Equals(GlobalVars.reservedKeys))
            {
                btnSetRight.Text = Convert.ToString(e.KeyData);
                GlobalVars.reservedKeys.Add(e.KeyData);
                GlobalVars.KEY_RIGHT = e.KeyData;
                btnControlReturn.Enabled = true;
            }
        }
    }
}
