using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Systems;
using RunningGame.Components;

namespace RunningGame.Entities
{


    class Speedy:Entity
    {
        float defaultWidth = 10;
        float defaultHeight = 10;

        float startingX;
        float startingY;

        public Speedy(Level level, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);
            
            //Sets the starting x and y.
            //Leave this for all entities with a position
            startingX = x;
            startingY = y;

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }

        public Speedy(Level level, int id, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);

            //Sets the starting x and y.
            //Leave this for all entities with a position
            startingX = x;
            startingY = y;

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }

        public void addMyComponents(float x, float y)
        {
            //position and velocity
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));
            addComponent(new ColliderComponent(this, GlobalVars.SPEEDY_COLLIDER));
            
        }
        
        public override void revertToStartingState()
        {
        }


    }
}
