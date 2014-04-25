using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    public class TimedShooterComponent : Component{

        public float timeBetweenBursts = 0.0f;
        public int numShotsPerBurst = 1;
        public float timeBetweenShotsInBurst = 0.05f;
        public int currentBurstNum = 0;
        public int state = 0; //0 = hurt player, 1 = hurt enemy
        public Entity myEnt;

        public string badSpriteName = "badShooter";
        public string goodShooterName = "goodShooter";

        public string fireTimerString = "fire";

        public TimedShooterComponent(float timeBetweenBursts, int numShotsPerBurst, Entity myEnt) {
            this.componentName = GlobalVars.TIMED_SHOOTER_COMPONENT_NAME;
            this.timeBetweenBursts = timeBetweenBursts;
            this.numShotsPerBurst = numShotsPerBurst;
            this.myEnt = myEnt;
        }


        public void setHurtPlayer() {
            state = 0;
            DrawComponent drawComp = ( DrawComponent )myEnt.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            if ( drawComp != null ) {
                int imgIndex = drawComp.getSprite().currentImageIndex;
                drawComp.setSprite( this.badSpriteName, false );
                drawComp.getSprite().currentImageIndex = imgIndex;
            }
        }
        public void setHurtEnemy() {
            state = 1;
            DrawComponent drawComp = ( DrawComponent )myEnt.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            if ( drawComp != null ) {
                int imgIndex = drawComp.getSprite().currentImageIndex;
                drawComp.setSprite( this.goodShooterName, false );
                drawComp.getSprite().currentImageIndex = imgIndex;
            }
        }

        public bool shouldHurtPlayer() {
            return state == 0;
        }
        public bool shouldHurtEnemy() {
            return state == 1;
        }

    }
}
