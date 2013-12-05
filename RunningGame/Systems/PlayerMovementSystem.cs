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

    /*
     * This system is used to handle player specific movement.
     */
    [Serializable()]
    public class PlayerMovementSystem : GameSystem
    {

        Level level;
        List<string> requiredComponents = new List<string>();
        public float playerHorizSlowSpeed = 50.0f;

        public PlayerMovementSystem(Level activeLevel)
        {
            //Required Components
            requiredComponents.Add(GlobalVars.PLAYER_COMPONENT_NAME); //Player Component
            requiredComponents.Add(GlobalVars.PLAYER_INPUT_COMPONENT_NAME); //Player Input Component
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity

            //Set the level
            level = activeLevel;
            
        }

        //-------------------------------- Overrides ----------------------------------
        public override Level GetActiveLevel()
        {
            return level;
        }
        public override List<string> getRequiredComponents()
        {
            return requiredComponents;
        }
        public override void Update(float deltaTime)
        {


            foreach (Entity e in getApplicableEntities())
            {
                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                PlayerInputComponent pelInComp = (PlayerInputComponent)e.getComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME);
                AnimationComponent animComp = (AnimationComponent)e.getComponent(GlobalVars.ANIMATION_COMPONENT_NAME);
                checkForInput(posComp, velComp, pelInComp, animComp);

                //Reset passedAirJumps if needed
                if (pelInComp.passedAirjumps != 0 && level.getCollisionSystem().findObjectsBetweenPoints(
                    posComp.x - posComp.width / 2, posComp.y + (posComp.height / 2) + 1, posComp.x + posComp.width / 2, posComp.y +
                    (posComp.height / 2) + 1).Count > 0)
                {
                    pelInComp.passedAirjumps = 0;
                }

                //If there's a key down and the player isn't moving horizontally, check to make sure there's a collision
                if (Math.Abs(velComp.x) < Math.Abs(pelInComp.platformerMoveSpeed))
                {
                    if (level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed)
                    {
                        float leftX = (posComp.x - posComp.width / 2-1);
                        float upperY = (posComp.y - posComp.height / 2);
                        float lowerY = (posComp.y + posComp.height / 2);

                        if (!(level.getCollisionSystem().findObjectsBetweenPoints(leftX, upperY, leftX, lowerY).Count > 0))
                        {
                            beginMoveRight(posComp, velComp, pelInComp, animComp);
                        }
                    }
                    if (level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed)
                    {
                        float rightX = (posComp.x + posComp.width / 2 + 1);
                        float upperY = (posComp.y - posComp.height / 2);
                        float lowerY = (posComp.y + posComp.height / 2);

                        if (!(level.getCollisionSystem().findObjectsBetweenPoints(rightX, upperY, rightX, lowerY).Count > 0))
                        {
                            beginMoveLeft(posComp, velComp, pelInComp, animComp);
                        }
                    }
                }

                if (level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed || level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed)
                {
                    level.getPlayer().startAnimation();
                }
                else
                {
                    if(level.getPlayer() != null)
                    level.getPlayer().stopAnimation();
                }


                //Slow horizontal if no left/rght key down
                if (velComp.x != 0 && !level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed && !level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed)
                {
                    //If it's on top of something
                    float leftX = (posComp.x - posComp.width / 2);
                    float rightX = (posComp.x + posComp.width / 2);
                    float lowerY = (posComp.y + posComp.height / 2 + 1);
                    if ((level.getCollisionSystem().findObjectsBetweenPoints(leftX, lowerY, rightX, lowerY).Count > 0))
                    {
                        if (velComp.x < 0)
                        {
                            velComp.x += playerHorizSlowSpeed;
                            if (velComp.x > 0)
                                velComp.x = 0;
                        }
                        else
                        {
                            velComp.x -= playerHorizSlowSpeed;
                            if (velComp.x < 0)
                                velComp.x = 0;
                        }
                    }
                }
            }

        }
        //-----------------------------------------------------------------------------

        //----------------------------------- Input ----------------------------------- 
        public void checkForInput(PositionComponent posComp, VelocityComponent velComp, PlayerInputComponent pelInComp, AnimationComponent animComp)
        {
            if (level.getInputSystem().myKeys[GlobalVars.KEY_JUMP].down)
            {
                playerJump(posComp, velComp, pelInComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].down)
            {
                beginMoveLeft(posComp, velComp, pelInComp, animComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].down)
            {
                beginMoveRight(posComp, velComp, pelInComp, animComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].up)
            {
                endRightHorizontalMove(posComp, velComp, animComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].up)
            {
                endLeftHorizontalMove(posComp, velComp, animComp);
            }
        }
        //--------------------------------------------------------------------------------



        //------------------------------------- Actions ----------------------------------
        public void playerJump(PositionComponent posComp, VelocityComponent velComp, PlayerInputComponent pelInComp)
        {
            //If it's landed on something, jump
            float checkY = posComp.y + (posComp.height / 2) + 1;
            if (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2, checkY, posComp.x + posComp.width / 2, checkY).Count > 0)
            {
                velComp.setVelocity(velComp.x, pelInComp.jumpStrength);
                pelInComp.passedAirjumps = 0;
            }
            else
            {
                if (pelInComp.passedAirjumps < GlobalVars.numAirJumps)
                {
                    velComp.setVelocity(velComp.x, pelInComp.jumpStrength);
                    pelInComp.passedAirjumps++;
                }
            }
        }
        public void beginMoveLeft(PositionComponent posComp, VelocityComponent velComp, PlayerInputComponent pelInComp, AnimationComponent animComp)
        {
            velComp.setVelocity(-pelInComp.platformerMoveSpeed, velComp.y);
            if(!pelInComp.player.isLookingLeft())
                pelInComp.player.faceLeft();
            level.getPlayer().startAnimation();
            
        }
        public void beginMoveRight(PositionComponent posComp, VelocityComponent velComp, PlayerInputComponent pelInComp, AnimationComponent animComp)
        {
            velComp.setVelocity(pelInComp.platformerMoveSpeed, velComp.y);
            if(!pelInComp.player.isLookingRight())
                pelInComp.player.faceRight();
            level.getPlayer().startAnimation();
        }
        public void endLeftHorizontalMove(PositionComponent posComp, VelocityComponent velComp, AnimationComponent animComp)
        {
            if (velComp.x < 0) velComp.setVelocity(0, velComp.y);
            level.getPlayer().stopAnimation();
        }
        public void endRightHorizontalMove(PositionComponent posComp, VelocityComponent velComp, AnimationComponent animComp)
        {
            if (velComp.x > 0) velComp.setVelocity(0, velComp.y);
            level.getPlayer().stopAnimation();
        }
        public void endUpperMove(PositionComponent posComp, VelocityComponent velComp)
        {
            if (velComp.y < 0) velComp.setVelocity(velComp.x, 0);
        }
        public void endLowerMove(PositionComponent posComp, VelocityComponent velComp)
        {
            if (velComp.y > 0) velComp.setVelocity(velComp.x, 0);
        }
        //--------------------------------------------------------------------------------
    }
}
