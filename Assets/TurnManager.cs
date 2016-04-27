using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public int turn = 0;
    IncidentManager m_IncidentManager;

    public Text turnsText;
    public Text resolvedCases;
    [HideInInspector]public string caseResolved = "";
	// Use this for initialization
	void Start () {
        m_IncidentManager = this.GetComponent<IncidentManager>();

        NextTurn();
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
    

    void Reset()
    {
        turn = 0;
        m_IncidentManager.ClearList();
        NextTurn();
    }
}
