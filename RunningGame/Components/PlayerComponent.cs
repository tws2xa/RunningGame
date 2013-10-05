using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;

namespace RunningGame.Components
{

    //The entity is the player.

    class PlayerComponent:Component
    {


        public PlayerComponent()
        {
            this.componentName = GlobalVars.PLAYER_COMPONENT_NAME;
        }
        

    }
}
