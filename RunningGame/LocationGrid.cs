using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using RunningGame.Components;

namespace RunningGame
{

    /* The LocationGrid splits the game world into a grid.
     * Each cell in the grid contains any object that intersects
     * that cell in the game world.
     * This is useful to speedup collisions detection - you only need
     * to check objects in the cell you're moving into, rather than
     * everything in the game.
     */

    class LocationGrid
    {

        public Dictionary<RectangleF, ArrayList> grid;

        float rowHeight = 25;
        float colWidth = 25;

        public LocationGrid(Level level)
        {
            grid = new Dictionary<RectangleF, ArrayList>();
            //Create the grid (With extra rows/cols on either side)
            for (float i = -colWidth; i <= level.levelWidth+colWidth; i += colWidth)
            {
                for (float j = -rowHeight; j <= level.levelHeight+rowHeight; j += rowHeight)
                {
                    RectangleF rect = new RectangleF(i, j, colWidth, rowHeight);
                    grid.Add(rect, new ArrayList());
                }
            }

        }

        //Add an entity to any grid slot that it intersects with
        public void addEntity(Entity e)
        {

            foreach (RectangleF rect in getIntersectingRectangles(e))
            {
                if (!grid[rect].Contains(e))
                {
                    grid[rect].Add(e);
                }
            }

        }



        public void handleMovedEntity(Entity e, float prevX, float prevY)
        {
            removeEntity(e, prevX, prevY);
            addEntity(e);
        }


        public void removeEntity(Entity e, float prevX, float prevY)
        {
            foreach (RectangleF rect in getIntersectingRectangles(e, prevX, prevY))
            {
                if (grid[rect].Contains(e))
                {
                    grid[rect].Remove(e);
                }
            }
        }



        //Returns all rectangles an entity intersects with.
        public ArrayList getIntersectingRectangles(Entity e)
        {
            ArrayList retList = new ArrayList();
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            return getIntersectingRectangles(e, posComp.x, posComp.y);
        }
        //Returns all rectangles an entity intersected last frame
        public ArrayList getIntersectingRectangles(Entity e, float x, float y)
        {

            ArrayList retList = new ArrayList();

            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            PointF upperLeftPoint = new PointF(x - posComp.width / 2, y - posComp.height / 2); //Easier to calculate stuff using this rather than center


            int numHorizontalRect = (int)Math.Ceiling(((upperLeftPoint.X % colWidth) + posComp.width) / colWidth); //number of rectangles it intersects horizontally
            int numVertRect = (int)Math.Ceiling(((upperLeftPoint.Y % rowHeight) + posComp.height) / rowHeight); //number of rectangles it intersects vertically


            //Go through all rectangles it intersects
            for (float i = (upperLeftPoint.X - upperLeftPoint.X % colWidth); i < (upperLeftPoint.X - upperLeftPoint.X % colWidth + numHorizontalRect * colWidth); i += colWidth)
            {
                for (float j = (upperLeftPoint.Y - upperLeftPoint.Y % rowHeight); j < (upperLeftPoint.Y - upperLeftPoint.Y % rowHeight + numVertRect * rowHeight); j += rowHeight)
                {
                    RectangleF rect = new RectangleF(i, j, colWidth, rowHeight);
                    if (grid.ContainsKey(rect))
                        retList.Add(rect);
                    else
                        Console.WriteLine(e + " is Intersecting nonexistant rectangle " + rect);
                }
            }

            return retList;
        }


        public ArrayList findObjectsAtPoint(float x, float y)
        {
            ArrayList retList = new ArrayList();

            RectangleF checkRect = getRectangleWithPoint(x, y);

            if (!grid.ContainsKey(checkRect))
            {
                Console.WriteLine("Grid does not contain " + checkRect);
                return new ArrayList();
            }

            foreach (Entity e in grid[checkRect])
            {
                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

                RectangleF r = new RectangleF(posComp.x - posComp.width / 2, posComp.y - posComp.height / 2, posComp.width, posComp.height);
                if (r.Contains(x, y)) retList.Add(e);
            }

            return retList;
        }


        public ArrayList findObjectsBetweenPoints(float x1, float y1, float x2, float y2)
        {
            ArrayList retList = new ArrayList();

            int skipNum = 1;

            float lowerX = Math.Min(x1, x2);
            float higherX = Math.Max(x1, x2);
            float lowerY = Math.Min(y1, y2);
            float higherY = Math.Max(y1, y2);

            
            for (float x = lowerX; x <= higherX; x += skipNum)
            {
                for (float y = lowerY; y <= higherY; y += skipNum)
                {
                    retList = mergeArrayLists(retList, findObjectsAtPoint(x, y));
                }
            }
            
            return retList;

        }


        //Returns rectangle containing a point
        public RectangleF getRectangleWithPoint(float x, float y)
        {
            float rectX = x -(x % colWidth);
            float rectY = y - (y % rowHeight);

            return new RectangleF(rectX, rectY, colWidth, rowHeight);
        }



        public ArrayList checkForCollisions(Entity e, float x, float y)
        {
            ArrayList collisions = new ArrayList();

            foreach (RectangleF rect in getIntersectingRectangles(e, x, y))
            {
                foreach (Entity other in grid[rect])
                {
                    if (other != e)
                    {
                        if (checkTwoEntityCollision(x, y, e, other)) collisions.Add(other);
                    }
                }
            }

            return collisions;
        }



        public bool checkTwoEntityCollision(float x1, float y1, Entity e1, Entity e2)
        {

            PositionComponent posComp1 = (PositionComponent)e1.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            PositionComponent posComp2 = (PositionComponent)e2.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

            float buffer = -0.01f;

            float xDiff = (float)Math.Abs(x1 - posComp2.x);
            float yDiff = (float)Math.Abs(y1 - posComp2.y);

            return ( (xDiff - (posComp1.width / 2 + posComp2.width / 2)) <= buffer && (yDiff - (posComp1.height / 2 + posComp2.height / 2)) <= buffer);

        }
        
        
        public ArrayList mergeArrayLists(ArrayList a1, ArrayList a2)
        {
            foreach (Object o in a2)
            {
                a1.Add(o);
            }

            return a1;
        }

        //Draws an APPROXIMATE representation of the grid
        public void Draw(Graphics g)
        {
            foreach (RectangleF r in grid.Keys)
            {
                if(grid[r].Count > 0) g.FillRectangle(Brushes.Red, new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));

                g.DrawRectangle(new Pen(Brushes.Black), new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));

                g.DrawString(grid[r].Count + "", SystemFonts.DefaultFont, Brushes.Black, new RectangleF(r.X + colWidth/3, r.Y + rowHeight/3, 2*colWidth/3, 2*rowHeight/3));

            }
        }


    }
}
