using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static string POSITION_COMPONENT_NAME = "position";
        public static string DRAW_COMPONENT_NAME = "draw";
        public static string VELOCITY_COMPONENT_NAME = "velocity";
        public static string GRAVITY_COMPONENT_NAME = "gravity";
        public static string PLAYER_COMPONENT_NAME = "playerInput";
        public static string COLLIDER_COMPONENT_NAME = "collider";

        //Collider Types
        public static string PLAYER_COLLIDER_TYPE = "playerCollider";
        public static string BASIC_SOLID_COLLIDER_TYPE = "basicSolidCollider";
        public static string INSTANT_DEATH_COLLIDER_TYPE = "instantDeathCollider";
        public static string POWERUP_COLLIDER_TYPE = "powerupCollider";

        //Collection of all in game entities
        public static Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
        //Collection of entities that are at the start of the level but have been removed.
        public static Dictionary<int, Entity> removedStartingEntities = new Dictionary<int, Entity>();

        //Standard Gravity for objects in game
        public static float STANDARD_GRAVITY = 200.0f;

        //Reading in images as levels.
        public static float LEVEL_READER_TILE_WIDTH = 10; //How wide is one pixel?
        public static float LEVEL_READER_TILE_HEIGHT = 10; //How tall is one pixel?

        //Other Constants
        public static float MIN_TILE_SIZE = 10; //Width & Height, the smallest anything can be.

    }
}
