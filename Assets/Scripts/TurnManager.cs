//#define SELECT_INCIDENTS

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public int turn = 0;
    IncidentManager m_IncidentManager;

    public Text turnsText;
    public Text gameOverText;
    public Text HighScoreText;
    public int GameOverLimit = 4;

    public GameObject startScreen;

    public GameObject GameOver;
    public GameObject NextTurnButton;
	// Use this for initialization
	void Start () {
        NextTurnButton.SetActive(false);
        GameOver.SetActive(false);
        startScreen.SetActive(true);
        m_IncidentManager = this.GetComponent<IncidentManager>();
    }
	public void StartGame()
    {
        NextTurn();
    }
	public void NextTurn()
    {
        turn++;
        
        NextTurnButton.SetActive(false);
        GameObject.Find("OfficerManager").GetComponent<OfficerController>().EndTurn();
        turnsText.text = Localization.Get("BASIC_TEXT_TURN") + "\n" + turn;
        if (m_IncidentManager == null)
            m_IncidentManager = this.GetComponent<IncidentManager>();
        m_IncidentManager.UpdateIncidents();
        
        if (m_IncidentManager.isGameOver())
        {
            //GAME OVER, too many incidents un resolved
            StartCoroutine(ShowGameOver());
        }
        else
        {
            //decide which incident to show this turn
            m_IncidentManager.IsIncidentWaitingToShow(turn);
            m_IncidentManager.CreateNewIncident(turn);
            m_IncidentManager.UpdateCitizens();
#if SELECT_INCIDENTS
            GameObject.Find("IncidentDialog").GetComponent<DialogBox>().DeactivateAll();
#else
            m_IncidentManager.ShowIncident(turn);
#endif
        }
        //update at the end to give the player a chance to get citizen happiness over 20%
        m_IncidentManager.EndTurn();
    }
    IEnumerator ShowGameOver()
    {
        gameOverText.text = string.Format(Localization.Get("BASIC_TEXT_GAMEOVER_BODY"), turn);
        int bestTurns = PlayerPrefs.GetInt("BestTurns");
        if (turn > bestTurns)
        {
            bestTurns = turn;
            PlayerPrefs.SetInt("BestTurns", bestTurns);
        }
        HighScoreText.text = Localization.Get("BASIC_TEXT_SCORE") + ": " + turn + "\n" + Localization.Get("BASIC_TEXT_BEST") + ": " + bestTurns;
        HighScoreText.text = HighScoreText.text.ToUpper();
        GameOver.SetActive(true);
        
        yield return new WaitForSeconds(5f);
        Reset();
    }

    void Reset()
    {
        SimplifiedJson.identifier = 1;
        Application.LoadLevel(0);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            GameOverLimit++;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (GameOverLimit > 1)
                GameOverLimit--;
        }

    }
}
