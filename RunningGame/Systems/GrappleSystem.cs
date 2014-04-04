using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;

namespace RunningGame.Systems {
    [Serializable()]
    public class GrappleSystem : GameSystem {

        List<string> requiredComponents = new List<string>();
        Level level;

        float growSpeed = 600; // Pixels per Sec
        float followSpeed = 300;
        float retreatSpeed = 1000;

        bool stopPlayer = false;

        //0 = don't remove
        //1 = remove on shoot
        //2 = remove on latch
        public int removeGravity = 2;

        public bool isGrappling = false;

        public List<string> grappleColliders = new List<string>()
        {
            GlobalVars.BASIC_SOLID_COLLIDER_TYPE,
            GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE
        };

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public GrappleSystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.POSITION_COMPONENT_NAME ); //Position
            requiredComponents.Add( GlobalVars.GRAPPLE_COMPONENT_NAME ); //Grapple Link


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
        public override void Update( float deltaTime ) {
            bool loopWasEntered = false;
            foreach ( Entity e in getApplicableEntities() ) //There should only ever be 0 or 1
            {
                loopWasEntered = true;
                GrappleComponent grapComp = ( GrappleComponent )e.getComponent( GlobalVars.GRAPPLE_COMPONENT_NAME );

                //Growing
                if ( grapComp.state == 0 && level.getPlayer() != null ) {

                    float newX = grapComp.getLastPoint().X;
                    float newY = grapComp.getLastPoint().Y;

                    //Check if the player has moved. If so move the start point of the grapple.
                    PositionComponent playerPos = ( PositionComponent )level.getPlayer().getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                    if ( playerPos.getLocAsPoint() != grapComp.getFirstPoint() ) {
                        //Move start point
                        grapComp.setFirstPoint( playerPos.getLocAsPoint() );

                        //Check to see if it's intersecting anything now
                        Entity mrIntersection = null;
                        if ( checkForStopsBetweenPoints( grapComp.getFirstPoint(), grapComp.getLastPoint(), grapComp, ref mrIntersection ) ) {
                            //If it DID intersect something, destroy the grapple.
                            //finishGrapple(e, false);

                            //AKSHUALLY connect the grapple to the new thing
                            PositionComponent pos = ( PositionComponent )mrIntersection.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                            PointF setPos = getGrappleAttachPoint( mrIntersection, grapComp );
                            //grapComp.setEndPoint( pos.getLocAsPoint() );
                            grapComp.setEndPoint( setPos );
                            grapComp.state = 1;
                            return;
                        }
                    }

                    // If no collision - progress forwards!

                    //h = speed*time
                    //x = h*cos(theta)
                    //y = h*sin(theta)

                    float h = growSpeed * deltaTime;

                    newX += h * ( float )Math.Cos( grapComp.direction );
                    newY += h * ( float )Math.Sin( grapComp.direction );


                    System.Drawing.PointF p = new System.Drawing.PointF( newX, newY );

                    //Don't allow it to go off the sides of the screen
                    if ( newX < -h || newY < -h || newX > level.levelWidth+h || newY > level.levelHeight+h) {
                        grapComp.state = 2;
                        return;
                    }

                    //check if it's past the max grapple distance
                    if ( getDist( grapComp.getFirstPoint(), grapComp.getLastPoint() ) > GlobalVars.MAX_GRAPPLE_DISTANCE ) {
                        grapComp.state = 2; //Retreat!
                        return;
                    }




                    PointF p1 = new PointF( newX, newY );

                    //Check if it's done grappling
                    //colEnt is the entity which the grapple latches onto
                    Entity colEnt = null;
                    if ( checkForStopsBetweenPoints( grapComp.getLastPoint(), p1, grapComp, ref colEnt ) ) {
                        PositionComponent pos = ( PositionComponent )colEnt.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                        PointF setPos = getGrappleAttachPoint( colEnt, grapComp );
                        //grapComp.setEndPoint( pos.getLocAsPoint() );
                        grapComp.setEndPoint( setPos );
                        grapComp.myLink = colEnt;
                        grapComp.state = 1;
                        if ( removeGravity == 2 ) level.getPlayer().removeComponent( GlobalVars.GRAVITY_COMPONENT_NAME );
                        return;
                    }

                    //Not colliding with anything, set the end point to the calculated point.
                    grapComp.setEndPoint( p );


                }
                    //Following
                else if ( grapComp.state == 1 && level.getPlayer() != null ) {

                    if ( level.getPlayer() == null ) {
                        level.removeEntity( e );
                        return; //No player, nothing to do!
                    }

                    //Get player's position component
                    PositionComponent playerPos = ( PositionComponent )level.getPlayer().getComponent( GlobalVars.POSITION_COMPONENT_NAME );

                    /*
                    //Check to see if it's intersecting anything mid-way
                    Entity mrIntersection = null;
                    if ( checkForStopsBetweenPointsExclude( grapComp.getFirstPoint(), grapComp.getLastPoint(), grapComp.myLink, ref mrIntersection ) ) {
                        //If it DID intersect something, destroy the grapple.
                        //finishGrapple(e, false);
                        //return;
                        PositionComponent pos = ( PositionComponent )mrIntersection.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                        grapComp.setEndPoint( pos.getLocAsPoint() );
                    }
                    

                    //Check if, for whatever reason, the player wasn't able to move.
                    float buff = 0.5f;
                    buff = GlobalVars.MIN_TILE_SIZE;
                    if ( Math.Abs( playerPos.x - grapComp.getFirstPoint().X ) > buff || Math.Abs( playerPos.y - grapComp.getFirstPoint().Y ) > buff ) {
                        Console.WriteLine( "Finish 1" );
                        //finishGrapple( e, true );
                        grapComp.state = 2;
                        stopPlayer = true;
                        return;
                    }
                    */

                    double distBefore = getDist( playerPos.getLocAsPoint(), grapComp.getLastPoint() );

                    //Move first point in grapple up
                    float newX = grapComp.getFirstPoint().X;
                    float newY = grapComp.getFirstPoint().Y;

                    float h = followSpeed * deltaTime;

                    newX += h * ( float )Math.Cos( grapComp.direction );
                    newY += h * ( float )Math.Sin( grapComp.direction );

                    System.Drawing.PointF p = new System.Drawing.PointF( newX, newY );

                    
                    //This checks if the next point is near final point in the grapple.
                    //If so, remove the grapple and break.
                    ColliderComponent playerCol = ( ColliderComponent )level.getPlayer().getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                    float buffer = GlobalVars.MIN_TILE_SIZE;
                    if ( playerCol != null ) {
                        buffer += playerCol.width/1.5f;
                    } else {
                        buffer += playerPos.width/1.5f;
                    }
                    if ( Math.Abs( newX - grapComp.getLastPoint().X ) <= buffer && Math.Abs( newY - grapComp.getLastPoint().Y ) <= buffer ) {
                        //Console.WriteLine( "Finish 3!" );
                        //finishGrapple( e, true );
                        grapComp.state = 2;
                        stopPlayer = true;
                        return;
                    }
                    

                    //Move player
                    level.getMovementSystem().changePosition( playerPos, p.X, playerPos.y, false, false );
                    level.getMovementSystem().changePosition( playerPos, playerPos.x, p.Y, false, false );
                    
                    grapComp.setFirstPoint( playerPos.getLocAsPoint() );

                    //Check to make sure the player isn't getting further away. If he is, sthap!
                    double nowDist = getDist( grapComp.getFirstPoint(), grapComp.getLastPoint() );
                    if ( nowDist >= distBefore ) {
                        //Console.WriteLine( "Finish 2!" );
                        //finishGrapple( e, true );
                        grapComp.state = 2;
                        stopPlayer = true;
                        return;
                    }

                }
                    //Retreating
                else if ( grapComp.state == 2 && level.getPlayer() != null ) {
                    PositionComponent playerPos = ( PositionComponent )level.getPlayer().getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                    grapComp.setFirstPoint( playerPos.getLocAsPoint() );

                    double distBefore = getDist( grapComp.getFirstPoint(), grapComp.getLastPoint() );

                    float newX = grapComp.getLastPoint().X;
                    float newY = grapComp.getLastPoint().Y;

                    float h = retreatSpeed * deltaTime;

                    if ( h > distBefore ) h = ( float )distBefore;

                    //Calculate the direction
                    double dir = Math.Atan( ( grapComp.getFirstPoint().Y - newY ) / ( grapComp.getFirstPoint().X - newX ) );

                    if ( grapComp.getFirstPoint().X < newX ) {
                        dir += Math.PI;
                    }

                    newX += h * ( float )Math.Cos( dir );
                    newY += h * ( float )Math.Sin( dir );

                    System.Drawing.PointF p = new System.Drawing.PointF( newX, newY );

                    //Make sure it isn't getting longer
                    if ( getDist( p, grapComp.getFirstPoint() ) > distBefore ) {
                        finishGrapple( e );
                        return;
                    }

                    
                    //Check if it's fully retreated - if so, delete the grapple!
                    float buffer = GlobalVars.MIN_TILE_SIZE;
                    if ( Math.Abs( newX - grapComp.getFirstPoint().X ) <= buffer && Math.Abs( newY - grapComp.getFirstPoint().Y ) <= buffer ) {
                        finishGrapple( e );
                        return;
                    }
                    

                    grapComp.setEndPoint( p );

                }

            }
            isGrappling = loopWasEntered;

        }
        //----------------------------------------------------------------------------------------------

