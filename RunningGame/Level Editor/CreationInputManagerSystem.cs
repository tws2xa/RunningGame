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
    public class CreationInputManagerSystem : GameSystem
    {
        
        //All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        CreationLevel level;

        public bool hasAddedKeys = false;
        public Keys escKey = Keys.Escape;
        public Keys delKey = Keys.Delete;
        public Keys shiftKey = Keys.ShiftKey;
        
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
                level.getInputSystem().addKey(escKey);
                level.getInputSystem().addKey(delKey);
                level.getInputSystem().addKey(shiftKey);
                hasAddedKeys = true;
            }


            //Check for mouse click. Select/Deselect Entity
            if (level.getInputSystem().mouseClick)
            {
                if (level.vars.protoEntity == null)
                {
                    //Select an entity
                    selectEntityAt(level.getInputSystem().mouseX, level.getInputSystem().mouseY);
                }
                else
                {
                    //Create entity from proto entity
                    createEntityFromProto();
                }
            }
            
            if (level.getInputSystem().myKeys[escKey].down)
            {
                deselectEntity();
                removeProtoEntity();
            }

            if (level.getInputSystem().myKeys[delKey].down)
            {
                if (level.vars.selectedEntity != null)
                {
                    level.removeEntity(level.vars.selectedEntity);
                    deselectEntity();
                }
            }

            if (level.getInputSystem().myKeys[shiftKey].down)
            {

            }
            
        }
        //---------------------------------- Helpers -----------------------------------------------

        public bool shiftPressed()
        {
            return (level.getInputSystem().myKeys[shiftKey].pressed);
        }


        //---------------------------------- Entity Selection --------------------------------------

        public void selectEntityAt(float x, float y)
        {
            ArrayList ents = level.getCollisionSystem().findObjectAtPoint(x, y);
            if (ents.Count > 0)
            {
                selectEntity((Entity)ents[0]);
            }
            else if (!shiftPressed())
            {
                deselectEntity();
            }
        }

        public void selectEntity(Entity e)
        {
            level.vars.selectedEntity = e;
            level.vars.editForm.refreshEntityPropertiesList();
            if (!shiftPressed())
                level.vars.allSelectedEntities.Clear();
            level.vars.allSelectedEntities.Insert(0, e);
        }

        public void deselectEntity()
        {
            level.vars.selectedEntity = null;
            level.vars.editForm.refreshEntityPropertiesList();
        }

        //---------------------------------------- Proto Entity Stuff --------------------------------

        public void removeProtoEntity()
        {
            if (level.vars.protoEntity != null)
                level.removeEntity(level.vars.protoEntity);
            level.vars.protoEntity = null;
        }
        public void createEntityFromProto()
        {

            if (level.vars.protoEntity.myEntType == typeof(Player) && level.getPlayer() != null)
            {
                Console.WriteLine("Trying to add a second player. That's a bad idea.");
                return;
            }

            Entity e = (Entity)Activator.CreateInstance(level.vars.protoEntity.myEntType, level, 0, 0);
            if (e.getComponent(GlobalVars.POSITION_COMPONENT_NAME) != null)
            {
                PositionComponent newPosComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                PositionComponent protoPosComp = (PositionComponent)level.vars.protoEntity.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                e.isStartingEntity = true;
                level.getMovementSystem().changePosition(newPosComp, protoPosComp.x, protoPosComp.y, false);
            }
            level.addEntity(e.randId, e);
            removeProtoEntity();
            selectEntity(e);
        }

    }
}
