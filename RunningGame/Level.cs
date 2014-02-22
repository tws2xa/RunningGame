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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

namespace RunningGame {
    /*
     * A level is basically what it souds like.
     * It keeps track of all the entities within a stage
     * in the game, and updates them. It also holds
     * the SystemManager which is what controls the game systems.
     */
    [Serializable()]
    public class Level {

        Random rand; //for creating entitiy random id's

        public Graphics g { get; set; }
        public float cameraWidth { get; set; }
        public float cameraHeight { get; set; }
        public float levelWidth { get; set; }
        public float levelHeight { get; set; }
        public bool paused = false; //Is the game paused?

        public int worldNum; //Which world is it?
        public int levelNum; //Which level in that world?
        public bool colorOrbObtained = true; //In level 1, has the color orb been obtained yet?

        public bool shouldEndLevel = false; //Should it end the level at the end of the frame?

        public float fps;

        //Used for calculating fps and deltaTime
        long prevTicks = DateTime.Now.Ticks;
        long currentTicks;
        long pastTicks;

        public SystemManager sysManager; //Manages and Controls all systems
        bool sysManagerInit = false; //Has the systemManager been initialized yet?

        //Timer for ending the level - how long a break/fade is there before it cuts to next level?
        float endLvlTime = 0.5f; //Typical length for setting the timer to when ending the level. In seconds.
        float endLvlTimer = -1.0f; //Timer. Do not modify.

        public bool levelFullyLoaded = false;

        public Font displayFont = SystemFonts.DefaultFont;

        //A queue of methods (values) to run after a certain period of time(key, in seconds).
        public Dictionary<Action, float> timerMethods = new Dictionary<Action, float>(); 


        //Used for displaying instruction text:
        public string dispTxt = "";
        public Color dispCol = Color.White;
        public float dispTimeIn = 0;
        public float dispConstTime = 0;
        public float dispTimeOut = 0;

        public Level() { }

        public Level( float windowWidth, float windowHeight, string levelFile, int worldNum, int levelNum, bool isPaintFile, Graphics g, Font displayFont ) {
            this.displayFont = displayFont;
            this.worldNum = worldNum;
            this.levelNum = levelNum;
            this.colorOrbObtained = ( levelNum != 1 ); //False when level 1 begins, otherwise true.

            if ( isPaintFile )
                initializePaint( windowWidth, windowHeight, levelFile, g );
            //else
            //initializeNotPaint(windowWidth, windowHeight, levelFile, g);
        }

        /* Used for level editor. Doesn't work.
        public void initializeNotPaint(float windowWidth, float windowHeight, string levelFile, Graphics g)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Stream f = Assembly.GetExecutingAssembly().GetManifestResourceStream(levelFile);

            List<Object> ents = (List<Object>)bf.Deserialize(f);

            cameraWidth = windowWidth;
            cameraHeight = windowHeight;
            this.levelWidth = (float)ents[0];
            this.levelHeight = (float)ents[1];

            if (!sysManagerInit) sysManager = new SystemManager(this);

            sysManagerInit = true;

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;

            //
            for (int i = 2; i < ents.Count; i++)
            {
                Entity oldEnt = (Entity)ents[i];
                Entity newEnt = (Entity)Activator.CreateInstance(oldEnt.GetType(), this, 0, 0);

                newEnt.randId = oldEnt.randId;

                //Copy all the fields! Ugh
                CopyFields(oldEnt, newEnt);
                foreach (Component oldC in oldEnt.getComponents())
                {
                    if (newEnt.hasComponent(oldC.componentName))
                    {
                        Component newC = newEnt.getComponent(oldC.componentName);
                        CopyFields(oldC, newC);
                    }
                }

                newEnt.level = this;
                addEntity(newEnt.randId, newEnt);
            }
            //


            
            for (int i = 2; i < ents.Count; i++)
            {
                Entity e = (Entity)ents[i];
                e.level = this;
                addEntity(e.randId, e);
            }
            

            Entity bkgEnt = getMyBackgroundEntity();
            addEntity(bkgEnt.randId, bkgEnt);

        }
        */

