using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public int turn = 0;
    IncidentManager m_IncidentManager;

    public Text turnsText;
    public Text gameOverText;
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
        StartCoroutine(WaitToDisableStartScreen());
    }
    IEnumerator WaitToDisableStartScreen()
    {
        yield return new WaitForSeconds(0.75f);
        startScreen.SetActive(false);
    }
	public void NextTurn()
    {
        turn++;
        NextTurnButton.SetActive(false);
        GameObject.Find("OfficerManager").GetComponent<OfficerController>().EndTurn();
        turnsText.text = "Turn\n" + turn;
        if (m_IncidentManager == null)
            m_IncidentManager = this.GetComponent<IncidentManager>();
        m_IncidentManager.UpdateIncidents();
        if (m_IncidentManager.incidents.Count > GameOverLimit)
        {
            //GAME OVER, too many incidents un resolved
            StartCoroutine(ShowGameOver());
        }
        else
        {
            //decide which incident to show this turn
            m_IncidentManager.IsIncidentWaitingToShow(turn);
            m_IncidentManager.CreateNewIncident(turn); 
            m_IncidentManager.ShowIncident(turn);
            
        }
    }
    IEnumerator ShowGameOver()
    {
        gameOverText.text = string.Format("You Survived {0} Turns\nAnd Made Arrests for {1}% of Cases", turn, m_IncidentManager.GetArrestPercentage());
        GameOver.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);
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
