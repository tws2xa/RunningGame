using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Entities;
using RunningGame.Components;

namespace RunningGame.Systems {
    public class VelToZeroSystem : GameSystem{

        
        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public VelToZeroSystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.VEL_TO_ZERO_COMPONENT_NAME );
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME );


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
                VelocityComponent velComp = (VelocityComponent)e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                VelToZeroComponent velToZeroComp = ( VelToZeroComponent )e.getComponent( GlobalVars.VEL_TO_ZERO_COMPONENT_NAME );

                if ( !velToZeroComp.blockSlow ) {
                    if ( velComp.x > 0 ) {
                        velComp.x -= velToZeroComp.xSlow * deltaTime;
                        if ( velComp.x < 0 )
                            velComp.x = 0;
                    } else if ( velComp.x < 0 ) {
                        velComp.x += velToZeroComp.xSlow * deltaTime;
                        if ( velComp.x > 0 )
                            velComp.x = 0;
                    }

                    if ( velComp.y > 0 ) {
                        velComp.y -= velToZeroComp.ySlow * deltaTime;
                        if ( velComp.y < 0 )
                            velComp.y = 0;
                    } else if ( velComp.y < 0 ) {
                        velComp.y += velToZeroComp.ySlow * deltaTime;
                        if ( velComp.y > 0 )
                            velComp.y = 0;
                    }
                } else {
                    velToZeroComp.blockSlow = false;
                }


            }

        }
        //----------------------------------------------------------------------------------------------


    }
}
