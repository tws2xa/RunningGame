using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;

namespace RunningGame.Components {
    class PushableComponent : Component {

        public bool wasPushedLastFrame = false;
        public bool dontSlow = false;
        public bool horiz = true;
        public bool vert = true;
        public int horizMovementStopped = 0; //0 = stopped in neither direction, 1 = left, 2 = right, 3 = both
        public int vertMovementStopped = 0; //0 = stopped in neither direction, 1 = up, 2 = down, 3 = both

        public PushableComponent() {
            
            componentName = GlobalVars.PUSHABLE_COMPONENT_NAME;

        }

    }
}
