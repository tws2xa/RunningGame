using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Systems;

namespace RunningGame.Systems
{
    class ScreenWrapSystem:GameSystem
    {

        ArrayList requiredComponents = new ArrayList();
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public ScreenWrapSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.SCREEN_WRAP_COMPONENT_NAME); //Screen Wrap
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity


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

        //Check if it's out of the screen in a wrap direction, if so, wrap.
        public override void Update(float deltaTime)
        {

            foreach (Entity e in getApplicableEntities())
            {

                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                ScreenWrapComponent scrWrapComp = (ScreenWrapComponent)e.getComponent(GlobalVars.SCREEN_WRAP_COMPONENT_NAME);
                //Off sides of screen check
                if (scrWrapComp.right && posComp.x > level.levelWidth + posComp.width && velComp.x > 0) wrapRight(posComp);
                if (scrWrapComp.left && posComp.x < -posComp.width && velComp.x < 0) wrapLeft(posComp);
                if (scrWrapComp.down && posComp.y > level.levelHeight + posComp.height && velComp.y > 0) wrapDown(posComp);
                if (scrWrapComp.up && posComp.y < -posComp.height && velComp.y < 0) wrapUp(posComp);
            }

        }

        //------------------------------------------------------------------------------------------
        public void wrapLeft(PositionComponent posComp)
        {
            level.getMovementSystem().changeSingleAxisLocation('X', posComp, level.levelWidth + posComp.width, true); //Screen Wrap
        }
        public void wrapRight(PositionComponent posComp)
        {
            level.getMovementSystem().changeSingleAxisLocation('X', posComp, -posComp.width, true); //Screen Wrap
        }
        public void wrapUp(PositionComponent posComp)
        {
            level.getMovementSystem().changeSingleAxisLocation('Y', posComp, level.levelHeight + posComp.height, true); //Screen Wrap
        }
        public void wrapDown(PositionComponent posComp)
        {
            level.getMovementSystem().changeSingleAxisLocation('Y', posComp, -posComp.height, true); //Screen Wrap
        }

    }
}
