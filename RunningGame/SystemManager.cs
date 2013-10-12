using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Systems;
using System.Windows.Forms;

namespace RunningGame
{

    /* 
     * This is the class that handles all the different systems.
     * It's basically a convinient way to initialize, update, and control
     * them from a central area.
     */
    [Serializable()]
    public class SystemManager
    {

        Level level;

        public DrawSystem drawSystem;
        public GravitySystem gravSystem;
        public MovementSystem moveSystem;
        public PlayerMovementSystem playerSystem;
        public CollisionDetectionSystem colSystem;
        public HealthSystem healthSystem;
        public AnimationSystem animSystem;
        public SquishSystem squishSystem;
        public InputSystem inputSystem;
        public DebugSystem debugSystem;
        public ScreenEdgeSystem scrEdgeSystem;
        public SwitchListenerSystem slSystem;
        public GlideSystem glideSystem;

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
            playerSystem = new PlayerMovementSystem(level);
            colSystem = new CollisionDetectionSystem(level);
            drawSystem = new DrawSystem(level.g, level);
            healthSystem = new HealthSystem(level);
            animSystem = new AnimationSystem(level);
            squishSystem = new SquishSystem(level);
            inputSystem = new InputSystem(level);
            scrEdgeSystem = new ScreenEdgeSystem(level);
            slSystem = new SwitchListenerSystem(level);
            glideSystem = new GlideSystem(level);

            debugSystem = new DebugSystem(level);

        }


        //Game Logic Stuff
        public void Update(float deltaTime)
        {
            moveSystem.Update(deltaTime);
            scrEdgeSystem.Update(deltaTime);
            playerSystem.Update(deltaTime);
            colSystem.Update(deltaTime);
            gravSystem.Update(deltaTime);
            drawSystem.Update(deltaTime);
            healthSystem.Update(deltaTime);
            animSystem.Update(deltaTime);
            squishSystem.Update(deltaTime);
            inputSystem.Update(deltaTime);
            slSystem.Update(deltaTime);
            debugSystem.Update(deltaTime);
            glideSystem.Update(deltaTime);
            
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
        public void MouseMoved(MouseEventArgs e)
        {
            inputSystem.MouseMoved(e);
        }

        //Any systems that require drawing
        public void Draw(System.Drawing.Graphics g)
        {
            drawSystem.Draw(g);
            //colSystem.Draw(g);
        }


    }
}
