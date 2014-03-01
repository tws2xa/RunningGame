using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace RunningGame.Systems {
    [Serializable()]
    public class InputSystem : GameSystem {

        /*
        public bool rightKeyDown = false;
        public bool rightKeyPressed = false;
        public bool rightKeyUp = false;

        public bool leftKeyDown = false;
        public bool leftKeyPressed = false;
        public bool leftKeyUp = false;

        public bool upKeyDown = false;
        public bool upKeyPressed = false;
        public bool upKeyUp = false;
        */

        public Dictionary<Keys, KeyBools> myKeys = new Dictionary<Keys, KeyBools>();

        public float mouseX = 0;
        public float mouseY = 0;
        public bool mouseLeftClick = false;
        public int mouseLeftClickCounter = 0;
        public bool mouseRightClick = false;
        public int mouseRightClickCounter = 0;

        List<string> requiredComponents = new List<string>();
        Level level;

        public InputSystem( Level level ) {

            this.level = level; //Always have this

            myKeys.Add( GlobalVars.KEY_JUMP, new KeyBools() );
            myKeys.Add( GlobalVars.KEY_LEFT, new KeyBools() );
            myKeys.Add( GlobalVars.KEY_RIGHT, new KeyBools() );
            myKeys.Add( GlobalVars.KEY_DOWN, new KeyBools() );

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
            foreach ( KeyBools b in myKeys.Values ) {
                if ( b.down ) {
                    b.downTimer += 1;
                    if ( b.downTimer > 0 ) {
                        b.downTimer = 0;
                        b.down = false;
                    }
                }
                if ( b.up ) {
                    b.upTimer += 1;
                    if ( b.upTimer > 0 ) {
                        b.upTimer = 0;
                        b.up = false;
                    }
                }
            }

            if ( mouseLeftClick ) {
                mouseLeftClickCounter++;
                if ( mouseLeftClickCounter > 0 ) {
                    mouseLeftClickCounter = 0;
                    mouseLeftClick = false;
                }
            }
            if ( mouseRightClick ) {
                mouseRightClickCounter++;
                if ( mouseRightClickCounter > 0 ) {
                    mouseRightClickCounter = 0;
                    mouseRightClick = false;
                }
            }
        }
        //----------------------------------------------------------------------------------------------

        //Input
        public void KeyDown( KeyEventArgs e ) {
            if ( myKeys.ContainsKey( e.KeyData ) ) {
                if ( !myKeys[e.KeyData].down ) //If it's just been pressed, set down to true
                    myKeys[e.KeyData].down = true;
                myKeys[e.KeyData].pressed = true;
            }
        }
        public void KeyUp( KeyEventArgs e ) {
            if ( myKeys.ContainsKey( e.KeyData ) ) {

                myKeys[e.KeyData].pressed = false;
                myKeys[e.KeyData].up = true;

            }
        }
        public void MouseClick( MouseEventArgs e ) {
            if ( e.Button == MouseButtons.Left ) {
                mouseX = e.X;
                mouseY = e.Y;
                mouseLeftClick = true;
            } else if ( e.Button == MouseButtons.Right ) {
                mouseX = e.X;
                mouseY = e.Y;
                mouseRightClick = true;
            }
        }
        public void MouseMoved( MouseEventArgs e ) {
            mouseX = e.X;
            mouseY = e.Y;
        }

        public void addKey( Keys key ) {
            if(!myKeys.ContainsKey(key))
                myKeys.Add( key, new KeyBools() );
        }


    }

    //Holds 3 bools, pressed, down, and up.
    //Pressed = just pressed
    //Down = Held down
    //Up = Just released
    public class KeyBools {
        public bool pressed = false;
        public bool down = false;
        public bool up = false;

        public int downTimer = 0;
        public int upTimer = 0;
    }
}
