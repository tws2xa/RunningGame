using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using System.Media;

namespace RunningGame.Systems {
    [Serializable()]
    public class SoundSystem : GameSystem //Always extend GameSystem
    {
        List<string> requiredComponents = new List<string>();
        Level level;

        public SoundSystem(Level level) {
            this.level = level;
        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override List<string> getRequiredComponents() {
            return requiredComponents;
        }

        //Must have this. Same for all Systems.
        public override Level GetActiveLevel() {
            return level;
        }

        //You must have an Update.
        //Always read in deltaTime, and only deltaTime (it's the time that's passed since the last frame)
        //Use deltaTime for things like changing velocity or changing position from velocity
        //This is where you do anything that you want to happen every frame.
        //There is a chance that your system won't need to do anything in update. Still have it.
        public override void Update(float deltaTime) {

        }
        //----------------------------------------------------------------------------------------------

        //Here put any helper methods or really anything else you may want.
        //You may find it handy to have methods here that other systems can access.
        public void playSound(string soundLocation, bool loop) {
            System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(soundLocation);
            SoundPlayer player = new SoundPlayer(stream);
            if (loop)
                player.PlayLooping();
            else
                player.Play();
        }
        public void stopSound(SoundPlayer player) {
            player.Stop();
        }

    }
}