        public double getDist( PointF p1, PointF p2 ) {
            return Math.Sqrt( Math.Pow( p1.X - p2.X, 2 ) + Math.Pow( p1.Y - p2.Y, 2 ) );
        }

        /*public bool checkForStopsBetweenPoints( PointF p1, PointF p2 ) {
            List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( p1.X, p1.Y, p2.X, p2.Y );
            if ( cols.Count > 0 ) {
                foreach ( Entity e in cols ) {
                    ColliderComponent col = ( ColliderComponent )e.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                    if ( grappleColliders.Contains( col.colliderType ) ) {
                        return true;
                    }
                }
            }
            return false;
        }*/

        public bool checkForStopsBetweenPoints( PointF p1, PointF p2, GrappleComponent grap, ref Entity ent ) {

            List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( p1.X, p1.Y, p2.X, p2.Y );
            
            double minDist = Double.MaxValue;
            Entity minEnt = null;

            if ( cols.Count > 0 ) {
                foreach ( Entity e in cols ) {
                    ColliderComponent col = ( ColliderComponent )e.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                    if ( grappleColliders.Contains( col.colliderType ) ) {
                        PositionComponent entPos = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                        double dist = getDist( grap.getLastPoint(), col.getLocationAsPoint( entPos ) );
                        if ( dist < minDist ) {
                            minEnt = e;
                            minDist = dist;
                        }
                    }
                }
            }

            if ( minEnt != null ) {
                ent = minEnt;
                return true;
            }
            return false;
        }

