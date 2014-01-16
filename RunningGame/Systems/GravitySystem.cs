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
                PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );

                float sideBuffer = -1;
                float floorBuffer = 1; //Distance it checks below object for the ground

                float leftX = ( posComp.x - posComp.width / 2 - sideBuffer );
                float rightX = ( posComp.x + posComp.width / 2 + sideBuffer );
                float lowerY = ( posComp.y + posComp.height / 2 + floorBuffer );
                //Console.WriteLine("Lower y: " + lowerY);
                //List<Entity> cols = level.getCollisionSystem().checkForCollision(e, posComp.x, lowerY, posComp.width, posComp.height);
                List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( leftX, lowerY, rightX, lowerY );

                bool shouldApplyGravity = true; //False if there's a solid object below

                foreach ( Entity ent in cols ) {
                    ColliderComponent collider = ( ColliderComponent )ent.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                    PositionComponent posComp2 = ( PositionComponent )ent.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                    //If the object is below the player, and it's solid, don't apply gravity.
                    if ( ( posComp.y + ( posComp.height / 2 ) ) <= ( posComp2.y - ( posComp2.height / 2 ) ) && collider.colliderType == GlobalVars.BASIC_SOLID_COLLIDER_TYPE ) {
                        float newY = posComp2.y - posComp2.height / 2 - posComp.height / 2;

                        if ( moveToContactWhenTouchGround && Math.Abs( posComp.y - newY ) > 1 ) {
                            level.getMovementSystem().changePosition( posComp, posComp.x, newY, true );
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
