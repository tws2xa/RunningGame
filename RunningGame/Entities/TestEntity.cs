using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;
using System.Collections;

namespace RunningGame.Entities {

    /*
     * Meet the ever so handy test entity!
     * It can be whatever you want it to be.
     */
    [Serializable()]
    public class TestEntity : Entity {

        float defaultWidth = 10;
        float defaultHeight = 10;

        public TestEntity( Level level, float x, float y ) {
            this.level = level;

            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            addMyComponents( x, y );
        }

        public TestEntity( Level level, int id, float x, float y ) {
            this.level = level;

            initializeEntity( id, level );

            addMyComponents( x, y );
        }

        public void addMyComponents( float x, float y ) {
            //Position Component
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            //Draw component
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( defaultWidth, defaultHeight, level, true ), true );
            drawComp.addSprite( "Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Other.WhiteSquare.png", "Main" );
            drawComp.setSprite( "Main" );

            //Velocity Component
            addComponent( new VelocityComponent( 0, 0 ), true );

            //Collider
            addComponent( new ColliderComponent( this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE ), true );

            //Gravity Component
            addComponent( new GravityComponent( 0, GlobalVars.STANDARD_GRAVITY ), true );

        }

        public override void revertToStartingState() {
            PositionComponent posComp = ( PositionComponent )this.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            level.getMovementSystem().changePosition( posComp, posComp.startingX, posComp.startingY, true );
            level.getMovementSystem().changeSize( posComp, posComp.startingWidth, posComp.startingHeight );

            VelocityComponent velComp = ( VelocityComponent )this.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            velComp.x = 0;
            velComp.y = 0;
        }

    }
}
