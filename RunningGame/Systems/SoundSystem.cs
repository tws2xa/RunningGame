using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using System.Media;
using WMPLib;

namespace RunningGame.Systems {
    [Serializable()]
    public class SoundSystem
    {

        bool mediaPlayer = false;

        Dictionary<string, SoundPlayer> playingSounds = new Dictionary<string, SoundPlayer>();
        Dictionary<string, WindowsMediaPlayer> playingSoundsMP = new Dictionary<string, WindowsMediaPlayer>();

        //Here put any helper methods or really anything else you may want.
        //You may find it handy to have methods here that other systems can access.
        public void playSound( string soundLocation, bool loop ) {
            if ( GlobalVars.soundOn ) {
                if ( !mediaPlayer ) {
                    System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( soundLocation );
                    SoundPlayer player = new SoundPlayer( stream );
                    if ( playingSoundsMP.ContainsKey( soundLocation ) )
                        stopSound( soundLocation );
                    if ( loop ) {
                        player.PlayLooping();
                    } else {
                        player.Play();
                    }
                    playingSounds.Add( soundLocation, player );
                } else {
                    WindowsMediaPlayer player = new WindowsMediaPlayer();
                    System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                    System.Reflection.ManifestResourceInfo info = a.GetManifestResourceInfo( soundLocation );
                    player.URL = info.FileName;
                    
                    Console.WriteLine( "URL: " + player.URL );
                    if ( playingSoundsMP.ContainsKey( soundLocation ) )
                        stopSound( soundLocation );
                    if ( loop ) {
                        player.settings.setMode( "loop", true );
                        player.controls.play();
                    } else {
                        player.controls.play();
                    }
                    playingSoundsMP.Add( soundLocation, player );
                }
            }
        }

        public void stopSound( string soundLocation ) {
            if ( mediaPlayer && playingSoundsMP.ContainsKey( soundLocation ) ) {
                playingSoundsMP[soundLocation].controls.stop();
                playingSoundsMP.Remove( soundLocation );
            } else if ( !mediaPlayer && playingSounds.ContainsKey( soundLocation ) ) {
                playingSounds[soundLocation].Stop();
                playingSounds.Remove( soundLocation );
            }
        }

        public bool isPlaying( string titleMusic ) {
            if ( mediaPlayer )
                return playingSoundsMP.ContainsKey( titleMusic );
            else
                return playingSounds.ContainsKey( titleMusic );
        }
    }
}
