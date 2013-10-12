using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Systems;
using RunningGame.Components;

namespace RunningGame.Entities
{

    /*
     * This is a template to help people create entities
     */

    public class EntityTemplate : Entity //Always extent the Entity Class
    {
    
        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public EntityTemplate(Level level, float x, float y)
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
        public EntityTemplate(Level level, int id, float x, float y)
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
            //addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" or maybe .bmp
             */
            //addComponent(new DrawComponent("RunningGame.Resources.WhiteSquare.bmp", "Main", defaultWidth, defaultHeight, true));


            /* ANIMATION COMPONENT - Does it need animating?
             * The float that this reads in is the amount of time (in seconds) between frames.
             * So, if it was 5, you would be on one frame for 5 seconds, then switch to the next, then 5 seconds later
             * It'd switch to the next etc, etc...
             */
            //addComponent(new AnimationComponent(0.0005f));

            /*VELOCITY COMPONENT - Does it move?
             */
            //addComponent(new VelocityComponent(0, 0));

            /*PLAYER COMPONENT - Is it the player?
             */
            //addComponent(new PlayerInputComponent());

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            //addComponent(new ColliderComponent(this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE));

            /*GRAVITY COMPONENT - Does it have Gravity?
             *There's a standard gravity in GlobalVars
             */
            //addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));


            /*HEALTH COMPONENT - Does it have health, can it die?
             *Parameters: maxHealth, startingHealth, draw a health bar?, recharge amount, recharge time
             *Basically, every rechargeTime, the entity regenerates rechargeAmount
             */
            //addComponent(new HealthComponent(100, 100, true, 1, 0.5f));

            /*
             * SQIUSH COMPONENT - Is it squishy?
             * This has a TON of variables...
             * the first two read in are the normal width and height
             * Then you read in the max width and height, then min width and height
             * You can then optionally give it a max and min surface area.
             * 
             * There are other variables which affect the squishy behavior
             * That you can change after instantiation, but not through the constructor.
             * They all have default values in the componenent which will probably be enough.
             */
            //addComponent(new SquishComponent(defaultWidth, defaultHeight, defaultWidth*3.0f, defaultHeight*3.0f, defaultWidth/3.0f, defaultHeight/3.0f));
        }
        
        //You must have this, but it may be empty.
        //What should the entity do in order to revert to its starting state?
        //Common things are:
            //Set position back to startingX and startingY
            //Set velocity to 0 in both directions
        //Note: Some things, like ground, dont move, and really don't need anything here.
        //Note: Some things, like a bullet, won't ever exist at the start of a level, so you could probably leave this empty.
        public override void revertToStartingState()
        {
            //Stuff
        }
         
    }
}
