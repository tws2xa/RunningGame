using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;
using RunningGame.Properties;
using System.Diagnostics;

namespace RunningGame
{
    /*
     * A level is basically what it souds like.
     * It keeps track of all the entities within a stage
     * in the game, and updates them. It also holds
     * the SystemManager which is what controls the game systems.
     */
    class Level
    {

        Random rand; //for creating entitiy ids
        Dictionary<int, Entity> entities; //all entities in the level
        public Graphics g { get; set; }
        public float cameraWidth { get; set; }
        public float cameraHeight { get; set; }
        public float levelWidth { get; set; }
        public float levelHeight {get;set;}
        Dictionary<int, Entity> levelBeginState; //Entity states at the level
        public bool paused = false; //Is the game paused?

        public float fps;

        long prevTicks = DateTime.Now.Ticks;
        long currentTicks;
        long pastTicks;

        public SystemManager sysManager; //Controls all systems
        bool sysManagerInit = false;

        public bool levelFullyLoaded = false;

        public Level(float windowWidth, float windowHeight, string levelFile, Graphics g)
        {

            rand = new Random();
            this.g = g;


            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream myStream = myAssembly.GetManifestResourceStream(levelFile);
            Bitmap lvlImg = new Bitmap(myStream);

            cameraWidth = windowWidth;
            cameraHeight = windowHeight;
            this.levelWidth = lvlImg.Width*GlobalVars.LEVEL_READER_TILE_WIDTH;
            this.levelHeight = lvlImg.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT;

            //Entities
            entities = new Dictionary<int, Entity>();
              
            if(!sysManagerInit) sysManager = new SystemManager(this);

            sysManagerInit = true;

            LevelImageReader lvlImgReader = new LevelImageReader(this, lvlImg);
            lvlImgReader.readImage();

            //levelBeginState = new Dictionary<int, Entity>(entities); //Copy the beginning game state

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;
        }


        //Game logic
        public void Update()
        {

            //Time in seconds between frames
            currentTicks = DateTime.Now.Ticks;
            pastTicks = currentTicks - prevTicks;
            prevTicks = currentTicks;

            float deltaTime = (float)(TimeSpan.FromTicks(pastTicks).TotalSeconds);
            fps = (1 / deltaTime);

            if (levelFullyLoaded && !paused)
            {
                sysManager.Update(deltaTime); //Update systems
            }

        }

        //When an entity is given a collider - notify collider system
        public void colliderAdded(Entity e)
        {
            sysManager.colliderAdded(e);
        }


        //Reset the game to it's original startup state
        public void resetLevel()
        {
            paused = true; // Pause the game briefly
            Entity[] ents = entities.Values.ToArray();
            for (int i = 0; i < ents.Length; i++)
            {
                if (ents[i].isStartingEntity)
                    ents[i].revertToStartingState();
                else
                {
                    removeEntity(ents[i]);
                }
            }
            foreach (Entity e in GlobalVars.removedStartingEntities.Values)
            {
                e.revertToStartingState();
                addEntity(e.randId, e);
                if (e.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME))
                    colliderAdded(e);
            }
            GlobalVars.removedStartingEntities.Clear();
            paused = false; //Restart the game  
            
        }

        public void removeAllEntities()
        {
            while(entities.Values.Count > 0)
            {
                Entity e = entities.Values.ToArray()[0];
                //e.Destroy();
            }
            entities.Clear();
        }

        //Input
        public void KeyDown(KeyEventArgs e)
        {
            sysManager.KeyDown(e);
        }
        public void KeyUp(KeyEventArgs e)
        {
            sysManager.KeyUp(e);
        }
        public void KeyPressed(KeyPressEventArgs e)
        {
            sysManager.KeyPressed(e);
        }
        public void MouseClick(MouseEventArgs e)
        {
            //getCollisionSystem().MouseClick(e.X, e.Y);
            sysManager.MouseClick(e);
        }


        //Draw everything!
        public void Draw(Graphics g)
        {
            sysManager.Draw(g);
            g.DrawString(fps.ToString("F") + "", SystemFonts.DefaultFont, Brushes.Black, new RectangleF(10, 10, cameraWidth-20, cameraHeight-20));
        }

        //Add an entity to the list of entities
        public void addEntity(int id, Entity e)
        {
            if (!sysManagerInit)
            {
                sysManager = new SystemManager(this);
                sysManagerInit = true;
            }
            entities.Add(id, e);
        }
        public void removeEntity(Entity e)
        {
            if (e.isStartingEntity)
                GlobalVars.removedStartingEntities.Add(e.randId, e);
            if(e.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME))
                getCollisionSystem().colliderRemoved(e);
            entities.Remove(e.randId);
        }


        //Getters
        public Dictionary<int, Entity> getEntities() {
            return entities;
        }
        public MovementSystem getMovementSystem()
        {
            return sysManager.moveSystem;
        }
        public CollisionDetectionSystem getCollisionSystem()
        {
            return sysManager.colSystem;
        }
        public Entity getPlayer()
        {
            foreach (Entity e in entities.Values)
            {
                if (e.hasComponent(GlobalVars.PLAYER_COMPONENT_NAME)) return e;
            }

            return null;
        }

    }
}
