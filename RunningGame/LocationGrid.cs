using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using RunningGame.Components;
using RunningGame.Level_Editor;

namespace RunningGame {

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
    public class LocationGrid {

        public Dictionary<RectangleF, Dictionary<int, Entity>> grid;

        float rowHeight = 20;
        float colWidth = 20;

        public bool debugOnObjBetweenPnts = false;

        Level level;

        //For drawing the grid
        Color filledCol = Color.FromArgb( 100, Color.Red );
        Pen borderPen = Pens.Black;

        public LocationGrid( Level level ) {
            this.level = level;
            grid = new Dictionary<RectangleF, Dictionary<int, Entity>>();
            //Create the grid (With extra rows/cols on either side)
            for ( float i = -colWidth * 4; i <= level.levelWidth + colWidth * 8; i += colWidth ) {
                for ( float j = -rowHeight * 4; j <= level.levelHeight + rowHeight * 8; j += rowHeight ) {
                    RectangleF rect = new RectangleF( i, j, colWidth, rowHeight );
                    grid.Add( rect, new Dictionary<int, Entity>() );
                }
            }

        }

        //Add an entity to any grid slot that it intersects with
        public void addEntity( Entity e ) {

            foreach ( RectangleF rect in getIntersectingRectangles( e ) ) {
                if ( !grid[rect].ContainsKey( e.randId ) ) {
                    grid[rect].Add( e.randId, e );
                }
            }

        }



        public void handleMovedEntity( Entity e ) {
            PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            removeEntity( e, posComp.prevX, posComp.prevY, posComp.prevW, posComp.prevH );
            addEntity( e );
        }


        public void removeEntity( Entity e, float prevX, float prevY, float prevWidth, float prevHeight ) {
            foreach ( Dictionary<int, Entity> d in grid.Values ) {
                if ( d.ContainsKey( e.randId ) )
                    d.Remove( e.randId );
            }

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

        public void removeStationaryEntity( Entity e ) {
            PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            removeEntity( e, posComp.x, posComp.y, posComp.width, posComp.height );
        }



        //Returns all rectangles an entity intersects with.
        public List<RectangleF> getIntersectingRectangles( Entity e ) {
            PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            return getIntersectingRectangles( posComp.x, posComp.y, posComp.width, posComp.height );
        }
        //Returns all rectangles an entity intersected last frame
        public List<RectangleF> getIntersectingRectangles( float x, float y, float width, float height ) {

            List<RectangleF> retList = new List<RectangleF>();

            PointF upperLeftPoint = new PointF( x - width / 2, y - height / 2 ); //Easier to calculate stuff using this rather than center


            int numHorizontalRect = ( int )Math.Ceiling( ( ( upperLeftPoint.X % colWidth ) + width ) / colWidth ); //number of rectangles it intersects horizontally
            int numVertRect = ( int )Math.Ceiling( ( ( upperLeftPoint.Y % rowHeight ) + height ) / rowHeight ); //number of rectangles it intersects vertically


            //Go through all rectangles it intersects
            if ( colWidth > 0 && rowHeight > 0 ) {
                float iMin = ( upperLeftPoint.X - upperLeftPoint.X % colWidth );
                float iMax = ( upperLeftPoint.X - upperLeftPoint.X % colWidth + numHorizontalRect * colWidth );
                for ( float i = iMin; i < iMax; i += colWidth ) {
                    float jMin = ( upperLeftPoint.Y - upperLeftPoint.Y % rowHeight );
                    float jMax = ( upperLeftPoint.Y - upperLeftPoint.Y % rowHeight + numVertRect * rowHeight );
                    for ( float j = jMin; j < jMax; j += rowHeight ) {
                        RectangleF rect = new RectangleF( i, j, colWidth, rowHeight );
                        if ( grid.ContainsKey( rect ) )
                            retList.Add( rect );
                        //else
                        //Console.WriteLine("Looking for nonexistant rectangle " + rect);
                    }
                }
            } else {
                Console.WriteLine( "I have no idea why colwidth: " + colWidth + " or RowHeight: " + rowHeight + " would be negative... but it'd explain a lot." );
            }
            return retList;
        }


        public List<Entity> findObjectsAtPoint( float x, float y ) {
            List<Entity> retList = new List<Entity>();

            RectangleF checkRect = getRectangleWithPoint( x, y );

            if ( !grid.ContainsKey( checkRect ) ) {
                //Console.WriteLine("Grid does not contain " + checkRect);
                return new List<Entity>();
            }

            foreach ( Entity e in grid[checkRect].Values ) {
                PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                ColliderComponent colComp = ( ColliderComponent )e.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );

                RectangleF r = new RectangleF( colComp.getX(posComp) - colComp.width / 2, colComp.getY(posComp) - colComp.height / 2, colComp.width, colComp.height );
                if ( r.Contains( x, y ) ) retList.Add( e );
            }

            return retList;
        }


