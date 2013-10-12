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

    public class DrawComponent : Component
    {

        //May or may not need graphics depending on how the Drawing System works
        //public Bitmap sprite {get; set;}
        //public Dictionary<string, Sprite> images = new Dictionary<string, Sprite>();
        public StringObjPairList images = new StringObjPairList();

        public float width {get; set;}
        public float height { get; set; }
        public bool sizeLocked { get; set; }
        public string activeSprite;

        //Try not to use this constructor
        public DrawComponent(Bitmap img, String spriteName, float width, float height, bool sizeLocked)
        {
            this.componentName = GlobalVars.DRAW_COMPONENT_NAME;
            this.width = width;
            this.height = height;
            this.sizeLocked = sizeLocked;

            activeSprite = spriteName;

            Sprite newSprite = new Sprite(spriteName, img);
            images.Add(spriteName, newSprite);
        }

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
            return getSprite().getCurrentImage();
        }

        public Sprite getSprite()
        {
            return (Sprite)images.getValFromKey(activeSprite);
        }

        public void rotateFlipSprite(string spriteName, RotateFlipType rotation)
        {
            if (images.ContainsKey(spriteName))
            {

                ArrayList newImages = new ArrayList();

                Sprite s = (Sprite)images.getValFromKey(spriteName);

                foreach (Image b in s.images)
                {
                    //Must create a copy so it doesn't flip ALL things using this image
                    try
                    {
                        Bitmap newImage = new Bitmap(b);
                        newImage.RotateFlip(rotation);
                        newImages.Add(newImage);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine("Couldn't flip an image! Exception: " + e);
                    }
                    
                }

                s.images = newImages;
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
                getSprite().currentImageIndex = 0;
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
                    getSprite().currentImageIndex = 0;
                }
            }
            else
                Console.WriteLine("Trying to set sprite to nonexistant image: " + spriteName);
        }

    }
}
