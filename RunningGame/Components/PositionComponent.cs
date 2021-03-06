﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using RunningGame.Systems;
using System.Collections;

namespace RunningGame.Components {

    //The entity has an x, y, width, and height.

    [Serializable()]
    public class PositionComponent : Component {
        public float prevX { get; set; }
        public float prevY { get; set; }
        public float prevW { get; set; }
        public float prevH { get; set; }
        public float x, y;
        public float startingX, startingY;
        public float width;
        public float height;
        public float startingWidth, startingHeight;

        public bool positionHasChanged { get; set; }

        public Entity myEntity;
        public CollisionDetectionSystem colSys;

        //When a collision occurs, this becomes whatever it collided with
        //public List<Entity> collidedWith { get; set; }

        public PositionComponent( float x, float y, float w, float h, Entity myEntity ) {
            componentName = GlobalVars.POSITION_COMPONENT_NAME;
            this.prevX = x;
            this.prevY = y;
            this.x = x;
            this.y = y;
            startingX = x;
            startingY = y;
            this.prevW = w;
            this.prevH = h;
            this.width = w;
            this.height = h;
            this.startingWidth = w;
            this.startingHeight = h;

            this.myEntity = myEntity;

            if ( this.myEntity.level.sysManager != null && this.myEntity.level.sysManager.colSystem != null ) {
                colSys = this.myEntity.level.sysManager.colSystem;
            } else {
                colSys = null;
            }

            //collidedWith = new List<Entity>();
            positionHasChanged = false;
        }


        //Sets the current location as the starting location
        public void setCurrentLocToStartingLoc() {
            startingX = x;
            startingY = y;
        }

        //Returns an integer position as a Point
        public System.Drawing.Point getIntegerPoint() {
            return new System.Drawing.Point( ( int )Math.Round( x ), ( int )Math.Round( y ) );
        }
        public System.Drawing.PointF getLocAsPoint() {
            return new System.Drawing.PointF( x, y );
        }
        public System.Drawing.PointF getSizeAsPoint() {
            return new System.Drawing.PointF( width, height );
        }

    }
}
