using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    [Serializable()]
    public class BulletEntity : Entity {

        public float defaultWidth = 40;
        public float defaultHeight = 40;

        //-------------------------------------------Constructors--------------------------------------------
        public BulletEntity( Level level, float x, float y ) {
            this.level = level;
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            addMyComponents( x, y, 0, 0 );
        }
        public BulletEntity( Level level, int id, float x, float y ) {
            this.level = level;

            initializeEntity( id, level );

            addMyComponents( x, y, 0, 0 );
        }

        //Can take in a velocity as well
        public BulletEntity( Level level, float x, float y, float velX, float velY ) {
            this.level = level;

            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            addMyComponents( x, y, velX, velY );
        }
        public BulletEntity( Level level, int id, float x, float y, float velX, float velY ) {
            this.level = level;

            initializeEntity( id, level );

            addMyComponents( x, y, velX, velY );
        }

        //------------------------------------------------------------------------------------------------------------------

        public void addMyComponents( float x, float y, float velX, float velY ) {

            this.updateOutOfView = true;

            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            //Gravity
            addComponent( new GravityComponent(0, GlobalVars.STANDARD_GRAVITY/4), true );

            /*DRAW COMPONENT - Does it get drawn to the game world?
             * NOTE: Was PaintBlob11.png before Bullet11.png
             */
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( defaultWidth, defaultHeight, level, true ), true );


            List<string> bulletAnimation = new List<string>()
            {
                "Artwork.Foreground.Bullet.Bullet1",
                "Artwork.Foreground.Bullet.Bullet2"
            };

            List<string> bulletAnimDefaults = new List<string>()
            {
                "RunningGame.Resources.Artwork.Foreground.Bullet.Bullet111",
                "RunningGame.Resources.Artwork.Foreground.Bullet.Bullet211"
            };

            
            //drawComp.addSprite( "Artwork.Foreground.Bullet", "RunningGame.Resources.Artwork.Foreground.Bullet11.png", "Main" );
            drawComp.addAnimatedSprite( bulletAnimation, bulletAnimDefaults, "Main" );
            drawComp.setSprite( "Main" );
            

            /* ANIMATION COMPONENT - Does it need animating?
             */
            addComponent( new AnimationComponent( 0.08f ), true );

            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent( new VelocityComponent( velX, velY ), true );

            /*COLLIDER - Does it hit things?
             */
            addComponent( new ColliderComponent( this, GlobalVars.BULLET_COLLIDER_TYPE, 10, 10 ), true );

            /* OUT OF SCREEN COOMPONENT - Destroy on exiting screen.
             */
            addComponent( new ScreenEdgeComponent( 3, 3, 3, 3 ), true );
        }

        public override void revertToStartingState() {
            //Stuff
        }

    }
}
