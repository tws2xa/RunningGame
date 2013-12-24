using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {

    [Serializable()]
    public class DoorEntity : Entity {


        //If you're object has width/height Probably good to have a defaults for both.
        //These are the dimensions your entitiy will start out with.
        float defaultWidth = 30;
        float defaultHeight = 60;

        //-------------------------------------------Constructors--------------------------------------------
        public DoorEntity(Level level, float x, float y) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y, Int32.MinValue);
        }
        public DoorEntity(Level level, int id, float x, float y) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);


            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y, Int32.MinValue);
        }
        public DoorEntity(Level level, float x, float y, int switchId) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y, switchId);
        }
        public DoorEntity(Level level, int id, float x, float y, int switchId) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity(id, level);

            //Add the components.
            //Leave this for all entities.
            addMyComponents(x, y, switchId);
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y, int switchId) {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);

            /*DRAW COMPONENT - Does it get drawn to the game world?
             *You'll need to know the address for your image.
             *It'll probably be something along the lines of "RunningGame.Resources.[      ].png" or maybe .bmp
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            drawComp.addSprite("Artwork.Foreground.DoorClosed", "RunningGame.Resources.Artwork.Foreground.DoorClosed11.png", GlobalVars.DOOR_CLOSED_SPRITE_NAME);
            drawComp.addSprite("Artwork.Foreground.DoorOpen", "RunningGame.Resources.Artwork.Foreground.DoorOpen11.png", GlobalVars.DOOR_OPEN_SPRITE_NAME);
            drawComp.setSprite(GlobalVars.DOOR_CLOSED_SPRITE_NAME);

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE), true);

            /*SWITCH LISTENER - It listens for a switch
            */
            addComponent(new SwitchListenerComponent(switchId, GlobalVars.DOOR_EVENT_TYPE), true);

        }

        public override void revertToStartingState() {
            //Nothing?
            //Maybe close?
            //Maybe check switch?
            //This is going to be a pain later...
        }


    }
}
