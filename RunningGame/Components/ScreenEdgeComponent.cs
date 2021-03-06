﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    [Serializable()]
    public class ScreenEdgeComponent : Component {

        //0 - Nothing
        //1 - Stop
        //2 - Wrap
        //3 - Destroy!
        //4 - End Da Level
        //5 - Death.
        public int left, right, up, down;

        public ScreenEdgeComponent( int left, int right, int up, int down ) {

            this.componentName = GlobalVars.SCREEN_EDGE_COMPONENT_NAME;

            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
        }
    }
}
