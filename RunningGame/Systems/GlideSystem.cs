using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;

namespace RunningGame.Systems
{
    class GlideSystem: GameSystem
    {
        //All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

           public GlideSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.GRAVITY_COMPONENT_NAME); //Health component

            this.level = level; //Always have this
           }

           public override ArrayList getRequiredComponents()
           {
               return requiredComponents;
           }

           //Must have this. Same for all Systems.
           public override Level GetActiveLevel()
           {
               return level;
           }

           //You must have an Update.
           //Always read in deltaTime, and only deltaTime (it's the time that's passed since the last frame)
           //Use deltaTime for things like changing velocity or changing position from velocity
           //This is where you do anything that you want to happen every frame.
           //There is a chance that your system won't need to do anything in update. Still have it.
           public override void Update(float deltaTime)
           {
          

           }
    }
}
