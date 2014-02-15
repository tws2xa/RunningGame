using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Entities;
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

        public MovementSystem( Level level ) {

            //Required Components
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME ); //Velocity Component
            requiredComponents.Add( GlobalVars.POSITION_COMPONENT_NAME ); //Position Component

            this.level = level;

            colHandler = new CollisionHandler( level );

        }

        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }
        public override Level GetActiveLevel() {
            return level;
        }

        //Apply any velocities
        public override void Update( float deltaTime ) {
            if ( !( level is CreationLevel ) ) {
                foreach ( Entity e in getApplicableEntities() ) {

                    //Pull out needed components
                    PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                    VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );

                    if ( posComp.x != posComp.prevX ) posComp.prevX = posComp.x;
                    if ( posComp.y != posComp.prevY ) posComp.prevY = posComp.y;

                    //Add velocity to position
                    incrementPosition( posComp, velComp.x * deltaTime, velComp.y * deltaTime );
                    
                }

            }
        }


        //----------------------------------------------DIRECT CHANGES IN POSITION------------------------------------------------
        //Location
        public void incrementPosition( PositionComponent posComp, float incX, float incY ) {

            //Must do one at a time for moveToContact
            if(incX != 0) changePosition( posComp, posComp.x + incX, posComp.y, true, false);
            if(incY != 0) changePosition( posComp, posComp.x, posComp.y + incY, false, true );

            /*
            if ( incX != 0 ) {
                changeSingleAxisLocation( 'X', posComp, posComp.x + incX, true );
            }
            if ( incY != 0 ) {
                changeSingleAxisLocation( 'Y', posComp, posComp.y + incY, true );
            }
             */
        }

        public void changePosition( PositionComponent posComp, float newX, float newY, bool moveToContactH, bool moveToContactV ) {

            changeLocation( posComp, newX, newY, moveToContactH, moveToContactV );
            
            /*if ( newX != posComp.x ) {
                //Console.WriteLine("Changing x from " + posComp.x + " to " + newX);
                changeSingleAxisLocation( 'X', posComp, newX, moveToContact );
            }
            if ( newY != posComp.y ) {
                //Console.WriteLine("Changing y from " + posComp.y + " to " + newY);
                changeSingleAxisLocation( 'Y', posComp, newY, moveToContact );
            }*/
        }
        public void teleportToNoCollisionCheck( PositionComponent posComp, float newX, float newY ) {
            posComp.prevX = posComp.x;
            posComp.x = newX;
            posComp.prevY = posComp.y;
            posComp.y = newY;
            posComp.positionHasChanged = true;
        }

        public void changeX( PositionComponent posComp, float newVal, bool moveToContact ) {
            changePosition( posComp, newVal, posComp.y, moveToContact, false );
        }

        public void changeY( PositionComponent posComp, float newVal, bool moveToContact ) {
            changePosition( posComp, posComp.x, newVal, false, moveToContact );
        }

        /*
        public void changeSingleAxisLocation( Char axis, PositionComponent posComp, float newVal, bool moveToContact ) {
            bool movementBlocked = false;

            bool isX = true;
            float xVal = 0; //New X
            float yVal = 0; //New Y

            if ( axis == 'X' || axis == 'x' ) {
                xVal = newVal;
                yVal = posComp.y;
            } else if ( axis == 'Y' || axis == 'y' ) {
                isX = false;
                xVal = posComp.x;
                yVal = newVal;
            } else {
                Console.WriteLine( "Changing Unrecognized Axis in Movement System: " + axis );
                return;
            }

            //Check for collisions before it moves
            if ( posComp.myEntity.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) ) {

                VelocityComponent velComp = ( VelocityComponent )posComp.myEntity.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );

                //List of all collisions caused by potential move
                List<Entity> collisions = level.getCollisionSystem().checkForCollision( posComp.myEntity, xVal, yVal, posComp.width, posComp.height );

                //Perform all collisions
                if ( collisions.Count > 0 && !( level is CreationLevel ) ) {
                    foreach ( Entity e in collisions ) {
                        if ( e.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) ) {
                            //Handle the collision. handleCollision(...) will return true if it should stop the movement.
                            if ( colHandler.handleCollision( posComp.myEntity, e ) ) {

                                if ( isX ) velComp.x = 0;
                                else velComp.y = 0;

                                movementBlocked = true;

                                //Move to edge of object if already mostly at the edge... ONLY IF MOVE TO CONTACT IS TRUE
                                if ( moveToContact ) {
                                    PositionComponent otherPosComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );

                                    if ( isX ) {
                                        if ( posComp.x < ( otherPosComp.x ) )
                                            level.getMovementSystem().changePosition( posComp, getClosestPositionWithNoCollision( posComp, otherPosComp, true, true ),
                                                posComp.y, false );
                                        else
                                            level.getMovementSystem().changePosition( posComp, getClosestPositionWithNoCollision( posComp, otherPosComp, true, false ),
                                                posComp.y, false );

                                        //posComp.positionHasChanged = true;
                                    } else {
                                        if ( posComp.y < ( otherPosComp.y ) )
                                            level.getMovementSystem().changePosition( posComp, posComp.x, getClosestPositionWithNoCollision( posComp, otherPosComp, false, true ),
                                                false );
                                        else
                                            level.getMovementSystem().changePosition( posComp, posComp.x, getClosestPositionWithNoCollision( posComp, otherPosComp, false, false ),
                                                false );

                                        //posComp.positionHasChanged = true;
                                    }

                                }
                            }
                        }
                    }
                }
            }

            if ( isX && posComp.myEntity.hasComponent( GlobalVars.PUSHABLE_COMPONENT_NAME ) ) {
                PushableComponent pushComp = ( PushableComponent )posComp.myEntity.getComponent( GlobalVars.PUSHABLE_COMPONENT_NAME );
                if ( movementBlocked ) {
                    if ( xVal < posComp.x ) { //left
                        if ( pushComp.horizMovementStopped == 0 ) pushComp.horizMovementStopped = 1;
                        else if ( pushComp.horizMovementStopped == 2 ) pushComp.horizMovementStopped = 3;
                    }
                    if ( xVal > posComp.x ) { //right
                        if ( pushComp.horizMovementStopped == 0 ) pushComp.horizMovementStopped = 2;
                        else if ( pushComp.horizMovementStopped == 1 ) pushComp.horizMovementStopped = 3;
                    }
                } else {
                    if ( xVal < posComp.x ) {
                        if ( pushComp.horizMovementStopped == 3 ) pushComp.horizMovementStopped = 2;
                        else if ( pushComp.horizMovementStopped == 1 ) pushComp.horizMovementStopped = 0;
                    }
                    if ( xVal > posComp.x ) {
                        if ( pushComp.horizMovementStopped == 3 ) pushComp.horizMovementStopped = 1;
                        else if ( pushComp.horizMovementStopped == 2 ) pushComp.horizMovementStopped = 0;
                    }
                }
            }
            if ( !isX && posComp.myEntity.hasComponent( GlobalVars.PUSHABLE_COMPONENT_NAME ) ) {
                PushableComponent pushComp = ( PushableComponent )posComp.myEntity.getComponent( GlobalVars.PUSHABLE_COMPONENT_NAME );
                if ( movementBlocked ) {
                    if ( yVal < posComp.y ) { //up
                        if ( pushComp.vertMovementStopped == 0 ) pushComp.vertMovementStopped = 1;
                        else if ( pushComp.vertMovementStopped == 2 ) pushComp.vertMovementStopped = 3;
                    }
                    if ( yVal > posComp.y ) { //right
                        if ( pushComp.vertMovementStopped == 0 ) pushComp.vertMovementStopped = 2;
                        else if ( pushComp.vertMovementStopped == 1 ) pushComp.vertMovementStopped = 3;
                    }
                } else {
                    if ( yVal < posComp.y ) {
                        if ( pushComp.vertMovementStopped == 3 ) pushComp.vertMovementStopped = 2;
                        else if ( pushComp.vertMovementStopped == 1 ) pushComp.vertMovementStopped = 0;
                    }
                    if ( yVal > posComp.y ) {
                        if ( pushComp.vertMovementStopped == 3 ) pushComp.vertMovementStopped = 1;
                        else if ( pushComp.vertMovementStopped == 2 ) pushComp.vertMovementStopped = 0;
                    }
                }
            }

            //If movement isn't blocked - move and set position changed to true
            if ( !movementBlocked ) {
                if ( isX ) {
                    posComp.prevX = posComp.x;
                    posComp.x = newVal;
                } else {
                    posComp.prevY = posComp.y;
                    posComp.y = newVal;
                }
                posComp.positionHasChanged = true;
            }
        }
        */

        //-----------------------------------------CHANGE LOCATION METHODS--------------------------------------------

        //Walking gitters caused here because of moveToContact issues (i think)

        public void changeLocation( PositionComponent posComp, float newX, float newY, bool moveToContactH, bool moveToContactV ) {
            bool movementBlocked = false;

            //Check for collisions before it moves
            if ( posComp.myEntity.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) ) {
                movementBlocked = manageMovementColliderCheck( posComp, newX, newY, moveToContactH, moveToContactV );
            }

            if ( posComp.myEntity.hasComponent( GlobalVars.PUSHABLE_COMPONENT_NAME ) ) {
                handlePushWithMovement(posComp, newX, newY, movementBlocked);
            }

            //If movement isn't blocked - move and set position changed to true
            if ( !movementBlocked ) {

                if ( posComp.x != newX ) {
                    posComp.prevX = posComp.x;
                }

                if ( posComp.y != newY ) {
                    posComp.prevY = posComp.y;
                }

                posComp.x = newX;
                posComp.y = newY;

                posComp.positionHasChanged = true;
            }
        }

        //Handle any pushing that needs to be done when moved
        public void handlePushWithMovement(PositionComponent posComp, float newX, float newY, bool movementBlocked) {

            if ( !( posComp.myEntity.hasComponent( GlobalVars.PUSHABLE_COMPONENT_NAME ) ) ) return;
            
            if ( newX != posComp.x ) {
                PushableComponent pushComp = ( PushableComponent )posComp.myEntity.getComponent( GlobalVars.PUSHABLE_COMPONENT_NAME );
                if ( movementBlocked ) {
                    if ( newX < posComp.x ) { //left
                        if ( pushComp.horizMovementStopped == 0 ) pushComp.horizMovementStopped = 1;
                        else if ( pushComp.horizMovementStopped == 2 ) pushComp.horizMovementStopped = 3;
                    }
                    if ( newX > posComp.x ) { //right
                        if ( pushComp.horizMovementStopped == 0 ) pushComp.horizMovementStopped = 2;
                        else if ( pushComp.horizMovementStopped == 1 ) pushComp.horizMovementStopped = 3;
                    }
                } else {
                    if ( newX < posComp.x ) {
                        if ( pushComp.horizMovementStopped == 3 ) pushComp.horizMovementStopped = 2;
                        else if ( pushComp.horizMovementStopped == 1 ) pushComp.horizMovementStopped = 0;
                    }
                    if ( newX > posComp.x ) {
                        if ( pushComp.horizMovementStopped == 3 ) pushComp.horizMovementStopped = 1;
                        else if ( pushComp.horizMovementStopped == 2 ) pushComp.horizMovementStopped = 0;
                    }
                }
            }
            if ( newY != posComp.y ) {
                PushableComponent pushComp = ( PushableComponent )posComp.myEntity.getComponent( GlobalVars.PUSHABLE_COMPONENT_NAME );
                if ( movementBlocked ) {
                    if ( newY < posComp.y ) { //up
                        if ( pushComp.vertMovementStopped == 0 ) pushComp.vertMovementStopped = 1;
                        else if ( pushComp.vertMovementStopped == 2 ) pushComp.vertMovementStopped = 3;
                    }
                    if ( newY > posComp.y ) { //right
                        if ( pushComp.vertMovementStopped == 0 ) pushComp.vertMovementStopped = 2;
                        else if ( pushComp.vertMovementStopped == 1 ) pushComp.vertMovementStopped = 3;
                    }
                } else {
                    if ( newY < posComp.y ) {
                        if ( pushComp.vertMovementStopped == 3 ) pushComp.vertMovementStopped = 2;
                        else if ( pushComp.vertMovementStopped == 1 ) pushComp.vertMovementStopped = 0;
                    }
                    if ( newY > posComp.y ) {
                        if ( pushComp.vertMovementStopped == 3 ) pushComp.vertMovementStopped = 1;
                        else if ( pushComp.vertMovementStopped == 2 ) pushComp.vertMovementStopped = 0;
                    }
                }
            }
        }

        //Returns whether or not movement has been blocked by a collision
        public bool manageMovementColliderCheck(PositionComponent posComp, float newX, float newY, bool moveToContactH, bool moveToContactV) {

            VelocityComponent velComp = ( VelocityComponent )posComp.myEntity.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );

            //List of all collisions caused by potential move
            List<Entity> collisions = level.getCollisionSystem().checkForCollision( posComp.myEntity, newX, newY, posComp.width, posComp.height );

            bool movementBlocked = false;

            //Perform all collisions
            if ( collisions.Count > 0 && !( level is CreationLevel ) ) {

                foreach ( Entity e in collisions ) {
                    if  ( ! ( e.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) ) ) continue;

                    //Handle the collision. handleCollision(...) will return true if it should stop the movement.
                    if ( colHandler.handleCollision( posComp.myEntity, e ) ) {
                        movementBlocked = true;
                        stopMovementAfterCollision(e, posComp, velComp, newX, newY, moveToContactH, moveToContactV);
                    }
                }
            }
            return movementBlocked;
        }

        //Stop an entities movement after a collision
        public void stopMovementAfterCollision(Entity collisionEntity, PositionComponent posComp, VelocityComponent velComp, float newX, float newY, bool moveToContactH, bool moveToContactV) {

            if ( newX != posComp.x ) velComp.x = 0;
            if ( newY != posComp.y ) velComp.y = 0;

            //Move to edge of object if already mostly at the edge... ONLY IF MOVE TO CONTACT IS TRUE
            if ( moveToContactH || moveToContactV ) {
                PositionComponent otherPosComp = ( PositionComponent )collisionEntity.getComponent( GlobalVars.POSITION_COMPONENT_NAME );

                float allowedDistanceChange = 1.0f;

                if (moveToContactH && (Math.Abs(newX - posComp.x) > allowedDistanceChange) ) {
                    
                    if ( posComp.x < ( otherPosComp.x ) )
                        level.getMovementSystem().changePosition( posComp, getClosestPositionWithNoCollision( posComp, otherPosComp, true, true ), posComp.y, false, false );
                    else
                        level.getMovementSystem().changePosition( posComp, getClosestPositionWithNoCollision( posComp, otherPosComp, true, false ), posComp.y, false, false );
                } if ( moveToContactV && (Math.Abs(newY - posComp.y) > allowedDistanceChange) ) {
                    if ( posComp.y < ( otherPosComp.y ) )
                        level.getMovementSystem().changePosition( posComp, posComp.x, getClosestPositionWithNoCollision( posComp, otherPosComp, false, true ), false, false );
                    else
                        level.getMovementSystem().changePosition( posComp, posComp.x, getClosestPositionWithNoCollision( posComp, otherPosComp, false, false ), false, false );
                }
            }

        }

        //Returns the closest location (x or y) to entity 2 that entity 1 can be before a collision occurs
        public float getClosestPositionWithNoCollision( PositionComponent posComp1, PositionComponent posComp2, bool xDir, bool leftOrUp ) {
            float retPos = 0.0f;

            ColliderComponent colComp1 = ( ColliderComponent )posComp1.myEntity.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
            ColliderComponent colComp2 = ( ColliderComponent )posComp2.myEntity.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );

            //Separated out for easy changing.
            float e1X = posComp1.x;
            float e1Y = posComp1.y;
            float e2X = posComp2.x;
            float e2Y = posComp2.y;
            float e1Width = colComp1.width;
            float e1Height = colComp1.height;
            float e2Width = colComp2.width;
            float e2Height = colComp2.height;

            //Center width/height values
            if ( e1Width != posComp1.width ) {
                float diff = ( posComp1.width - e1Width );
                e1X += diff / 2;
            }
            if ( e2Width != posComp2.width ) {
                float diff = ( posComp2.width - e2Width );
                e2X += diff / 2;
            }
            if ( e1Height != posComp1.height ) {
                float diff = ( posComp1.height - e1Height );
                e1Y += diff / 2;
            }
            if ( e2Height != posComp2.height ) {
                float diff = ( posComp2.height - e2Height );
                e2Y += diff / 2;
            }

            if ( xDir && leftOrUp ) retPos = e2X - e2Width / 2 - e1Width / 2;
            else if ( xDir && !leftOrUp ) retPos = e2X + e2Width / 2 + e1Width / 2;
            else if ( !xDir && leftOrUp ) retPos = e2Y - e2Height / 2 - e1Height / 2;
            else if ( !xDir && !leftOrUp ) retPos = e2Y + e2Height / 2 + e1Height / 2;

            if ( !GlobalVars.preciseCollisionChecking ) {
                return retPos;
            }

            float newX = e2X;
            float newY = e2Y;

            if ( xDir ) newX = retPos;
            else newY = retPos;

            List<Entity> cols = level.getCollisionSystem().checkForCollision( posComp1.myEntity, newX, newY, e1Width, e1Height );
            while ( cols.Count <= 0 ) {
                if ( leftOrUp ) retPos += 1;
                else retPos -= 1;
                if ( xDir ) newX = retPos;
                else newY = retPos;
                cols = level.getCollisionSystem().checkForCollision( posComp1.myEntity, newX, newY, e1Width, e1Height );
            }

            return retPos;
        }


        //------------------------------------------------------------------------------------------------------------


        //Size
        public void changeSize( PositionComponent posComp, float newW, float newH ) {
            posComp.prevW = posComp.width;
            posComp.width = newW;

            posComp.prevH = posComp.height;
            posComp.height = newH;

            posComp.positionHasChanged = true;
        }
        public void changeWidth( PositionComponent posComp, float newW ) {
            changeSize( posComp, newW, posComp.height );
        }
        public void changeHeight( PositionComponent posComp, float newH ) {
            changeSize( posComp, posComp.width, newH );
        }
        //--------------------------------------------------------------------------------------------------------------------------

    }
}