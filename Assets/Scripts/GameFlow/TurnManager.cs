//#define SELECT_INCIDENTS

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public int turn = 0;
    IncidentManager m_IncidentManager;
    public GameOver GameOver;

    public Text turnsText;
    public Text gameOverText;
    public Text GameScoreText;
    public Text HighScoreText;

    public GameObject startScreen;
    public GameObject settingsScreen;
    public GameObject SettingsScreenQuitToMenuOption;
    public GameObject NextTurnButton;
    public Text EndTurnSatisfaction;

    public Tutorial Tutorial;

	void Start () {        
        NextTurnButton.SetActive(false);
		EndTurnSatisfaction.gameObject.SetActive(false);
		GameOver.gameObject.SetActive(false);
        settingsScreen.SetActive(false);
        startScreen.SetActive(true);
        m_IncidentManager = this.GetComponent<IncidentManager>();
    }
	public void StartGame()
    {
        NextTurn();
    }
	private void NextTurn()
    {
        AudioManager.Instance.PlayNextTurn();

        turn++;
        
        NextTurnButton.SetActive(false);
		EndTurnSatisfaction.gameObject.SetActive(false);
		GameObject.Find("OfficerManager").GetComponent<OfficerController>().EndTurn();
        turnsText.text = turn.ToString();
        if (m_IncidentManager == null)
            m_IncidentManager = this.GetComponent<IncidentManager>();
        
        m_IncidentManager.UpdateIncidents();
        //m_IncidentManager.CheckExpiredIncidents(turn);

        if (m_IncidentManager.IsGameOver())
        {
            //GAME OVER
            GameOver.gameObject.SetActive(true);
            GameOver.ShowGameOver(turn-1, m_IncidentManager.GetTotalCasesClosed());
        }
        else
        {
            //update at the end to give the player a chance to get citizen happiness over 20%
            m_IncidentManager.EndTurn();
            //decide which incident to show this turn
            m_IncidentManager.IsIncidentWaitingToShow(turn);    //not using the bool callback to populate the next incident list
            m_IncidentManager.CreateNewIncident(turn);
#if SELECT_INCIDENTS
            GameObject.Find("IncidentDialog").GetComponent<DialogBox>().DeactivateAll();
#else
            m_IncidentManager.ShowIncident(turn);
#endif
        }
        
    }
    
    public void Reset()
    {
        //reset the case identifier to ensure that the case numbers are reset
        SimplifiedJson.Identifier = 1;
        AudioManager.Instance.PlayBackgroundMusic();
        AudioManager.Instance.SetBackgroundMusicBalance(100f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        AudioManager.Instance.NegativeButtonPress();
    }
    public void GoToInspect()
    {
        SendEmail();
        //Application.OpenURL("http://inspec2t-project.eu/en/");
    }
    void SendEmail()
    {
        string email = "felix@playgen.com";
        string subject = MyEscapeURL("Resource Force Feedback");
        string body = MyEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
    public void PauseGame()
    {
        //we could set Time.scale to 0 but there is little need so we will just show the pause screen
        AudioManager.Instance.PositiveButtonPress();
        SettingsScreenQuitToMenuOption.SetActive(!startScreen.activeSelf);
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
}
