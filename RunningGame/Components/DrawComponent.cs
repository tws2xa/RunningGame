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

    [Serializable()]
    public class DrawComponent : Component
    {

        //public string defaultImageName = "defaultImageName";

        //May or may not need graphics depending on how the Drawing System works
        //public Bitmap sprite {get; set;}
        //public Dictionary<string, Sprite> images = new Dictionary<string, Sprite>();
        public Dictionary<string, Sprite> images = new Dictionary<string, Sprite>();

        public float width {get; set;}
        public float height { get; set; }
        public bool sizeLocked;
        public string activeSprite;

        public Level level;

        //Try not to use this constructor
        public DrawComponent(Bitmap img, string spriteName, float width, float height, Level level, bool sizeLocked)
        {
            this.componentName = GlobalVars.DRAW_COMPONENT_NAME;
            this.width = width;
            this.height = height;
            this.sizeLocked = sizeLocked;
            this.level = level;

            activeSprite = spriteName;

            
            Sprite newSprite = new Sprite(spriteName, img);
            images.Add(spriteName, newSprite);
            
        }

        //Default image is the address of the image shown if the other images can't be found
        public DrawComponent(float width, float height, Level level, bool sizeLocked)
        {


            this.componentName = GlobalVars.DRAW_COMPONENT_NAME;
            this.width = width;
            this.height = height;
            this.sizeLocked = sizeLocked;
            this.level = level;

            
            //Bitmap image = readInImage(defaultImage);

            /*
            Sprite newSprite = new Sprite(defaultImageName, image);
            images.Add(defaultImageName, newSprite);
             * */
            
        }

        public void addSprite(string baseName, string defaultFileName, string spriteName)
        {
            Bitmap image = readInImage(getImageFilePathName(baseName));

            //If image not found, check for ones of the same world, perhaps different level
            if (image == null)
                image = readInImage(getImageFilePathOther1(baseName));
            if (image == null)
                image = readInImage(getImageFilePathOther2(baseName));

            //Still null? Goto default
            if (image == null)
            {
                image = readInImage(defaultFileName);
            }

            if (image == null) Console.WriteLine("Error: Null image for " + spriteName + " baseName: " + baseName + " defaultFile: " + defaultFileName);

            Sprite spr = new Sprite(spriteName, image);

            images.Add(spriteName, spr);
        }

        public void addAnimatedSprite(List<string> baseAddresses, List<string> defaultAddresses, string spriteName)
        {
            List<Bitmap> newImages = new List<Bitmap>();
            int i = 0;
            foreach (string str in baseAddresses)
            {
                Bitmap img = readInImage(getImageFilePathName(str));

                if (img == null) img = readInImage(defaultAddresses[i]);

                newImages.Add(img);
                i++;
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

                if (myStream == null) return null;

                Bitmap sprite = new Bitmap(myStream); //Getting an error here? Did you remember to make your image an embedded resource?
                myStream.Close();

                if (sizeLocked)
                {
                    sprite = new Bitmap(sprite, new Size((int)Math.Ceiling(width), (int)Math.Ceiling(height)));
                }

                Bitmap newSprite = sprite.Clone(new RectangleF(0, 0, sprite.Width-1, sprite.Height-1), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                GlobalVars.imagesInStore.Add(makeImageKey(imageAddress, width, height), newSprite);
                return newSprite;
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
            if (activeSprite == null)
            {
                return new Sprite("placeholder", new Bitmap((int)width, (int)height));
            }
            return (Sprite)images[activeSprite];
        }

        public void rotateFlipSprite(string spriteName, RotateFlipType rotation)
        {
            if (images.ContainsKey(spriteName))
            {

                List<Bitmap> newImages = new List<Bitmap>();

                Sprite s = (Sprite)images[spriteName];

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

        public void resizeImages(int width, int height)
        {

            if (!sizeLocked)
            {
                Console.WriteLine("Probably no need to call resizeImages on something not size locked. The image gets resized every frame.");
                return;
            }

            foreach (Sprite sprite in images.Values)
            {
                for (int i=0; i<sprite.images.Count; i++)
                {
                    Bitmap b = new Bitmap((Image)sprite.images[i], width, height);
                    sprite.images[i] = b;
                }
            }
        }

        public string getImageFilePathName(string baseName)
        {
            string retStr = "RunningGame.Resources.";
            retStr += baseName;
            retStr += level.worldNum;
            retStr += level.levelNum;
            retStr += ".png";
            return retStr;
        }
        public string getImageFilePathOther1(string baseName)
        {
            int addNum = level.levelNum;
            if (addNum == 1) addNum = 2;
            else if (addNum == 2) addNum = 3;
            else if(addNum == 3) addNum = 1;

            string retStr = "RunningGame.Resources.";
            retStr += baseName;
            retStr += level.worldNum;
            retStr += addNum;
            retStr += ".png";
            return retStr;
        }
        public string getImageFilePathOther2(string baseName)
        {
            int addNum = level.levelNum;
            if (addNum == 1) addNum = 3;
            else if (addNum == 2) addNum = 1;
            else if (addNum == 3) addNum = 2;

            string retStr = "RunningGame.Resources.";
            retStr += baseName;
            retStr += level.worldNum;
            retStr += addNum;
            retStr += ".png";
            return retStr;
        }
    }
}
