using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{

    [Serializable()]
    class TimedSwitchEntity : Entity
    {

        float defaultWidth = 20;
        float defaultHeight = 20;
        float defaultTime = 5.0f;

        bool startingState; //Active or non-active at level start?

        //-------------------------------------------Constructors--------------------------------------------
        public TimedSwitchEntity(Level level, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);
            
            startingState = false;

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }
        public TimedSwitchEntity(Level level, int id, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);

            startingState = false;

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }
        public TimedSwitchEntity(Level level, int id, float x, float y, bool active)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);

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
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true));
            drawComp.addSprite("Artwork.Foreground.switchUp", "RunningGame.Resources.Artwork.Foreground.switchUp11.png", GlobalVars.SWITCH_INACTIVE_SPRITE_NAME);
            drawComp.addSprite("Artwork.Foreground.switchRight", "RunningGame.Resources.Artwork.Foreground.switchRight11.png", GlobalVars.SWITCH_ACTIVE_SPRITE_NAME);
            drawComp.setSprite(GlobalVars.SWITCH_INACTIVE_SPRITE_NAME);

            //COLLIDER - Does it hit things?
            addComponent(new ColliderComponent(this, GlobalVars.SWITCH_COLLIDER_TYPE));

            //Swich Component - Is it a switch? Yes.
            addComponent(new SwitchComponent(startingState));

            //Timed Switch Component It's a timed switch!
            addComponent(new TimedSwitchComponent(defaultTime));
        }
        
        public override void revertToStartingState()
        {
            SwitchComponent sc = (SwitchComponent)getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
            sc.setActive(startingState);

            DrawComponent drawComp = (DrawComponent)getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            if (startingState)
                drawComp.setSprite(GlobalVars.SWITCH_ACTIVE_SPRITE_NAME);
            else
                drawComp.setSprite(GlobalVars.SWITCH_INACTIVE_SPRITE_NAME);
        }

    }
}
