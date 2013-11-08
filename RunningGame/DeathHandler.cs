using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame
{
    /*
     * This class is where any and all entity "deaths" are handled.
     * Basically, if the health system notices that something has dropped to or below 0 health
     * It goes over to this class and tells it about the death.
     * 
     * Cheery Stuff
     */

    [Serializable()]
    class DeathHandler
    {

        //The level
        Level level;
        //How long does the fade between level resets take?
        float resetTime = 1.0f;
        float levelResetTimer = -1.0f; //The timer - don't modify this!

        public DeathHandler(Level level)
        {
            //Set the level
            this.level = level;
        }

        //This is really only used if the level reset timer is set
        public void update(float deltaTime)
        {
            //If the level reset timer is set...
            if (levelResetTimer > 0)
            {
                //Decrease the timer
                levelResetTimer -= deltaTime;
                //If the timer has passed (or hit) 0...
                if (levelResetTimer <= 0)
                {
                    //Reset the level.
                    level.resetLevel();
                    //Stop the timer.
                    levelResetTimer = -1;
                }
            }
        }

        //When an entity dies, this is called.
        public void handleDeath(Entity e) {
            //------- First check for any special cases

            //Is it the player? If so, reset the level.
            if(e.hasComponent(GlobalVars.PLAYER_COMPONENT_NAME)) {

                //If the reset level timer hasn't already been set...
                if (levelResetTimer < 0)
                {
                    //Get the player (Who just died)
                    Player pl = (Player)level.getPlayer();

                    //Grab all necessary components
                    PositionComponent posComp = (PositionComponent)pl.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    VelocityComponent velComp = (VelocityComponent)pl.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

                    //Create a dead player entity
                    DeadPlayerEntity deadPlayer = new DeadPlayerEntity(level, posComp.x, posComp.y);
                    //Set the dead player entity's velocity equal to the velocity of the old player
                    VelocityComponent deadVelComp = (VelocityComponent)deadPlayer.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                    deadVelComp.x = velComp.x;
                    deadVelComp.y = velComp.y;

                    //If the player was looking left, flip the dead player entity's sprite so it's also looking left
                    if (pl.isLookingLeft())
                    {
                        DrawComponent drawComp = (DrawComponent)deadPlayer.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
                        drawComp.rotateFlipSprite("Main", System.Drawing.RotateFlipType.RotateNoneFlipX);
                    }

                    //Add the dead player entity
                    level.addEntity(deadPlayer);
                    
                    //Remove the player
                    level.removeEntity(pl);

                    //Start the red flash
                    level.sysManager.drawSystem.setFlash(System.Drawing.Color.DarkRed, resetTime);
                    
                    //Start the reset timer.
                    levelResetTimer = resetTime * 0.6f; ;
                }
            }

            //------- Not a special case? Default to simply destroying the entity
            else {
                level.removeEntity(e);
            }
        }

    }
}
