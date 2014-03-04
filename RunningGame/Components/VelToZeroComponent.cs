using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    public class VelToZeroComponent : Component {

        public float xSlow, ySlow;
        public bool blockSlow = false;

        public VelToZeroComponent(float x, float y) {
            this.componentName = GlobalVars.VEL_TO_ZERO_COMPONENT_NAME;
            this.xSlow = x;
            this.ySlow = y;
        }

    }
}
