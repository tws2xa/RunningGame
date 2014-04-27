using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;

namespace RunningGame.Entities {
    [Serializable()]
    public class PowerupPickupEntity : Entity {

        float spriteWidth = 20;
        float spriteHeight = 20;
        float defaultWidth = 20;
        float defaultHeight = 20;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public PowerupPickupEntity( Level level, float x, float y, int compNum ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, compNum );
        }
        public PowerupPickupEntity( Level level, int id, float x, float y, int compNum ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, compNum );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents( float x, float y, int compNum ) {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" ONLY png!!
             *First create the component
             *Then add the image
             *Then set the image to the active image
             */
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( spriteWidth, spriteHeight, level, true ), true );
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            string stem = "RunningGame.Resources.Artwork.Foreground.PowerupPickups.";
            
            if ( compNum == GlobalVars.BOUNCE_NUM )
                drawComp.addSprite( "", stem + "BouncePickuo.png", "Main" );
            else if ( compNum == GlobalVars.SPEED_NUM )
                drawComp.addSprite( "", stem + "SpeedyPickup.png", "Main" );
            else if ( compNum == GlobalVars.JMP_NUM )
                drawComp.addSprite( "", stem + "DoubleJumpPickup.png", "Main" );
            else if ( compNum == GlobalVars.GLIDE_NUM )
                drawComp.addSprite( "", stem + "GlidePickup.png", "Main" );
            else if ( compNum == GlobalVars.SPAWN_NUM )
                drawComp.addSprite( "", stem + "SpawnPickup.png", "Main" );
            else if ( compNum == GlobalVars.GRAP_NUM )
                drawComp.addSprite( "", stem + "GrapplePickup.png", "Main" );
            else
                drawComp.addSprite( "Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Other.WhiteSquare.png", "Main" );
            
            /*
            if ( compNum == GlobalVars.BOUNCE_NUM )
                drawComp.addSprite( "", stem + "PowerUpGreen.png", "Main" );
            else if ( compNum == GlobalVars.SPEED_NUM )
                drawComp.addSprite( "", stem + "PowerUpBlue.png", "Main" );
            else if ( compNum == GlobalVars.JMP_NUM )
                drawComp.addSprite( "", stem + "PowerUpPurple.png", "Main" );
            else if ( compNum == GlobalVars.GLIDE_NUM )
                drawComp.addSprite( "", stem + "PowerUpYellow.png", "Main" );
            else if ( compNum == GlobalVars.SPAWN_NUM )
                drawComp.addSprite( "", stem + "PowerUpOrange.png", "Main" );
            else if ( compNum == GlobalVars.GRAP_NUM )
                drawComp.addSprite( "", stem + "PowerUpRed.png", "Main" );
            else
                drawComp.addSprite( "Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Other.WhiteSquare.png", "Main" );
            */

            drawComp.setSprite( "Main" ); //Set image to active image

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
            addComponent( new ColliderComponent( this, GlobalVars.POWERUP_PICKUP_COLLIDER_TYPE ), true );

            /*POWERUP PICKUP COMPONENT
             */
            addComponent( new PowerupPickupComponent( compNum ), true );

        }

        //What should the entity do in order to revert to its starting state?
        public override void revertToStartingState() {
            //Stuff
        }

    }
}
