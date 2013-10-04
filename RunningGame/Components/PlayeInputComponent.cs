using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Entities;

namespace RunningGame.Components
{
    class PlayerInputComponent:Component
    {
        public float jumpStrength { get; set; }
        public float platformerMoveSpeed { get; set; }
        public int numAirJumps { get; set; } //number of jumps possible in the air (numAirJumps = 1 means you can double jump)
        public int passedAirjumps { get; set; }
        public Player player { get; set; }

        public PlayerInputComponent(Entity player)
        {

            this.componentName = GlobalVars.PLAYER_INPUT_COMPONENT_NAME;

            this.player = (Player)player;
            jumpStrength = -150f;
            platformerMoveSpeed = 150f;
            numAirJumps = 2; //number of jumps possible in the air (numAirJumps = 1 means you can double jump)
            passedAirjumps = 0;
        }

    }
}
