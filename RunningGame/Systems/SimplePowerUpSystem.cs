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
        
        //Sizes
        float bouncySize = 10.0f;
        float speedySize = 10.0f;
        float spawnBlockSize = 20.0f;

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
        
        //Spawn Bock Info
        Queue<spawnBlockEntity> spawnBlocks = new Queue<spawnBlockEntity>(); // A queue of the spawn blocks
        int maxNumSpawnBlocks = 3;

        //speedy powerup infos
        public float speedyTime = 1.0f;
        public float speedyTimer = -1.0f;
        public bool speedyActive = false;

        //Grapple
        bool hasRunOnce = false; //Used to add keys once and only once. Can't in constructor because inputSystem not ready yet


        float curDeltaTime = 0.0f;

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
            curDeltaTime = deltaTime;
            if (!hasRunOnce)
            {
                level.getInputSystem().addKey(glideKey);

                level.getInputSystem().addKey(cycleDownPowerupKey);
                level.getInputSystem().addKey(cycleUpPowerupKey);
                level.getInputSystem().addKey(equippedPowerupKey);

                PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                spawnDistance = posComp.width / 2 + 15.0f;
       

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

            if (!level.sysManager.visSystem.orbActive)
            {
                checkForInput();
            }
        }
        //----------------------------------------------------------------------------------------------
    
        public void speedyEntity(float x, float y)
        {

            if (level.getCollisionSystem().findObjectsBetweenPoints(x-speedySize/2, y-speedySize/2, x+speedySize/2, y+speedySize/2).Count > 0) return;
            if (level.getCollisionSystem().findObjectsBetweenPoints(x - speedySize / 2, y + speedySize / 2, x + speedySize / 2, y - speedySize / 2).Count > 0) return;

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

            System.Drawing.Color purpleColor = System.Drawing.Color.Purple;
            System.Drawing.Color blueColor = System.Drawing.Color.Blue;
            System.Drawing.Color orangeColor = System.Drawing.Color.Orange;
            System.Drawing.Color defaultColor = System.Drawing.Color.AliceBlue;

            System.Drawing.Color newBorderCol = defaultColor; //Default Color.

            if (up)
            {
                if (bouncyEquipped)
                {
                    bouncyEquipped = false;
                    if (speedyUnlocked)
                    {
                        speedyEquipped = true;
                        level.getPlayer().setBlueImage();
                        newBorderCol = blueColor;
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
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
                        newBorderCol = orangeColor;
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
                    }
                }
                else if (blockSpawnEquipped)
                {
                    bouncyEquipped = false;
                    speedyEquipped = false;
                    blockSpawnEquipped = false;
                    level.getPlayer().setNormalImage();
                }
                else
                {
                    if (bouncyUnlocked)
                    {
                        bouncyEquipped = true;
                        level.getPlayer().setPurpleImage();
                        newBorderCol = purpleColor;
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
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
                }
                else if (speedyEquipped)
                {
                    if (bouncyUnlocked)
                    {
                        bouncyEquipped = true;
                        level.getPlayer().setPurpleImage();
                        newBorderCol = purpleColor;
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
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
                        newBorderCol = blueColor;
                    }
                    else
                    {
                        level.getPlayer().setNormalImage();
                    }
                    blockSpawnEquipped = false;
                }
                else //Nothing equiped
                {
                    if (spawnUnlocked)
                    {
                        blockSpawnEquipped = true;
                        level.getPlayer().setOrangeImage();
                        newBorderCol = orangeColor;
                        bouncyEquipped = false;
                        speedyEquipped = false;
                    }
                    else if(speedyUnlocked)
                    {
                        speedyEquipped = true;
                        level.getPlayer().setBlueImage();
                        newBorderCol = blueColor;

                        bouncyEquipped = false;
                        blockSpawnEquipped = false;
                    }
                    else if(bouncyUnlocked)
                    {
                        bouncyEquipped = true;
                        level.getPlayer().setPurpleImage();
                        newBorderCol = purpleColor;
                        speedyEquipped = false;
                        blockSpawnEquipped = false;
                    }
                    else
                    {
                        bouncyEquipped = false;
                        speedyEquipped = false;
                        blockSpawnEquipped = false;
                        level.getPlayer().setNormalImage();
                    }

                }
            }

            if (newBorderCol != defaultColor)
            {
                if (level.sysManager.drawSystem.getMainView() != null)
                {
                    System.Drawing.SolidBrush borderBrush = new System.Drawing.SolidBrush(newBorderCol);
                    level.sysManager.drawSystem.getMainView().borderBrush = borderBrush;
                    level.sysManager.drawSystem.getMainView().borderSize = 25;
                    level.sysManager.drawSystem.getMainView().hasBorder = true;
                }
            }
            else
            {
                level.sysManager.drawSystem.getMainView().hasBorder = false;
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
            return;
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

            if (player == null) return;

            speedyEntity(posComp.x + getSpawnDistance(player), posComp.y);

        }

        public void bounceEntity(float x, float y)
        {

            if (level.getCollisionSystem().findObjectsBetweenPoints(x - bouncySize / 2, y - bouncySize / 2, x + bouncySize / 2, y + bouncySize / 2).Count > 0) return;
            if (level.getCollisionSystem().findObjectsBetweenPoints(x - bouncySize / 2, y + bouncySize / 2, x + bouncySize / 2, y - bouncySize / 2).Count > 0) return;

            Entity newBounceEntity = new PreGroundBounce(level, x, y);

            level.addEntity(newBounceEntity.randId, newBounceEntity);
        }
        public void createBounce()
        {
            PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            Player player = (Player)level.getPlayer();

            bounceEntity(posComp.x + getSpawnDistance(player), posComp.y);

        }
        public void Grapple()
        {
            if (level.getPlayer() == null) return;
            PositionComponent playerPos = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            
            //Get the direction
            double dir = 0;


            float mouseX = level.getInputSystem().mouseX + level.sysManager.drawSystem.getMainView().x;
            float mouseY = level.getInputSystem().mouseY + level.sysManager.drawSystem.getMainView().y;

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

            createBlockEntity(posComp.x + getSpawnDistance(player), posComp.y);

        }

        public void createBlockEntity(float x, float y)
        {


            if (level.getCollisionSystem().findObjectsBetweenPoints(x - spawnBlockSize / 2, y - spawnBlockSize / 2, x + spawnBlockSize / 2, y + spawnBlockSize / 2).Count > 0) return;
            if (level.getCollisionSystem().findObjectsBetweenPoints(x - spawnBlockSize / 2, y + spawnBlockSize / 2, x + spawnBlockSize / 2, y - spawnBlockSize / 2).Count > 0) return;
            
            if (spawnBlocks.Count >= maxNumSpawnBlocks)
            {
                spawnBlockEntity old = spawnBlocks.Dequeue();
                level.removeEntity(old);
            }
            //Entity newEntity = new [YOUR ENTITY HERE](level, x, y);
            spawnBlockEntity newEntity = new spawnBlockEntity(level, x, y);
            level.addEntity(newEntity.randId, newEntity); //This should just stay the same
            spawnBlocks.Enqueue(newEntity);
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

        public float getSpawnDistance(Player player)
        {
            if(player == null) return 0;
            VelocityComponent velComp = (VelocityComponent)player.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            return getSpawnDistance(player, velComp);
        }


        public float getSpawnDistance(Player player, VelocityComponent velComp)
        {
            if (velComp == null) return 0;


            float dist = spawnDistance + Math.Abs(velComp.x*curDeltaTime);
            
            if (player.isLookingLeft())
            {
                return (-1*dist);
            }
            else
            {
                return dist;
            }

        }

    }

}
    

