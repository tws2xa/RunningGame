using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;

namespace RunningGame.Entities
{

    /*
     * Grapple is like this freaky meta-entity
     * Do not use Grapple as an example of what to do.
     * 
     * Basically it gets drawn, but has no draw component
     * It collides with things, but has no collider
     * Its width and height are completely irrelevant.
     * 
     * These issues are due to the fact that it's a line, not a rectangle.
     * It's drawn in its own little section of the view
     * Its collisions are handled in the GrappleSystem
     */

    [Serializable()]
    public class GrappleEntity:Entity
    {
        
        float defaultWidth = GlobalVars.MIN_TILE_SIZE;
        float defaultHeight = GlobalVars.MIN_TILE_SIZE;
        
        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public GrappleEntity(Level level, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;
            this.depth = 2;
            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y, 0);
        }
        public GrappleEntity(Level level, int id, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y, 0);
        }

        //Takes in a direction
        public GrappleEntity(Level level, int id, float x, float y, double dir)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y, dir);
        }



        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y, double dir)
        {
            /*POSITION COMPONENT - Does it have a position?
             */
            PositionComponent posComp = (PositionComponent)addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            /*
             * GRAPPLE COMPONENT
             */
            addComponent(new GrappleComponent(x, y, dir));

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
