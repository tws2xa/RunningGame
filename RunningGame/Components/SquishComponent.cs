using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    class SquishComponent:Component
    {


        //"Base" width and height of the entity
        public float baseWidth;
        public float baseHeight;

        //Multipliers for how severely a difference from center value affects the width/height
        public float xIncSpeedStretchMultiplier = 0.06f;
        public float yIncSpeedStretchMultiplier = 0.12f;
        public float xDecSpeedStretchMultiplier = 0.03f;
        public float yDecSpeedStretchMultiplier = 0.07f;

        //Max strech in either direction
        public float maxWidth;
        public float maxHeight;

        //Min stretch in either direction
        public float minWidth;
        public float minHeight;

        //Max and Min Surface Area
        public float maxSurfaceArea;
        public float minSurfaceArea;

        //Used to ease back into shape instead
        public float xEaseRate = 1f;
        public float yEaseRate = 1f;

        //Used for resizieng based off of acceleration
        public float prevXVelocity = 0;
        public float prevYVelocity = 0;

        //At what point does it start changing?
        public float xStretchThreshold = 0.1f;
        public float yStretchThreshold = 0.1f;

        public SquishComponent(float baseWidth, float baseHeight, float maxWidth, float maxHeight, float minWidth, float minHeight, float maxSurfaceArea, float minSurfaceArea)
        {

            this.componentName = GlobalVars.SQUISH_COMPONENT_NAME;

            this.baseWidth = baseWidth;
            this.baseHeight = baseHeight;

            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;

            this.minWidth = minWidth;
            this.minHeight = minHeight;

            this.maxSurfaceArea = maxWidth * maxHeight;
            this.minSurfaceArea = minWidth * minHeight;

            this.maxSurfaceArea = maxSurfaceArea;
            this.minSurfaceArea = minSurfaceArea;
        }
        public SquishComponent(float baseWidth, float baseHeight, float maxWidth, float maxHeight, float minWidth, float minHeight)
        {
            
            this.componentName = GlobalVars.SQUISH_COMPONENT_NAME;

            this.baseWidth = baseWidth;
            this.baseHeight = baseHeight;

            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;

            this.minWidth = minWidth;
            this.minHeight = minHeight;

            this.maxSurfaceArea = maxWidth*maxHeight;
            this.minSurfaceArea = minWidth*minHeight;
        }

    }
}