        public List<Entity> findObjectsBetweenPoints( float x1, float y1, float x2, float y2 ) {
            List<Entity> retList = new List<Entity>();

            float skipNum = 1.0f;


            

            //--------------------------------------
            /*
            double theta = Math.PI / 2;

            float opp = Math.Abs( y2 - y1 );
            float adj = Math.Abs( x2 - x1 );


            if ( x2 != x1 ) {
                theta = Math.Atan( ( opp ) / ( adj ) );
                
                if ( x2 < x1 && y2 < y1) theta = Math.PI - theta; //Quad 2
                if ( x2 < x1 && y2 > y1 ) theta = Math.PI + theta; //Quad 3
                if ( x2 > x1 && y2 > y1 ) theta = 2 * Math.PI - theta; //Quad 4
                
            } else if ( y2 < y1 ) {
                theta = 3 * Math.PI / 2;
            }
            */
            //--------------------------------------
            
            float xDiff = x1 - x2;
            float yDiff = y1 - y2;

            if ( xDiff == 0 && yDiff == 0 ) {
                return new List<Entity>();
            }
            
            double theta = Math.Atan( yDiff / xDiff );

            if ( x2 < x1 ) {
                theta += Math.PI;
            }
            
            //-------------------------------------- 

            if ( debugOnObjBetweenPnts ) {
                Console.WriteLine( theta * 360 / (Math.PI * 2) );
            }

            bool printStuff = false;
            if ( theta != 0 && theta != Math.PI / 2 && theta != Math.PI && theta != Math.PI * 3 / 2 && theta != 2 * Math.PI ) {
                printStuff = false;
            }

            if ( printStuff ) {
                Console.WriteLine( "Theta: " + theta * 360 / ( Math.PI * 2 ) + " degrees = " + theta + " radians." );
            }

            float checkX = x1;
            float checkY = y1;

            bool hasChanged = false;

            double dist = getDist( new PointF( checkX, checkY ), new PointF( x2, y2 ) );


            //level.sysManager.drawSystem.debugLines.Add( new PointF( x1, y1 ) );
            //level.sysManager.drawSystem.debugLines.Add( new PointF( x2, y2 ) );
            int counter = 0;
            while ( !hasChanged ) {
                counter++;
                if ( counter > 1000 ) {
                    Console.WriteLine( "Whoa there!" );
                    counter = 0;
                }
                /*if ( printStuff ) {
                    Console.Write( "\t(" + checkX + ", " + checkY + ") -> " );
                }*/

                level.sysManager.drawSystem.debugPoints.Add( new PointF( checkX, checkY ) );
                List<Entity> addList = findObjectsAtPoint(checkX, checkY);
                retList = retList.Union( addList ).ToList<Entity>();
                checkX += skipNum * ( float )Math.Cos( theta );
                checkY += skipNum * ( float )Math.Sin( theta );


                /*if ( printStuff && addList.Count > 0 ) {
                    Console.Write( "Adding: " );
                    foreach ( Entity e in addList ) {
                        if ( !( e is Entities.Player ) ) {
                            Console.WriteLine( "NOT PLAYER" );
                        }
                        Console.Write( e + " " );
                    }
                    Console.WriteLine( "" );
                }*/

                
                /*if ( printStuff ) {
                    Console.WriteLine( "(" + checkX + ", " + checkY + ")" );
                }*/
                
                double oldDist = dist;
                dist = getDist( new PointF( checkX, checkY ), new PointF( x2, y2 ) );
                if ( dist > oldDist ) {
                    hasChanged = true; //If it's gotten longer, not shorter - stop.
                }

                /*
                if ( hasChanged && printStuff ) {
                    Console.WriteLine( "\tDist to: (" + x2 + ", " + y2 + ")" );
                    Console.WriteLine( "\tOld: " + oldDist );
                    Console.WriteLine( "\tNew: " + dist );
                    Console.WriteLine( "\tAll done checking" );
                }
                */
            }

            return retList;

        }


