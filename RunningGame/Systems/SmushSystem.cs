using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Systems {
    public class SmushSystem:GameSystem {

        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        const string upperWaitTimer = "upperWaitTimer";
        const string lowerWaitTimer = "lowerWaitTimer";


        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public SmushSystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.SMUSH_COMPONENT_NAME ); //Smush
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME ); //Velocity
            requiredComponents.Add( GlobalVars.TIMER_COMPONENT_NAME ); //Timer


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

        //You must have an Update.
        //Always read in deltaTime, and only deltaTime (it's the time that's passed since the last frame)
        //Use deltaTime for things like changing velocity or changing position from velocity
        //This is where you do anything that you want to happen every frame.
        //There is a chance that your system won't need to do anything in update. Still have it.
        public override void Update( float deltaTime ) {
            foreach ( Entity e in getApplicableEntities() ) {
                SmushComponent smushComp = ( SmushComponent )e.getComponent( GlobalVars.SMUSH_COMPONENT_NAME );
                if ( !smushComp.getInitializedTimer() ) {
                    initializeTimer( e );
                }

                TimerComponent timeComp = ( TimerComponent )e.getComponent( GlobalVars.TIMER_COMPONENT_NAME );

                if ( smushComp.isFrozen() && smushComp.isWaitingUpper() ) {
                    timeComp.setTimer( upperWaitTimer, smushComp.getUpperWaitTime() );
                }

                List<string> completedTimers = new List<string>(timeComp.getCompletedTimers());
                foreach ( string timer in completedTimers ) {
                    handleCompletedTimer( timer, e );
                    timeComp.removeCompletedTimer( timer );
                }

                if ( !smushComp.isWaitingUpper() && !smushComp.isWaitingLower() ) {
                    checkForDirectionChange( e );
                }

            }
        }
        //----------------------------------------------------------------------------------------------

        //--------------------------HANDLE TIMERS--------------------------
        private void initializeTimer( Entity e ) {
            SmushComponent smushComp = ( SmushComponent )e.getComponent( GlobalVars.SMUSH_COMPONENT_NAME );
            startUpperWait( e );
            smushComp.setHasInitializedTimer( true );
        }

        private void handleCompletedTimer( string timerName, Entity e ) {
            switch(timerName){
                case(upperWaitTimer):
                    handleUpperTimerComplete( e );
                    break;
                case(lowerWaitTimer):
                    handleLowerTimerComplete( e );
                    break;
                default:
                    Console.WriteLine( "Smush System trying to handle timer with unrecognized name: " + timerName );
                    break;
            }
        }

        private void handleUpperTimerComplete( Entity e ) {
            startFall( e );
        }
        private void handleLowerTimerComplete( Entity e ) {
            startRise(e);
        }


        //--------------------------HANDLE DIRECTION CHANGE--------------------------
        private void checkForDirectionChange( Entity e ) {
            SmushComponent smushComp = ( SmushComponent )e.getComponent( GlobalVars.SMUSH_COMPONENT_NAME );
            VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );

            float stopBuffer = 0.01f;

            if ( smushComp.getFallSpeed().X != 0 && Math.Abs(velComp.x) < stopBuffer ) {
                if ( smushComp.isFalling() ) {
                    startLowerWait( e );
                } else if ( smushComp.isRising() ) {
                    startUpperWait( e );
                }
            }
            if ( smushComp.getFallSpeed().Y != 0 && Math.Abs(velComp.y) < stopBuffer ) {
                if ( smushComp.isFalling() ) {
                    startLowerWait( e );
                } else if ( smushComp.isRising() ) {
                    startUpperWait( e );
                }
            }    
        }

        //--------------------------CHANGE PHASE METHODS--------------------------

        //Start falling
        private void startFall( Entity e ) {
            SmushComponent smushComp = ( SmushComponent )e.getComponent( GlobalVars.SMUSH_COMPONENT_NAME );
            VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            startFall( velComp, smushComp );
        }
        private void startFall(VelocityComponent velComp, SmushComponent smushComp) {
            velComp.setVelocity( smushComp.getFallSpeed() );
            smushComp.setStateFall();
        }

        //Start Rising
        private void startRise( Entity e ) {
            SmushComponent smushComp = ( SmushComponent )e.getComponent( GlobalVars.SMUSH_COMPONENT_NAME );
            VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            startRise( velComp, smushComp );
        }
        private void startRise( VelocityComponent velComp, SmushComponent smushComp ) {
            //Console.WriteLine("Setting " + velComp + " vel to " + smushComp.getRiseSpeed());
            velComp.setVelocity( smushComp.getRiseSpeed() );
            smushComp.setStateRise();
        }
        
        //Start Upper Waiting
        private void startUpperWait( Entity e ) {
            SmushComponent smushComp = ( SmushComponent )e.getComponent( GlobalVars.SMUSH_COMPONENT_NAME );
            VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            TimerComponent timerComp = ( TimerComponent )e.getComponent( GlobalVars.TIMER_COMPONENT_NAME );
            startUpperWait( velComp, smushComp, timerComp, e );
        }
        private void startUpperWait( VelocityComponent velComp, SmushComponent smushComp, TimerComponent timerComp, Entity e ) {
            velComp.setVelocity( 0, 0 );
            if ( !timerComp.hasTimer( upperWaitTimer ) ) {
                timerComp.addTimer( upperWaitTimer, smushComp.getUpperWaitTime() );
            }
            smushComp.setStateUpperWait();
        }

        //Start Lower Waiting
        private void startLowerWait( Entity e ) {
            SmushComponent smushComp = ( SmushComponent )e.getComponent( GlobalVars.SMUSH_COMPONENT_NAME );
            VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            TimerComponent timerComp = ( TimerComponent )e.getComponent( GlobalVars.TIMER_COMPONENT_NAME );
            startLowerWait( velComp, smushComp, timerComp, e );
        }
        private void startLowerWait( VelocityComponent velComp, SmushComponent smushComp, TimerComponent timerComp, Entity e ) {
            velComp.setVelocity( 0, 0 );
            smushComp.setStateLowerWait();
            if ( !timerComp.hasTimer( upperWaitTimer ) ) {
                timerComp.addTimer( lowerWaitTimer, smushComp.getLowerWaitTime() );
            }
        }

    }
}
