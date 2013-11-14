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

namespace RunningGame.Systems
{
    [Serializable()]
    public class GrappleSystem:GameSystem
    {

        
        ArrayList requiredComponents = new ArrayList();
        Level level;
        
        float growSpeed = 600; // Pixels per Sec
        float followSpeed = 300;
        float retreatSpeed = 1000;

        public bool isGrappling = false;

        public List<string> grappleColliders = new List<string>()
        {
            GlobalVars.BASIC_SOLID_COLLIDER_TYPE
        };

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public GrappleSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.GRAPPLE_COMPONENT_NAME); //Grapple Link


            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override ArrayList getRequiredComponents()
        {
            return requiredComponents;
        }
        
        //Must have this. Same for all Systems.
        public override Level GetActiveLevel()
        {
            return level;
        }

        //You must have an Update.
        public override void Update(float deltaTime)
        {
            bool loopWasEntered = false;
            foreach (Entity e in getApplicableEntities()) //There should only ever be 0 or 1
            {
                loopWasEntered = true;
                GrappleComponent grapComp = (GrappleComponent)e.getComponent(GlobalVars.GRAPPLE_COMPONENT_NAME);

                //Growing
                if (grapComp.state == 0)
                {
                    
                    float newX = grapComp.getLastPoint().X;
                    float newY = grapComp.getLastPoint().Y;

                    PointF p1 = new PointF(newX - 5, newY + 5);
                    PointF p2 = new PointF(newX + 5, newY - 5);

                    //Check if it's done grappling
                    if (isGrappleableEntity(p1, p2))
                    {
                        grapComp.state = 1;
                        return;
                    }

                    //Check if the player has moved. If so move the start point of the grapple.
                    PositionComponent playerPos = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    if (playerPos.getPointF() != grapComp.getFirstPoint())
                    {
                        //Move start point
                        grapComp.setFirstPoint(playerPos.getPointF());

                        //Check to see if it's intersecting anything now
                        if (isGrappleableEntity(grapComp.getFirstPoint(), grapComp.getLastPoint()))
                        {
                            //If it DID intersect something, destroy the grapple.
                            finishGrapple(e);
                            return;
                        }
                    }

                    // If no collision - progress forwards!

                    //h = speed*time
                    //x = h*cos(theta)
                    //y = h*sin(theta)

                    float h = growSpeed * deltaTime;

                    newX += h * (float)Math.Cos(grapComp.direction);
                    newY += h * (float)Math.Sin(grapComp.direction);


                    System.Drawing.PointF p = new System.Drawing.PointF(newX, newY);

                    grapComp.setEndPoint(p);

                    //Don't allow it to go off the sides of the screen
                    if (newX < 0 || newY < 0 || newX > level.levelWidth || newY > level.levelHeight)
                    {
                        grapComp.state = 2;
                        return;
                    }

                    //check if it's past the max grapple distance
                    if (getDist(grapComp.getFirstPoint(), grapComp.getLastPoint()) > GlobalVars.MAX_GRAPPLE_DISTANCE)
                    {
                        grapComp.state = 2; //Retreat!
                    }

                }
                //Following
                else if (grapComp.state == 1)
                {

                    if (level.getPlayer() == null)
                    {
                        level.removeEntity(e);
                        return; //No player, nothing to do!
                    }

                    //Get player's position component
                    PositionComponent playerPos = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    //Get first point in grapple
                    PointF newPlayerPos = grapComp.getFirstPoint();
                    //Move player
                    level.getMovementSystem().changePosition(playerPos, newPlayerPos.X, newPlayerPos.Y, true);

                    float buff = 0.5f;

                    if (Math.Abs(playerPos.x - grapComp.getFirstPoint().X) > buff || Math.Abs(playerPos.y - grapComp.getFirstPoint().Y) > buff)
                    {
                        finishGrapple(e);
                    }

                    double distBefore = getDist(playerPos.getPointF(), grapComp.getLastPoint());

                    //Move first point in grapple up
                    float newX = grapComp.getFirstPoint().X;
                    float newY = grapComp.getFirstPoint().Y;

                    float h = followSpeed * deltaTime;

                    newX += h * (float)Math.Cos(grapComp.direction);
                    newY += h * (float)Math.Sin(grapComp.direction);

                    System.Drawing.PointF p = new System.Drawing.PointF(newX, newY);

                    //Check to make sure the player isn't getting further away. If he is, sthap!
                    if (getDist(p, grapComp.getLastPoint()) > distBefore)
                    {
                        finishGrapple(e);
                        return;
                    }

                    //This checks if the next point is near final point in the grapple.
                    //If so, remove the grapple and break.
                    float buffer = GlobalVars.MIN_TILE_SIZE;
                    if (Math.Abs(newX - grapComp.getLastPoint().X ) <= buffer && Math.Abs(newY - grapComp.getLastPoint().Y) <= buffer)
                    {
                        finishGrapple(e);
                        return;
                    }

                    grapComp.setFirstPoint(p);

                }
                //Retreating
                else if (grapComp.state == 2)
                {

                    double distBefore = getDist(grapComp.getFirstPoint(), grapComp.getLastPoint());

                    float newX = grapComp.getLastPoint().X;
                    float newY = grapComp.getLastPoint().Y;

                    float h = retreatSpeed * deltaTime;

                    newX += h * (float)Math.Cos(grapComp.direction+Math.PI);
                    newY += h * (float)Math.Sin(grapComp.direction+Math.PI);

                    System.Drawing.PointF p = new System.Drawing.PointF(newX, newY);

                    //Make sure it isn't getting longer
                    if (getDist(p, grapComp.getFirstPoint()) > distBefore)
                    {
                        finishGrapple(e);
                        return;
                    }

                    //Check if it's fully retreated - if so, delete the grapple!
                    float buffer = GlobalVars.MIN_TILE_SIZE;
                    if (Math.Abs(newX - grapComp.getFirstPoint().X) <= buffer && Math.Abs(newY - grapComp.getFirstPoint().Y) <= buffer)
                    {
                        finishGrapple(e);
                        return;
                    }


                    grapComp.setEndPoint(p);

                }

            }
            isGrappling = loopWasEntered;

        }
        //----------------------------------------------------------------------------------------------

        public double getDist(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public bool isGrappleableEntity(PointF p1, PointF p2)
        {
            ArrayList cols = level.getCollisionSystem().findObjectsBetweenPoints(p1.X, p1.Y, p2.X, p2.Y);
            if (cols.Count > 0)
            {
                foreach (Entity e in cols)
                {
                    ColliderComponent col = (ColliderComponent)e.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);
                    if (grappleColliders.Contains(col.colliderType))
                    {
                        //level.removeEntity(e);
                        return true;
                    }
                }
            }
            return false;
        }

        public void finishGrapple(Entity grapple)
        {
            VelocityComponent velComp = (VelocityComponent)level.getPlayer().getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

            velComp.x = 0;
            velComp.y = 0;

            level.removeEntity(grapple);
            return;
        }

    }
}
