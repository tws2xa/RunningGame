using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame
{
    /*
     * This class is where any and all entity "deaths" are handled.
     * Basically, if the health system notices that something has dropped to or below 0 health
     * It goes over to this class and tells it about the death.
     * 
     * Cheery Stuff
     */

    [Serializable()]
    class DeathHandler
    {

        Level level;

        public DeathHandler(Level level)
        {
            this.level = level;
        }

        public void handleDeath(Entity e) {
            //------- First check for any special cases

            //Is it the player? If so, reset the level.
            if(e.hasComponent(GlobalVars.PLAYER_COMPONENT_NAME)) {
                level.resetLevel();
            }
            //------- Not a special case? default to simply destroying the entity
            else {
                level.removeEntity(e);
            }
        }

    }
}
