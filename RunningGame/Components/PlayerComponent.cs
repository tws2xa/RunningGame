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

        //Key bindings
        public Keys jumpKey {get; set;}
        public Keys leftKey { get; set; }
        public Keys rightKey { get; set; }

        public PlayerComponent()
        {
            this.componentName = GlobalVars.PLAYER_COMPONENT_NAME;

            //Default Key bindings
            //JumpKey = Keys.Space;

            //Platformer Controls
            jumpKey = Keys.W;
            leftKey = Keys.A;
            rightKey = Keys.D;

        }
        

    }
}
