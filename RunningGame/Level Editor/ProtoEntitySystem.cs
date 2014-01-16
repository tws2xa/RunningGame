using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using System.Drawing;

namespace RunningGame.Level_Editor {
    [Serializable()]
    public class ProtoEntitySystem : GameSystem {

        List<string> requiredComponents = new List<string>();
        CreationLevel level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public ProtoEntitySystem( CreationLevel level ) {

            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }

        //Must have this. Same for all Systems.
        public override Level GetActiveLevel() {
            return level;
        }

        public override void Update( float deltaTime ) {
            if ( level.vars.protoEntity != null ) {
                PositionComponent posComp = ( PositionComponent )level.vars.protoEntity.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                PointF p;
                if ( level.vars.gridLock )
                    p = getGridPoint( level.getInputSystem().mouseX, level.getInputSystem().mouseY );
                else
                    p = new PointF( level.getInputSystem().mouseX, level.getInputSystem().mouseY );
                level.getMovementSystem().changePosition( posComp, p.X, p.Y, false );
            }
        }

        //-----------------------------------------------------------------------------------------------

        public PointF getGridPoint( float x, float y ) {
            float xLoc = x - ( x % level.vars.gridSize );
            float YLoc = y - ( y % level.vars.gridSize );
            return new PointF( xLoc, YLoc );
        }


    }
}
