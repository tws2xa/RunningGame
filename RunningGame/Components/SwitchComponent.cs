using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Entities;

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

        public void setActive( bool active, Entity me ) {
            this.active = active;

            if(active && me.hasComponent(GlobalVars.TIMED_SWITCH_COMPONENT_NAME) ){
                makeTimeDial(me);
            }

            notifyObservers();
        }

        public void makeTimeDial(Entity e) {
            TimedSwitchComponent timeComp = ( TimedSwitchComponent )e.getComponent( GlobalVars.TIMED_SWITCH_COMPONENT_NAME );
            PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            ColliderComponent colComp = ( ColliderComponent )e.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );

            float extraW = 6;
            float extraH = 6;

            float x = colComp.getX( posComp );
            float y = colComp.getY( posComp );
            float w = colComp.width + extraW;
            float h = colComp.height + extraH;
            float time = timeComp.baseTime;

            e.level.makeTimeDial( x, y, w, h, time, false );

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
