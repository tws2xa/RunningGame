using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RunningGame.Entities;
using RunningGame.Components;
using System.Collections;
using RunningGame.Level_Editor;

namespace RunningGame
{

    /*
     * This class reads in a level image, and adds the
     * appropriate entities into the game world.
     */
    [Serializable()]
    class LevelImageReader
    {

        
        Bitmap img;

        Color basicGroundCol    = Color.FromArgb(0, 0, 0); //Basic Ground is black.
        Color playerCol         = Color.FromArgb(0, 0, 255); //Player is blue.
        Color vertMovPlatCol    = Color.FromArgb(0, 255, 0); //Vertical Plafroms are green!
        Color simpleEnemyColor  = Color.FromArgb(255, 0, 0); //Walking Enemies are red.
        Color flyingEnemyColor  = Color.FromArgb(255, 255, 0); //Flying enemies are yellow!
        Color endLevelCol       = Color.FromArgb(255, 255, 255); //End level is white
        Color testEntityColor   = Color.FromArgb(42, 42, 42); //Test entity is 42, 42, 42.

        Color bouncePickup      = Color.FromArgb(100, 100, 0);
        Color speedyPickup      = Color.FromArgb(100, 100, 1);
        Color jmpPickup         = Color.FromArgb(100, 100, 2);
        Color glidePickup       = Color.FromArgb(100, 100, 3);
        Color spawnPickup       = Color.FromArgb(100, 100, 4);
        Color grapPickup        = Color.FromArgb(100, 100, 5);

        //Link doors with switches by giving them the same B
        //Permanent Switch - G = 255
        //Pressure Switch - G = 0
        //Timed Switch - G = Time in deci-seconds... i.e. 100 -> 10 seconds. 015 = 1.5 seconds.
        int switchReserveRed = 200; //Any color with R = 200 is a switch
        int permSwitchG = 255;
        int presSwitchG = 0;
        int doorReserveGreen = 200; //Any color with G = 200 is a door

        int spikeRed = 255; //R
        int spikeGreen = 100; //G
                            // B determins Dir

        Dictionary<int, Entity> switches;
        Dictionary<SwitchListenerComponent, int> unmachedSwitchListeners;

        Random rand = new Random();

        float tileWidth = GlobalVars.LEVEL_READER_TILE_WIDTH;
        float tileHeight = GlobalVars.LEVEL_READER_TILE_HEIGHT;

