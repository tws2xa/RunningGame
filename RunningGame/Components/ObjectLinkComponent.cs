using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{

    /*
     * This component links one object to another.
     * "Linked" basically means whichever object has this component
     * Will move the same amount that the object it's linked to moves.
     */

    [Serializable()]
    public class ObjectLinkComponent:Component
    {

        public ObjectLinkComponent(Entity myEntity, Entity linkedTo)
        {
            this.componentName = GlobalVars.OBJECT_LINK_COMPONENT_NAME;
        }

    }
}
