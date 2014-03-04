using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    class VelToZeroComponent : Component {

        bool x, y;

        public VelToZeroComponent(bool x, bool y) {
            this.componentName = GlobalVars.VEL_TO_ZERO_COMPONENT_NAME;
            this.x = x;
            this.y = y;
        }

    }
}
