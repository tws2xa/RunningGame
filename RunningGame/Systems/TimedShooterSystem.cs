using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame.Systems {
    public class TimedShooterSystem : GameSystem{

        
        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public TimedShooterSystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.TIMED_SHOOTER_COMPONENT_NAME );
            requiredComponents.Add( GlobalVars.TIMER_COMPONENT_NAME );


            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }

        //Must have this. Same for all Systems.
        public override Level GetActiveLevel() {
            return level;
        }

        public override void Update( float deltaTime ) {

            foreach ( Entity e in getApplicableEntities() ) {
                TimerComponent timeComp = ( TimerComponent )e.getComponent( GlobalVars.TIMER_COMPONENT_NAME );
                TimedShooterComponent shooterComp = ( TimedShooterComponent )e.getComponent( GlobalVars.TIMED_SHOOTER_COMPONENT_NAME );
                DirectionalComponent dirComp = ( DirectionalComponent )e.getComponent( GlobalVars.DIRECTION_COMPONENT_NAME );
                int dir = 1;
                if ( dirComp != null ) {
                    dir = dirComp.dir;
                }

                //Make a copy so editing the component's list doesn't kill the loop
                List<string> completedTimers = new List<string>(timeComp.getCompletedTimers());
                foreach(string name in completedTimers) {
                    if ( name == shooterComp.fireTimerString ) {
                        timeComp.removeCompletedTimer( name );
                        beginFire( e, shooterComp, timeComp, dir );
                    } else {
                        Console.WriteLine( "Unrecognized timer: " + name + " has been completed." );
                    }
                }

            }

        }
        //----------------------------------------------------------------------------------------------


        public void beginFire(Entity e, TimedShooterComponent shooterComp, TimerComponent timeComp, int dir) {
            fireItem(e, dir);
            shooterComp.currentBurstNum++;
            if ( shooterComp.currentBurstNum < shooterComp.numShotsPerBurst ) {
                timeComp.addTimer( shooterComp.fireTimerString, shooterComp.timeBetweenShotsInBurst );
            } else {
                shooterComp.currentBurstNum = 0;
                timeComp.addTimer( shooterComp.fireTimerString, shooterComp.timeBetweenBursts );
            }
        }

        public void fireItem(Entity e, int dir) {
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            ShooterBullet bullet = new ShooterBullet( level, level.rand.Next(Int32.MinValue, Int32.MaxValue), posComp.x, posComp.y, dir );
            VelocityComponent bulletVel = ( VelocityComponent )bullet.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            float bulletSpeed = 160.0f;
            switch ( dir % 4) {
                case( 0 ):
                    bulletVel.setVelocity( 0, -bulletSpeed );
                    break;
                case ( 1 ):
                    bulletVel.setVelocity( bulletSpeed, 0);
                    break;
                case ( 2 ):
                    bulletVel.setVelocity( 0, bulletSpeed );
                    break;
                case ( 3 ):
                    bulletVel.setVelocity( -bulletSpeed, 0 );
                    break;
            }
            level.addEntity( bullet );
        }

    }
}
