using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;

namespace RunningGame.Systems {
    [Serializable()]
    public class PushableSystem : GameSystem {

        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        float slowRate = 500; //Deceleration rate in pixels/sec when not being pushed.

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public PushableSystem(Level level) {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME ); //Velocity
            requiredComponents.Add(GlobalVars.PUSHABLE_COMPONENT_NAME); //Pushable

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
        public override void Update(float deltaTime) {
            foreach ( Entity e in getApplicableEntities() ) {
                PushableComponent pushComp = (PushableComponent)e.getComponent( GlobalVars.PUSHABLE_COMPONENT_NAME );

                if ( !pushComp.dontSlow && !pushComp.wasPushedLastFrame ) {
                    VelocityComponent velComp = (VelocityComponent)e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );

                    if ( Math.Abs( velComp.x ) > slowRate ) {
                        if ( velComp.x < 0 ) velComp.x += slowRate;
                        else velComp.x -= slowRate;
                    } else if ( velComp.x != 0 ) {
                        velComp.x = 0;
                    }

                }

                if ( pushComp.wasPushedLastFrame ) {
                    pushComp.wasPushedLastFrame = false;
                }
            }
        }
        //----------------------------------------------------------------------------------------------


    }
}
