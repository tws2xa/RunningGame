using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;
using System.Collections;

namespace RunningGame.Entities
{
    class PreGroundBounce : Entity
    {
        float defaultWidth = 11;
        float defaultHeight = 12;

        //string blockAnimationName = "blockAnimation";
        public PreGroundBounce(Level level, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }
        public PreGroundBounce(Level level,int id, float x, float y)
        {
            this.level = level;

            initializeEntity(id, level);

            addMyComponents(x, y);
        }

        public void addMyComponents(float x, float y)
        {
            //Position Component
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);

            //Draw component
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            drawComp.addSprite("Artwork.Foreground.BlockSquare", "RunningGame.Resources.Artwork.Foreground.BlockSquare.png", "Main");
            drawComp.setSprite("Main");


            //Velocity Component
            addComponent(new VelocityComponent(0, 0), true);

            //Collider
            addComponent(new ColliderComponent(this, GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE), true);

            //Gravity Component
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY), true);

            //Out of screen removal
            addComponent(new ScreenEdgeComponent(3, 3, 3, 3));
        }
        public override void revertToStartingState()
        {
      
        }
    }
}
