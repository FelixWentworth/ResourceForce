using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    /// <summary>
    /// this class will show a brief tutorial of how to play the game, it will highlight sections and describe what they do
    /// </summary>
    public GameObject inputBlocker;
    public GameObject Screenshot;
    public GameObject[] tutorialObjects;
    public enum TutorialPoint { TopBar = 0, StatusBar = 1, Officers = 2, Information = 3, Incidents = 4, Buttons = 6}
    public TutorialPoint currentPoint = TutorialPoint.TopBar;

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
    public void ShowStep()
    {
        DeactivateAll();
        inputBlocker.SetActive(true);
        Screenshot.SetActive(true);
        int step = (int)currentPoint;
        tutorialObjects[step].SetActive(true);
    }
    public void NextStep()
    {
        int step = (int)currentPoint;
        step++;
        if (step <= tutorialObjects.Length - 1)
        {
            currentPoint = (TutorialPoint)step;
            ShowStep();
        }
        else
        {
            DeactivateAll();
            inputBlocker.SetActive(false);
            Screenshot.SetActive(false);
            GameObject.Find("TurnManager").GetComponent<TurnManager>().StartGame();
            PlayerPrefs.SetInt("NewPlayer", 1);
        }
    }
}
