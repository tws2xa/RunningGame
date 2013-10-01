using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    class ScreenWrapComponent:Component
    {

        public bool left, right, up, down;

        public ScreenWrapComponent(bool left, bool right, bool up, bool down)
        {

            this.componentName = GlobalVars.SCREEN_WRAP_COMPONENT_NAME;

            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
        }
    }
}
