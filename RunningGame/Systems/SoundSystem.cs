using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using System.Media;

namespace RunningGame.Systems
{
    /*
     * A Template for the system class
     */
    class SoundSystem : GameSystem //Always extend GameSystem
    {
        //All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        Level level;

        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public SoundSystem(Level level)
        {
            requiredComponents.Add(GlobalVars.SOUND_COMPONENT_NAME);
            
            this.level = level; //Always have this

        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
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
            //Your brilliant coding Skillz go here. Or below in other methods.
        }
        //----------------------------------------------------------------------------------------------

        //Here put any helper methods or really anything else you may want.
        //You may find it handy to have methods here that other systems can access.
        public void playSound(SoundPlayer player)
        {
            player.Play();
            //player.PlaySync();
            //player.PlayLooping();
        }
        public void stopSound(SoundPlayer player)
        {
            player.Stop();
        }

    }
}
