using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Entities;

namespace RunningGame.Components {
    [Serializable()]
    public class VisionInputComponent : Component {

        public float platformerMoveSpeed { get; set; }
        public VisionOrb vision { get; set; }

        public VisionInputComponent( Entity VisionOrb ) {
            this.componentName = GlobalVars.VISION_ORB_INPUT_COMPONENT_NAME;

            this.vision = ( VisionOrb )vision;
            platformerMoveSpeed = 150f;
        }



    }
}
