using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RunningGame.Systems;
using RunningGame.Entities;

namespace RunningGame {

    /* 
     * This is the class that handles all the different systems.
     * It's basically a convinient way to initialize, update, and control
     * them from a central area.
     */
    [Serializable()]
    public class SystemManager {

        Level level;

        public DrawSystem drawSystem;
        public GravitySystem gravSystem;
        public MovementSystem moveSystem;
        public PlayerMovementSystem playerSystem;
        public VisionOrbSystem visSystem;
        public CollisionDetectionSystem colSystem;
        public HealthSystem healthSystem;
        public AnimationSystem animSystem;
        public SquishSystem squishSystem;
        public InputSystem inputSystem;
        public DebugSystem debugSystem;
        public ScreenEdgeSystem scrEdgeSystem;
        public SwitchListenerSystem slSystem;
        public SwitchSystem switchSystem;
        public SimplePowerUpSystem spSystem;
        public SimpleEnemyAISystem simpEnemySystem;
        public PlayerWeaponSystem weapSystem;
        public BackgroundPositionSystem bkgPosSystem;
        public MovingPlatformSystem movPlatSystem;
        public GrappleSystem grapSystem;
        public PushableSystem pushSystem;
        public TimerSystem timerSystem;
        public TimedShooterSystem timedShooterSystem;
        public VelToZeroSystem velZeroSystem;
        public SmushSystem smushSystem;

        List<GameSystem> systems = new List<GameSystem>();

        public SystemManager( Level level ) {

            this.level = level;

            initializeSystems();

        }

        // Create all systems
        public void initializeSystems() {
            gravSystem = new GravitySystem( level ); systems.Add( gravSystem );
            moveSystem = new MovementSystem( level ); systems.Add( moveSystem );
            playerSystem = new PlayerMovementSystem( level ); systems.Add( playerSystem );
            visSystem = new VisionOrbSystem( level ); systems.Add( visSystem );
            colSystem = new CollisionDetectionSystem( level ); systems.Add( colSystem );
            drawSystem = new DrawSystem( level.g, level ); systems.Add( drawSystem );
            healthSystem = new HealthSystem( level ); systems.Add( healthSystem );
            animSystem = new AnimationSystem( level ); systems.Add( animSystem );
            timerSystem = new TimerSystem( level ); systems.Add( timerSystem );
            timedShooterSystem = new TimedShooterSystem( level ); systems.Add( timedShooterSystem );
            squishSystem = new SquishSystem( level ); systems.Add( squishSystem );
            inputSystem = new InputSystem( level ); systems.Add( inputSystem );
            scrEdgeSystem = new ScreenEdgeSystem( level ); systems.Add( scrEdgeSystem );
            slSystem = new SwitchListenerSystem( level ); systems.Add( slSystem );
            switchSystem = new SwitchSystem( level ); systems.Add( switchSystem );
            spSystem = new SimplePowerUpSystem( level ); systems.Add( spSystem );
            simpEnemySystem = new SimpleEnemyAISystem( level ); systems.Add( simpEnemySystem );
            weapSystem = new PlayerWeaponSystem( level ); systems.Add( weapSystem );
            bkgPosSystem = new BackgroundPositionSystem( level ); systems.Add( bkgPosSystem );
            debugSystem = new DebugSystem( level ); systems.Add( debugSystem );
            movPlatSystem = new MovingPlatformSystem( level ); systems.Add( movPlatSystem );
            grapSystem = new GrappleSystem( level ); systems.Add( grapSystem );
            pushSystem = new PushableSystem( level ); systems.Add( pushSystem );
            velZeroSystem = new VelToZeroSystem( level ); systems.Add( velZeroSystem );
            smushSystem = new SmushSystem( level ); systems.Add( smushSystem );

        }


        //Game Logic Stuff
        public void Update( float deltaTime ) {
            velZeroSystem.Update( deltaTime );
            timerSystem.Update( deltaTime );
            moveSystem.Update( deltaTime );
            bkgPosSystem.Update( deltaTime );
            pushSystem.Update( deltaTime );
            movPlatSystem.Update( deltaTime );
            scrEdgeSystem.Update( deltaTime );
            playerSystem.Update( deltaTime );
            visSystem.Update( deltaTime );
            grapSystem.Update( deltaTime );
            timedShooterSystem.Update( deltaTime );
            spSystem.Update( deltaTime );
            weapSystem.Update( deltaTime );
            colSystem.Update( deltaTime );
            gravSystem.Update( deltaTime );
            drawSystem.Update( deltaTime );
            healthSystem.Update( deltaTime );
            animSystem.Update( deltaTime );
            squishSystem.Update( deltaTime );
            slSystem.Update( deltaTime );
            switchSystem.Update( deltaTime );
            simpEnemySystem.Update( deltaTime );
            smushSystem.Update( deltaTime );
            debugSystem.Update( deltaTime );
            inputSystem.Update( deltaTime );

        }

        //Notify collider system of a new collider
        public void colliderAdded( Entity e ) {
            colSystem.colliderAdded( e );
        }

        //Input
        public void KeyDown( KeyEventArgs e ) {
            inputSystem.KeyDown( e );
        }
        public void KeyUp( KeyEventArgs e ) {
            inputSystem.KeyUp( e );
        }
        public void KeyPressed( KeyPressEventArgs e ) {
            //Derp
        }
        public void MouseClick( MouseEventArgs e ) {
            //colSystem.MouseClick(e.X, e.Y); //This'll allow you to click and see which entities are in a cell
            inputSystem.MouseClick( e );
        }
        public void MouseMoved( MouseEventArgs e ) {
            inputSystem.MouseMoved( e );
        }

        //Any systems that require drawing
        public void Draw( System.Drawing.Graphics g ) {
            drawSystem.Draw( g );
            //colSystem.Draw(g);
        }

        public void ClearSystems() {
            foreach ( GameSystem sys in systems ) {
                sys.applicableEntities.Clear();
            }
        }
        public void entityAdded( Entity e ) {
            foreach ( GameSystem sys in systems ) {
                if ( sys.checkIfEntityIsApplicable( e ) ) {
                    if ( ( sys.actOnGround() || !( e is BasicGround ) ) && !sys.applicableEntities.ContainsKey( e.randId ) ) {
                        sys.applicableEntities.Add( e.randId, e );
                        //sys.applicableEntities = sys.applicableEntities.OrderBy(o => o.depth).ToList();
                    }
                }
            }
        }

        public void entityRemoved( Entity e ) {
            foreach ( GameSystem sys in systems ) {
                if ( ( sys.actOnGround() || !( e is BasicGround ) ) && sys.checkIfEntityIsApplicable( e ) ) {
                    if ( sys.applicableEntities.ContainsKey( e.randId ) )
                        sys.applicableEntities.Remove( e.randId );
                }
            }
        }

        public void componentRemoved( Entity e ) {
            foreach ( GameSystem sys in systems ) {
                if ( sys.applicableEntities.ContainsKey( e.randId ) )
                    if ( !sys.checkIfEntityIsApplicable( e ) ) sys.applicableEntities.Remove( e.randId );
            }
        }


        public void componentAdded( Entity e ) {
            foreach ( GameSystem sys in systems ) {
                if ( !sys.applicableEntities.ContainsKey( e.randId ) )
                    if ( sys.checkIfEntityIsApplicable( e ) ) sys.applicableEntities.Add( e.randId, e );
            }
        }
    }
}