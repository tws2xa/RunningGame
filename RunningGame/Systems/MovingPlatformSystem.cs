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
                PositionComponent platPos = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                ColliderComponent platCol = (ColliderComponent) e.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);
                VelocityComponent platVel = ( VelocityComponent )e.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                MovingPlatformComponent movPlatComp = ( MovingPlatformComponent )e.getComponent( GlobalVars.MOVING_PLATFORM_COMPONENT_NAME );

                float mySpeed = platVel.x;

                if ( movPlatComp.vertical ) {
                    mySpeed = platVel.y;
                }

                //If it's been stopped for more than one frame, try changing the direction and see if it can move that way instead.
                if ( mySpeed == 0 ) {
                    float newSpeed = GlobalVars.MOVING_PLATFORM_SPEED;

                    if ( !movPlatComp.wasStoppedLastFrame )
                        newSpeed = GlobalVars.MOVING_PLATFORM_SPEED;
                    else
                        newSpeed = -GlobalVars.MOVING_PLATFORM_SPEED;

                    if ( movPlatComp.vertical ) {
                        platVel.y = newSpeed;
                    } else {
                        platVel.x = newSpeed;
                    }

                    movPlatComp.wasStoppedLastFrame = true;
                } else if ( movPlatComp.wasStoppedLastFrame ) {
                    movPlatComp.wasStoppedLastFrame = false;
                }

                float checkBuffer = 3;

                float upperLeftX = platCol.getX(platPos) - platCol.width/2;
                float upperRightX = platCol.getX(platPos) + platCol.width/2;
                float upperY = platCol.getY(platPos) - platCol.height/2 - checkBuffer;

                System.Drawing.PointF leftPoint = new System.Drawing.PointF(upperLeftX, upperY);
                System.Drawing.PointF rightPoint = new System.Drawing.PointF(upperRightX, upperY);
                List<Entity> aboveEnts = level.getCollisionSystem().findObjectsBetweenPoints( leftPoint, rightPoint );

                foreach(Entity otherEnt in aboveEnts) {
                    
                    if ( !stoppingEntities.Contains(  otherEnt.GetType() ) && otherEnt.hasComponent( GlobalVars.VELOCITY_COMPONENT_NAME ) ) {
                        PositionComponent otherPos = (PositionComponent) otherEnt.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                        VelocityComponent otherVel = ( VelocityComponent )otherEnt.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                        ColliderComponent otherCol = ( ColliderComponent )otherEnt.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );

                        System.Drawing.PointF grav = new System.Drawing.PointF( 0.0f, 0.0f );
                        if ( otherEnt.hasComponent( GlobalVars.GRAVITY_COMPONENT_NAME ) ) {
                            GravityComponent otherGrav = ( GravityComponent )otherEnt.getComponent( GlobalVars.GRAVITY_COMPONENT_NAME );
                            grav = new System.Drawing.PointF( otherGrav.x, otherGrav.y );
                        }

                        if ( movPlatComp.vertical ) {   
                            if ( (platVel.y == 0 || platVel.y <= 0 && (otherVel.y+grav.Y*deltaTime) >= platVel.y ) || ( platVel.y >= 0 && (otherVel.y + grav.Y*deltaTime) <= platVel.y ) ) {
                                otherVel.y = platVel.y - grav.Y*deltaTime;
                            }

                            level.getMovementSystem().changePosition( otherPos, otherCol.getX(otherPos), platCol.getY( platPos ) - platCol.height / 2 - otherCol.height / 2 - 1, false, false );
                        } else {
                            if ( ( platVel.x == 0 || platVel.x <= 0 && (otherVel.x+grav.X*deltaTime) >= platVel.x ) || ( platVel.x >= 0 && (otherVel.x+grav.X*deltaTime) <= platVel.y ) ) {
                                otherVel.x = platVel.x - grav.X*deltaTime;
                            }

                            if ( otherEnt is RunningGame.Entities.Player ) {
                                level.getMovementSystem().changePosition( otherPos, otherCol.getX( otherPos ) + platVel.x * deltaTime, platCol.getY( platPos ) - platCol.height / 2 - otherCol.height / 2 - 3, false, false );
                            } else {
                                level.getMovementSystem().changePosition( otherPos, otherCol.getX( otherPos ), platCol.getY( platPos ) - platCol.height / 2 - otherCol.height / 2 - 0, false, false );
                            }
                        }
                        if(otherEnt.hasComponent(GlobalVars.VEL_TO_ZERO_COMPONENT_NAME)) {
                            VelToZeroComponent velZedComp = (VelToZeroComponent)otherEnt.getComponent(GlobalVars.VEL_TO_ZERO_COMPONENT_NAME);
                            velZedComp.blockSlow = true;
                        }
                    }
                }

            }

        }
        //----------------------------------------------------------------------------------------------

    }
}