        public LevelImageReader(Level level, Bitmap img)
        {

            this.img = img;

            level.levelWidth = (img.Width)*tileWidth;
            level.levelHeight = (img.Height)*tileHeight;

            switches = new Dictionary<int, Entity>();
            unmachedSwitchListeners = new Dictionary<SwitchListenerComponent, int>();

        }
        public void readImage(Level level)
        {
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color col = img.GetPixel(x, y);

                    float levelX = x;
                    float levelY = y;

                    if (col.R == switchReserveRed)
                    {
                        Entity s = null;

                        if (col.G == permSwitchG)
                        {
                            s = new SwitchEntity(level, rand.Next(Int32.MinValue, Int32.MaxValue), levelX * tileWidth, levelY * tileHeight);
                        }
                        else if (col.G == presSwitchG)
                        {
                            //Pressure Switch
                            s = new PressureSwitchEntity(level, rand.Next(Int32.MinValue, Int32.MaxValue), levelX * tileWidth, levelY * tileWidth);
                            TimedSwitchComponent timeComp = (TimedSwitchComponent)s.getComponent(GlobalVars.TIMED_SWITCH_COMPONENT_NAME);
                            timeComp.baseTime = 0;
                        }
                        else
                        {
                            s = new TimedSwitchEntity(level, rand.Next(Int32.MinValue, Int32.MaxValue), levelX * tileWidth, levelY * tileWidth);
                            TimedSwitchComponent timeComp = (TimedSwitchComponent)s.getComponent(GlobalVars.TIMED_SWITCH_COMPONENT_NAME);
                            timeComp.baseTime = col.G / 10;
                        }
                        s.isStartingEntity = true;
                        adjustLocation(s, level);
                        switches.Add(col.B, s);
                        level.addEntity(s.randId, s);
                    }
                    else if (col.G == doorReserveGreen)
                    {
                        DoorEntity door = new DoorEntity(level, rand.Next(Int32.MinValue, Int32.MaxValue), levelX * tileWidth, levelY * tileHeight);
                        adjustLocation(door, level);
                        SwitchListenerComponent slComp = (SwitchListenerComponent)door.getComponent(GlobalVars.SWITCH_LISTENER_COMPONENT_NAME);
                        door.isStartingEntity = true;
                        //check for its switch
                        if (switches.ContainsKey(col.B))
                        {
                            slComp.switchId = switches[col.B].randId;
                        }
                        else
                        {
                            unmachedSwitchListeners.Add(slComp, col.B);
                        }
                        level.addEntity(door);
                    }
                    else if (col.R == spikeRed && col.G == spikeGreen)
                    {

                        SpikeEntity spike = new SpikeEntity(level, rand.Next(Int32.MinValue, Int32.MaxValue), levelX * tileWidth, levelY * tileHeight, col.B);
                        adjustLocation(spike, level);
                        spike.isStartingEntity = true;
                        level.addEntity(spike);
                    }
                    else if (col == playerCol)
                    {
                        Player player = new Player(level, rand.Next(Int32.MinValue, Int32.MaxValue), levelX*tileWidth, levelY*tileHeight);
                        adjustLocation(player, level);
                        player.isStartingEntity = true;
                        level.addEntity(player.randId, player);
                    }
                    else if (col == basicGroundCol)
                    {

                        float groundX = (levelX) * tileWidth;
                        float groundWidth = tileWidth; 

                        
                        BasicGround ground = new BasicGround(level, rand.Next(Int32.MinValue, Int32.MaxValue), groundX, (levelY)*tileHeight, groundWidth, tileHeight);
                        adjustLocation(ground, level);
                        ground.isStartingEntity = true;
                        level.addEntity(ground.randId, ground);
                        
                        //If no ground above it, change to a grass sprite
                        List<Entity> above = level.getCollisionSystem().findObjectAtPoint((levelX) * tileWidth, (levelY - 1) * tileWidth);
                        if (above.Count <= 0 || !(above[0] is BasicGround))
                        {
                            ground.changeSprite(false);
                        }
                        
                    }
                    else if (col == testEntityColor)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);
                        TestEntity test = new TestEntity(level, id, xLoc, yLoc);
                        adjustLocation(test, level);
                        test.isStartingEntity = true;
                        level.addEntity(test.randId, test);
                    }
                    else if (col == simpleEnemyColor)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);
                        SimpleEnemyEntity enemy = new SimpleEnemyEntity(level, id, xLoc, yLoc);
                        adjustLocation(enemy, level);
                        enemy.isStartingEntity = true;
                        level.addEntity(enemy.randId, enemy);
                    }
                    else if (col == flyingEnemyColor)
                    {

                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);
                        FlyingEnemyEntity enemy = new FlyingEnemyEntity(level, id, xLoc, yLoc);
                        adjustLocation(enemy, level);
                        enemy.isStartingEntity = true;
                        level.addEntity(enemy.randId, enemy);
                    }
                    else if (col == endLevelCol)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);
                        EndLevelEntity lvlEnd = new EndLevelEntity(level, id, xLoc, yLoc);
                        adjustLocation(lvlEnd, level);
                        lvlEnd.isStartingEntity = true;
                        level.addEntity(lvlEnd.randId, lvlEnd);
                    }
                    else if (col == vertMovPlatCol)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);

                        MovingPlatformEntity plat = new MovingPlatformEntity(level, id, xLoc, yLoc);
                        adjustLocation(plat, level);
                        
                        plat.isStartingEntity = true;
                        level.addEntity(plat);
                    }
                    else if (col == bouncePickup)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);

                        PowerupPickupEntity pickup = new PowerupPickupEntity(level, id, xLoc, yLoc, GlobalVars.BOUNCE_NUM);
                        adjustLocation(pickup, level);

                        pickup.isStartingEntity = true;
                        level.addEntity(pickup);
                    }
                    else if (col == speedyPickup)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);

                        PowerupPickupEntity pickup = new PowerupPickupEntity(level, id, xLoc, yLoc, GlobalVars.SPEED_NUM);
                        adjustLocation(pickup, level);

                        pickup.isStartingEntity = true;
                        level.addEntity(pickup);
                    }
                    else if (col == jmpPickup)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);

                        PowerupPickupEntity pickup = new PowerupPickupEntity(level, id, xLoc, yLoc, GlobalVars.JMP_NUM);
                        adjustLocation(pickup, level);

                        pickup.isStartingEntity = true;
                        level.addEntity(pickup);
                    }
                    else if (col == glidePickup)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);

                        PowerupPickupEntity pickup = new PowerupPickupEntity(level, id, xLoc, yLoc, GlobalVars.GLIDE_NUM);
                        adjustLocation(pickup, level);

                        pickup.isStartingEntity = true;
                        level.addEntity(pickup);
                    }
                    else if (col == spawnPickup)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);

                        PowerupPickupEntity pickup = new PowerupPickupEntity(level, id, xLoc, yLoc, GlobalVars.SPAWN_NUM);
                        adjustLocation(pickup, level);

                        pickup.isStartingEntity = true;
                        level.addEntity(pickup);
                    }
                    else if (col == grapPickup)
                    {
                        float xLoc = (levelX) * tileWidth;
                        float yLoc = (levelY) * tileHeight;
                        int id = rand.Next(Int32.MinValue, Int32.MaxValue);

                        PowerupPickupEntity pickup = new PowerupPickupEntity(level, id, xLoc, yLoc, GlobalVars.GRAP_NUM);
                        adjustLocation(pickup, level);

                        pickup.isStartingEntity = true;
                        level.addEntity(pickup);
                    }
                }
            }

            //Match any unmatched doors
            foreach (SwitchListenerComponent sc in unmachedSwitchListeners.Keys)
            {
                if (switches.ContainsKey(unmachedSwitchListeners[sc]))
                {
                    SwitchListenerComponent slComp = sc;
                    slComp.switchId = switches[unmachedSwitchListeners[sc]].randId;
                }
                else
                {
                    Console.WriteLine("Unmatched Switch Listener - B: " + unmachedSwitchListeners[sc]);
                }
            }

        }


        public void adjustLocation(Entity e, Level level)
        {
            PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().teleportToNoCollisionCheck(posComp, posComp.x + posComp.width / 2, posComp.y + posComp.height / 2);
            posComp.setCurrentLocToStartingLoc();
        }

    }
}
