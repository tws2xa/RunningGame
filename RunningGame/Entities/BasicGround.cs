using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;

namespace RunningGame.Entities {

    /*
     * The basic ground entity is just the normal dirt/grass
     * You see all over the test levels.
     * It's color code in the level editor is pure black: RGB(0, 0, 0)
     */


    [Serializable()]
    public class BasicGround : Entity {

        float defaultWidth = 11f;
        float defaultHeight = 11f;

        string dirtSpriteName = "Dirt";
        string grassSpriteName = "Grass";

        public BasicGround( Level level, float x, float y ) {
            this.level = level;

            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            addMyComponents( x, y, defaultWidth, defaultHeight );
        }
        public BasicGround( Level level, float x, float y, float width, float height ) {
            this.level = level;

            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );


            addMyComponents( x, y, width, height );
        }
        public BasicGround( Level level, int id, float x, float y, float width, float height ) {
            this.level = level;

            initializeEntity( id, level );


            addMyComponents( x, y, width, height );
        }


        public void addMyComponents( float x, float y, float width, float height ) {
            //Position Component
            addComponent( new PositionComponent( x, y, width, height, this ), true );

            if ( !GlobalVars.fullForegroundImage) {
                //Draw component
                DrawComponent drawComp = new DrawComponent( defaultWidth, defaultHeight, level, true );
                drawComp.addSprite( "Artwork.Foreground.Grass.Dirt", "RunningGame.Resources.Artwork.Foreground.Grass.Dirt61.png", dirtSpriteName );
                string grassFile = "placeholder";
                if ( !GlobalVars.simpleGround ) {
                    grassFile = "Artwork.Foreground.Grass.Grass0";
                    Random rand = new Random( this.randId );
                    switch ( rand.Next( 0, 5 ) ) {
                        case ( 1 ):
                            grassFile = "Artwork.Foreground.Grass.Grass1";
                            break;
                        case ( 2 ):
                            grassFile = "Artwork.Foreground.Grass.Grass2";
                            break;
                        case ( 3 ):
                            grassFile = "Artwork.Foreground.Grass.Grass3";
                            break;
                        case ( 4 ):
                            grassFile = "Artwork.Foreground.Grass.Grass4";
                            break;
                    }
                } else {
                    grassFile = "Artwork.Foreground.Grass.SimpGrass";
                }

                drawComp.addSprite( grassFile, "RunningGame.Resources.Artwork.Foreground.Grass61.png", grassSpriteName );
                drawComp.setSprite( dirtSpriteName );
                addComponent( drawComp, true );
            }
            //Collider
            ColliderComponent c = ( ColliderComponent )addComponent( new ColliderComponent( this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE, defaultWidth-1, defaultHeight-1 ) );
            c.hasTransparentPixels = false;
            c.collideOnNoSprite = true;
        }


        public override void revertToStartingState() {
            // Do nothing. Ground does not change in game.
        }

        public void changeSprite( bool dirt ) {
            if ( !this.hasComponent( GlobalVars.DRAW_COMPONENT_NAME ) ) return;
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            
            if ( dirt ) {
                drawComp.setSprite( dirtSpriteName );
            } else {
                drawComp.setSprite( grassSpriteName );
            }
        }

        public bool isGrass() {
            if ( !this.hasComponent( GlobalVars.DRAW_COMPONENT_NAME ) ) return false;
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );

            return (drawComp.activeSprite == grassSpriteName ||drawComp.activeSprite == drawComp.getPrecolorImageName(grassSpriteName));

        }
    }
}
