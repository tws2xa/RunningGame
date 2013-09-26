using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    /*
     * A Template for Creating Components
     */
    class ComponentTemplate:Component //Always extend Component
    {
        //Basically the component just holds variables.
        //Some may even be empty
        

        //Be sure to include a Constructor
        //Inside the constructor you probably want to pass in and set all the variables
        public ComponentTemplate()
        {
            /*
             * Always always ALWAYS set the component name to whatever you have it as in GlobalVars (add one if need be)
             */
            //componentName = GlobalVars.[MY_COMPONENT_NAME];

        }


        //Then add get/set methods for variables
    }
}
