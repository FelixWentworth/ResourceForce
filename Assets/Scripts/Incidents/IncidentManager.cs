//#define SELECT_INCIDENTS
#define ALLOW_DUPLICATE_INCIDENTS

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
    public IncidentQueue m_IncidentQueue;
    protected int currentTurn;
    protected float happiness = 75f;

    public DialogBox m_dialogBox;
    public SatisfactionDisplays m_satisfactionDisplay;

    private Dictionary<int, IncidentHistory> _incidentHistories = new Dictionary<int, IncidentHistory>();

#if SELECT_INCIDENTS
    private int incidentShowingIndex = 1;
#endif
    void Start()
    {
        m_satisfactionDisplay.SetSatisfactionDisplays(happiness);
    }
    public void CreateNewIncident(int zTurn)
    {
        if (incidents.Count == 10)
        {
            //we do not have the space for any more incidents
            return;
        }
        //now get a random incident data from JSON file
        if (jsonReader == null)
            jsonReader = this.GetComponent<SimplifiedJson>();
		// create new incidents, randome amount between 1 and 3
	    var num = UnityEngine.Random.Range(1, 4);
	    for (int i = 0; i < num; i++)
	    {
			Incident newIncident = new Incident();
			if (incidents.Count == 10)
			    return;
#if ALLOW_DUPLICATE_INCIDENTS
            jsonReader.CreateNewIncident(ref newIncident);
#else
            jsonReader.CreateNewIncident(ref newIncident, incidents);
#endif
            newIncident.turnToShow = zTurn;
		    newIncident.turnToDevelop = zTurn + newIncident.turnsToAdd + 1;
		    //our complete list of incidents
		    incidents.Add(newIncident);
		    //our list of incidents waiting to show this turn
		    NextIncident.Add(newIncident);
		    m_IncidentQueue.AddToQueue(newIncident);
	    }
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
        string status = Localization.Get("BASIC_TEXT_ACTIVE_CASES") + "\n";
        for (int i = 0; i < incidents.Count; i++)
        {
            //an incident has been resolved so remove it from our list
            {
                status += Localization.Get("INCIDENT_CASE") + incidents[i].caseNumber + ": " + incidents[i].nameBeforeDeveloped + "\n";
                m_IncidentQueue.UpdateSeverity(incidents[i].caseNumber, incidents[i].severity);
            }
        }
        CaseStatus.text = status;
    }
    public void CheckExpiredIncidents(int turn)
    {
        for (int i=0; i<incidents.Count; i++)
        {
            if (incidents[i].turnToDevelop < turn)
            {
                incidents[i].satisfactionImpact = -1 * incidents[i].severity;
            }
        }
    }
    public void _showIncident(Text myText)
    {
        //used in button driven incidents
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
        m_dialogBox.currentIncident = currentIncident;
#if SELECT_INCIDENTS
        incidentShowingIndex = 0;
#endif
        if (zCaseNumber != -1)
        {
            //we have a case to show
            bool incidentFound = false;
            for (int i = 0; i < NextIncident.Count; i++)
            {
                //find the case to show by caseNumber
                if (NextIncident[i].caseNumber == zCaseNumber)
                {
                    currentIncident = NextIncident[i];
                    m_dialogBox.currentIncident = currentIncident;
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
        currentIncident.Show(ref currentIncident);

        m_IncidentQueue.ToggleBackground(currentIncident.caseNumber);

        //make sure the current incident is not showing as new as we now know the player has seen it
        currentIncident.isNew = false;
    }
    public void CaseClosed(int impact, bool expired = false)
    {
        //update the citizen security/happiness
        if (expired)
        {
            if (impact >= 0)
                impact = (impact + 1) * -1;
            happiness += impact;
        }
        else
        {
            happiness += impact;
        }
        happiness = Mathf.Clamp(happiness, 0, 100);
        m_satisfactionDisplay.SetSatisfactionDisplays(happiness);
    }
    public void EndTurn()
    {
        //punish the player for having cases open, stopping players from just ignoring all cases
        happiness -= 1f * GetTotalSeverity();
        happiness = Mathf.Clamp(happiness, 0, 100);
        m_satisfactionDisplay.SetSatisfactionDisplays(happiness);
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
        m_dialogBox.currentIncident = currentIncident;
#else
        Incident currentIncident = NextIncident[0];
        m_dialogBox.currentIncident = currentIncident;
#endif
        m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().WaitPressed(ref currentIncident);
       // GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref currentIncident, true);
        m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.Waiting);
        //update our lists
        NextIncident[0] = currentIncident;
        for (int i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].caseNumber == currentIncident.caseNumber)
            {
                incidents[i] = currentIncident;
                break;
            }
        }

        AddIncidentHistory(m_dialogBox.currentIncident, IncidentHistoryElement.Decision.Ignore);

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
            m_dialogBox.currentIncident = currentIncident;
#else
            Incident currentIncident = NextIncident[0];
            m_dialogBox.currentIncident = currentIncident;
#endif
            m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
            m_OfficerController.RemoveOfficer(currentIncident.officer, currentIncident.turnsToAdd);
            GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().OfficerPressed(ref currentIncident);

            m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.OfficersSent);
            NextIncident[0] = currentIncident;
            for (int i = 0; i < incidents.Count; i++)
            {
                if (incidents[i].caseNumber == currentIncident.caseNumber)
                {
                    incidents[i] = currentIncident;
                    break;
                }
            }

            AddIncidentHistory(m_dialogBox.currentIncident, IncidentHistoryElement.Decision.Officer);

            ShowNext();
        }
    }

    public void CitizenHelpPressed()
    {
        
#if SELECT_INCIDENTS
        Incident currentIncident = NextIncident[incidentShowingIndex];
        m_dialogBox.currentIncident = currentIncident;
#else
        Incident currentIncident = NextIncident[0];
        m_dialogBox.currentIncident = currentIncident;
#endif
        m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
        m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.CitizenRequest);
        //make sure the incident is updated next turn, we will handle the citizen request result when we next show the incident
        currentIncident.turnToShow++;
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().CitizenPressed(ref currentIncident);
        NextIncident[0] = currentIncident;
        for (int i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].caseNumber == currentIncident.caseNumber)
            {
                incidents[i] = currentIncident;
                break;
            }
        }

        AddIncidentHistory(m_dialogBox.currentIncident, IncidentHistoryElement.Decision.Citizen);

        ShowNext();
    }
    public void ShowNext()
    {
        //hide the highlight on all objects
        m_IncidentQueue.RemoveAllHighlights();
#if SELECT_INCIDENTS
        NextIncident.RemoveAt(incidentShowingIndex);
        incidentShowingIndex = 0;
#else
        NextIncident.RemoveAt(0);
#endif

        if (NextIncident.Count == 0)//no more incidents to show
        {
            TurnManager tmp = this.gameObject.GetComponent<TurnManager>();
            tmp.NextTurnButton.SetActive(true);
			tmp.EndTurnSatisfaction.gameObject.SetActive(true);
            int ignored = GetIgnoredCasesCount();
            if (ignored > 0)
            {
                var satisfactionText = Localization.Get("BASIC_TEXT_SATISFACTION_END_TURN");
                tmp.EndTurnSatisfaction.text = string.Format(satisfactionText, ignored, ignored > 1 ? "s" : "", GetTotalSeverity());
            }
            else
            {
                tmp.EndTurnSatisfaction.text = Localization.Get("BASIC_TEXT_NO_IGNORED_CASES");
            }
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

                CaseClosed(incidents[i].satisfactionImpact);
                incidents.RemoveAt(i);
                i--;
            }
        }
        ShowNext();
    }
    public bool isGameOver()
    {
        return GetHappiness() < 10f;
    }
    public int GetHappiness()
    {
        return Mathf.RoundToInt(happiness);
    }
    public int GetTotalSeverity()
    {
        int total = 0;
        foreach(IncidentCase i in m_IncidentQueue.allCases)
        {
            if (i.severityNumber != 0 && i.gameObject.activeSelf && i.m_state == IncidentCase.State.Waiting)
            {
                total += i.severityNumber;
            }
        }
        return total;
    }
    public int GetIgnoredCasesCount()
    {
        int total = 0;
        foreach (IncidentCase i in m_IncidentQueue.allCases)
        {
            if (i.m_state == IncidentCase.State.Waiting)
            {
                total += 1;
            }
        }
        return total;
    }

    private void AddIncidentHistory(Incident incident, IncidentHistoryElement.Decision decision)
    {
        var type = incident.type;
        var feedbackRating = 1;
        var feedback = "";

        var b = new Incident();
        var historyElement = new IncidentHistoryElement
        {
            Type = type,
            Description = Localization.Get(incident.incidentName) + "\n\n" + "<color=yellow>" + m_dialogBox.GetTip() + "</color>",
            Feedback = feedback,
            FeedbackRating = feedbackRating,
            PlayerDecision = decision
        };

        UpdateHistory(incident, historyElement);
    }
    private void UpdateHistory(Incident incident, IncidentHistoryElement element)
    {
        var incidentHistory = new IncidentHistory();

        if (_incidentHistories.ContainsKey(incident.caseNumber))
        {
            _incidentHistories.TryGetValue(incident.caseNumber, out incidentHistory);
        }

        incidentHistory.IncidentHistoryElements.Add(element);
        _incidentHistories[incident.caseNumber] = incidentHistory;
    }

    public List<IncidentHistoryElement> GetIncidentHistory(int caseNumber)
    {
        IncidentHistory incidentHistory;

        return _incidentHistories.TryGetValue(caseNumber, out incidentHistory) ? 
            incidentHistory.IncidentHistoryElements 
            : 
            new List<IncidentHistoryElement>();
    }
}

public class Incident {
    public int scenarioNum;
    public int index;
    public int officer;
    public string incidentName;
    public string type;
    public int turnToShow;
    public int turnsToAdd;
    public int severity;
    public int caseNumber;
    public int waitIndex;
    public int officerIndex;
    public int citizenIndex;
    public int satisfactionImpact;

    //values which are not set during setup
    public int turnToDevelop;
    public string nameBeforeDeveloped;
    public bool developed;
    public bool resolved;
    public bool positiveResolution = false;
    public bool isNew = true;

    private TurnManager m_turnManager;
    private DialogBox m_dialogBox;

    public void Show(ref Incident zIncident)
    {
        //use the dialog box to show the current incident
        if (m_dialogBox == null)
            m_dialogBox = GameObject.Find("IncidentDialog").GetComponent<DialogBox>();
       m_dialogBox.Show(zIncident);
    }
}
