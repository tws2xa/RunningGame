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
        //public Bitmap sprite {get; set;}
        public Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
        public float width {get; set;}
        public float height { get; set; }
        public bool sizeLocked { get; set; }
        public string activeSprite;
        //public Color myCol { get; set; }

        public DrawComponent(String spriteAddress, String spriteName, float width, float height, bool sizeLocked)
        {


            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream myStream = myAssembly.GetManifestResourceStream(spriteAddress);
            Bitmap sprite = new Bitmap(myStream);

            this.componentName = GlobalVars.DRAW_COMPONENT_NAME;
            this.width = width;
            this.height = height;
            this.sizeLocked = sizeLocked;
            if (sizeLocked)
            {
                sprite = new Bitmap(sprite, new Size((int)Math.Ceiling(width), (int)Math.Ceiling(height)));
            }
            activeSprite = spriteName;
            images.Add(spriteName, sprite);
            
        }

        public void addImage(string spriteAddress, string spriteName)
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream myStream = myAssembly.GetManifestResourceStream(spriteAddress);
            Bitmap sprite = new Bitmap(myStream);

            if (sizeLocked)
            {
                sprite = new Bitmap(sprite, new Size((int)Math.Ceiling(width), (int)Math.Ceiling(height)));
            }

            images.Add(spriteName, sprite);
        }
        public Image getSprite()
        {
            return images[activeSprite];
        }

        public void rotateFlipSprite(string spriteName, RotateFlipType rotation)
        {
            if(images.ContainsKey(spriteName))
                images[spriteName].RotateFlip(rotation);
            else
                Console.WriteLine("Trying to rotate/flip a nonexistant image: " + spriteName);
        }

        public void setSprite(string spriteName)
        {
            if (images.ContainsKey(spriteName))
                activeSprite = spriteName;
            else
                Console.WriteLine("Trying to set sprite to nonexistant image: " + spriteName);
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
