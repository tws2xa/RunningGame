using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame.Systems
{
    [Serializable()]
    public class PlayerWeaponSystem:GameSystem
    {
        ArrayList requiredComponents = new ArrayList();
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public PlayerWeaponSystem(Level level)
        {
            //No required components
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
            if (level.getInputSystem().mouseClick)
            {
                fireWeapon((PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME));
            }
        }
        //--------------------------------------------------------------------------------------------

        public void fireWeapon(PositionComponent posComp)
        {
            //Do maths!
            float mouseX = level.getInputSystem().mouseX + level.sysManager.drawSystem.mainView.x;
            float mouseY = level.getInputSystem().mouseY + level.sysManager.drawSystem.mainView.y;

            float xDiff = mouseX - posComp.x;
            float yDiff = mouseY - posComp.y;

            double theta = Math.Atan(xDiff / yDiff);

            double xVel = Math.Sin(theta) * GlobalVars.BULLET_SPEED;
            double yVel = Math.Cos(theta) * GlobalVars.BULLET_SPEED;

            if (xDiff > 0 && xVel < 0) xVel = -xVel;
            if (xDiff < 0 && xVel > 0) xVel = -xVel;
            if (yDiff > 0 && yVel < 0) yVel = -yVel;
            if (yDiff < 0 && yVel > 0) yVel = -yVel;

            //Make the bullet
            BulletEntity bullet = new BulletEntity(level, posComp.x, posComp.y, (float)xVel, (float)yVel);
            level.addEntity(bullet.randId, bullet);

        }

    }
}
