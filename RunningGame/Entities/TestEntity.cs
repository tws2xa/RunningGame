using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;

namespace RunningGame.Entities
{

    /*
     * Meet the ever so handy test entity!
     * It can be whatever you want it to be.
     */

    class TestEntity:Entity
    {

        float defaultWidth = 10;
        float defaultHeight = 10;

        float startingX;
        float startingY;

        public TestEntity(Level level, float x, float y)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);
            
            startingX = x;
            startingY = y;

            addMyComponents(x, y);
        }

        public TestEntity(Level level, int id, float x, float y)
        {
            this.level = level;

            initializeEntity(id, level);

            startingX = x;
            startingY = y;

            addMyComponents(x, y);
        }

        public void addMyComponents(float x, float y)
        {
            //Position Component
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            //Draw component
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream myStream = myAssembly.GetManifestResourceStream("RunningGame.Resources.WhiteSquare.bmp");
            Bitmap sprite = new Bitmap(myStream);
            addComponent(new DrawComponent(sprite, defaultWidth, defaultHeight, true));

            //Velocity Component
            addComponent(new VelocityComponent(0, 0));

            //Player Component
            //addComponent(new PlayerInputComponent());

            //Collider
            addComponent(new ColliderComponent(this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE));

            //Gravity Component
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));
            

        }

        /*
        public override Entity CopyStartingState()
        {
            TestEntity newEnt = new TestEntity(level, randId, startingX, startingY);
            return newEnt;
        }
        */

        
        public override void revertToStartingState()
        {
            PositionComponent posComp = (PositionComponent)this.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().changePosition(posComp, startingX, startingY);
            level.getMovementSystem().changeSize(posComp, defaultWidth, defaultHeight);

            VelocityComponent velComp = (VelocityComponent)this.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            velComp.y = 0;
        }
         
    }
}
