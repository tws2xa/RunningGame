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
    class spawnBlockEntity: Entity
    {
        float defaultWidth = 18;
        float defaultHeight = 18;

        //string blockAnimationName = "blockAnimation";

        public spawnBlockEntity(Level level, float x, float y)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);
            
            addMyComponents(x, y);
        }
        
        public void addMyComponents(float x, float y)
        {

            this.updateOutOfView = true;

            //Position Component
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));

            //Draw component
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true));
            drawComp.addSprite("Artwork.Foreground.BlockSquare", "RunningGame.Resources.Artwork.Foreground.BlockSquare.png", "Main");
            drawComp.setSprite("Main");
           

            //Velocity Component
            addComponent(new VelocityComponent(0, 0));

            //Collider
            addComponent(new ColliderComponent(this, GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE));

            //Gravity Component
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));

            //Spawn Block Component
            addComponent(new SpawnBlockComponent());

            //Off side of screen
            addComponent(new ScreenEdgeComponent(3, 3, 3, 3));
        }

        public override void revertToStartingState()
        {
            PositionComponent posComp = (PositionComponent)this.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().changePosition(posComp, posComp.startingX, posComp.startingY, true);
            level.getMovementSystem().changeSize(posComp, posComp.startingWidth, posComp.startingHeight);

            VelocityComponent velComp = (VelocityComponent)this.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            velComp.y = 0;
        }
    }
}
