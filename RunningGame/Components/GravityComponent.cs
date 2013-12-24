using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {

    //The entity is affected by gravity

    [Serializable()]
    public class GravityComponent : Component {

        //x and y gravity
        public float x { get; set; }
        public float y { get; set; }

        public GravityComponent() {
            this.componentName = GlobalVars.GRAVITY_COMPONENT_NAME;
            this.x = 0;
            this.y = GlobalVars.STANDARD_GRAVITY;
        }
        public GravityComponent(float x, float y) {
            this.componentName = GlobalVars.GRAVITY_COMPONENT_NAME;
            this.x = x;
            this.y = y;
        }

        public void setGravity(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public void incGravity(float x, float y) {
            this.x += x;
            this.y += y;
        }
    }
}
