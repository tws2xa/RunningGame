using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Entities;

namespace RunningGame.Components {
    [Serializable()]
    public class PlayerInputComponent : Component {
        public float jumpStrength { get; set; }
        public float playerHorizMoveSpeed { get; set; }
        //public int numAirJumps { get; set; } //number of jumps possible in the air (numAirJumps = 1 means you can double jump)
        public int passedAirjumps { get; set; }
        public Player player { get; set; }

        public PlayerInputComponent( Entity player ) {

            this.componentName = GlobalVars.PLAYER_INPUT_COMPONENT_NAME;

            this.player = ( Player )player;
            jumpStrength = -150f;
            playerHorizMoveSpeed = GlobalVars.PLAYER_HORIZ_MOVE_SPEED;
            //numAirJumps = GlobalVars.normNumAirJumps; //number of jumps possible in the air (numAirJumps = 1 means you can double jump)
            passedAirjumps = 2;
        }

    }
}
