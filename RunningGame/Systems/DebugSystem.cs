﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Entities;
using RunningGame.Components;
using System.Windows.Forms;
using System.Drawing;

namespace RunningGame.Systems {
    [Serializable()]
    public class DebugSystem : GameSystem {
        List<string> requiredComponents = new List<string>();
        Level level;

        Keys addEntityKey = Keys.N;
        Keys harmPlayerKey = Keys.H;
        Keys resetLevelKey = GlobalVars.KEY_RESET;
        Keys endLevelKey = GlobalVars.KEY_END;
        Keys skipLevelKey = Keys.F1;
        Keys typeKey = Keys.T; //prints out the all entity types to console
        Keys infoKey = Keys.I;


        Keys toggleBounce = Keys.D1;
        Keys toggleSpeedy = Keys.D2;
        Keys toggleDoubleJump = Keys.D3;
        Keys toggleGlide = Keys.D4;
        Keys toggleSpawn = Keys.D5;
        Keys toggleGrapple = Keys.D6;



        bool hasRunOnce = false; //Used to add keys once and only once. Can't in constructor because inputSystem not ready yet
        bool addingDoor = false; //False = adding Switch

        int switchId;

        public DebugSystem( Level level ) {
            this.level = level; //Always have this
        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }

        //Must have this. Same for all Systems.
        public override Level GetActiveLevel() {
            return level;
        }

        public override void Update( float deltaTime ) {

            if ( !hasRunOnce ) {
                level.getInputSystem().addKey( addEntityKey );
                level.getInputSystem().addKey( harmPlayerKey );
                level.getInputSystem().addKey( resetLevelKey );
                level.getInputSystem().addKey( endLevelKey );
                level.getInputSystem().addKey( skipLevelKey );
                level.getInputSystem().addKey( typeKey );
                level.getInputSystem().addKey( infoKey );


                level.getInputSystem().addKey( toggleBounce );
                level.getInputSystem().addKey( toggleDoubleJump );
                level.getInputSystem().addKey( toggleGlide );
                level.getInputSystem().addKey( toggleGrapple );
                level.getInputSystem().addKey( toggleSpeedy );
                level.getInputSystem().addKey( toggleSpawn );

                hasRunOnce = true;
            }

            checkForInput();
        }
        //----------------------------------------------------------------------------------------------


        public void checkForInput() {
            /*
            if ( level.getInputSystem().myKeys[addEntityKey].down ) {
                PositionComponent posComp = ( PositionComponent )level.getPlayer().getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                debugAddEntity( posComp.x + posComp.width * 1.5f, posComp.y );
                //addDoorOrSwitch(posComp.x + posComp.width * 1.5f, posComp.y);
            }

            if ( level.getInputSystem().myKeys[harmPlayerKey].down ) {
                HealthComponent healthComp = ( HealthComponent )level.getPlayer().getComponent( GlobalVars.HEALTH_COMPONENT_NAME );
                healthComp.subtractFromHealth( 25 );
            }


            if ( level.getInputSystem().myKeys[toggleBounce].down ) {
                togglePowerup( GlobalVars.BOUNCE_NUM );
            }
            if ( level.getInputSystem().myKeys[toggleSpeedy].down ) {
                togglePowerup( GlobalVars.SPEED_NUM );
            }
            if ( level.getInputSystem().myKeys[toggleDoubleJump].down ) {
                togglePowerup( GlobalVars.JMP_NUM );
            }
            if ( level.getInputSystem().myKeys[toggleGlide].down ) {
                togglePowerup( GlobalVars.GLIDE_NUM );
            }
            if ( level.getInputSystem().myKeys[toggleGrapple].down ) {
                togglePowerup( GlobalVars.GRAP_NUM );
            }
            if ( level.getInputSystem().myKeys[toggleSpawn].down ) {
                togglePowerup( GlobalVars.SPAWN_NUM );
            }

            if ( level.getInputSystem().myKeys[typeKey].down ) {
                getTypes();
            }
            */

            if ( level.getInputSystem().myKeys[infoKey].down ) {
                getInfo();
            }

            if ( level.getInputSystem().myKeys[resetLevelKey].up ) {
                level.resetLevel();
            }
            if (level.getInputSystem().myKeys[addEntityKey].up)
            {
                level.sysManager.drawSystem.activateTextFlash("hellooo", Color.CornflowerBlue, 0.5f, 2, 1);
            }

            if ( level.getInputSystem().myKeys[skipLevelKey].up ) {
                level.beginEndLevel( 0.0f );
            } else if ( level.getInputSystem().myKeys[endLevelKey].up ) {
                level.worldNum = GlobalVars.numWorlds;
                level.levelNum = GlobalVars.numLevelsPerWorld;
                level.escapeEnd = true;
                level.beginEndLevel( 0.0f );
            }

        }

        /*
         * Here is where you change which entitiy pressing N will add
         * All you should really have to do is change where it says
         * TestEntity to whatever you want to create.
         */
        public void getTypes() {
            List<Entity> entities = getApplicableEntities();

            foreach ( Entity e in entities ) {
                if ( !( e is BasicGround ) )
                    Console.WriteLine( e );
            }

        }

        public void getInfo() {

            Console.WriteLine( "Controls:" );

            Console.WriteLine( "Left: " + GlobalVars.KEY_LEFT );
            Console.WriteLine( "Right: " + GlobalVars.KEY_RIGHT );
            Console.WriteLine( "Up: " + GlobalVars.KEY_JUMP );


            /*
            foreach ( Entity e in GlobalVars.nonGroundEntities.Values ) {
                if ( e is MovingPlatformEntity ) {
                    printInfo( e );
                }
            }
            */
        }

        public void printInfo( Entity e ) {
            Console.WriteLine( "Info For: " + e );
            Console.WriteLine( "\t------ COMPONENTS ------" );
            foreach(Component c in e.getComponents()) {
                Console.WriteLine("\t" + c);
            }
            if(e.hasComponent(GlobalVars.POSITION_COMPONENT_NAME)) {
                Console.WriteLine( "\t------ POSITION ------" );
                PositionComponent posComp = ( PositionComponent )e.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                Console.WriteLine("\t(" + posComp.x + ", " + posComp.y + ")");
            }
        }

        void togglePowerup( int pupNum ) {
            level.sysManager.spSystem.togglePowerup( pupNum );
        }

        public void makeFlash( float time, Color color ) {
            DrawSystem ds = level.sysManager.drawSystem;
            ds.setFlash( color, time );
        }
        public void debugAddEntity( float x, float y ) {

            //Entity newEntity = new [YOUR ENTITY HERE](level, x, y);
            Entity newEntity = new PreGroundSpeedy( level, x, y );
            level.addEntity( newEntity.randId, newEntity ); //This should just stay the same
        }

        /*
        public void addDoorOrSwitch( float x, float y ) {
            if ( addingDoor ) {
                DoorEntity d = new DoorEntity( level, x, y - 20, switchId );
                level.addEntity( d.randId, d );
                addingDoor = false;
            } else {
                TimedSwitchEntity s = new TimedSwitchEntity( level, x, y );
                switchId = s.randId;
                level.addEntity( s.randId, s );
                addingDoor = true;
            }
        }
        */
    }
}
