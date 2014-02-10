using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    class SpikeEntity : Entity {

        float defaultWidth = 10.0f;
        float defaultHeight = 10.0f;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public SpikeEntity( Level level, float x, float y, int dir ) {
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
        public SpikeEntity( Level level, int id, float x, float y, int dir ) {
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
            PositionComponent posComp = ( PositionComponent )addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" ONLY png!!
             *First create the component
             *Then add the image
             *Then set the image to the active image
             */
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( defaultWidth, defaultHeight, level, true ), true );
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            string sprName = "Main";
            drawComp.addSprite( "Artwork.Foreground.Spike", "RunningGame.Resources.Artwork.Foreground.Spike.png", sprName );
            drawComp.setSprite( sprName ); //Set image to active image

            //Won't always be the same as spr name
            //Like if it's been de-colored as a result of being in a
            //World's 1st level before the color orb is obtained.
            string currentSprite = drawComp.activeSprite;
            
            //Rotate accordingly
            for ( int i = 0; i < dir; i++ ) {
                drawComp.rotateFlipSprite( sprName, System.Drawing.RotateFlipType.Rotate90FlipNone );
                if ( currentSprite != sprName ) {
                    drawComp.rotateFlipSprite( currentSprite, System.Drawing.RotateFlipType.Rotate90FlipNone );
                }
            }


            /*
             * DIRECTIONAL COMPONENT - it has a direction (used by collision)
             */
            addComponent( new DirectionalComponent( dir ), true );

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent( new ColliderComponent( this, GlobalVars.SPIKE_COLLIDER_TYPE ), true );
        }

        public override void revertToStartingState() {
            //Stuff
        }
    }
}
