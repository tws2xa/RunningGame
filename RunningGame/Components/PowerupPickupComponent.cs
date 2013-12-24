using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    [Serializable()]
    public class PowerupPickupComponent : Component {

        public int compNum = -1;

        public PowerupPickupComponent(int compNum) {
            this.componentName = GlobalVars.POWERUP_PICKUP_COMPONENT_NAME;
            this.compNum = compNum;
        }

    }
}
