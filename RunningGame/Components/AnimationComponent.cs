using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    [Serializable()]
    public class AnimationComponent : Component {

        public float animationFrameTime { get; set; } //Time in seconds between frames
        public float timeUntilNextFrame { get; set; } //Time before next frame switch
        public float pauseTimeAfterCycle { get; set; } //Pause for a certain time after each animation cycle?
        public bool pauseIndefinitelyAfterCycle { get; set; }
        public bool animationOn;
        public string imageAfterCycleName = null;

        public AnimationComponent( float animationFrameTime ) {
            /*
             * Always always ALWAYS set the component name to whatever you have it as in GlobalVars (add one if need be)
             */
            componentName = GlobalVars.ANIMATION_COMPONENT_NAME;

            this.animationFrameTime = animationFrameTime;
            timeUntilNextFrame = animationFrameTime;
            animationOn = true;
            pauseTimeAfterCycle = 0.0f;
            pauseIndefinitelyAfterCycle = false;

        }

    }
}
