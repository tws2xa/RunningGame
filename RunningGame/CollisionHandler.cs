using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;

namespace RunningGame
{

    /*
     * The collision handler controls what happens when a collision occurs.
     * It does NOT detect the collisions. For that see the CollisionDetectionSystem class.
     * 
     * The collision dictionary stores keys for different types of collisions,
     * and functions which tell the game what to do when that collision occurs.
     * 
     * For example: One type of collision occurs when a basic solid collider collides with another basic solid collider.
     * The key would look something like "BasicSolidBasicSolid" (there's a key creation method,
     * so don't worry too much about that) - and the function would have code in it to simply
     * stop the two solids from moving (to do this - it returns true).
     */

    [Serializable()]
    class CollisionHandler
    {

        //The dictionary of special case collisions
        public Dictionary<string, Func<Entity, Entity, bool>> collisionDictionary = new Dictionary<string, Func<Entity, Entity, bool>>();
        //This dictionary holds default collision function
        public Dictionary<string, Func<Entity, Entity, bool>> defaultCollisions = new Dictionary<string, Func<Entity, Entity, bool>>();

        Random rand = new Random();

        //The level
        public Level level;

        public CollisionHandler(Level level)
        {
            //Set the level
            this.level = level;

            //Create the Func objects for all the collision functions
            //Func<Entity, Entity, bool> [Var Name] = [Name of your method];
            Func<Entity, Entity, bool> simpleStopCollisionFunction = simpleStopCollision;
            Func<Entity, Entity, bool> speedyPlayerCollisionFunction = speedyPlayerCollision;
            Func<Entity, Entity, bool> speedyGroundCollisionFunction = speedyGroundCollision;
            Func<Entity, Entity, bool> removeSpeedyCollisionFunction = removeSpeedyCollision;
            Func<Entity, Entity, bool> playerSwitchCollisonFunction = switchFlipCollision;
            Func<Entity, Entity, bool> playerEnemyCollisionFunction = enemyPlayerCollision;
            Func<Entity, Entity, bool> bulletNonEnemyCollisionFunction = bulletNonEnemyCollision;
            Func<Entity, Entity, bool> bulletEnemyCollisionFunction = DestroyBothCollision;
            Func<Entity, Entity, bool> endLevelCollisionFunction = endLevelCollision;
            Func<Entity, Entity, bool> doNothingCollisionFunction = doNothingCollision;
            Func<Entity, Entity, bool> otherPlatformCollisionFunction = platformOtherCollision;
           
            //Set defaults (i.e. If there is no specific collision listed, what does it do?)
            defaultCollisions.Add(GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, removeSpeedyCollision);
            defaultCollisions.Add(GlobalVars.BASIC_SOLID_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.BULLET_COLLIDER_TYPE, bulletNonEnemyCollision);
            defaultCollisions.Add(GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.END_LEVEL_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.MOVING_PLATFORM_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.SPEEDY_POSTGROUND_COLLIDER_TYPE, simpleStopCollision);
            

            //Add non-default collisions to dictionary
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SPEEDY_POSTGROUND_COLLIDER_TYPE, speedyPlayerCollisionFunction);
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SWITCH_COLLIDER_TYPE, playerSwitchCollisonFunction);
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.END_LEVEL_COLLIDER_TYPE, endLevelCollisionFunction);

            addToDictionary(GlobalVars.BULLET_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, doNothingCollision);
            addToDictionary(GlobalVars.BULLET_COLLIDER_TYPE, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, bulletEnemyCollisionFunction);
            addToDictionary(GlobalVars.BULLET_COLLIDER_TYPE, GlobalVars.SWITCH_COLLIDER_TYPE, switchFlipCollision);

            addToDictionary(GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, playerEnemyCollisionFunction);
            
            addToDictionary(GlobalVars.MOVING_PLATFORM_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, otherPlatformCollisionFunction);

            addToDictionary(GlobalVars.BASIC_SOLID_COLLIDER_TYPE, GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, speedyGroundCollisionFunction);
            addToDictionary(GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, doNothingCollision);
            addToDictionary(GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, doNothingCollision);
        }

        //This adds something to the default collison dictionary.
        public void setDefaultCollision(string colliderType, Func<Entity, Entity, bool> func)
        {
            defaultCollisions.Add(colliderType, func);
        }

        //This adds a specific collision to the special case collision dictionary
        public void addToDictionary(string type1, string type2, Func<Entity, Entity, bool> func)
        {
            collisionDictionary.Add(getCollisionTypeName(type1, type2), func);
        }

        //If this returns true - stop movement; False - do not stop movement.
        public bool handleCollision(Entity e1, Entity e2)
        {
            //Get the two colliders
            ColliderComponent col1 = (ColliderComponent)e1.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);
            ColliderComponent col2 = (ColliderComponent)e2.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);
 
            //Get the type of collision that has occured
            string collisionTypeName = getCollisionTypeName(col1.colliderType, col2.colliderType);

            //If it's not listed as a special case collision type - get the default collision
            if (!collisionDictionary.ContainsKey(collisionTypeName))
            {
                //Checks col1 first, then col2. This is arbitrary.
                if (defaultCollisions.ContainsKey(col1.colliderType))
                {
                    //Call the collision method and return the stopMovement value
                    bool stopMovement = defaultCollisions[col1.colliderType](e1, e2);
                    return stopMovement;
                }
                else if (defaultCollisions.ContainsKey(col2.colliderType))
                {
                    //If it is listed - call the collision method and return the stopMovement value
                    bool stopMovement = defaultCollisions[col2.colliderType](e1, e2);
                    return stopMovement;
                }

                //If no default collision is listed, and it's not in the special cases either - do nothing
                return false; //Return false = do not stop the movement
            }
            else //There is a special case
            {
                //Call the special case collision method and return the stopMovement value
                bool stopMovement = collisionDictionary[collisionTypeName](e1, e2);
                return stopMovement;
            }

        }

        //Returns the string that references the type of collision
        public string getCollisionTypeName(string type1, string type2)
        {

            //Basically puts the names in alphabetical order (I think) and combines them into one string.
            int num = string.CompareOrdinal(type1, type2); //?
            if (num < 0)
                return (type1 + "" + type2);
            else
                return (type2 + "" + type1);
        }

        //This update is just used if the end level timer has been started.
        public void update(float deltaTime)
        {
            
        }


        //------------------------------ COLLISION METHODS ---------------------------------------

        /*
         * Each collision method takes in two entities, and returns a bool.
         * The bool value is whetehr or not to stop the movement.
         * True = Stop Movement
         * False = Do Not Stop Movement
         */

        //Don't do anything
        public bool doNothingCollision(Entity e1, Entity e2)
        {
            return false;
        }

        //Just stop the movement
        public static bool simpleStopCollision(Entity e1, Entity e2)
        {
            return true; //Don't allow the movement.
        }

        public bool speedyGroundCollision(Entity e1, Entity e2)
        {
            Entity speedy = null;
            Entity theGround = null;

            if (e1 is BasicGround) 
            {
                theGround = e1;
                speedy = e2;
            }
            else if (e2 is BasicGround)
            {
                theGround = e2;
                speedy = e1;
            }
            else
            {
                //Console.WriteLine("SpeedyGroundCollision with no ground");
                return removeSpeedyCollision(e1, e2);
            }

            PositionComponent ground = (PositionComponent)theGround.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            System.Drawing.PointF loc = ground.getPointF();

            level.removeEntity(theGround);
            level.removeEntity(speedy);

            Entity newSpeedy = new Speedy(level, rand.Next(Int32.MinValue, Int32.MaxValue), loc.X, loc.Y - 1);
            level.addEntity(newSpeedy);

            return false;
        }

        //Speed the player up
        public bool speedyPlayerCollision(Entity e1, Entity e2)
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
        
             Player p = (Player)thePlayer;
            VelocityComponent vel = (VelocityComponent)p.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            if (vel.x >= 0)
            {
                vel.x = GlobalVars.SPEEDY_SPEED;
            } else {
                vel.x = -GlobalVars.SPEEDY_SPEED;
            }

            p.removeComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME);

            level.sysManager.spSystem.speedyTimer = level.sysManager.spSystem.speedyTime;
            level.sysManager.spSystem.speedyActive = true;

            return false;
        }

        //This flip the switch (assuming e1 or e2 is a switch)
        public static bool switchFlipCollision(Entity e1, Entity e2)
        {
            //Find out which one is the switch
            SwitchComponent sc; //The switch
            Entity s; //The other one
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
                //This should only occur when both neither e1 nor e2 is a switch.
                Console.WriteLine("Switch collision with no switch?");
                return false;
            }

            //If the switch is not active, make it active and switch the image
            if (!sc.active)
            {
                sc.setActive(true);
                DrawComponent drawComp = (DrawComponent)s.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
                drawComp.setSprite(GlobalVars.SWITCH_ACTIVE_SPRITE_NAME);
            }

            //Don't stop the movement
            return false;

        }

        //This occurs when the player collides with an enemy
        public static bool enemyPlayerCollision(Entity e1, Entity e2)
        {
            //Figure out which entity is the player
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

            //Decrease the player's health (In this case, it just removes it completely.
            HealthComponent playerHealthComp = (HealthComponent)player.getComponent(GlobalVars.HEALTH_COMPONENT_NAME);
            playerHealthComp.subtractFromHealth(playerHealthComp.health + 1); //Kill the player! D:
            
            //Don't bother stopping movement
            return false;

        }
        
        //When the bullet collides with something that isn't an enemy
        public bool bulletNonEnemyCollision(Entity e1, Entity e2)
        {
            //Get the bullet
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

            //Remove the bullet
            level.removeEntity(bullet);

            //Don't stop movement
            return false;
        }

        //When an enemy collides with a bullet
        public bool DestroyBothCollision(Entity e1, Entity e2)
        {
            //Remove both the enemy and the bullet
            level.removeEntity(e1);
            level.removeEntity(e2);
            return false;
        }


        //This ends the level
        public bool endLevelCollision(Entity e1, Entity e2)
        {
            //If the end level timer isn't already ticking...
            level.beginEndLevel();
            //Don't stop movement
            return false;
        }

        public bool platformOtherCollision(Entity e1, Entity e2)
        {
            MovingPlatformEntity plat;
            Entity other;
            if (e1 is MovingPlatformEntity)
            {
                plat = (MovingPlatformEntity)e1;
                other = e2;
            }
            else if (e2 is MovingPlatformEntity)
            {
                plat = (MovingPlatformEntity)e2;
                other = e1;
            }
            else
            {
                Console.WriteLine("Moving Plaform Collision without Moving Platform!");
                return false;
            }


            PositionComponent platPos = (PositionComponent)plat.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            PositionComponent otherPos = (PositionComponent)other.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

            float buffer = 2;

            //If other is not above the platform, just do a simple stop for other.
            float diff = (otherPos.y+otherPos.height/2) - (platPos.y-platPos.height/2);
            if (Math.Abs(diff) > buffer)
            {
                return true;
            }

            MovingPlatformComponent platComp = (MovingPlatformComponent)plat.getComponent(GlobalVars.MOVING_PLATFORM_COMPONENT_NAME);
            VelocityComponent platVel = (VelocityComponent)plat.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            VelocityComponent otherVel = (VelocityComponent)other.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            
            //If vertical, move other's y to the moving platform's
            if (platComp.vertical)
            {
                level.getMovementSystem().changePosition(otherPos, otherPos.x, platPos.y - platPos.height / 2 - otherPos.height / 2, false);
                otherVel.y = platVel.y;
            }

            return false;
        }

        public bool removeSpeedyCollision(Entity e1, Entity e2)
        {
            Entity pre = null;
            if (e1 is PreGroundSpeedy) pre = e1;
            else if (e2 is PreGroundSpeedy) pre = e2;
            else
            {
                Console.WriteLine("Remove Speedy with no Speedy");
                return false;
            }

            level.removeEntity(pre);

            return false;
        }

        /*
        public bool GrappleSolidCollision(Entity e1, Entity e2)
        {

            GrappleLinkEntity link = null;
            Entity other = null;
            if (e1 is GrappleLinkEntity)
            {
                link = (GrappleLinkEntity)e1;
                other = e2;

            } else if (e2 is GrappleLinkEntity)
            {
                link = (GrappleLinkEntity)e2;
                other = e1;
            } else
            {
                Console.WriteLine("Grapple Solid Collision with no grapple?");
            }

            GrappleLinkComponent grapComp = (GrappleLinkComponent)link.getComponent(GlobalVars.GRAPPLE_LINK_COMPONENT_NAME);
            grapComp.state = 3;

            return false;
        }
        */
    }
}
