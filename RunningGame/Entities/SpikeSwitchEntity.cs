using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {

    [Serializable()]
    class SpikeSwitchEntity : Entity {

        float defaultWidth = 10;
        float defaultHeight = 20;

        bool startingState; //Active or non-active at level start?

        //-------------------------------------------Constructors--------------------------------------------
        public SpikeSwitchEntity( Level level, float x, float y, float time ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            startingState = false;

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, time );
        }
        public SpikeSwitchEntity( Level level, int id, float x, float y, float time ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            startingState = false;

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, time );
        }
        public SpikeSwitchEntity( Level level, int id, float x, float y, bool active, float time ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            startingState = active;

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, time );
        }

        //------------------------------------------------------------------------------------------------------------------

        public void addMyComponents( float x, float y, float time ) {

            this.resetOnCheckpoint = false;

            //POSITION COMPONENT - Does it have a position?
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            //DRAW COMPONENT - Does it get drawn to the game world?
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( defaultWidth, defaultHeight, level, true ), true );
            drawComp.addSprite( "Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Other.WhiteSquare.png", GlobalVars.SWITCH_INACTIVE_SPRITE_NAME );
            drawComp.addSprite( "Artwork.Other.DebugSquare2", "RunningGame.Resources.Artwork.Other.DebugSquare2.png", GlobalVars.SWITCH_ACTIVE_SPRITE_NAME );
            drawComp.setSprite( GlobalVars.SWITCH_INACTIVE_SPRITE_NAME );

            //COLLIDER - Does it hit things?
            addComponent( new ColliderComponent( this, GlobalVars.SPIKE_SWITCH_COLLIDER ), true );

            //Swich Component - Is it a switch? Yes.
            addComponent( new SwitchComponent( startingState ), true );

            if ( time > -1) {
                //Timed Switch Component It's a timed switch!
                addComponent( new TimedSwitchComponent( time ), true );
            }
        }

        public override void revertToStartingState() {
            SwitchComponent sc = ( SwitchComponent )getComponent( GlobalVars.SWITCH_COMPONENT_NAME );
            sc.setActive( startingState, this );

            DrawComponent drawComp = ( DrawComponent )getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            if ( startingState )
                drawComp.setSprite( GlobalVars.SWITCH_ACTIVE_SPRITE_NAME );
            else
                drawComp.setSprite( GlobalVars.SWITCH_INACTIVE_SPRITE_NAME );
        }

    }
}
