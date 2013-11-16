using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Systems
{
    [Serializable()]
    public class MovingPlatformSystem:GameSystem
    {
        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public MovingPlatformSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity
            requiredComponents.Add(GlobalVars.MOVING_PLATFORM_COMPONENT_NAME); //Moving Platform


            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override List<string> getRequiredComponents()
        {
            return requiredComponents;
        }
        
        //Must have this. Same for all Systems.
        public override Level GetActiveLevel()
        {
            return level;
        }

        //You must have an Update.
        //Always read in deltaTime, and only deltaTime (it's the time that's passed since the last frame)
        //Use deltaTime for things like changing velocity or changing position from velocity
        //This is where you do anything that you want to happen every frame.
        //There is a chance that your system won't need to do anything in update. Still have it.
        public override void Update(float deltaTime)
        {
            foreach (Entity e in getApplicableEntities())
            {
                VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                MovingPlatformComponent movPlatComp = (MovingPlatformComponent)e.getComponent(GlobalVars.MOVING_PLATFORM_COMPONENT_NAME);

                float mySpeed = velComp.x;

                if (movPlatComp.vertical)
                {
                    mySpeed = velComp.y;
                }

                //If it's been stopped for more than one frame, try changing the direction and see if it can move that way instead.
                if (mySpeed == 0)
                {
                    float newSpeed = GlobalVars.MOVING_PLATFORM_SPEED;

                    if (!movPlatComp.wasStoppedLastFrame)
                        newSpeed = GlobalVars.MOVING_PLATFORM_SPEED;
                    else
                        newSpeed = -GlobalVars.MOVING_PLATFORM_SPEED;

                    if (movPlatComp.vertical)
                    {
                        velComp.y = newSpeed;
                    }
                    else
                    {
                        velComp.x = newSpeed;
                    }

                    movPlatComp.wasStoppedLastFrame = true;
                }
                else if (movPlatComp.wasStoppedLastFrame)
                {
                    movPlatComp.wasStoppedLastFrame = false;
                }
            }

        }
        //----------------------------------------------------------------------------------------------

    }
}
