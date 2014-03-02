using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    public class TimerComponent : Component {

        Dictionary<string, float> timers = new Dictionary<string, float>();
        List<string> completedTimers = new List<String>();

        public TimerComponent() {
            this.componentName = GlobalVars.TIMER_COMPONENT_NAME;
        }


        public Dictionary<string, float> getTimers() {
            return timers;
        }

        public void decTimers(float amt) {
            foreach ( string name in timers.Keys) {
                timers[name] -= amt;
                if ( timers[name] <= 0 ) {
                    completedTimers.Add( name );
                    timers.Remove( name );
                }
            }
        }

        public List<string> getCompletedTimers() {
            return completedTimers;
        }

        public void finishCompletedTimer( string name ) {
            completedTimers.Remove( name );
        }


    }
}
