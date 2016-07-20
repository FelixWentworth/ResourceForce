﻿using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    //setup singleton
    private bool muted = false;
    public GameObject SoundOffIcon;
    public GameObject SoundOnIcon;

    private static AudioManager instance = null;

    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        UpdateSoundIcons();
        PlayBackgroundMusic();
        SetBackgroundMusicBalance(75f);
        if (instance != null && instance != this)
        {
            //if the singleton has already been created
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
       
    }

    public AudioSource backgroundMusic;
    public AudioSource endangerMusic;
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
        sendOfficerMusic.Play();
        positiveButtonMusic.Play();
    }
    public void NegativeButtonPress()
    {
        //play negative button press audio
        //negativeButtonMusic.Play();
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
        endangerMusic.Play();
    }
    public void StopBackgroundMusic()
    {
        //stop the background music
        backgroundMusic.Stop();
        endangerMusic.Stop();
    }
    public void SetBackgroundMusicBalance(float satisfaction)
    {
        //set the volumes of the background music based on the satisfaction level
        //only play the endagered music when the satisfaction is lower than 70%
        if (satisfaction < 70f)
        {
            float volume = Mathf.Clamp01(satisfaction / 70f);

            backgroundMusic.volume = volume/2f;
            endangerMusic.volume = 1 - volume;
            
        }
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
        //play new game jingle
        positiveButtonMusic.Play();
    }
    public void PlayNextTurn()
    {
        //play the next turn music
        positiveButtonMusic.Play();
    }
    public void CaseClosed()
    {
        //play case closed audio
        //caseClosedMusic.Play();
    }

    public void ToggleMute()
    {
        muted = !muted;

        UpdateSoundIcons();        

        AudioListener.volume = muted ? 0 : 1;
    }
    private void UpdateSoundIcons()
    {
        SoundOffIcon.SetActive(muted);
        SoundOnIcon.SetActive(!muted);
    }
}