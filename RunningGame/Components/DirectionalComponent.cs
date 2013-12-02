using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    class DirectionalComponent:Component
    {

        public int dir = 0;

        public DirectionalComponent(int dir)
        {
            this.componentName = GlobalVars.DIRECTION_COMPONENT_NAME;
            this.dir = dir;
        }

    }
}
