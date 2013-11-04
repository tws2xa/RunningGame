using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Level_Editor;

namespace RunningGame.Systems
{

    /*
     * The collision detection system is what is used to
     * check for collisions.
     * 
     * It checks for entities with collider components,
     * and then looks to see if any of the entities have changed position.
     * If they have, it alerts the LocationGrid and changes the cells accordingly.
     * 
     * It has a few methods to check for collisions/objects, which must be
     * called seperately. So another system can come to this one
     * and ask if there would be a collision in location (x, y).
     * 
     */

    [Serializable()]
    public class CollisionDetectionSystem : GameSystem
    {

        ArrayList requiredComponents = new ArrayList();
        Level level;
        public LocationGrid locGrid;

        public CollisionDetectionSystem(Level level)
        {
            //Required components
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.COLLIDER_COMPONENT_NAME); //Collider

            this.level = level;


            locGrid = new LocationGrid(level);

        }

        //-------------------------------------- Overrides -------------------------------------------
        public override ArrayList getRequiredComponents()
        {
            return requiredComponents;
        }

        public override Level GetActiveLevel()
        {
            return level;
        }

        public override void Update(float deltaTime)
        {
            foreach (Entity e in getApplicableEntities())
            {
                //Check each entity for whether or not it's been moved. If so - handle it.
                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                if (posComp.positionHasChanged)
                {
                    locGrid.handleMovedEntity(e);
                    posComp.positionHasChanged = false;
                }

            }
            
        }
        //----------------------------------------------------------------------------------------------

        public void colliderAdded(Entity e)
        {
            locGrid.addEntity(e);
        }
        public void colliderRemoved(Entity e)
        {
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            locGrid.removeEntity(e, posComp.prevX, posComp.prevY, posComp.prevW, posComp.prevH);
        }

        public ArrayList checkForCollision(Entity e, float newX, float newY, float width, float height)
        {
            return locGrid.checkForCollisions(e, newX, newY, width, height);
        }

        public ArrayList findObjectAtPoint(float x, float y)
        {
            return locGrid.findObjectsAtPoint(x, y);
        }

        public ArrayList findObjectsBetweenPoints(float x1, float y1, float x2, float y2)
        {
            return locGrid.findObjectsBetweenPoints(x1, y1, x2, y2);
        }

        public void MouseClick(float x, float y)
        {
            locGrid.MouseClick(x, y);
        }

        //Draw the tree
        public void Draw(System.Drawing.Graphics g)
        {
            locGrid.Draw(g);
        }

    }
}
