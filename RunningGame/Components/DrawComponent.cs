using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace RunningGame.Components
{
    //The entity has a sprite and will be drawn

    class DrawComponent:Component
    {

        //May or may not need graphics depending on how the Drawing System works
        //public Bitmap sprite {get; set;}
        public Dictionary<string, Sprite> images = new Dictionary<string, Sprite>();
        public float width {get; set;}
        public float height { get; set; }
        public bool sizeLocked { get; set; }
        public string activeSprite;

        public DrawComponent(String spriteAddress, String spriteName, float width, float height, bool sizeLocked)
        {


            this.componentName = GlobalVars.DRAW_COMPONENT_NAME;
            this.width = width;
            this.height = height;
            this.sizeLocked = sizeLocked;

            Bitmap image = readInImage(spriteAddress);

            activeSprite = spriteName;

            Sprite newSprite = new Sprite(spriteName, image);
            images.Add(spriteName, newSprite);
            
        }

        public void addSprite(string spriteAddress, string spriteName)
        {
            Bitmap image = readInImage(spriteAddress);

            Sprite spr = new Sprite(spriteName, image);

            images.Add(spriteName, spr);
        }

        public void addAnimatedSprite(ArrayList addresses, string spriteName)
        {
            ArrayList newImages = new ArrayList();

            foreach (string str in addresses)
            {
                Bitmap img = readInImage(str);
                newImages.Add(img);
            }

            Sprite spr = new Sprite(spriteName, newImages);

            images.Add(spriteName, spr);

        }

        public Bitmap readInImage(string imageAddress)
        {
            if (GlobalVars.imagesInStore.ContainsKey(makeImageKey(imageAddress, width, height)))
            {
                return GlobalVars.imagesInStore[makeImageKey(imageAddress, width, height)];
            }
            else
            {
                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream myStream = myAssembly.GetManifestResourceStream(imageAddress);
                Bitmap sprite = new Bitmap(myStream);
                myStream.Close();

                if (sizeLocked)
                {
                    sprite = new Bitmap(sprite, new Size((int)Math.Ceiling(width), (int)Math.Ceiling(height)));
                }

                GlobalVars.imagesInStore.Add(makeImageKey(imageAddress, width, height), sprite);
                return sprite;
            }
        }

        public string makeImageKey(string address, float width, float height)
        {
            return (address + "" + width + "" + height);
        }

        public Image getImage()
        {
            return images[activeSprite].getCurrentImage();
        }

        public Sprite getSprite()
        {
            return images[activeSprite];
        }

        public void rotateFlipSprite(string spriteName, RotateFlipType rotation)
        {
            if (images.ContainsKey(spriteName))
            {

                ArrayList newImages = new ArrayList();

                foreach (Image b in images[spriteName].images)
                {
                    //Must create a copy so it doesn't flip ALL things using this image
                    Bitmap newImage = new Bitmap(b);
                    newImage.RotateFlip(rotation);
                    newImages.Add(newImage);
                }

                images[spriteName].images = newImages;
            }
            else
                Console.WriteLine("Trying to rotate/flip a nonexistant image: " + spriteName);
        }
        
        //Auto resets the animation
        public void setSprite(string spriteName)
        {
            if (images.ContainsKey(spriteName))
            {
                activeSprite = spriteName;
                images[activeSprite].currentImageIndex = 0;
            }
            else
                Console.WriteLine("Trying to set sprite to nonexistant image: " + spriteName);
        }
        //Can tell it not to reset animation if you'd like.
        public void setSprite(string spriteName, bool resetAnimation)
        {
            if (images.ContainsKey(spriteName))
            {
                activeSprite = spriteName;
                if (resetAnimation)
                {
                    images[activeSprite].currentImageIndex = 0;
                }
            }
            else
                Console.WriteLine("Trying to set sprite to nonexistant image: " + spriteName);
        }

    }
}
