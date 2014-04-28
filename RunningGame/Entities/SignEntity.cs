using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{
     [Serializable()]
    class SignEntity : Entity
    {
         float defaultWidth = 30;
         float defaultHeight = 30;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public SignEntity( Level level, float x, float y, int msg ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, msg );
        }
        public SignEntity( Level level, int id, float x, float y, int msg ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, msg );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y, int msg)
        {

            string message = "A MSG";

            switch ( msg ) {
                case(0):
                    message = "Left Click to Shoot!";
                    break;
                case(1):
                    message = "Exit the right side of the Level to Continue!";
                    break;
                case(2):
                    message = "You can activate switches by shooting them!";
                    break;
                case(3):
                    message = "Combine powerups in creative\nways for the best effects!";
                    break;
                case(4):
                    message = "Not all enemies are vulnerable to your weapon,\nbut perhaps there are other ways to handle them...";
                    break;
            }

            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);
            addComponent(new SignComponent(message), true);

            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" ONLY png!!
             *First create the component
             *Then add the image
             *Then set the image to the active image
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            // Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite("Artwork.Foreground.Sign", "RunningGame.Resources.Artwork.Foreground.Sign10.png", "Main");
            drawComp.setSprite("Main"); //Set image to active image

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.SIGN_COLLIDER_TYPE), true);
        }
            
            public override void revertToStartingState() {
            
            }
        }
    }
