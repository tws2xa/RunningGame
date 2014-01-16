using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;
using System.Drawing;

namespace RunningGame.Level_Editor {

    [Serializable()]
    public class ProtoEntity : Entity {

        float myWidth = 10;
        float myHeight = 10;
        int protoAlpha = 150;

        public Type myEntType { get; set; } //Entity type that the proto-entity represents

        Bitmap myImg;

        //-------------------------------------------Constructors--------------------------------------------

        public ProtoEntity( Level level, Type entityType ) {
            initialize( level, new Random().Next( Int32.MinValue, Int32.MaxValue ), entityType );
        }
        public ProtoEntity( Level level, int id, Type entityType ) {
            initialize( level, id, entityType );
        }


        public void initialize( Level level, int id, Type entityType ) {

            myEntType = entityType;

            //First create an entity of type entityType
            Entity e = ( Entity )Activator.CreateInstance( entityType, level, 0, 0 );

            if ( e.hasComponent( GlobalVars.ANIMATION_COMPONENT_NAME ) )
                e.removeComponent( GlobalVars.ANIMATION_COMPONENT_NAME );

            PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            DrawComponent drawComp = ( DrawComponent )e.getComponent( GlobalVars.DRAW_COMPONENT_NAME );

            this.level = level;


            if ( posComp != null ) {
                myWidth = posComp.width;
                myHeight = posComp.height;
            }

            if ( drawComp != null ) {
                if ( drawComp.sizeLocked ) {
                    lock ( drawComp.getImage() )
                        myImg = new Bitmap( drawComp.getImage() );
                } else {
                    lock ( drawComp.getImage() )
                        myImg = new Bitmap( drawComp.getImage(), new Size( ( int )myWidth, ( int )myHeight ) );
                }

                //Make slightly transparent
                for ( int x = 0; x < myImg.Width; x++ ) {
                    for ( int y = 0; y < myImg.Height; y++ ) {
                        if ( myImg.GetPixel( x, y ).A > protoAlpha )
                            myImg.SetPixel( x, y, Color.FromArgb( protoAlpha, myImg.GetPixel( x, y ) ) );
                    }
                }
            } else {
                //Just solid blue box
                myImg = new Bitmap( ( int )myWidth, ( int )myHeight );
                Graphics g = Graphics.FromImage( myImg );
                g.FillRectangle( Brushes.Blue, new Rectangle( 0, 0, myImg.Size.Width, myImg.Size.Height ) );
            }

            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            addMyComponents( 0, 0 );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents( float x, float y ) {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent( new PositionComponent( x, y, myWidth, myHeight, this ) );

            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            addComponent( new DrawComponent( myImg, "Main", myWidth, myHeight, level, true ) );

        }

        public override void revertToStartingState() {
            //Nada
        }
    }
}
