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
    class Game
    {
        private int winWidth;
        private int winHeight;

        private Level currentLevel;
        private bool gameRunning;
        
        private Graphics mainWinGraphics;
        private Brush backBrush = Brushes.Wheat; //Backcolor for the game
        
        private Bitmap bufferImage;
        private Graphics dbGraphics;

        private ArrayList downKeys = new ArrayList(); //Used to prevent repeated calls when a key is held in keyDown

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
            currentLevel = new Level(winWidth, winHeight, "RunningGame.Resources.TestEntityLevel.png", dbGraphics);
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

            if (downKeys.Contains(e.KeyData)) downKeys.Remove(e.KeyData); //Remove key if it is in the downKeys list

            currentLevel.KeyUp(e);
        }
        public void KeyDown(KeyEventArgs e)
        {

            if (downKeys.Contains(e.KeyData)) return; //If it was down before and hasn't been released (it's held down) don't call again.

            //Otherwise add it to the list of down keys
            downKeys.Add(e.KeyData);

            currentLevel.KeyDown(e);
        }
        public void KeyPressed(KeyPressEventArgs e)
        {
            currentLevel.KeyPressed(e);
        }


        //Called when the window is closed
        public void close()
        {
            gameRunning = false;
        }
    }
}
