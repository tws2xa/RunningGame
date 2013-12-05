using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame.Systems
{
    public class SimplePowerUpSystem: GameSystem
    {
        
        List<string> requiredComponents = new List<string>();
        Level level;

        //Keys
        Keys glideKey = Keys.Space;
        //Keys bounceKey = Keys.B;
        Keys equippedPowerupKey = Keys.F;
        Keys cycleDownPowerupKey = Keys.Q;
        Keys cycleUpPowerupKey = Keys.E;

        //Glide powerup informations
        float Glide_Gravity_Decrease = 130.0f;
        float glideDuration = 1.5f;
        float glideTimer;
        bool glideActive = false;
        float maxVelocity = 70.0f;

        float spawnDistance; //Defined in hasRunOnce
        
        //Powerup Locks
        bool glideUnlocked = false;
        bool speedyUnlocked = false;
        bool spawnUnlocked = false;
        bool grappleUnlocked = false;
        bool bouncyUnlocked = false;

        //Equips
        public bool speedyEquipped = false;
        bool blockSpawnEquipped = false;
        bool bouncyEquipped = false;

        //bounce powerup info
        

        //speedy powerup infos
        public float speedyTime = 1.0f;
        public float speedyTimer = -1.0f;
        public bool speedyActive = false;

        //Grapple
        bool hasRunOnce = false; //Used to add keys once and only once. Can't in constructor because inputSystem not ready yet

        public SimplePowerUpSystem(Level level)
        {
            this.level = level; //Always have this
            glideTimer = glideDuration;
            spawnDistance = 0;
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

            if (!hasRunOnce)
            {
                level.getInputSystem().addKey(glideKey);

                level.getInputSystem().addKey(cycleDownPowerupKey);
                level.getInputSystem().addKey(cycleUpPowerupKey);
                level.getInputSystem().addKey(equippedPowerupKey);

                PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                spawnDistance = posComp.width / 2 + 10.0f;
       

                hasRunOnce = true;
            }
            if (glideActive && level.getPlayer() != null) 
            {
                
                VelocityComponent velComp = (VelocityComponent)this.level.getPlayer().getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                if (velComp.y > maxVelocity)
                {
                    velComp.setVelocity(velComp.x, maxVelocity);
                }
                glideTimer = glideTimer - deltaTime;
                if (glideTimer < 0.0f)
                {
                    GravityComponent gravComp = (GravityComponent)this.level.getPlayer().getComponent(GlobalVars.GRAVITY_COMPONENT_NAME);
                    if (gravComp != null)
                    {
                        gravComp.setGravity(gravComp.x, GlobalVars.STANDARD_GRAVITY);
                    }
                    glideActive = false;
                    glideTimer = glideDuration;

                }

            } 

            if (speedyTimer > 0 && level.getPlayer() != null)
            {
                if (level.getPlayer() == null) return;
                speedyTimer -= deltaTime;
                if (!level.getPlayer().hasComponent(GlobalVars.VELOCITY_COMPONENT_NAME)) return;
                VelocityComponent velComp = (VelocityComponent)this.level.getPlayer().getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                if (speedyTimer <= 0 || Math.Abs(velComp.x) < GlobalVars.SPEEDY_SPEED) 
                {
                    velComp.setVelocity(0, velComp.y);
                    speedyTimer = -1;
                    speedyActive = false;
                    if (!level.getPlayer().hasComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME))
                    {
                        level.getPlayer().addComponent(new PlayerInputComponent(level.getPlayer()));
                    }
                }
            }

            checkForInput();
        }
        //----------------------------------------------------------------------------------------------
    
        public void speedyEntity(float x, float y)
        {
            Entity newEntity = new PreGroundSpeedy(level, x, y);

            level.addEntity(newEntity.randId, newEntity); 
        }
        public void checkForInput()
        {
            if (glideUnlocked && level.getInputSystem().myKeys[glideKey].down)
            {
                glide();
            }

            if (level.getInputSystem().myKeys[cycleUpPowerupKey].down)
            {
                CycleThroughEquips(true);
            }

            if (level.getInputSystem().myKeys[cycleDownPowerupKey].down)
            {
                CycleThroughEquips(false);
            }


            if (level.getInputSystem().myKeys[equippedPowerupKey].down)
            {
                equppedPowerup();
            }

            if (grappleUnlocked && level.getInputSystem().mouseRightClick)
            {
                Grapple();
            }
        }

        //Order (from top to bottom)
        //Bounce
        //Speed
        //Spawn
        //None
        public void CycleThroughEquips(bool up)
        {
            if (up)
            {
                if (bouncyEquipped)
                {
                    bouncyEquipped = false;
                    if (speedyUnlocked)
                    {
                        speedyEquipped = true;
                        level.getPlayer().setBlueImage();
                        refacePlayer();
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
                        refacePlayer();
                        return;
                    }
                    blockSpawnEquipped = false;
                }
                else if (speedyEquipped)
                {
                    bouncyEquipped = false;
                    speedyEquipped = false;
                    if (spawnUnlocked)
                    {
                        blockSpawnEquipped = true;
                        level.getPlayer().setOrangeImage();
                        refacePlayer();
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
                        refacePlayer();
                        return;
                    }
                }
                else if (blockSpawnEquipped)
                {
                    bouncyEquipped = false;
                    speedyEquipped = false;
                    blockSpawnEquipped = false;
                    level.getPlayer().setNormalImage();
                    refacePlayer();
                }
                else
                {
                    if (bouncyUnlocked)
                    {
                        bouncyEquipped = true;
                        level.getPlayer().setPurpleImage();
                        refacePlayer();
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
                        refacePlayer();
                        return;
                    }
                    speedyEquipped = false;
                    blockSpawnEquipped = false;
                }
            }
            else
            {

                if (bouncyEquipped)
                {
                    bouncyEquipped = false;
                    speedyEquipped = false;
                    blockSpawnEquipped = false;
                    level.getPlayer().setNormalImage();
                    refacePlayer();
                }
                else if (speedyEquipped)
                {
                    if (bouncyUnlocked)
                    {
                        bouncyEquipped = true;
                        level.getPlayer().setPurpleImage();
                        refacePlayer();
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
                        refacePlayer();
                        return;
                    }
                    speedyEquipped = false;
                    blockSpawnEquipped = false;
                }
                else if (blockSpawnEquipped)
                {
                    bouncyEquipped = false;
                    if (speedyUnlocked)
                    {
                        speedyEquipped = true;
                        level.getPlayer().setBlueImage();
                        refacePlayer();
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
                        refacePlayer();
                        return;
                    }
                    blockSpawnEquipped = false;
                }
                else //Nothing equiped
                {
                    if (spawnUnlocked)
                    {
                        blockSpawnEquipped = true;
                        level.getPlayer().setOrangeImage();
                        refacePlayer();
                        bouncyEquipped = false;
                        speedyEquipped = false;
                        return;
                    }
                    else if(speedyUnlocked)
                    {
                        speedyEquipped = true;
                        level.getPlayer().setBlueImage();
                        refacePlayer();

                        bouncyEquipped = false;
                        blockSpawnEquipped = false;
                        return;
                    }
                    else if(bouncyUnlocked)
                    {
                        bouncyEquipped = true;
                        level.getPlayer().setPurpleImage();
                        refacePlayer();
                        speedyEquipped = false;
                        blockSpawnEquipped = false;
                        return;
                    }
                    else
                    {
                        bouncyEquipped = false;
                        speedyEquipped = false;
                        blockSpawnEquipped = false;
                        level.getPlayer().setNormalImage();
                        refacePlayer();
                        return;
                    }
                    
                }
            }
        }

        public void toNoPowerups()
        {
            bouncyEquipped = false;
            speedyEquipped = false;
            blockSpawnEquipped = false;
            if (level.getPlayer() != null)
            {
                level.getPlayer().setNormalImage();
            }
            refacePlayer();
            return;
        }

        public void refacePlayer()
        {
            /*level.getPlayer().faceDirection(level.getPlayer().isLookingRight());

            Console.WriteLine("Left: " + level.getPlayer().isLookingLeft());
            Console.WriteLine("Right: " + level.getPlayer().isLookingRight());

            if (level.getPlayer().isLookingRight()) level.getPlayer().faceRight();
            else level.getPlayer().faceLeft();*/
        }

        public void equppedPowerup()
        {

            if (bouncyEquipped)
            {
                //Bouncy Call Here
                createBounce();
            }
            else if (speedyEquipped)
            {
                createSpeedy();
            }
            else if (blockSpawnEquipped)
            {
                blockSpawn();
            }
            else
            {
                //Derp
            }
        }

        public void createSpeedy()
        {
            PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            Player player = (Player)level.getPlayer();

            if (player.isLookingRight())
            {

                speedyEntity(posComp.x + spawnDistance, posComp.y);

            }
            else if (player.isLookingLeft())
            {
                speedyEntity(posComp.x - spawnDistance, posComp.y);
            }
        }

        public void bounceEntity(float x, float y)
        {
            Entity newBounceEntity = new PreGroundBounce(level, x, y);

            level.addEntity(newBounceEntity.randId, newBounceEntity);
        }
        public void createBounce()
        {
            PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            Player player = (Player)level.getPlayer();

            if (player.isLookingRight())
            {

                bounceEntity(posComp.x + spawnDistance, posComp.y);

            }
            else if (player.isLookingLeft())
            {
                bounceEntity(posComp.x - spawnDistance, posComp.y);
            }
        }
        public void Grapple()
        {
            if (level.getPlayer() == null) return;
            PositionComponent playerPos = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            
            //Get the direction
            double dir = 0;


            float mouseX = level.getInputSystem().mouseX + level.sysManager.drawSystem.mainView.x;
            float mouseY = level.getInputSystem().mouseY + level.sysManager.drawSystem.mainView.y;

            float xDiff = mouseX - playerPos.x;
            float yDiff = mouseY - playerPos.y;

            dir = Math.Atan(yDiff / xDiff);

            if (mouseX < playerPos.x)
            {
                dir += Math.PI;
                if (!level.getPlayer().isLookingLeft())
                {
                    level.getPlayer().faceLeft();
                }
            }
            else if (mouseX > playerPos.x && !level.getPlayer().isLookingRight())
            {
                level.getPlayer().faceRight();
            }

            //Add the entity
            GrappleEntity grap = new GrappleEntity(level, new Random().Next(), playerPos.x, playerPos.y, dir);
            level.addEntity(grap);
            VelocityComponent velComp = (VelocityComponent)level.getPlayer().getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            level.getPlayer().removeComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME);
            if(level.sysManager.grapSystem.removeGravity == 1) level.getPlayer().removeComponent(GlobalVars.GRAVITY_COMPONENT_NAME);
        }

        public void glide()
        {
            GravityComponent gravComp = (GravityComponent)this.level.getPlayer().getComponent(GlobalVars.GRAVITY_COMPONENT_NAME);
            gravComp.setGravity(gravComp.x, (Glide_Gravity_Decrease));
            glideActive = true;
        }

        public void blockSpawn()
        {
            PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            Player player = (Player)level.getPlayer();

                if (player.isLookingRight())
                {

                    blockEntity(posComp.x + spawnDistance, posComp.y);

                }

                if (player.isLookingLeft())

                {
                    blockEntity(posComp.x - spawnDistance, posComp.y);
                }
                

            }

        public void blockEntity(float x, float y)
        {   
            
            //Entity newEntity = new [YOUR ENTITY HERE](level, x, y);
            Entity newEntity = new spawnBlockEntity(level, x, y);

            level.addEntity(newEntity.randId, newEntity); //This should just stay the same
        }




        public void togglePowerup(int pupNum)
        {
            switch (pupNum)
            {
                case(GlobalVars.BOUNCE_NUM):
                    bouncyUnlocked = !getUnlocked(pupNum);
                    break;
                case (GlobalVars.SPEED_NUM):
                    speedyUnlocked = !getUnlocked(pupNum);
                    break;
                case (GlobalVars.JMP_NUM):
                    if (getUnlocked(pupNum))
                    {
                        GlobalVars.numAirJumps = GlobalVars.normNumAirJumps;
                    }
                    else
                    {
                        GlobalVars.numAirJumps = GlobalVars.doubleJumpNumAirJumps;
                    }
                    break;
                case (GlobalVars.SPAWN_NUM):
                    spawnUnlocked = !getUnlocked(pupNum);
                    break;
                case(GlobalVars.GLIDE_NUM):
                    glideUnlocked = !getUnlocked(pupNum);
                    break;
                case (GlobalVars.GRAP_NUM):
                    grappleUnlocked = !getUnlocked(pupNum);
                    break;

            }
        }

        public void unlockPowerup(int pupNum)
        {
            if (!getUnlocked(pupNum)) togglePowerup(pupNum);
        }

        public void lockPowerup(int pupNum)
        {
            if (getUnlocked(pupNum)) togglePowerup(pupNum);
        }
        
        public bool getUnlocked(int pupNum)
        {
            switch (pupNum)
            {
                case (GlobalVars.BOUNCE_NUM):
                    return bouncyUnlocked;
                case (GlobalVars.SPEED_NUM):
                    return speedyUnlocked;
                case (GlobalVars.JMP_NUM):
                    PlayerInputComponent inpComp = (PlayerInputComponent)level.getPlayer().getComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME);
                    return (GlobalVars.numAirJumps == GlobalVars.doubleJumpNumAirJumps);
                case (GlobalVars.SPAWN_NUM):
                    return spawnUnlocked;
                case(GlobalVars.GLIDE_NUM):
                    return glideUnlocked;
                case (GlobalVars.GRAP_NUM):
                    return grappleUnlocked;
            }
            return false;
        }

    }
}
    

