using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    /// <summary>
    /// this class will show a brief tutorial of how to play the game, it will highlight sections and describe what they do
    /// </summary>
    public GameObject inputBlocker;
    public GameObject Screenshot;
    public GameObject[] tutorialObjects;
    public enum TutorialPoint { TopBar = 0, IncidentBar = 1, Information = 2, OfficerBar = 3, WaitButton = 4, CitizenButton = 5, OfficerButton = 6}
    public TutorialPoint currentPoint = (TutorialPoint)0;

    void Awake()
    {
        DeactivateAll();
        inputBlocker.SetActive(false);
        Screenshot.SetActive(false);
    }
    private void DeactivateAll()
    {
        for (int i = 0; i < tutorialObjects.Length; i++)
        {
            //ensure our objects start disabled
            tutorialObjects[i].SetActive(false);
        }
    }

    public void StartTutorial()
    {
        currentPoint = (TutorialPoint) 0;
        ShowStep();
    }
    public void ShowStep()
    {
        DeactivateAll();
        inputBlocker.SetActive(true);
        Screenshot.SetActive(true);
        var step = (int)currentPoint;
        tutorialObjects[step].SetActive(true);
    }
    public void NextStep()
    {
        var step = (int)currentPoint;
        step++;
        if (step <= tutorialObjects.Length - 1)
        {
            currentPoint = (TutorialPoint)step;
            ShowStep();
        }
        else
        {
            CloseTutorial();
        }
    }

    public void Skip()
    {
        CloseTutorial();
    }

    private void CloseTutorial()
    {
        DeactivateAll();
        inputBlocker.SetActive(false);
        Screenshot.SetActive(false);

        this.gameObject.SetActive(false);

        PlayerPrefs.SetInt("NewPlayer", 1);
    }
}
