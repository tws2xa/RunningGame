using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RunningGame.Entities;
using RunningGame.Components;

namespace RunningGame.Systems {
    [Serializable()]
    public class VisionOrbSystem : GameSystem {

        Level level;
        List<string> requiredComponents = new List<string>();
        public float horizSlowSpeed = 50.0f;

        public bool orbActive = false;

        bool hasRunOnce = false;
        Keys visionKey = Keys.V;
        float visionOrbSize = 20.0f;

        //Zoom Out
        int zoomOut = 50;

        //Fade
        bool doFade = true;
        float fadeTime = 0.15f;
        float fadeTimer = -1.0f;
        Color toOrbCol = Color.LightGray;
        Color toPlayerCol = Color.LightGray;

        //Border
        bool doBorder = true;
        SolidBrush borderBrush = (SolidBrush)Brushes.White;
        int mainBorderSize = 50;

        //Player Window
        bool doPlayerWindow = true;
        float plWinWidth = 200.0f;
        float plWinHeight = 170.0f;
        int plBorderSize = 5;
        SolidBrush plBorderBrush = (SolidBrush)Brushes.White;
        float plWinXLoc = 0; //Redefined in creation method
        float plWinYLoc = 0; //Redefined in creation method
        View plView = null;

        Random rand = new Random();

        public VisionOrbSystem(Level activeLevel) {
            //Required Components
            requiredComponents.Add(GlobalVars.VISION_ORB_INPUT_COMPONENT_NAME); //Player Input Component
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.VELOCITY_COMPONENT_NAME); //Velocity

            //Set the level
            level = activeLevel;

        }

