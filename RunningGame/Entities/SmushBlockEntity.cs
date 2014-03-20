using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    class SmushBlockEntity:Entity {

        float defaultWidth = 20;
        float defaultHeight = 20;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public SmushBlockEntity( Level level, float x, float y, int dir ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, dir );
        }
        public SmushBlockEntity( Level level, int id, float x, float y, int dir ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, dir );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents( float x, float y, int dir ) {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);

            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" ONLY png!!
             *First create the component
             *Then add the image
             *Then set the image to the active image
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite("Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Other.WhiteSquare.png", "Main");
            drawComp.setSprite("Main"); //Set image to active image

            /* ANIMATION COMPONENT - Does it need animating?
             * The float that this reads in is the amount of time (in seconds) between frames.
             * So, if it was 5, you would be on one frame for 5 seconds, then switch to the next, then 5 seconds later
             * It'd switch to the next etc, etc...
             */
            //addComponent(new AnimationComponent(0.0005f), true);

            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent(new VelocityComponent(0, 0));

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.SMUSH_BLOCK_COLLIDER), true);

            /*DIRECTION - It has a direction
             */
            addComponent( new DirectionalComponent( dir ) );

            /*SMUSH - It's a smusher!
             */
            addComponent( new SmushComponent( 2.0f ) );

            /*TIMER - It does stuff with a timer.
             */
            addComponent( new TimerComponent() );

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
            //addComponent(new SquishComponent(defaultWidth, defaultHeight, defaultWidth*3.0f, defaultHeight*3.0f, defaultWidth/3.0f, defaultHeight/3.0f), true);
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
        public override void revertToStartingState() {
            //Stuff
        }

    }
}
