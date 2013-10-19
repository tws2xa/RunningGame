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
    public class SwitchSystem:GameSystem
    {
        
        //All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public SwitchSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.SWITCH_COMPONENT_NAME);

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

        //You must have an Update.
        public override void Update(float deltaTime)
        {
            foreach (Entity e in getApplicableEntities())
            {
                SwitchComponent switchComp = (SwitchComponent)e.getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
                if (e.hasComponent(GlobalVars.TIMED_SWITCH_COMPONENT_NAME))
                {
                    if (switchComp.active)
                    {
                        TimedSwitchComponent timedComp = (TimedSwitchComponent)e.getComponent(GlobalVars.TIMED_SWITCH_COMPONENT_NAME);
                        timedComp.timer+=deltaTime;
                        if (timedComp.timer > timedComp.baseTime)
                        {
                            switchComp.setActive(false);
                            timedComp.timer = 0;
                        }
                    }
                }

                //change sprite if needed
                DrawComponent drawComp = (DrawComponent)e.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
                if (switchComp.active && drawComp.activeSprite != GlobalVars.SWITCH_ACTIVE_SPRITE_NAME)
                {
                    drawComp.setSprite(GlobalVars.SWITCH_ACTIVE_SPRITE_NAME);
                }
                if (!switchComp.active && drawComp.activeSprite != GlobalVars.SWITCH_INACTIVE_SPRITE_NAME)
                {
                    drawComp.setSprite(GlobalVars.SWITCH_INACTIVE_SPRITE_NAME);
                }
            }
        }
        //----------------------------------------------------------------------------------------------

    }
}