        public bool checkForStopsBetweenPointsExclude( PointF p1, PointF p2, Entity exclude ) {
            List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( p1.X, p1.Y, p2.X, p2.Y );
            if ( cols.Count > 0 ) {
                foreach ( Entity e in cols ) {
                    ColliderComponent col = ( ColliderComponent )e.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                    if ( e != exclude && grappleColliders.Contains( col.colliderType ) ) {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool checkForStopsBetweenPointsExclude( PointF p1, PointF p2, Entity exclude, ref Entity colEnt ) {
            List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( p1.X, p1.Y, p2.X, p2.Y );
            if ( cols.Count > 0 ) {
                foreach ( Entity e in cols ) {
                    ColliderComponent col = ( ColliderComponent )e.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                    if ( e != exclude && grappleColliders.Contains( col.colliderType ) ) {
                        colEnt = e;
                        return true;
                    }
                }
            }
            return false;
        }

        public void finishGrapple( Entity grapple ) {

            if ( !level.getPlayer().hasComponent( GlobalVars.PLAYER_INPUT_COMPONENT_NAME ) ) {
                PlayerInputComponent plInComp = ( PlayerInputComponent )level.getPlayer().addComponent( new PlayerInputComponent( level.getPlayer() ) );
                plInComp.passedAirjumps = 0;
            }
            if ( !level.getPlayer().hasComponent( GlobalVars.GRAVITY_COMPONENT_NAME ) ) {
                level.getPlayer().addComponent( new GravityComponent( 0, GlobalVars.STANDARD_GRAVITY ) );
            }


            if ( stopPlayer ) {
                VelocityComponent velComp = ( VelocityComponent )level.getPlayer().getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );

                velComp.x = 0;
                velComp.y = 0;

                stopPlayer = false;
            }
            if ( level.curGrap == grapple ) {
                level.curGrap = null;
            }
            level.removeEntity( grapple );
            return;
        }

        //Picks either the upper, lower, left, or right sides for the grapple to attach to.
        public PointF getGrappleAttachPoint( Entity attachEnt, GrappleComponent grapComp) {

            PositionComponent attachPosComp = ( PositionComponent )attachEnt.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            PointF attachPos = attachPosComp.getLocAsPoint();
            PointF attachSize = attachPosComp.getSizeAsPoint();

            if ( attachEnt.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) ) {
                ColliderComponent colComp = ( ColliderComponent )attachEnt.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                attachPos = colComp.getLocationAsPoint(attachPosComp);
                attachSize = colComp.getSizeAsPoint();
            }

            PointF grapPos = grapComp.getLastPoint();
            PointF left = new PointF(attachPos.X - attachSize.X/2, attachPos.Y);
            PointF right = new PointF( attachPos.X + attachSize.X / 2, attachPos.Y );
            PointF up = new PointF( attachPos.X, attachPos.Y - attachSize.Y / 2 );
            PointF down = new PointF( attachPos.X, attachPos.Y + attachSize.Y / 2 );

            double distLeft = getDist(grapPos, left);
            double distRight = getDist( grapPos, right );
            double distUp = getDist( grapPos, up );
            double distDown = getDist( grapPos, down );

            double minA = Math.Min( distLeft, distRight );
            double minB = Math.Min( distUp, distDown );
            double min = Math.Min( minA, minB );

            if ( min == distLeft ) {
                return left;
            } else if ( min == distRight ) {
                return right;
            } else if ( min == distUp ) {
                return up;
            } else if ( min == distDown ) {
                return down;
            } else {
                Console.WriteLine( "Error picking point for grapple attachment!" );
                return attachPos;
            }

        }

    }
}
