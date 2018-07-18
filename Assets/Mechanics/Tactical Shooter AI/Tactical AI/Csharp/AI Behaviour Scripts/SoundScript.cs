using UnityEngine;
using System.Collections;

/*
 * Plays sounds when other scripts tell it to.
 * Sounds can be given increased or decreased odds to play.
 * */

namespace TacticalAI{
[RequireComponent (typeof (AudioSource))]
public class SoundScript : MonoBehaviour {
	
	public bool audioEnabled = true;
	AudioSource audioSource;
	
	//Damaged
	[Range (0.0f, 1.0f)]
	public float oddsToPlayDamagedAudio = 0.5f;
	public SoundClip[] damagedAudio;

    //Death
    [Range(0.0f, 1.0f)]
    public float oddsToPlayDeathAudio = 0.5f;
    public SoundClip[] deathAudio;

    //Spotted
    [Range (0.0f, 1.0f)]
	public float oddsToPlaySpottedTargetAudio = 0.5f;
	public SoundClip[] spottedTargetAudio;
	
	//Suppressed
	[Range (0.0f, 1.0f)]
	public float oddsToPlaySuppressedAudio = 0.5f;
	public SoundClip[] suppressedAudio;
	
	//Cover
	[Range (0.0f, 1.0f)]
	public float oddsToPlayCoverAudio = 0.5f;
	public SoundClip[] coverAudio;

    //Reloading
    [Range(0.0f, 1.0f)]
    public float oddsToPlayReloadAudio = 0.5f;
    public SoundClip[] reloadAudio;
		
    //Take input from other scripts
    public void PlayDamagedAudio()
		{
			PlayAClip(damagedAudio, oddsToPlayDamagedAudio);
		}
	
	public void PlaySpottedAudio()
		{
			PlayAClip(spottedTargetAudio, oddsToPlaySpottedTargetAudio);
		}
		
	public void PlaySuppressedAudio()
		{
			PlayAClip(suppressedAudio, oddsToPlaySuppressedAudio);
		}
	
	public void PlayCoverAudio()
		{
			PlayAClip(coverAudio, oddsToPlayCoverAudio);
		}

    public void PlayReloadAudio()
        {
            PlayAClip(reloadAudio, oddsToPlayReloadAudio);
        }

    public void PlayDeathAudio()
        {
            PlayAClip(deathAudio, oddsToPlayDeathAudio);
        }
    
    public void OnAIDeath()
        {
            PlayDeathAudio();
        }

        public void PlayAClip(SoundClip[] audios,float odds)
		{
            //Make sure we can play an audio clip
			if(audioEnabled && GetComponent<AudioSource>() && !GetComponent<AudioSource>().isPlaying && Random.value < odds && audios != null)
				{
                    //Select a clip to play.  Clips with a higher "oddsToPlay" value have more greater chances to be chosesn.
                    //Not sure why I named the variable "damagedTotalOdds."
					int damagedTotalOdds = 0;
					int i;
					for(i = 0; i < audios.Length; i++)
						{
							damagedTotalOdds +=	audios[i].oddsToPlay;
						}
						
					damagedTotalOdds = Random.Range(0, damagedTotalOdds);
					
                    //Cycle through the audio clips.  Easier to show with an example than a description.
                    /*Clips
                     * A - odds 3
                     * B - odds 2
                     * C - odds 4
                     * 
                     * damagedTotalOdds is randomly chosen above to be 4
                     * 
                     * i = 0, damagedTotalOdds = 4-3=1.  As we haven't reached 0, we move on to B.
                     * i = 1, damagedTotalOdds = 1-2=-1.  As the result is less than zero, clip B is the one played.
                     */

                    for (i = 0; i < audios.Length; i++)
						{
							damagedTotalOdds -= audios[i].oddsToPlay;
							if(damagedTotalOdds <= 0)
								{
                                    //if(GetComponent<AudioSource>().enabled)
                                    AudioSource.PlayClipAtPoint(audios[i].audioClip, transform.position);
                                    return;
								}
						}
				}
		}

}
}

//Small class to hold audio and odds data
namespace TacticalAI{
    [System.Serializable]
	public class SoundClip
	{
		public AudioClip audioClip;
		public int oddsToPlay = 1;
	}
}
	