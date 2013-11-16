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
        
        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        float pressureSwitchActiveHeight = 8f; //How much a pressure switch shrinks
        float pressureSwitchInactiveHeight = 10;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public SwitchSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.SWITCH_COMPONENT_NAME);

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
        public override void Update(float deltaTime)
        {
            foreach (Entity e in getApplicableEntities())
            {
                SwitchComponent switchComp = (SwitchComponent)e.getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
                if (e.hasComponent(GlobalVars.TIMED_SWITCH_COMPONENT_NAME))
                {
                    TimedSwitchComponent timedComp = (TimedSwitchComponent)e.getComponent(GlobalVars.TIMED_SWITCH_COMPONENT_NAME);
                    //If it's not a pressure switch, and it's active. Count down
                    if ((timedComp.baseTime > 0) && switchComp.active)
                    {
                        timedComp.timer+=deltaTime;
                        if (timedComp.timer > timedComp.baseTime)
                        {
                            switchComp.setActive(false);
                            timedComp.timer = 0;
                        }
                    }

                    //If it's a pressure switch
                    if (timedComp.baseTime <= 0)
                    {
                        PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                        List<Entity> aboveCollisions = level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2, posComp.y - posComp.height / 2 - 1, posComp.x + posComp.width / 2, posComp.y - posComp.height / 2 - 1);
                        //If there's something above the switch, and it's inactive - make it active!
                        if (aboveCollisions.Count > 0)
                        {
                            if (!switchComp.active)
                            {
                                float hDiff = (pressureSwitchActiveHeight - pressureSwitchInactiveHeight)/2;
                                level.getMovementSystem().changeHeight(posComp, pressureSwitchActiveHeight);
                                level.getMovementSystem().teleportToNoCollisionCheck(posComp, posComp.x, posComp.y - hDiff);
                                switchComp.setActive(true);
                                
                                //Move down all objects above the switch
                                foreach (Entity above in aboveCollisions)
                                {
                                    if (above.hasComponent(GlobalVars.POSITION_COMPONENT_NAME))
                                    {
                                        PositionComponent pos = (PositionComponent)above.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                                        level.getMovementSystem().changePosition(pos, pos.x, pos.y - hDiff, true);
                                    }
                                }
                            }
                        }
                        else if(switchComp.active)
                        {
                            float hDiff = (pressureSwitchActiveHeight - pressureSwitchInactiveHeight)/2;
                            level.getMovementSystem().changeHeight(posComp, pressureSwitchInactiveHeight);
                            level.getMovementSystem().teleportToNoCollisionCheck(posComp, posComp.x, posComp.y + hDiff);
                            switchComp.setActive(false);
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
