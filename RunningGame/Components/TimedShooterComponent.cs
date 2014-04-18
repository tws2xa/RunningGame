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

        public string fireTimerString = "fire";

        public TimedShooterComponent(float timeBetweenBursts, int numShotsPerBurst) {
            this.componentName = GlobalVars.TIMED_SHOOTER_COMPONENT_NAME;
            this.timeBetweenBursts = timeBetweenBursts;
            this.numShotsPerBurst = numShotsPerBurst;
        }


        public void setHurtPlayer() {
            state = 0;
        }
        public void setHurtEnemy() {
            state = 1;
        }

        public bool shouldHurtPlayer() {
            return state == 0;
        }
        public bool shouldHurtEnemy() {
            return state == 1;
        }

    }
}
