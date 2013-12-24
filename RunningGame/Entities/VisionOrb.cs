using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame.Entities {
    [Serializable()]
    public class VisionOrb : Entity {

        int defaultWidth = 20;
        int defaultHeight = 20;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public VisionOrb(Level level, float x, float y) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }
        public VisionOrb(Level level, int id, float x, float y) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y) {
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
            drawComp.addSprite("Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Other.WhiteSquare.png", "Main");
            drawComp.setSprite("Main"); //Set image to active image

            /* ANIMATION COMPONENT - Does it need animating?
             * The float that this reads in is the amount of time (in seconds) between frames.
             * So, if it was 5, you would be on one frame for 5 seconds, then switch to the next, then 5 seconds later
             * It'd switch to the next etc, etc...
             */
            addComponent(new AnimationComponent(0.05f));


            List<string> anim = new List<string>()
            {
                "Artwork.Foreground.Orb0",
                "Artwork.Foreground.Orb1",
                "Artwork.Foreground.Orb2",
                "Artwork.Foreground.Orb3",
                "Artwork.Foreground.Orb4",
                "Artwork.Foreground.Orb3",
                "Artwork.Foreground.Orb2",
                "Artwork.Foreground.Orb1"
            };

            List<string> animDefaults = new List<string>()
            {
                "RunningGame.Resources.Artwork.Foreground.Orb0.png",
                "RunningGame.Resources.Artwork.Foreground.Orb1.png",
                "RunningGame.Resources.Artwork.Foreground.Orb2.png",
                "RunningGame.Resources.Artwork.Foreground.Orb3.png",
                "RunningGame.Resources.Artwork.Foreground.Orb4.png",
                "RunningGame.Resources.Artwork.Foreground.Orb3.png",
                "RunningGame.Resources.Artwork.Foreground.Orb2.png",
                "RunningGame.Resources.Artwork.Foreground.Orb1.png"
            };

            drawComp.addAnimatedSprite(anim, animDefaults, "MainAnim");
            drawComp.setSprite("MainAnim");
            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent(new VelocityComponent(0, 0), true);

            /*VISION COMPONENT
             */
            addComponent(new VisionInputComponent(this), true);

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.VISION_COLLIDER_TYPE), true);

            /*HEALTH COMPONENT - Does it have health, can it die?
             *Parameters: maxHealth, startingHealth, draw a health bar?, recharge amount, recharge time
             *Basically, every rechargeTime, the entity regenerates rechargeAmount
             */
            addComponent(new HealthComponent(100, 100, true, 5), true);

            //Edge of screen component
            addComponent(new ScreenEdgeComponent(1, 1, 1, 1), true);
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
