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

        bool hasDecreasedQuality = false;

        public Pen GrapplePen = new Pen(Brushes.Red, 3);

        public bool hasBorder = false;
        public bool borderFade = true;
        int amntSolid = 5; //How many layers to leave solid if fading
        public SolidBrush borderBrush = (SolidBrush)Brushes.Brown;
        public float borderSize = 2.0f;

        public Level level;

        public BackgroundEntity bkgEnt = null;

        public bool seperateStaticObjImage = false; //If false, static objects get drawn to bkg - otherwise a seperate image.
        public Bitmap staticObjImg = null;

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

            drawImg = new Bitmap((int)level.levelWidth, (int)level.levelHeight);
            g = Graphics.FromImage(drawImg);
            bkgBrush = Brushes.DeepSkyBlue;
        }

        public void Draw(Graphics mainG, List<Entity> entities)
        {


            //g.FillRectangle(bkgBrush, new Rectangle(0, 0, (int)width, (int)height)); //Clear

            if (!hasDecreasedQuality)
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor; // or NearestNeighbour
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

                mainG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor; // or NearestNeighbour
                mainG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                mainG.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                mainG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                mainG.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                
                hasDecreasedQuality = true;
            }

            //Find background Entity first if need be
            if (bkgEnt == null)
            {

                foreach (Entity e in entities)
                {

                    if (e is BackgroundEntity)
                    {
                        bkgEnt = (BackgroundEntity)e; //Find background entity
                    }
                }
            }

            if (staticObjImg == null)
            {
                if (seperateStaticObjImage)
                {
                    staticObjImg = new Bitmap((int)Math.Ceiling(level.levelWidth), (int)Math.Ceiling(level.levelHeight));
                }
                else
                {
                    foreach (Entity e in entities)
                    {

                        if (e is BackgroundEntity)
                        {
                            if(bkgEnt == null)
                                bkgEnt = (BackgroundEntity)e; //Find background entity
                            DrawComponent bkgDraw = (DrawComponent)bkgEnt.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
                            staticObjImg = (Bitmap)bkgDraw.getImage();
                        }
                    }
                }

                //Draw static entities onto background
                foreach(Entity ent in GlobalVars.groundEntities.Values) {

                    DrawComponent grnDraw = (DrawComponent)ent.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

                    /*DrawComponent bkgDraw = (DrawComponent)bkgEnt.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

                    PositionComponent posComp = (PositionComponent)ent.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    PointF drawPoint = posComp.getPointF();
                    drawPoint.X -= (posComp.width / 2.0f);
                    drawPoint.Y -= (posComp.height / 2.0f);

                    Graphics graph = Graphics.FromImage(bkgDraw.getImage());*/


                    PositionComponent posComp = (PositionComponent)ent.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    PointF drawPoint = posComp.getPointF();
                    drawPoint.X -= (posComp.width / 2.0f);
                    drawPoint.Y -= (posComp.height / 2.0f);

                    Graphics graph = Graphics.FromImage(staticObjImg);
                    lock (grnDraw.getImage())
                    {
                        graph.DrawImageUnscaled(grnDraw.getImage(), new Point((int)drawPoint.X, (int)drawPoint.Y)); //Draw the image to the view
                    }
                    grnDraw.needRedraw = false;
                            
                }
                
            }

            //First, if there's a background entity, draw that!
            if (bkgEnt != null)
                drawBkgEntity(bkgEnt);
            if (seperateStaticObjImage)
            {
                drawStaticObjImage();
            }

            
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
                        /*
                        // Calc the pos relative to the view
                        start.X -= this.x;
                        start.Y -= this.y;
                        end.X -= this.x;
                        end.Y -= this.y;

                        start.X *= wRatio;
                        start.Y *= hRatio;
                        end.X *= wRatio;
                        end.Y *= hRatio;
                        */
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
            
            //mainG.DrawImage(drawImg, new Point((int)displayX, (int)displayY)); //Draw the view to the main window
            //mainG.DrawImageUnscaled(drawImg, new Point((int)displayX, (int)displayY)); //Draw the view to the main window
            mainG.DrawImage(drawImg, new RectangleF(displayX, displayY, displayWidth, displayHeight), new RectangleF(x, y, width, height), GraphicsUnit.Pixel);
                       
            
            //Draw Border
            if (this.hasBorder)
            {
                if (!this.borderFade)
                {
                    mainG.DrawRectangle(new Pen(borderBrush, borderSize), new Rectangle((int)(displayX), (int)(displayY),
                    (int)(displayWidth), (int)(displayHeight)));
                }
                else
                {
                    int alphaDiff = (int)Math.Ceiling(255.0f / (borderSize - amntSolid)); //How much to decrease alpha per layer
                    //Draw the solid bit
                    //mainG.DrawRectangle(new Pen(borderBrush, amntSolid), new Rectangle((int)(displayX), (int)(displayY),
                    //(int)(displayWidth), (int)(displayHeight)));

                    int alphaVal = 255;
                    alphaVal -= alphaDiff;

                    for (int i = 0; i <= borderSize; i++)
                    {
                        if (alphaVal < 0) alphaVal = 0;
                        Color tmpCol = Color.FromArgb(alphaVal, borderBrush.Color);
                        Pen pen = new Pen(new SolidBrush(tmpCol), 1);
                        mainG.DrawRectangle(pen, new Rectangle((int)(displayX+i), (int)(displayY+i),
                            (int)(displayWidth-2*i), (int)(displayHeight-2*i)));alphaVal -= alphaDiff;
                        alphaVal -= alphaDiff;
                        if (alphaVal < 0) alphaVal = 0;
                    }
                }
            }
            //look into double buffers, mainG and G are different!
            //use mainG
            if (level.sysManager.drawSystem.getFlashTime() > 0)
            {
                mainG.FillRectangle(level.sysManager.drawSystem.getFlashBrush(), new Rectangle((int)(displayX), (int)(displayY),
                (int)(displayWidth), (int)(displayHeight)));
            }

        }

        public void drawStaticObjImage()
        {
            if(staticObjImg == null) return;
            g.DrawImage(staticObjImg, new RectangleF(x, y, width, height), new RectangleF(x, y, width, height), GraphicsUnit.Pixel);
        }
        public void drawBkgEntity(Entity e)
        {
            //Pull out all required components
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            DrawComponent drawComp = (DrawComponent)e.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

            if (isInView(posComp))
            {
                if (g != null)
                {
                    Image img = drawComp.getImage();

                    //Get center instead of upper left
                    PointF drawPoint = posComp.getPointF();
                    drawPoint.X -= (posComp.width / 2.0f);
                    drawPoint.Y -= (posComp.height / 2.0f);

                    drawPoint.X -= this.x;
                    drawPoint.Y -= this.y;

                    drawPoint.X *= wRatio;
                    drawPoint.Y *= hRatio;


                    lock (img)
                    {
                        //g.DrawImage(img, new RectangleF(0, 0, width, height), new RectangleF(x, y, width, height), GraphicsUnit.Pixel);
                       g.DrawImage(img, new RectangleF(x, y, width, height), new RectangleF(x, y, width, height), GraphicsUnit.Pixel);
                    }
                }
            }

        }

        public void drawEntity(Entity e)
        {
            
            //Pull out all required components
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            DrawComponent drawComp = (DrawComponent)e.getComponent(GlobalVars.DRAW_COMPONENT_NAME);

            if (drawComp.needRedraw)
            {

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

                        /*
                        //Get center instead of upper left
                        PointF drawPoint = posComp.getPointF();
                        drawPoint.X -= (posComp.width / 2.0f);
                        drawPoint.Y -= (posComp.height / 2.0f);

                        drawPoint.X -= this.x;
                        drawPoint.Y -= this.y;

                        drawPoint.X *= wRatio;
                        drawPoint.Y *= hRatio;
                         */

                        PointF drawPoint = posComp.getPointF();
                        drawPoint.X -= (posComp.width / 2.0f);
                        drawPoint.Y -= (posComp.height / 2.0f);

                        lock (img)
                        {
                            g.DrawImageUnscaled(img, new Point((int)drawPoint.X, (int)drawPoint.Y)); //Draw the image to the view
                        }
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
                                g.DrawRectangle(Pens.Black, backRect); //Border

                            }
                        }

                        if (e is BasicGround & bkgEnt != null)
                        {
                            DrawComponent bkgDraw = (DrawComponent) bkgEnt.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
                            Graphics graph = Graphics.FromImage(bkgDraw.getImage());
                            lock (img)
                            {
                                graph.DrawImageUnscaled(img, new Point((int)drawPoint.X, (int)drawPoint.Y)); //Draw the image to the view
                            }
                            drawComp.needRedraw = false;
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

        public void centerOnFollowEntity()
        {
            moveCamera(followPosComp.x, followPosComp.y);
        }
    }
}