        //Also used for level editor. Doesn't work.
        public void CopyFields( object oldObj, object newObj ) {
            foreach ( FieldInfo oldInfo in oldObj.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) ) {
                foreach ( FieldInfo newInfo in newObj.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) ) {
                    if ( oldInfo.Name == newInfo.Name ) {
                        newInfo.SetValue( newObj, oldInfo.GetValue( oldObj ) );
                    }
                }
            }
        }

        //Loads a level from a paint file.
        public void initializePaint( float windowWidth, float windowHeight, string levelFile, Graphics g ) {

            rand = new Random();
            this.g = g;


            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream myStream = myAssembly.GetManifestResourceStream( levelFile );
            Bitmap lvlImg = new Bitmap( myStream );

            cameraWidth = windowWidth;
            cameraHeight = windowHeight;
            this.levelWidth = lvlImg.Width * GlobalVars.LEVEL_READER_TILE_WIDTH;
            this.levelHeight = lvlImg.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT;

            if ( !sysManagerInit ) sysManager = new SystemManager( this );

            sysManagerInit = true;

            sysManager.ClearSystems();

            LevelImageReader lvlImgReader = new LevelImageReader( this, lvlImg );
            lvlImgReader.readImage( this );

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;

            Entity bkgEnt = getMyBackgroundEntity();
            addEntity( bkgEnt.randId, bkgEnt );


            //Set the player powerup staring values
            setPowerups();

            resetLevel();
        }

        //This looks at the world and level numbers to figure out which powerups
        //Should and should not be enabled at the start of the level.
        public void setPowerups() {
            if ( worldNum > 1 || ( worldNum == 1 && levelNum > 1 ) ) {
                sysManager.spSystem.unlockPowerup( GlobalVars.JMP_NUM );
                if ( worldNum > 2 || ( worldNum == 2 && levelNum > 1 ) ) {
                    sysManager.spSystem.unlockPowerup( GlobalVars.SPEED_NUM );
                    if ( worldNum > 3 || ( worldNum == 3 && levelNum > 1 ) ) {
                        sysManager.spSystem.unlockPowerup( GlobalVars.BOUNCE_NUM );
                        if ( worldNum > 4 || ( worldNum == 4 && levelNum > 1 ) ) {
                            sysManager.spSystem.unlockPowerup( GlobalVars.GLIDE_NUM );
                            if ( worldNum > 5 || ( worldNum == 5 && levelNum > 1 ) ) {
                                sysManager.spSystem.unlockPowerup( GlobalVars.SPAWN_NUM );
                                if ( worldNum > 6 || ( worldNum == 6 && levelNum > 1 ) ) {
                                    sysManager.spSystem.unlockPowerup( GlobalVars.GRAP_NUM );
                                } else {
                                    sysManager.spSystem.lockPowerup( GlobalVars.GRAP_NUM );
                                }
                            } else {
                                sysManager.spSystem.lockPowerup( GlobalVars.SPAWN_NUM );
                                sysManager.spSystem.lockPowerup( GlobalVars.GRAP_NUM );
                            }
                        } else {
                            sysManager.spSystem.lockPowerup(GlobalVars.GLIDE_NUM);
                            sysManager.spSystem.lockPowerup( GlobalVars.SPAWN_NUM );
                            sysManager.spSystem.lockPowerup( GlobalVars.GRAP_NUM );
                        }
                    } else {
                        sysManager.spSystem.lockPowerup( GlobalVars.BOUNCE_NUM );
                        sysManager.spSystem.lockPowerup(GlobalVars.GLIDE_NUM);
                        sysManager.spSystem.lockPowerup( GlobalVars.SPAWN_NUM );
                        sysManager.spSystem.lockPowerup( GlobalVars.GRAP_NUM );
                    }
                } else {
                    sysManager.spSystem.lockPowerup( GlobalVars.SPEED_NUM );
                    sysManager.spSystem.lockPowerup( GlobalVars.BOUNCE_NUM );
                    sysManager.spSystem.lockPowerup(GlobalVars.GLIDE_NUM);
                    sysManager.spSystem.lockPowerup( GlobalVars.SPAWN_NUM );
                    sysManager.spSystem.lockPowerup( GlobalVars.GRAP_NUM );
                }
            } else {
                sysManager.spSystem.lockPowerup( GlobalVars.JMP_NUM );
                sysManager.spSystem.lockPowerup( GlobalVars.SPEED_NUM );
                sysManager.spSystem.lockPowerup( GlobalVars.BOUNCE_NUM );
                sysManager.spSystem.lockPowerup(GlobalVars.GLIDE_NUM);
                sysManager.spSystem.lockPowerup( GlobalVars.SPAWN_NUM );
                sysManager.spSystem.lockPowerup( GlobalVars.GRAP_NUM );
            }

        }


        //Game logic
        public virtual void Update() {

            //Time in seconds between frames
            currentTicks = DateTime.Now.Ticks;
            pastTicks = currentTicks - prevTicks;
            prevTicks = currentTicks;

            float deltaTime = ( float )( TimeSpan.FromTicks( pastTicks ).TotalSeconds );
            fps = ( 1 / deltaTime );

            if ( levelFullyLoaded && !paused ) {

                //If the timer has been started
                if ( endLvlTimer >= 0 ) {
                    //Decrement it by the time that has passed
                    endLvlTimer -= deltaTime;

                    //If it's less than 0, tell the level to end.
                    if ( endLvlTimer <= 0 ) {
                        endLvlTimer = -1;
                        shouldEndLevel = true;
                    }
                }

                List<Action> toRemove = new List<Action>();
                List<Action> allActions = timerMethods.Keys.ToList<Action>();
                foreach ( Action action in allActions ) {
                    if ( timerMethods[action] >= 0 ) {
                        timerMethods[action] -= deltaTime;

                        if ( timerMethods[action] <= 0 ) {
                            action.Invoke();
                            toRemove.Add( action );
                        }

                    } else {
                        toRemove.Add( action );
                    }
                }

                foreach ( Action action in toRemove ) {
                    timerMethods.Remove( action );
                }

                sysManager.Update( deltaTime ); //Update systems
            }
        }

        //Begin an end level routine with default time
        public void beginEndLevel() {
            if ( endLvlTimer < 0 ) {
                //Get the draw system, call the white clash, and start the end level timer.
                DrawSystem drawSys = sysManager.drawSystem;
                drawSys.setFlash( System.Drawing.Color.WhiteSmoke, endLvlTime * 2 );
                endLvlTimer = endLvlTime;
            }
        }
        //Begin an end level routine with given time delay
        public void beginEndLevel( float time ) {
            if ( endLvlTimer < 0 ) {
                //Get the draw system, call the white clash, and start the end level timer.
                DrawSystem drawSys = sysManager.drawSystem;
                drawSys.setFlash( System.Drawing.Color.WhiteSmoke, time * 2 );
                endLvlTimer = time;
            }
        }


        //When an entity is given a collider - notify collider system so it can update the location grid
        public virtual void colliderAdded( Entity e ) {
            sysManager.colliderAdded( e );
        }


        //Reset the game to it's original startup state
        public virtual void resetLevel() {
            paused = true; // Pause the game briefly

            //Deactivate the vision orb if it's active
            if ( sysManager.visSystem.orbActive ) {
                sysManager.visSystem.destroyVisionOrb();
            }

            //Remove border
            if ( sysManager.drawSystem.getMainView().hasBorder ) sysManager.drawSystem.getMainView().hasBorder = false;

            //Remove non-starting entities, and restore starting entities to their initial state
            Entity[] ents = GlobalVars.nonGroundEntities.Values.ToArray();
            for ( int i = 0; i < ents.Length; i++ ) {
                if ( ents[i].isStartingEntity )
                    ents[i].revertToStartingState();
                else {
                    removeEntity( ents[i] );
                }
            }
            //Do the same for ground
            Entity[] grndents = GlobalVars.groundEntities.Values.ToArray();
            for ( int i = 0; i < grndents.Length; i++ ) {
                if ( grndents[i].isStartingEntity )
                    grndents[i].revertToStartingState();
                else {
                    removeEntity( grndents[i] );
                }
            }
            foreach ( Entity e in GlobalVars.removedStartingEntities.Values ) {
                e.revertToStartingState();
                addEntity( e.randId, e );
            }
            GlobalVars.removedStartingEntities.Clear();

            if ( levelNum == 1 ) {
                setToPreColors(); //Reset Colors if first level in a world.
            }
            setPowerups(); //Reset the powerups

            paused = false; //Restart the game  

        }

        //Get rid of all the entities in the level
        public virtual void removeAllEntities() {

            while ( GlobalVars.nonGroundEntities.Values.Count > 0 ) {
                Entity e = GlobalVars.nonGroundEntities.Values.ToArray()[0];
                removeEntity( e );
            }

            GlobalVars.nonGroundEntities.Clear();

            while ( GlobalVars.groundEntities.Values.Count > 0 ) {
                Entity e = GlobalVars.groundEntities.Values.ToArray()[0];
                removeEntity( e );
            }

            GlobalVars.groundEntities.Clear();
        }

        //Input (Just passed on to the system manager.)
        public virtual void KeyDown( KeyEventArgs e ) {
            sysManager.KeyDown( e );
        }
        public virtual void KeyUp( KeyEventArgs e ) {
            sysManager.KeyUp( e );
        }
        public virtual void KeyPressed( KeyPressEventArgs e ) {
            sysManager.KeyPressed( e );
        }
        public virtual void MouseClick( MouseEventArgs e ) {
            //getCollisionSystem().MouseClick(e.X, e.Y);
            sysManager.MouseClick( e );
        }
        public virtual void MouseMoved( MouseEventArgs e ) {
            sysManager.MouseMoved( e );
        }



        //Draw everything!
        public virtual void Draw( Graphics g ) {

            sysManager.Draw( g );
            //this part is the check for the flash
            //if flashtime is greater than 0, then it means that flash needs to be done

            g.DrawString( GlobalVars.preciseCollisionChecking.ToString(), SystemFonts.DefaultFont, Brushes.Black, new RectangleF( 10, 30, cameraWidth - 20, cameraHeight - 20 ) );
            g.DrawString( fps.ToString( "F" ) + "", SystemFonts.DefaultFont, Brushes.Black, new RectangleF( 10, 10, cameraWidth - 20, cameraHeight - 20 ) );
        }

        //Add an entity to the list of entities
        public virtual void addEntity( Entity e ) {
            addEntity( e.randId, e );
        }

        //Add an entity to the list of entities. Takes in the entity and it's ID
        public virtual void addEntity( int id, Entity e ) {
            if ( !sysManagerInit ) {
                sysManager = new SystemManager( this );
                sysManagerInit = true;
            }

            if ( e is BasicGround ) {
                if ( GlobalVars.groundEntities.ContainsKey( id ) ) {
                    GlobalVars.groundEntities.Remove( id );
                }
                GlobalVars.groundEntities.Add( id, e );
                colliderAdded( e );
            } else {
                if ( GlobalVars.nonGroundEntities.ContainsKey( id ) ) GlobalVars.nonGroundEntities.Remove( id );
                GlobalVars.nonGroundEntities.Add( id, e );
                if ( e.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) )
                    colliderAdded( e );
            }

            sysManager.entityAdded( e );

        }

        //Removes an entity from the level.
        public virtual bool removeEntity( Entity e ) {
            if ( e == null ) {
                Console.WriteLine( "You tryin' ta remove a null entity? Whachu doin' dat fo'?" );
                return false;
            }
            //If the entity has a collider - notify the collision system of the change and remove it from locGrid
            if ( e.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) ) {
                getCollisionSystem().colliderRemoved( e );
            }

            //If it's ground, remove it from the ground list. Otherwise remove it from the other list
            if ( e is BasicGround ) {
                if ( GlobalVars.groundEntities.ContainsKey( e.randId ) ) {
                    //If it's a starting entity, add it to the list of removed starting entities
                    //So that it can be restored if the level is reset
                    if ( e.isStartingEntity )
                        GlobalVars.removedStartingEntities.Add( e.randId, e );
                    GlobalVars.groundEntities.Remove( e.randId );
                    sysManager.entityRemoved( e );
                    return true;
                }
            } else {
                //If it's a starting entity, add it to the list of removed starting entities
                //So that it can be restored if the level is reset
                if ( GlobalVars.nonGroundEntities.ContainsKey( e.randId ) ) {
                    if ( e.isStartingEntity )
                        GlobalVars.removedStartingEntities.Add( e.randId, e );
                    GlobalVars.nonGroundEntities.Remove( e.randId );
                    sysManager.entityRemoved( e );
                    return true;
                }
            }
            return false; //Not found
        }

        //This creates the background entity for the level.
        public BackgroundEntity getMyBackgroundEntity() {

            string fullImageAddress = "RunningGame.Resources.Artwork.Background.Bkg11.png";
            string imageStub = "Artwork.Background.Bkg";
            string preColorImageStub = "Artwork.Background.BkgPreColor";


            Bitmap tempImg = getBkgImg();
            float newWidth = tempImg.Width;
            float newHeight = tempImg.Height;

            float scaleFactor = 1.1f;

            while ( newWidth / scaleFactor > levelWidth || newHeight / scaleFactor > levelHeight ) {
                newWidth /= scaleFactor;
                newHeight /= scaleFactor;
            }

            while ( newWidth < levelWidth || newHeight < levelHeight ) {
                newWidth *= scaleFactor;
                newHeight *= scaleFactor;
            }

            BackgroundEntity bkgEnt = new BackgroundEntity( this, 0, 0, newWidth, newHeight );
            DrawComponent drawComp = ( DrawComponent )bkgEnt.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            PositionComponent posComp = ( PositionComponent )bkgEnt.getComponent( GlobalVars.POSITION_COMPONENT_NAME );


            /*
            //Static Background
            if (sysManager.bkgPosSystem.scrollType == 0)
            {
                float initWidth = drawComp.getImage().Width;
                float initHeight = drawComp.getImage().Height;

                float xDiff = Math.Abs(initWidth - levelWidth);
                float yDiff = Math.Abs(initHeight - levelHeight);

                //Make it fit horizontally
                if (xDiff < yDiff)
                {
                    float ratio = initWidth / levelWidth;
                    getMovementSystem().changeSize(posComp, levelWidth, initHeight * ratio);
                    drawComp.resizeImages((int)levelWidth, (int)(initHeight * ratio));
                    getMovementSystem().changePosition(posComp, levelWidth/2, levelHeight-initHeight*ratio/2, false);
                }
                //Make it fit vertically
                else
                {
                    float ratio = initHeight / levelHeight;
                    getMovementSystem().changeSize(posComp, initWidth*ratio , levelHeight);
                    drawComp.resizeImages((int)(initWidth*ratio), (int)(levelHeight));
                    getMovementSystem().changePosition(posComp, initWidth * ratio / 2, levelHeight / 2, false);
                }
            }
            else if (sysManager.bkgPosSystem.scrollType == 3)
            {
                Bitmap tempImg = getBkgImg();
                float newWidth = tempImg.Width;
                float newHeight = tempImg.Height;

                float scaleFactor = 1.1f;

                while (newWidth/scaleFactor > levelWidth || newHeight/scaleFactor > levelHeight)
                {
                    newWidth /= scaleFactor;
                    newHeight /= scaleFactor;
                }

                while (newWidth < levelWidth || newHeight < levelHeight)
                {
                    newWidth *= scaleFactor;
                    newHeight *= scaleFactor;
                }

                getMovementSystem().changeSize(posComp, newWidth, newHeight);
                drawComp.width = newWidth;
                drawComp.height = newHeight;
                drawComp.resizeImages((int)newWidth, (int)newHeight);
                getMovementSystem().changePosition(posComp, newWidth / 2, newHeight / 2, false);
            }
             * */
            /*
            //Proportion Scrolling
            if (sysManager.bkgPosSystem.scrollType == 1 || sysManager.bkgPosSystem.scrollType == 2)
            {
                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                // THIS WONT WORK BECAUSE IT"S GETTING THE DEFAULT IMAGE NOT IMAGE STUB
                System.IO.Stream myStream = myAssembly.GetManifestResourceStream(fullImageAddress);
                Bitmap sprite = new Bitmap(myStream); //Getting an error here? Did you remember to make your image an embedded resource?
                myStream.Close();

                float w = sprite.Width;
                float h = sprite.Height;

                float multiplier = 0.5f;

                while (w > levelWidth || (h > levelHeight && sysManager.bkgPosSystem.scrollType == 1))
                {
                    w *= multiplier;
                    h *= multiplier;
                }

                getMovementSystem().changeSize(posComp, w, h);

            }
            */
            bool tryBkgColorChange = false;
            if ( tryBkgColorChange ) {
                bool wentToDefault = false;
                if ( levelNum == 1 ) {
                    wentToDefault = drawComp.addSprite( preColorImageStub, fullImageAddress, "PreColorBkg" );
                }

                drawComp.addSprite( imageStub, fullImageAddress, "MainBkg" );

                if ( wentToDefault || levelNum != 1 ) {
                    drawComp.setSprite( "MainBkg" );
                } else {
                    drawComp.setSprite( "PreColorBkg" );
                }
            } else {
                drawComp.addSprite( imageStub, fullImageAddress, "MainBkg" );
                drawComp.setSprite( "MainBkg" );
            }

            bkgEnt.isStartingEntity = true;

            return bkgEnt;

        }

        //Returns the background image that belongs to the level
        //Uses the file naming convention RunningGame.Resources.Artwork.Background.Bkg[World#][Level#]
        public Bitmap getBkgImg() {
            string defaultAddress = "RunningGame.Resources.Artwork.Background.Bkg11.png";

            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            string addr = "";
            System.IO.Stream myStream = null;

            if ( levelNum == 1 ) {

                addr = ( "RunningGame.Resources.Artwork.Background.Bkg" + worldNum + "" + levelNum + "PreColor.png" );
                myStream = myAssembly.GetManifestResourceStream( addr );

            } 

            if(myStream == null) {
                addr = ( "RunningGame.Resources.Artwork.Background.Bkg" + worldNum + "" + levelNum + ".png" );
                myStream = myAssembly.GetManifestResourceStream( addr );
            }

            if ( myStream == null ) {
                myStream = myAssembly.GetManifestResourceStream( defaultAddress );
            }

            // Are you getting an error here?
            //Did you remember to make your image an embedded resource?
            Bitmap sprite = new Bitmap( myStream ); 
            myStream.Close();

            return sprite;
        }

        //Getters
        public virtual Dictionary<int, Entity> getNonGroundEntities() {
            return GlobalVars.nonGroundEntities;
        }
        public virtual Dictionary<int, Entity> getGroundEntities() {
            return GlobalVars.groundEntities;
        }
        public virtual MovementSystem getMovementSystem() {
            return sysManager.moveSystem;
        }
        public virtual CollisionDetectionSystem getCollisionSystem() {
            return sysManager.colSystem;
        }
        public virtual InputSystem getInputSystem() {
            if ( sysManager != null )
                return sysManager.inputSystem;
            else return null;
        }
        public virtual Player getPlayer() {
            foreach ( Entity e in GlobalVars.nonGroundEntities.Values ) {
                if ( e.hasComponent( GlobalVars.PLAYER_COMPONENT_NAME ) ) return ( Player )e;
            }

            return null;
        }

        //Close the level.
        public void Close() {
            removeAllEntities();
            GlobalVars.removedStartingEntities.Clear();
        }




        public void setToPostColors() {
            this.colorOrbObtained = true;
            
            foreach ( Entity e in sysManager.drawSystem.getApplicableEntities() ) {
                DrawComponent drawComp = ( DrawComponent )e.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
                drawComp.switchToPostColorImage();
            }
        }
        public void setInstrText( string txt, Color col, float timeIn, float constTime, float timeOut ) {
            dispTxt = txt;
            dispCol = col;
            dispTimeIn = timeIn;
            dispConstTime = constTime;
            dispTimeOut = timeOut;
        }
        public void displayInstrText() {
            sysManager.drawSystem.activateTextFlash( dispTxt, dispCol, dispTimeIn, dispConstTime, dispTimeOut );
        }
        public void setToPreColors() {
            this.colorOrbObtained = false;

            foreach ( Entity e in sysManager.drawSystem.getApplicableEntities() ) {
                DrawComponent drawComp = ( DrawComponent )e.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
                drawComp.switchToPostColorImage();
            }
        }
    }
}
