using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;
using System.Drawing;

namespace RunningGame.Components {

    //Adding this means the entity will collide with stuffs

    [Serializable()]
    public class ColliderComponent : Component {

        public Entity myEntity { get; set; }
        public string colliderType;
        public bool canPassLevelBoundries { get; set; }
        public DrawComponent drawComponent = null;
        public float width, height;
        public bool collideOnNoSprite = true;
        public bool hasTransparentPixels = true; //If an object's image has no transparent pixels, setting this to false can save a lot of time.

        public ColliderComponent( Entity myEntity, string colliderType ) {
            this.componentName = GlobalVars.COLLIDER_COMPONENT_NAME;
            this.colliderType = colliderType;

            this.myEntity = myEntity;


            if ( myEntity.hasComponent( GlobalVars.DRAW_COMPONENT_NAME ) ) {
                drawComponent = ( DrawComponent )myEntity.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
                this.width = drawComponent.width;
                this.height = drawComponent.height;
            } else {
                this.width = 0;
                this.height = 0;
                Console.WriteLine( "Error: Could not find drawComponent for width and height of collider: " + myEntity);
            }

            myEntity.level.colliderAdded( myEntity );
        }

        public ColliderComponent( Entity myEntity, string colliderType, float width, float height ) {
            this.componentName = GlobalVars.COLLIDER_COMPONENT_NAME;
            this.colliderType = colliderType;

            this.myEntity = myEntity;

            this.width = width;
            this.height = height;

            if ( myEntity.hasComponent( GlobalVars.DRAW_COMPONENT_NAME ) ) {
                drawComponent = ( DrawComponent )myEntity.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            }

            myEntity.level.colliderAdded( myEntity );
        }

    }
}
