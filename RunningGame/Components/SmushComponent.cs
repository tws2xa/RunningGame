using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components {
    class SmushComponent :Component{

        float defaultFallSpd = 400;
        float defaultRiseSpd = -100;

        float xFallSpeed = 0.0f;
        float xRiseSpeed = 0.0f;
        float yFallSpeed = 400.0f;
        float yRiseSpeed = -100.0f;
        float upperWaitTime = 0.01f; //In seconds
        float lowerWaitTime = 0.5f; //In seconds

        bool hasInitializedTimer = false;
        bool frozen = false;
        int waitFallRiseState = 0; //0 = upper wait; 1 = Fall; 2 = lower wait; 3 = rise
        
        //Be sure to include a Constructor
        //Inside the constructor you probably want to pass in and set all the variables
        public SmushComponent(float upperWaitTime) {
            /*
             * Always always ALWAYS set the component name to whatever you have it as in GlobalVars (add one if need be)
             */
            componentName = GlobalVars.SMUSH_COMPONENT_NAME;
            this.upperWaitTime = upperWaitTime;
        }
        public SmushComponent(float upperWaitTime, float xFallSpeed, float xRiseSpeed, float yFallSpeed, float yRiseSpeed) {
            /*
             * Always always ALWAYS set the component name to whatever you have it as in GlobalVars (add one if need be)
             */
            componentName = GlobalVars.SMUSH_COMPONENT_NAME;
            this.upperWaitTime = upperWaitTime;
            this.xFallSpeed = xFallSpeed;
            this.xRiseSpeed = xRiseSpeed;
            this.yFallSpeed = yFallSpeed;
            this.yRiseSpeed = yRiseSpeed;
        }

        public void setHasInitializedTimer( bool val ) {
            hasInitializedTimer = val;
        }

        public System.Drawing.PointF getFallSpeed() {
            return new System.Drawing.PointF(xFallSpeed, yFallSpeed);
        }
        public System.Drawing.PointF getRiseSpeed() {
            return new System.Drawing.PointF(xRiseSpeed, yRiseSpeed);
        }

        public float getUpperWaitTime() {
            return upperWaitTime;
        }
        public void setUpperWaitTime( float val ) {
            this.upperWaitTime = val;
        }

        public float getLowerWaitTime() {
            return lowerWaitTime;
        }
        public void setLowerWaitTime( float val ) {
            this.lowerWaitTime = val;
        }
        public bool getInitializedTimer() {
            return hasInitializedTimer;
        }

        public void setStateUpperWait() {
            waitFallRiseState = 0;
        }
        public void setStateFall() {
            waitFallRiseState = 1;
        }
        public void setStateLowerWait() {
            waitFallRiseState = 2;
        }
        public void setStateRise() {
            waitFallRiseState = 3;
        }

        public bool isWaitingUpper() {
            return waitFallRiseState == 0;
        }
        public bool isFalling() {
            return waitFallRiseState == 1;
        }
        public bool isWaitingLower() {
            return waitFallRiseState == 2;
        }
        public bool isRising() {
            return waitFallRiseState == 3;
        }


        public void setToUp() {
            this.xFallSpeed = 0;
            this.xRiseSpeed = 0;
            this.yFallSpeed = -this.defaultFallSpd;
            this.yRiseSpeed = -this.defaultRiseSpd;
        }
        public void setToDown() {
            this.xFallSpeed = 0;
            this.xRiseSpeed = 0;
            this.yFallSpeed = this.defaultFallSpd;
            this.yRiseSpeed = this.defaultRiseSpd;
        }
        public void setToRight() {
            this.xFallSpeed = this.defaultFallSpd;
            this.xRiseSpeed = this.defaultRiseSpd;
            this.yFallSpeed = 0;
            this.yRiseSpeed = 0;
        }
        public void setToLeft() {
            this.xFallSpeed = -this.defaultFallSpd;
            this.xRiseSpeed = -this.defaultRiseSpd;
            this.yFallSpeed = 0;
            this.yRiseSpeed = 0;
        }

        public void setFrozen( bool frozen ) {
            this.frozen = frozen;
        }
        public bool isFrozen() {
            return frozen;
        }
    }
}
