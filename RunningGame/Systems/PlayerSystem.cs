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
     *      Resizes the player based on his acceleration
     *      Increases the player's horizontal speed (If we decide to go the running-game route)
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



        //---------------------- VARS TO DO WITH RESIZING THE PLAYER ----------------------
        //"Base" width and height of the player
        float baseWidth;
        float baseHeight;

        //At which velocities is the shape normal
        //float normWidth = 0;
        //float normHeight = 0;

        //Multipliers for how severely a difference from center value affects the width/height
        float xIncSpeedStretchMultiplier = 0.06f;
        float yIncSpeedStretchMultiplier = 0.12f;
        float xDecSpeedStretchMultiplier = 0.03f;
        float yDecSpeedStretchMultiplier = 0.07f;

        //Max strech in either direction
        float maxWidth;
        float maxHeight;

        //Min stretch in either direction
        float minWidth;
        float minHeight;

        //Max and Min Surface Area
        float maxSurfaceArea;
        float minSurfaceArea;

        //Used to ease back into shape instead
        float xEaseRate = 1f;
        float yEaseRate = 1f;

        //Used for resizieng based off of acceleration
        float prevXVelocity = 0;
        float prevYVelocity = 0;

        //At what point does it start changing?
        float xStretchThreshold = 0.1f;
        float yStretchThreshold = 0.1f;

        //bool justChangedX = false;
        //bool justChangedY = false;

        //---------------------------------------------------------------------------------------------------


        float xAccel = 0f;
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

                baseWidth = posComp.width;
                baseHeight = posComp.height;

                maxWidth = baseWidth * 3;
                maxHeight = baseHeight * 3;

                minWidth = baseWidth / 3;
                minHeight = baseHeight / 3;

                maxSurfaceArea = baseWidth * baseHeight * 1.5f;
                minSurfaceArea = baseWidth * baseHeight / 2f;

                Console.WriteLine("Max: " + maxSurfaceArea + " -- Min: " + minSurfaceArea);

                prevXVelocity = velComp.x;
                prevYVelocity = velComp.y;

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

            //Resize player (from velocity or acceleration)
            resizePlayerFromAcceleration();

            //Recover the player to normal size over time
            recoverSize();

            //Speed up horizontally
            //velComp.incVelocity(xAccel * deltaTime, 0);

            //Intersect sides of screen check
            if (posComp.x + posComp.width / 2 >= level.levelWidth && velComp.x > 0) intersectRightSideScreen();
            if (posComp.x - posComp.width / 2 <= 0 && velComp.x < 0) intersectLeftSideScreen();
            if (posComp.y + posComp.height / 2 >= level.levelHeight && velComp.y > 0) intersectBottomSideScreen();
            if (posComp.y - posComp.height / 2 <= 0 && velComp.y < 0) intersectTopSizeScreen();

            //Off sides of screen check
            if (posComp.x > level.levelWidth + posComp.width && velComp.x > 0) offRightSideScreen();
            if (posComp.x < -posComp.width && velComp.x < 0) offLeftSideScreen();
            if (posComp.y > level.levelHeight + posComp.height && velComp.y > 0) offBottomSideScreen();
            if (posComp.y < -posComp.height && velComp.y < 0) offTopSizeScreen();

            //Reset passedAirJumps if needed
            if (passedAirjumps != 0 && level.getCollisionSystem().findObjectsBetweenPoints(
                posComp.x - posComp.width / 2, posComp.y + (posComp.height / 2) + 1, posComp.x + posComp.width / 2, posComp.y +
                (posComp.height / 2) + 1).Count > 0)
            {
                passedAirjumps = 0;
            }

        }
        //-----------------------------------------------------------------------------


        //------------------ Stuff for when player intersects/goes off screen --------------------
        public void intersectLeftSideScreen()
        {
            //level.getMovementSystem().changeSingleAxisLocation('X', posComp, level.levelWidth + posComp.width);
            endLeftHorizontalMove();
            posComp.x = posComp.width / 2;
        }
        public void intersectRightSideScreen()
        {

        }
        public void intersectTopSizeScreen()
        {
            endUpperMove();
            posComp.y = posComp.height / 2;
        }
        public void intersectBottomSideScreen()
        {

        }


        public void offLeftSideScreen()
        {
            //level.getMovementSystem().changeSingleAxisLocation('X', posComp, level.levelWidth + posComp.width); //Screen Wrap
            endLeftHorizontalMove();
        }
        public void offRightSideScreen()
        {
            level.getMovementSystem().changeSingleAxisLocation('X', posComp, -posComp.width); //Screen Wrap
        }
        public void offTopSizeScreen()
        {
            //level.getMovementSystem().changeSingleAxisLocation('Y', posComp, level.levelHeight + posComp.height); //Screen Wrap
            endUpperMove();
        }
        public void offBottomSideScreen()
        {
            level.getMovementSystem().changeSingleAxisLocation('Y', posComp, -posComp.height); //Screen Wrap
        }


        //-----------------------------------------------------------------------------


        //----------------------------------- Input ----------------------------------- 
        public void KeyDown(KeyEventArgs e)
        {
            if (hasNullComponent()) return;

            if (e.KeyData == playerComp.jumpKey)
            {
                playerJump();
            }
            if (e.KeyData == playerComp.leftKey)
            {
                beginMoveLeft();
            }
            if (e.KeyData == playerComp.rightKey)
            {
                beginMoveRight();
            }


            if (e.KeyData == Keys.R)
            {
                level.resetLevel();
            }
        }
        public void KeyUp(KeyEventArgs e)
        {

            if (hasNullComponent()) return;

            if (e.KeyData == playerComp.leftKey)
            {
                endLeftHorizontalMove();
            }
            if (e.KeyData == playerComp.rightKey)
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

        /*
         * Keeping these since there's quite a lot of code in them that may be
         * useful in the future.
        public void resizePlayerFromVelocity()
        {
            //Get the velocity in each direction
            float xSpeed = velComp.x;
            float ySpeed = velComp.y;

            //Difference from center speeds
            float diffX = Math.Abs(xSpeed - normWidth);
            float diffY = Math.Abs(ySpeed - normHeight);

            //Calculate new width and height
            float newWidth = baseWidth + (diffX * xIncSpeedStretchMultiplier);
            float newHeight = baseHeight + (diffY * yIncSpeedStretchMultiplier);

            //Don't go below minimum
            if (newWidth < minWidth) newWidth = minWidth;
            if (newHeight < minHeight) newHeight = minHeight;
            //Or over the max
            if (newWidth > maxWidth) newWidth = maxWidth;
            if (newHeight > maxHeight) newHeight = maxHeight;


            if (newWidth * newHeight > maxSurfaceArea)
            {
                //if only one has been changed - shrink the other
                if (Math.Abs(newWidth - posComp.width) < xStretchThreshold)
                {
                    newWidth = (maxSurfaceArea) / (newHeight);
                }
                else if (Math.Abs(newHeight - posComp.height) < yStretchThreshold)
                {
                    newHeight = (maxSurfaceArea) / (newWidth);
                }
                //If they've both been changed - take the ratio
                else
                {
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult / 2;
                    newHeight = newHeight * mult / 2;
                }
            }
            //Don't Go Under Min Surface Area
            if (newWidth * newHeight < minSurfaceArea)
            {
                //if only one changed then expand the other
                if (Math.Abs(newWidth - posComp.width) < xStretchThreshold)
                {
                    newWidth = (minSurfaceArea) / (newHeight);
                }
                else if (Math.Abs(newHeight - posComp.height) < yStretchThreshold)
                {
                    newHeight = (minSurfaceArea) / (newWidth);
                }
                //If they've both been changed - take the ratio
                else
                {
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult / 2;
                    newHeight = newHeight * mult / 2;
                }
            }




            //Assign the players new width
            level.getMovementSystem().changeSize(posComp, newWidth, newHeight);
        }

        public void resizePlayerFromAccelerationOLD()
        {

            float xSpeed = velComp.x;
            float ySpeed = velComp.y;

            float xChange = getChange(xSpeed, prevXVelocity);
            float yChange = getChange(ySpeed, prevYVelocity);

            float newWidth = getNewDim(xChange, xStretchThreshold, justChangedX, xIncSpeedStretchMultiplier, posComp.width, baseWidth, xEaseRate);
            float newHeight = getNewDim(yChange, yStretchThreshold, justChangedY, yIncSpeedStretchMultiplier, posComp.height, baseHeight, yEaseRate);

            
            //Don't go below minimum
            if (newWidth < minWidth) newWidth = minWidth;
            if (newHeight < minHeight) newHeight = minHeight;
            //Or over the max
            if (newWidth > maxWidth) newWidth = maxWidth;
            if (newHeight > maxHeight) newHeight = maxHeight;
            

            //Don't Go Over Max Surface Area
            if (newWidth * newHeight > maxSurfaceArea)
            {
                //if only one has been changed - shrink the other
                if (Math.Abs(newWidth - posComp.width) < xStretchThreshold)
                {
                    //newWidth = (maxSurfaceArea) / (newHeight);
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult;
                }
                else if (Math.Abs(newHeight - posComp.height) < yStretchThreshold)
                {
                    //newHeight = (maxSurfaceArea) / (newWidth);
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newHeight = newHeight * mult;
                }
                //If they've both been changed - take the ratio
                else
                {
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult / 2;
                    newHeight = newHeight * mult / 2;
                }
            }
            //Don't Go Under Min Surface Area
            if (newWidth * newHeight < minSurfaceArea)
            {
                //if only one changed then expand the other
                if (Math.Abs(newWidth - posComp.width) < xStretchThreshold)
                {
                    newWidth = (minSurfaceArea) / (newHeight);
                }
                else if (Math.Abs(newHeight - posComp.height) < yStretchThreshold)
                {
                    newHeight = (minSurfaceArea) / (newWidth);
                }
                //If they've both been changed - take the ratio
                else
                {
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult / 2;
                    newHeight = newHeight * mult / 2;
                }
            }



            //Set the new size
            level.getMovementSystem().changeSize(posComp, newWidth, newHeight);

            //Store previous velocities
            prevXVelocity = xSpeed;
            prevYVelocity = ySpeed;

        }
         
        public float getChange(float current, float prev)
        {
            float change = current - prev;

            //Too lazy to think math stuff - below is to get the sign

            //zero = shrink (usually)
            if (current == 0) change = -Math.Abs(change);

            //Different signs
            if (!sameSigns(current, prev))
            {
                //down to up = grow
                if (current < 0) change = Math.Abs(change);
                else change = -Math.Abs(change);
            }

            //Smaller value of the same sign = shrink
            if (sameSigns(current, prev))
            {
                //slower = shrink
                if (Math.Abs(current) < Math.Abs(prev)) change = -Math.Abs(change);
                else change = Math.Abs(change);
            }

            return change;
        }
        

        public float getNewDim(float change, float trigger, bool justChanged, float multiplier, float newDim, float baseDim, float easeRate)
        {
            if (Math.Abs(change) >= trigger && !justChanged)
            {
                newDim = baseDim + (change * multiplier);
                justChanged = true;
            }
            else
            {

                justChanged = false;

                //Ease back
                if (newDim < baseDim)
                {
                    newDim += easeRate;
                    if (newDim > baseDim) newDim = baseDim;
                }
                else if (newDim > baseDim)
                {
                    newDim -= easeRate;
                    if (newDim < baseDim) newDim = baseDim;
                }

            }

            return newDim;
        }
         */


        public void resizePlayerFromAcceleration()
        {
            float xSpeed = velComp.x;
            float ySpeed = velComp.y;


            float xChange = Math.Abs(xSpeed) - Math.Abs(prevXVelocity);
            float yChange = Math.Abs(ySpeed) - Math.Abs(prevYVelocity);

            float xSquishAmount = (-xChange * xDecSpeedStretchMultiplier);
            float ySquishAmount = (-yChange * yDecSpeedStretchMultiplier);

            if (xChange > 0)
            {
                xSquishAmount = (-xChange * xIncSpeedStretchMultiplier);
            }
            if (yChange > 0)
            {
                ySquishAmount = (-yChange * yIncSpeedStretchMultiplier);
            }


            //---------------------------- RESTRICTION CALCULATIONS ----------------------
            float newWidth = posComp.width - xSquishAmount;
            float newHeight = posComp.height - ySquishAmount;
            
            if (newWidth > maxWidth) newWidth = maxWidth;
            if (newHeight > maxHeight) newHeight = maxHeight;

            if (newWidth < minWidth) newWidth = minWidth;
            if (newHeight < minHeight) newHeight = minHeight;
            
            
            //Don't Go Over Max Surface Area
            if ( (newWidth * newHeight) > maxSurfaceArea)
            {
                //if only one has been changed, and the other is relatively normal - shrink the other
                if (Math.Abs(newWidth - posComp.width) < xStretchThreshold)
                {
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult;
                }
                else if (Math.Abs(newHeight - posComp.height) < yStretchThreshold)
                {
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newHeight = newHeight * mult;
                }
                //If they've both been changed - take the ratio
                else
                {
                    float mult = maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * (float)Math.Sqrt(mult);
                    newHeight = newHeight * (float)Math.Sqrt(mult);
                }
            }
            
            //Don't Go Under Min Surface Area
            if ( (newWidth * newHeight) < minSurfaceArea)
            {
                //if only one changed then expand the other
                if (Math.Abs(newWidth - posComp.width) < xStretchThreshold)
                {
                    float mult = minSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult;
                }
                else if (Math.Abs(newHeight - posComp.height) < yStretchThreshold)
                {
                    float mult = minSurfaceArea / (newWidth * newHeight);
                    newHeight = newHeight * mult;
                }
                //If they've both been changed - take the ratio
                else
                {
                    float mult = minSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * (float)Math.Sqrt(mult);
                    newHeight = newHeight * (float)Math.Sqrt(mult);
                }
            }
            
            //-------------------------------------------------------------------
            
            xSquishAmount = posComp.width - newWidth;
            ySquishAmount = posComp.height - newHeight;
            
              
            //if it going right, squish right.
            if (prevXVelocity > 0 && Math.Abs(xSquishAmount) >= xStretchThreshold)
            {
                squishRight(xSquishAmount);
            }
            else if (prevXVelocity < 0 && Math.Abs(xSquishAmount) >= xStretchThreshold)
            {
                squishLeft(xSquishAmount);
            }
            else if (Math.Abs(xSquishAmount) >= xStretchThreshold)
            {
                squishWidthCenter(xSquishAmount);
            }

            //If going down, squish down
            if (prevYVelocity > 0 && Math.Abs(ySquishAmount) >= yStretchThreshold)
            {
                squishDown(ySquishAmount);
            }
            else if (velComp.y == 0 && prevYVelocity == 0 && Math.Abs(ySquishAmount) >= yStretchThreshold)
            {
                squishHeightCenter(ySquishAmount);
            }
            else if(Math.Abs(ySquishAmount) >= yStretchThreshold) // && velComp.y < 0
            {
                squishDown(ySquishAmount);
            }

            prevXVelocity = xSpeed;
            prevYVelocity = ySpeed;
        }

        //-------------------------- Squish Functions --------------------------------------------

        public void squishDown(float amt)
        {

            while (posComp.height - amt < minHeight)
            {
                amt--;
            }


            level.getMovementSystem().changeHeight(posComp, posComp.height - amt);
            //posComp.height = posComp.height - amt;
            level.getMovementSystem().changePosition(posComp, posComp.x, posComp.y + amt / 2);

        }

        public void squishUp(float amt)
        {

            while (posComp.height - amt < minHeight)
            {
                amt--;
            }


            level.getMovementSystem().changeHeight(posComp, posComp.height - amt);
            //posComp.height = posComp.height - amt;
            level.getMovementSystem().changePosition(posComp, posComp.x, posComp.y - amt / 2);

        }

        public void squishLeft(float amt)
        {

            while (posComp.width - amt < minWidth)
            {
                amt--;
            }

            level.getMovementSystem().changeWidth(posComp, posComp.width - amt);
            //posComp.width = posComp.width - amt;
            level.getMovementSystem().changePosition(posComp, posComp.x - amt / 2, posComp.y);
        }

        public void squishRight(float amt)
        {

            while (posComp.width - amt < minWidth)
            {
                amt--;
            }

            level.getMovementSystem().changeWidth(posComp, posComp.width - amt);
            //posComp.width = posComp.width - amt;
            level.getMovementSystem().changePosition(posComp, posComp.x + amt / 2, posComp.y);
        }

        public void squishWidthCenter(float amt)
        {
            while (posComp.width - amt < minWidth)
            {
                amt--;
            }

            level.getMovementSystem().changeWidth(posComp, posComp.width - amt);
            //posComp.width = posComp.width - amt;
        }

        public void squishHeightCenter(float amt)
        {
            while (posComp.height - amt < minHeight)
            {
                amt--;
            }

            level.getMovementSystem().changeHeight(posComp, posComp.height - amt);
            //posComp.height = posComp.height - amt;
        }

        //----------------------------------------------------------------------------------------

        public void recoverSize()
        {
            //Restore Width
            if (posComp.width < baseWidth)
            {
                //Collisions on left or right side?
                bool leftSide = (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2 - 1,
                    posComp.y - posComp.height / 2-1, posComp.x - posComp.width / 2 - 1, posComp.y + posComp.height / 2-1).Count > 0);
                bool rightSide = (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x + posComp.width / 2 + 1,
                    posComp.y - posComp.height / 2-1, posComp.x + posComp.width / 2 + 1, posComp.y + posComp.height / 2-1).Count > 0);

                if (!leftSide && !rightSide)
                {
                    //squishLeft(-xEaseRate / 2);
                    //squishRight(-xEaseRate / 2);
                    squishWidthCenter(-xEaseRate);
                }
                if (leftSide && !rightSide)
                {
                    squishRight(-xEaseRate);
                }
                if (!leftSide && rightSide)
                {
                    squishLeft(-xEaseRate);
                }

                //Don't over compensate
                if (posComp.width > baseWidth) level.getMovementSystem().changeWidth(posComp, baseWidth);
            }
            else if (posComp.width > baseWidth)
            {
                posComp.width -= xEaseRate;
                if (posComp.width < baseWidth) level.getMovementSystem().changeWidth(posComp, baseWidth);
            }


            //Restore Height
            if (posComp.height < baseHeight)
            {

                //Collisions on left or right side?
                bool upperSide = (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2+1,
                    posComp.y + posComp.height / 2 + 1, posComp.x + posComp.width / 2-1, posComp.y + posComp.height / 2 + 1).Count > 0);
                bool lowerSide = (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2+1,
                    posComp.y - posComp.height / 2 - 1, posComp.x + posComp.width / 2-1, posComp.y - posComp.height / 2 - 1).Count > 0);

                if (!upperSide && !lowerSide)
                {
                    squishHeightCenter(-xEaseRate);
                }
                if (upperSide && !lowerSide)
                {
                    squishDown(-xEaseRate);
                }
                if (!upperSide && lowerSide)
                {
                    squishUp(-xEaseRate);
                }
                
                //Don't overcompensate
                if (posComp.height > baseHeight) level.getMovementSystem().changeHeight(posComp, baseHeight);
            }
            else if (posComp.height > baseHeight)
            {
                posComp.height -= yEaseRate;
                if (posComp.height < baseHeight) level.getMovementSystem().changeHeight(posComp, baseHeight);
            }

        }

        public bool sameSigns(float num1, float num2)
        {
            return ((num1 < 0 && num2 < 0) || (num1 > 0 && num2 > 0));
        }

    }
}
