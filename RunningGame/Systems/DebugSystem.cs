using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Entities;
using RunningGame.Components;
using System.Windows.Forms;

namespace RunningGame.Systems
{
    class DebugSystem:GameSystem
    {
        ArrayList requiredComponents = new ArrayList();
        Level level;

        Keys addEntityKey = Keys.N;

        public DebugSystem(Level level)
        {
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

            if(!level.getInputSystem().myKeys.ContainsKey(addEntityKey))
                level.getInputSystem().addKey(Keys.N);


            checkForInput();
        }
        //----------------------------------------------------------------------------------------------


        public void checkForInput()
        {
            if (level.getInputSystem().myKeys[addEntityKey].pressed)
            {
                PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                debugAddEntity(posComp.x + posComp.width * 1.5f, posComp.y);
            }
        }

        /*
         * Here is where you change which entitiy pressing N will add
         * All you should really have to do is change where it says
         * TestEntity to whatever you want to create.
         */
        public void debugAddEntity(float x, float y)
        {   
            
            //Entity newEntity = new [YOUR ENTITY HERE](level, x, y);
            Entity newEntity = new TestEntity(level, x, y);

            level.addEntity(newEntity.randId, newEntity); //This should just stay the same
        }
    }
}
