using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    [Serializable()]
    public class MovingPlatformComponent:Component
    {

        public bool vertical = true; //Is it vertical
        public bool wasStoppedLastFrame = false; //Has it been still for a frame?
        //bool downRight = true; //Is it moving down if vertical, or right if horizonatal (May be unnecessary)
        public Entity myEnt;

        public MovingPlatformComponent(Entity e)
        {
            componentName = GlobalVars.MOVING_PLATFORM_COMPONENT_NAME;
            myEnt = e;
        }

        public void changeDirection(bool vert)
        {
            VelocityComponent velComp = (VelocityComponent)myEnt.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            
            vertical = vert;

            if (vert)
            {
                velComp.y = velComp.x;
                velComp.x = 0;
            }
            else
            {
                velComp.x = velComp.y;
                velComp.y = 0;
            }

        }


    }
}
