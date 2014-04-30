using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RunningGame.Components;
using System.Collections;
using RunningGame.Level_Editor;
using RunningGame.Entities;

namespace RunningGame.Systems {

    /*
     * The draw system checks for entities with a position and draw component.
     * Basically it goes through all entities with these components and draws them.
     */

    [Serializable()]
    public class DrawSystem : GameSystem {

        Graphics g;
        Level level;
        CreationLevel creatLev = null;
        //public View mainView;
        public List<View> views = new List<View>();
        List<string> requiredComponents = new List<string>();

        /************FLASH STUFF Begins here*/
        float flashTime = 0;
        int alpha = 0; // have to figure out the right number/ratio
        float deltaAlpha = 0;
        Color flashColor = Color.White;
        SolidBrush flashBrush = new SolidBrush( Color.FromArgb( 0, Color.White ) ); //white is completely arbitrary
        Boolean flashDirection = true; // if truue, then the flash direction is going forwar, (fades into color), if false
        //it means that it's fading out
        //alpha is a proportion 
        //new solid brush color and alpha 
        //set color, then set that timer.
        //draw function, if that timer is greater than 0, make alpha proportion, decrease time in update.

        /*FLASH STUFF ends here*/

        //Text Flash
        public float deltaInAlpha = 0;
        public float deltaOutAlpha = 0;
        public int textState = -1;
        public float textTimer = -1;
        public SolidBrush textBrush = new SolidBrush(Color.FromArgb(0, Color.White));
        public string text = "";
        public Font textFont = SystemFonts.DefaultFont;
        public bool textShadow = true;

        public bool drawDebugStuff = false;
        public List<PointF> debugLines = new List<PointF>();
        public List<PointF> debugPoints = new List<PointF>();

        public Dictionary<string, Color> constMessages = new Dictionary<string, Color>();

        [NonSerialized]
        Pen selectedEntBorderColor = Pens.Red;
        [NonSerialized]
        Brush selectedEntFillColor = new SolidBrush( Color.FromArgb( 100, Color.CornflowerBlue ) );

        //View miniMap;

        bool miniMapOn = false;

        public DrawSystem( Graphics g, Level level ) {

            this.textFont = level.displayFont;
            
            //Required Components
            requiredComponents.Add( GlobalVars.DRAW_COMPONENT_NAME ); //Draw Component
            requiredComponents.Add( GlobalVars.POSITION_COMPONENT_NAME ); //Position Component
            
            this.g = g;
            this.level = level;

            if ( level is CreationLevel ) {
                creatLev = ( CreationLevel )level;
            }

            View mainView = new View( 0, 0, level.cameraWidth, level.cameraHeight, 0, 0, level.cameraWidth, level.cameraHeight, level, level.getPlayer() );
            addView( mainView );

            if ( miniMapOn ) {
                View miniMap = new View( 0, 0, level.levelWidth, level.levelHeight, level.cameraWidth - 210, 10, 200, 100, level );
                miniMap.bkgBrush = Brushes.DarkTurquoise;
                miniMap.hasBorder = true;
                addView( miniMap );
            }

        }
        public DrawSystem( Graphics g, CreationLevel level ) {
            //Required Components
            requiredComponents.Add( GlobalVars.DRAW_COMPONENT_NAME ); //Draw Component
            requiredComponents.Add( GlobalVars.POSITION_COMPONENT_NAME ); //Position Component

            this.g = g;
            this.level = level;

            if ( level is CreationLevel ) {
                creatLev = ( CreationLevel )level;
            }

            View mainView = new View( 0, 0, level.levelWidth, level.levelHeight, 0, 0, level.levelWidth, level.levelHeight, level );
            addView( mainView );

            if ( miniMapOn ) {
                View miniMap = new View( 0, 0, level.levelWidth, level.levelHeight, level.cameraWidth - 100, 10, 200, 100, level );
                miniMap.bkgBrush = Brushes.DarkTurquoise;
                miniMap.hasBorder = true;
                addView( miniMap );
            }
        }
        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }
        public override Level GetActiveLevel() {
            return level;
        }

