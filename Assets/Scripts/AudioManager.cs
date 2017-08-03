using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    public bool IsButtonFeeedbackPlaying
    {
        get { return sendOfficerMusic.isPlaying || citizenMusic.isPlaying || ignoreMusic.isPlaying; }
    }

    public void StopButtonMusic()
    {
        sendOfficerMusic.Stop();
        citizenMusic.Stop();
        ignoreMusic.Stop();
    }

    //get slider so that we can update the position to match the volume
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            //if the singleton has already been created
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

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
    public AudioSource ignoreMusic;
    public AudioSource citizenMusic;
    public AudioSource gameOverMusic;
    public AudioSource caseClosedMusic;

    public List<AudioClip> SendOfficerClips;
    public List<AudioClip> IgnoreClips;
    public List<AudioClip> CitizenClips;


    public void PressOfficerButton(int severity)
    {
        //play officer button press audio
        sendOfficerMusic.clip = SendOfficerClips[severity - 1];

        sendOfficerMusic.Play();
    }

    public void PressCitizenButton(int severity)
    {
        //play citizen button press audio
        citizenMusic.clip = CitizenClips[severity - 1];

        citizenMusic.Play();
    }

    public void PressIgnoreButton(int severity)
    {
        //play ignore button press audio
        ignoreMusic.clip = IgnoreClips[severity - 1];
        
        ignoreMusic.Play();
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
