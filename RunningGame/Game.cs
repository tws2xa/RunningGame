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

namespace RunningGame {
    
    /*
     * Game is basically what sets everything up.
     * Eventually it starts the level.
     * 
    */
    [Serializable()]
    public class Game {
        private int winWidth;
        private int winHeight;

        private Level currentLevel;
        private bool gameRunning;

        Point zeroPoint = new Point( 0, 0 );

        private FormSpring frm; //Form it's being run on.
        Font displayFont = SystemFonts.DefaultFont;

        [NonSerialized]
        private Graphics mainWinGraphics; //Graphcis object attached to the main window
        [NonSerialized]
        private Brush backBrush = Brushes.Wheat; //Backcolor for the game

        [NonSerialized]
        private Bitmap bufferImage; //Image used for double buffering
        [NonSerialized]
        private Graphics dbGraphics; //Graphics attached to bufferImage

        //Start the game and load the given world/level
        public Game( Graphics winGraphics, int w, int h, string str, int world, int level, FormSpring frm, Font fnt ) {

            this.frm = frm;
            this.displayFont = fnt;

            mainWinGraphics = winGraphics;
            winWidth = w;
            winHeight = h;

            gameRunning = true;

            currentLevel = null;

            bufferImage = new Bitmap( w, h );
            dbGraphics = Graphics.FromImage( bufferImage );

            if ( str == null || str == "" )
                startLevel( world, level ); //Start debug level
            else
                startLevel( str, world, level ); //Start given level


            Thread gameThread = new Thread( new ThreadStart( Update ) );
            gameThread.Start();
        }


        public void startLevel( int world, int level ) {
            GlobalVars.groundEntities.Clear();
            GlobalVars.nonGroundEntities.Clear();
            GlobalVars.removedStartingEntities.Clear();
            //HERE IS WHERE YOU SAY WHICH LEVEL TO LOAD ON DEBUG
            //YOU GET TO IT BY PRESSING THE DEBUG BUTTON
            currentLevel = new Level( this, winWidth, winHeight, "RunningGame.Resources.Levels.DebugLevel.png", 1, 1, true, dbGraphics, displayFont );
        }

        //This first clears all lists, then loads a given level.
        public void startLevel( string str, int world, int level ) {
            GlobalVars.groundEntities.Clear();
            GlobalVars.nonGroundEntities.Clear();
            GlobalVars.removedStartingEntities.Clear();
            currentLevel = new Level( this, winWidth, winHeight, str, world, level, true, dbGraphics, displayFont );
        }

        //This is run once every frame
        void Update() {

            while ( gameRunning ) {
                //Update the level
                currentLevel.Update();


                //Check if the end of the level has been flagged
                //If so, end the level!
                if ( currentLevel.shouldEndLevel ) {
                    
                    int levelNum = currentLevel.levelNum;
                    int worldNum = currentLevel.worldNum;

                    bool escapeEnd = currentLevel.escapeEnd;

                    currentLevel.Close();
                    currentLevel = null;

                    //Check for the next level
                    if ( levelNum == GlobalVars.numLevelsPerWorld ) {
                        if ( worldNum == GlobalVars.numWorlds ) { //Game complete
                            if ( escapeEnd ) {
                                frm.Reset();
                            } else {
                                EndGame();
                            }
                        }  else //Next world, level 1
                            frm.Reset( worldNum + 1, 1 );
                    } else {
                        //Next level in same world
                        frm.Reset( worldNum, levelNum + 1 );
                    }
                    break;
                }


                Draw(); //Draw everything.
            }

        }

        public void EndGame() {
            stopAllSounds();
            playSound( "RunningGame.Resources.Sounds.ending.wav", true );
            frm.showHideEnd(true);
        }

        //Draws everything in the level, if there is an active level.
        public void Draw() {
            //If there is a level in progress
            //Draw it to the double buffer image
            if ( currentLevel != null ) {
                currentLevel.Draw( dbGraphics );
            } else {
                //Draw stuff for game menu.
            }


            bool blockColor = false;
            //hi
            if ( blockColor ) {
                Color colorToBlock = Color.FromArgb( 0, 0, 255 );

                for(int i=0; i<bufferImage.Width; i++) {
                    for(int j=0; j<bufferImage.Height; j++) {

                        Color oldCol = bufferImage.GetPixel( i, j );
                        int newR = oldCol.R;
                        int newG = oldCol.G;
                        int newB = oldCol.B;
                        if ( newR < colorToBlock.R ) newR = 0;
                        else newR -= colorToBlock.R;
                        if ( newG < colorToBlock.G ) newG = 0;
                        else newG -= colorToBlock.G;
                        if ( newB < colorToBlock.B ) newB = 0;
                        else newB -= colorToBlock.B;
                        Color newCol = Color.FromArgb( newR, newG, newB );
                        bufferImage.SetPixel( i, j, newCol );
                    }

                }

            }

            //Draw the double buffer image to the window.
            try {
                mainWinGraphics.DrawImageUnscaled( bufferImage, zeroPoint );
            } catch ( Exception e ) {
                Console.WriteLine( "Caught Exception: " + e );
            }

        }

        //Input.
        //Passes it on to the level.
        public void KeyUp( KeyEventArgs e ) {
            if(currentLevel != null)
                currentLevel.KeyUp( e );
        }
        public void KeyDown( KeyEventArgs e ) {
            if ( currentLevel != null )
                currentLevel.KeyDown( e );
        }
        public void KeyPressed( KeyPressEventArgs e ) {
            if ( currentLevel != null )
                currentLevel.KeyPressed( e );
        }
        public void MouseClick( MouseEventArgs e ) {
            if ( currentLevel != null )
                currentLevel.MouseClick( e );
        }


        public void playSound( string location, bool loop ) {
            frm.sndSystem.playSound( location, loop );
        }
        public void stopAllSounds() {
            frm.sndSystem.stopAllSounds();
        }
        public void stopSound( string location ) {
            frm.sndSystem.stopSound( location );
        }
        public bool soundPlaying( string location ) {
            return frm.sndSystem.isPlaying( location );
        }

        //Called when the window is closed
        //Stops the game from running
        public void close() {
            gameRunning = false;
        }
    }
}
