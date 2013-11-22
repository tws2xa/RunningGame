using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RunningGame.Components;
using System.Collections;
using RunningGame.Level_Editor;
using RunningGame.Entities;

namespace RunningGame.Systems
{

    /*
     * The draw system checks for entities with a position and draw component.
     * Basically it goes through all entities with these components and draws them.
     */

    [Serializable()]
    public class DrawSystem : GameSystem
    {

        Graphics g;
        Level level;
        CreationLevel creatLev = null;
        public View mainView;
        List<string> requiredComponents = new List<string>();

        /************FLASH STUFF Begins here*/
        float flashTime = 0;
        int alpha = 0; // have to figure out the right number/ratio
        float deltaAlpha = 0;
        Color flashColor = Color.White;
        SolidBrush flashBrush = new SolidBrush(Color.FromArgb(0, Color.White)); //white is completely arbitrary
        Boolean flashDirection = true; // if truue, then the flash direction is going forwar, (fades into color), if false
        //it means that it's fading out
        //alpha is a proportion 
        //new solid brush color and alpha 
        //set color, then set that timer.
        //draw function, if that timer is greater than 0, make alpha proportion, decrease time in update.

        /*FLASH STUFF ends here*/
        [NonSerialized] Pen selectedEntBorderColor = Pens.Red;
        [NonSerialized] Brush selectedEntFillColor = new SolidBrush(Color.FromArgb(100, Color.CornflowerBlue));

        View miniMap;

        bool miniMapOn = false;

        public DrawSystem(Graphics g, Level level)
        {
            //Required Components
            requiredComponents.Add(GlobalVars.DRAW_COMPONENT_NAME); //Draw Component
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position Component

            this.g = g;
            this.level = level;

            if (level is CreationLevel)
            {
                creatLev = (CreationLevel)level;
            }

            mainView = new View(0, 0, level.cameraWidth, level.cameraHeight, 0, 0, level.cameraWidth, level.cameraHeight, level, level.getPlayer());
            
            miniMap = new View(0, 0, level.levelWidth, level.levelHeight, level.cameraWidth-210, 10, 200, 100, level);
            miniMap.bkgBrush = Brushes.DarkTurquoise;
            miniMap.hasBorder = true;

        }
        public DrawSystem(Graphics g, CreationLevel level)
        {
            //Required Components
            requiredComponents.Add(GlobalVars.DRAW_COMPONENT_NAME); //Draw Component
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position Component

            this.g = g;
            this.level = level;

            if (level is CreationLevel)
            {
                creatLev = (CreationLevel)level;
            }

            mainView = new View(0, 0, level.levelWidth, level.levelHeight, 0, 0, level.levelWidth, level.levelHeight, level);


            miniMap = new View(0, 0, level.levelWidth, level.levelHeight, level.cameraWidth - 100, 10, 200, 100, level);
            miniMap.bkgBrush = Brushes.DarkTurquoise;
            miniMap.hasBorder = true;
        }
        public override List<string> getRequiredComponents()
        {
            return requiredComponents;
        }
        public override Level GetActiveLevel()
        {
            return level;
        }

        public override void Update(float deltaTime)
        {

            if (mainView.followEntity == null)
            {
                mainView.setFollowEntity(level.getPlayer());
            }

            
            //*this part takes care of flashes on the screen
            if (flashTime > 0)
            {
                flashTime = flashTime - deltaTime;
                if (flashDirection)
                    alpha += (int)(deltaAlpha * deltaTime);
                else
                {
                    //flashDirection is false
                    alpha -= (int)(deltaAlpha * deltaTime);
                }
                if (alpha >= 255)
                {
                    alpha = 255;
                    flashDirection = false;
                }

                if (alpha <= 0)
                {
                    flashTime = 0;
                    alpha = 0;
                }

                flashBrush.Color = Color.FromArgb(alpha, flashColor);

            }
            else
            {
                alpha = 0;
            }
            /*
            if (flashTime < 0)
                flashTime = 0;
            */
            //color decrease by delta time
            // total time passed and total time for the alpha
            // might want to do make flash in the draw system
            //draw a rectangle
            //total time 
            //Update views
            mainView.Update();
        }


        public float getFlashTime()
        {
            return flashTime;
        }
        public Brush getFlashBrush()
        {
            return flashBrush;
        }
        public void setFlash(Color c, float time)
        {
           
            ((SolidBrush)flashBrush).Color = Color.FromArgb(0,  c);
            flashTime = time;
            deltaAlpha = ((255*2)/(time)); // the 20 is arbitary for now since I can't figure out how to set the ratio, since I don't know 
            //how to acces delta time from here
            flashDirection = true;
            flashColor = c;
        }
        //Explanantion of g
        //g is a brush for an image
        //use g to draw on that image
        // that image is to draw on the window, passed down. 
        // g goes from levelWindow(form spring), to the game, to the level, to the draw system, to the view
        // what you use to draw things to the image
        // g is a property(essentially the image)
        // every image has a graphics object associated with it, latched on it. 
        public void Draw(Graphics g)
        {
            List<Entity> entityList = getApplicableEntities();
            
            //this is where all the entities are drawn, so modify this for depth
            mainView.Draw(g, entityList);


            /*
            //If you are in the level editor. Box the selected entities
            //Ignore this unless you are playing with level editor
            if (creatLev != null && creatLev.vars.selectedEntity != null)
            {
                foreach (Entity e in creatLev.vars.allSelectedEntities)
                {
                    PositionComponent posComp = (PositionComponent)creatLev.vars.selectedEntity.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                    g.FillRectangle(selectedEntFillColor, posComp.x - posComp.width / 2, posComp.y - posComp.height / 2, posComp.width, posComp.height);
                    g.DrawRectangle(selectedEntBorderColor, posComp.x - posComp.width / 2, posComp.y - posComp.height / 2, posComp.width, posComp.height);
                }
            }
             * */

            if(miniMapOn)
                miniMap.Draw(g, entityList);

        }

    }
}
