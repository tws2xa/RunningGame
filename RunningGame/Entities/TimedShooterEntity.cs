using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    public class TimedShooterEntity : Entity{

        float defaultWidth = 20f;
        float defaultHeight = 20f;
        string startingSprite = "Bkp";

        //-------------------------------------------Constructors--------------------------------------------
        public TimedShooterEntity( Level level, float x, float y, float timeBetweenBursts, int shotsPerBurst, int dir, int switchId ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, timeBetweenBursts, shotsPerBurst, dir, switchId );
        }
        public TimedShooterEntity( Level level, int id, float x, float y, float timeBetweenBursts, int shotsPerBurst, int dir, int switchId ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, timeBetweenBursts, shotsPerBurst, dir, switchId );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents( float x, float y, float timeBetweenBursts, int shotsPerBurst, int dir, int switchId ) {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);

            /*VELOCITY - Just cuz'
             */
            addComponent( new VelocityComponent(), true );

            /* TIMED SHOOTER COMPONENT - It shoots at a given time interval.
             */
            TimedShooterComponent shooterComp = ( TimedShooterComponent )addComponent( new TimedShooterComponent( timeBetweenBursts, shotsPerBurst, this ), true );
            startingSprite = shooterComp.badSpriteName;

            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite("Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Other.WhiteSquare.png", "Bkp");

            List<string> shooterAnimation = new List<string>() {
                "Artwork.Foreground.Shooter.shooterp0",
                "Artwork.Foreground.Shooter.shooterp1",
                "Artwork.Foreground.Shooter.shooterp2",
                "Artwork.Foreground.Shooter.shooterp3",
                "Artwork.Foreground.Shooter.shooterp4",
                "Artwork.Foreground.Shooter.shooterp5",
                "Artwork.Foreground.Shooter.shooterp6",
                "Artwork.Foreground.Shooter.shooterp7",
                "Artwork.Foreground.Shooter.shooterp8",
                "Artwork.Foreground.Shooter.shooterp9",
                "Artwork.Foreground.Shooter.shooterp10",
                "Artwork.Foreground.Shooter.shooterp11",
                "Artwork.Foreground.Shooter.shooterp12"
            };
            List<string> shooterAnimDefaults = new List<string>() {
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp0.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp1.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp2.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp3.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp4.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp5.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp6.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp7.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp8.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp9.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp10.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp11.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.shooterp12.png"
            };


            List<string> goodShooterAnimation = new List<string>() {
                "Artwork.Foreground.Shooter.GoodShooter0",
                "Artwork.Foreground.Shooter.GoodShooter1",
                "Artwork.Foreground.Shooter.GoodShooter2",
                "Artwork.Foreground.Shooter.GoodShooter3",
                "Artwork.Foreground.Shooter.GoodShooter4",
                "Artwork.Foreground.Shooter.GoodShooter5",
                "Artwork.Foreground.Shooter.GoodShooter6",
                "Artwork.Foreground.Shooter.GoodShooter7",
                "Artwork.Foreground.Shooter.GoodShooter8",
                "Artwork.Foreground.Shooter.GoodShooter9",
                "Artwork.Foreground.Shooter.GoodShooter10",
                "Artwork.Foreground.Shooter.GoodShooter11",
                "Artwork.Foreground.Shooter.GoodShooter12"
            };
            List<string> goodShooterAnimDefaults = new List<string>() {
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter0.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter1.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter2.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter3.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter4.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter5.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter6.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter7.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter8.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter9.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter10.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter11.png",
                "RunningGame.Resources.Artwork.Foreground.Shooter.GoodShooter12.png"
            };

            drawComp.addAnimatedSprite( shooterAnimation, shooterAnimDefaults, shooterComp.badSpriteName );
            drawComp.addAnimatedSprite( goodShooterAnimation, goodShooterAnimDefaults, shooterComp.goodShooterName );
            drawComp.setSprite( startingSprite ); //Set image to active image

            float animationTime = timeBetweenBursts / shooterAnimDefaults.Count();

            /* ANIMATION COMPONENT - Does it need animating?
             */
            AnimationComponent animComp = (AnimationComponent)addComponent(new AnimationComponent(animationTime), true);

            /*COLLIDER - Does it hit things?
             */
            addComponent(new ColliderComponent(this, GlobalVars.TIMED_SHOOTER_COLLIDER_TYPE), true);
            
            /* TIMER COMPONENT - It makes use of timed method execution.
             */
            TimerComponent timeComp = ( TimerComponent )addComponent( new TimerComponent(), true );
            timeComp.addTimer( shooterComp.fireTimerString, timeBetweenBursts );

            /* DIRECTION COMPONENT - It points in a particular direciton.
             */
            addComponent( new DirectionalComponent( dir ) );

            /*VEL TO ZERO - If it's moving, it will try to stop moving.
             */
            addComponent( new VelToZeroComponent( 100, 100 ) );

            /*SWITCH LISTENER - It listens for switches.
             */
            addComponent( new SwitchListenerComponent( switchId, GlobalVars.TIMED_SHOOTER_SWITCH_EVENT ) );

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
            TimedShooterComponent shooterComp = ( TimedShooterComponent )this.getComponent( GlobalVars.TIMED_SHOOTER_COMPONENT_NAME );
            TimerComponent timerComp = ( TimerComponent )this.getComponent( GlobalVars.TIMER_COMPONENT_NAME );
            timerComp.clearAllTimers();
            timerComp.addTimer( shooterComp.fireTimerString, shooterComp.timeBetweenBursts );
            PositionComponent posComp = ( PositionComponent )this.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            level.getMovementSystem().teleportToNoCollisionCheck( posComp, posComp.startingX, posComp.startingY );
            VelocityComponent velComp = ( VelocityComponent )this.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            velComp.setVelocity( 0, 0 );
            DrawComponent drawComp = (DrawComponent)this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            drawComp.setSprite( startingSprite, true );
        }

    }
}
