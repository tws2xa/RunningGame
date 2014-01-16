using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RunningGame.Components {
    [Serializable()]
    public class SwitchComponent : Component {

        //Is it active?
        public bool active { get; set; }
        //Who is listening to this switch?
        public List<SwitchListenerComponent> listeners;

        //One constructor defaults to inactive, the other allows you to choose.
        public SwitchComponent() {
            this.componentName = GlobalVars.SWITCH_COMPONENT_NAME;
            this.active = false;
            listeners = new List<SwitchListenerComponent>();
        }
        public SwitchComponent( bool active ) {
            this.componentName = GlobalVars.SWITCH_COMPONENT_NAME;
            this.active = active;
            listeners = new List<SwitchListenerComponent>();
        }

        public void toggle() {
            active = !active;
            notifyObservers();
        }

        public void setActive( bool active ) {
            this.active = active;
            notifyObservers();
        }

        //Tell all the switch listeners that the switch changed state
        public void notifyObservers() {
            foreach ( SwitchListenerComponent c in listeners ) {
                // Set Changed 
                c.setChanged( true );
            }
        }
    }
}