        public double getDist( PointF p1, PointF p2 ) {
            return Math.Sqrt( Math.Pow( p1.X - p2.X, 2 ) + Math.Pow( p1.Y - p2.Y, 2 ) );
        }


        //Returns rectangle containing a point
        public RectangleF getRectangleWithPoint( float x, float y ) {
            float rectX = x - ( x % colWidth );
            float rectY = y - ( y % rowHeight );

            return new RectangleF( rectX, rectY, colWidth, rowHeight );
        }



        public List<Entity> checkForCollisions( Entity e, float x, float y, float w, float h ) {

            List<Entity> collisions = new List<Entity>();

            if ( e == null || w == 0 || h == 0 ) return collisions;

            Array rectArray = getIntersectingRectangles( x, y, w, h ).ToArray();

            foreach ( RectangleF rect in rectArray ) {
                foreach ( Entity other in grid[rect].Values ) {
                    if ( other != e ) {
                        if ( checkTwoEntityCollision( x, y, e, other ) ) collisions.Add( other );
                    }
                }
            }

            return collisions;
        }



        public bool checkTwoEntityCollision( float x1, float y1, Entity e1, Entity e2 ) {
            //Console.WriteLine(y1);
            PositionComponent posComp1 = ( PositionComponent )e1.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            PositionComponent posComp2 = ( PositionComponent )e2.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            ColliderComponent colComp1 = ( ColliderComponent )e1.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
            ColliderComponent colComp2 = ( ColliderComponent )e2.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );

            //Separated out for easy changing.
            float e1X = posComp1.x;
            float e1Y = posComp1.y;
            float e2X = posComp2.x;
            float e2Y = posComp2.y;
            float e1Width = colComp1.width;
            float e1Height = colComp1.height;
            float e2Width = colComp2.width;
            float e2Height = colComp2.height;

            //Center width/height values
            if ( e1Width != posComp1.width ) {
                float diff = (posComp1.width - e1Width);
                e1X += diff / 2;
            }
            if ( e2Width != posComp2.width ) {
                float diff = ( posComp2.width - e2Width );
                e2X += diff / 2;
            }
            if ( e1Height != posComp1.height ) {
                float diff = ( posComp1.height - e1Height );
                e1Y += diff / 2;
            }
            if ( e2Height != posComp2.height ) {
                float diff = ( posComp2.height - e2Height );
                e2Y += diff / 2;
            }


            float xbuffer = 0;
            float ybuffer = 0;

            float xDiff = ( float )Math.Abs( x1 - e2X );
            float yDiff = ( float )Math.Abs( y1 - e2Y );

            if ( !GlobalVars.preciseCollisionChecking ) {
                return ( ( xDiff - ( e1Width / 2 + e2Width / 2 ) ) < xbuffer && ( yDiff - ( e1Height / 2 + e2Height / 2 ) ) < ybuffer );
            } else {
                if ( ( xDiff - ( e1Width / 2 + e2Width / 2 ) ) < xbuffer && ( yDiff - ( e1Height / 2 + e2Height / 2 ) ) < ybuffer ) {
                    return handleTransparentCollision( x1, y1, e1, e2 );
                }
                return false;
            }

        }



