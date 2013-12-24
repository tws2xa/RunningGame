using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    [Serializable()]
    public class BackgroundEntity : Entity {

        public float defaultWidth;
        public float defaultHeight;

        public BackgroundEntity(Level level, float x, float y) {
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
            addMyComponents(x, y, defaultWidth, defaultHeight);
        }
        public BackgroundEntity(Level level, float x, float y, float w, float h) {
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
            addMyComponents(x, y, w, h);
        }
        public BackgroundEntity(Level level, int id, float x, float y) {
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
            addMyComponents(x, y, defaultWidth, defaultHeight);
        }


        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y, float w, float h) {
            /*POSITION COMPONENT
             */
            addComponent(new PositionComponent(x, y, w, h, this));

            /*DRAW COMPONENT
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            drawComp.useAlreadyLoadedImage = false;
            drawComp.addSprite("Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Background.Bkg11.png", "Main");
            drawComp.setSprite("Main");

            /* ANIMATION COMPONENT
             */
            //addComponent(new AnimationComponent(0.0005f));

            /*BACKGROUND COMPONENT
             */
            addComponent(new BackgroundComponent(), true);
        }

        public override void revertToStartingState() {
            //Noting
        }

    }
}
