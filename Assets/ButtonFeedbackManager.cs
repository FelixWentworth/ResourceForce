using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ButtonFeedbackManager : MonoBehaviour
{
    public static ButtonFeedbackManager Instance;

    public List<float> OfficerAnimSpeeds;
    public List<float> IgnoreAnimSpeeds;
    public List<float> CitizenAnimSpeeds;


    public GameObject IgnoreFeedback;
    public GameObject CitizenFeedback;
    public GameObject OfficerFeedback;

    public enum FeedbackType
    {
        Ignore, 
        Citizen,
        Officer
    }

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetObjects();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowFeedback(FeedbackType.Officer, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowFeedback(FeedbackType.Officer, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowFeedback(FeedbackType.Officer, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ShowFeedback(FeedbackType.Ignore, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ShowFeedback(FeedbackType.Ignore, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ShowFeedback(FeedbackType.Ignore, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ShowFeedback(FeedbackType.Citizen, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ShowFeedback(FeedbackType.Citizen, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ShowFeedback(FeedbackType.Citizen, 3);
        }
    }

    public void ResetObjects()
    {
        IgnoreFeedback.SetActive(false);
        CitizenFeedback.SetActive(false);
        OfficerFeedback.SetActive(false);

        AudioManager.Instance.StopButtonMusic();
    }

    public void ShowFeedback(FeedbackType type, int severity)
    {
        float speed = GetSpeed(type, severity);

        switch (type)
        {
            case FeedbackType.Ignore:
                AudioManager.Instance.PressIgnoreButton(severity);
                StartAnimation(IgnoreFeedback, speed);
                break;
            case FeedbackType.Citizen:
                AudioManager.Instance.PressCitizenButton(severity);
                StartAnimation(CitizenFeedback, speed);
                break;
            case FeedbackType.Officer:
                AudioManager.Instance.PressOfficerButton(severity);
                StartAnimation(OfficerFeedback, speed);
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }

        StartCoroutine(WaitForAudio());
    }

    private float GetSpeed(FeedbackType type, int severity)
    {
        switch (type)
        {
            case FeedbackType.Ignore:
                return IgnoreAnimSpeeds[severity - 1];
            case FeedbackType.Citizen:
                return CitizenAnimSpeeds[severity - 1];
            case FeedbackType.Officer:
                return OfficerAnimSpeeds[severity - 1];
        }
        return 1f;
    }

    private void StartAnimation(GameObject go, float speed)
    {
        go.SetActive(true);
        var animator = go.GetComponent<Animator>();
        animator.speed = speed;
    }

    private IEnumerator WaitForAudio()
    {
        while (AudioManager.Instance.IsButtonFeeedbackPlaying)
        {
            yield return null;
        }

        ResetObjects();
    }
}
