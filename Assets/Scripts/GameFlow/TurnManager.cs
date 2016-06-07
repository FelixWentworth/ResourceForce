﻿//#define SELECT_INCIDENTS

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public int turn = 0;
    IncidentManager m_IncidentManager;

    public Text turnsText;
    public Text gameOverText;
    public Text HighScoreText;

    public GameObject startScreen;
    public GameObject GameOver;
    public GameObject NextTurnButton;

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
	private void NextTurn()
    {
        turn++;
        
        NextTurnButton.SetActive(false);
        GameObject.Find("OfficerManager").GetComponent<OfficerController>().EndTurn();
        turnsText.text = Localization.Get("BASIC_TEXT_TURN") + " " + turn;
        if (m_IncidentManager == null)
            m_IncidentManager = this.GetComponent<IncidentManager>();
        m_IncidentManager.UpdateIncidents();
        
        if (m_IncidentManager.isGameOver())
        {
            //GAME OVER, too many incidents un resolved
            ShowGameOver();
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
    private void ShowGameOver()
    {
        int zTurn = turn - 1;
        gameOverText.text = string.Format(Localization.Get("BASIC_TEXT_GAMEOVER_BODY"), zTurn);
        int bestTurns = PlayerPrefs.GetInt("BestTurns");
        if (zTurn > bestTurns)
        {
            bestTurns = zTurn;
            PlayerPrefs.SetInt("BestTurns", bestTurns);
        }
        HighScoreText.text = Localization.Get("BASIC_TEXT_SCORE") + ": " + zTurn + "\n" + Localization.Get("BASIC_TEXT_BEST") + ": " + bestTurns;
        HighScoreText.text = HighScoreText.text.ToUpper();
        GameOver.SetActive(true);
    }
    
    public void Reset()
    {
        //reset the case identifier to ensure that the case numbers are reset
        SimplifiedJson.identifier = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}