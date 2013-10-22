using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace RunningGame
{

    /*
     * Global vars is just a useful static class to hold
     * game-wide constant variables.
     * They can be accessed by saying something like:
     * GlobalVars.NULL_COMPONENT_NAME;
     */
    [Serializable()]
    static class GlobalVars
    {
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
        public static string PRESSURE_SWITCH_COMPONENT_NAME = "pressureSwitchComp";

        //Collider Types
        public static string PLAYER_COLLIDER_TYPE = "playerCollider";
        public static string BASIC_SOLID_COLLIDER_TYPE = "basicSolidCollider";
        public static string INSTANT_DEATH_COLLIDER_TYPE = "instantDeathCollider";
        public static string GLIDE_COLLIDER_TYPE = "glideCollider";
        public static string POWERUP_COLLIDER_TYPE = "powerupCollider";
        public static string SPEEDY_COLLIDER_TYPE = "speedyCollider";
        public static string SWITCH_COLLIDER_TYPE = "switchCollider";
        public static string PRESSURE_SWITCH_COLLIDER_TYPE = "pressureSwitchCollider";
        public static string SIMPLE_ENEMY_COLLIDER_TYPE = "simpleEnemyCollider";
        public static string BULLET_COLLIDER_TYPE = "bulletCollider";

        //Collection of all in game entities
        public static Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
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

        //Key Bindings
        public static Keys KEY_JUMP = Keys.W;
        public static Keys KEY_LEFT = Keys.A;
        public static Keys KEY_RIGHT = Keys.D;

        //Switch Events
        public static string DOOR_EVENT_TYPE = "doorEvent";

        //Other Constants
        public static float MIN_TILE_SIZE = 10; //Width & Height, the smallest anything can be.
        public static string DOOR_OPEN_SPRITE_NAME = "openDoorSprite";
        public static string DOOR_CLOSED_SPRITE_NAME = "closedDoorSprite";
        public static string SWITCH_INACTIVE_SPRITE_NAME = "inactiveSwitch";
        public static string SWITCH_ACTIVE_SPRITE_NAME = "activeSwitch";
        public static float SIMPLE_ENEMY_H_SPEED = 100.0f;
        public static float BULLET_SPEED = 250.0f;
    }
}
