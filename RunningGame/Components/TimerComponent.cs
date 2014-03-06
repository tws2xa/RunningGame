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

        public void addTimer( string name, float time ) {
            timers.Add( name, time );
        }

        public void removeTimer( string name ) {
            timers.Remove( name );
            finishCompletedTimer( name );
        }

        public void decTimers(float amt) {

            List<string> names = new List<string>( timers.Keys );
            foreach ( string name in names) {
                timers[name] -= amt;
                if ( timers[name] <= 0 ) {
                    timers.Remove( name );
                    completedTimers.Add( name );
                }
            }
        }

        public List<string> getCompletedTimers() {
            return completedTimers;
        }

        public void finishCompletedTimer( string name ) {
            completedTimers.Remove( name );
        }



        internal void clearAllTimers() {
            timers.Clear();
            completedTimers.Clear();
        }
    }
}
