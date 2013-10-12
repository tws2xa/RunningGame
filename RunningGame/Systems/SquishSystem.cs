using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Systems
{
    public class SquishSystem : GameSystem
    {

        //All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public SquishSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.SQUISH_COMPONENT_NAME); //Squish
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); // Velocity


            this.level = level; //Always have this

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
            foreach (Entity e in getApplicableEntities())
            {
                //Resize from acceleration
                resizeEntityFromAcceleration(e);

                //Recover to normal size over time
                recoverSize(e);
            }
        }
        //----------------------------------------------------------------------------------------------

        public void resizeEntityFromAcceleration(Entity e)
        {

            VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            SquishComponent squishComp = (SquishComponent)e.getComponent(GlobalVars.SQUISH_COMPONENT_NAME);
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

            float xSpeed = velComp.x;
            float ySpeed = velComp.y;

            float xChange = Math.Abs(xSpeed) - Math.Abs(squishComp.prevXVelocity);
            float yChange = Math.Abs(ySpeed) - Math.Abs(squishComp.prevYVelocity);

            float xSquishAmount = (-xChange * squishComp.xDecSpeedStretchMultiplier);
            float ySquishAmount = (-yChange * squishComp.yDecSpeedStretchMultiplier);

            if (xChange > 0)
            {
                xSquishAmount = (-xChange * squishComp.xIncSpeedStretchMultiplier);
            }
            if (yChange > 0)
            {
                ySquishAmount = (-yChange * squishComp.yIncSpeedStretchMultiplier);
            }


            //---------------------------- RESTRICTION CALCULATIONS ----------------------
            float newWidth = posComp.width - xSquishAmount;
            float newHeight = posComp.height - ySquishAmount;

            if (newWidth > squishComp.maxWidth) newWidth = squishComp.maxWidth;
            if (newHeight > squishComp.maxHeight) newHeight = squishComp.maxHeight;

            if (newWidth < squishComp.minWidth) newWidth = squishComp.minWidth;
            if (newHeight < squishComp.minHeight) newHeight = squishComp.minHeight;


            //Don't Go Over Max Surface Area
            if ((newWidth * newHeight) > squishComp.maxSurfaceArea)
            {
                //if only one has been changed, and the other is relatively normal - shrink the other
                if (Math.Abs(newWidth - posComp.width) < squishComp.xStretchThreshold)
                {
                    float mult = squishComp.maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult;
                }
                else if (Math.Abs(newHeight - posComp.height) < squishComp.yStretchThreshold)
                {
                    float mult = squishComp.maxSurfaceArea / (newWidth * newHeight);
                    newHeight = newHeight * mult;
                }
                //If they've both been changed - take the ratio
                else
                {
                    float mult = squishComp.maxSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * (float)Math.Sqrt(mult);
                    newHeight = newHeight * (float)Math.Sqrt(mult);
                }
            }

            //Don't Go Under Min Surface Area
            if ((newWidth * newHeight) < squishComp.minSurfaceArea)
            {
                //if only one changed then expand the other
                if (Math.Abs(newWidth - posComp.width) < squishComp.xStretchThreshold)
                {
                    float mult = squishComp.minSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * mult;
                }
                else if (Math.Abs(newHeight - posComp.height) < squishComp.yStretchThreshold)
                {
                    float mult = squishComp.minSurfaceArea / (newWidth * newHeight);
                    newHeight = newHeight * mult;
                }
                //If they've both been changed - take the ratio
                else
                {
                    float mult = squishComp.minSurfaceArea / (newWidth * newHeight);
                    newWidth = newWidth * (float)Math.Sqrt(mult);
                    newHeight = newHeight * (float)Math.Sqrt(mult);
                }
            }

            //-------------------------------------------------------------------

            xSquishAmount = posComp.width - newWidth;
            ySquishAmount = posComp.height - newHeight;


            //if it going right, squish right.
            if (squishComp.prevXVelocity > 0 && Math.Abs(xSquishAmount) >= squishComp.xStretchThreshold)
            {
                squishRight(posComp, squishComp, xSquishAmount);
            }
            else if (squishComp.prevXVelocity < 0 && Math.Abs(xSquishAmount) >= squishComp.xStretchThreshold)
            {
                squishLeft(posComp, squishComp, xSquishAmount);
            }
            else if (Math.Abs(xSquishAmount) >= squishComp.xStretchThreshold)
            {
                squishWidthCenter(posComp, squishComp, xSquishAmount);
            }

            //If going down, squish down
            if (squishComp.prevYVelocity > 0 && Math.Abs(ySquishAmount) >= squishComp.yStretchThreshold)
            {
                squishDown(posComp, squishComp, ySquishAmount);
            }
            else if (velComp.y == 0 && squishComp.prevYVelocity == 0 && Math.Abs(ySquishAmount) >= squishComp.yStretchThreshold)
            {
                squishHeightCenter(posComp, squishComp, ySquishAmount);
            }
            else if (Math.Abs(ySquishAmount) >= squishComp.yStretchThreshold) // && velComp.y < 0
            {
                squishDown(posComp, squishComp, ySquishAmount);
            }

            squishComp.prevXVelocity = xSpeed;
            squishComp.prevYVelocity = ySpeed;
        }



        //--------------------------------- Stretch Methods ----------------------------------------

        public void squishDown(PositionComponent posComp, SquishComponent squishComp, float amt)
        {

            while (posComp.height - amt < squishComp.minHeight)
            {
                amt--;
            }


            level.getMovementSystem().changeHeight(posComp, posComp.height - amt);
            level.getMovementSystem().changePosition(posComp, posComp.x, posComp.y + amt / 2, true);

        }

        public void squishUp(PositionComponent posComp, SquishComponent squishComp, float amt)
        {

            while (posComp.height - amt < squishComp.minHeight)
            {
                amt--;
            }


            level.getMovementSystem().changeHeight(posComp, posComp.height - amt);
            level.getMovementSystem().changePosition(posComp, posComp.x, posComp.y - amt / 2, true);

        }

        public void squishLeft(PositionComponent posComp, SquishComponent squishComp, float amt)
        {

            while (posComp.width - amt < squishComp.minWidth)
            {
                amt--;
            }

            level.getMovementSystem().changeWidth(posComp, posComp.width - amt);
            level.getMovementSystem().changePosition(posComp, posComp.x - amt / 2, posComp.y, true);
        }

        public void squishRight(PositionComponent posComp, SquishComponent squishComp, float amt)
        {

            while (posComp.width - amt < squishComp.minWidth)
            {
                amt--;
            }

            level.getMovementSystem().changeWidth(posComp, posComp.width - amt);
            level.getMovementSystem().changePosition(posComp, posComp.x + amt / 2, posComp.y, true);
        }

        public void squishWidthCenter(PositionComponent posComp, SquishComponent squishComp, float amt)
        {
            while (posComp.width - amt < squishComp.minWidth)
            {
                amt--;
            }

            level.getMovementSystem().changeWidth(posComp, posComp.width - amt);
        }

        public void squishHeightCenter(PositionComponent posComp, SquishComponent squishComp, float amt)
        {
            while (posComp.height - amt < squishComp.minHeight)
            {
                amt--;
            }

            level.getMovementSystem().changeHeight(posComp, posComp.height - amt);
        }

        public void recoverSize(Entity e)
        {

            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            SquishComponent squishComp = (SquishComponent)e.getComponent(GlobalVars.SQUISH_COMPONENT_NAME);

            //Restore Width
            if (posComp.width < squishComp.baseWidth)
            {
                //Collisions on left or right side?
                bool leftSide = (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2 - 1,
                    posComp.y - posComp.height / 2 - 1, posComp.x - posComp.width / 2 - 1, posComp.y + posComp.height / 2 - 1).Count > 0);
                bool rightSide = (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x + posComp.width / 2 + 1,
                    posComp.y - posComp.height / 2 - 1, posComp.x + posComp.width / 2 + 1, posComp.y + posComp.height / 2 - 1).Count > 0);

                if (!leftSide && !rightSide)
                {
                    squishWidthCenter(posComp, squishComp, -squishComp.xEaseRate);
                }
                if (leftSide && !rightSide)
                {
                    squishRight(posComp, squishComp, -squishComp.xEaseRate);
                }
                if (!leftSide && rightSide)
                {
                    squishLeft(posComp, squishComp, -squishComp.xEaseRate);
                }

                //Don't over compensate
                if (posComp.width > squishComp.baseWidth) level.getMovementSystem().changeWidth(posComp, squishComp.baseWidth);
            }
            else if (posComp.width > squishComp.baseWidth)
            {
                level.getMovementSystem().changeWidth(posComp, posComp.width - squishComp.xEaseRate);
                if (posComp.width < squishComp.baseWidth) level.getMovementSystem().changeWidth(posComp, squishComp.baseWidth);
            }


            //Restore Height
            if (posComp.height < squishComp.baseHeight)
            {

                //Collisions on left or right side?
                bool upperSide = (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2 + 1,
                    posComp.y + posComp.height / 2 + 1, posComp.x + posComp.width / 2 - 1, posComp.y + posComp.height / 2 + 1).Count > 0);
                bool lowerSide = (level.getCollisionSystem().findObjectsBetweenPoints(posComp.x - posComp.width / 2 + 1,
                    posComp.y - posComp.height / 2 - 1, posComp.x + posComp.width / 2 - 1, posComp.y - posComp.height / 2 - 1).Count > 0);

                if (!upperSide && !lowerSide)
                {
                    squishHeightCenter(posComp, squishComp, -squishComp.xEaseRate);
                }
                if (upperSide && !lowerSide)
                {
                    squishDown(posComp, squishComp, -squishComp.xEaseRate);
                }
                if (!upperSide && lowerSide)
                {
                    squishUp(posComp, squishComp, -squishComp.xEaseRate);
                }

                //Don't overcompensate
                if (posComp.height > squishComp.baseHeight) level.getMovementSystem().changeHeight(posComp, squishComp.baseHeight);
            }
            else if (posComp.height > squishComp.baseHeight)
            {
                level.getMovementSystem().changeHeight(posComp, posComp.height - squishComp.yEaseRate);
                if (posComp.height < squishComp.baseHeight) level.getMovementSystem().changeHeight(posComp, squishComp.baseHeight);
            }

        }
    }
}
