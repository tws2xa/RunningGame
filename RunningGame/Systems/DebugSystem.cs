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
    [Serializable()]
    public class DebugSystem : GameSystem
    {
        ArrayList requiredComponents = new ArrayList();
        Level level;

        Keys addEntityKey = Keys.N;
        Keys harmPlayerKey = Keys.H;
        Keys resetLevelKey = Keys.R;


        bool hasRunOnce = false; //Used to add keys once and only once. Can't in constructor because inputSystem not ready yet
        bool addingDoor = false; //False = adding Switch

        int switchId;

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

            if (!hasRunOnce)
            {
                level.getInputSystem().addKey(addEntityKey);
                level.getInputSystem().addKey(harmPlayerKey);
                level.getInputSystem().addKey(resetLevelKey);
           
                hasRunOnce = true;
            }

            checkForInput();
        }
        //----------------------------------------------------------------------------------------------


        public void checkForInput()
        {
            if (level.getInputSystem().myKeys[addEntityKey].down)
            {
                PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
              
  
                    debugAddEntity(posComp.x + posComp.width * 1.5f, posComp.y);

            }

            if (level.getInputSystem().myKeys[harmPlayerKey].down)
            {
                HealthComponent healthComp = (HealthComponent)level.getPlayer().getComponent(GlobalVars.HEALTH_COMPONENT_NAME);
                healthComp.subtractFromHealth(25);
            }

            if (level.getInputSystem().myKeys[resetLevelKey].up)
            {
                level.resetLevel();
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

        public void addDoorOrSwitch(float x, float y)
        {
            if (addingDoor)
            {
                DoorEntity d = new DoorEntity(level, x, y, switchId);
                level.addEntity(d.randId, d);
                addingDoor = false;
            }
            else
            {
                SwitchEntity s = new SwitchEntity(level, x, y);
                switchId = s.randId;
                level.addEntity(s.randId, s);
                addingDoor = true;
            }
        }
    }
}
