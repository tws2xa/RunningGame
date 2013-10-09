using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RunningGame.Systems;

namespace RunningGame.Level_Editor
{
    class CreationSystemManager
    {
        
        CreationLevel level;

        public DrawSystem drawSystem;
        public MovementSystem moveSystem;
        public CollisionDetectionSystem colSystem;
        public InputSystem inputSystem;

        public CreationSystemManager(CreationLevel level)
        {

            this.level = level;
            initializeSystems();

        }

        // Create all systems
        public void initializeSystems()
        {
            moveSystem = new MovementSystem(level);
            colSystem = new CollisionDetectionSystem(level);
            drawSystem = new DrawSystem(level.g, level);
            inputSystem = new InputSystem(level);
        }


        //Game Logic Stuff
        public void Update(float deltaTime)
        {
            moveSystem.Update(deltaTime);
            colSystem.Update(deltaTime);
            drawSystem.Update(deltaTime);
            inputSystem.Update(deltaTime);
            
        }

        //Notify collider system of a new collider
        public void colliderAdded(Entity e)
        {
            colSystem.colliderAdded(e);
        }

        //Input
        public void KeyDown(KeyEventArgs e)
        {
            inputSystem.KeyDown(e);
        }
        public void KeyUp(KeyEventArgs e)
        {
            inputSystem.KeyUp(e);
        }
        public void KeyPressed(KeyPressEventArgs e)
        {
            //Derp
        }
        public void MouseClick(MouseEventArgs e)
        {
            //colSystem.MouseClick(e.X, e.Y); //This'll allow you to click and see which entities are in a cell
            inputSystem.MouseClick(e);
        }

        //Any systems that require drawing
        public void Draw(System.Drawing.Graphics g)
        {
            drawSystem.Draw(g);
            //colSystem.Draw(g);
        }

    }
}
