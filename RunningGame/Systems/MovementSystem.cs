using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Systems
{

    /*
     * This system is not very well implemented - my bad.
     * There's a lot of mixup between this and the PlayerSystem, I will have
     * to try and sort that out at some point.
     * 
     * What this is supposed to be is the go-to place to move an entity.
     * It also handles moving objects that have a certain velocity.
     */

    class MovementSystem : GameSystem
    {

        ArrayList requiredComponents = new ArrayList();
        Level level;

        CollisionHandler colHandler;

        public MovementSystem(Level level)
        {

            //Required Components
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity Component
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position Component

            this.level = level;

            colHandler = new CollisionHandler();

        }

        public override ArrayList getRequiredComponents()
        {
            return requiredComponents;
        }
        public override Level GetActiveLevel()
        {
            return level;
        }

        //Apply any velocities
        public override void Update(float deltaTime)
        {

            foreach (Entity e in getApplicableEntities())
            {

                //Pull out needed components
                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

                if (posComp.x != posComp.prevX) posComp.prevX = posComp.x;
                if (posComp.y != posComp.prevY) posComp.prevY = posComp.y;

                //Add velocity to position
                incrementPosition(posComp, velComp.x * deltaTime, velComp.y * deltaTime);

            }
        }


        //----------------------------------------------DIRECT CHANGES IN POSITION------------------------------------------------
        //Location
        public void incrementPosition(PositionComponent posComp, float incX, float incY)
        {
            if (incX != 0)
            {
                changeSingleAxisLocation('X', posComp, posComp.x + incX);
            }
            if (incY != 0)
            {
                changeSingleAxisLocation('Y', posComp, posComp.y + incY);
            }
        }

        public void changePosition(PositionComponent posComp, float newX, float newY)
        {
            if (newX != posComp.x)
            {
                changeSingleAxisLocation('X', posComp, newX);
            }
            if (newY != posComp.y)
            {
                changeSingleAxisLocation('Y', posComp, newY);
            }
        }

        /*
        public void changeSingleAxisLocation(Char axis, PositionComponent posComp, float newVal)
        {
            bool movementBlocked = false;

            bool isX = true;
            float xVal = 0; //New X
            float yVal = 0; //New Y

            if (axis == 'X' || axis == 'x')
            {
                xVal = newVal;
                yVal = posComp.y;
            }
            else if (axis == 'Y' || axis == 'y')
            {
                isX = false;
                xVal = posComp.x;
                yVal = newVal;
            }
            else
            {
                Console.WriteLine("Changing Unrecognized Axis in Movement System: " + axis);
                return;
            }

            //Check for collisions
            if (posComp.myEntity.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME))
            {

                //TODO - Check a line rather than just the point. Maybe check every minimum tile size?

                
                 //* Theory:
                 //* diff = difference between current position and move position
                 //* while(diff > minTileSize) {
                 //*      checkForCollisions(loc+minTileSize)
                 //*      if none: move
                 //*      else: handle collision, if stop - moveTo then exit
                 //*      diff = difference between newLoc and goal;
                 //* }
                 //* checkForCollisions(loc+diff);
                 //* if none: move
                 //* else: handle collision, if stop - moveTo then stop.
                 
                VelocityComponent velComp = (VelocityComponent)posComp.myEntity.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

                float diff = 0;
                if (isX)
                {
                    diff = velComp.x;
                }
                else
                {
                    diff = velComp.y;
                }















                //List of all collisions caused by potential move
                ArrayList collisions = level.getCollisionSystem().checkForCollision(posComp.myEntity, xVal, yVal);
                //ArrayList collisions = level.getCollisionSystem().findObjectsBetweenPoints(posComp.x, posComp.y, xVal, yVal);

                //If there's a collision
                if (collisions.Count > 0)
                {
                    foreach (Entity e in collisions)
                    {
                        //Handle the collision
                        if (colHandler.handleCollision(posComp.myEntity, e))
                        {

                            if (isX) velComp.x = 0;
                            else velComp.y = 0;

                            movementBlocked = true;

                            //Move to edge of object if already mostly at the edge...
                            PositionComponent otherPosComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

                            if (isX)
                            {
                                if (posComp.x < (otherPosComp.x))
                                    posComp.x = (otherPosComp.x - otherPosComp.width / 2 - posComp.width / 2);
                                else
                                    posComp.x = (otherPosComp.x + otherPosComp.width / 2 + posComp.width / 2);
                            }
                            else
                            {
                                if (posComp.y < (otherPosComp.y))
                                    posComp.y = (otherPosComp.y - otherPosComp.height / 2 - posComp.height / 2);
                                else
                                    posComp.y = (otherPosComp.y + otherPosComp.height / 2 + posComp.height / 2);
                            }
                        }
                    }
                }
            }


            //If movement isn't blocked - move and set position changed to true
            if (!movementBlocked)
            {
                if (isX)
                {
                    posComp.prevX = posComp.x;
                    posComp.x = newVal;
                }
                else
                {
                    posComp.prevY = posComp.y;
                    posComp.y = newVal;
                }
                posComp.positionHasChanged = true;
            }
        }
        */

        
         //* BACKUP OF CHANGING A SINGLE AXIS LOCATION
         
        public void changeSingleAxisLocation(Char axis, PositionComponent posComp, float newVal)
        {
            bool movementBlocked = false;

            bool isX = true;
            float xVal = 0; //New X
            float yVal = 0; //New Y

            if (axis == 'X' || axis == 'x')
            {
                xVal = newVal;
                yVal = posComp.y;
            }
            else if (axis == 'Y' || axis == 'y')
            {
                isX = false;
                xVal = posComp.x;
                yVal = newVal;
            }
            else
            {
                Console.WriteLine("Changing Unrecognized Axis in Movement System: " + axis);
                return;
            }

            //Check for collisions
            if (posComp.myEntity.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME))
            {

                //TODO - Check a line rather than just the point. Maybe check every minimum tile size?

                 // Theory:
                 // diff = difference between current position and move position
                 // while(diff > 0) {
                 //      if(diff > minTileSize) {
                 //          checkForCollisions(loc+minTileSize)
                 //          if none - move
                 //          else - handle collision, if stop then exit
                 //          diff = difference between newLoc and goal;
                 //      } else {
                 //          checkForCollisions(loc+diff);
                 //          if none - move
                 //          else - handle collision, if stop then exit
                 //      }
                 // }
                 
                 
                VelocityComponent velComp = (VelocityComponent)posComp.myEntity.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

                //List of all collisions caused by potential move
                ArrayList collisions = level.getCollisionSystem().checkForCollision(posComp.myEntity, xVal, yVal, posComp.width, posComp.height);
                //ArrayList collisions = level.getCollisionSystem().findObjectsBetweenPoints(posComp.x, posComp.y, xVal, yVal);

                //If there's a collision
                if (collisions.Count > 0)
                {
                    foreach (Entity e in collisions)
                    {
                        //Handle the collision
                        if (colHandler.handleCollision(posComp.myEntity, e))
                        {

                            if (isX) velComp.x = 0;
                            else velComp.y = 0;

                            movementBlocked = true;

                            //Move to edge of object if already mostly at the edge...
                            PositionComponent otherPosComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

                            if (isX)
                            {
                                if (posComp.x < (otherPosComp.x))
                                    level.getMovementSystem().changePosition(posComp, otherPosComp.x - otherPosComp.width / 2 - posComp.width / 2, posComp.y);
                                else
                                    level.getMovementSystem().changePosition(posComp, otherPosComp.x + otherPosComp.width / 2 + posComp.width / 2, posComp.y);

                                posComp.positionHasChanged = true;
                            }
                            else
                            {
                                if (posComp.y < (otherPosComp.y))
                                    level.getMovementSystem().changePosition(posComp, posComp.x, otherPosComp.y - otherPosComp.height / 2 - posComp.height / 2);
                                else
                                    level.getMovementSystem().changePosition(posComp, posComp.x, otherPosComp.y + otherPosComp.height / 2 + posComp.height / 2);

                                posComp.positionHasChanged = true;
                            }
                        }
                    }
                }
            }


            //If movement isn't blocked - move and set position changed to true
            if (!movementBlocked)
            {
                if (isX)
                {
                    posComp.prevX = posComp.x;
                    posComp.x = newVal;
                }
                else
                {
                    posComp.prevY = posComp.y;
                    posComp.y = newVal;
                }
                posComp.positionHasChanged = true;
            }
        }
        



        //Size
        public void changeSize(PositionComponent posComp, float newW, float newH)
        {
            posComp.prevW = posComp.width;
            posComp.width = newW;

            posComp.prevH = posComp.height;
            posComp.height = newH;

            posComp.positionHasChanged = true;
        }
        public void changeWidth(PositionComponent posComp, float newW)
        {
            changeSize(posComp, newW, posComp.height);
        }
        public void changeHeight(PositionComponent posComp, float newH)
        {
            changeSize(posComp, posComp.width, newH);
        }
        //--------------------------------------------------------------------------------------------------------------------------

    }
}
