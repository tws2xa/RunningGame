using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RunningGame.Components
{
    //The entity has a sprite and will be drawn

    class DrawComponent:Component
    {

        //May or may not need graphics depending on how the Drawing System works
        public Bitmap sprite {get; set;}
        public float width {get; set;}
        public float height { get; set; }
        public bool sizeLocked { get; set; }
        //public Color myCol { get; set; }

        public DrawComponent(Bitmap sprite, float width, float height, bool sizeLocked)
        {
            this.componentName = GlobalVars.DRAW_COMPONENT_NAME;
            this.width = width;
            this.height = height;
            this.sizeLocked = sizeLocked;
            if (sizeLocked)
            {
                this.sprite = new Bitmap(sprite, new Size((int)Math.Ceiling(width), (int)Math.Ceiling(height)));
            }
            else
            {
                this.sprite = sprite;
            }
            
        }

        /*
        public DrawComponent(Color col)
        {
            this.componentName = GlobalVars.DRAW_COMPONENT_NAME;
            this.sprite = null;
            this.myCol = col;
        }
        */

    }
}
