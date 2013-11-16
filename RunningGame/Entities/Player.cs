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

        float defaultWidth = 20;
        float defaultHeight = 20;

        string rightImageName = "right";
        string leftImageName = "left";

        string blinkLeft = "binkLeft";
        string blinkRight = "blinkRight";

        public Player() { }

        public Player(Level level, float x, float y)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

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

            drawComp.addSprite("Artwork.Creatures.Player1", "RunningGame.Resources.Artwork.Creatures.Player111.png", rightImageName);
            drawComp.addSprite("Artwork.Creatures.Player1", "RunningGame.Resources.Artwork.Creatures.Player111.png", leftImageName);
            drawComp.rotateFlipSprite(leftImageName, RotateFlipType.RotateNoneFlipX);
            addComponent(drawComp);


            List<string> blinkAnimation = new List<string>
            {
                "Artwork.Creatures.Player1",
                "Artwork.Creatures.Player2"
            };
            List<string> blinkDefaults = new List<string>()
            {
                "RunningGame.Resources.Artwork.Creatures.Player111.png",
                "RunningGame.Resources.Artwork.Creatures.Player211.png"
            };

            drawComp.addAnimatedSprite(blinkAnimation, blinkDefaults, blinkRight);
            drawComp.addAnimatedSprite(blinkAnimation, blinkDefaults, blinkLeft);

            drawComp.rotateFlipSprite(blinkLeft, RotateFlipType.RotateNoneFlipX);

            drawComp.setSprite(blinkRight);

            //Animation Component
            AnimationComponent animComp = (AnimationComponent)addComponent(new AnimationComponent(0.0005f));
            animComp.pauseTimeAfterCycle = 5.0f;

            //Player Component
            addComponent(new PlayerComponent());

            //Player Input Component
            addComponent(new PlayerInputComponent(this));

            //Collider
            addComponent(new ColliderComponent(this, GlobalVars.PLAYER_COLLIDER_TYPE));

            //Squish Component
            addComponent(new SquishComponent(defaultWidth, defaultHeight, defaultWidth * 3, defaultHeight * 3, defaultWidth / 3f, defaultHeight / 3f, defaultWidth*defaultHeight*1.5f, defaultWidth*defaultHeight/2f));

            //Gravity Component
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));

            //Health Component
            addComponent(new HealthComponent(100, true, 1, 0.5f));

            //Screen Edge Stop/Wrap/End Level
            addComponent(new ScreenEdgeComponent(1, 4, 1, 0));

        }
        
        public override void revertToStartingState()
        {
            PositionComponent posComp = (PositionComponent)this.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().teleportToNoCollisionCheck(posComp, posComp.startingX, posComp.startingY);
            level.getMovementSystem().changeSize(posComp, posComp.startingWidth, posComp.startingHeight);

            VelocityComponent velComp = (VelocityComponent)this.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            velComp.y = 0;

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
            if (drawComp.activeSprite == leftImageName || drawComp.activeSprite == rightImageName)
                drawComp.setSprite(rightImageName);
            else
                drawComp.setSprite(blinkRight);
        }
        public void faceLeft()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            if (drawComp.activeSprite == leftImageName || drawComp.activeSprite == rightImageName)
                drawComp.setSprite(leftImageName);
            else
                drawComp.setSprite(blinkLeft);
        }

        public bool isLookingLeft()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            return (drawComp.activeSprite == leftImageName || drawComp.activeSprite == blinkLeft);
        }

        public bool isLookingRight()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            return (drawComp.activeSprite == rightImageName || drawComp.activeSprite == blinkRight);
        }
    }
}
