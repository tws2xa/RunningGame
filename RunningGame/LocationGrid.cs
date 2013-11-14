using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using RunningGame.Components;
using RunningGame.Level_Editor;

namespace RunningGame
{

    /* The LocationGrid splits the game world into a grid.
     * Each cell in the grid contains any object that intersects
     * that cell in the game world.
     * This is useful to speedup collisions detection - you only need
     * to check objects in the cell you're moving into, rather than
     * everything in the game.
     * 
     * TODO: handleMovedEntity could probably work more efficiently
     */

    [Serializable()]
    public class LocationGrid
    {

        public Dictionary<RectangleF, ArrayList> grid;

        float rowHeight = 20;
        float colWidth = 20;

        Level level;

        //For drawing the grid
        Color filledCol = Color.FromArgb(100, Color.Red);
        Pen borderPen = Pens.Black;

        public LocationGrid(Level level)
        {
            this.level = level;
            grid = new Dictionary<RectangleF, ArrayList>();
            //Create the grid (With extra rows/cols on either side)
            for (float i = -colWidth*2; i <= level.levelWidth+colWidth*4; i += colWidth)
            {
                for (float j = -rowHeight*2; j <= level.levelHeight+rowHeight*4; j += rowHeight)
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



        public void handleMovedEntity(Entity e)
        {
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            removeEntity(e, posComp.prevX, posComp.prevY, posComp.prevW, posComp.prevH);
            addEntity(e);
        }


        public void removeEntity(Entity e, float prevX, float prevY, float prevWidth, float prevHeight)
        {
            foreach (ArrayList list in grid.Values)
                list.Remove(e);

            /*
            foreach (RectangleF rect in getIntersectingRectangles(prevX, prevY, prevWidth, prevHeight))
            {
                
                //if(!grid[rect].Contains(e)) {
                //    Console.WriteLine("LocGrid Doesn't contain " + e);
                //    string str = "";
                //    foreach (Entity ent in grid[rect])
                //    {
                //        str += (": " + ent + " :");
                //    }
                //    Console.WriteLine("LocGrid. Size:  " + grid[rect].Count + " " + str);
                // }
                if (!grid[rect].Contains(e)) Console.WriteLine("Trying to remove nonexistant " + e);
                grid[rect].Remove(e);
            }
            */
        }

        public void removeStationaryEntity(Entity e)
        {
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            removeEntity(e, posComp.x, posComp.y, posComp.width, posComp.height);
        }



        //Returns all rectangles an entity intersects with.
        public ArrayList getIntersectingRectangles(Entity e)
        {
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            return getIntersectingRectangles(posComp.x, posComp.y, posComp.width, posComp.height);
        }
        //Returns all rectangles an entity intersected last frame
        public ArrayList getIntersectingRectangles(float x, float y, float width, float height)
        {

            ArrayList retList = new ArrayList();

            PointF upperLeftPoint = new PointF(x - width / 2, y - height / 2); //Easier to calculate stuff using this rather than center


            int numHorizontalRect = (int)Math.Ceiling(((upperLeftPoint.X % colWidth) + width) / colWidth); //number of rectangles it intersects horizontally
            int numVertRect = (int)Math.Ceiling(((upperLeftPoint.Y % rowHeight) + height) / rowHeight); //number of rectangles it intersects vertically


            //Go through all rectangles it intersects
            if (colWidth > 0 && rowHeight > 0)
            {
                float iMin = (upperLeftPoint.X - upperLeftPoint.X % colWidth);
                float iMax = (upperLeftPoint.X - upperLeftPoint.X % colWidth + numHorizontalRect * colWidth);
                for (float i = iMin; i < iMax; i += colWidth)
                {
                    float jMin = (upperLeftPoint.Y - upperLeftPoint.Y % rowHeight);
                    float jMax = (upperLeftPoint.Y - upperLeftPoint.Y % rowHeight + numVertRect * rowHeight);
                    for (float j = jMin; j < jMax; j += rowHeight)
                    {
                        RectangleF rect = new RectangleF(i, j, colWidth, rowHeight);
                        if (grid.ContainsKey(rect))
                            retList.Add(rect);
                        else
                            Console.WriteLine("Looking for nonexistant rectangle " + rect);
                    }
                }
            }
            else
            {
                Console.WriteLine("I have no idea why colwidth: " + colWidth + " or RowHeight: " + rowHeight + " would be negative... but it'd explain a lot.");
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


            double theta = Math.Atan((higherY - lowerY) / (higherX - lowerX));

            float checkX = lowerX;
            float checkY = lowerY;

            while (checkX <= lowerX && checkY <= lowerY)
            {
                retList = mergeArrayLists(retList, findObjectsAtPoint(checkX, checkY));
                checkX += skipNum * (float)Math.Cos(theta);
                checkY += skipNum * (float)Math.Sin(theta);
            }

            /*
            for (float x = lowerX; x <= higherX; x += skipNum)
            {
                for (float y = lowerY; y <= higherY; y += skipNum)
                {
                    retList = mergeArrayLists(retList, findObjectsAtPoint(x, y));
                }
            }
             * */
            
            return retList;

        }


        //Returns rectangle containing a point
        public RectangleF getRectangleWithPoint(float x, float y)
        {
            float rectX = x -(x % colWidth);
            float rectY = y - (y % rowHeight);

            return new RectangleF(rectX, rectY, colWidth, rowHeight);
        }



        public ArrayList checkForCollisions(Entity e, float x, float y, float w, float h)
        {

            
            ArrayList collisions = new ArrayList();

            if (e == null || w == 0 || h == 0) return collisions;

            Array rectArray = getIntersectingRectangles(x, y, w, h).ToArray();

            foreach (RectangleF rect in rectArray)
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
                if(!(a1.Contains(o)))
                {
                    a1.Add(o);
                }
            }

            return a1;
        }

        //Draws an APPROXIMATE representation of the grid
        public void Draw(Graphics g)
        {

            PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);

            foreach (RectangleF r in getIntersectingRectangles(posComp.prevX, posComp.prevY, posComp.prevW, posComp.prevH))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(120, Color.Green)), new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));
            }
            foreach (RectangleF r in grid.Keys)
            {
                if(grid[r].Count > 0) g.FillRectangle(new SolidBrush(filledCol), new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));
                //else g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Blue)), new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));

                g.DrawRectangle(borderPen, new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));

                //if(grid[r].Count > 0) g.DrawString(grid[r].Count + "", SystemFonts.DefaultFont, Brushes.Black, new RectangleF(r.X + colWidth/3, r.Y + rowHeight/3, 2*colWidth/3, 2*rowHeight/3));

            }
        }

        public void MouseClick(float x, float y)
        {
            
            RectangleF rect = getRectangleWithPoint(x, y);
            /*Array ents = grid[rect].ToArray();
            foreach (Entity e in ents)
            {
                level.removeEntity(e);
            }*/
            
            string str = "";
            foreach (Entity ent in grid[rect])
            {
                str += (": " + ent + " :");
            }
            Console.WriteLine("LocGrid. Size:  " + grid[rect].Count + " " + str);
            
        }
    }
}
