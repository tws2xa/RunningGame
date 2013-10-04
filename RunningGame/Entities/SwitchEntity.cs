using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{
    class SwitchEntity:Entity
    {

        float defaultWidth = 20;
        float defaultHeight = 20;

        //These are used for resetting the entity. They're set in the constructor.
        float startingX;
        float startingY;
        bool startingState; //Active or non-active at level start?

        //-------------------------------------------Constructors--------------------------------------------
        public SwitchEntity(Level level, float x, float y)
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

            startingState = false;

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }
        public SwitchEntity(Level level, int id, float x, float y)
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

            startingState = false;

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }
        public SwitchEntity(Level level, int id, float x, float y, bool active)
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

            startingState = active;

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }

        //------------------------------------------------------------------------------------------------------------------

        public void addMyComponents(float x, float y)
        {
            //POSITION COMPONENT - Does it have a position?
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            //DRAW COMPONENT - Does it get drawn to the game world?
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent("RunningGame.Resources.SwitchOff.png", GlobalVars.SWITCH_INACTIVE_SPRITE_NAME, defaultWidth, defaultHeight, true));
            drawComp.addSprite("RunningGame.Resources.SwitchOn.png", GlobalVars.SWITCH_ACTIVE_SPRITE_NAME);

            //COLLIDER - Does it hit things?
            addComponent(new ColliderComponent(this, GlobalVars.SWITCH_COLLIDER_TYPE));

            //Swich Component - Is it a switch? Yes.
            addComponent(new SwitchComponent(startingState));
        }
        
        public override void revertToStartingState()
        {
            SwitchComponent sc = (SwitchComponent)getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
            sc.setActive(startingState);
        }

    }
}
