using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    private enum TutorialStates
    {
        Start = 0,

        Incident1,
        Incident2,
        Incident3,
        EndTurnShown,
        ScenarioResolved,
        FreeChoice,
        Satisfaction,
        GameOver,

        End
    }

    private TutorialStates _tutorialState;

    [Serializable]
    private class OnBoarding
    {
        public TutorialStates State;
        public GameObject Screen;
    }

    [SerializeField] private List<OnBoarding> OnBoardingSteps;

    public Text TurnsText;
    public List<Scenario> TutorialScenarios;

    public int Turn;
    /// <summary>
    /// this class will show a brief tutorial of how to play the game, it will highlight sections and describe what they do
    /// </summary>

    private IncidentManager _incidentManager;

    void Awake()
    {
        SetLocalizedText();
       
        _incidentManager = GetComponent<IncidentManager>();

        _tutorialState = TutorialStates.Start;
        NextState();
    }

    void Start()
    {
        // Generate the incidents for the tutorial
        _incidentManager.AddTutorialContent(TutorialScenarios);

        _incidentManager.ShowIncident(0);
        TurnsText.text = Turn.ToString();
    }

    private void SetLocalizedText()
    {
        foreach (var scenario in TutorialScenarios)
        {
            SetContentText(scenario.Content.Scene);

        }
    }

    private void SetContentText(IncidentContent scenario)
    {
        scenario.Title = Localization.Get(scenario.Title);
        scenario.Description = Localization.Get(scenario.Description);
        foreach (var choice in scenario.Choices)
        {
            choice.Choice.Feedback = Localization.Get(choice.Choice.Feedback);
            SetContentText(choice.Scene);
        }
    }

    public void OptionPressed()
    {
        foreach (var step in OnBoardingSteps)
        {
            step.Screen.SetActive(false);
        }
        if (_tutorialState >= TutorialStates.FreeChoice)
        {
            NextState();
        }
    }

    public void CloseCasePressed()
    {
        foreach (var step in OnBoardingSteps)
        {
            step.Screen.SetActive(false);
        }
        // Wait some time to allow for animations
        StartCoroutine(GoToNextState());
    }

    private IEnumerator GoToNextState()
    {
        yield return new WaitForSeconds(1.25f);
        NextState();
    }

    public void FeedbackDismissed()
    {
        NextState();
    }

    public void Exit()
    {
        PlayerPrefs.SetInt("NewPlayer", 1);
        SceneManager.LoadScene(0);
    }


    private void NextState()
    {
        _tutorialState = _tutorialState + 1;
        if (_tutorialState == TutorialStates.End)
        {
            // Go back to the main game
            SceneManager.LoadScene(0);
        }
        else
        {
            ShowNextTip();
        }
    }

    private void ShowNextTip()
    {
        foreach (var step in OnBoardingSteps)
        {
            step.Screen.SetActive(step.State == _tutorialState);
        }
    }

    public void NextTurn()
    {
        //PlayerPrefs.DeleteAll();
        AudioManager.Instance.PlayNextTurn();

        Turn++;
        _incidentManager.NextTurnButton.SetActive(false);
        _incidentManager.EndTurnSatisfactionObject.SetActive(false);
        GameObject.Find("OfficerManager").GetComponent<OfficerController>().EndTurn();
        TurnsText.text = Turn.ToString();
        if (_incidentManager == null)
            _incidentManager = this.GetComponent<IncidentManager>();

        _incidentManager.UpdateIncidents();

        //update at the end to give the player a chance to get citizen happiness over 20%
        _incidentManager.EndTurn();
        //decide which incident to show this turn
        _incidentManager.IsIncidentWaitingToShow(Turn);    //not using the bool callback to populate the next incident list
        //_incidentManager.CreateNewIncident(turn);

        _incidentManager.ShowIncident(Turn);
        NextState();
    }
}
