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

        Level level;
        float levelResetTimer = -1.0f;
        float resetTime = 1.0f;

        public DeathHandler(Level level)
        {
            this.level = level;
        }

        public void update(float deltaTime)
        {
            if (levelResetTimer > 0)
            {
                levelResetTimer -= deltaTime;
                if (levelResetTimer <= 0)
                {
                    level.resetLevel();
                    levelResetTimer = -1;
                }
            }
        }

        public void handleDeath(Entity e) {
            //------- First check for any special cases

            //Is it the player? If so, reset the level.
            if(e.hasComponent(GlobalVars.PLAYER_COMPONENT_NAME)) {

                if (levelResetTimer < 0)
                {
                    Player pl = (Player)level.getPlayer();

                    PositionComponent posComp = (PositionComponent)pl.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    VelocityComponent velComp = (VelocityComponent)pl.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

                    DeadPlayerEntity deadPlayer = new DeadPlayerEntity(level, posComp.x, posComp.y);
                    VelocityComponent deadVelComp = (VelocityComponent)deadPlayer.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

                    deadVelComp.x = velComp.x;
                    deadVelComp.y = velComp.y;

                    if (pl.isLookingLeft())
                    {
                        DrawComponent drawComp = (DrawComponent)deadPlayer.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
                        drawComp.rotateFlipSprite("Main", System.Drawing.RotateFlipType.RotateNoneFlipX);
                    }

                    level.addEntity(deadPlayer);

                    level.removeEntity(pl);
                    level.sysManager.drawSystem.setFlash(System.Drawing.Color.DarkRed, resetTime);
                    levelResetTimer = resetTime * 0.6f; ;
                }
            }
            //------- Not a special case? default to simply destroying the entity
            else {
                level.removeEntity(e);
            }
        }

    }
}
