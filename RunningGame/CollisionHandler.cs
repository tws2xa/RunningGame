using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame
{

    /*
     * The collision handler controls what happens when a collision occurs.
     * It does NOT detect the collisions. For that see the CollisionDetectionSystem class.
     * 
     * The collision dictionary stores keys for different types of collisions,
     * and functions wihch tell the game what to do when that collision occurs.
     * 
     * For example: One type of collision could be a basic solid collides with a basic solid.
     * The key would look something like "BasicSolidBasicSolid" (there's a key creation method,
     * so don't worry too much about that) - and the function would have code in it to simply
     * stop the two solids from moving.
     */

    class CollisionHandler
    {


        public Dictionary<string, Func<Entity, Entity, bool>> collisionDictionary = new Dictionary<string, Func<Entity, Entity, bool>>();


        public CollisionHandler()
        {

            //Func<Entity, Entity, bool> [Var Name] = [Name of your method];
            Func<Entity, Entity, bool> simpleStopCollisionFunction = simpleStopCollision;
            Func<Entity, Entity, bool> speedyPlayerCollisionFunction = speedyPlayerCollision;
            Func<Entity, Entity, bool> playerSwitchCollisonFunction = switchPlayerCollision;


            //Add collisions to dictionary
            collisionDictionary.Add(getCollisionTypeName(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.BASIC_SOLID_COLLIDER_TYPE), simpleStopCollisionFunction);
            collisionDictionary.Add(getCollisionTypeName(GlobalVars.BASIC_SOLID_COLLIDER_TYPE, GlobalVars.BASIC_SOLID_COLLIDER_TYPE),
                simpleStopCollisionFunction);
            collisionDictionary.Add(getCollisionTypeName(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SPEEDY_COLLIDER_TYPE), speedyPlayerCollisionFunction);
            collisionDictionary.Add(getCollisionTypeName(GlobalVars.BASIC_SOLID_COLLIDER_TYPE, GlobalVars.SPEEDY_COLLIDER_TYPE), simpleStopCollisionFunction);
            collisionDictionary.Add(getCollisionTypeName(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SWITCH_COLLIDER_TYPE), playerSwitchCollisonFunction);

        }
    
        //Return true = stop movement. False = do not stop movement.
        public bool handleCollision(Entity e1, Entity e2)
        {
            ColliderComponent col1 = (ColliderComponent)e1.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);
            ColliderComponent col2 = (ColliderComponent)e2.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);

            string collisionTypeName = getCollisionTypeName(col1.colliderType, col2.colliderType);

            //If it's not a listed collision type
            if (!collisionDictionary.ContainsKey(collisionTypeName))
            {
                //Do nothing
                return false; //Return false = do not stop the movement
            }
            else
            {
                //If it is listed - call the collision method and return the stopMovement value
                bool stopMovement = collisionDictionary[collisionTypeName](e1, e2);
                return stopMovement;
            }

        }

        public static bool simpleStopCollision(Entity e1, Entity e2)
        {
            return true; //Don't allow the movement.
        }

        public static bool speedyPlayerCollision(Entity e1, Entity e2)
        {
            Entity thePlayer = null;
            Entity other = null;
            //Speedy Code
            if (e1.hasComponent(GlobalVars.PLAYER_COMPONENT_NAME))
            {
                other = e2;
                thePlayer = e1;
            }
            else if (e2.hasComponent(GlobalVars.PLAYER_COMPONENT_NAME))
            {
                other = e1;
                thePlayer = e2;
            }

            if (thePlayer == null || other == null) return false;

            //Do collision code here

            return true;
        }

        public static bool switchPlayerCollision(Entity e1, Entity e2)
        {
            SwitchComponent sc;
            if (e1.hasComponent(GlobalVars.SWITCH_COMPONENT_NAME))
            {
                sc = (SwitchComponent)e1.getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
            }
            else if(e2.hasComponent(GlobalVars.SWITCH_COMPONENT_NAME))
            {
                sc = (SwitchComponent)e2.getComponent(GlobalVars.SWITCH_COMPONENT_NAME)
            } else
            {
                Console.WriteLine("Switch collision with no switch?");
                return false;
            }

            if(!sc.active)
                sc.setActive(true);
            return true;

        }

        public string getCollisionTypeName(string type1, string type2)
        {

            //This was giving stack overflow exceptions. Not sure why. Working on it. If you're having trouble, let me know.

            int num = String.CompareOrdinal(type1, type2); //?
            if (num < 0)
                return (type1 + "" + type2);
            else
                return (type2 + "" + type1);
        }
    }
}
