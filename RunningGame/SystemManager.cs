using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Systems;
using System.Windows.Forms;

namespace RunningGame
{

    /* This is the class that handles all the different systems.
     * It's basically a convinient way to initialize, update, and control
     * them from a central area.
     */

    class SystemManager
    {

        Level level;

        public DrawSystem drawSystem;
        public GravitySystem gravSystem;
        public MovementSystem moveSystem;
        public PlayerSystem playerSystem;
        public CollisionDetectionSystem colSystem;
        public HealthSystem healthSystem;
        public AnimationSystem animSystem;
        public SquishSystem squishSystem;
        public InputSystem inputSystem;
        public DebugSystem debugSystem;

        public SystemManager(Level level)
        {

            this.level = level;

            initializeSystems();

        }

        // Create all systems
        public void initializeSystems()
        {
            gravSystem = new GravitySystem(level);
            moveSystem = new MovementSystem(level);
            playerSystem = new PlayerSystem(level);
            colSystem = new CollisionDetectionSystem(level);
            drawSystem = new DrawSystem(level.g, level);
            healthSystem = new HealthSystem(level);
            animSystem = new AnimationSystem(level);
            squishSystem = new SquishSystem(level);
            inputSystem = new InputSystem(level);

            debugSystem = new DebugSystem(level);

        }


        //Game Logic Stuff
        public void Update(float deltaTime)
        {
            moveSystem.Update(deltaTime);
            playerSystem.Update(deltaTime);
            colSystem.Update(deltaTime);
            gravSystem.Update(deltaTime);
            drawSystem.Update(deltaTime);
            healthSystem.Update(deltaTime);
            animSystem.Update(deltaTime);
            squishSystem.Update(deltaTime);
            inputSystem.Update(deltaTime);

            debugSystem.Update(deltaTime);
            
        }

        //Notify collider system of a new collider
        public void colliderAdded(Entity e)
        {
            colSystem.colliderAdded(e);
        }

        //Input
        public void KeyDown(KeyEventArgs e)
        {
            playerSystem.KeyDown(e);
            inputSystem.KeyDown(e);
        }
        public void KeyUp(KeyEventArgs e)
        {
            playerSystem.KeyUp(e);
            inputSystem.KeyUp(e);
        }
        public void KeyPressed(KeyPressEventArgs e)
        {
            playerSystem.KeyPressed(e);
        }
        public void MouseClick(MouseEventArgs e)
        {
            //colSystem.MouseClick(e.X, e.Y);
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
