using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    class GeneralStateComponent : Component{

        public int state;

        public GeneralStateComponent(int state) {

            this.componentName = GlobalVars.GENERAL_STATE_COMPONENT_NAME;
            this.state = state;

        }

    }
}
