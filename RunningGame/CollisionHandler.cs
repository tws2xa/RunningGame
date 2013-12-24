using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;

namespace RunningGame {

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
    class CollisionHandler {

        //The dictionary of special case collisions
        public Dictionary<string, Func<Entity, Entity, bool>> collisionDictionary = new Dictionary<string, Func<Entity, Entity, bool>>();
        //This dictionary holds default collision function
        public Dictionary<string, Func<Entity, Entity, bool>> defaultCollisions = new Dictionary<string, Func<Entity, Entity, bool>>();

        Random rand = new Random();

        //The level
        public Level level;


        public float playerSpikeCollisionLeewaySameDir = 3.0f; //Overlap needed for collision to trigger
        public float playerSpikeOppDirNecessaryOverlap = 5.0f; //If moving h, how much overlap must there be in v direction and vice versa

        //public float playerEnemyCollisionLeeway = 5.0f; //Overlap needed for collision to trigger

        public CollisionHandler(Level level) {
            //Set the level
            this.level = level;

            //Create the Func objects for all the collision functions
            //Func<Entity, Entity, bool> [Var Name] = [Name of your method];

            //Set defaults (i.e. If there is no specific collision listed, what does it do?)
            defaultCollisions.Add(GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, removeSpeedyCollision);
            defaultCollisions.Add(GlobalVars.BASIC_SOLID_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.BULLET_COLLIDER_TYPE, bulletNonEnemyCollision);
            defaultCollisions.Add(GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.END_LEVEL_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.MOVING_PLATFORM_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.SPEEDY_POSTGROUND_COLLIDER_TYPE, simpleStopCollision);
            defaultCollisions.Add(GlobalVars.POWERUP_PICKUP_COLLIDER_TYPE, doNothingCollision);
            defaultCollisions.Add(GlobalVars.SPIKE_COLLIDER_TYPE, doNothingCollision);
            defaultCollisions.Add(GlobalVars.VISION_COLLIDER_TYPE, doNothingCollision);
            defaultCollisions.Add(GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE, bounceGroundCollision);


            //Add non-default collisions to dictionary
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SPEEDY_POSTGROUND_COLLIDER_TYPE, speedyOtherCollision);
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SWITCH_COLLIDER_TYPE, switchFlipCollision);
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.END_LEVEL_COLLIDER_TYPE, endLevelCollision);
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.POWERUP_PICKUP_COLLIDER_TYPE, powerupPickupPlayerCollision);
            addToDictionary(GlobalVars.PLAYER_COLLIDER_TYPE, GlobalVars.SPIKE_COLLIDER_TYPE, spikePlayerCollision);

            addToDictionary(GlobalVars.BULLET_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, doNothingCollision);
            addToDictionary(GlobalVars.BULLET_COLLIDER_TYPE, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, DestroyBothCollision);
            addToDictionary(GlobalVars.BULLET_COLLIDER_TYPE, GlobalVars.SWITCH_COLLIDER_TYPE, switchFlipCollision);

            addToDictionary(GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE, GlobalVars.BASIC_SOLID_COLLIDER_TYPE, bounceGroundCollision);
            addToDictionary(GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, doNothingCollision);
            addToDictionary(GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, doNothingCollision);
            addToDictionary(GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE, GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE, doNothingCollision);

            addToDictionary(GlobalVars.BOUNCE_POSTGROUND_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, bouncePlayerCollision);
            addToDictionary(GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE, GlobalVars.BOUNCE_POSTGROUND_COLLIDER_TYPE, removeBounceCollision);
            addToDictionary(GlobalVars.BOUNCE_PREGROUND_COLLIDER_TYPE, GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE, removeBounceCollision);

            addToDictionary(GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, killPlayerCollision);
            addToDictionary(GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE, spawnEnemyCollision);

            addToDictionary(GlobalVars.MOVING_PLATFORM_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, platformOtherCollision);

            addToDictionary(GlobalVars.VISION_COLLIDER_TYPE, GlobalVars.BASIC_SOLID_COLLIDER_TYPE, simpleStopCollision);

            addToDictionary(GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, GlobalVars.BASIC_SOLID_COLLIDER_TYPE, speedyGroundCollision);
            addToDictionary(GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, GlobalVars.PLAYER_COLLIDER_TYPE, doNothingCollision);
            addToDictionary(GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE, doNothingCollision);
            addToDictionary(GlobalVars.SPEEDY_PREGROUND_COLLIDER_TYPE, GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE, removeSpeedyCollision);
            addToDictionary(GlobalVars.SPEEDY_POSTGROUND_COLLIDER_TYPE, GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE, speedyOtherCollision);
        }

        //This adds something to the default collison dictionary.
        public void setDefaultCollision(string colliderType, Func<Entity, Entity, bool> func) {
            defaultCollisions.Add(colliderType, func);
        }

        //This adds a specific collision to the special case collision dictionary
        public void addToDictionary(string type1, string type2, Func<Entity, Entity, bool> func) {
            collisionDictionary.Add(getCollisionTypeName(type1, type2), func);
        }

        //If this returns true - stop movement; False - do not stop movement.
        public bool handleCollision(Entity e1, Entity e2) {
            //Get the two colliders
            ColliderComponent col1 = (ColliderComponent)e1.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);
            ColliderComponent col2 = (ColliderComponent)e2.getComponent(GlobalVars.COLLIDER_COMPONENT_NAME);

            //Get the type of collision that has occured
            string collisionTypeName = getCollisionTypeName(col1.colliderType, col2.colliderType);

            //If it's not listed as a special case collision type - get the default collision
            if (!collisionDictionary.ContainsKey(collisionTypeName)) {
                //Checks if there's a doNothingCollision, if so it does nothing. Otherwise it pick's e1's default or e2s default.
                if (defaultCollisions.ContainsKey(col1.colliderType) && defaultCollisions[col1.colliderType] == doNothingCollision) {
                    return false;
                }
                if (defaultCollisions.ContainsKey(col2.colliderType) && defaultCollisions.ContainsKey(col2.colliderType)) {
                    if (defaultCollisions[col2.colliderType] == doNothingCollision)
                        return false;
                }

                bool stop1 = false;
                bool stop2 = false;
                if (defaultCollisions.ContainsKey(col1.colliderType)) {
                    stop1 = defaultCollisions[col1.colliderType](e1, e2);

                }
                if (defaultCollisions.ContainsKey(col2.colliderType)) {
                    //If it is listed - call the collision method and return the stopMovement value
                    stop2 = defaultCollisions[col2.colliderType](e1, e2);
                }

                //If no default collision is listed, and it's not in the special cases either - do nothing
                return (stop1 || stop2); //Return false = do not stop the movement
            } else //There is a special case
            {
                //Call the special case collision method and return the stopMovement value
                bool stopMovement = collisionDictionary[collisionTypeName](e1, e2);
                return stopMovement;
            }

        }

        //Returns the string that references the type of collision
        public string getCollisionTypeName(string type1, string type2) {

            //Basically puts the names in alphabetical order (I think) and combines them into one string.
            int num = string.CompareOrdinal(type1, type2); //?
            if (num < 0)
                return (type1 + "" + type2);
            else
                return (type2 + "" + type1);
        }

        //This update is just used if the end level timer has been started.
        public void update(float deltaTime) {

        }


        //------------------------------ COLLISION METHODS ---------------------------------------

        /*
         * Each collision method takes in two entities, and returns a bool.
         * The bool value is whetehr or not to stop the movement.
         * True = Stop Movement
         * False = Do Not Stop Movement
         */

        //Dont do anything for now, increase player.y component up whenever collide with block
        public bool bouncePlayerCollision(Entity e1, Entity e2) {
            float increment = 350f;
            Player player;
            Bounce bounce;
            if (e1 is Player) {
                player = (Player)e1;
                bounce = (Bounce)e2;
            } else if (e2 is Player) {
                player = (Player)e2;
                bounce = (Bounce)e1;
            } else {
                Console.WriteLine("Player Collision with no bounce block...");
                return false;
            }

            //PositionComponent pos = (PositionComponent)player.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            //pos.y= pos.y - increment;
            VelocityComponent vel = (VelocityComponent)player.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            vel.y = -increment;

            PositionComponent bouncePos = (PositionComponent)bounce.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            float xLoc = bouncePos.x;
            float yLoc = bouncePos.y;
            float changingX = xLoc + GlobalVars.MIN_TILE_SIZE;

            //Remove current bouncy
            removeSplatAddGround(bounce);

            //Remove adjacent bouncies
            List<Entity> adj = level.getCollisionSystem().findObjectAtPoint(changingX, yLoc);
            while (adj.Count > 0 && adj[0] is Bounce) {
                removeSplatAddGround(adj[0]);
                changingX += GlobalVars.MIN_TILE_SIZE;
                adj = level.getCollisionSystem().findObjectAtPoint(changingX, yLoc);
            }

            changingX = xLoc - GlobalVars.MIN_TILE_SIZE;
            adj = level.getCollisionSystem().findObjectAtPoint(changingX, yLoc);
            while (adj.Count > 0 && adj[0] is Bounce) {
                removeSplatAddGround(adj[0]);
                changingX -= GlobalVars.MIN_TILE_SIZE;
                adj = level.getCollisionSystem().findObjectAtPoint(changingX, yLoc);
            }

            return false;

        }

        public void removeSplatAddGround(Entity splat) {

            PositionComponent splatPos = (PositionComponent)splat.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            float xLoc = splatPos.x;
            float yLoc = splatPos.y + 1;

            level.removeEntity(splat);
            BasicGround ground = new BasicGround(level, GenerateRandId(), xLoc, yLoc, GlobalVars.MIN_TILE_SIZE, GlobalVars.MIN_TILE_SIZE);

            //If no ground above it, change to a grass sprite
            List<Entity> above = level.getCollisionSystem().findObjectAtPoint(xLoc, yLoc - GlobalVars.MIN_TILE_SIZE);
            if (above.Count <= 0 || !(above[0] is BasicGround)) {
                ground.changeSprite(false);
            }

            level.addEntity(ground);

        }

        public bool removeBounceCollision(Entity e1, Entity e2) {
            Entity preBounce = null;
            if (e1 is PreGroundBounce) {
                preBounce = e1;
            } else if (e2 is PreGroundBounce) {
                preBounce = e2;
            } else {
                Console.WriteLine("Error: Could not find Bouncy in removeBounceCollision");
                return false;
            }
            level.removeEntity(preBounce);
            return false;
        }
        public bool bounceGroundCollision(Entity e1, Entity e2) {
            Entity bounceB = null;
            Entity ground = null;
            if (e1 is BasicGround) {
                ground = e1;
                bounceB = e2;
            }
            if (e2 is BasicGround) {
                ground = e2;
                bounceB = e1;
            }
            if (ground == null || bounceB == null) return false;
            PositionComponent theGround = (PositionComponent)ground.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            System.Drawing.PointF loc = theGround.getPointF();

            level.removeEntity(ground);
            level.removeEntity(bounceB);

            Entity newBounceGround = new Bounce(level, GenerateRandId(), loc.X, loc.Y - 1);
            level.addEntity(newBounceGround);

            return false;
        }
        //Don't do anything
        public bool doNothingCollision(Entity e1, Entity e2) {
            return false;
        }

        //Just stop the movement
        public static bool simpleStopCollision(Entity e1, Entity e2) {
            return true; //Don't allow the movement.
        }

        public bool speedyGroundCollision(Entity e1, Entity e2) {
            Entity speedy = null;
            Entity theGround = null;

            if (e1 is BasicGround) {
                theGround = e1;
                speedy = e2;
            } else if (e2 is BasicGround) {
                theGround = e2;
                speedy = e1;
            } else {
                //Console.WriteLine("SpeedyGroundCollision with no ground");
                return removeSpeedyCollision(e1, e2);
            }

            PositionComponent ground = (PositionComponent)theGround.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            System.Drawing.PointF loc = ground.getPointF();

            level.removeEntity(theGround);
            level.removeEntity(speedy);

            Entity newSpeedy = new Speedy(level, GenerateRandId(), loc.X, loc.Y - 1);
            level.addEntity(newSpeedy);

            return false;
        }

        //Speed the player up
        public bool speedyOtherCollision(Entity e1, Entity e2) {
            Entity other = null;
            Entity speedyBlock = null;
            //Speedy Code
            if (e2 is Speedy) {
                speedyBlock = e2;
                other = e1;
            } else if (e2.hasComponent(GlobalVars.PLAYER_COMPONENT_NAME)) {
                speedyBlock = e1;
                other = e2;
            }

            if (other == null || speedyBlock == null) return false;

            //Do collision code here
            if (!other.hasComponent(GlobalVars.VELOCITY_COMPONENT_NAME)) return false;

            if (other is Player) {
                VelocityComponent vel = (VelocityComponent)other.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                if (vel.x > 0) {
                    vel.x = GlobalVars.SPEEDY_SPEED;
                } else if (vel.x < 0) {
                    vel.x = -GlobalVars.SPEEDY_SPEED;
                } else //velocity is 0
                {
                    //Go whichever way the player is looking
                    if (level.getPlayer().isLookingLeft()) {
                        vel.x = -GlobalVars.SPEEDY_SPEED;
                    } else {
                        vel.x = GlobalVars.SPEEDY_SPEED;
                    }
                }

                other.removeComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME);
            } else if (other is spawnBlockEntity) {
                if (level.getPlayer() == null) return true;

                PositionComponent plPos = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                PositionComponent spPos = (PositionComponent)other.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                VelocityComponent vel = (VelocityComponent)other.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                if (spPos.x >= plPos.x) {
                    vel.x = GlobalVars.SPEEDY_SPEED;
                } else {
                    vel.x = -GlobalVars.SPEEDY_SPEED;
                }

                SpawnBlockComponent spawnComp = (SpawnBlockComponent)other.getComponent(GlobalVars.SPAWN_BLOCK_COMPONENT_NAME);
                if (spawnComp.state == 0)
                    spawnComp.state = 1;
            }

            level.sysManager.spSystem.speedyTimer = level.sysManager.spSystem.speedyTime;
            level.sysManager.spSystem.speedyActive = true;



            PositionComponent speedyPos = (PositionComponent)speedyBlock.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            float xLoc = speedyPos.x;
            float yLoc = speedyPos.y;
            float changingX = xLoc + GlobalVars.MIN_TILE_SIZE;


            //Remove current bouncy
            removeSplatAddGround(speedyBlock);

            //Remove adjacent bouncies
            List<Entity> adj = level.getCollisionSystem().findObjectAtPoint(changingX, yLoc);
            while (adj.Count > 0 && adj[0] is Speedy) {
                removeSplatAddGround(adj[0]);
                changingX += GlobalVars.MIN_TILE_SIZE;
                adj = level.getCollisionSystem().findObjectAtPoint(changingX, yLoc);
            }

            changingX = xLoc - GlobalVars.MIN_TILE_SIZE;
            adj = level.getCollisionSystem().findObjectAtPoint(changingX, yLoc);
            while (adj.Count > 0 && adj[0] is Speedy) {
                removeSplatAddGround(adj[0]);
                changingX -= GlobalVars.MIN_TILE_SIZE;
                adj = level.getCollisionSystem().findObjectAtPoint(changingX, yLoc);
            }

            return false;
        }

        //This flip the switch (assuming e1 or e2 is a switch)
        public static bool switchFlipCollision(Entity e1, Entity e2) {
            //Find out which one is the switch
            SwitchComponent sc; //The switch
            Entity s; //The other one
            if (e1.hasComponent(GlobalVars.SWITCH_COMPONENT_NAME)) {
                s = e1;
                sc = (SwitchComponent)e1.getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
            } else if (e2.hasComponent(GlobalVars.SWITCH_COMPONENT_NAME)) {
                s = e2;
                sc = (SwitchComponent)e2.getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
            } else {
                //This should only occur when both neither e1 nor e2 is a switch.
                Console.WriteLine("Switch collision with no switch?");
                return false;
            }

            //If the switch is not active, make it active and switch the image
            if (!sc.active) {
                sc.setActive(true);
                DrawComponent drawComp = (DrawComponent)s.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
                drawComp.setSprite(GlobalVars.SWITCH_ACTIVE_SPRITE_NAME);
            }

            //Don't stop the movement
            return false;

        }

        //This occurs when the player collides with an enemy
        public static bool killPlayerCollision(Entity e1, Entity e2) {
            //Figure out which entity is the player
            Player player;
            if (e1 is Player) {
                player = (Player)e1;
            } else if (e2 is Player) {
                player = (Player)e2;
            } else {
                Console.WriteLine("Kill Player Collision with no player...");
                return false;
            }

            //Decrease the player's health (In this case, it just removes it completely.
            HealthComponent playerHealthComp = (HealthComponent)player.getComponent(GlobalVars.HEALTH_COMPONENT_NAME);
            playerHealthComp.subtractFromHealth(playerHealthComp.health + 1); //Kill the player! D:

            //Do bother stopping movement
            return true;

        }

        //When the bullet collides with something that isn't an enemy
        public bool bulletNonEnemyCollision(Entity e1, Entity e2) {
            //Get the bullet
            BulletEntity bullet;
            if (e1 is BulletEntity) {
                bullet = (BulletEntity)e1;
            } else if (e2 is BulletEntity) {
                bullet = (BulletEntity)e2;
            } else {
                Console.WriteLine("Bullet Collision with no bullet...");
                return false;
            }

            //Remove the bullet
            level.removeEntity(bullet);

            //Don't stop movement
            return false;
        }

        //When an enemy collides with a bullet
        public bool DestroyBothCollision(Entity e1, Entity e2) {
            //Remove both the enemy and the bullet
            level.removeEntity(e1);
            level.removeEntity(e2);
            return false;
        }


        //This ends the level
        public bool endLevelCollision(Entity e1, Entity e2) {
            //If the end level timer isn't already ticking...
            level.beginEndLevel();
            //Don't stop movement
            return false;
        }

        public bool platformOtherCollision(Entity e1, Entity e2) {
            MovingPlatformEntity plat;
            Entity other;
            if (e1 is MovingPlatformEntity) {
                plat = (MovingPlatformEntity)e1;
                other = e2;
            } else if (e2 is MovingPlatformEntity) {
                plat = (MovingPlatformEntity)e2;
                other = e1;
            } else {
                Console.WriteLine("Moving Plaform Collision without Moving Platform!");
                return false;
            }


            PositionComponent platPos = (PositionComponent)plat.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            PositionComponent otherPos = (PositionComponent)other.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

            float buffer = 2;

            //If other is not above the platform, just do a simple stop for other.
            float diff = (otherPos.y + otherPos.height / 2) - (platPos.y - platPos.height / 2);
            if (Math.Abs(diff) > buffer) {
                return true;
            }

            MovingPlatformComponent platComp = (MovingPlatformComponent)plat.getComponent(GlobalVars.MOVING_PLATFORM_COMPONENT_NAME);
            VelocityComponent platVel = (VelocityComponent)plat.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            VelocityComponent otherVel = (VelocityComponent)other.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);

            //If vertical, move other's y to the moving platform's
            if (platComp.vertical) {
                level.getMovementSystem().changePosition(otherPos, otherPos.x, platPos.y - platPos.height / 2 - otherPos.height / 2, false);
                otherVel.y = platVel.y;
            }

            return false;
        }

        public bool removeSpeedyCollision(Entity e1, Entity e2) {
            Entity pre = null;
            if (e1 is PreGroundSpeedy) pre = e1;
            else if (e2 is PreGroundSpeedy) pre = e2;
            else {
                Console.WriteLine("Remove Speedy with no Speedy");
                return false;
            }

            level.removeEntity(pre);

            return false;
        }

        public bool powerupPickupPlayerCollision(Entity e1, Entity e2) {
            Entity pickup = null;
            if (e1 is PowerupPickupEntity) pickup = e1;
            else if (e2 is PowerupPickupEntity) pickup = e2;
            else {
                Console.WriteLine("Powerup Pickup Collision with no Powerup Pickup!");
                return false;
            }

            PowerupPickupComponent ppComp = (PowerupPickupComponent)pickup.getComponent(GlobalVars.POWERUP_PICKUP_COMPONENT_NAME);
            level.sysManager.spSystem.unlockPowerup(ppComp.compNum);

            System.Drawing.Color col = System.Drawing.Color.Peru; //Peru is a color.

            switch (ppComp.compNum) {
                case (GlobalVars.BOUNCE_NUM):
                    col = System.Drawing.Color.Purple;
                    break;
                case (GlobalVars.SPEED_NUM):
                    col = System.Drawing.Color.Teal;
                    break;
                case (GlobalVars.JMP_NUM):
                    col = System.Drawing.Color.LightGreen;
                    break;
                case (GlobalVars.GLIDE_NUM):
                    col = System.Drawing.Color.Yellow;
                    break;
                case (GlobalVars.SPAWN_NUM):
                    col = System.Drawing.Color.Orange;
                    break;
                case (GlobalVars.GRAP_NUM):
                    col = System.Drawing.Color.Red;
                    break;
            }

            level.sysManager.drawSystem.setFlash(col, 0.5f);

            level.removeEntity(pickup);

            return false;
        }

        public bool spawnEnemyCollision(Entity e1, Entity e2) {
            Entity spawnBlock = null;
            Entity other = null;

            if (e1 is spawnBlockEntity) { spawnBlock = e1; other = e2; } else if (e2 is spawnBlockEntity) { spawnBlock = e2; other = e1; } else { Console.WriteLine("Spawn Enemy Collision with no Spawn!"); return false; }

            SpawnBlockComponent spComp = (SpawnBlockComponent)spawnBlock.getComponent(GlobalVars.SPAWN_BLOCK_COMPONENT_NAME);

            if (spComp.state == 2) {
                if (level.removeEntity(other))
                    level.removeEntity(spawnBlock);
            } else if (spComp.state == 1) {
                if (level.removeEntity(other))
                    spComp.state = 2;
            } else if (spComp.state == 0) {
                return simpleStopCollision(e1, e2);
            }

            return false;

        }

        public bool spikePlayerCollision(Entity e1, Entity e2) {
            SpikeEntity spike = null;
            Player player = null;

            if (e1 is Player) {
                player = (Player)e1;
                spike = (SpikeEntity)e2;
            } else if (e2 is Player) {
                player = (Player)e2;
                spike = (SpikeEntity)e1;
            } else {
                Console.WriteLine("Error: Spike Player Collision with no player!");
                return false;
            }

            DirectionalComponent dirComp = (DirectionalComponent)spike.getComponent(GlobalVars.DIRECTION_COMPONENT_NAME);
            PositionComponent posSpikes = (PositionComponent)spike.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            PositionComponent posPlayer = (PositionComponent)player.getComponent(GlobalVars.POSITION_COMPONENT_NAME);



            if (checkSpikeCollision(posPlayer, posSpikes, dirComp.dir % 4)) {
                switch (dirComp.dir % 4) {
                    case (0): //Player needs to be above spikes
                        //Console.WriteLine("Above " + (posSpikes.y - posSpikes.height / 2 + playerSpikeCollisionLeewaySameDir) + ", " + (posPlayer.y + posPlayer.height / 2));
                        return killPlayerCollision(e1, e2);
                    case (1)://Player needs to be right of spikes
                        //Console.WriteLine("Right " + (posSpikes.x + posSpikes.width / 2 - playerSpikeCollisionLeewaySameDir) + ", " + (posPlayer.x - posPlayer.width / 2));
                        return killPlayerCollision(e1, e2);
                    case (2):
                        //Console.WriteLine("Below " + (posSpikes.y + posSpikes.height / 2 - playerSpikeCollisionLeewaySameDir) + ", " + (posPlayer.y - posPlayer.height / 2));
                        return killPlayerCollision(e1, e2);
                    case (3)://Player needs to be left of spikes
                        //Console.WriteLine("Left " + (posSpikes.x - posSpikes.width / 2 + playerSpikeCollisionLeewaySameDir) + ", " + (posPlayer.x + posPlayer.width / 2));
                        return killPlayerCollision(e1, e2);
                }
            }

            return false;

        }


        private bool checkSpikeCollision(PositionComponent posPlayer, PositionComponent posSpikes, int dir) {

            switch (dir) {
                case (0): //Player Above
                    if ((posSpikes.y - posSpikes.height / 2 + playerSpikeCollisionLeewaySameDir) <= (posPlayer.y + posPlayer.height / 2)) {
                        float playerLeft = posPlayer.x - posPlayer.width / 2;
                        float playerRight = posPlayer.x + posPlayer.width / 2;
                        float spikeLeft = posSpikes.x - posSpikes.width / 2;
                        float spikeRight = posSpikes.x + posSpikes.width / 2;

                        if (((posSpikes.x < posPlayer.x) & (Math.Abs(playerLeft - spikeRight) > playerSpikeOppDirNecessaryOverlap))
                            || ((posSpikes.x >= posPlayer.x) & (Math.Abs(playerRight - spikeLeft) > playerSpikeOppDirNecessaryOverlap))) {
                            return true;
                        }
                        return false;
                    } else return false;
                case (1)://Player right
                    if ((posSpikes.x + posSpikes.width / 2 - playerSpikeCollisionLeewaySameDir) >= (posPlayer.x - posPlayer.width / 2)) {
                        float playerUp = posPlayer.y - posPlayer.height / 2;
                        float playerBottom = posPlayer.y + posPlayer.height / 2;
                        float spikeUp = posSpikes.y - posSpikes.height / 2;
                        float spikeBottom = posSpikes.y + posSpikes.height / 2;

                        if (((posSpikes.y < posPlayer.y) & (Math.Abs(playerUp - spikeBottom) > playerSpikeOppDirNecessaryOverlap))
                            || ((posSpikes.y >= posPlayer.y) & (Math.Abs(playerBottom - spikeUp) > playerSpikeOppDirNecessaryOverlap))) {
                            return true;
                        }
                        return false;
                    } else return false;
                case (2): //Player Below
                    if ((posSpikes.y + posSpikes.height / 2 - playerSpikeCollisionLeewaySameDir) >= (posPlayer.y - posPlayer.height / 2)) {
                        float playerLeft = posPlayer.x - posPlayer.width / 2;
                        float playerRight = posPlayer.x + posPlayer.width / 2;
                        float spikeLeft = posSpikes.x - posSpikes.width / 2;
                        float spikeRight = posSpikes.x + posSpikes.width / 2;

                        if (((posSpikes.x < posPlayer.x) & (Math.Abs(playerLeft - spikeRight) > playerSpikeOppDirNecessaryOverlap))
                            || ((posSpikes.x >= posPlayer.x) & (Math.Abs(playerRight - spikeLeft) > playerSpikeOppDirNecessaryOverlap))) {
                            return true;
                        }
                        return false;
                    } else return false;
                case (3)://Player left
                    if ((posSpikes.x - posSpikes.width / 2 + playerSpikeCollisionLeewaySameDir) <= (posPlayer.x + posPlayer.width / 2)) {
                        float playerUp = posPlayer.y - posPlayer.height / 2;
                        float playerBottom = posPlayer.y + posPlayer.height / 2;
                        float spikeUp = posSpikes.y - posSpikes.height / 2;
                        float spikeBottom = posSpikes.y + posSpikes.height / 2;

                        if (((posSpikes.y < posPlayer.y) & (Math.Abs(playerUp - spikeBottom) > playerSpikeOppDirNecessaryOverlap))
                            || ((posSpikes.y >= posPlayer.y) & (Math.Abs(playerBottom - spikeUp) > playerSpikeOppDirNecessaryOverlap))) {
                            return true;
                        }
                        return false;
                    } else return false;
                default:
                    return false;
            }


        }

        public int GenerateRandId() {
            int id = rand.Next(Int32.MinValue, Int32.MaxValue);
            while (GlobalVars.removedStartingEntities.ContainsKey(id) || GlobalVars.nonGroundEntities.ContainsKey(id) || GlobalVars.groundEntities.ContainsKey(id)) {
                id = rand.Next(Int32.MinValue, Int32.MaxValue);
            }
            return id;
        }

    }
}
