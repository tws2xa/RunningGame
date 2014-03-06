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
    public class SoundSystem
    {
        Dictionary<string, SoundPlayer> playingSounds = new Dictionary<string, SoundPlayer>();

        //Here put any helper methods or really anything else you may want.
        //You may find it handy to have methods here that other systems can access.
        public void playSound( string soundLocation, bool loop ) {
            if ( GlobalVars.soundOn ) {
                System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( soundLocation );
                SoundPlayer player = new SoundPlayer( stream );
                if ( playingSounds.ContainsKey( soundLocation ) )
                    stopSound( soundLocation );
                if ( loop )
                    player.PlayLooping();
                else
                    player.Play();
                playingSounds.Add( soundLocation, player );
            }
        }

        public void stopSound( string soundLocation ) {
            if ( playingSounds.ContainsKey( soundLocation ) ) {
                playingSounds[soundLocation].Stop();
                playingSounds.Remove( soundLocation );
            }
        }

        public bool isPlaying( string titleMusic ) {
            return playingSounds.ContainsKey( titleMusic );
        }
    }
}
