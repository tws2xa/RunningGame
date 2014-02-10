using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame {
    //Applies gravity to all objects with gravity, position, and velocity components
    [Serializable()]
    public class GravitySystem : GameSystem {
        List<string> requiredComponents = new List<string>();
        Level level;

        public bool moveToContactWhenTouchGround = false;

        public GravitySystem( Level activeLevel ) {
            //Required Components
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME ); //Velocity
            requiredComponents.Add( GlobalVars.POSITION_COMPONENT_NAME ); //Position
            requiredComponents.Add( GlobalVars.GRAVITY_COMPONENT_NAME ); //Gravity

            //Set the level
            level = activeLevel;
        }

        public override Level GetActiveLevel() {
            return level;
        }

        //Run once each frame deltaTime is the amount of seconds since the last frame
        public override void Update( float deltaTime ) {
            foreach ( Entity e in getApplicableEntities() ) {

                //Don't apply gravity if the object is on top of something
                PositionComponent posComp1 = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                ColliderComponent colComp1 = ( ColliderComponent )e.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );

                float sideBuffer = -1;
                float floorBuffer = 1; //Distance it checks below object for the ground

                float e1X = posComp1.x;
                float e1Y = posComp1.y;
                float e1Width = posComp1.width;
                float e1Height = posComp1.height;

                //If it has a collider, use its Width and Height instead.
                if ( colComp1 != null ) {
                    e1Width = colComp1.width;
                    e1Height = colComp1.height;
                }

                if ( e1Width != posComp1.width ) {
                    float diff = ( posComp1.width - e1Width );
                    e1X += diff / 2;
                }
                if ( e1Height != posComp1.height ) {
                    float diff = ( posComp1.height - e1Height );
                    e1Y += diff / 2;
                }

                float leftX = ( e1X - e1Width / 2 - sideBuffer );
                float rightX = ( e1X + e1Width / 2 + sideBuffer );
                float lowerY = ( e1Y + e1Height / 2 + floorBuffer );
                //Console.WriteLine("Lower y: " + lowerY);
                //List<Entity> cols = level.getCollisionSystem().checkForCollision(e, posComp.x, lowerY, posComp.width, posComp.height);
                List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( leftX, lowerY, rightX, lowerY );

                bool shouldApplyGravity = true; //False if there's a solid object below

                foreach ( Entity ent in cols ) {
                    ColliderComponent colComp2 = ( ColliderComponent )ent.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                    PositionComponent posComp2 = ( PositionComponent )ent.getComponent( GlobalVars.POSITION_COMPONENT_NAME );

                    //Separated out for easy changing.
                    float e2X = posComp2.x;
                    float e2Y = posComp2.y;
                    float e2Width = colComp2.width;
                    float e2Height = colComp2.height;

                    //Center width/height values
                    if ( e2Width != posComp2.width ) {
                        float diff = ( posComp2.width - e2Width );
                        e2X += diff / 2;
                    }
                    if ( e2Height != posComp2.height ) {
                        float diff = ( posComp2.height - e2Height );
                        e2Y += diff / 2;
                    }
                    
                    //If the object is below the player, and it's solid, don't apply gravity.
                    if ( ( e1Y + ( e1Height / 2 ) ) <= ( e2Y - ( e2Height / 2 ) ) && colComp2.colliderType == GlobalVars.BASIC_SOLID_COLLIDER_TYPE ) {
                        float newY = posComp1.y - posComp2.height / 2 - posComp1.height / 2;

                        if ( moveToContactWhenTouchGround && Math.Abs( e1Y - newY ) > 0.1 ) {
                            level.getMovementSystem().changePosition( posComp1, e1X, newY, false, true);
                        }

                        shouldApplyGravity = false;
                        break;
                    }
                }
                if ( shouldApplyGravity ) {
                    //Pull out all required components
                    VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                    GravityComponent gravComp = ( GravityComponent )e.getComponent( GlobalVars.GRAVITY_COMPONENT_NAME );

                    //Add the x and y gravity to their respective velocity components
                    velComp.incVelocity( gravComp.x * deltaTime, gravComp.y * deltaTime );

                    //Console.WriteLine("Gravity for " + e + " - X Vel : " + velComp.x + " - Y Vel - " + velComp.y);
                }
            }

        }

        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }


    }
}
