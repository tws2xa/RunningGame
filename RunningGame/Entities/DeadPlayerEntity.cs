using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{
    class DeadPlayerEntity:Entity
    {

        float defaultWidth = 39;
        float defaultHeight = 49;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public DeadPlayerEntity(Level level, float x, float y)
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
        public DeadPlayerEntity(Level level, int id, float x, float y)
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

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y)
        {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);
            
            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            drawComp.addSprite("Artwork.Resources.Creatures.player", "RunningGame.Resources.Artwork.Creatures.player1.png", "Main"); //Add image
            drawComp.setSprite("Main"); //Set image to active image

            /* ANIMATION COMPONENT - Does it need animating?
             * The float that this reads in is the amount of time (in seconds) between frames.
             * So, if it was 5, you would be on one frame for 5 seconds, then switch to the next, then 5 seconds later
             * It'd switch to the next etc, etc...
             */
            //addComponent(new AnimationComponent(0.0005f));

            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent(new VelocityComponent(0, 0), true);

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE), true);

            /*GRAVITY COMPONENT - Does it have Gravity?
             *There's a standard gravity in GlobalVars
             */
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY), true);

        }
        
        //Will never start
        public override void revertToStartingState()
        {
            //Stuff
        }
         

    }
}
