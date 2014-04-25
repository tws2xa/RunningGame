using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using RunningGame.Entities;
using RunningGame.Components;
using RunningGame.Level_Editor;

namespace RunningGame {

    /*
     * This class reads in a level image, and adds the
     * appropriate entities into the game world.
     */
    [Serializable()]
    class LevelImageReader {


        Bitmap img;

        Color basicGroundCol = Color.FromArgb( 0, 0, 0 ); //Basic Ground is black.
        Color playerCol = Color.FromArgb( 0, 0, 255 ); //Player is blue.
        Color vertMovPlatCol = Color.FromArgb( 0, 255, 0 ); //Vertical Plafroms are green!
        //Color horizMovPlatCol = Color.FromArgb( 0, 255, 255 ); //Horizontal Plafroms are cyan!
        Color simpleEnemyColor = Color.FromArgb( 255, 0, 0 ); //Walking Enemies are red.
        Color shieldWalkingEnemyColor = Color.FromArgb( 255, 0, 1 );
        Color flyingEnemyColor = Color.FromArgb( 255, 255, 0 ); //Flying enemies are yellow!
        Color shieldFlyingEnemyColor = Color.FromArgb( 255, 255, 1 );
        Color checkPointCollider = Color.FromArgb( 255, 255, 255 ); //End level is white
        Color movePlatformTurn = Color.FromArgb( 140, 140, 140 ); //Turn Platform Entity
        Color testEntityColor = Color.FromArgb( 42, 42, 42 ); //Test entity is 42, 42, 42.
        Color visionOrbUnlock = Color.FromArgb( 13, 13, 13 ); //Add the vision orb

        Color jmpPickup = Color.FromArgb( 100, 100, 0 );
        Color speedyPickup = Color.FromArgb( 100, 100, 1 );
        Color bouncePickup = Color.FromArgb( 100, 100, 2 );
        Color glidePickup = Color.FromArgb( 100, 100, 3 );
        Color spawnPickup = Color.FromArgb( 100, 100, 4 );
        Color grapPickup = Color.FromArgb( 100, 100, 5 );

        //Link doors with switches by giving them the same B
        //Timed Switch - G = Time in deci-seconds... i.e. 100 -> 10 seconds. 015 = 1.5 seconds.
        int switchReserveRed = 200; //Any color with R = 200 is a switch
        int spikeSwitchReserveRed = 201;
        int permSwitchG = 255; //Permanent Switch - G = 255
        int presSwitchG = 0; //Pressure Switch - G = 0
        int tallDoorReserveGreen = 200; //Any color with G = 200 is a door
        int wideDoorReserveGreen = 201;
        int openTallDoorReserveGreen = 202;
        int openWideDoorReserveGreen = 203;
        int smushRed = 77; //G Determines Switch // B Determines Dir

        int spikeRed = 255; //R
        int spikeGreen = 100; //G
        // B determins Dir

        int shooterRedUp = 110;
        int shooterRedRight = 111;
        int shooterRedDown = 112;
        int shooterRedLeft = 113;
        //Green = Time
        //Blue = Shots per burst

        Dictionary<int, Entity> switches;
        Dictionary<SwitchListenerComponent, int> unmachedSwitchListeners;

        Random rand = new Random();

        //For one game in the level png file, what size is the corresponding in-game section
        float tileWidth = GlobalVars.LEVEL_READER_TILE_WIDTH;
        float tileHeight = GlobalVars.LEVEL_READER_TILE_HEIGHT;


        float tallDoorWidth = 30;
        float tallDoorHeight = 60;
        float wideDoorWidth = 60;
        float wideDoorHeight = 30;

        public LevelImageReader( Level level, Bitmap img ) {

            this.img = img;

            level.levelWidth = ( img.Width ) * tileWidth;
            level.levelHeight = ( img.Height ) * tileHeight;

            switches = new Dictionary<int, Entity>();
            unmachedSwitchListeners = new Dictionary<SwitchListenerComponent, int>();

        }