        public override void Update( float deltaTime ) {

            if ( views[0].followEntity == null ) {
                views[0].setFollowEntity( level.getPlayer() );
            }


            //*this part takes care of flashes on the screen
            if ( flashTime > 0 ) {
                flashTime = flashTime - deltaTime;
                if ( flashDirection )
                    alpha += ( int )( deltaAlpha * deltaTime );
                else {
                    //flashDirection is false
                    alpha -= ( int )( deltaAlpha * deltaTime );
                }
                if ( alpha >= 255 ) {
                    alpha = 255;
                    flashDirection = false;
                }

                if ( alpha <= 0 ) {
                    flashTime = 0;
                    alpha = 0;
                }

                flashBrush.Color = Color.FromArgb( alpha, flashColor );

            } else {
                alpha = 0;
            }

            if (textState == 0)
            {
                int textAlpha = textBrush.Color.A;
                textAlpha += (int)(deltaInAlpha * deltaTime);
                if (textAlpha >= 255)
                {
                    textState = 1;
                    textAlpha = 255;
                }
                textBrush.Color = Color.FromArgb(textAlpha, textBrush.Color.R, textBrush.Color.G, textBrush.Color.B);

            }
            else if (textState == 1)
            {
                textTimer -= deltaTime;
                if (textTimer <= 0)
                {
                    textTimer = -1;
                    textState = 2;
                }

            }
            else if(textState == 2)
            {
                int textAlpha = textBrush.Color.A;
                textAlpha -= (int)(deltaOutAlpha * deltaTime);
                if (textAlpha <= 0)
                {
                    textState = -1;
                    textAlpha = 0;
                }
                textBrush.Color = Color.FromArgb(textAlpha, textBrush.Color.R, textBrush.Color.G, textBrush.Color.B);
            }

            /*
            if (flashTime < 0)
                flashTime = 0;
            */
            //color decrease by delta time
            // total time passed and total time for the alpha
            // might want to do make flash in the draw system
            //draw a rectangle
            //total time 
            //Update views
            foreach ( View v in views ) {
                v.Update();
            }
        }


        public float getFlashTime() {
            return flashTime;
        }
        public Brush getFlashBrush() {
            return flashBrush;
        }
        public void setFlash( Color c, float time ) {

            ( ( SolidBrush )flashBrush ).Color = Color.FromArgb( 0, c );
            flashTime = time;
            deltaAlpha = ( ( 255 * 2 ) / ( time ) ); // the 20 is arbitary for now since I can't figure out how to set the ratio, since I don't know 
            //how to acces delta time from here
            flashDirection = true;
            flashColor = c;
        }

        public void activateTextFlash(string text, Color c, float timerIn, float time, float timerOut)
        {
            if (textState >= 0)
                return;
            this.text = text;
            deltaInAlpha = (255 / timerIn);
            deltaOutAlpha = (255 / timerOut);
            textBrush.Color = Color.FromArgb(0, c);
            textState = 0;
            textTimer = time;
        }

        public void deactivateTextFlash() {
            this.textState = -1;
        }

