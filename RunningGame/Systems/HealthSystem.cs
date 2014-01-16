using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Systems {
    [Serializable()]
    public class HealthSystem : GameSystem {

        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        DeathHandler deathHandler;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public HealthSystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.HEALTH_COMPONENT_NAME ); //Health component

            this.level = level; //Always have this

            deathHandler = new DeathHandler( level );

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
                HealthComponent healthComp = ( HealthComponent )e.getComponent( GlobalVars.HEALTH_COMPONENT_NAME );
                if ( healthComp.isDead() ) {
                    //Tell the death handler
                    deathHandler.handleDeath( e );
                }

                if ( !healthComp.hasFullHealth() ) {
                    if ( healthComp.timeSinceRecharge >= healthComp.rechargeTime ) {
                        healthComp.addToHealth( healthComp.rechargeAmt );
                        healthComp.timeSinceRecharge = 0;
                    } else {
                        healthComp.timeSinceRecharge += deltaTime;
                    }
                }

                if ( healthComp.health > healthComp.maxHealth ) healthComp.restoreHealth();
            }

            deathHandler.update( deltaTime );

        }
        //----------------------------------------------------------------------------------------------

    }
}
