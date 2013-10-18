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
    public class SimpleEnemyAISystem:GameSystem
    {//All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public SimpleEnemyAISystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity
            requiredComponents.Add(GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME);//Simple Enemy


            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override ArrayList getRequiredComponents()
        {
            return requiredComponents;
        }
        
        //Must have this. Same for all Systems.
        public override Level GetActiveLevel()
        {
            return level;
        }

        public override void Update(float deltaTime)
        {
            foreach (Entity e in getApplicableEntities())
            {
                SimpleEnemyComponent simpEntComp = (SimpleEnemyComponent)e.getComponent(GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME);
                VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                if (velComp.x == 0)
                {
                    SimpleEnemyComponent simpEnemyComp = (SimpleEnemyComponent)e.getComponent(GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME);

                    if (!simpEntComp.wasStoppedLastFrame)
                        velComp.x = simpEnemyComp.mySpeed;
                    else
                        velComp.x = -simpEnemyComp.mySpeed;

                    simpEntComp.wasStoppedLastFrame = true;
                }
                else if(simpEntComp.wasStoppedLastFrame)
                {
                    simpEntComp.wasStoppedLastFrame = false;
                }
            }
        }
    }
}
