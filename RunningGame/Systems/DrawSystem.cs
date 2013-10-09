using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RunningGame.Components;
using System.Collections;
using RunningGame.Level_Editor;

namespace RunningGame.Systems
{

    /*
     * The draw system checks for entities with a position and draw component.
     * Basically it goes through all entities with these components and draws them.
     */

    class DrawSystem:GameSystem
    {

        Graphics g;
        Level level;
        CreationLevel creatLev = null;
        public View mainView;
        ArrayList requiredComponents = new ArrayList();

        Pen selectedEntBorderColor = Pens.Red;
        Brush selectedEntFillColor = new SolidBrush(Color.FromArgb(100, Color.CornflowerBlue));

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

            mainView = new View(50, 0, level.cameraWidth, level.cameraHeight, 0, 0, level.cameraWidth, level.cameraHeight, level, level.getPlayer());
            
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
        public override ArrayList getRequiredComponents()
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

            //Update views
            mainView.Update();
        }

        public void Draw(Graphics g)
        {
            ArrayList entityList = getApplicableEntities();
            mainView.Draw(g, entityList);

            if (creatLev != null && creatLev.vars.selectedEntity != null)
            {
                PositionComponent posComp = (PositionComponent)creatLev.vars.selectedEntity.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
                g.DrawRectangle(selectedEntBorderColor, posComp.x-posComp.width/2, posComp.y-posComp.height/2, posComp.width, posComp.height);
                g.FillRectangle(selectedEntFillColor, posComp.x - posComp.width / 2, posComp.y - posComp.height / 2, posComp.width, posComp.height);
            }

            if(miniMapOn)
                miniMap.Draw(g, entityList);

        }

    }
}
