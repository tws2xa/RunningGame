using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{
    class SpikeEntity:Entity
    {

        float defaultWidth = 10.0f;
        float defaultHeight = 10.0f;

        //-------------------------------------------Constructors--------------------------------------------
        //One takes in an ID, the other generats it.
        //Both take in the starting x and y of the entity.
        //Both take in the level that it's being applied to.
        //You probably won't have to edit these at all.
        public SpikeEntity(Level level, float x, float y, int dir)
        {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y, dir);
        }
        public SpikeEntity(Level level, int id, float x, float y, int dir)
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
        public void addMyComponents(float x, float y, int dir)
        {
            /*POSITION COMPONENT - Does it have a position?
             */
            PositionComponent posComp = (PositionComponent)addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
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
            string sprName = "Main";
            drawComp.addSprite("Artwork.Foreground.Spike", "RunningGame.Resources.Artwork.Foreground.Spike.png", sprName);
            drawComp.setSprite(sprName); //Set image to active image
            
            //Rotate accordingly
            
            /*
            switch (dir%4)
            {
                case (0):
                    level.getMovementSystem().changePosition(posComp, posComp.x+1, posComp.y + 1, false);
                    break;
                case (1):
                    drawComp.rotateFlipSprite(sprName, System.Drawing.RotateFlipType.Rotate90FlipNone);
                    level.getMovementSystem().changePosition(posComp, posComp.x-1, posComp.y+1, false);
                    break;
                case (2):
                    drawComp.rotateFlipSprite(sprName, System.Drawing.RotateFlipType.Rotate180FlipNone);
                    level.getMovementSystem().changePosition(posComp, posComp.x+1, posComp.y - 1, false);
                    break;
                case (3):
                    drawComp.rotateFlipSprite(sprName, System.Drawing.RotateFlipType.Rotate270FlipNone);
                    level.getMovementSystem().changePosition(posComp, posComp.x+1, posComp.y+1, false);
                    break;
            }
            */
            
            for(int i=0; i<dir; i++)
            {
                drawComp.rotateFlipSprite(sprName, System.Drawing.RotateFlipType.Rotate90FlipNone);
            }
            
            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.KILL_PLAYER_COLLIDER_TYPE));
        }
        
        public override void revertToStartingState()
        {
            //Stuff
        }
    }
}
