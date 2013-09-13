using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RunningGame.Entities;
using RunningGame.Components;
using System.Collections;

namespace RunningGame
{

    /*
     * This class reads in a level image, and adds the
     * appropriate entities into the game world.
     */

    class LevelImageReader
    {

        Level level;
        Bitmap img;

        Color playerCol = Color.FromArgb(0, 0, 255); //Player is blue
        Color basicGroundCol = Color.FromArgb(0, 0, 0); //Basic Ground is black
        Color testEntityColor = Color.FromArgb(42, 42, 42);

        Random rand = new Random();

        float tileWidth = GlobalVars.LEVEL_READER_TILE_WIDTH;
        float tileHeight = GlobalVars.LEVEL_READER_TILE_HEIGHT;

        public LevelImageReader(Level level, Bitmap img)
        {

            this.level = level;
            this.img = img;


            level.levelWidth = (img.Width)*tileWidth;
            level.levelHeight = (img.Height)*tileHeight;
        }


        public void readImage()
        {
            //Console.WriteLine(img.Width + ", " + img.Height);
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {

                    Color col = img.GetPixel(x, y);

                    float levelX = x+0.5f;
                    float levelY = y+0.5f;

                    if (col == playerCol)
                    {
                        Player player = new Player(level, rand.Next(Int32.MinValue, Int32.MaxValue), levelX*tileWidth, levelY*tileHeight);
                        player.isStartingEntity = true;
                        level.addEntity(player.randId, player);
                    }
                    else if (col == basicGroundCol)
                    {

                        float groundX = (levelX) * tileWidth;
                        float groundWidth = tileWidth; 

                        
                        BasicGround ground = new BasicGround(level, rand.Next(Int32.MinValue, Int32.MaxValue), groundX, (levelY)*tileHeight, groundWidth, tileHeight);
                        ground.isStartingEntity = true;
                        level.addEntity(ground.randId, ground);
                        
                        //If no ground above it, change to a grass sprite
                        ArrayList above = level.getCollisionSystem().findObjectAtPoint((levelX) * tileWidth, (levelY - 1) * tileWidth);
                        if (above.Count <= 0 || !(above[0] is BasicGround))
                        {
                            ground.changeSprite(false);
                        }
                        
                    }
                    else if (col == testEntityColor)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);
                        //Console.WriteLine("Creating Test Entity: " + id);
                        TestEntity test = new TestEntity(level, id, xLoc, yLoc);
                        test.isStartingEntity = true;
                        level.addEntity(test.randId, test);
                    }
                }
            }

        }

    }
}
