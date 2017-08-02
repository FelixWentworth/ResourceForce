using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioManager : MonoBehaviour {

    //setup singleton

    private static AudioManager instance = null;

    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    //get slider so that we can update the position to match the volume
    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.GetInt("VolumeSet") == 1)
        {
            //the volume has previously been set, get the volume the player has chosen
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            //set the volume to full as default
            AudioListener.volume = 1.0f;
        }
        PlayBackgroundMusic();
    }


    public AudioSource backgroundMusic;
    public AudioSource positiveButtonMusic;
    public AudioSource negativeButtonMusic;
    public AudioSource warningBoxMusic;
    public AudioSource sendOfficerMusic;
    public AudioSource gameOverMusic;
    public AudioSource caseClosedMusic;

	public void PressCitizenButton()
    {
        //play citizen button press audio 
        positiveButtonMusic.Play();
    }
    public void PressWaitButton()
    {
        //play wait button press audio
        positiveButtonMusic.Play();
    }
    public void PressOfficerButton()
    {
        //play officer button press audio
        //sendOfficerMusic.Play();
        positiveButtonMusic.Play();
    }

    public void PressCaseCloseButton()
    {
        positiveButtonMusic.Play();
    }
    public void NegativeButtonPress()
    {
        //play negative button press audio
        //negativeButtonMusic.Play();
        positiveButtonMusic.Play();
    }
    public void PositiveButtonPress()
    {
        //play positive button press audio
        positiveButtonMusic.Play();
    }
    public void PlayBackgroundMusic()
    {
        //play the background music
        backgroundMusic.Play();
    }
    public void StopBackgroundMusic()
    {
        //stop the background music
        backgroundMusic.Stop();
    }
    public void ShowWarningMessage()
    {
        //play show warning message
        warningBoxMusic.Play();
    }
    public void PlayGameOver()
    {
        //play game over jingle
        StopBackgroundMusic();
        gameOverMusic.Play();
    }
    public void PlayNewGame()
    {
        // play new game jingle
        positiveButtonMusic.Play();
    }
    public void PlayNextTurn()
    {
        // play the next turn music
        positiveButtonMusic.Play();
    }
    public void CaseClosed()
    {
        // play case closed audio
        // caseClosedMusic.Play();
    }
    private void SaveVolume(float vol)
    {
        PlayerPrefs.SetFloat("Volume", vol);
        PlayerPrefs.SetInt("VolumeSet", 1);
    }

    public void ToggleAudio()
    {
        AudioListener.volume = AudioListener.volume == 1f ? 0f : 1f;
        SaveVolume(AudioListener.volume);
    }

    public bool IsAudioEnabled()
    {
        return AudioListener.volume == 1f;
    }
}
