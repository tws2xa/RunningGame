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
    public class ScreenEdgeSystem : GameSystem
    {

        ArrayList requiredComponents = new ArrayList();
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public ScreenEdgeSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.SCREEN_EDGE_COMPONENT_NAME); //Screen Wrap
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
                ScreenEdgeComponent scrEdgComp = (ScreenEdgeComponent)e.getComponent(GlobalVars.SCREEN_EDGE_COMPONENT_NAME);


                //Intersect sides of screen check (Stop)
                if (scrEdgComp.right == 1 && posComp.x + posComp.width / 2 >= level.levelWidth && velComp.x > 0) stopRight(posComp, velComp);
                if (scrEdgComp.left == 1 && posComp.x - posComp.width / 2 <= 0 && velComp.x < 0) stopLeft(posComp, velComp);
                if (scrEdgComp.down == 1 && posComp.y + posComp.height / 2 >= level.levelHeight && velComp.y > 0) stopDown(posComp, velComp);
                if (scrEdgComp.up == 1 && posComp.y - posComp.height / 2 <= 0 && velComp.y < 0) stopUp(posComp, velComp);


                //Off sides of screen check (Wrap)
                if (scrEdgComp.right == 2 && posComp.x > level.levelWidth + posComp.width && velComp.x > 0) wrapRight(posComp);
                if (scrEdgComp.left == 2 && posComp.x < -posComp.width && velComp.x < 0) wrapLeft(posComp);
                if (scrEdgComp.down == 2 && posComp.y > level.levelHeight + posComp.height && velComp.y > 0) wrapDown(posComp);
                if (scrEdgComp.up == 2 && posComp.y < -posComp.height && velComp.y < 0) wrapUp(posComp);
            }

        }

        //------------------------------------------------------------------------------------------

        //WRAP

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

        //STOP

        public void stopLeft(PositionComponent posComp, VelocityComponent velComp)
        {
            level.getMovementSystem().changeSingleAxisLocation('X', posComp, posComp.width/2, true);
            if (velComp.x < 0) velComp.setVelocity(0, velComp.y);
        }
        public void stopRight(PositionComponent posComp, VelocityComponent velComp)
        {
            level.getMovementSystem().changeSingleAxisLocation('X', posComp, level.levelWidth-posComp.width/2, true);
            if (velComp.x > 0) velComp.setVelocity(0, velComp.y);
        }
        public void stopUp(PositionComponent posComp, VelocityComponent velComp)
        {
            level.getMovementSystem().changeSingleAxisLocation('Y', posComp, posComp.height/2, true);
            if (velComp.y < 0) velComp.setVelocity(velComp.x, 0);
        }
        public void stopDown(PositionComponent posComp, VelocityComponent velComp)
        {
            level.getMovementSystem().changeSingleAxisLocation('Y', posComp, level.levelHeight - posComp.height/2, true);
            if (velComp.y > 0) velComp.setVelocity(velComp.x, 0);
        }
    }
}
