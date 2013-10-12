using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame
{

    /*
     * The component class from which all other components are born.
     * 
     * Some components
     * Input - The object receives input
     * Position - Stores a location, width, and height for the object
     * Velocity - Stores x and y velocities for an object
     * Draw - The object has a sprite and will be drawn
     * Collider - The object will collide with other objects
     * Gravity - The object is affected by gravity
     * 
     */


    public abstract class Component
    {
        public string componentName = GlobalVars.NULL_COMPONENT_NAME;

        //Two components are equal if they have the same component name
        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType()) return false;

            Component other = (Component)obj;
            return (other.componentName == this.componentName);
        }

        public override string ToString()
        {
            return componentName;
        }
    }
}
