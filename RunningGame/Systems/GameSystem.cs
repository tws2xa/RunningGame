using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace RunningGame
{

    /*
     * This is the abstract class from which all other systems are born.
     * It has useful methods like getApplicableEntities(), which checks for
     * any entities that have the required components for the system to act.
     */

    abstract class GameSystem
    {

        public abstract Level GetActiveLevel();

        public abstract void Update(float deltaTime);

        public abstract ArrayList getRequiredComponents(); //String names of all required components (Use GlobalVars script to get the names)


        //Returns a list of all entities with required components
        public ArrayList getApplicableEntities()
        {
            
            ArrayList applicableEntities = new ArrayList();

            if (!GetActiveLevel().paused)
            {
                foreach (Entity e in GetActiveLevel().getEntities().Values)
                {
                    bool addEntity = true;
                    foreach (String c in getRequiredComponents())
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
            return applicableEntities;

        }

    }
}
