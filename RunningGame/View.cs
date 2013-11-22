using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame
{
    /*Essentially a camera.
     *The level can have multiple views, which can all be displayed in different locations on the game window
     *This could be useful for something like a minimap, multiplayer, etc...
     * All levels will need a main view which will probably be full screen.
     * 
     * Basically, the view holds the game world location of what it is looking at (these are the x, y, width, and height)
     * It also holds the information on where it is displayed in the game window (these are the display variables)
     * 
     * In the draw method, It draws all given entities to one image, the drawImg.
     * Then it draws that image to the main window within the bounds of its display variables
     * 
     * In the update method, if it has an entity that it is set to follow, it will check if it needs to move
     * xBor and yBor are the distance the following entity can get to the edge until the view is pushed
     */
    [Serializable()]
    public class View
    {

        public float x, y, width, height, displayX, displayY, displayWidth, displayHeight, wRatio, hRatio;
        Bitmap drawImg;
        Graphics g;
        public Brush bkgBrush { get; set; }
        public Entity followEntity { get; set; } //Null if not following anything
        PositionComponent followPosComp;
        float xBor { get; set; }
        float yBor { get; set; }

        public Pen GrapplePen = new Pen(Brushes.Red, 3);

        public bool hasBorder = false;
        public Brush borderBrush = Brushes.Brown;
        public float borderSize = 2.0f;

        public Level level;

        public BackgroundEntity bkgEnt = null;

        //Constructor that defaults to a 1:1 ratio for width and height, upper left corner
        public View(float x, float y, float width, float height, Level level)
        {
            Initialize(x, y, width, height, 0, 0, width, height, level, null);
        }
        //Fills in all values
        public View(float x, float y, float width, float height, float displayX, float displayY, float displayWidth, float displayHeight, Level level)
        {
            Initialize(x, y, width, height, displayX, displayY, displayWidth, displayHeight, level, null);
        }
        //Constructor that defaults to a 1:1 ratio for width and height, upper left corner
        public View(float x, float y, float width, float height, Level level, Entity followEntity)
        {
            Initialize(x, y, width, height, 0, 0, width, height, level, followEntity);
        }
        //Fills in all values
        public View(float x, float y, float width, float height, float displayX, float displayY, float displayWidth, float displayHeight, Level level, Entity followEntity)
        {
            Initialize(x, y, width, height, displayX, displayY, displayWidth, displayHeight, level, followEntity);
        }

        public void Initialize(float x, float y, float width, float height, float displayX, float displayY, float displayWidth, float displayHeight, Level level, Entity followEntity)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.displayX = displayX;
            this.displayY = displayY;
            this.displayWidth = displayWidth;
            this.displayHeight = displayHeight;
            this.level = level;

            this.followEntity = followEntity;
            if (followEntity != null)
            {
                followPosComp = (PositionComponent)followEntity.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            }
            xBor = width / 5;
            yBor = height / 5;


            wRatio = displayWidth / width;
            hRatio = displayHeight / height;

            drawImg = new Bitmap((int)displayWidth, (int)displayHeight);
            g = Graphics.FromImage(drawImg);
            bkgBrush = Brushes.DeepSkyBlue;
        }

        public void Draw(Graphics mainG, List<Entity> entities)
        {


            //g.FillRectangle(bkgBrush, new Rectangle(0, 0, (int)width, (int)height)); //Clear
            
            
            //First, if there's a background entity, draw that!
            if (bkgEnt == null)
            {
                foreach (Entity e in entities)
                {
         
                    if (e is BackgroundEntity)
                    {
                        bkgEnt = (BackgroundEntity)e;
                        break;
                    }
                }
            }

            drawEntity(bkgEnt);

            //If there's a grapple, draw it
            if (level.sysManager.grapSystem.isGrappling)
            {
                foreach (Entity e in GlobalVars.nonGroundEntities.Values)
                {
                    if (e is GrappleEntity)
                    {
                        GrappleComponent grapComp = (GrappleComponent)e.getComponent(GlobalVars.GRAPPLE_COMPONENT_NAME);

                        PointF start = grapComp.getFirstPoint();
                        PointF end = grapComp.getLastPoint();

                        // Calc the pos relative to the view
                        start.X -= this.x;
                        start.Y -= this.y;
                        end.X -= this.x;
                        end.Y -= this.y;

                        start.X *= wRatio;
                        start.Y *= hRatio;
                        end.X *= wRatio;
                        end.Y *= hRatio;

                        g.DrawLine(GrapplePen, start, end);
                        break; //Should only be one - this'll save some time.
                    }
                }
            }


            //For all applicable entities (Entities with required components)
            foreach (Entity e in entities)
            {
                if (!(e is BackgroundEntity))
                  drawEntity(e);
            }
            

            
            mainG.DrawImage(drawImg, new Point((int)displayX, (int)displayY)); //Draw the view to the main window
            
            
            //Draw Border
            if (this.hasBorder)
            {
                mainG.DrawRectangle(new Pen(borderBrush, borderSize), new Rectangle((int)(displayX), (int)(displayY),
                (int)(displayWidth), (int)(displayHeight)));
            }
            //look into double buffers, mainG and G are different!
            //use mainG

            if (level.sysManager.drawSystem.getFlashTime() > 0)
            {
                mainG.FillRectangle(level.sysManager.drawSystem.getFlashBrush(), new Rectangle((int)(displayX), (int)(displayY),
                (int)(displayWidth), (int)(displayHeight)));
            }

        }


        public void drawBkgEntity()
        {

            //Pull out all required components
            PositionComponent posComp = (PositionComponent)bkgEnt.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            DrawComponent drawComp = (DrawComponent)bkgEnt.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

            if (isInView(posComp))
            {
                if (g != null)
                {
                    Image img = drawComp.getImage();

                    //Bitmap bigImg = (Bitmap)drawComp.getImage();
                    //Bitmap img = bigImg.Clone(new Rectangle((int)(displayX), (int)(displayY), (int)Math.Ceiling(displayWidth), (int)Math.Ceiling(displayHeight)), bigImg.PixelFormat);

                    //Get center instead of upper left
                    PointF drawPoint = new PointF(displayX, displayY);
                    
                    //drawPoint.X -= (img.Width / 2.0f);
                    //drawPoint.Y -= (img.Height / 2.0f);

                    //drawPoint.X -= this.x;
                    //drawPoint.Y -= this.y;

                    drawPoint.X *= wRatio;
                    drawPoint.Y *= hRatio;


                    lock (img)
                        g.DrawImage(img, drawPoint); //Draw the image to the view.
                }
            }
            
        }

        public void drawEntity(Entity e)
        {
            
            //Pull out all required components
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            DrawComponent drawComp = (DrawComponent)e.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

            if (isInView(posComp))
            {
                if (g != null)
                {
                    Image img = null;
                    //If size is locked, don't resize the image.
                    if (drawComp.sizeLocked && wRatio == 1 && hRatio == 1)
                    {
                        img = drawComp.getImage();
                    }
                    else
                    {
                        Size imageSize = new Size((int)(posComp.width * wRatio), (int)(posComp.height * hRatio));
                        img = new Bitmap(drawComp.getImage(), imageSize);
                    }

                    //Get center instead of upper left
                    PointF drawPoint = posComp.getPointF();
                    drawPoint.X -= (posComp.width / 2.0f);
                    drawPoint.Y -= (posComp.height / 2.0f);

                    drawPoint.X -= this.x;
                    drawPoint.Y -= this.y;

                    drawPoint.X *= wRatio;
                    drawPoint.Y *= hRatio;

                    lock (img)
                        g.DrawImage(img, drawPoint); //Draw the image to the view
                    
                    //Health bar if need be
                    if (e.hasComponent(GlobalVars.HEALTH_COMPONENT_NAME))
                    {
                        HealthComponent healthComp = (HealthComponent)e.getComponent(GlobalVars.HEALTH_COMPONENT_NAME);
                        if (healthComp.healthBar && (healthComp.showBarOnFull || !healthComp.hasFullHealth()))
                        {
                            int barHeight = 4;
                            int ySpace = 3;
                            int xSpace = 0;

                            int xLoc = ((int)Math.Round(posComp.x - posComp.width / 2) + xSpace);
                            int yLoc = ((int)Math.Round(posComp.y - posComp.height / 2) - barHeight - ySpace);
                            int fullWidth = ((int)Math.Round(posComp.width) - 2 * xSpace);

                            Rectangle backRect = new Rectangle(xLoc, yLoc, fullWidth, barHeight);
                            Rectangle foreRect = new Rectangle(xLoc, yLoc, (int)Math.Round(fullWidth * healthComp.getHealthPercentage()), barHeight);

                            g.FillRectangle(healthComp.backHealthBarBrush, backRect);
                            g.FillRectangle(healthComp.foreHealthBarBrush, foreRect);
                            g.DrawRectangle(Pens.Black, backRect);// Border

                        }
                    }
                }
            }
            
        }

        public void Update()
        {
            if (followEntity == null) return;
            moveCamera(followPosComp.x, followPosComp.y);
            /*
            if ((followPosComp.x - x) < xBor)
            {
                this.x = followPosComp.x - xBor;
            }
            if ((followPosComp.y - y) < yBor)
            {
                this.y = followPosComp.y - yBor;
            }
            if ((followPosComp.x - this.x) > (this.width - xBor))
            {
                this.x = (followPosComp.x - this.width + xBor);
            }
            if ((followPosComp.y - this.y) > (this.height - yBor))
            {
                this.y = (followPosComp.y - this.height + yBor);
            }

            //Don't view off of level
            if (this.x < 0) this.x = 0;
            if (this.y < 0) this.y = 0;
            if (this.x + this.width > level.levelWidth) this.x = (level.levelWidth - this.width);
            if (this.y + this.height > level.levelHeight) this.y = (level.levelHeight - this.height);
            */
        }

        public void moveCamera(float newX, float newY)
        {

            if ((newX - x) < xBor)
            {
                this.x = newX - xBor;
            }
            if ((newY - y) < yBor)
            {
                this.y = newY - yBor;
            }
            if ((newX- this.x) > (this.width - xBor))
            {
                this.x = (newX - this.width + xBor);
            }
            if ((newY- this.y) > (this.height - yBor))
            {
                this.y = (newY - this.height + yBor);
            }

            //Don't view off of level
            if (this.x < 0) this.x = 0;
            if (this.y < 0) this.y = 0;
            if (this.x + this.width > level.levelWidth) this.x = (level.levelWidth - this.width);
            if (this.y + this.height > level.levelHeight) this.y = (level.levelHeight - this.height);
        }

        public bool isInView(PositionComponent posComp)
        {

            if ((posComp.x + posComp.width) < x || (posComp.y + posComp.height) < y)
            {
                return false;
            }

            if ((posComp.x - posComp.width) > (x + width) || (posComp.y - posComp.height) > (y + height))
            {
                return false;
            }

            return true;

        }


        public void setFollowEntity(Entity e)
        {
            this.followEntity = e;
            if (followEntity != null)
            {
                followPosComp = (PositionComponent)followEntity.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            }
        }
    }
}
