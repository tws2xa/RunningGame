using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    [Serializable()]
    public class DirectionalComponent : Component {

        int dir = 0;

        public DirectionalComponent( int dir ) {
            this.componentName = GlobalVars.DIRECTION_COMPONENT_NAME;
            this.dir = dir;
        }

        public int getDir() {
            return dir;
        }

        public void setUp() {
            dir = 0;
        }
        public void setRight() {
            dir = 1;
        }
        public void setDown() {
            dir = 2;
        }
        public void setLeft() {
            dir = 3;
        }

        public bool isUp() {
            return dir == 0;
        }
        public bool isRight() {
            return dir == 1;
        }
        public bool isDown() {
            return dir == 2;
        }
        public bool isLeft() {
            return dir == 3;
        }
    }
}
