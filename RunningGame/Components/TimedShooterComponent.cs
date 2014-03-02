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

        public string fireTimerString = "fire";

        public TimedShooterComponent(float timeBetweenBursts, int numShotsPerBurst) {
            this.componentName = GlobalVars.TIMED_SHOOTER_COMPONENT;
            this.timeBetweenBursts = timeBetweenBursts;
            this.numShotsPerBurst = numShotsPerBurst;
        }

    }
}
