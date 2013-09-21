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

    class EntityTemplate:Entity //Always extent the Entity Class
    {
        //If you're object has width/height Probably good to have a defaults for both.
        //These are the dimensions your entitiy will start out with.
        float defaultWidth = 10;
        float defaultHeight = 10;

        //These are used for resetting the entity. They're set in the constructor.
        float startingX;
        float startingY;

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
            
            //Sets the starting x and y.
            //Leave this for all entities with a position
            startingX = x;
            startingY = y;

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

            //Sets the starting x and y.
            //Leave this for all entities with a position
            startingX = x;
            startingY = y;

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y)
        {
            //POSITION COMPONENT - Does it have a position?
            //addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            //DRAW COMPONENT - Does it get drawn to the game world?
            //You'll need to know the address for your image.
            //It'll probably be something along the lines of "RunningGame.Resources.[      ].png" or maybe .bmp
            //addComponent(new DrawComponent("RunningGame.Resources.WhiteSquare.bmp", "Main", defaultWidth, defaultHeight, true));

            //VELOCITY COMPONENT - Does it move?
            //addComponent(new VelocityComponent(0, 0));

            //PLAYER COMPONENT - Is it the player?
            //addComponent(new PlayerInputComponent());

            //COLLIDER - Does it hit things?
            //The second field is the collider type. Look in GlobalVars for a string with the right name.
            //addComponent(new ColliderComponent(this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE));

            //GRAVITY COMPONENT - Does it have Gravity?
            //There's a standard gravity in GlobalVars
            //addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));

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
