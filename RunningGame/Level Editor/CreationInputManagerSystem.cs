using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Systems;
using RunningGame.Entities;
using System.Windows.Forms;

namespace RunningGame.Level_Editor
{
    class CreationInputManagerSystem:GameSystem
    {
        
        //All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        CreationLevel level;

        public bool hasAddedKeys = false;
        public Keys escKey = Keys.Escape;
        
        public CreationInputManagerSystem(CreationLevel level)
        {
            //Here is where you add the Required components
            //No Req Components

            //Add keys to system manager

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


            if (!hasAddedKeys)
            {
                level.getInputSystem().addKey(Keys.Escape);
                hasAddedKeys = true;
            }


            //Check for mouse click. Select/Deselect Entity
            if (level.getInputSystem().mouseClick)
            {
                selectEntityAt(level.getInputSystem().mouseX, level.getInputSystem().mouseY);
            }
            
            if (level.getInputSystem().myKeys[escKey].down)
            {
                deselectEntity();
            }
            
        }
        
        //---------------------------------- Entity Selection --------------------------------------

        public void selectEntityAt(float x, float y)
        {
            ArrayList ents = level.getCollisionSystem().findObjectAtPoint(x, y);
            if (ents.Count > 0)
            {
                selectEntity((Entity)ents[0]);
            }
            else
            {
                deselectEntity();
            }
        }

        public void selectEntity(Entity e)
        {
            level.vars.selectedEntity = e;
        }

        public void deselectEntity()
        {
            level.vars.selectedEntity = null;
        }

        //--------------------------------------------------------------------------------------------

    }
}
