using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RunningGame.Entities;
using RunningGame.Components;

namespace RunningGame {
    class CheckPointData {

        Level level;
        PointF playerLoc;
        bool preColor = false;

        Dictionary<int, Entity> destroyedStartEnts = new Dictionary<int, Entity>();

        public CheckPointData(Level level) {
            this.level = level;
            if ( level.levelNum == 1 ) {
                preColor = true;
            }
            updatePlayerLoc();
        }

        public void setCheckpoint(CheckPointEntity checkpoint ) {
            level.hasHadCheckpoint = true;
            destroyedStartEnts = new Dictionary<int, Entity>( GlobalVars.removedStartingEntities );
            if ( level.levelNum == 1 ) {
                preColor = !level.colorOrbObtained;
            }
            setSwitchCheckpoints();
            updatePlayerLoc();
        }

        public void setSwitchCheckpoints() {
            foreach ( Entity e in GlobalVars.nonGroundEntities.Values ) {
                if ( e is SwitchEntity ) {
                    SwitchEntity mySwitch = ( SwitchEntity )e;
                    SwitchComponent switchComp = (SwitchComponent)e.getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
                    mySwitch.lastCheckpointState = switchComp.active;
                }
            }
        }

        public void updatePlayerLoc() {
            Player player = level.getPlayer();
            if ( player == null ) {
                Console.WriteLine( "Error: Setting checkpoint with no player" );
                return;
            }
            PositionComponent posComp = ( PositionComponent )player.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            if ( posComp == null ) {
                Console.WriteLine( "Error: Trying to set checkpoint, but player has no position component" );
                return;
            }

            playerLoc = new PointF( posComp.getLocAsPoint().X, posComp.getLocAsPoint().Y );
        }

        public Dictionary<int, Entity> getRemovedEnts() {
            return destroyedStartEnts;
        }

        public bool colorOrbObtained() {
            return ( level.levelNum != 1 || !this.preColor );
        }

        public PointF getPlayerLoc() {
            return playerLoc;
        }

    }
}
