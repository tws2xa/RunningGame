using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    //Signals that an object is in the level background.
    //For now just used for the background image
    class BackgroundComponent : Component {

        public BackgroundComponent() {
            componentName = GlobalVars.BACKGROUND_COMPONENT_NAME;
        }

    }

}
