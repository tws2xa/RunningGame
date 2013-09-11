using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;

namespace RunningGame.Entities
{

    /*
     * The basic ground entity is just the normal dirt/grass
     * You see all over the test levels.
     * It's color code in the level editor is pure black: RGB(0, 0, 0)
     */

    class BasicGround:Entity
    {

        float defaultWidth = 11f;
        float defaultHeight = 11f;

        Bitmap grassSprite;
        Bitmap dirtSprite;

           public BasicGround(Level level)
           {
                this.level = level;

                initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

                addMyComponents(0, 0, defaultWidth, defaultHeight);
            }

            public BasicGround(Level level, int id)
            {
                this.level = level;

                initializeEntity(id, level);


                addMyComponents(0, 0, defaultWidth, defaultHeight);

            }

            public BasicGround(Level level, float x, float y, float width, float height)
            {
                this.level = level;

                initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);


                addMyComponents(x, y, width, height);
            }
            public BasicGround(Level level, int id, float x, float y, float width, float height)
            {
                this.level = level;

                initializeEntity(id, level);


                addMyComponents(x, y, width, height);
            }
            

            public void addMyComponents(float x, float y, float width, float height)
            {
                //Position Component
                addComponent(new PositionComponent(x, y, width, height, this));

                //Draw component
                
                //Normal Sprite
                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream myStream = myAssembly.GetManifestResourceStream("RunningGame.Resources.GrassSquare.bmp");
                Bitmap grassSprite = new Bitmap(myStream);
                
                myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                myStream = myAssembly.GetManifestResourceStream("RunningGame.Resources.DirtSquare.bmp");
                Bitmap dirtSprite = new Bitmap(myStream);
                myStream.Close();

                addComponent(new DrawComponent(dirtSprite, defaultWidth, defaultHeight, true));

                //Collider
                addComponent(new ColliderComponent(this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE));

            }

            /*
            public override Entity CopyStartingState()
            {
                PositionComponent posComp = (PositionComponent)this.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                BasicGround newEnt = new BasicGround(level, randId, posComp.x, posComp.y, posComp.width, posComp.height);
                return newEnt;
            } 
            */

        
            public override void revertToStartingState()
            {
                // Do nothing. Ground does not change in game.
            }
        
            public void changeSprite(bool dirt)
            {
                DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

                //If either is null, load it in
                if (!dirt && grassSprite == null)
                {
                    System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                    System.IO.Stream myStream = myAssembly.GetManifestResourceStream("RunningGame.Resources.GrassSquare.bmp");
                    Bitmap img = new Bitmap(myStream);
                    myStream.Close();

                    grassSprite = new Bitmap(img, new Size((int)defaultWidth, (int)defaultHeight));
                }
                if (dirt && dirtSprite == null)
                {
                    System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                    System.IO.Stream myStream = myAssembly.GetManifestResourceStream("RunningGame.Resources.DirtSquare.bmp");
                    Bitmap img = new Bitmap(myStream);
                    myStream.Close();

                    dirtSprite = new Bitmap(img, new Size((int)defaultWidth, (int)defaultHeight));
                }

                if (dirt)
                {
                    drawComp.sprite = dirtSprite;
                }
                else
                {
                    drawComp.sprite = grassSprite;
                }
            }
    }
}