        //Check to see if only transparent pixels are overlapping.
        public bool handleTransparentCollision( float x1, float y1, Entity e1, Entity e2 ) {

            ColliderComponent firstCol = ( ColliderComponent )e1.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
            ColliderComponent secondCol = ( ColliderComponent )e2.getComponent( GlobalVars.COLLIDER_COMPONENT_NAME );

            PositionComponent firstPos = ( PositionComponent )e1.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            PositionComponent secondPos = ( PositionComponent )e2.getComponent( GlobalVars.POSITION_COMPONENT_NAME );

            //Something's missing a required component
            if ( firstCol == null || secondCol == null || firstPos == null || secondPos == null ) {
                Console.WriteLine( "Missing Required Component for handling collision between: " + e1 + " and " + e2 );
                return false;
            }
            int minAlpha = 50;
            int pixelBuffer = 0; //Extra pixels to check on either side

            Bitmap firstImg = null;
            Bitmap secondImg = null;
            if ( firstCol.drawComponent != null )
                if ( firstCol.drawComponent.getImage() != null )
                    firstImg = ( Bitmap )firstCol.drawComponent.getImage();
            if ( secondCol.drawComponent != null )
                if ( secondCol.drawComponent.getImage() != null )
                    secondImg = ( Bitmap )secondCol.drawComponent.getImage();

            bool alwaysCollide1 = false;
            bool alwaysCollide2 = false;

            if ( firstImg == null )
                alwaysCollide1 = firstCol.collideOnNoSprite;
            else {
                alwaysCollide1 = !firstCol.hasTransparentPixels;
                //Resize as needed
                if ( !firstCol.drawComponent.sizeLocked ) {
                    Bitmap newImg1 = new Bitmap( firstImg, new Size( ( int )firstPos.width, ( int )firstPos.height ) );
                    firstImg = newImg1;
                }
            }
            if ( secondImg == null )
                alwaysCollide2 = secondCol.collideOnNoSprite;
            else {
                alwaysCollide2 = !secondCol.hasTransparentPixels;
                //Resize if needed
                if ( !secondCol.drawComponent.sizeLocked ) {
                    Bitmap newImg2 = new Bitmap( secondImg, new Size( ( int )secondPos.width, ( int )secondPos.height ) );
                    secondImg = newImg2;
                }
            }

            //If they're both always going to collide, just return true. No point wasting time checking.
            if ( alwaysCollide1 && alwaysCollide2 ) {
                return true;
            }


            //Pixel locations to check
            int left1 = 0;
            int left2 = 0;
            int up1 = 0;
            int up2 = 0;
            int hOverlap = 0;
            int vOverlap = 0;

            //Edges of each entitiy
            float leftEdge1 = x1 - firstPos.width / 2;
            float leftEdge2 = secondPos.x - secondPos.width / 2;
            float rightEdge1 = x1 + firstPos.width / 2;
            float rightEdge2 = secondPos.x + secondPos.width / 2;
            float upperEdge1 = y1 - firstPos.height / 2;
            float upperEdge2 = secondPos.y - secondPos.height / 2;
            float lowerEdge1 = y1 + firstPos.height / 2;
            float lowerEdge2 = secondPos.y + secondPos.height / 2;

            if ( leftEdge1 <= leftEdge2 ) {
                left2 = 0;
                left1 = ( int )leftEdge2 - ( int )leftEdge1;
                if ( rightEdge1 <= rightEdge2 ) {
                    hOverlap = ( int )rightEdge1 - ( int )leftEdge2;
                } else {
                    hOverlap = ( int )rightEdge2 - ( int )leftEdge2;
                }
            } else {
                left1 = 0;
                left2 = ( int )leftEdge1 - ( int )leftEdge2;
                if ( rightEdge1 <= rightEdge2 ) {
                    hOverlap = ( int )rightEdge1 - ( int )leftEdge1;
                } else {
                    hOverlap = ( int )rightEdge2 - ( int )leftEdge1;
                }
            }

            if ( upperEdge1 <= upperEdge2 ) {
                up2 = 0;
                up1 = ( int )upperEdge2 - ( int )upperEdge1;

                if ( lowerEdge1 <= lowerEdge2 ) {
                    vOverlap = ( int )lowerEdge1 - ( int )upperEdge2;
                } else {
                    vOverlap = ( int )lowerEdge2 - ( int )upperEdge2;
                }
            } else {
                up1 = 0;
                up2 = ( int )upperEdge1 - ( int )upperEdge2;
                if ( lowerEdge1 <= lowerEdge2 ) {
                    vOverlap = ( int )lowerEdge1 - ( int )upperEdge1;
                } else {
                    vOverlap = ( int )lowerEdge2 - ( int )upperEdge1;
                }
            }

            //Loop through and see if any non-transparent pixels overlap
            for ( int i = -pixelBuffer; i < hOverlap - 1 + pixelBuffer; i++ ) {
                for ( int j = -pixelBuffer; j < vOverlap - 1 + pixelBuffer; j++ ) {

                    //Which pixels to check
                    int xSpot1 = left1 + i;
                    int xSpot2 = left2 + i;
                    int ySpot1 = up1 + j;
                    int ySpot2 = up2 + j;

                    if ( xSpot1 < 0 || xSpot2 < 0 || ySpot1 < 0 || ySpot2 < 0 )
                        continue;
                    if ( !( firstImg == null ) && ( xSpot1 >= firstImg.Width || ySpot1 >= firstImg.Height ) )
                        continue;
                    if ( !( secondImg == null ) && ( xSpot2 >= secondImg.Width || ySpot2 >= secondImg.Height ) )
                        continue;

                    //Check the pixel at each spot, if both are non-transparent, collision.
                    if ( ( alwaysCollide1 || ( firstImg != null && firstImg.GetPixel( xSpot1, ySpot1 ).A >= minAlpha ) ) &&
                        ( alwaysCollide2 || ( secondImg != null && secondImg.GetPixel( xSpot2, ySpot2 ).A >= minAlpha ) ) ) {
                        return true;
                    }

                }
            }
            return false;
        }

