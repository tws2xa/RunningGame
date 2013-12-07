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
        List<string> requiredComponents = new List<string>();
        Level level;

        public bool recoil = true;
        public float recoilMultiplier = 0.5f;
        public float recoilCap = 200.0f; //Don't recoil if velocity is over this

        bool bulletFired = false;
        int numBullets = 0;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public PlayerWeaponSystem(Level level)
        {
            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override List<string> getRequiredComponents()
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
            //Only count up num bullets if a weapon has been fired.
            //If it hasn't then don't waste the time.
            numBullets = 0;
            if (bulletFired)
            {
                foreach (Entity e in GlobalVars.nonGroundEntities.Values)
                {
                    if (e is BulletEntity) numBullets++;
                }

                if (numBullets <= 0) bulletFired = false;
            }

            if (!level.sysManager.visSystem.orbActive && level.getInputSystem().mouseLeftClick)
            {
                if (level.getPlayer() != null)
                {
                    //Count up number of already existing bullets
                    if (numBullets < GlobalVars.MAX_NUM_BULLETS)
                    {
                        fireWeapon((PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME));
                    }
                }
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

            if (level.sysManager.spSystem.speedyActive && xVel > 0) xVel += GlobalVars.SPEEDY_SPEED;
            else if (level.sysManager.spSystem.speedyActive && xVel < 0) xVel -= GlobalVars.SPEEDY_SPEED;


            //Make the bullet
            BulletEntity bullet = new BulletEntity(level, posComp.x, posComp.y, (float)xVel, (float)yVel);
            level.addEntity(bullet.randId, bullet);
            //level.sysManager.sndSystem.playSound("RunningGame.Resources.Sounds.boop.wav", false);

            //Recoil
            Player player = (Player)level.getPlayer();
            if (recoil && player.hasComponent(GlobalVars.VELOCITY_COMPONENT_NAME))
            {
                VelocityComponent playerVelComp = (VelocityComponent)player.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                //Don't recoil if the player is walking in the direcion of the shot
                if (!((xVel > 0 && level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed) || (xVel < 0 && level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed)))
                {
                    if(!level.sysManager.spSystem.speedyActive) playerVelComp.x -= (float)xVel * recoilMultiplier;
                }
                else
                {
                    //If the player is in the air, still recoil
                    float leftX = (posComp.x - posComp.width / 2);
                    float rightX = (posComp.x + posComp.width / 2);
                    float lowerY = (posComp.y + posComp.height / 2 + 1);
                    if (!(level.getCollisionSystem().findObjectsBetweenPoints(leftX, lowerY, rightX, lowerY).Count > 0))
                    {
                        //Check it isn't over the cap
                        if (!((playerVelComp.x < 0 && playerVelComp.x < recoilCap) || (playerVelComp.x > 0 && playerVelComp.x > recoilCap)))
                        {
                            if (!level.sysManager.spSystem.speedyActive) playerVelComp.x -= (float)xVel * recoilMultiplier;
                        }
                    }
                }

                //Check it isn't over the recoil cap
                if (!((playerVelComp.y < 0 && playerVelComp.y < recoilCap) || (playerVelComp.y > 0 && playerVelComp.y > recoilCap)))
                {
                    playerVelComp.y -= (float)yVel * recoilMultiplier;
                }
            }

            //Turn if need be
            if (player.isLookingLeft() && xVel > 0)
            {
                player.faceRight();
            }
            else if (player.isLookingRight() && xVel < 0)
            {
                player.faceLeft();
            }
            bulletFired = true;
        }

    }
}
