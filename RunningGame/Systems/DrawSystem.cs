using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RunningGame.Components;
using System.Collections;

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
        View mainView;
        ArrayList requiredComponents = new ArrayList();


        View miniMap;

        bool miniMapOn = false;

        public DrawSystem(Graphics g, Level level)
        {

            //Required Components
            requiredComponents.Add(GlobalVars.DRAW_COMPONENT_NAME); //Draw Component
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position Component

            this.g = g;
            this.level = level;
            mainView = new View(50, 0, level.cameraWidth, level.cameraHeight, 0, 0, level.cameraWidth, level.cameraHeight, level, level.getPlayer());
            
            miniMap = new View(0, 0, level.levelWidth, level.levelHeight, level.cameraWidth-210, 10, 200, 100, level);
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
            
            if(miniMapOn)
                miniMap.Draw(g, entityList);

        }

    }
}