        //Reads in a paint image and adds all entities to the level.
        public void readImage( Level level ) {
            for ( int y = 0; y < img.Height; y++ ) {
                for ( int x = 0; x < img.Width; x++ ) {
                    //Get the color of the pixel
                    Color col = img.GetPixel( x, y );

                    float levelX = x;
                    float levelY = y;

                    //First check for cases which have some variation in possible color values
                    //i.e. something identified with only it's RG, or GB, or RB, or just R or G or B
                    if ( col.R == switchReserveRed ) {
                    
                        Entity s = null;

                        if ( col.G == permSwitchG ) {
                            s = new SwitchEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight );
                        } else if ( col.G == presSwitchG ) {
                            //Pressure Switch
                            s = new PressureSwitchEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileWidth );
                            TimedSwitchComponent timeComp = ( TimedSwitchComponent )s.getComponent( GlobalVars.TIMED_SWITCH_COMPONENT_NAME );
                            timeComp.baseTime = 0;
                        } else {
                            s = new TimedSwitchEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileWidth );
                            TimedSwitchComponent timeComp = ( TimedSwitchComponent )s.getComponent( GlobalVars.TIMED_SWITCH_COMPONENT_NAME );
                            timeComp.baseTime = col.G / 10;
                        }
                        s.isStartingEntity = true;
                        adjustLocation( s, level );
                        switches.Add( col.B, s );
                        level.addEntity( s.randId, s );

                    } else if ( col.R == spikeSwitchReserveRed ) {

                        float time = -1;
                        if ( col.G < 255 ) {
                            time = col.G / 10;
                        }
                        Entity s = new SpikeSwitchEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight, time );

