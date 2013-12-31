using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Level_Editor;

namespace RunningGame.Systems {

    /*
     * This system is not very well implemented - my bad.
     * There's a lot of mixup between this and the PlayerSystem, I will have
     * to try and sort that out at some point.
     * 
     * What this is supposed to be is the go-to place to move an entity.
     * It also handles moving objects that have a certain velocity.
     */

    [Serializable()]
    public class MovementSystem : GameSystem {
        List<string> requiredComponents = new List<string>();
        Level level;

        CollisionHandler colHandler;

        public MovementSystem(Level level) {

            //Required Components
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity Component
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position Component

            this.level = level;

            colHandler = new CollisionHandler(level);

        }

        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }
        public override Level GetActiveLevel() {
            return level;
        }

        //Apply any velocities
        public override void Update(float deltaTime) {
            if (!(level is CreationLevel)) {
                foreach (Entity e in getApplicableEntities()) {

                    //Pull out needed components
                    PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

                    if (posComp.x != posComp.prevX) posComp.prevX = posComp.x;
                    if (posComp.y != posComp.prevY) posComp.prevY = posComp.y;

                    //Add velocity to position
                    incrementPosition(posComp, velComp.x * deltaTime, velComp.y * deltaTime);

                }

            }
        }


        //----------------------------------------------DIRECT CHANGES IN POSITION------------------------------------------------
        //Location
        public void incrementPosition(PositionComponent posComp, float incX, float incY) {
            if (incX != 0) {
                changeSingleAxisLocation('X', posComp, posComp.x + incX, true);
            }
            if (incY != 0) {
                changeSingleAxisLocation('Y', posComp, posComp.y + incY, true);
            }
        }

        public void changePosition(PositionComponent posComp, float newX, float newY, bool moveToContact) {
            if (newX != posComp.x) {
                //Console.WriteLine("Changing x from " + posComp.x + " to " + newX);
                changeSingleAxisLocation('X', posComp, newX, moveToContact);
            }
            if (newY != posComp.y) {
                //Console.WriteLine("Changing y from " + posComp.y + " to " + newY);
                changeSingleAxisLocation('Y', posComp, newY, moveToContact);
            }
        }
        public void teleportToNoCollisionCheck(PositionComponent posComp, float newX, float newY) {
            posComp.prevX = posComp.x;
            posComp.x = newX;
            posComp.prevY = posComp.y;
            posComp.y = newY;
            posComp.positionHasChanged = true;
        }

        public void changeSingleAxisLocation(Char axis, PositionComponent posComp, float newVal, bool moveToContact) {
            bool movementBlocked = false;

            bool isX = true;
            float xVal = 0; //New X
            float yVal = 0; //New Y

            if (axis == 'X' || axis == 'x') {
                xVal = newVal;
                yVal = posComp.y;
            } else if (axis == 'Y' || axis == 'y') {
                isX = false;
                xVal = posComp.x;
                yVal = newVal;
            } else {
                Console.WriteLine("Changing Unrecognized Axis in Movement System: " + axis);
                return;
            }

            //Check for collisions before it moves
            if (posComp.myEntity.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME)) {

                VelocityComponent velComp = (VelocityComponent)posComp.myEntity.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

                //List of all collisions caused by potential move
                List<Entity> collisions = level.getCollisionSystem().checkForCollision(posComp.myEntity, xVal, yVal, posComp.width, posComp.height);

                //Perform all collisions
                if (collisions.Count > 0 && !(level is CreationLevel)) {
                    foreach (Entity e in collisions) {
                        if (e.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME)) {
                            //Handle the collision. handleCollision(...) will return true if it should stop the movement.
                            if (colHandler.handleCollision(posComp.myEntity, e)) {

                                if (isX) velComp.x = 0;
                                else velComp.y = 0;

                                movementBlocked = true;

                                //Move to edge of object if already mostly at the edge... ONLY IF MOVE TO CONTACT IS TRUE
                                if (moveToContact) {
                                    PositionComponent otherPosComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

                                    if (isX) {
                                        if (posComp.x < (otherPosComp.x))
                                            level.getMovementSystem().changePosition(posComp, getClosestPositionWithNoCollision(posComp, otherPosComp, true, true),
                                                posComp.y, false);
                                        else
                                            level.getMovementSystem().changePosition(posComp, getClosestPositionWithNoCollision(posComp, otherPosComp, true, false),
                                                posComp.y, false);

                                        //posComp.positionHasChanged = true;
                                    } else {
                                        if (posComp.y < (otherPosComp.y))
                                            level.getMovementSystem().changePosition(posComp, posComp.x, getClosestPositionWithNoCollision(posComp, otherPosComp, false, true),
                                                false);
                                        else
                                            level.getMovementSystem().changePosition(posComp, posComp.x, getClosestPositionWithNoCollision(posComp, otherPosComp, false, false),
                                                false);

                                        //posComp.positionHasChanged = true;
                                    }

                                }
                            }
                        }
                    }
                }
            }


            //If movement isn't blocked - move and set position changed to true
            if (!movementBlocked) {
                if (isX) {
                    posComp.prevX = posComp.x;
                    posComp.x = newVal;
                } else {
                    posComp.prevY = posComp.y;
                    posComp.y = newVal;
                }
                posComp.positionHasChanged = true;
            }
        }


        public float getClosestPositionWithNoCollision(PositionComponent pos1, PositionComponent pos2, bool xDir, bool leftOrUp) {
            float retPos = 0.0f;

            if (xDir && leftOrUp) retPos = pos2.x - pos2.width / 2 - pos1.width / 2;
            else if (xDir && !leftOrUp) retPos = pos2.x + pos2.width / 2 + pos1.width / 2;
            else if (!xDir && leftOrUp) retPos = pos2.y - pos2.height / 2 - pos1.height / 2;
            else if (!xDir && !leftOrUp) retPos = pos2.y + pos2.height / 2 + pos1.height / 2;

            if (!level.getCollisionSystem().locGrid.preciseCollisionChecking) {
                return retPos;
            }

            float newX = pos1.x;
            float newY = pos1.y;

            if (xDir) newX = retPos;
            else newY = retPos;

            List<Entity> cols = level.getCollisionSystem().checkForCollision(pos1.myEntity, newX, newY, pos1.width, pos1.height);
            while (cols.Count <= 0) {
                if (leftOrUp) retPos += 1;
                else retPos -= 1;
                if (xDir) newX = retPos;
                else newY = retPos;
                cols = level.getCollisionSystem().checkForCollision(pos1.myEntity, newX, newY, pos1.width, pos1.height);
            }


            return retPos;
        }

        //Size
        public void changeSize(PositionComponent posComp, float newW, float newH) {
            posComp.prevW = posComp.width;
            posComp.width = newW;

            posComp.prevH = posComp.height;
            posComp.height = newH;

            posComp.positionHasChanged = true;
        }
        public void changeWidth(PositionComponent posComp, float newW) {
            changeSize(posComp, newW, posComp.height);
        }
        public void changeHeight(PositionComponent posComp, float newH) {
            changeSize(posComp, posComp.width, newH);
        }
        //--------------------------------------------------------------------------------------------------------------------------

    }
}
