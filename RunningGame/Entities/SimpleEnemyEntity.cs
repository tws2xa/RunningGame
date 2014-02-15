using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Collections;

namespace RunningGame.Entities {
    [Serializable()]
    public class SimpleEnemyEntity : Entity {


        public float defaultWidth = 30;
        public float defaultHeight = 60;
        public string leftImageName = "EnemyWalkLeft";
        public string rightImageName = "EnemyRightLeft";

        //-------------------------------------------Constructors--------------------------------------------

        public SimpleEnemyEntity( Level level, float x, float y ) {
            this.level = level;
            this.depth = 3;
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            addMyComponents( x, y );
        }
        public SimpleEnemyEntity( Level level, int id, float x, float y ) {
            this.level = level;

            initializeEntity( id, level );

            addMyComponents( x, y );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents( float x, float y ) {

            this.updateOutOfView = true;

            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( defaultWidth, defaultHeight, level, true ), true );

            List<string> enemyAnimation = new List<string>()
            {
                "Artwork.Creatures.Enemy1",
                "Artwork.Creatures.Enemy2",
            };

            List<string> enemyAnimDefaults = new List<string>()
            {
                "RunningGame.Resources.Artwork.Creatures.Enemy111.png",
                "RunningGame.Resources.Artwork.Creatures.Enemy211.png"
            };

            drawComp.addAnimatedSprite( enemyAnimation, enemyAnimDefaults, leftImageName );
            drawComp.setSprite( leftImageName );

            drawComp.addAnimatedSprite( enemyAnimation, enemyAnimDefaults, rightImageName );
            drawComp.rotateFlipSprite( rightImageName, System.Drawing.RotateFlipType.RotateNoneFlipX );

            /* ANIMATION COMPONENT - Does it need animating?
             */
            addComponent( new AnimationComponent( 0.10f ), true );

            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent( new VelocityComponent( 0, 0 ), true );

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent( new ColliderComponent( this, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, defaultWidth, defaultHeight-4), true );

            /*GRAVITY COMPONENT - Does it have Gravity?
             */
            addComponent( new GravityComponent( 0, GlobalVars.STANDARD_GRAVITY ), true );


            /*HEALTH COMPONENT - Does it have health, can it die?
             */
            addComponent( new HealthComponent( 100, true, 0, 100.0f ), true );

            /*SIMPLE ENEMY COMPONENT
             */
            addComponent( new SimpleEnemyComponent( GlobalVars.SIMPLE_ENEMY_H_SPEED + new Random().Next( -10, 10 ), true ), true );

            addComponent( new ScreenEdgeComponent( 1, 1, 1, 1 ), true );

        }

        //Revert!
        public override void revertToStartingState() {
            PositionComponent posComp = ( PositionComponent )getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            level.getMovementSystem().teleportToNoCollisionCheck( posComp, posComp.startingX, posComp.startingY );

            VelocityComponent velComp = ( VelocityComponent )getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            velComp.x = 0;
            velComp.y = 0;

            SimpleEnemyComponent simpEnemyComp = ( SimpleEnemyComponent )getComponent( GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME );
            simpEnemyComp.hasLandedOnce = false;
            simpEnemyComp.hasRunOnce = false;

            HealthComponent healthComp = ( HealthComponent )getComponent( GlobalVars.HEALTH_COMPONENT_NAME );
            healthComp.restoreHealth();
        }




        public void faceRight() {
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            drawComp.setSprite( rightImageName );
        }
        public void faceLeft() {
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            drawComp.setSprite( leftImageName );
        }

        public bool isLookingLeft() {
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            return ( drawComp.activeSprite == leftImageName );
        }

        public bool isLookingRight() {
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            return ( drawComp.activeSprite == rightImageName );
        }
    }
}
