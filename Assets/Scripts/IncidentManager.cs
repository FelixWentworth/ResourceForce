//#define SELECT_INCIDENTS

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

#if SELECT_INCIDENTS
    private int incidentShowingIndex = 1;
#endif
    public void CreateNewIncident(int zTurn)
    {

        //NextIncident.Clear();
        Incident newIncident = new Incident();

        //now get a random incident data from JSON file
        if (jsonReader == null)
            jsonReader = this.GetComponent<SimplifiedJson>();

        jsonReader.CreateNewIncident(ref newIncident);
        newIncident.turnToShow = zTurn;
        newIncident.turnToDevelop = zTurn + 3;
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
    public void _showIncident(Text myText)
    {
        int _caseNumer = int.Parse(myText.text);
        int turn = GameObject.Find("TurnManager").GetComponent<TurnManager>().turn;
        ShowIncident(turn, _caseNumer);
    }
    public void ShowIncident(int turn, int zCaseNumber = -1)
    {
        currentTurn = turn;
        if (NextIncident == null)
            CreateNewIncident(turn);
        
        Incident currentIncident = NextIncident[0];
#if SELECT_INCIDENTS
        incidentShowingIndex = 0;
#endif
        if (zCaseNumber != -1)
        {
            //player has chosen a message to look at on the side
            bool incidentFound = false;
            for (int i = 0; i<NextIncident.Count; i++)
            {
                if (NextIncident[i].caseNumber == zCaseNumber)
                {
                    currentIncident = NextIncident[i];
#if SELECT_INCIDENTS
                    incidentShowingIndex = i;
#endif
                    incidentFound = true;
                    break;
                }
            }
            if (!incidentFound) //clicked on a case that has since been removed
                return;
        }
        if (currentIncident.citizenHelp)
        {
            currentIncident.citizenHelp = false;  //make sure this is not repeated every turn
            currentIncident.ShowCitizenHelp(ref currentIncident);
            
        }
        else if (currentIncident.resolved)
        {
            //show the case closed screen
            currentIncident.ShowCaseClosed(ref currentIncident);
        }
        else
        {
            currentIncident.Show(ref currentIncident);
            currentIncident = currentIncident;
        }
        m_IncidentQueue.ToggleBackground(currentIncident.caseNumber);
        CaseNumber.text = "Subject: Case " + currentIncident.caseNumber.ToString();
        CaseNumber.text += currentIncident.isNew ? "" : " (ongoing)";
        if (currentIncident.isNew)
            currentIncident.isNew = false;
    }
    public void IncreaseArrestsMade()
    {
        arrestsNum++;
        ArrestsMade.text = "Arrests\n" + arrestsNum + "/" + (SimplifiedJson.identifier - 1);
    }
    public void ClearList()
    {
        incidents.Clear();
        NextIncident.Clear();
    }
    public void WaitPressed()
    {
#if SELECT_INCIDENTS
        Incident currentIncident = NextIncident[incidentShowingIndex];
#else
        Incident currentIncident = NextIncident[0];
#endif
        m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
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
#if SELECT_INCIDENTS
            Incident currentIncident = NextIncident[incidentShowingIndex];
#else
        Incident currentIncident = NextIncident[0];
#endif
            m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
            m_OfficerController.RemoveOfficer(currentIncident.officer);
            GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref currentIncident, false);
            m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.OfficersSent);

            ShowNext();
        }
    }   
    public void CitizenHelpPressed()
    {
#if SELECT_INCIDENTS
        Incident currentIncident = NextIncident[incidentShowingIndex];
#else
        Incident currentIncident = NextIncident[0];
#endif
        m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
        m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.CitizenRequest);
        //make sure the incident is updated next turn, we will handle the citizen request result when we next show the incident
        currentIncident.turnToShow++;
        currentIncident.citizenHelp = true;
        ShowNext();
    }
    public void ShowNext()
    {
#if SELECT_INCIDENTS
        NextIncident.RemoveAt(incidentShowingIndex);
        incidentShowingIndex = 0;
#else
        NextIncident.RemoveAt(0);
#endif

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
                if (incidents[i].positiveResolution)
                    IncreaseArrestsMade();

                incidents.RemoveAt(i);
                i--;
            }
        }
        ShowNext();
    }

    public int GetArrestPercentage()
    {
        return Mathf.RoundToInt((arrestsNum * 100) / (SimplifiedJson.identifier - 1));
    }
    public void UpdateCitizens()
    {
        for (int i = 0; i<incidents.Count; i++)
        {
            incidents[i].NewTurn();
        }
    }
}

public class Incident {
    public int index;
    public int officer;
    public string area;
    public string incidentName;
    public int turnToShow;
    public int turnsToAdd;
    public int severity;
    public int caseNumber;

    //values which are not set during setup
    public int turnToDevelop;
    public string nameBeforeDeveloped;
    public bool developed;
    public bool citizenHelp;
    public bool resolved;
    public bool positiveResolution = false;
    public bool isNew = true;
    private bool CitizenAvailable;

    private TurnManager m_turnManager;
    private DialogBox m_dialogBox;

    public void NewTurn()
    {
        //decide if we should show the citizen help box
        int rand = UnityEngine.Random.Range(0, 10);
        CitizenAvailable = rand == 1;
    }

    public void Show(ref Incident zIncident)
    {
        //use the dialog box to show the current incident
        

        if (m_dialogBox == null)
            m_dialogBox = GameObject.Find("IncidentDialog").GetComponent<DialogBox>();
        m_dialogBox.ShowBox(incidentName, area, officer, caseNumber, developed, CitizenAvailable);
    }
    public void ShowCaseClosed(ref Incident zIncident)
    {
        if (m_dialogBox == null)
            m_dialogBox = GameObject.Find("IncidentDialog").GetComponent<DialogBox>();
        m_dialogBox.ShowCaseClosedBox(caseNumber, zIncident.positiveResolution);
    }
    public void ShowCitizenHelp(ref Incident zIncident)
    {
        if (m_dialogBox == null)
            m_dialogBox = GameObject.Find("IncidentDialog").GetComponent<DialogBox>();
        m_dialogBox.ShowCitizenHelp(caseNumber);
    }
    public void ClearDialogBox()
    {
        if (m_dialogBox == null)
            m_dialogBox = GameObject.Find("IncidentDialog").GetComponent<DialogBox>();
        m_dialogBox.ClearDialogBox();
    }
    public void NoMoreIncidents()
    {
        if (m_dialogBox == null)
            m_dialogBox = GameObject.Find("IncidentDialog").GetComponent<DialogBox>();
        m_dialogBox.NoMoreIncidents();
    }
}
