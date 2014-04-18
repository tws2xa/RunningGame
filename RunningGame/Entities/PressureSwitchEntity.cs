using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    [Serializable()]
    public class PressureSwitchEntity : Entity {
        float defaultWidth = 20;
        float defaultHeight = 10;

        bool startingState; //Active or non-active at level start?

        //-------------------------------------------Constructors--------------------------------------------
        public PressureSwitchEntity( Level level, float x, float y ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            startingState = false;

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y );
        }
        public PressureSwitchEntity( Level level, int id, float x, float y ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            startingState = false;

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y );
        }
        public PressureSwitchEntity( Level level, int id, float x, float y, bool active ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            startingState = active;

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y );
        }

        //------------------------------------------------------------------------------------------------------------------

        public void addMyComponents( float x, float y ) {

            this.resetOnCheckpoint = false;

            //POSITION COMPONENT - Does it have a position?
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            //DRAW COMPONENT - Does it get drawn to the game world?
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( defaultWidth, defaultHeight, level, false ), true );
            drawComp.addSprite( "Artwork.Foreground.buttonUp", "RunningGame.Resources.Artwork.Foreground.buttonUp11.png", GlobalVars.SWITCH_INACTIVE_SPRITE_NAME );
            drawComp.addSprite( "Artwork.Foreground.buttonDown", "RunningGame.Resources.Artwork.Foreground.buttonDown11.png", GlobalVars.SWITCH_ACTIVE_SPRITE_NAME );
            drawComp.setSprite( GlobalVars.SWITCH_INACTIVE_SPRITE_NAME );

            //COLLIDER - Does it hit things?
            addComponent( new ColliderComponent( this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE), true );

            //Swich Component - Is it a switch? Yes.
            addComponent( new SwitchComponent( startingState ), true );

            //It's a pressure switch! (Timed Switch, Time = 0)
            addComponent( new TimedSwitchComponent( 0 ), true );
        }

        public override void revertToStartingState() {
            SwitchComponent sc = ( SwitchComponent )getComponent( GlobalVars.SWITCH_COMPONENT_NAME );

            PositionComponent posComp = ( PositionComponent )getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            float hDiff = posComp.height;
            level.getMovementSystem().changeHeight( posComp, defaultHeight );
            hDiff = posComp.height - hDiff;
            level.getMovementSystem().teleportToNoCollisionCheck( posComp, posComp.x, posComp.y - hDiff / 2 );

            sc.setActive( false );

        }
    }
}
