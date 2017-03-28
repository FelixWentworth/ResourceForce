using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public int turn = 0;
    IncidentManager _incidentManager;
    public GameOver GameOver;

    public Text turnsText;
    public Text gameOverText;
    public Text GameScoreText;
    public Text HighScoreText;

    public GameObject startScreen;
    public GameObject settingsScreen;
    public GameObject SettingsScreenQuitToMenuOption;
    public GameObject SettingsScreenScenarioReportOption;
    public GameObject NextTurnButton;
    public GameObject EndTurnSatisfaction;

    public Feedback FeedbackObject;

    public Tutorial Tutorial;

	void Start () {        
        NextTurnButton.SetActive(false);
		EndTurnSatisfaction.SetActive(false);
		GameOver.gameObject.SetActive(false);
        settingsScreen.SetActive(false);
        FeedbackObject.gameObject.SetActive(false);
        startScreen.SetActive(true);
        _incidentManager = this.GetComponent<IncidentManager>();
    }
	public void StartGame()
    {
        ScenarioTracker.ClearHistory();

        NextTurn();
    }
	private void NextTurn()
    {
        AudioManager.Instance.PlayNextTurn();

        turn++;
        NextTurnButton.SetActive(false);
		EndTurnSatisfaction.SetActive(false);
		GameObject.Find("OfficerManager").GetComponent<OfficerController>().EndTurn();
        turnsText.text = turn.ToString();
        if (_incidentManager == null)
            _incidentManager = this.GetComponent<IncidentManager>();
        
        _incidentManager.UpdateIncidents();
        //m_IncidentManager.CheckExpiredIncidents(turn);

        if (_incidentManager.IsGameOver())
        {
            //GAME OVER
            GameOver.gameObject.SetActive(true);
            GameOver.ShowGameOver(turn-1, _incidentManager.GetTotalCasesClosed(), _incidentManager.GetTotalCasesClosedWell());
        }
        else
        {
            //update at the end to give the player a chance to get citizen happiness over 20%
            _incidentManager.EndTurn();
            //decide which incident to show this turn
            _incidentManager.IsIncidentWaitingToShow(turn);    //not using the bool callback to populate the next incident list
                                                               //_incidentManager.CreateNewIncident(turn);

            _incidentManager.AddNewIncidents(turn);

            
#if SELECT_INCIDENTS
            GameObject.Find("IncidentDialog").GetComponent<DialogBox>().DeactivateAll();
#else
            _incidentManager.ShowIncident(turn);
#endif
        }
    }

    public void Reset()
    {
        //reset the case identifier to ensure that the case numbers are reset
        SimplifiedJson.Identifier = 1;
        AudioManager.Instance.PlayBackgroundMusic();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        AudioManager.Instance.NegativeButtonPress();
    }
    public void GoToInspect()
    {
        SendFeedback();
        //SendEmail();
        //Application.OpenURL("http://inspec2t-project.eu/en/");
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
    public void PauseGame()
    {
        //we could set Time.scale to 0 but there is little need so we will just show the pause screen
        AudioManager.Instance.PositiveButtonPress();
        SettingsScreenQuitToMenuOption.SetActive(turn > 0);
        SettingsScreenScenarioReportOption.SetActive(turn > 0);
        settingsScreen.SetActive(!settingsScreen.activeSelf);
    }
    public void ResumeGame()
    {
        AudioManager.Instance.PositiveButtonPress();
        settingsScreen.SetActive(false);
    }

    public void ReplayTutorial()
    {
        AudioManager.Instance.PositiveButtonPress();
        Tutorial.gameObject.SetActive(true);
        Tutorial.StartTutorial();
        settingsScreen.SetActive(false);
    }

    public void ChangeLocation()
    {
        //tell the start screen to show the location when it loads
        StartTransition.overrideShowLocation = true;
        //reset the game
        Reset();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

#region send emails
    public void Report()
    {
        FeedbackObject.Setup(SendReportEmail);
        FeedbackObject.gameObject.SetActive(true);
    }

    public void SendReportEmail(string body)
    {
        StartCoroutine(SendReport(body));
    }
    public IEnumerator SendReport(string body)
    {
        var subject = "SCENARIO ISSUE";
        var bodyWithScenarioHistory = "";
        if (_incidentManager.NextIncident.Count > 0)
        {
            var currentIncident = _incidentManager.NextIncident[0];
        
            bodyWithScenarioHistory = ScenarioTracker.GetScenarioHistory(body, currentIncident.scenarioNum.ToString(),
                currentIncident.index.ToString(), Location.CurrentLocation,
                (DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English"));
        }
        else
        {
            bodyWithScenarioHistory = ScenarioTracker.GetScenarioHistory(body, "", "", "", "");
        }

        var www = new WWW(ElasticEmail.GetAddress(), ElasticEmail.GetForm(subject, bodyWithScenarioHistory));
        yield return www;

        var message = www.error == null
            ? Localization.Get("BASIC_TEXT_FEEDBACK_SENT")
            : Localization.Get("BASIC_TEXT_ERROR");

        yield return Loading.Stop(message, 1f);

        if (www.error == null)
        {
            FeedbackObject.gameObject.SetActive(false);
        }
    }

    public void SendFeedback()
    {
        FeedbackObject.Setup(SendFeedbackEmail);
        FeedbackObject.gameObject.SetActive(true);
    }

    public void SendFeedbackEmail(string body)
    {
        StartCoroutine(SendFeedback(body));
    }
    public IEnumerator SendFeedback( string body)
    {
        var subject = "FEEDBACK";

        var www = new WWW(ElasticEmail.GetAddress(), ElasticEmail.GetForm(subject, body));

        yield return www;

        var message = www.error == null
            ? Localization.Get("BASIC_TEXT_FEEDBACK_SENT")
            : Localization.Get("BASIC_TEXT_ERROR");

        yield return Loading.Stop(message, 1f);

        if (www.error == null)
        {
            FeedbackObject.gameObject.SetActive(false);
        }
    }
#endregion
}
