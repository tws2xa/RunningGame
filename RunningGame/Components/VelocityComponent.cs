using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{

    //The entity has a velocity
    [Serializable()]
    public class VelocityComponent : Component
    {

        public float x { get; set; }
        public float y { get; set; }

        public VelocityComponent()
        {
            this.componentName = GlobalVars.VELOCITY_COMPONENT_NAME;
            this.x = 0;
            this.y = 0;
        }
        public VelocityComponent(float x, float y)
        {
            this.componentName = GlobalVars.VELOCITY_COMPONENT_NAME;
            this.x = x;
            this.y = y;
        }

        //Set velocity
        public void setVelocity(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        //Add to velocity
        public void incVelocity(float x, float y)
        {
            this.x += x;
            this.y += y;
        }

    }
}
