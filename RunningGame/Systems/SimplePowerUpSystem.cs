using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using RunningGame.Components;

namespace RunningGame.Systems
{
    public class SimplePowerUpSystem: GameSystem
    {
        ArrayList requiredComponents = new ArrayList();
        Level level;

        float Glide_Gravity_Decrease = 100.0f;
        Keys glideKey = Keys.G;

        bool hasRunOnce = false; //Used to add keys once and only once. Can't in constructor because inputSystem not ready yet
        bool addingDoor = false; //False = adding Switch

        int switchId;

        public SimplePowerUpSystem(Level level)
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
                level.getInputSystem().addKey(glideKey);
                hasRunOnce = true;
            }

            checkForInput();
        }
        //----------------------------------------------------------------------------------------------


        public void checkForInput()
        {
            /*if (level.getInputSystem().myKeys[glideKey].down)
            {
                glide();
            }*/
            if (level.getInputSystem().myKeys[glideKey].down)
            {
                glide();
            }
        }

        public void glide()
        {
            GravityComponent gravComp = (GravityComponent)this.level.getPlayer().getComponent(GlobalVars.GRAVITY_COMPONENT_NAME);
            gravComp.setGravity(gravComp.x, (gravComp.y-Glide_Gravity_Decrease));
        }
    }
    
}
