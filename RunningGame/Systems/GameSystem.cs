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

        public abstract ArrayList getRequiredComponents(); //string names of all required components (Use GlobalVars script to get the names)


        //Returns a list of all entities with required components
        public ArrayList getApplicableEntities()
        {
            
            ArrayList applicableEntities = new ArrayList();

            if (!GetActiveLevel().paused)
            {
                try
                {
                    foreach (Entity e in GetActiveLevel().getEntities().Values)
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

    }
}
