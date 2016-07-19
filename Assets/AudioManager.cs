﻿using UnityEngine;
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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            //if the singleton has already been created
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public AudioSource backgroundMusic;
    public AudioSource positiveButtonMusic;
    public AudioSource negativeButtonMusic;
    public AudioSource warningBoxMusic;
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
        //gameOverMusic.Play();
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
}