        public List<Entity> mergeArrayLists( List<Entity> a1, List<Entity> a2 ) {
            foreach ( Entity o in a2 ) {
                if ( !( a1.Contains( o ) ) ) {
                    a1.Add( o );
                }
            }

            return a1;
        }

        //Draws an APPROXIMATE representation of the grid
        public void Draw( Graphics g ) {

            PositionComponent posComp = ( PositionComponent )level.getPlayer().getComponent( GlobalVars.POSITION_COMPONENT_NAME );

            foreach ( RectangleF r in getIntersectingRectangles( posComp.prevX, posComp.prevY, posComp.prevW, posComp.prevH ) ) {
                g.FillRectangle( new SolidBrush( Color.FromArgb( 120, Color.Green ) ), new Rectangle( ( int )r.X, ( int )r.Y, ( int )r.Width, ( int )r.Height ) );
            }
            foreach ( RectangleF r in grid.Keys ) {
                if ( grid[r].Count > 0 ) g.FillRectangle( new SolidBrush( filledCol ), new Rectangle( ( int )r.X, ( int )r.Y, ( int )r.Width, ( int )r.Height ) );
                //else g.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.Blue)), new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height));

                g.DrawRectangle( borderPen, new Rectangle( ( int )r.X, ( int )r.Y, ( int )r.Width, ( int )r.Height ) );

                //if(grid[r].Count > 0) g.DrawString(grid[r].Count + "", SystemFonts.DefaultFont, Brushes.Black, new RectangleF(r.X + colWidth/3, r.Y + rowHeight/3, 2*colWidth/3, 2*rowHeight/3));

            }
        }

        public void MouseClick( float x, float y ) {

            RectangleF rect = getRectangleWithPoint( x, y );
            /*Array ents = grid[rect].ToArray();
            foreach (Entity e in ents)
            {
                level.removeEntity(e);
            }*/

            string str = "";
            foreach ( Entity ent in grid[rect].Values ) {
                str += ( ": " + ent + " :" );
            }
            Console.WriteLine( "LocGrid. Size:  " + grid[rect].Count + " " + str );

        }
    }
}
