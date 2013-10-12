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
    public class Glide : Entity
    {
        float defaultWidth = 10;
        float defaultHeight = 10;

        public Glide(Level level, float x, float y)
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

        public Glide(Level level, int id, float x, float y)
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
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));
            addComponent(new ColliderComponent(this, GlobalVars.SPEEDY_COLLIDER_TYPE));
            
        }
        
        public override void revertToStartingState()
        {
        }
    }
}
