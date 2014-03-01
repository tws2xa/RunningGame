using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using RunningGame.Entities;
using RunningGame.Components;

namespace RunningGame.Systems {
    [Serializable()]
    public class SwitchListenerSystem : GameSystem {

        List<string> requiredComponents = new List<string>();
        Level level;

        //Map event types to methods
        Dictionary<string, Func<Entity, bool, bool>> events = new Dictionary<string, Func<Entity, bool, bool>>();

        public SwitchListenerSystem( Level level ) {
            //Here is where you add the Required components
            requiredComponents.Add( GlobalVars.SWITCH_LISTENER_COMPONENT_NAME ); //Position

            //Fill the events dictionary
            events.Add( GlobalVars.DOOR_EVENT_TYPE, doorSwitch );

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
            foreach ( Entity e in getApplicableEntities() ) {
                SwitchListenerComponent slComp = ( SwitchListenerComponent )e.getComponent( GlobalVars.SWITCH_LISTENER_COMPONENT_NAME );

                //If there has been a change, perform what is needed.
                if ( slComp.getChanged() ) {
                    bool handled = events[slComp.eventType].Invoke( e, slComp.getSwitchActive() );
                    if(handled) slComp.setChanged( false );
                }
            }
        }
        //--------------------------------------------------------------------------------------------------

        public bool doorSwitch( Entity e, bool active ) {
            //Opened
            if ( active ) {
                if ( e.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) ) {
                    e.removeComponent( GlobalVars.COLLIDER_COMPONENT_NAME );
                    level.getCollisionSystem().colliderRemoved( e );
                }
                DrawComponent drawComp = ( DrawComponent )e.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
                drawComp.setSprite( GlobalVars.DOOR_OPEN_SPRITE_NAME );
            }
                //Closed
            else {
                //Check for the player within the door.
                PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                PointF lowerLeft = new PointF(posComp.x - posComp.width/2, posComp.y + posComp.height/2);
                PointF upperRight = new PointF(posComp.x + posComp.width/2, posComp.y - posComp.height/2);
                List<Entity> cols = level.getCollisionSystem().findObjectsBetweenPoints( lowerLeft, upperRight );
                //Don't close on player.
                foreach ( Entity ent in cols ) {
                    if ( ent is Player ) {
                        VelocityComponent velComp = ( VelocityComponent )ent.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
                        PositionComponent playerComp = ( PositionComponent )ent.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
                        
                        float kickOutSpeed = 200.0f;
                        if ( playerComp.x > posComp.x ) {
                            velComp.setVelocity(kickOutSpeed, velComp.y);
                        } else {
                            velComp.setVelocity( -kickOutSpeed, velComp.y );
                        }

                        return false;
                    }
                }
                if ( !e.hasComponent( GlobalVars.COLLIDER_COMPONENT_NAME ) )
                    e.addComponent( new ColliderComponent( e, GlobalVars.BASIC_SOLID_COLLIDER_TYPE ) );
                DrawComponent drawComp = ( DrawComponent )e.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
                drawComp.setSprite( GlobalVars.DOOR_CLOSED_SPRITE_NAME );
            }

            return true; //Just cuz
        }
    }
}
