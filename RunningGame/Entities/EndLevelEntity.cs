using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{
    class EndLevelEntity:Entity
    {

        float defaultWidth = 20;
        float defaultHeight = 20;

         //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public EndLevelEntity(Level level, float x, float y)
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
        public EndLevelEntity(Level level, int id, float x, float y)
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

        //------------------------------------------------------------------------------------------------------------------

        public void addMyComponents(float x, float y)
        {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" ONLY png!!
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true));
            drawComp.addSprite("RunningGame.Resources.WhiteSquare.png", "Main");
            drawComp.setSprite("Main");

            /* ANIMATION COMPONENT - Does it need animating?
             * The float that this reads in is the amount of time (in seconds) between frames.
             * So, if it was 5, you would be on one frame for 5 seconds, then switch to the next, then 5 seconds later
             * It'd switch to the next etc, etc...
             */
            //addComponent(new AnimationComponent(0.0005f));

            /*VELOCITY COMPONENT - Does it move?
             */
            //addComponent(new VelocityComponent(0, 0));

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.END_LEVEL_COLLIDER_TYPE));

            /*GRAVITY COMPONENT - Does it have Gravity?
             *There's a standard gravity in GlobalVars
             */
            //addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));
        }
        
        //You must have this, but it may be empty.
        //What should the entity do in order to revert to its starting state?
        //Common things are:
            //Set position back to startingX and startingY
                //NOTE: If doing this, you probably want to use the MovementSystem's teleportToNoCollisionCheck() method
                //rather than the usual changePosition()
            //Set velocity to 0 in both directions
        //Note: Some things, like ground, dont move, and really don't need anything here.
        //Note: Some things, like a bullet, won't ever exist at the start of a level, so you could probably leave this empty.
        public override void revertToStartingState()
        {
            //Stuff
        }

    }
}
