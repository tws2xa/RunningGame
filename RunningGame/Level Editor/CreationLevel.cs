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

namespace RunningGame.Level_Editor {

    [Serializable()]
    public class CreationLevel : Level {

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
        new public CreationSystemManager sysManager; //Controls all systems
        bool sysManagerInit = false;

        long prevTicks = DateTime.Now.Ticks;
        long currentTicks;
        long pastTicks;

        public CreationGlobalVars vars;

        public CreationLevel(float levelWidth, float levelHeight, float panelWidth, float panelHeight, Graphics g) {

            vars = new CreationGlobalVars();

            rand = new Random();
            this.g = g;

            this.cameraWidth = panelWidth;
            this.cameraHeight = panelHeight;
            this.levelWidth = levelWidth;
            this.levelHeight = levelHeight;

            if (!sysManagerInit) sysManager = new CreationSystemManager(this);

            sysManagerInit = true;

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;
        }

        public CreationLevel(float panelWidth, float panelHeight, string levelFile, Graphics g) {

            vars = new CreationGlobalVars();

            rand = new Random();
            this.g = g;

            Bitmap lvlImg = (Bitmap)Bitmap.FromFile(levelFile);

            cameraWidth = panelWidth;
            cameraHeight = panelHeight;
            this.levelWidth = lvlImg.Width * GlobalVars.LEVEL_READER_TILE_WIDTH;
            this.levelHeight = lvlImg.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT;

            if (!sysManagerInit) sysManager = new CreationSystemManager(this);

            sysManagerInit = true;

            LevelImageReader lvlImgReader = new LevelImageReader(this, lvlImg);
            lvlImgReader.readImage(this);

            //levelBeginState = new Dictionary<int, Entity>(entities); //Copy the beginning game state

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;
        }

        public void loadFromPaint(string fileName, Graphics newG) {

            sysManagerInit = false;

            this.g = newG;

            Bitmap lvlImg = (Bitmap)Bitmap.FromFile(fileName);

            this.levelWidth = lvlImg.Width * GlobalVars.LEVEL_READER_TILE_WIDTH;
            this.levelHeight = lvlImg.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT;

            sysManager = new CreationSystemManager(this);

            sysManagerInit = true;

            LevelImageReader lvlImgReader = new LevelImageReader(this, lvlImg);
            lvlImgReader.readImage(this);
        }

        //Game logic
        public override void Update() {

            //Time in seconds between frames
            currentTicks = DateTime.Now.Ticks;
            pastTicks = currentTicks - prevTicks;
            prevTicks = currentTicks;

            float deltaTime = (float)(TimeSpan.FromTicks(pastTicks).TotalSeconds);
            fps = (1 / deltaTime);

            if (levelFullyLoaded && !paused) {
                sysManager.Update(deltaTime); //Update systems
            }

        }

        //When an entity is given a collider - notify collider system
        public override void colliderAdded(Entity e) {
            sysManager.colliderAdded(e);
        }


        //Reset the game to it's original startup state
        public override void resetLevel() {
            paused = true; // Pause the game briefly
            Entity[] ents = GlobalVars.nonGroundEntities.Values.ToArray();
            for (int i = 0; i < ents.Length; i++) {
                if (ents[i].isStartingEntity)
                    ents[i].revertToStartingState();
                else {
                    removeEntity(ents[i]);
                }
            }
            Entity[] grndents = GlobalVars.groundEntities.Values.ToArray();
            for (int i = 0; i < grndents.Length; i++) {
                if (grndents[i].isStartingEntity)
                    grndents[i].revertToStartingState();
                else {
                    removeEntity(grndents[i]);
                }
            }
            foreach (Entity e in GlobalVars.removedStartingEntities.Values) {
                e.revertToStartingState();
                addEntity(e.randId, e);
            }
            GlobalVars.removedStartingEntities.Clear();
            paused = false; //Restart the game  

        }

        public override void removeAllEntities() {
            /*
            while(GlobalVars.allEntities.Values.Count > 0)
            {
                Entity e = GlobalVars.allEntities.Values.ToArray()[0];
                //e.Destroy();
            }*/
            GlobalVars.nonGroundEntities.Clear();
            GlobalVars.groundEntities.Clear();
        }

        //Input
        public override void KeyDown(KeyEventArgs e) {
            sysManager.KeyDown(e);
        }
        public override void KeyUp(KeyEventArgs e) {
            sysManager.KeyUp(e);
        }
        public override void KeyPressed(KeyPressEventArgs e) {
            sysManager.KeyPressed(e);
        }
        public override void MouseClick(MouseEventArgs e) {
            //getCollisionSystem().MouseClick(e.X, e.Y);
            sysManager.MouseClick(e);
        }
        public override void MouseMoved(MouseEventArgs e) {
            sysManager.MouseMoved(e);
        }
        public void MouseLeave(EventArgs e) {
            if (vars.protoEntity != null) {
                PositionComponent posComp = (PositionComponent)vars.protoEntity.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                getMovementSystem().changePosition(posComp, -100, -100, false);
            }
        }

        //Draw everything!
        public override void Draw(Graphics g) {
            sysManager.Draw(g);
        }

        //Add an entity to the list of entities
        public override void addEntity(int id, Entity e) {
            if (!sysManagerInit) {
                sysManager = new CreationSystemManager(this);
                sysManagerInit = true;
            }
            if (e is BasicGround) {
                GlobalVars.groundEntities.Add(id, e);
            } else {
                GlobalVars.nonGroundEntities.Add(id, e);
            }
            if (e.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME)) {
                getCollisionSystem().colliderAdded(e);
            }
        }
        public override bool removeEntity(Entity e) {
            if (e == null) {
                Console.WriteLine("Trying to remove null entity");
                return false;
            }
            if (e.isStartingEntity)
                GlobalVars.removedStartingEntities.Add(e.randId, e);
            if (e.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME))
                getCollisionSystem().colliderRemoved(e);

            if (e is BasicGround) {
                GlobalVars.groundEntities.Remove(e.randId);
            } else {
                GlobalVars.nonGroundEntities.Remove(e.randId);
            }

            return true;
        }


        //Getters
        public override Dictionary<int, Entity> getNonGroundEntities() {
            return GlobalVars.nonGroundEntities;
        }
        public override Dictionary<int, Entity> getGroundEntities() {
            return GlobalVars.groundEntities;
        }

        public override MovementSystem getMovementSystem() {
            return sysManager.moveSystem;
        }
        public override CollisionDetectionSystem getCollisionSystem() {
            return sysManager.colSystem;
        }
        public override InputSystem getInputSystem() {
            if (sysManager != null)
                return sysManager.inputSystem;
            else return null;
        }
        public override Player getPlayer() {
            foreach (Entity e in GlobalVars.nonGroundEntities.Values) {
                if (e is Player)
                    return (Player)e;
            }
            return null;
        }

    }
}
