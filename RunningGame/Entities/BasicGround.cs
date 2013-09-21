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

        string dirtSpriteName = "dirt";
        string grassSpriteName = "grass";

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
                DrawComponent drawComp = new DrawComponent("RunningGame.Resources.DirtSquare.bmp", dirtSpriteName, defaultWidth, defaultHeight, true);
                drawComp.addImage("RunningGame.Resources.GrassSquare.bmp", grassSpriteName);
                addComponent(drawComp);

                //Collider
                addComponent(new ColliderComponent(this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE));

            }

        
            public override void revertToStartingState()
            {
                // Do nothing. Ground does not change in game.
            }
        
            public void changeSprite(bool dirt)
            {
                DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

                if (dirt)
                {
                    drawComp.setSprite(dirtSpriteName);
                }
                else
                {
                    drawComp.setSprite(grassSpriteName);
                }
            }
    }
}
