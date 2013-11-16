﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame
{

    /*
     * This is the abstract class from which all other systems are born.
     * It has useful methods like getApplicableEntities(), which checks for
     * any entities that have the required components for the system to act.
     */

    [Serializable()]
    public abstract class GameSystem
    {

        public abstract Level GetActiveLevel();

        public abstract void Update(float deltaTime);

        public abstract List<string> getRequiredComponents(); //string names of all required components (Use GlobalVars script to get the names)

        //0 = haven't checked
        //1 = no
        //2 = yes
        int doActOnGround = 0;

        //Returns a list of all entities with required components
        public List<Entity> getApplicableEntities()
        {
            
            List<Entity> applicableEntities = new List<Entity>();

           

            if (!GetActiveLevel().paused)
            {
                try
                {

                    if (actOnGround())
                    {
                        foreach (Entity e in GlobalVars.groundEntities.Values)
                        {
                            if (e.updateOutOfView || isInView(e))
                            {
                                applicableEntities.Add(e);
                            }
                        }
                    }

                    foreach (Entity e in GlobalVars.nonGroundEntities.Values)
                    {
                        if (e.updateOutOfView || isInView(e))
                        {

                            bool addEntity = true;
                            foreach (string c in getRequiredComponents())
                            {
                                //If there is a single missing component - don't add.
                                if (!e.hasComponent(c))
                                {
                                    addEntity = false;
                                    break;
                                }
                            }

                            if (addEntity) applicableEntities.Add(e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in get applicable entities: " + e);
                }
            }


            applicableEntities = applicableEntities.OrderBy(o => o.depth).ToList();
            return applicableEntities;

        }

       
        public bool isInView(Entity e)
        {

            if (!e.hasComponent(GlobalVars.POSITION_COMPONENT_NAME)) return true; //No position = always in view. Why not?

            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

            float x = GetActiveLevel().sysManager.drawSystem.mainView.x;
            float y = GetActiveLevel().sysManager.drawSystem.mainView.y;

            float width = GetActiveLevel().sysManager.drawSystem.mainView.width;
            float height = GetActiveLevel().sysManager.drawSystem.mainView.height;

            if ((posComp.x + posComp.width) < x || (posComp.y + posComp.height) < y) return false;

            if ((posComp.x - posComp.width) > (x + width) || (posComp.y - posComp.height) > (y + height)) return false;

            return true;

        }


        public bool actOnGround()
        {
            if (doActOnGround == 0)
            {
                List<string> reqComps = this.getRequiredComponents();

                foreach (string s in reqComps)
                {

                    if(!(s == GlobalVars.POSITION_COMPONENT_NAME || s == GlobalVars.DRAW_COMPONENT_NAME || s == GlobalVars.COLLIDER_COMPONENT_NAME))
                    {
                        doActOnGround = 1;
                        break;
                    }

                }

                if (doActOnGround != 1) doActOnGround = 2;
            }
            
            return (doActOnGround == 2);
        }

    }
}
