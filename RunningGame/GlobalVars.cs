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

    static class GlobalVars
    {
        //Component names
        public static string NULL_COMPONENT_NAME = "NULL";
        public static string POSITION_COMPONENT_NAME = "positionComp";
        public static string DRAW_COMPONENT_NAME = "drawComp";
        public static string VELOCITY_COMPONENT_NAME = "velocityComp";
        public static string GRAVITY_COMPONENT_NAME = "gravityComp";
        public static string PLAYER_COMPONENT_NAME = "playerInputComp";
        public static string COLLIDER_COMPONENT_NAME = "colliderComp";
        public static string HEALTH_COMPONENT_NAME = "healthComp";
        public static string ANIMATION_COMPONENT_NAME = "animationComp";
        public static string SQUISH_COMPONENT_NAME = "squishComp";
        public static string SCREEN_WRAP_COMPONENT_NAME = "screenWrapComp";

        //Collider Types
        public static string PLAYER_COLLIDER_TYPE = "playerCollider";
        public static string BASIC_SOLID_COLLIDER_TYPE = "basicSolidCollider";
        public static string INSTANT_DEATH_COLLIDER_TYPE = "instantDeathCollider";
        public static string POWERUP_COLLIDER_TYPE = "powerupCollider";
        public static string SPEEDY_COLLIDER = "speedyCollider";

        //Collection of all in game entities
        public static Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
        //Collection of entities that are at the start of the level but have been removed.
        public static Dictionary<int, Entity> removedStartingEntities = new Dictionary<int, Entity>();

        //For storing images that have already been read in (image address, image)
        public static Dictionary<string, Bitmap> imagesInStore = new Dictionary<string, Bitmap>();

        //Standard Gravity for objects in game
        public static float STANDARD_GRAVITY = 200.0f;

        //Reading in images as levels.
        public static float LEVEL_READER_TILE_WIDTH = 10; //How wide is one pixel?
        public static float LEVEL_READER_TILE_HEIGHT = 10; //How tall is one pixel?

        //Key Bindings
        public static Keys KEY_JUMP = Keys.W;
        public static Keys KEY_LEFT = Keys.A;
        public static Keys KEY_RIGHT = Keys.D;

        //Other Constants
        public static float MIN_TILE_SIZE = 10; //Width & Height, the smallest anything can be.

    }
}
