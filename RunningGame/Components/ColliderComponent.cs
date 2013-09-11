using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{

    //Adding this means the entity will collide with stuffs

    class ColliderComponent:Component
    {

        public Entity myEntity { get; set; }
        public string colliderType;
        public bool canPassLevelBoundries {get; set;}

        public ColliderComponent(Entity myEntity, string colliderType)
        {
            this.componentName = GlobalVars.COLLIDER_COMPONENT_NAME;
            this.colliderType = colliderType;

            this.myEntity = myEntity;

            myEntity.level.colliderAdded(myEntity);
        }

    }
}
