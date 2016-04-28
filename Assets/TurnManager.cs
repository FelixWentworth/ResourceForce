using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public int turn = 0;
    IncidentManager m_IncidentManager;

    public Text turnsText;
    public Text resolvedCases;
    public Text gameOverText;
    public Text maxCases;
    public int GameOverLimit = 4;
    [HideInInspector]public string caseResolved = "";

    public GameObject GameOver;
	// Use this for initialization
	void Start () {
        GameOver.SetActive(false);
        m_IncidentManager = this.GetComponent<IncidentManager>();

        NextTurn();
        maxCases.text = "Max Cases: " + GameOverLimit;
    }
	
	public void NextTurn()
    {
        turn++;
        resolvedCases.text = caseResolved;
        caseResolved = "";
        GameObject.Find("OfficerManager").GetComponent<OfficerController>().EndTurn();
        turnsText.text = "Turn: " + turn;
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
            if (m_IncidentManager.IsIncidentWaitingToShow(turn))
            {
                m_IncidentManager.ShowIncident(turn);
            }
            else
            {
                m_IncidentManager.CreateNewIncident(turn);
                m_IncidentManager.ShowIncident(turn);
            }
        }
    }
    IEnumerator ShowGameOver()
    {
        gameOverText.text = string.Format("You Survived {0} Turns", turn);
        GameOver.SetActive(true);
        
        yield return new WaitForSeconds(1.5f);
        Reset();
    }

    void Reset()
    {
        Application.LoadLevel(0);
        //turn = 0;
        //m_IncidentManager.ClearList();
        //NextTurn();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            GameOverLimit++;
            maxCases.text = "Max Cases: " + GameOverLimit;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (GameOverLimit > 1)
                GameOverLimit--;
            maxCases.text = "Max Cases: " + GameOverLimit;
        }

    }
}
