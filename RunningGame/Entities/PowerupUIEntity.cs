﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame.Entities
{
    public class PowerupUIEntity:Entity
    {

        float defaultWidth = 50;
        float defaultHeight = 50;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public PowerupUIEntity(Level level, float x, float y)
        {
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
        public PowerupUIEntity(Level level, int id, float x, float y)
        {
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
        public void addMyComponents(float x, float y)
        {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));

            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" ONLY png!!
             *First create the component
             *Then add the image
             *Then set the image to the active image
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true));
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite("Artwork.Other.Bounce", "RunningGame.Resources.Artwork.Other.PurpleBounce.png", GlobalVars.POWERUP_INDICATOR_BOUNCE);
            drawComp.addSprite("Artwork.Other.Speedy", "RunningGame.Resources.Artwork.Other.BlueSpeedy.png", GlobalVars.POWERUP_INDICATOR_SPEEDY);
            drawComp.addSprite("Artwork.Other.Spawn", "RunningGame.Resources.Artwork.Other.OrangeSpawn.png", GlobalVars.POWERUP_INDICATOR_SPAWN);
            drawComp.addSprite("Artwork.Other.None", "RunningGame.Resources.Artwork.Other.GrayNone.png", GlobalVars.POWERUP_INDICATOR_NONE);
            
            
            drawComp.setSprite(GlobalVars.POWERUP_INDICATOR_NONE); //Set image to active image



            /* ANIMATION COMPONENT - Does it need animating?
             * The float that this reads in is the amount of time (in seconds) between frames.
             * So, if it was 5, you would be on one frame for 5 seconds, then switch to the next, then 5 seconds later
             * It'd switch to the next etc, etc...
             */
            //addComponent(new AnimationComponent(0.0005f));
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
        public override void revertToStartingState()
        {
            //Stuff
        }
         
    }
}