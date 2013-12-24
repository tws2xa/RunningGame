using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;
using System.Collections;

namespace RunningGame.Entities {
    class spawnBlockEntity : Entity {
        float defaultWidth = 18;
        float defaultHeight = 18;

        //string blockAnimationName = "blockAnimation";

        public spawnBlockEntity(Level level, float x, float y) {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            addMyComponents(x, y);
        }

        public void addMyComponents(float x, float y) {

            this.updateOutOfView = true;

            //Position Component
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);

            //Draw component
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            drawComp.addSprite("Artwork.Foreground.BlockSquare", "RunningGame.Resources.Artwork.Foreground.BlockSquare.png", "Main");
            drawComp.setSprite("Main");


            //Velocity Component
            addComponent(new VelocityComponent(0, 0), true);

            //Collider
            addComponent(new ColliderComponent(this, GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE), true);

            //Gravity Component
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY), true);

            //Spawn Block Component
            addComponent(new SpawnBlockComponent(), true);

            //Off side of screen
            addComponent(new ScreenEdgeComponent(3, 3, 3, 3), true);
        }

        public override void revertToStartingState() {
            PositionComponent posComp = (PositionComponent)this.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().changePosition(posComp, posComp.startingX, posComp.startingY, true);
            level.getMovementSystem().changeSize(posComp, posComp.startingWidth, posComp.startingHeight);

            VelocityComponent velComp = (VelocityComponent)this.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            velComp.y = 0;
        }
    }
}
