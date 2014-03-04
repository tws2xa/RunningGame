using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace RunningGame {

    /*
     * Global vars is just a useful static class to hold
     * game-wide constant variables.
     * They can be accessed by saying something like:
     * GlobalVars.NULL_COMPONENT_NAME;
     */
    [Serializable()]
    static class GlobalVars {

        //Menu
        public static int worldNum = 0;

        //Component names
        public static string NULL_COMPONENT_NAME = "NULL";
        public static string POSITION_COMPONENT_NAME = "positionComp";
        public static string DRAW_COMPONENT_NAME = "drawComp";
        public static string VELOCITY_COMPONENT_NAME = "velocityComp";
        public static string GRAVITY_COMPONENT_NAME = "gravityComp";
        public static string PLAYER_COMPONENT_NAME = "playerComp";
        public static string COLLIDER_COMPONENT_NAME = "colliderComp";
        public static string HEALTH_COMPONENT_NAME = "healthComp";
        public static string ANIMATION_COMPONENT_NAME = "animationComp";
        public static string SQUISH_COMPONENT_NAME = "squishComp";
        public static string SCREEN_EDGE_COMPONENT_NAME = "screenEdgeComp";
        public static string PLAYER_INPUT_COMPONENT_NAME = "playerInputComp";
        public static string SWITCH_COMPONENT_NAME = "switchComp";
        public static string SWITCH_LISTENER_COMPONENT_NAME = "switchListenerComp";
        public static string SOUND_COMPONENT_NAME = "soundComp";
        public static string GLIDE_COMPONENT_NAME = "glideComp";
        public static string SIMPLE_ENEMY_COMPONENT_NAME = "simpleEnemyComp";
        public static string TIMED_SWITCH_COMPONENT_NAME = "timedSwitchComp";
        public static string OBJECT_LINK_COMPONENT_NAME = "objLinkedComp";
        public static string BACKGROUND_COMPONENT_NAME = "backgroundComp";
        public static string MOVING_PLATFORM_COMPONENT_NAME = "movPlatComp";
        public static string GRAPPLE_COMPONENT_NAME = "grappleComponent";
        public static string POWERUP_PICKUP_COMPONENT_NAME = "pwrupPkpComp";
        public static string SPAWN_BLOCK_COMPONENT_NAME = "spawnBlkComp";
        public static string DIRECTION_COMPONENT_NAME = "spikeComp";
        public static string VISION_ORB_INPUT_COMPONENT_NAME = "visionInputComp";
        public static string PUSHABLE_COMPONENT_NAME = "pushableComp";
        public static string TIMER_COMPONENT_NAME = "timerComp";
        public static string TIMED_SHOOTER_COMPONENT_NAME = "timedShooterComp";
        public static string VEL_TO_ZERO_COMPONENT_NAME = "velToZeroComp";

        //Collider Types
        public static string PLAYER_COLLIDER_TYPE = "playerCollider";
        public static string BASIC_SOLID_COLLIDER_TYPE = "basicSolidCollider";

        public static string BOUNCE_POSTGROUND_COLLIDER_TYPE = "bounceBlockCollider";
        public static string BOUNCE_PREGROUND_COLLIDER_TYPE = "bounceGroundCollider";

        public static string INSTANT_DEATH_COLLIDER_TYPE = "instantDeathCollider";
        public static string GLIDE_COLLIDER_TYPE = "glideCollider";
        public static string POWERUP_COLLIDER_TYPE = "powerupCollider";
        public static string SPEEDY_PREGROUND_COLLIDER_TYPE = "speedyPreGroundCollider";
        public static string SPEEDY_POSTGROUND_COLLIDER_TYPE = "speedyPostGroundCollider";
        public static string SWITCH_COLLIDER_TYPE = "switchCollider";
        public static string SIMPLE_ENEMY_COLLIDER_TYPE = "simpleEnemyCollider";
        public static string BULLET_COLLIDER_TYPE = "bulletCollider";
        public static string END_LEVEL_COLLIDER_TYPE = "endLevelCollider";
        public static string MOVING_PLATFORM_COLLIDER_TYPE = "movPlatCollider";
        public static string POWERUP_PICKUP_COLLIDER_TYPE = "pwrupPkpCollider";
        public static string SPAWN_BLOCK_COLLIDER_TYPE = "spawnBlock";
        public static string SPIKE_COLLIDER_TYPE = "killPlayerCollider";
        public static string VISION_COLLIDER_TYPE = "visionCollider";
        public static string PLATFORM_TURN_COLLIDER_TYPE = "platTurnCollider";
        public static string SHOOTER_BULLET_COLLIDER_TYPE = "shooterBulletCollider";
        public static string TIMED_SHOOTER_COLLIDER_TYPE = "timedShooterCollider";

        //Collection of all in game entities
        public static Dictionary<int, Entity> nonGroundEntities = new Dictionary<int, Entity>();
        public static Dictionary<int, Entity> groundEntities = new Dictionary<int, Entity>();
        //Collection of entities that are at the start of the level but have been removed.
        public static Dictionary<int, Entity> removedStartingEntities = new Dictionary<int, Entity>();

        //For storing images that have already been read in (image address, image)
        public static Dictionary<string, Bitmap> imagesInStore = new Dictionary<string, Bitmap>();

        //Standard Gravity for objects in game
        public static float STANDARD_GRAVITY = 200.0f;

        //Standard Glide (lower gravity)for object in game
        public static float STANDARD_GLIDE = 150.0f;

        //Reading in images as levels.
        public static float LEVEL_READER_TILE_WIDTH = 10; //How wide is one pixel?
        public static float LEVEL_READER_TILE_HEIGHT = 10; //How tall is one pixel?

        //Powerup nums
        public const int BOUNCE_NUM = 1; //Purple
        public const int SPEED_NUM = 2; //Blue
        public const int JMP_NUM = 3; //Green
        public const int GLIDE_NUM = 4; //Yellow
        public const int SPAWN_NUM = 5; //Orange
        public const int GRAP_NUM = 6; //Red


        //Key Bindings
        public static Keys KEY_JUMP = Keys.W;
        public static Keys KEY_LEFT = Keys.A;
        public static Keys KEY_RIGHT = Keys.D;
        public static Keys KEY_DOWN = Keys.S;
        public static Keys KEY_RESET = Keys.R;
        public static Keys KEY_CYCLE_DOWN = Keys.Q;
        public static Keys KEY_CYCLE_UP = Keys.E;
        public static Keys KEY_USE_EQUIPPED = Keys.F;
        public static Keys KEY_GLIDE = Keys.Space;
        public static Keys KEY_END = Keys.Escape;

        //Switch Events
        public static string DOOR_EVENT_TYPE = "doorEvent";

        //2d List of level file names ([world][level])
        public static List<List<string>> levels = new List<List<string>>();

        //Other Constants
        public static int numWorlds = 6;
        public static int numLevelsPerWorld = 3;
        public static float MIN_TILE_SIZE = 10; //Width & Height, the smallest anything can be.
        public static string DOOR_OPEN_SPRITE_NAME = "openDoorSprite";
        public static string DOOR_CLOSED_SPRITE_NAME = "closedDoorSprite";
        public static string SWITCH_INACTIVE_SPRITE_NAME = "inactiveSwitch";
        public static string SWITCH_ACTIVE_SPRITE_NAME = "activeSwitch";
        public static float SIMPLE_ENEMY_H_SPEED = 50.0f;
        public static float BULLET_SPEED = 250.0f;
        public static float MOVING_PLATFORM_SPEED = 50.0f;
        public static float MAX_GRAPPLE_DISTANCE = 500.0f; //Pixels
        public static int MAX_NUM_BULLETS = 2;
        public static float SPEEDY_SPEED = 600.0f;
        public static Bitmap grndImg = null;
        public static float playerAnimatonSpeed = 0.07f;
        public static int doubleJumpNumAirJumps = 1;
        public static int normNumAirJumps = 0;
        public static int numAirJumps = 0;
        public static float PLAYER_HORIZ_MOVE_SPEED = 150.0f;
        public static string PRECOLOR_SPRITE_NAME = "preColSprite";
        public static string MAIN_SPRITE_NAME = "MainSprite";

        //Settings
        public static bool preciseCollisionChecking = false;
        public static bool soundOn = true;
        public static bool fullForegroundImage = false;
        public static bool simpleGround = true;
        public static bool debugBoxes = false;

        public static List<Keys> reservedKeys = new List<Keys>(){
       Keys.A, Keys.W, Keys.D
        };
    }
}
