using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class IncidentManager : MonoBehaviour
{

    public List<Incident> incidents = new List<Incident>();
    private SimplifiedJson jsonReader;

    public List<Incident> NextIncident = new List<Incident>();
    OfficerController m_OfficerController;

    public Text CaseStatus;
    public Text CaseNumber;
    //public Text CaseReview;
    public IncidentQueue m_IncidentQueue;
    protected int currentTurn;
    public Text ArrestsMade;
    protected int arrestsNum;

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
        m_IncidentQueue.AddToQueue(newIncident);
        ArrestsMade.text = "Arrests\n" + arrestsNum + "/" + (SimplifiedJson.identifier-1);
    }
    public bool IsIncidentWaitingToShow(int zTurn)
    {
        NextIncident.Clear();
        for (int i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].turnToShow <= zTurn)
            {
                //this incident wants to be shown on this turn
                m_IncidentQueue.ShowWarningIcon(incidents[i].caseNumber);
                NextIncident.Add(incidents[i]);

            }
        }
        return (NextIncident.Count > 0);
    }
    public void UpdateIncidents()
    {
        string status = "Active Cases\n";
        for (int i = 0; i < incidents.Count; i++)
        {
            //an incident has been resolved so remove it from our list
            {
                status += "Case " + incidents[i].caseNumber + ": " + incidents[i].nameBeforeDeveloped + "\n";
                m_IncidentQueue.UpdateSeverity(incidents[i].caseNumber, incidents[i].severity);
            }
        }
        CaseStatus.text = status;
        ArrestsMade.text = "Arrests\n" + arrestsNum + "/" + (SimplifiedJson.identifier - 1);
    }
    public void ShowIncident(int turn)
    {
        currentTurn = turn;
        if (NextIncident == null)
            CreateNewIncident(turn);
        Incident currentIncident = NextIncident[0];
        if (currentIncident.resolved)
        {
            //show the case closed screen
            NextIncident[0].ShowCaseClosed(ref currentIncident);
            arrestsNum++;
            ArrestsMade.text = "Arrests\n" + arrestsNum + "/" + (SimplifiedJson.identifier - 1);
        }
        else
        {
            NextIncident[0].Show(ref currentIncident);
            NextIncident[0] = currentIncident;
        }
        m_IncidentQueue.ToggleBackground(currentIncident.caseNumber);
        CaseNumber.text = "Case Number: " + currentIncident.caseNumber.ToString();
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
        m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.Waiting);
        ShowNext();
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
            m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.OfficersSent);

            ShowNext();
        }
    }   
    public void ShowNext()
    {
        NextIncident.RemoveAt(0);
        if (NextIncident.Count == 0)//no more incidents to show
        {
            this.gameObject.GetComponent<TurnManager>().NextTurnButton.SetActive(true);
            CaseNumber.text = "";
            if (incidents.Count != 0)
                incidents[0].NoMoreIncidents();
        }
        else
            ShowIncident(currentTurn);
    }
    public void CloseCase(int caseNumber)
    {
        for (int i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].caseNumber == caseNumber)
            {
                //we have found the case to remove
                m_IncidentQueue.RemoveFromQueue(incidents[i].caseNumber);
                incidents.RemoveAt(i);

                i--;
            }
        }
        ShowNext();
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
    public int severity;
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
    public void ShowCaseClosed(ref Incident zIncident)
    {
        GameObject.Find("IncidentDialog").GetComponent<DialogBox>().ShowCaseClosedBox(caseNumber);
    }
    public void ClearDialogBox()
    {
        GameObject.Find("IncidentDialog").GetComponent<DialogBox>().ClearDialogBox();
    }
    public void NoMoreIncidents()
    {
        GameObject.Find("IncidentDialog").GetComponent<DialogBox>().NoMoreIncidents();
    }
}
