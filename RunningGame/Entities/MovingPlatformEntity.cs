using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Entities {
    [Serializable()]
    public class MovingPlatformEntity : Entity {

        public float defaultWidth = 40;
        public float defaultHeight = 10;
        public float imageHeight = 10;
        public float imageWidth = 40;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public MovingPlatformEntity( Level level, float x, float y ) {
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
        public MovingPlatformEntity( Level level, int id, float x, float y ) {
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
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( imageWidth, imageHeight, level, true ), true );
            drawComp.addSprite( "Artwork.Foreground.MovPlat", "RunningGame.Resources.Artwork.Foreground.MovPlat1.png", "Main" ); //Add image
            drawComp.setSprite( "Main" ); //Set image to active image

            /*Animation
            addComponent( new AnimationComponent( 0.05f ) );


            List<string> anim = new List<string>()
            {
                "Artwork.Foreground.MovPlat1",
                "Artwork.Foreground.MovPlat2",
                "Artwork.Foreground.MovPlat3",
                "Artwork.Foreground.MovPlat4",
                "Artwork.Foreground.MovPlat5",
                "Artwork.Foreground.MovPlat6",
                "Artwork.Foreground.MovPlat5",
                "Artwork.Foreground.MovPlat4",
                "Artwork.Foreground.MovPlat3",
                "Artwork.Foreground.MovPlat2",
            };

            List<string> animDefaults = new List<string>()
            {
                "RunningGame.Resources.Artwork.Foreground.MovPLat111.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat211.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat311.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat411.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat511.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat611.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat511.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat411.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat311.png",
                "RunningGame.Resources.Artwork.Foreground.MovPLat211.png",
            };

            drawComp.addAnimatedSprite( anim, animDefaults, "MainAnim" );
            drawComp.setSprite( "MainAnim" );
            */
            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent( new VelocityComponent( 0, GlobalVars.MOVING_PLATFORM_SPEED ), true );

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent( new ColliderComponent( this, GlobalVars.MOVING_PLATFORM_COLLIDER_TYPE, defaultWidth, defaultHeight ), true );

            /*MOVING PLATFORM COMPONENT
             */
            addComponent( new MovingPlatformComponent( this ), true );

        }

        //You must have this, but it may be empty.
        //What should the entity do in order to revert to its starting state?
        public override void revertToStartingState() {
            PositionComponent posComp = ( PositionComponent )this.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            level.getMovementSystem().teleportToNoCollisionCheck( posComp, posComp.startingX, posComp.startingY );
        }

    }
}
