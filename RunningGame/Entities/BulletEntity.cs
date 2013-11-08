using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{
    [Serializable()]
    public class BulletEntity:Entity
    {

        public float defaultWidth = 15;
        public float defaultHeight = 15;

        //-------------------------------------------Constructors--------------------------------------------
        public BulletEntity(Level level, float x, float y)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            addMyComponents(x, y, 0, 0);
        }
        public BulletEntity(Level level, int id, float x, float y)
        {
            this.level = level;

            initializeEntity(id, level);

            addMyComponents(x, y, 0, 0);
        }

        //Can take in a velocity as well
        public BulletEntity(Level level, float x, float y, float velX, float velY)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            addMyComponents(x, y, velX, velY);
        }
        public BulletEntity(Level level, int id, float x, float y, float velX, float velY)
        {
            this.level = level;

            initializeEntity(id, level);

            addMyComponents(x, y, velX, velY);
        }

        //------------------------------------------------------------------------------------------------------------------

        public void addMyComponents(float x, float y, float velX, float velY)
        {

            this.updateOutOfView = true;

            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, true));
            drawComp.addSprite("RunningGame.Resources.PaintBlobSimple.png", "Main");
            drawComp.setSprite("Main");

            /* ANIMATION COMPONENT - Does it need animating?
             */
            addComponent(new AnimationComponent(0.0005f));

            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent(new VelocityComponent(velX, velY));

            /*COLLIDER - Does it hit things?
             */
            addComponent(new ColliderComponent(this, GlobalVars.BULLET_COLLIDER_TYPE));

            addComponent(new ScreenEdgeComponent(3, 3, 3, 3));
            }
        
        public override void revertToStartingState()
        {
            //Stuff
        }

    }
}
