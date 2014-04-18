using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame.Systems {

    /*
     * This system is used to handle player specific movement.
     */
    [Serializable()]
    public class PlayerMovementSystem : GameSystem {

        Level level;
        List<string> requiredComponents = new List<string>();
        public float playerHorizSlowSpeed = 50.0f;

        public PlayerMovementSystem( Level activeLevel ) {
            //Required Components
            requiredComponents.Add( GlobalVars.PLAYER_COMPONENT_NAME ); //Player Component
            requiredComponents.Add( GlobalVars.PLAYER_INPUT_COMPONENT_NAME ); //Player Input Component
            requiredComponents.Add( GlobalVars.POSITION_COMPONENT_NAME ); //Position
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME ); //Velocity

            //Set the level
            level = activeLevel;

        }

        //-------------------------------- Overrides ----------------------------------
        public override Level GetActiveLevel() {
            return level;
        }
        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }
        public override void Update( float deltaTime ) {


            foreach ( Entity e in getApplicableEntities() ) {
                PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                PlayerInputComponent pelInComp = ( PlayerInputComponent )e.getComponent( GlobalVars.PLAYER_INPUT_COMPONENT_NAME );
                AnimationComponent animComp = ( AnimationComponent )e.getComponent( GlobalVars.ANIMATION_COMPONENT_NAME );
                checkForInput( posComp, velComp, pelInComp, animComp );

                //Reset passedAirJumps if needed
                if ( pelInComp.passedAirjumps != 0 && level.getCollisionSystem().findObjectsBetweenPoints(
                    posComp.x - posComp.width / 2, posComp.y + ( posComp.height / 2 ) + 1, posComp.x + posComp.width / 2, posComp.y +
                    ( posComp.height / 2 ) + 1 ).Count > 0 ) {
                    pelInComp.passedAirjumps = 0;
                }

                //If there's a key down and the player isn't moving horizontally, check to make sure there's a collision
                restartHorizontalMovementIfNoBlock( velComp, posComp, pelInComp, animComp );
                if ( level != null && level.getPlayer() != null ) {
                    if ( level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed || level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed ) {
                        level.getPlayer().startAnimation();
                    } else {
                        if ( level.getPlayer() != null )
                            level.getPlayer().stopAnimation();
                    }
                }

                //Slow horizontal if no left/right key down
                if ( velComp.x != 0 && !level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed && !level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed ) {
                    
                    if ( velComp.x < 0 ) {
                        velComp.x += playerHorizSlowSpeed;
                        if ( velComp.x > 0 )
                            velComp.x = 0;
                    } else {
                        velComp.x -= playerHorizSlowSpeed;
                        if ( velComp.x < 0 )
                            velComp.x = 0;
                    }
                    
                }
            }

        }
        //-----------------------------------------------------------------------------

        //------------------------------- Helper Methods -------------------------------
        /*
         * This method checks to see if a left/right key is down and there's no movement.
         * If that's the case - it makes sure there's something blocking the movement, or it restarts it.
         * 
         * This is used for situations like when the player is running against a wall and jumps, if the jump
         * clears the wall then the horizontal movement should restart without the need for a second key press.
         */
        public void restartHorizontalMovementIfNoBlock( VelocityComponent velComp, PositionComponent posComp, PlayerInputComponent pelInComp, AnimationComponent animComp ) {
            if ( Math.Abs( velComp.x ) < Math.Abs( pelInComp.playerHorizMoveSpeed ) ) {

                float allowedOverlap = 3.0f;
                float extraHDistCheck = GlobalVars.MIN_TILE_SIZE / 2;
                float upperY = ( posComp.y - posComp.height / 2 );
                float lowerY = ( posComp.y + posComp.height / 2 - allowedOverlap );

                if ( level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed && velComp.x == 0 ) {

                    float rightX = ( posComp.x + posComp.width / 2 + extraHDistCheck );
                    
                    bool tmpPrecice = GlobalVars.preciseCollisionChecking;
                    GlobalVars.preciseCollisionChecking = false; //turn off precise collision detection to prevent jitters.

                    List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( rightX, upperY, rightX, lowerY );
                    if ( cols.Count <= 0 ) {
                        beginMoveRight( posComp, velComp, pelInComp, animComp );
                    }

                    GlobalVars.preciseCollisionChecking = tmpPrecice; //put collision detection back to its normal setting

                }
                if ( level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed && velComp.x == 0) {

                    float leftX = ( posComp.x - posComp.width / 2 - extraHDistCheck );
                    bool tmpPrecice = GlobalVars.preciseCollisionChecking;
                    GlobalVars.preciseCollisionChecking = false; //turn off precise collision detection to prevent jitters.
                    
                    List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( leftX, upperY, leftX, lowerY );
                    
                    if ( cols.Count <= 0 ) {
                        beginMoveLeft( posComp, velComp, pelInComp, animComp );
                    }

                    GlobalVars.preciseCollisionChecking = tmpPrecice; //Put it back on asap!
                }

            }
        }

        //----------------------------------- Input ----------------------------------- 
        public void checkForInput( PositionComponent posComp, VelocityComponent velComp, PlayerInputComponent pelInComp, AnimationComponent animComp ) {
            if ( level.getInputSystem().myKeys[GlobalVars.KEY_JUMP].down ) {
                playerJump( posComp, velComp, pelInComp );
            }
            if ( level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].down ) {
                beginMoveLeft( posComp, velComp, pelInComp, animComp );
            }
            if ( level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].down ) {
                beginMoveRight( posComp, velComp, pelInComp, animComp );
            }
            if ( level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].up ) {
                endRightHorizontalMove( posComp, velComp, animComp );
            }
            if ( level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].up ) {
                endLeftHorizontalMove( posComp, velComp, animComp );
            }
        }
        //--------------------------------------------------------------------------------



        //------------------------------------- Actions ----------------------------------
        public void playerJump( PositionComponent posComp, VelocityComponent velComp, PlayerInputComponent pelInComp ) {
            //If it's landed on something, jump
            float checkY = posComp.y + ( posComp.height / 2 ) + GlobalVars.MIN_TILE_SIZE / 2;
            if ( level.getCollisionSystem().findObjectsBetweenPoints( posComp.x - posComp.width / 2, checkY, posComp.x + posComp.width / 2, checkY ).Count > 0 ) {
                velComp.setVelocity( velComp.x, pelInComp.jumpStrength );
                pelInComp.passedAirjumps = 0;
            } else {
                if ( pelInComp.passedAirjumps < GlobalVars.numAirJumps ) {
                    velComp.setVelocity( velComp.x, pelInComp.jumpStrength );
                    pelInComp.passedAirjumps++;
                }
            }
        }
        public void beginMoveLeft( PositionComponent posComp, VelocityComponent velComp, PlayerInputComponent pelInComp, AnimationComponent animComp ) {
            if ( pelInComp != null && pelInComp.player != null ) {
                velComp.setVelocity( -pelInComp.playerHorizMoveSpeed, velComp.y );
                if ( !pelInComp.player.isLookingLeft() )
                    pelInComp.player.faceLeft();
                level.getPlayer().startAnimation();
            }
        }
        public void beginMoveRight( PositionComponent posComp, VelocityComponent velComp, PlayerInputComponent pelInComp, AnimationComponent animComp ) {
            if ( pelInComp != null && pelInComp.player != null ) {
                velComp.setVelocity( pelInComp.playerHorizMoveSpeed, velComp.y );
                if ( !pelInComp.player.isLookingRight() )
                    pelInComp.player.faceRight();
                level.getPlayer().startAnimation();
            }
        }
        public void endLeftHorizontalMove( PositionComponent posComp, VelocityComponent velComp, AnimationComponent animComp ) {
            if ( level.getPlayer() == null ) return;
            if ( velComp.x < 0 ) velComp.setVelocity( 0, velComp.y );
            level.getPlayer().stopAnimation();
        }
        public void endRightHorizontalMove( PositionComponent posComp, VelocityComponent velComp, AnimationComponent animComp ) {
            if ( level.getPlayer() == null ) return;
            if ( velComp.x > 0 ) velComp.setVelocity( 0, velComp.y );
            level.getPlayer().stopAnimation();
        }
        public void endUpperMove( PositionComponent posComp, VelocityComponent velComp ) {
            if ( velComp.y < 0 ) velComp.setVelocity( velComp.x, 0 );
        }
        public void endLowerMove( PositionComponent posComp, VelocityComponent velComp ) {
            if ( velComp.y > 0 ) velComp.setVelocity( velComp.x, 0 );
        }
        //--------------------------------------------------------------------------------
    }
}
