using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Systems;

namespace RunningGame.Systems {
    [Serializable()]
    public class ScreenEdgeSystem : GameSystem {

        List<string> requiredComponents = new List<string>();
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public ScreenEdgeSystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.POSITION_COMPONENT_NAME ); //Position
            requiredComponents.Add( GlobalVars.SCREEN_EDGE_COMPONENT_NAME ); //Screen Wrap
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME ); //Velocity


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

        //Check if it's out of the screen in a wrap direction, if so, wrap.
        public override void Update( float deltaTime ) {

            foreach ( Entity e in getApplicableEntities() ) {

                PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                ScreenEdgeComponent scrEdgComp = ( ScreenEdgeComponent )e.getComponent( GlobalVars.SCREEN_EDGE_COMPONENT_NAME );


                //Intersect sides of screen check
                if ( scrEdgComp.right == 1 && posComp.x + posComp.width / 2 >= level.levelWidth && velComp.x >= 0 ) stopRight( posComp, velComp );
                if ( scrEdgComp.left == 1 && posComp.x - posComp.width / 2 <= 0 && velComp.x <= 0 ) stopLeft( posComp, velComp );
                if ( scrEdgComp.down == 1 && posComp.y + posComp.height / 2 >= level.levelHeight && velComp.y >= 0 ) stopDown( posComp, velComp );
                if ( scrEdgComp.up == 1 && posComp.y - posComp.height / 2 <= 0 && velComp.y <= 0 ) stopUp( posComp, velComp );


                float buffer = 3.0f;

                //Off sides of screen check
                if ( posComp.x > ( level.levelWidth + posComp.width / 2 - buffer ) && velComp.x > 0 ) {
                    if ( scrEdgComp.right == 2 ) {
                        wrapRight( posComp );
                    } else if ( scrEdgComp.right == 3 ) {
                        level.removeEntity( posComp.myEntity );
                    } else if ( scrEdgComp.right == 4 ) {
                        level.beginEndLevel();
                        level.removeEntity( posComp.myEntity );
                    } else if ( scrEdgComp.right == 5 ) {
                        if ( !e.hasComponent( GlobalVars.HEALTH_COMPONENT_NAME ) ) return;
                        HealthComponent healthComp = ( HealthComponent )e.getComponent( GlobalVars.HEALTH_COMPONENT_NAME );
                        healthComp.subtractFromHealth( healthComp.maxHealth + 1 );
                    }
                }
                if ( posComp.x < ( -posComp.width / 2 + buffer ) && velComp.x < 0 ) {
                    if ( scrEdgComp.left == 2 )
                        wrapLeft( posComp );
                    else if ( scrEdgComp.left == 3 ) {
                        level.removeEntity( posComp.myEntity );
                    } else if ( scrEdgComp.left == 4 ) {
                        level.beginEndLevel();
                        level.removeEntity( posComp.myEntity );
                    } else if ( scrEdgComp.left == 5 ) {
                        if ( !e.hasComponent( GlobalVars.HEALTH_COMPONENT_NAME ) ) return;
                        HealthComponent healthComp = ( HealthComponent )e.getComponent( GlobalVars.HEALTH_COMPONENT_NAME );
                        healthComp.subtractFromHealth( healthComp.maxHealth );
                    }
                }
                if ( posComp.y > ( level.levelHeight + posComp.height / 2 - buffer ) && velComp.y > 0 ) {
                    if ( scrEdgComp.down == 2 )
                        wrapDown( posComp );
                    else if ( scrEdgComp.down == 3 ) {
                        level.removeEntity( posComp.myEntity );
                    } else if ( scrEdgComp.down == 4 ) {
                        level.beginEndLevel();
                        level.removeEntity( posComp.myEntity );
                    } else if ( scrEdgComp.down == 5 ) {
                        if ( !e.hasComponent( GlobalVars.HEALTH_COMPONENT_NAME ) ) return;
                        HealthComponent healthComp = ( HealthComponent )e.getComponent( GlobalVars.HEALTH_COMPONENT_NAME );
                        healthComp.subtractFromHealth( healthComp.maxHealth );
                    }
                }
                if ( posComp.y < ( -posComp.height / 2 + buffer ) && velComp.y < 0 ) {
                    if ( scrEdgComp.up == 2 )
                        wrapUp( posComp );
                    else if ( scrEdgComp.down == 3 ) {
                        level.removeEntity( posComp.myEntity );
                    } else if ( scrEdgComp.up == 4 ) {
                        level.beginEndLevel();
                        level.removeEntity( posComp.myEntity );
                    } else if ( scrEdgComp.up == 5 ) {
                        if ( !e.hasComponent( GlobalVars.HEALTH_COMPONENT_NAME ) ) return;
                        HealthComponent healthComp = ( HealthComponent )e.getComponent( GlobalVars.HEALTH_COMPONENT_NAME );
                        healthComp.subtractFromHealth( healthComp.maxHealth );
                    }
                }
            }

        }

        //------------------------------------------------------------------------------------------

        //WRAP

        public void wrapLeft( PositionComponent posComp ) {
            level.getMovementSystem().changeX( posComp, level.levelWidth + posComp.width, true ); //Screen Wrap
        }
        public void wrapRight( PositionComponent posComp ) {
            level.getMovementSystem().changeX( posComp, -posComp.width, true ); //Screen Wrap
        }
        public void wrapUp( PositionComponent posComp ) {
            level.getMovementSystem().changeY( posComp, level.levelHeight + posComp.height, true ); //Screen Wrap
        }
        public void wrapDown( PositionComponent posComp ) {
            level.getMovementSystem().changeY( posComp, -posComp.height, true ); //Screen Wrap
        }

        //STOP

        public void stopLeft( PositionComponent posComp, VelocityComponent velComp ) {
            level.getMovementSystem().changeX( posComp, posComp.width / 2, true );
            if ( velComp.x < 0 ) velComp.setVelocity( 0, velComp.y );
        }
        public void stopRight( PositionComponent posComp, VelocityComponent velComp ) {
            level.getMovementSystem().changeX( posComp, level.levelWidth - posComp.width / 2, true );
            if ( velComp.x > 0 ) velComp.setVelocity( 0, velComp.y );
        }
        public void stopUp( PositionComponent posComp, VelocityComponent velComp ) {
            level.getMovementSystem().changeY( posComp, posComp.height / 2, true );
            if ( velComp.y < 0 ) velComp.setVelocity( velComp.x, 0 );
        }
        public void stopDown( PositionComponent posComp, VelocityComponent velComp ) {
            level.getMovementSystem().changeY( posComp, level.levelHeight - posComp.height / 2, true );
            if ( velComp.y > 0 ) velComp.setVelocity( velComp.x, 0 );
        }
    }
}
