using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;
using System.Collections;

namespace RunningGame.Entities
{
    /*
     * This is the player - yo.
     * It's a lot like other entities, the only special things
     * are the faceLeft, and faceRight methods.
     * These change which way the player sprite is looking.
     */
    [Serializable()]
    public class Player : Entity
    {

        float defaultWidth = 39;
        float defaultHeight = 49;
        
        string rightImageName = "right";
        string leftImageName = "left";

        string walkLeft = "normLeft";
        string walkRight = "normRight";

        string walkBlueLeft = "bL";
        string walkBlueRight = "bR";

        string walkOrangeLeft = "oL";
        string walkOrangeRight = "oR";
        
        string walkPurpleLeft = "pL";
        string walkPurpleRight = "pE";

        string activeLeftImage;
        string activeRightImage;
        
        public Player() { }

        public Player(Level level, float x, float y)
        {
            this.level = level;
            this.depth = 1;
            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            activeLeftImage = walkLeft;
            activeRightImage = walkRight;

            addMyComponents(x, y);

        }

        public Player(Level level, int id, float x, float y)
        {
            this.level = level;

            initializeEntity(id, level);

            addMyComponents(x, y);
        }



        public void addMyComponents(float x, float y)
        {

            //Position Component
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));

            //Velocity Component
            addComponent(new VelocityComponent(0, 0));

            //Draw component
            DrawComponent drawComp = new DrawComponent((int)defaultWidth, (int)defaultHeight, level, false);

            drawComp.addSprite("Artwork.Creatures.player1", "RunningGame.Resources.Artwork.Creatures.player1.png", rightImageName);
            drawComp.addSprite("Artwork.Creatures.player1", "RunningGame.Resources.Artwork.Creatures.player2.png", leftImageName);
            drawComp.rotateFlipSprite(leftImageName, RotateFlipType.RotateNoneFlipX);
            addComponent(drawComp);

            addWalkAnimation("RunningGame.Resources.Artwork.Creatures.player", walkLeft, walkRight, drawComp);
            addWalkAnimation("RunningGame.Resources.Artwork.Creatures.PlayerBlue", walkBlueLeft, walkBlueRight, drawComp);
            addWalkAnimation("RunningGame.Resources.Artwork.Creatures.PlayerOrange", walkOrangeLeft, walkOrangeRight, drawComp);
            addWalkAnimation("RunningGame.Resources.Artwork.Creatures.PlayerPurple", walkPurpleLeft, walkPurpleRight, drawComp);

            setNormalImage();


            //Animation Component
            AnimationComponent animComp = (AnimationComponent)addComponent(new AnimationComponent(GlobalVars.playerAnimatonSpeed));
            animComp.animationOn = false;
            
            //Player Component
            addComponent(new PlayerComponent());

            //Player Input Component
            addComponent(new PlayerInputComponent(this));

            //Collider
            addComponent(new ColliderComponent(this, GlobalVars.PLAYER_COLLIDER_TYPE));

            //Squish Component
            SquishComponent sqComp = (SquishComponent)addComponent(new SquishComponent(defaultWidth, defaultHeight, defaultWidth * 1.2f, defaultHeight * 1.2f, defaultWidth / 2f, defaultHeight / 2f, defaultWidth*defaultHeight*1.1f, defaultWidth*defaultHeight/1.5f));
            sqComp.maxHeight = defaultHeight;
            sqComp.maxWidth = defaultWidth * 1.1f;
            sqComp.minHeight = defaultHeight / 1.1f;
            sqComp.minWidth = defaultWidth / 1.1f;
            sqComp.maxSurfaceArea = defaultHeight * defaultWidth * 1.1f;
            sqComp.minSurfaceArea = defaultHeight * defaultWidth / 1.1f;

            //Gravity Component
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));

            //Health Component
            addComponent(new HealthComponent(100, true, 1, 0.5f));

            //Screen Edge Stop/Wrap/End Level
            addComponent(new ScreenEdgeComponent(1, 4, 1, 0));

        }


        public void addWalkAnimation(string baseName, string spriteNameLeft, string spriteNameRight, DrawComponent drawComp)
        {
            string imgOne = baseName + "1.png";
            string imgTwo = baseName + "2.png";
            List<string> walkAnimation = new List<string>
            {
                imgOne,
                imgTwo
            };
            List<string> walkDefaults = new List<string>()
            {
                imgOne,
                imgTwo
            };

            drawComp.addAnimatedSprite(walkAnimation, walkDefaults, spriteNameRight);
            drawComp.addAnimatedSprite(walkAnimation, walkDefaults, spriteNameLeft);

            drawComp.rotateFlipSprite(spriteNameLeft, RotateFlipType.RotateNoneFlipX);

        }

        public override void revertToStartingState()
        {
            PositionComponent posComp = (PositionComponent)this.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().teleportToNoCollisionCheck(posComp, posComp.startingX, posComp.startingY);
            level.getMovementSystem().changeSize(posComp, posComp.startingWidth, posComp.startingHeight);

            VelocityComponent velComp = (VelocityComponent)this.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            velComp.y = 0;

            AnimationComponent animComp = (AnimationComponent)this.getComponent(GlobalVars.ANIMATION_COMPONENT_NAME);
            animComp.animationOn = false;

            HealthComponent healthComp = (HealthComponent)this.getComponent(GlobalVars.HEALTH_COMPONENT_NAME);
            healthComp.restoreHealth();

            if (!hasComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME))
            {
                addComponent(new PlayerInputComponent(this));
            }

            if (!hasComponent(GlobalVars.GRAVITY_COMPONENT_NAME))
            {
                addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));
            }
        }
        

        public void faceRight()
        {
            
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.setSprite(activeRightImage);
        }
        public void faceLeft()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.setSprite(activeLeftImage);
        }

        public bool isLookingLeft()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            return (drawComp.activeSprite == activeLeftImage);
        }

        public bool isLookingRight()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            return (drawComp.activeSprite == activeRightImage);
        }



        public void setNormalImage()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.setSprite(walkRight);

            activeLeftImage = walkLeft;
            activeRightImage = walkRight;
        }
        public void setBlueImage()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.setSprite(walkBlueRight);

            activeLeftImage = walkBlueLeft;
            activeRightImage = walkBlueRight;
        }
        public void setOrangeImage()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.setSprite(walkOrangeRight);

            activeLeftImage = walkOrangeLeft;
            activeRightImage = walkOrangeRight;
        }
        public void setPurpleImage()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.setSprite(walkRight);

            activeLeftImage = walkPurpleLeft;
            activeRightImage = walkPurpleRight;
        }



        public void faceDirection(bool right)
        {
            if (right) this.faceRight();
            else this.faceLeft();
        }
    }
}
