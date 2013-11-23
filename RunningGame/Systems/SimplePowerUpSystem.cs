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
        //Keys speedyKey = Keys.L;
        //Keys blockSpawnKey = Keys.K;
        Keys equippedPowerupKey = Keys.F;
        Keys cycleDownPowerupKey = Keys.Q;


        //Glide powerup informations
        float Glide_Gravity_Decrease = 130.0f;
        float glideDuration = 1.5f;
        float glideTimer;
        bool glideActive = false;
        float maxVelocity = 70.0f;

        //Powerup Locks
        bool glideUnlocked = true;
        bool speedyUnlocked = true;
        bool spawnUnlocked = true;
        bool grappleUnlocked = true;
        bool bouncyUnlocked = true;

        //Equips
        public bool speedyEquipped = false;
        bool blockSpawnEquipped = false;
        bool bouncyEquippedTEMP = false;

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
                //level.getInputSystem().addKey(blockSpawnKey);
                //level.getInputSystem().addKey(speedyKey);
                level.getInputSystem().addKey(cycleDownPowerupKey);
                level.getInputSystem().addKey(equippedPowerupKey);

                hasRunOnce = true;
            }
            if (glideActive) 
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
                    gravComp.setGravity(gravComp.x, GlobalVars.STANDARD_GRAVITY);
                    glideActive = false;
                    glideTimer = glideDuration;

                }

            } 

            if (speedyTimer > 0)
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
            /*
            if (blockSpawnEquipped && level.getInputSystem().myKeys[blockSpawnKey].down)
            {
                blockSpawn();
            }
            if (speedyEquipped && level.getInputSystem().myKeys[speedyKey].down)
            {
                createSpeedy();
            }
             * */

            if (level.getInputSystem().myKeys[cycleDownPowerupKey].down)
            {
                CycleThroughEquips(true);
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
        public void CycleThroughEquips(bool down)
        {
            if (bouncyEquippedTEMP)
            {
                bouncyEquippedTEMP = false;
                if (speedyUnlocked)
                {
                    speedyEquipped = true;
                    level.getPlayer().setBlueImage();
                }
                else
                {
                    level.getPlayer().setNormalImage();
                    return;
                }
                blockSpawnEquipped = false;
            }
            else if (speedyEquipped)
            {
                bouncyEquippedTEMP = false;
                speedyEquipped = false;
                if (spawnUnlocked)
                {
                    level.getPlayer().setOrangeImage();
                    blockSpawnEquipped = true;
                }
                else
                {
                    level.getPlayer().setNormalImage();
                    return;
                }
            }
            else if (blockSpawnEquipped)
            {
                bouncyEquippedTEMP = false;
                speedyEquipped = false;
                blockSpawnEquipped = false;
                level.getPlayer().setNormalImage();
            }
            else
            {
                if (bouncyUnlocked)
                {
                    level.getPlayer().setPurpleImage();
                    bouncyEquippedTEMP = true;
                }
                else
                {
                    level.getPlayer().setNormalImage();
                    return;
                }
                speedyEquipped = false;
                blockSpawnEquipped = false;
            }
        }

        public void equppedPowerup()
        {

            if (bouncyEquippedTEMP)
            {
                //Bouncy Call Here
                Console.WriteLine("Bouncy!");
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

                speedyEntity(posComp.x + posComp.width * 1.5f, posComp.y);

            }
            else if (player.isLookingLeft())
            {
                speedyEntity(posComp.x - posComp.width * 1.5f, posComp.y);
            }
        }

        public void Grapple()
        {
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

                    blockEntity(posComp.x + posComp.width * 1.5f, posComp.y);

                }
                else if (player.isLookingLeft())
                {
                    blockEntity(posComp.x - posComp.width * 1.5f, posComp.y);
                }
                
            }
        public void blockEntity(float x, float y)
        {   
            
            //Entity newEntity = new [YOUR ENTITY HERE](level, x, y);
            Entity newEntity = new spawnBlockEntity(level, x, y);

            level.addEntity(newEntity.randId, newEntity); //This should just stay the same
        }
        }
    }
    

