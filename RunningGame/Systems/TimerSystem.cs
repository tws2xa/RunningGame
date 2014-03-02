using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Systems {
    public class TimerSystem : GameSystem{

        
        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public TimerSystem( Level level ) {
            //Here is where you add the Required components
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

        //Update all timers.
        public override void Update( float deltaTime ) {
            foreach ( Entity e in getApplicableEntities() ) {
                TimerComponent timerComp = ( TimerComponent )e.getComponent( GlobalVars.TIMER_COMPONENT_NAME );
                timerComp.decTimers( deltaTime );
            }
        }
        //----------------------------------------------------------------------------------------------


    }
}
