using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;

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

    [Serializable()]
    class CollisionHandler
    {


        public Dictionary<string, Func<Entity, Entity, bool>> collisionDictionary = new Dictionary<string, Func<Entity, Entity, bool>>();
        public Level level;
        public Dictionary<string, Func<Entity, Entity, bool>> defaultCollisions = new Dictionary<string, Func<Entity, Entity, bool>>();

        public CollisionHandler(Level level)
        {

            this.level = level;

            //Func<Entity, Entity, bool> [Var Name] = [Name of your method];
            Func<Entity, Entity, bool> simpleStopCollisionFunction = simpleStopCollision;
            Func<Entity, Entity, bool> speedyPlayerCollisionFunction = speedyPlayerCollision;
            Func<Entity, Entity, bool> playerSwitchCollisonFunction = switchPlayerCollision;
            Func<Entity, Entity, bool> playerEnemyCollisionFunction = enemyPlayerCollision;
            Func<Entity, Entity, bool> bulletNonEnemyCollisionFunction = bulletNonEnemyCollision;
            Func<Entity, Entity, bool> bulletEnemyCollisionFunction = bulletEnemyCollision;
            Func<Entity, Entity, bool> endLevelCollisionFunction = endLevelCollision;
            Func<Entity, Entity, bool> doNothingCollisionFunction = doNothingCollision;
           
            //Set defaults (i.e. If there is no specific collision listed, what does it do?)
            defaultCollisions.Add(GlobalVars.BASIC_SOLID_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.BULLET_COLLIDER_TYPE, bulletNonEnemyCollision);
            defaultCollisions.Add(GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.END_LEVEL_COLLIDER_TYPE, simpleStopCollision);

            //Add non-default collisions to dictionary
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SPEEDY_COLLIDER_TYPE, speedyPlayerCollisionFunction);
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SWITCH_COLLIDER_TYPE, playerSwitchCollisonFunction);
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.END_LEVEL_COLLIDER_TYPE, endLevelCollisionFunction);

            addToDictionary(GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, playerEnemyCollisionFunction);
            
            addToDictionary(GlobalVars.BULLET_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, doNothingCollision);
            addToDictionary(GlobalVars.BULLET_COLLIDER_TYPE, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, bulletEnemyCollisionFunction);
        }

        public void setDefaultCollision(string colliderType, Func<Entity, Entity, bool> func)
        {
            defaultCollisions.Add(colliderType, func);
        }

        public void addToDictionary(string type1, string type2, Func<Entity, Entity, bool> func)
        {
            collisionDictionary.Add(getCollisionTypeName(type1, type2), func);
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

                if (defaultCollisions.ContainsKey(col1.colliderType))
                {
                    //If it is listed - call the collision method and return the stopMovement value
                    bool stopMovement = defaultCollisions[col1.colliderType](e1, e2);
                    return stopMovement;
                }
                else if (defaultCollisions.ContainsKey(col2.colliderType))
                {
                    //If it is listed - call the collision method and return the stopMovement value
                    bool stopMovement = defaultCollisions[col2.colliderType](e1, e2);
                    return stopMovement;
                }

                //If no default collision, do nothing
                return false; //Return false = do not stop the movement
            }
            else
            {
                //If it is listed - call the collision method and return the stopMovement value
                bool stopMovement = collisionDictionary[collisionTypeName](e1, e2);
                return stopMovement;
            }

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


        //------------------------------ COLLISION METHODS ---------------------------------------

        public bool doNothingCollision(Entity e1, Entity e2)
        {
            return false;
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
        public static bool glidePlayerCollision(Entity e1, Entity e2)
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
            Entity s;
            if (e1.hasComponent(GlobalVars.SWITCH_COMPONENT_NAME))
            {
                s = e1;
                sc = (SwitchComponent)e1.getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
            }
            else if(e2.hasComponent(GlobalVars.SWITCH_COMPONENT_NAME))
            {
                s = e2;
                sc = (SwitchComponent)e2.getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
            }
            else
            {
                Console.WriteLine("Switch collision with no switch?");
                return false;
            }

            if (!sc.active)
            {
                sc.setActive(true);
                DrawComponent drawComp = (DrawComponent)s.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
                drawComp.setSprite(GlobalVars.SWITCH_ACTIVE_SPRITE_NAME);
            }


            return false;

        }

        public static bool enemyPlayerCollision(Entity e1, Entity e2)
        {

            Player player;
            if (e1 is Player)
            {
                player = (Player)e1;
            }
            else if (e2 is Player)
            {
                player = (Player)e2;
            }
            else
            {
                Console.WriteLine("Enemy Player Collision with no player...");
                return false;
            }
            HealthComponent playerHealthComp = (HealthComponent)player.getComponent(GlobalVars.HEALTH_COMPONENT_NAME);
            playerHealthComp.subtractFromHealth(playerHealthComp.health + 1); //Kill the player O:
            return false;

        }
        
        public bool bulletNonEnemyCollision(Entity e1, Entity e2)
        {
            BulletEntity bullet;
            if (e1 is BulletEntity)
            {
                bullet = (BulletEntity)e1;
            }
            else if (e2 is BulletEntity)
            {
                bullet = (BulletEntity)e2;
            }
            else
            {
                Console.WriteLine("Bullet Collision with no bullet...");
                return false;
            }

            level.removeEntity(bullet);

            return false;
        }

        public bool bulletEnemyCollision(Entity e1, Entity e2)
        {
            level.removeEntity(e1);
            level.removeEntity(e2);

            return false;
        }

        public bool endLevelCollision(Entity e1, Entity e2)
        {
            level.shouldEndLevel = true;
            return false;
        }

    }
}