        //-------------------------------- Overrides ----------------------------------
        public override Level GetActiveLevel() {
            return level;
        }
        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }
        public override void Update(float deltaTime) {

            if (!hasRunOnce) {
                level.getInputSystem().addKey(visionKey);
                hasRunOnce = true;
            }

            if (doFade && fadeTimer >= 0) {
                fadeTimer -= deltaTime;
                if (fadeTimer < 0) {
                    fadeTimer = -1.0f;
                    if (orbActive) {
                        destroyVisionOrb();
                        orbActive = false;
                    } else {
                        createVisionOrb();
                        orbActive = true;
                    }
                }
            }

            if (level.getInputSystem().myKeys[visionKey].down) {
                if (doFade) {
                    Color flashCol = toOrbCol;
                    if (orbActive) flashCol = toPlayerCol;
                    level.sysManager.drawSystem.setFlash(flashCol, fadeTime * 2);
                    fadeTimer = fadeTime;
                } else {
                    if (orbActive) {
                        destroyVisionOrb();
                        orbActive = false;
                    } else {
                        createVisionOrb();
                        orbActive = true;
                    }
                }

            }

            foreach (Entity e in getApplicableEntities()) {
                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                VelocityComponent velComp = (VelocityComponent)e.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
                VisionInputComponent visComp = (VisionInputComponent)e.getComponent(GlobalVars.VISION_ORB_INPUT_COMPONENT_NAME);
                checkForInput(posComp, velComp, visComp);

                if (orbActive && doPlayerWindow) {
                    checkPlayerWindowLocation();
                }

                //If there's a key down and the player isn't moving horizontally, check to make sure there's a collision
                if (Math.Abs(velComp.x) < Math.Abs(visComp.platformerMoveSpeed)) {
                    if (level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed) {
                        float leftX = (posComp.x - posComp.width / 2 - 1);
                        float upperY = (posComp.y - posComp.height / 2);
                        float lowerY = (posComp.y + posComp.height / 2);

                        if (!(level.getCollisionSystem().findObjectsBetweenPoints(leftX, upperY, leftX, lowerY).Count > 0)) {
                            beginMoveRight(posComp, velComp, visComp);
                        }
                    }
                    if (level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed) {
                        float rightX = (posComp.x + posComp.width / 2 + 1);
                        float upperY = (posComp.y - posComp.height / 2);
                        float lowerY = (posComp.y + posComp.height / 2);

                        if (!(level.getCollisionSystem().findObjectsBetweenPoints(rightX, upperY, rightX, lowerY).Count > 0)) {
                            beginMoveLeft(posComp, velComp, visComp);
                        }
                    }
                }
                if (Math.Abs(velComp.y) < Math.Abs(visComp.platformerMoveSpeed)) {
                    if (level.getInputSystem().myKeys[GlobalVars.KEY_JUMP].pressed) {

                        float rightX = (posComp.x + posComp.width / 2);
                        float upperY = (posComp.y - posComp.height / 2 - 1);
                        float leftX = (posComp.x + posComp.width / 2);

                        if (!(level.getCollisionSystem().findObjectsBetweenPoints(rightX, upperY, leftX, upperY).Count > 0)) {
                            beginMoveUp(posComp, velComp, visComp);
                        }
                    }
                }

                //Slow horizontal if no left/right key down
                if (velComp.x != 0 && !level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].pressed && !level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].pressed) {
                    if (velComp.x < 0) {
                        velComp.x += horizSlowSpeed;
                        if (velComp.x > 0)
                            velComp.x = 0;
                    } else {
                        velComp.x -= horizSlowSpeed;
                        if (velComp.x < 0)
                            velComp.x = 0;
                    }
                }
            }

        }
        //-----------------------------------------------------------------------------

        //----------------------------------- Input ----------------------------------- 
        public void checkForInput(PositionComponent posComp, VelocityComponent velComp, VisionInputComponent visInComp) {
            if (level.getInputSystem().myKeys[GlobalVars.KEY_JUMP].down) {
                beginMoveUp(posComp, velComp, visInComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_DOWN].down) {
                beginMoveDown(posComp, velComp, visInComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].down) {
                beginMoveLeft(posComp, velComp, visInComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].down) {
                beginMoveRight(posComp, velComp, visInComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_JUMP].up) {
                endUpperMove(posComp, velComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_DOWN].up) {
                endLowerMove(posComp, velComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_RIGHT].up) {
                endRightHorizontalMove(posComp, velComp);
            }
            if (level.getInputSystem().myKeys[GlobalVars.KEY_LEFT].up) {
                endLeftHorizontalMove(posComp, velComp);
            }
        }
        //--------------------------------------------------------------------------------



        //------------------------------------- Actions ----------------------------------

        public void beginMoveUp(PositionComponent posComp, VelocityComponent velComp, VisionInputComponent pelInComp) {
            velComp.setVelocity(velComp.x, -pelInComp.platformerMoveSpeed);
        }
        public void beginMoveDown(PositionComponent posComp, VelocityComponent velComp, VisionInputComponent pelInComp) {
            velComp.setVelocity(velComp.x, pelInComp.platformerMoveSpeed);
        }
        public void beginMoveLeft(PositionComponent posComp, VelocityComponent velComp, VisionInputComponent pelInComp) {
            velComp.setVelocity(-pelInComp.platformerMoveSpeed, velComp.y);
        }
        public void beginMoveRight(PositionComponent posComp, VelocityComponent velComp, VisionInputComponent pelInComp) {
            velComp.setVelocity(pelInComp.platformerMoveSpeed, velComp.y);
        }
        public void endLeftHorizontalMove(PositionComponent posComp, VelocityComponent velComp) {
            if (velComp.x < 0) velComp.setVelocity(0, velComp.y);
        }
        public void endRightHorizontalMove(PositionComponent posComp, VelocityComponent velComp) {
            if (velComp.x > 0) velComp.setVelocity(0, velComp.y);
        }
        public void endUpperMove(PositionComponent posComp, VelocityComponent velComp) {
            if (velComp.y < 0) velComp.setVelocity(velComp.x, 0);
        }
        public void endLowerMove(PositionComponent posComp, VelocityComponent velComp) {
            if (velComp.y > 0) velComp.setVelocity(velComp.x, 0);
        }
        //--------------------------------------------------------------------------------




        public void createVisionOrb() {
            if (level.getPlayer() == null) return;
            PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            Player player = (Player)level.getPlayer();
            VelocityComponent playerVel = (VelocityComponent)player.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            playerVel.x = 0;

            float x = posComp.x + getSpawnDistance(player);
            float y = posComp.y;


            if (level.getCollisionSystem().findObjectsBetweenPoints(x - visionOrbSize / 2, y - visionOrbSize / 2, x + visionOrbSize / 2, y + visionOrbSize / 2).Count > 0) return;
            if (level.getCollisionSystem().findObjectsBetweenPoints(x - visionOrbSize / 2, y + visionOrbSize / 2, x + visionOrbSize / 2, y - visionOrbSize / 2).Count > 0) return;

            player.stopAnimation();

            player.removeComponent(GlobalVars.PLAYER_INPUT_COMPONENT_NAME);

            VisionOrb newEntity = new VisionOrb(level, GenerateRandId(), x, y);
            level.addEntity(newEntity.randId, newEntity); //This should just stay the same

            level.sysManager.drawSystem.getMainView().setFollowEntity(newEntity);
            level.sysManager.drawSystem.getMainView().width += zoomOut;
            level.sysManager.drawSystem.getMainView().height += zoomOut;
            if (doBorder) {
                level.sysManager.drawSystem.getMainView().borderBrush = borderBrush;
                level.sysManager.drawSystem.getMainView().borderSize = mainBorderSize;
                level.sysManager.drawSystem.getMainView().hasBorder = true;
            }
            if (doPlayerWindow) {

                //Find player's quadrant
                if (posComp.x - level.sysManager.drawSystem.getMainView().x < level.sysManager.drawSystem.getMainView().width / 2) {
                    plWinXLoc = plBorderSize / 2;
                } else {
                    plWinXLoc = level.sysManager.drawSystem.getMainView().displayWidth - plWinWidth - plBorderSize / 2;
                }
                if (posComp.y - level.sysManager.drawSystem.getMainView().y < level.sysManager.drawSystem.getMainView().height / 2) {
                    plWinYLoc = plBorderSize / 2;
                } else {
                    plWinYLoc = level.sysManager.drawSystem.getMainView().displayHeight - plWinHeight - plBorderSize / 2;
                }

                plView = new View(plWinXLoc, plWinYLoc, plWinWidth, plWinHeight, level);
                plView.displayX = plWinXLoc;
                plView.displayY = plWinYLoc;
                plView.setFollowEntity(level.getPlayer());
                plView.hasBorder = true;
                plView.borderFade = false;
                plView.borderSize = plBorderSize;
                plView.borderBrush = plBorderBrush;
                level.sysManager.drawSystem.addView(plView);
            }
            orbActive = true;
        }

        public void destroyVisionOrb() {
            foreach (Entity e in getApplicableEntities()) {
                level.removeEntity(e);
            }

            level.sysManager.drawSystem.getMainView().setFollowEntity(level.getPlayer());
            level.sysManager.drawSystem.getMainView().width -= zoomOut;
            level.sysManager.drawSystem.getMainView().height -= zoomOut;
            if (doBorder) {
                level.sysManager.drawSystem.getMainView().hasBorder = false;
            }
            if (doPlayerWindow && plView != null) {
                if (!level.sysManager.drawSystem.removeView(plView)) {
                    level.sysManager.drawSystem.gotoJustMainView();
                }
                plView = null;
            }

            if (level.getPlayer() != null)
                level.getPlayer().addComponent(new PlayerInputComponent(level.getPlayer()));

            orbActive = false;
        }

        public float getSpawnDistance(Player player) {
            return level.sysManager.spSystem.getSpawnDistance(player); //Defer to Simple Powerup System's spawn distance.
        }


        public int GenerateRandId() {
            int id = rand.Next(Int32.MinValue, Int32.MaxValue);
            while (GlobalVars.removedStartingEntities.ContainsKey(id) || GlobalVars.nonGroundEntities.ContainsKey(id) || GlobalVars.groundEntities.ContainsKey(id)) {
                id = rand.Next(Int32.MinValue, Int32.MaxValue);
            }
            return id;
        }


        public void checkPlayerWindowLocation() {

            if (plView == null || level.getPlayer() == null) return;

            PositionComponent posComp = (PositionComponent)level.getPlayer().getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            //Find player's quadrant
            if (posComp.x - level.sysManager.drawSystem.getMainView().x < level.sysManager.drawSystem.getMainView().width / 2) {
                plWinXLoc = plBorderSize / 2;
            } else {
                plWinXLoc = level.sysManager.drawSystem.getMainView().displayWidth - plWinWidth - plBorderSize / 2;
            }
            if (posComp.y - level.sysManager.drawSystem.getMainView().y < level.sysManager.drawSystem.getMainView().height / 2) {
                plWinYLoc = plBorderSize / 2;
            } else {
                plWinYLoc = level.sysManager.drawSystem.getMainView().displayHeight - plWinHeight - plBorderSize / 2;
            }

            if (plWinXLoc != plView.displayX) plView.displayX = plWinXLoc;
            if (plWinYLoc != plView.displayY) plView.displayY = plWinYLoc;

        }




    }
}
