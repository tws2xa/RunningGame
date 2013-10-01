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
     * This system is one big mess.
     * 
     * I'm working on getting it so it's a bit neater. I promise.
     * 
     * As of now, it serves a few different functions: In the future I will hopefully
     * be able to split it up into a few different systems.
     * 
     * Current Functions:
     *      Listens for user input, and performs accordingly
     *      Handles intersections with the side of the screen
     *      Manages control things, like how many times the player can jump in the air
     * 
     * May also want to rework this to allow for multiple players.
     */
    class PlayerSystem : GameSystem
    {

        Level level;
        ArrayList requiredComponents = new ArrayList();

        Player player;

        //Components needed
        PlayerComponent playerComp;
        PositionComponent posComp;
        VelocityComponent velComp;
        DrawComponent drawComp;

        float jumpStrength = -150f;
        float platformerMoveSpeed = 150f;
        int numAirJumps = 2; //number of jumps possible in the air (numAirJumps = 1 means you can double jump)
        int passedAirjumps = 0;

        public PlayerSystem(Level activeLevel)
        {
            //Required Components
            requiredComponents.Add(GlobalVars.PLAYER_COMPONENT_NAME); //PlayerInput Component
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity
            requiredComponents.Add(GlobalVars.DRAW_COMPONENT_NAME); //Drawable

            //Set the level
            level = activeLevel;

            //Get the player
            findPlayerAndAssignComponents();
        }

        //Find the object labled player - then grab all the components from it
        public void findPlayerAndAssignComponents()
        {
            ArrayList applicableEntities = getApplicableEntities();

            //If there is no player - STAHP
            if (applicableEntities.Count <= 0)
            {
                if(level.levelFullyLoaded)
                    Console.WriteLine("No Player");
            }
            //Otherwise cool beans
            else
            {
                player = (Player)applicableEntities[0];

                playerComp = (PlayerComponent)player.getComponent(GlobalVars.PLAYER_COMPONENT_NAME);
                posComp = (PositionComponent)player.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                velComp = (VelocityComponent)player.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                drawComp = (DrawComponent)player.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

            }
        }


        //-------------------------------- Overrides ----------------------------------
        public override Level GetActiveLevel()
        {
            return level;
        }
        public override ArrayList getRequiredComponents()
        {
            return requiredComponents;
        }
        public override void Update(float deltaTime)
        {

            if (hasNullComponent())
            {
                findPlayerAndAssignComponents();
                return;
            }

            //Reset passedAirJumps if needed
            if (passedAirjumps != 0 && level.getCollisionSystem().findObjectsBetweenPoints(
                posComp.x - posComp.width / 2, posComp.y + (posComp.height / 2) + 1, posComp.x + posComp.width / 2, posComp.y +
                (posComp.height / 2) + 1).Count > 0)
            {
                passedAirjumps = 0;
            }

        }
        //-----------------------------------------------------------------------------

        //----------------------------------- Input ----------------------------------- 
        public void KeyDown(KeyEventArgs e)
        {
            if (hasNullComponent()) return;

            if (e.KeyData == GlobalVars.KEY_JUMP)
            {
                playerJump();
            }
            if (e.KeyData == GlobalVars.KEY_LEFT)
            {
                beginMoveLeft();
            }
            if (e.KeyData == GlobalVars.KEY_RIGHT)
            {
                beginMoveRight();
            }

        }
        public void KeyUp(KeyEventArgs e)
        {

            if (hasNullComponent()) return;

            if (e.KeyData == GlobalVars.KEY_LEFT)
            {
                endLeftHorizontalMove();
            }
            if (e.KeyData == GlobalVars.KEY_RIGHT)
            {
                endRightHorizontalMove();
            }

        }
        public void KeyPressed(KeyPressEventArgs e)
        {
            if (hasNullComponent()) return;
        }
        //--------------------------------------------------------------------------------



        //------------------------------------- Actions ----------------------------------
        public void playerJump()
        {
            //If it's landed on something, jump
            float checkY = posComp.y + (posComp.height / 2) + 1;
            if (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2, checkY, posComp.x + posComp.width / 2, checkY).Count > 0)
            {
                velComp.setVelocity(velComp.x, jumpStrength);
                passedAirjumps = 0;
            }
            else
            {
                if (passedAirjumps < numAirJumps)
                {
                    velComp.setVelocity(velComp.x, jumpStrength);
                    passedAirjumps++;
                }
            }
        }
        public void beginMoveLeft()
        {
            velComp.setVelocity(-platformerMoveSpeed, velComp.y);
            player.faceLeft();
        }
        public void beginMoveRight()
        {
            velComp.setVelocity(platformerMoveSpeed, velComp.y);
            player.faceRight();
        }
        public void endLeftHorizontalMove()
        {
            if (velComp.x < 0) velComp.setVelocity(0, velComp.y);
        }
        public void endRightHorizontalMove()
        {
            if (velComp.x > 0) velComp.setVelocity(0, velComp.y);
        }
        public void endUpperMove()
        {
            if (velComp.y < 0) velComp.setVelocity(velComp.x, 0);
        }
        public void endLowerMove()
        {
            if (velComp.y > 0) velComp.setVelocity(velComp.x, 0);
        }

        //--------------------------------------------------------------------------------

        //---------------------------------------- Other ---------------------------------
        public bool hasNullComponent()
        {
            return (playerComp == null || posComp == null || velComp == null || drawComp == null);
        }

        public bool sameSigns(float num1, float num2)
        {
            return ((num1 < 0 && num2 < 0) || (num1 > 0 && num2 > 0));
        }

    }
}
