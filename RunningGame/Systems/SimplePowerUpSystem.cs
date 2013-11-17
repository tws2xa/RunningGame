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
        //public PowerupUIEntity indicator;

        //Glide powerup informations
        bool glideEnabled = true;
        float Glide_Gravity_Decrease = 130.0f;
        Keys glideKey = Keys.G;
        float glideDuration = 1.5f;
        float glideTimer;
        bool glideActive = false;
        float maxVelocity = 70.0f;

        //addBlock information
        bool blockSpawnEnabled = true;
        Keys blockSpawnKey = Keys.K;


        //Grapple
        bool grappleEnabled = true;
        bool hasRunOnce = false; //Used to add keys once and only once. Can't in constructor because inputSystem not ready yet
       

        //Bouncy
        public bool bouncyEnabledTEMP = false;

        //Speedy
        public bool speedyEnabledTEMP = false;



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
                level.getInputSystem().addKey(blockSpawnKey);
                //Create and set the powerup ui indicator

                //PowerupUIEntity ind = new PowerupUIEntity(level, 0, 0);
                //level.addEntity(ind);
                //this.indicator = ind;

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


            checkForInput();
        }
        //----------------------------------------------------------------------------------------------


        public void checkForInput()
        {
            if (glideEnabled && level.getInputSystem().myKeys[glideKey].down)
            {
                glide();
            }
            if (blockSpawnEnabled && level.getInputSystem().myKeys[blockSpawnKey].down)
            {
                blockSpawn();
            }
            if (grappleEnabled && level.getInputSystem().mouseRightClick)
            {
                Grapple();
            }
        }

        //Order (from top to bottom)
        //Bounce
        //Speed
        //Spawn
        //None - Remove?
        public void CycleThroughEquips(bool down)
        {
            if (bouncyEnabledTEMP)
            {
                bouncyEnabledTEMP = false;
                speedyEnabledTEMP = true;
                blockSpawnEnabled = false;
            }
            else if (speedyEnabledTEMP)
            {
                bouncyEnabledTEMP = false;
                speedyEnabledTEMP = false;
                blockSpawnEnabled = true;
            }
            else if (blockSpawnEnabled)
            {

                bouncyEnabledTEMP = false;
                speedyEnabledTEMP = false;
                blockSpawnEnabled = false;
            }
            else
            {
                bouncyEnabledTEMP = true;
                speedyEnabledTEMP = false;
                blockSpawnEnabled = false;
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
    