        public void beginConstText( string text, Color c ) {
            if ( !constMessages.ContainsKey( text ) ) {
                constMessages.Add( text, c );
            }
        }
        public void endConstText( string text, Color c, float fade=0.01f ) {
            if ( constMessages.ContainsKey( text ) ) {
                activateTextFlash( text, constMessages[text], 0.01f, 0.01f, fade );
                constMessages.Remove( text );
            }
        }
        //Explanantion of g
        //g is a brush for an image
        //use g to draw on that image
        // that image is to draw on the window, passed down. 
        // g goes from levelWindow(form spring), to the game, to the level, to the draw system, to the view
        // what you use to draw things to the image
        // g is a property(essentially the image)
        // every image has a graphics object associated with it, latched on it. 
        public void Draw( Graphics g ) {
            List<Entity> entityList = getApplicableEntities();

            //this is where all the entities are drawn, so modify this for depth
            foreach ( View v in views ) {
                v.Draw( g, entityList );
            }


            //Draw text flash
            if ( level.sysManager != null && textState >= 0 ) {
                StringFormat centerFormat = new StringFormat();
                centerFormat.Alignment = StringAlignment.Center;
                centerFormat.LineAlignment = StringAlignment.Center;
                PointF textPosition = new PointF( getMainView().displayX + getMainView().displayWidth / 2, getMainView().displayY + getMainView().displayHeight / 4 );
                if ( textShadow ) {
                    float shadowOffsetX = 1.2f;
                    float shadowOffsetY = 1.0f;
                    int maxShadowOpacity = 170;
                    SolidBrush shadowBrush = ( SolidBrush )level.sysManager.drawSystem.textBrush.Clone();
                    shadowBrush.Color = Color.FromArgb( Math.Min( shadowBrush.Color.A, maxShadowOpacity ), Color.Black );
                    g.DrawString( text, textFont, shadowBrush, textPosition.X + shadowOffsetX, textPosition.Y + shadowOffsetY, centerFormat );
                }
                g.DrawString( text, textFont, textBrush, textPosition.X, textPosition.Y, centerFormat );
            }

            //Draw Const Text
            if ( level.sysManager != null && constMessages.Count > 0 ) {
                Dictionary<string, Color> msgs = level.sysManager.drawSystem.constMessages;
                foreach ( string str in msgs.Keys ) {
                    StringFormat centerFormat = new StringFormat();
                    centerFormat.Alignment = StringAlignment.Center;
                    centerFormat.LineAlignment = StringAlignment.Center;
                    PointF textPosition = new PointF( getMainView().displayX + getMainView().displayWidth / 2, getMainView().displayY + getMainView().displayHeight / 4 );
                    SolidBrush brush = new SolidBrush( msgs[str] );
                    if ( textShadow ) {
                        float shadowOffsetX = 1.2f;
                        float shadowOffsetY = 1.0f;
                        int maxShadowOpacity = 170;
                        SolidBrush shadowBrush = ( SolidBrush )brush.Clone();
                        shadowBrush.Color = Color.FromArgb( Math.Min( shadowBrush.Color.A, maxShadowOpacity ), Color.Black );
                        g.DrawString( str, textFont, shadowBrush, textPosition.X + shadowOffsetX, textPosition.Y + shadowOffsetY, centerFormat );
                    }
                    g.DrawString( str, textFont, brush, textPosition.X, textPosition.Y, centerFormat );
                }
            }

            //Draw fade
            if ( level.sysManager != null && getFlashTime() > 0 ) {
                g.FillRectangle( level.sysManager.drawSystem.getFlashBrush(), new Rectangle( ( int )( getMainView().displayX ), ( int )( getMainView().displayY ),
                ( int )( getMainView().displayWidth ), ( int )( getMainView().displayHeight ) ) );
            }


            /*
            //If you are in the level editor. Box the selected entities
            //Ignore this unless you are playing with level editor
            if (creatLev != null && creatLev.vars.selectedEntity != null)
            {
                foreach (Entity e in creatLev.vars.allSelectedEntities)
                {
                    PositionComponent posComp = (PositionComponent)creatLev.vars.selectedEntity.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    g.FillRectangle(selectedEntFillColor, posComp.x - posComp.width / 2, posComp.y - posComp.height / 2, posComp.width, posComp.height);
                    g.DrawRectangle(selectedEntBorderColor, posComp.x - posComp.width / 2, posComp.y - posComp.height / 2, posComp.width, posComp.height);
                }
            }
             * */

        }

        public void addView( View v ) {
            views.Add( v );
        }

        public View getMainView() {
            return views[0];
        }

        public bool removeView( View plView ) {
            return views.Remove( plView );
        }

        public void gotoJustMainView() {
            View main = views[0];
            views.Clear();
            views.Add( main );
        }
    }
}
