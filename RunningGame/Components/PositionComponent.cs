using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Systems;
using System.Collections;

namespace RunningGame.Components
{

    //The entity has an x, y, width, and height.

    class PositionComponent : Component
    {
        public float prevX { get; set; }
        public float prevY { get; set; }
        public float prevW { get; set; }
        public float prevH { get; set; }
        public float x;
        public float y;
        public float width { get; set; }
        public float height { get; set; }

        public bool positionHasChanged {get; set;}

        public Entity myEntity;
        public CollisionDetectionSystem colSys;

        //When a collision occurs, this becomes whatever it collided with
        public ArrayList collidedWith { get; set; }

        public PositionComponent(float x, float y, float w, float h, Entity myEntity)
        {
            componentName = GlobalVars.POSITION_COMPONENT_NAME;
            this.prevX = x;
            this.prevY = y;
            this.x = x;
            this.y = y;
            this.prevW = w;
            this.prevH = h;
            this.width = w;
            this.height = h;

            this.myEntity = myEntity;

            if (this.myEntity.level.sysManager != null && this.myEntity.level.sysManager.colSystem != null)
            {
                colSys = this.myEntity.level.sysManager.colSystem;
            }
            else
            {
                colSys = null;
            }

            collidedWith = new ArrayList();
            positionHasChanged = false;
        }


        //Returns an integer position as a Point
        public System.Drawing.Point getIntegerPoint()
        {
            return new System.Drawing.Point((int)Math.Round(x), (int)Math.Round(y));
        }
        public System.Drawing.PointF getPointF()
        {
            return new System.Drawing.PointF(x, y);
        }

    }
}
