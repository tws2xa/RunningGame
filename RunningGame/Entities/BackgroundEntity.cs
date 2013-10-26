using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{
    [Serializable()]
    public class BackgroundEntity:Entity
    {

        public float defaultWidth;
        public float defaultHeight;

        public BackgroundEntity(Level level, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;
            defaultWidth = level.levelWidth;
            defaultHeight = level.levelHeight;
            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y);
        }
        public BackgroundEntity(Level level, int id, float x, float y)
        {
            //Set level.
            //Leave for all entities
            this.level = level;
            defaultWidth = level.levelWidth;
            defaultHeight = level.levelHeight;
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
            /*POSITION COMPONENT
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            /*DRAW COMPONENT
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, true));
            drawComp.addSprite("RunningGame.Resources.WhiteSquare.png", "Main");
            drawComp.setSprite("Main");

            /* ANIMATION COMPONENT
             */
            //addComponent(new AnimationComponent(0.0005f));
        }
        
        public override void revertToStartingState()
        {
            //Stuff
        }

    }
}
