using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    [Serializable()]
    public class TimeDialEntity : Entity{

        float defaultWidth = 10;
        float defaultHeight = 10;
        
        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public TimeDialEntity( Level level, float x, float y, float width, float height, float time, bool fill ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            //Add the components.
            //Leave this for all entities.
            this.defaultWidth = width;
            this.defaultHeight = height;
            addMyComponents( x, y, time, fill );
        }
        public TimeDialEntity( Level level, int id, float x, float y, float width, float height, float time, bool fill ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            //Add the components.
            //Leave this for all entities.
            this.defaultWidth = width;
            this.defaultHeight = height;
            addMyComponents( x, y, time, fill );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents( float x, float y, float time, bool fill ) {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);

            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" ONLY png!!
             *First create the component
             *Then add the image
             *Then set the image to the active image
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite("Artwork.Foreground.Grass", "RunningGame.Resources.Artwork.Foreground.Grass11.png", "Bkp");



            List<string> animMain = new List<string>() {
                "Artwork.Foreground.TimeDial.CTP1",
                "Artwork.Foreground.TimeDial.CTP2",
                "Artwork.Foreground.TimeDial.CTP3",
                "Artwork.Foreground.TimeDial.CTP4",
                "Artwork.Foreground.TimeDial.CTP5",
                "Artwork.Foreground.TimeDial.CTP6",
                "Artwork.Foreground.TimeDial.CTP7",
                "Artwork.Foreground.TimeDial.CTP8",
                "Artwork.Foreground.TimeDial.CTP9",
                "Artwork.Foreground.TimeDial.CTP10",
                "Artwork.Foreground.TimeDial.CTP11",
                "Artwork.Foreground.TimeDial.CTP12"
            };
            List<string> animDefaults = new List<string>() {
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP0.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP1.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP2.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP3.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP4.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP5.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP6.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP7.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP8.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP9.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP10.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP11.png",
                "RunningGame.Resources.Artwork.Foreground.TimeDial.CTP12.png"
            };

            if ( !fill ) {
                animMain.Reverse();
                animDefaults.Reverse();
            }

            drawComp.addAnimatedSprite( animMain, animDefaults, "Main" );

            drawComp.setSprite("Main"); //Set image to active image

            float animationTime = time / animDefaults.Count();

            /* ANIMATION COMPONENT - Does it need animating?
             * The float that this reads in is the amount of time (in seconds) between frames.
             * So, if it was 5, you would be on one frame for 5 seconds, then switch to the next, then 5 seconds later
             * It'd switch to the next etc, etc...
             */
            AnimationComponent animComp = (AnimationComponent)addComponent(new AnimationComponent(animationTime), true);
            animComp.destroyAfterCycle = true;
        }

        //You must have this, but it may be empty.
        //What should the entity do in order to revert to its starting state?
        //Common things are:
        //Set position back to startingX and startingY
        //NOTE: If doing this, you probably want to use the MovementSystem's teleportToNoCollisionCheck() method
        //rather than the usual changePosition()
        //Set velocity to 0 in both directions
        //Note: Some things, like ground, dont move, and really don't need anything here.
        //Note: Some things, like a bullet, won't ever exist at the start of a level, so you could probably leave this empty.
        public override void revertToStartingState() {
            //Stuff
        }


    }
}
