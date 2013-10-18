using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    [Serializable()]
    class SimpleEnemyComponent:Component
    {
        public float mySpeed;
        public bool wasStoppedLastFrame { get; set; }
        public bool hasTouchedGround { get; set; }
        public bool checkCliff;
        public bool hasLandedOnce { get; set; }
        public bool hasRunOnce { get; set; }

        public SimpleEnemyComponent(float mySpeed)
        {
            this.componentName = GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME;
            this.mySpeed = mySpeed;
            wasStoppedLastFrame = false;
            hasLandedOnce = false;
            hasRunOnce = false;
            checkCliff = true;
        }

    }
}
