using System;
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

        public Dictionary<int, Entity> applicableEntities = new Dictionary<int, Entity>();

        //0 = haven't checked
        //1 = no
        //2 = yes
        int doActOnGround = 0;

        
        //Returns a list of all entities with required components
        public List<Entity> getApplicableEntities()
        {
            List<Entity> a = applicableEntities.Values.ToList<Entity>();
            List<Entity> ret = new List<Entity>();

            foreach (Entity e in a)
            {
                if (e.updateOutOfView || isInView(e))
                {
                    ret.Add(e);
                }
            }

            /*List<Entity> appEnts = getApplicableEntities2();

            if (ret.Count != appEnts.Count)
            {
                //return appEnts;
                bool doIAct = actOnGround();
                Console.WriteLine(doIAct);
                Console.WriteLine("Whoa!");
            }*/

            return ret;

        }


        public bool checkIfEntityIsApplicable(Entity e)
        {
            foreach (string c in getRequiredComponents())
            {
                //If there is a single missing component - don't add.
                if (!e.hasComponent(c))
                {
                    return false;
                }
            }
            return true;
        }
        
        public List<Entity> getApplicableEntities2()
        {
            List<Entity> appEnts = new List<Entity>();



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
                                appEnts.Add(e);
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

                            if (addEntity) appEnts.Add(e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in get applicable entities: " + e);
                }
            }

            //simple sorting by depth happens here
            //appEnts = appEnts.OrderBy(o => o.depth).ToList();
            return appEnts;
        }

        public bool isInView(Entity e)
        {

            if (!e.hasComponent(GlobalVars.POSITION_COMPONENT_NAME)) return true; //No position = always in view. Why not?

            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);


            foreach (View v in GetActiveLevel().sysManager.drawSystem.views)
            {
                float x = v.x;
                float y = v.y;

                float width = v.width;
                float height = v.height;

                if (!((posComp.x + posComp.width) < x || (posComp.y + posComp.height) < y)) return true;

                if (!((posComp.x - posComp.width) > (x + width) || (posComp.y - posComp.height) > (y + height))) return true;
            }

            return false;

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
