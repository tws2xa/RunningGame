using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    class SpawnBlockComponent : Component {

        //State 0 = sitting still
        //State 1 = Weaponized
        //State 2 = Hit one enemy already
        public int state = 0;

        public SpawnBlockComponent() {
            this.componentName = GlobalVars.SPAWN_BLOCK_COMPONENT_NAME;
        }

    }
}
