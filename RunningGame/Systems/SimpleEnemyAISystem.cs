using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame.Systems {
    [Serializable()]
    public class SimpleEnemyAISystem : GameSystem {//All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public SimpleEnemyAISystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.POSITION_COMPONENT_NAME ); //Position
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME ); //Velocity
            requiredComponents.Add( GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME );//Simple Enemy


            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }

        //Must have this. Same for all Systems.
        public override Level GetActiveLevel() {
            return level;
        }

        public override void Update( float deltaTime ) {
            foreach ( Entity e in getApplicableEntities() ) {
                //Grab needed components
                SimpleEnemyComponent simpEnemyComp = ( SimpleEnemyComponent )e.getComponent( GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME );
                VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );

                if ( simpEnemyComp.hasRunOnce && !simpEnemyComp.hasLandedOnce && velComp.y <= 0 ) {
                    simpEnemyComp.hasLandedOnce = true;
                    //e.updateOutOfView = false;
                }

                if ( velComp.x < 0 ) simpEnemyComp.movingLeft = true;
                else if ( velComp.x > 0 ) simpEnemyComp.movingLeft = false;
                else if ( velComp.x == 0 && simpEnemyComp.hasLandedOnce ) //If it's been stopped for more than one frame, try changing the direction and see if it can move that way instead.
                {
                    //SimpleEnemyComponent simpEnemyComp = (SimpleEnemyComponent)e.getComponent(GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME);

                    /*
                    if (!simpEnemyComp.wasStoppedLastFrame)
                        velComp.x = simpEnemyComp.mySpeed;
                    else
                        velComp.x = -simpEnemyComp.mySpeed;
                    */

                    float newVel = simpEnemyComp.mySpeed;
                    if ( !simpEnemyComp.movingLeft ) newVel *= -1;

                    if ( simpEnemyComp.wasStoppedLastFrame )
                        velComp.x = newVel;


                    simpEnemyComp.wasStoppedLastFrame = true;
                } else if ( simpEnemyComp.wasStoppedLastFrame ) {
                    simpEnemyComp.wasStoppedLastFrame = false;
                }

                //Change position if it's about to fall off a cliff, and checkCliff is true.
                if ( simpEnemyComp.hasLandedOnce && simpEnemyComp.checkCliff ) {
                    PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );

                    List<Entity> collisionsAheadAndBelow = level.getCollisionSystem().findObjectAtPoint( posComp.x + getSign( velComp.x ) * ( posComp.width / 2 + 1 ), posComp.y + posComp.height / 2 + 1 );

                    if ( collisionsAheadAndBelow.Count <= 0 ) {
                        velComp.x = -velComp.x;
                    }
                }

                //Face the right way
                if ( e is SimpleEnemyEntity ) {
                    SimpleEnemyEntity enemy = ( SimpleEnemyEntity )e;
                    if ( velComp.x > 0 && !enemy.isLookingRight() ) {
                        enemy.faceRight();
                    }
                    if ( velComp.x < 0 && !enemy.isLookingLeft() ) {
                        enemy.faceLeft();
                    }
                } else if ( e is FlyingEnemyEntity ) {
                    FlyingEnemyEntity enemy = ( FlyingEnemyEntity )e;
                    if ( velComp.x > 0 && !enemy.isLookingRight() ) {
                        enemy.faceRight();
                    }
                    if ( velComp.x < 0 && !enemy.isLookingLeft() ) {
                        enemy.faceLeft();
                    }
                }
                simpEnemyComp.hasRunOnce = true;
            }
        }

        //----------------------------------------------------------------------------------------
        public float getSign( float num ) {
            return ( num / Math.Abs( num ) );
        }
    }
}
