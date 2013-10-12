using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace RunningGame.Level_Editor
{
    class CreationGame
    {
        private int pnlWidth;
        private int pnlHeight;

        private CreationLevel currentLevel;
        private bool gameRunning;
        
        private Graphics mainPnlGraphics;
        private Brush backBrush = Brushes.Wheat; //Backcolor for the game
        
        private Bitmap bufferImage;
        private Graphics dbGraphics;

        public CreationGame(Graphics winGraphics, int w, int h)
        {
            mainPnlGraphics = winGraphics;
            pnlWidth = w;
            pnlHeight = h;

            gameRunning = true;

            currentLevel = null;

            bufferImage = new Bitmap(w, h);
            dbGraphics = Graphics.FromImage(bufferImage);

            
            startLevel();


            Thread gameThread = new Thread(new ThreadStart(Update));
            gameThread.Start();
        }

        public CreationGame(Graphics winGraphics, int w, int h ,string fileName)
        {
            mainPnlGraphics = winGraphics;
            pnlWidth = w;
            pnlHeight = h;

            gameRunning = true;

            currentLevel = null;

            bufferImage = new Bitmap(w, h);
            dbGraphics = Graphics.FromImage(bufferImage);


            startLevel(fileName);


            Thread gameThread = new Thread(new ThreadStart(Update));
            gameThread.Start();
        }

        public void startLevel()
        {
            currentLevel = new CreationLevel(pnlWidth, pnlHeight, pnlWidth, pnlHeight, dbGraphics);
        }
        public void startLevel(string fileName)
        {
            currentLevel = new CreationLevel(pnlWidth, pnlHeight, fileName, dbGraphics);
        }

        /*
        public void loadFromPaint(string fileName)
        {
            bufferImage = new Bitmap((int)currentLevel.levelWidth, (int)currentLevel.levelHeight);
            dbGraphics = Graphics.FromImage(bufferImage);
            currentLevel.loadFromPaint(fileName, dbGraphics);
            currentLevel.g = dbGraphics;
        }
         * */

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
            dbGraphics.FillRectangle(backBrush, new Rectangle(0, 0, pnlWidth, pnlHeight));
            
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
                mainPnlGraphics.DrawImage(bufferImage, new Point(0, 0));
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
        public void MouseMoved(MouseEventArgs e)
        {
            currentLevel.MouseMoved(e);
        }
        public void MouseLeave(EventArgs e)
        {
            currentLevel.MouseLeave(e);
        }

        //Called when the window is closed
        public void close()
        {
            gameRunning = false;
        }

        public CreationLevel getCurrentLevel()
        {
            return currentLevel;
        }
    }
}
