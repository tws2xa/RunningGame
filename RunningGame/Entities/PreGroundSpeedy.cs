using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Systems;
using RunningGame.Components;

namespace RunningGame.Entities
{

    [Serializable()]
    public class PreGroundSpeedy : Entity
    {
        float defaultWidth = 10;
        float defaultHeight = 10;

        //float startingX;
        //float startingY;

        public PreGroundSpeedy(Level level, float x, float y)
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

        public PreGroundSpeedy(Level level, int id, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }

        public void addMyComponents(float x, float y)
        {
            //position and velocity
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY), true);
            addComponent(new ColliderComponent(this, GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE), true);
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite("Artwork.Other.Blue", "RunningGame.Resources.Artwork.Foreground.Blue.png", "Main");
            drawComp.setSprite("Main"); //Set image to active image
            addComponent(new VelocityComponent(0, 0));

            //Out of screen removal
            addComponent(new ScreenEdgeComponent(3, 3, 3, 3));
        }
        
        public override void revertToStartingState()
        {
        }


    }
}
