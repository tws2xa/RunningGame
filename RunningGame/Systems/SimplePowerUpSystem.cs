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
        
        ArrayList requiredComponents = new ArrayList();
        Level level;

        //Glide powerup informations
        float Glide_Gravity_Decrease = 130.0f;
        Keys glideKey = Keys.G;
        float glideDuration = 2.0f;
        float glideTimer;
        bool glideActive = false;

        //addBlock information
        Keys blockSpawnKey = Keys.K;

        //Grapple
       
        bool hasRunOnce = false; //Used to add keys once and only once. Can't in constructor because inputSystem not ready yet
       

        public SimplePowerUpSystem(Level level)
        {
            this.level = level; //Always have this
            glideTimer = glideDuration;
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

            if (!hasRunOnce)
            {
                level.getInputSystem().addKey(glideKey);
                level.getInputSystem().addKey(blockSpawnKey);
                hasRunOnce = true;
            }
            if (glideActive) 
            {
               
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
            if (level.getInputSystem().myKeys[glideKey].down)
            {
                glide();
            }
            if (level.getInputSystem().myKeys[blockSpawnKey].down)
            {
                blockSpawn();
            }
            if (level.getInputSystem().mouseRightClick)
            {
                Grapple();
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
            }

            //Add the entity
            GrappleEntity grap = new GrappleEntity(level, new Random().Next(), playerPos.x, playerPos.y, dir);
            level.addEntity(grap);
            VelocityComponent velComp = (VelocityComponent)level.getPlayer().getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            level.getPlayer().removeComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME);
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
                else blockEntity(posComp.x - posComp.width * 1.5f, posComp.y);
                
            }
        public void blockEntity(float x, float y)
        {   
            
            //Entity newEntity = new [YOUR ENTITY HERE](level, x, y);
            Entity newEntity = new spawnBlockEntity(level, x, y);

            level.addEntity(newEntity.randId, newEntity); //This should just stay the same
        }
        }
    }
    

