using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame
{
    //Applies gravity to all objects with gravity, position, and velocity components
    public class GravitySystem : GameSystem
    {
        ArrayList requiredComponents = new ArrayList();
        Level level;

        public GravitySystem(Level activeLevel)
        {
            //Required Components
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.GRAVITY_COMPONENT_NAME); //Gravity

            //Set the level
            level = activeLevel;
        }

        public override Level GetActiveLevel()
        {
            return level;
        }

        //Run once each frame deltaTime is the amount of seconds since the last frame
        public override void Update(float deltaTime)
        {
            foreach(Entity e in getApplicableEntities()) {

                float floorBuffer = 1; //Distance it checks below object for a floor

                //Don't apply gravity if the object is on top of something
                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

                float leftX = (posComp.x - posComp.width / 2);
                float rightX = (posComp.x + posComp.width / 2);
                float lowerY = (posComp.y + posComp.height / 2 + floorBuffer);
                if (!(level.getCollisionSystem().findObjectsBetweenPoints(leftX, lowerY, rightX, lowerY).Count > 0))
                {

                    //Pull out all required components
                    VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                    GravityComponent gravComp = (GravityComponent)e.getComponent(GlobalVars.GRAVITY_COMPONENT_NAME);

                    //Add the x and y gravity to their respective velocity components
                    velComp.incVelocity(gravComp.x * deltaTime, gravComp.y * deltaTime);

                    //Console.WriteLine("Gravity for " + e + " - X Vel : " + velComp.x + " - Y Vel - " + velComp.y);
                }
            }

        }

        public override ArrayList getRequiredComponents()
        {
            return requiredComponents;
        }


    }
}