                        s.isStartingEntity = true;
                        adjustLocation( s, level );
                        switches.Add( col.B, s );
                        level.addEntity( s.randId, s );

                    } else if ( col.R == shooterRedUp || col.R == shooterRedRight || col.R == shooterRedDown || col.R == shooterRedLeft ) {
                        float betweenBursts = ( float )col.B / ( float )10;
                        int switchId = col.G;
                        
                        TimedShooterEntity shooter = new TimedShooterEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight, betweenBursts, 1, col.R-110, switchId );
                        if ( switchId == 0 ) {
                            shooter.removeComponent( GlobalVars.SWITCH_LISTENER_COMPONENT_NAME );
                        } else {
                            SwitchListenerComponent slComp = ( SwitchListenerComponent )shooter.getComponent( GlobalVars.SWITCH_LISTENER_COMPONENT_NAME );
                            if ( switches.ContainsKey( switchId ) ) {
                                slComp.switchId = switches[switchId].randId;
                            } else {
                                unmachedSwitchListeners.Add( slComp, switchId );
                            }
                        }
                        adjustLocation( shooter, level );
                        shooter.isStartingEntity = true;
                        level.addEntity( shooter );

                    } else if ( col.R == smushRed && ( col.B - 4 <= 0 ) ) {
                        int switchId = col.G;
                        SmushBlockEntity smush = new SmushBlockEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight, col.B, switchId );

                        if ( switchId == 0 ) {
                            smush.removeComponent( GlobalVars.SWITCH_LISTENER_COMPONENT_NAME );
                        } else {
                            SwitchListenerComponent slComp = ( SwitchListenerComponent )smush.getComponent( GlobalVars.SWITCH_LISTENER_COMPONENT_NAME );
                            if ( switches.ContainsKey( switchId ) ) {
                                slComp.switchId = switches[switchId].randId;
                            } else {
                                unmachedSwitchListeners.Add( slComp, switchId );
                            }
                        }

                        adjustLocation( smush, level );
                        smush.isStartingEntity = true;
                        level.addEntity( smush );
                    } else if ( col.G == tallDoorReserveGreen || col.G == wideDoorReserveGreen || col.G == openTallDoorReserveGreen || col.G == openWideDoorReserveGreen) {

                        float width = tallDoorWidth;
                        float height = tallDoorHeight;

                        if ( col.G == wideDoorReserveGreen || col.G == openWideDoorReserveGreen) {
                            width = wideDoorWidth;
                            height = wideDoorHeight;
                        }

                        DoorEntity door = new DoorEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight, width, height, (col.G%100 == 0 || col.G%100 == 1));
                        adjustLocation( door, level );
                        SwitchListenerComponent slComp = ( SwitchListenerComponent )door.getComponent( GlobalVars.SWITCH_LISTENER_COMPONENT_NAME );
                        door.isStartingEntity = true;
                        //check for its switch
                        if ( switches.ContainsKey( col.B ) ) {
                            slComp.switchId = switches[col.B].randId;
                        } else {
                            unmachedSwitchListeners.Add( slComp, col.B );
                        }
                        level.addEntity( door );
                    
                    } else if ( col.R == spikeRed && col.G == spikeGreen && ( col.B - 4 <= 0 ) ) {

                        SpikeEntity spike = new SpikeEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight, col.B );
                        adjustLocation( spike, level );
                        spike.isStartingEntity = true;
                        level.addEntity( spike );

                    }

                    //Now just check for the specific colors
                    else if ( col == playerCol ) {

                        Player player = new Player( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight );
                        adjustLocation( player, level );
                        PositionComponent posComp = ( PositionComponent )player.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                        level.getMovementSystem().teleportToNoCollisionCheck( posComp, posComp.x, posComp.y - GlobalVars.MIN_TILE_SIZE / 2 );
                        posComp.setCurrentLocToStartingLoc();
                        player.isStartingEntity = true;
                        level.addEntity( player.randId, player );

                    } else if ( col == movePlatformTurn ) {

                        PlatformTurnEntity platTurn = new PlatformTurnEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight );
                        adjustLocation( platTurn, level );
                        platTurn.isStartingEntity = true;
                        level.addEntity( platTurn.randId, platTurn );

                    } else if ( col == basicGroundCol ) {

                        float groundX = ( levelX ) * tileWidth;
                        float groundWidth = tileWidth;

                        BasicGround ground = new BasicGround( level, rand.Next( Int32.MinValue, Int32.MaxValue ), groundX, ( levelY ) * tileHeight, groundWidth, tileHeight );
                        adjustLocation( ground, level );
                        ground.isStartingEntity = true;
                        level.addEntity( ground.randId, ground );

                        if ( !GlobalVars.fullForegroundImage ) {
                            //If no ground above it, change to a grass sprite
                            List<Entity> above = level.getCollisionSystem().findObjectAtPoint( ( levelX ) * tileWidth, ( levelY - 1 ) * tileWidth );
                            if ( above.Count <= 0 || !( above[0] is BasicGround ) ) {
                                ground.changeSprite( false );
                            }
                        }

                    } else if ( col == testEntityColor ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );
                        TestEntity test = new TestEntity( level, id, xLoc, yLoc );
                        adjustLocation( test, level );
                        test.isStartingEntity = true;
                        level.addEntity( test.randId, test );

                    } else if ( col == simpleEnemyColor ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );
                        SimpleEnemyEntity enemy = new SimpleEnemyEntity( level, id, xLoc, yLoc, false );
                        adjustLocation( enemy, level );
                        enemy.isStartingEntity = true;
                        level.addEntity( enemy.randId, enemy );

                    } else if ( col == flyingEnemyColor ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );
                        FlyingEnemyEntity enemy = new FlyingEnemyEntity( level, id, xLoc, yLoc, false );
                        adjustLocation( enemy, level );
                        enemy.isStartingEntity = true;
                        level.addEntity( enemy.randId, enemy );

                    } else if ( col == shieldWalkingEnemyColor ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );
                        SimpleEnemyEntity enemy = new SimpleEnemyEntity( level, id, xLoc, yLoc, true );
                        adjustLocation( enemy, level );
                        enemy.isStartingEntity = true;
                        level.addEntity( enemy.randId, enemy );
                    } else if ( col == shieldFlyingEnemyColor ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );
                        FlyingEnemyEntity enemy = new FlyingEnemyEntity( level, id, xLoc, yLoc, true );
                        adjustLocation( enemy, level );
                        enemy.isStartingEntity = true;
                        level.addEntity( enemy.randId, enemy );
                    } else if ( col == checkPointCollider ) {

                        CheckPointEntity checkEnt = new CheckPointEntity( level, rand.Next( Int32.MinValue, Int32.MaxValue ), levelX * tileWidth, levelY * tileHeight );
                        adjustLocation( checkEnt, level );
                        checkEnt.isStartingEntity = true;
                        level.addEntity( checkEnt.randId, checkEnt );

                    }/*else if ( col == checkPointCollider ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );
                        EndLevelEntity lvlEnd = new EndLevelEntity( level, id, xLoc, yLoc );
                        adjustLocation( lvlEnd, level );
                        lvlEnd.isStartingEntity = true;
                        level.addEntity( lvlEnd.randId, lvlEnd );

                    }*/ else if ( col == vertMovPlatCol ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        MovingPlatformEntity plat = new MovingPlatformEntity( level, id, xLoc, yLoc );
                        adjustLocation( plat, level );

                        plat.isStartingEntity = true;
                        level.addEntity( plat );

                    } /*else if ( col == horizMovPlatCol ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        MovingPlatformEntity plat = new MovingPlatformEntity( level, id, xLoc, yLoc );
                        adjustLocation( plat, level );

                        plat.isStartingEntity = true;
                        MovingPlatformComponent movPlatComp = ( MovingPlatformComponent )plat.getComponent( GlobalVars.MOVING_PLATFORM_COMPONENT_NAME );
                        movPlatComp.vertical = false;
                        VelocityComponent velComp = ( VelocityComponent )plat.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                        velComp.y = 0;
                        level.addEntity( plat );

                    }*/ else if ( col == bouncePickup ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        PowerupPickupEntity pickup = new PowerupPickupEntity( level, id, xLoc, yLoc, GlobalVars.BOUNCE_NUM );
                        adjustLocation( pickup, level );

                        pickup.isStartingEntity = true;
                        level.addEntity( pickup );

                    } else if ( col == speedyPickup ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        PowerupPickupEntity pickup = new PowerupPickupEntity( level, id, xLoc, yLoc, GlobalVars.SPEED_NUM );
                        adjustLocation( pickup, level );

                        pickup.isStartingEntity = true;
                        level.addEntity( pickup );

                    } else if ( col == jmpPickup ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        PowerupPickupEntity pickup = new PowerupPickupEntity( level, id, xLoc, yLoc, GlobalVars.JMP_NUM );
                        adjustLocation( pickup, level );

                        pickup.isStartingEntity = true;
                        level.addEntity( pickup );

                    } else if ( col == glidePickup ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        PowerupPickupEntity pickup = new PowerupPickupEntity( level, id, xLoc, yLoc, GlobalVars.GLIDE_NUM );
                        adjustLocation( pickup, level );

                        pickup.isStartingEntity = true;
                        level.addEntity( pickup );

                    } else if ( col == spawnPickup ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        PowerupPickupEntity pickup = new PowerupPickupEntity( level, id, xLoc, yLoc, GlobalVars.SPAWN_NUM );
                        adjustLocation( pickup, level );

                        pickup.isStartingEntity = true;
                        level.addEntity( pickup );

                    } else if ( col == grapPickup ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        PowerupPickupEntity pickup = new PowerupPickupEntity( level, id, xLoc, yLoc, GlobalVars.GRAP_NUM );
                        adjustLocation( pickup, level );

                        pickup.isStartingEntity = true;
                        level.addEntity( pickup );

                    } else if ( col == visionOrbUnlock ) {

                        float xLoc = ( levelX ) * tileWidth;
                        float yLoc = ( levelY ) * tileHeight;
                        int id = rand.Next( Int32.MinValue, Int32.MaxValue );

                        VisionOrbUnlock visUnlock = new VisionOrbUnlock( level, id, xLoc, yLoc );
                        adjustLocation( visUnlock, level );

                        visUnlock.isStartingEntity = true;
                        level.addEntity( visUnlock );

                    }
                }
            }

            //Match any unmatched switch listeners to their switch
            foreach ( SwitchListenerComponent sc in unmachedSwitchListeners.Keys ) {
                if ( switches.ContainsKey( unmachedSwitchListeners[sc] ) ) {
                    SwitchListenerComponent slComp = sc;
                    slComp.switchId = switches[unmachedSwitchListeners[sc]].randId;
                } else {
                    Console.WriteLine( "Unmatched Switch Listener - B: " + unmachedSwitchListeners[sc] );
                }
            }

        }

        //This is used each time an entitt is added. It centers the entity.
        public void adjustLocation( Entity e, Level level ) {
            PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            level.getMovementSystem().teleportToNoCollisionCheck( posComp, posComp.x + posComp.width / 2, posComp.y + posComp.height / 2 );
            posComp.setCurrentLocToStartingLoc();
        }

    }
}
