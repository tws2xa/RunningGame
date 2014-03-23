using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    class CheckPointComponent:Component {

        bool activated = false;

        string checkedImageName = "checked";
        string uncheckedImageName = "unchecked";

        Entity myEnt;

        public CheckPointComponent(string uncheckedImageName, string checkedImageName, Entity ent) {
            this.componentName = GlobalVars.CHECKPOINT_COMPONENT_NAME;

            myEnt = ent;
            this.uncheckedImageName = uncheckedImageName;
            this.checkedImageName = checkedImageName;
        }

        public void setActive( bool val ) {
            this.activated = val;
            DrawComponent drawComp = ( DrawComponent )myEnt.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            if ( drawComp == null ) {
                Console.WriteLine( "Error: Trying to activate a switch with no draw component." );
                return;
            }
            if ( isActive() ) {
                drawComp.setSprite( checkedImageName );
            } else {
                drawComp.setSprite( uncheckedImageName );
            }
        }

        public bool isActive() {
            return activated;
        }

    }
}
