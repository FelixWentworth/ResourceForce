using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IncidentManager : MonoBehaviour {

    public List<Incident> incidents = new List<Incident>();
    private SimplifiedJson jsonReader;

    Incident NextIncident;
    OfficerController m_OfficerController;

    public Text CaseStatus;

	public void CreateNewIncident(int zTurn)
    {
        Incident newIncident = new Incident();
        
        //now get a random incident data from JSON file
        if (jsonReader == null)
            jsonReader = this.GetComponent<SimplifiedJson>();

        jsonReader.CreateNewIncident(ref newIncident);

        incidents.Add(newIncident);
        NextIncident = newIncident;
    }
    public bool IsIncidentWaitingToShow(int zTurn)
    {
        for (int i=0; i < incidents.Count; i++)
        {
            if (incidents[i].turnToShow <= zTurn)
            {
                //this incident wants to be shown on this turn
                NextIncident = incidents[i];
                return true;
            }
        }
        //no elements in our incident list need to be shown
        return false;
    }
    public void UpdateIncidents()
    {
        string status = "Active Cases\n";
        for (int i = 0; i < incidents.Count; i++)
        {
            //an incident has been resolved so remove it from our list
            if(incidents[i].resolved)
            {
                incidents.RemoveAt(i);
                i--;
            }
            else
            {
                status += "Case " + incidents[i].caseNumber + ": " + incidents[i].incidentName + "\n";
            }
        }
        CaseStatus.text = status;
    }
    public void ShowIncident(int turn)
    {
        if (NextIncident == null)
            CreateNewIncident(turn);
        NextIncident.Show(ref NextIncident);

        //make the incident null to make sure we dont show it again until its due
        //NextIncident = null;
    }
    public void ClearList()
    {
        incidents.Clear();
    }
    public void WaitPressed()
    {
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref NextIncident, true);
        NextIncident = null;
        this.gameObject.GetComponent<TurnManager>().NextTurn();
    }
    public void ResolvePressed()
    {
        if (m_OfficerController == null)
            m_OfficerController = GameObject.Find("OfficerManager").GetComponent<OfficerController>();
        if (m_OfficerController.m_officers.Count > NextIncident.officer)
        {
            GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref NextIncident, false);
            m_OfficerController.RemoveOfficer(NextIncident.officer);
            NextIncident = null;
            this.gameObject.GetComponent<TurnManager>().NextTurn();
        }
    }
}

public class Incident {
    public int index;
    public int officer;
    public string area;
    public string incidentName;
    public int turnToShow;
    public bool resolved;
    public int turnsToAdd;

    public int caseNumber;
    public bool developed;

    private TurnManager m_turnManager;

    public void Show(ref Incident zIncident)
    {
        //use the dialog box to show the current incident
        GameObject.Find("IncidentDialog").GetComponent<DialogBox>().ShowBox(incidentName, area, officer, caseNumber, developed);
    }
}
