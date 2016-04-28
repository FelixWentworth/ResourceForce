using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IncidentManager : MonoBehaviour {

    public List<Incident> incidents = new List<Incident>();
    private SimplifiedJson jsonReader;

    public List<Incident> NextIncident = new List<Incident>();
    OfficerController m_OfficerController;

    public Text CaseStatus;
    public Text CaseReview;
    protected int currentTurn;

	public void CreateNewIncident(int zTurn)
    {
        NextIncident.Clear();
        Incident newIncident = new Incident();
        
        //now get a random incident data from JSON file
        if (jsonReader == null)
            jsonReader = this.GetComponent<SimplifiedJson>();

        jsonReader.CreateNewIncident(ref newIncident);
        newIncident.turnToShow = zTurn;
        incidents.Add(newIncident);
        NextIncident.Add(newIncident);
    }
    public bool IsIncidentWaitingToShow(int zTurn)
    {
        NextIncident.Clear();
        for (int i=0; i < incidents.Count; i++)
        {
            if (incidents[i].turnToShow <= zTurn)
            {
                //this incident wants to be shown on this turn
                NextIncident.Add(incidents[i]);
            }
        }
        //no elements in our incident list need to be shown
        CaseReview.text = "Cases To Review This Turn: " + NextIncident.Count;
        return (NextIncident.Count > 0);
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
                status += "Case " + incidents[i].caseNumber + ": " + incidents[i].nameBeforeDeveloped + "\n";
            }
        }
        CaseStatus.text = status;
    }
    public void ShowIncident(int turn)
    {
        currentTurn = turn;
        if (NextIncident == null)
            CreateNewIncident(turn);
        Incident currentIncident = NextIncident[0];
        NextIncident[0].Show(ref currentIncident);
        NextIncident[0] = currentIncident;
        CaseReview.text = "Cases Left To Review This Turn: " + (NextIncident.Count - 1);
        //make the incident null to make sure we dont show it again until its due
        //NextIncident = null;
    }
    public void ClearList()
    {
        incidents.Clear();
        NextIncident.Clear();
    }
    public void WaitPressed()
    {
        Incident currentIncident = NextIncident[0];
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref currentIncident, true);
        NextIncident.RemoveAt(0);
        if (NextIncident.Count == 0)//no more incidents to show
            this.gameObject.GetComponent<TurnManager>().NextTurn();
        else
            ShowIncident(currentTurn);
        CaseReview.text = "Cases Left To Review This Turn: " + (NextIncident.Count - 1);
    }
    public void ResolvePressed()
    {
        if (m_OfficerController == null)
            m_OfficerController = GameObject.Find("OfficerManager").GetComponent<OfficerController>();
        if (m_OfficerController.m_officers.Count >= NextIncident[0].officer)
        {
            Incident currentIncident = NextIncident[0];
            m_OfficerController.RemoveOfficer(currentIncident.officer);
            GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref currentIncident, false);

            NextIncident.RemoveAt(0);
            if (NextIncident.Count == 0)//no more incidents to show
                this.gameObject.GetComponent<TurnManager>().NextTurn();
            else
                ShowIncident(currentTurn);
            CaseReview.text = "Cases Left To Review This Turn: " + (NextIncident.Count-1);
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
    public string nameBeforeDeveloped;
    public bool developed;

    private TurnManager m_turnManager;

    public void Show(ref Incident zIncident)
    {
        //use the dialog box to show the current incident
        //decide if we should show the citizen help box
        int rand = UnityEngine.Random.Range(0, 10);


        GameObject.Find("IncidentDialog").GetComponent<DialogBox>().ShowBox(incidentName, area, officer, caseNumber, developed, rand==1);
    }
}
