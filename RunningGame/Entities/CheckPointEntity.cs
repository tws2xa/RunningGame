using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    public class CheckPointEntity : Entity{

        float defaultWidth = 20;
        float defaultHeight = 20;

        string checkedImageName = "checked";
        string uncheckedImageName = "unchecked";

        //-------------------------------------------Constructors--------------------------------------------
        public CheckPointEntity( Level level, float x, float y ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y );
        }
        public CheckPointEntity( Level level, int id, float x, float y ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents( float x, float y ) {
            /*POSITION COMPONENT - it has a position
             */
            addComponent( new PositionComponent(x, y, defaultWidth, defaultHeight, this), true );

            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite( "Artwork.Foreground.Checkpoint.UncheckedCheckpoint", "RunningGame.Resources.Artwork.Foreground.Checkpoint.UncheckedCheckpoint.png", uncheckedImageName );
            drawComp.addSprite( "Artwork.Foreground.Checkpoint.CheckedCheckpoint", "RunningGame.Resources.Artwork.Foreground.Checkpoint.CheckedCheckpoint.png", checkedImageName );
            drawComp.setSprite(uncheckedImageName); //Set image to active image


            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.CHECKPOINT_COLLIDER_TYPE), true);

            /*CHECKPOINT - It's a checkpoint
             */
            addComponent( new CheckPointComponent(uncheckedImageName, checkedImageName, this), true );
        }

        public override void revertToStartingState() {
            CheckPointComponent checkComp = ( CheckPointComponent )this.getComponent( GlobalVars.CHECKPOINT_COMPONENT_NAME );
            checkComp.setActive( false );
        }

    }
}
