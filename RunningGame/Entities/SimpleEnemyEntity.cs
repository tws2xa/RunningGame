using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities
{
    [Serializable()]
    public class SimpleEnemyEntity:Entity
    {


        public float defaultWidth = 40;
        public float defaultHeight = 30;

        //-------------------------------------------Constructors--------------------------------------------

        public SimpleEnemyEntity(Level level, float x, float y)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            addMyComponents(x, y);
        }
        public SimpleEnemyEntity(Level level, int id, float x, float y)
        {
            this.level = level;

            initializeEntity(id, level);

            addMyComponents(x, y);
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y)
        {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            addComponent(new DrawComponent("RunningGame.Resources.Enemy1.png", "Main", defaultWidth, defaultHeight, true));


            /* ANIMATION COMPONENT - Does it need animating?
             */
            //addComponent(new AnimationComponent(0.0005f));

            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent(new VelocityComponent(GlobalVars.SIMPLE_ENEMY_H_SPEED + new Random().Next(-50, 50), 0));

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE));

            /*GRAVITY COMPONENT - Does it have Gravity?
             */
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));


            /*HEALTH COMPONENT - Does it have health, can it die?
             */
            addComponent(new HealthComponent(100, true, 0, 100.0f));

            /*SIMPLE ENEMY COMPONENT
             */
            addComponent(new SimpleEnemyComponent(GlobalVars.SIMPLE_ENEMY_H_SPEED));

            addComponent(new ScreenEdgeComponent(1, 1, 1, 1));

            /*
             * SQIUSH COMPONENT - Is it squishy?
             */
            //addComponent(new SquishComponent(defaultWidth, defaultHeight, defaultWidth*3.0f, defaultHeight*3.0f, defaultWidth/3.0f, defaultHeight/3.0f));
        }
        
        //You must have this, but it may be empty.
        //What should the entity do in order to revert to its starting state?
        //Common things are:
            //Set position back to startingX and startingY
            //Set velocity to 0 in both directions
        //Note: Some things, like ground, dont move, and really don't need anything here.
        //Note: Some things, like a bullet, won't ever exist at the start of a level, so you could probably leave this empty.
        public override void revertToStartingState()
        {
            PositionComponent posComp = (PositionComponent)getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().teleportToNoCollisionCheck(posComp, posComp.startingX, posComp.startingY);
            VelocityComponent velComp = (VelocityComponent)getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            SimpleEnemyComponent simpEnemyComp = (SimpleEnemyComponent)getComponent(GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME);
            velComp.x = simpEnemyComp.mySpeed;
            velComp.y = 0;
            HealthComponent healthComp = (HealthComponent)getComponent(GlobalVars.HEALTH_COMPONENT_NAME);
            healthComp.restoreHealth();
        }
         
    }
}
