using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections;

//Lina was here!

namespace RunningGame
{
    /*
     * Game is basically what sets everything up.
     * Holds Start Menu, could ask about any level modifiers etc
     * Eventually it starts the level.
    */
    [Serializable()]
    class Game
    {
        private int winWidth;
        private int winHeight;

        private Level currentLevel;
        private bool gameRunning;
        
        [NonSerialized] private Graphics mainWinGraphics;
        [NonSerialized] private Brush backBrush = Brushes.Wheat; //Backcolor for the game
        
        [NonSerialized] private Bitmap bufferImage;
        [NonSerialized] private Graphics dbGraphics;

        public Game(Graphics winGraphics, int w, int h)
        {
            mainWinGraphics = winGraphics;
            winWidth = w;
            winHeight = h;

            gameRunning = true;

            currentLevel = null;

            bufferImage = new Bitmap(w, h);
            dbGraphics = Graphics.FromImage(bufferImage);


            startLevel();


            Thread gameThread = new Thread(new ThreadStart(Update));
            gameThread.Start();
        }


        public void startLevel()
        {
            currentLevel = new Level(winWidth, winHeight, "RunningGame.Resources.TestLevelMidLength.png", true, dbGraphics);

            //currentLevel = new Level(winWidth, winHeight, "DoorSwitchTest3.bin", false, dbGraphics);
        }


        void Update()
        {

            while (gameRunning)
            {
                currentLevel.Update();
                Draw(); //Draw everything.
            }

        }

        public void Draw()
        {
            //Clear the previous frame
            dbGraphics.FillRectangle(backBrush, new Rectangle(0, 0, winWidth, winHeight));
            
            //If there is a level in progress
            if (currentLevel != null)
            {
                currentLevel.Draw(dbGraphics);
            }
            else
            {
                //Draw stuff for game menu.
            }

            try
            {
                mainWinGraphics.DrawImage(bufferImage, new Point(0, 0));
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught Exception: " + e);
            }

        }

        public void KeyUp(KeyEventArgs e)
        {
            currentLevel.KeyUp(e);
        }
        public void KeyDown(KeyEventArgs e)
        {
            currentLevel.KeyDown(e);
        }
        public void KeyPressed(KeyPressEventArgs e)
        {
            currentLevel.KeyPressed(e);
        }
        public void MouseClick(MouseEventArgs e)
        {
            currentLevel.MouseClick(e);
        }

        //Called when the window is closed
        public void close()
        {
            gameRunning = false;
        }
    }
}
