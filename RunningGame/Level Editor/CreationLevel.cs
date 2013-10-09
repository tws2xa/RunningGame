using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using RunningGame.Components;
using RunningGame.Systems;
using RunningGame.Entities;

namespace RunningGame.Level_Editor
{
    class CreationLevel:Level
    {

        /*
        new public Graphics g { get; set; }
        new public float cameraWidth { get; set; }
        new public float cameraHeight { get; set; }
        new public float levelWidth { get; set; }
        new public float levelHeight {get;set;}
        new public bool paused = false; //Is the game paused?

        new public float fps;


        new public bool levelFullyLoaded = false;
        */

        Random rand; //for creating entitiy ids
        public CreationSystemManager sysManager; //Controls all systems
        bool sysManagerInit = false;

        long prevTicks = DateTime.Now.Ticks;
        long currentTicks;
        long pastTicks;

        public CreationGlobalVars vars;

        public CreationLevel(float levelWidth, float levelHeight, float panelWidth, float panelHeight, Graphics g)
        {

            vars = new CreationGlobalVars();

            rand = new Random();
            this.g = g;

            this.cameraWidth = panelWidth;
            this.cameraHeight = panelHeight;
            this.levelWidth = levelWidth;
            this.levelHeight = levelHeight;

            if(!sysManagerInit) sysManager = new CreationSystemManager(this);

            sysManagerInit = true;

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;
        }

         public CreationLevel(float panelWidth, float panelHeight, string levelFile, Graphics g)
        {

            vars = new CreationGlobalVars();

            rand = new Random();
            this.g = g;

            Bitmap lvlImg = (Bitmap)Bitmap.FromFile(levelFile);

            cameraWidth = panelWidth;
            cameraHeight = panelHeight;
            this.levelWidth = lvlImg.Width*GlobalVars.LEVEL_READER_TILE_WIDTH;
            this.levelHeight = lvlImg.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT;

            if(!sysManagerInit) sysManager = new CreationSystemManager(this);

            sysManagerInit = true;

            LevelImageReader lvlImgReader = new LevelImageReader(this, lvlImg);
            lvlImgReader.readImage(this);

            //levelBeginState = new Dictionary<int, Entity>(entities); //Copy the beginning game state

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;
        }

        public void loadFromPaint(string fileName, Graphics newG)
        {

            sysManagerInit = false;

            this.g = newG;

            Bitmap lvlImg = (Bitmap)Bitmap.FromFile(fileName);

            this.levelWidth = lvlImg.Width*GlobalVars.LEVEL_READER_TILE_WIDTH;
            this.levelHeight = lvlImg.Height*GlobalVars.LEVEL_READER_TILE_HEIGHT;

            sysManager = new CreationSystemManager(this);

            sysManagerInit = true;

            LevelImageReader lvlImgReader = new LevelImageReader(this, lvlImg);
            lvlImgReader.readImage(this);
        }

        //Game logic
        public override void Update()
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
        public override void colliderAdded(Entity e)
        {
            sysManager.colliderAdded(e);
        }


        //Reset the game to it's original startup state
        public override void resetLevel()
        {
            paused = true; // Pause the game briefly
            Entity[] ents = GlobalVars.allEntities.Values.ToArray();
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

        public override void removeAllEntities()
        {
            while(GlobalVars.allEntities.Values.Count > 0)
            {
                Entity e = GlobalVars.allEntities.Values.ToArray()[0];
                //e.Destroy();
            }
            GlobalVars.allEntities.Clear();
        }

        //Input
        public override void KeyDown(KeyEventArgs e)
        {
            sysManager.KeyDown(e);
        }
        public override void KeyUp(KeyEventArgs e)
        {
            sysManager.KeyUp(e);
        }
        public override void KeyPressed(KeyPressEventArgs e)
        {
            sysManager.KeyPressed(e);
        }
        public override void MouseClick(MouseEventArgs e)
        {
            //getCollisionSystem().MouseClick(e.X, e.Y);
            sysManager.MouseClick(e);
        }


        //Draw everything!
        public override void Draw(Graphics g)
        {
            sysManager.Draw(g);
        }

        //Add an entity to the list of entities
        public override void addEntity(int id, Entity e)
        {
            if (!sysManagerInit)
            {
                sysManager = new CreationSystemManager(this);
                sysManagerInit = true;
            }
            GlobalVars.allEntities.Add(id, e);
        }
        public override void removeEntity(Entity e)
        {
            if (e.isStartingEntity)
                GlobalVars.removedStartingEntities.Add(e.randId, e);
            if(e.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME))
                getCollisionSystem().colliderRemoved(e);
            GlobalVars.allEntities.Remove(e.randId);
        }


        //Getters
        public override Dictionary<int, Entity> getEntities()
        {
            return GlobalVars.allEntities;
        }
        public override MovementSystem getMovementSystem()
        {
            return sysManager.moveSystem;
        }
        public override CollisionDetectionSystem getCollisionSystem()
        {
            return sysManager.colSystem;
        }
        public override InputSystem getInputSystem()
        {
            if (sysManager != null)
                return sysManager.inputSystem;
            else return null;
        }
        public override Entity getPlayer()
        {
            return null;
        }

    }
}
