using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Systems {
    [Serializable()]
    public class MovingPlatformSystem : GameSystem {
        //All systems MUST have an List of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        List<string> requiredComponents = new List<string>();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        List<Type> stoppingEntities = new List<Type>() {
            typeof(Entities.BasicGround)
        };

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public MovingPlatformSystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.VELOCITY_COMPONENT_NAME ); //Velocity
            requiredComponents.Add( GlobalVars.MOVING_PLATFORM_COMPONENT_NAME ); //Moving Platform


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

        //You must have an Update.
        //Always read in deltaTime, and only deltaTime (it's the time that's passed since the last frame)
        //Use deltaTime for things like changing velocity or changing position from velocity
        //This is where you do anything that you want to happen every frame.
        //There is a chance that your system won't need to do anything in update. Still have it.
        public override void Update( float deltaTime ) {
            foreach ( Entity e in getApplicableEntities() ) {
                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                ColliderComponent colComp = (ColliderComponent) e.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);
                VelocityComponent velComp = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                MovingPlatformComponent movPlatComp = ( MovingPlatformComponent )e.getComponent( GlobalVars.MOVING_PLATFORM_COMPONENT_NAME );

                float mySpeed = velComp.x;

                if ( movPlatComp.vertical ) {
                    mySpeed = velComp.y;
                }

                //If it's been stopped for more than one frame, try changing the direction and see if it can move that way instead.
                if ( mySpeed == 0 ) {
                    float newSpeed = GlobalVars.MOVING_PLATFORM_SPEED;

                    if ( !movPlatComp.wasStoppedLastFrame )
                        newSpeed = GlobalVars.MOVING_PLATFORM_SPEED;
                    else
                        newSpeed = -GlobalVars.MOVING_PLATFORM_SPEED;

                    if ( movPlatComp.vertical ) {
                        velComp.y = newSpeed;
                    } else {
                        velComp.x = newSpeed;
                    }

                    movPlatComp.wasStoppedLastFrame = true;
                } else if ( movPlatComp.wasStoppedLastFrame ) {
                    movPlatComp.wasStoppedLastFrame = false;
                }

                float upperLeftX = posComp.x - colComp.width/2;
                float upperRightX = posComp.x + colComp.width/2;
                float upperY = posComp.y - colComp.height/2 - 3;

                System.Drawing.PointF leftPoint = new System.Drawing.PointF(upperLeftX, upperY);
                System.Drawing.PointF rightPoint = new System.Drawing.PointF(upperRightX, upperY);
                List<Entity> aboveEnts = level.getCollisionSystem().findObjectsBetweenPoints( leftPoint, rightPoint );

                foreach(Entity otherEnt in aboveEnts) {

                    if ( !stoppingEntities.Contains(  otherEnt.GetType() ) && otherEnt.hasComponent( GlobalVars.VELOCITY_COMPONENT_NAME ) ) {
                        PositionComponent otherPos = (PositionComponent) otherEnt.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                        VelocityComponent otherVel = ( VelocityComponent )otherEnt.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                        ColliderComponent otherCol = ( ColliderComponent )otherEnt.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                        if ( movPlatComp.vertical ) {
                            if ( ( velComp.y <= 0 && otherVel.y >= velComp.y ) || ( velComp.y >= 0 && otherVel.y <= velComp.y ) ) {
                                otherVel.y = velComp.y;
                                if ( otherEnt.hasComponent( GlobalVars.GRAVITY_COMPONENT_NAME ) ) {
                                    GravityComponent otherGrav = ( GravityComponent )otherEnt.getComponent( GlobalVars.GRAVITY_COMPONENT_NAME );
                                    if(otherVel.y >= 0)
                                        otherVel.y -= otherGrav.y * deltaTime;
                                }
                                level.getMovementSystem().changePosition( otherPos, otherPos.x, posComp.y - colComp.height / 2 - otherCol.height / 2 - 3, false, false );
                            }
                        } else {
                            if ( ( velComp.x <= 0 && otherVel.x >= velComp.x ) || ( velComp.x >= 0 && otherVel.x <= velComp.y ) ) {
                                otherVel.x = velComp.x;
                                if ( otherEnt.hasComponent( GlobalVars.GRAVITY_COMPONENT_NAME ) ) {
                                    GravityComponent otherGrav = ( GravityComponent )otherEnt.getComponent( GlobalVars.GRAVITY_COMPONENT_NAME );
                                    
                                    otherVel.x -= otherGrav.x * deltaTime;
                                }

                                if ( otherEnt is Entities.Player ) {
                                    level.getMovementSystem().changePosition( otherPos, otherPos.x + velComp.x * deltaTime, posComp.y - colComp.height / 2 - otherCol.height / 2 - 3, false, false );
                                }  else {
                                    level.getMovementSystem().changePosition( otherPos, otherPos.x, posComp.y - colComp.height / 2 - otherCol.height / 2 - 3, false, false );
                                }
                            }
                        }
                    }
                }

            }

        }
        //----------------------------------------------------------------------------------------------

    }
}
