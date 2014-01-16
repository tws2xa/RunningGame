using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    [Serializable()]
    public class TimedSwitchComponent : Component {

        public float baseTime;
        public float timer;

        public TimedSwitchComponent( float time ) {

            this.componentName = GlobalVars.TIMED_SWITCH_COMPONENT_NAME;

            baseTime = time;
            timer = 0;
        }

    }
}
