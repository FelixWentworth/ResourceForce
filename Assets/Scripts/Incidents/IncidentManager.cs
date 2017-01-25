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
    private IncidentDifficultyManager _incidentDifficultyManager;

    public List<Incident> NextIncident = new List<Incident>();
    public OfficerController OfficerController;

    public Text CaseStatus;
    public IncidentQueue m_IncidentQueue;
    protected int currentTurn;
    protected float happiness = 75f;

    public DialogBox m_dialogBox;
    public SatisfactionDisplays m_satisfactionDisplay;

    public Color LowSeverity;
    public Color MidSeverity;
    public Color HighSeverity;
    public Color UnknownSeverity;

    public GameObject SatisfactionImpactGameObject;

    private Dictionary<int, IncidentHistory> _incidentHistories = new Dictionary<int, IncidentHistory>();

    private int _casesClosed;
    private int _casesClosedThisTurn;

    private int _endTurnSatisfaction;

    private const int MaxIncidents = 5;
   
#if SELECT_INCIDENTS
    private int incidentShowingIndex = 1;
#endif
    void Start()
    {
        m_satisfactionDisplay.SetSatisfactionDisplays(happiness);
        _casesClosed = 0;
        _casesClosedThisTurn = 0; 
    }
    public void CreateNewIncident(int zTurn)
    {
        //now get a random incident data from JSON file
        if (jsonReader == null)
            jsonReader = this.GetComponent<SimplifiedJson>();
        if (_incidentDifficultyManager == null)
        {
            _incidentDifficultyManager = this.GetComponent<IncidentDifficultyManager>();
        }
        // create new incidents, randome amount between 1 and 3

        var num = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < num; i++)
        {
            if (incidents.Count >= MaxIncidents)
            {
                return;
            }
            var newIncident = new Incident();
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

    public void AddNewIncidents(List<Incident> ongoingIncidents, int turn)
    {
        if (incidents.Count >= MaxIncidents)
        {
            return;
        }

        if (_incidentDifficultyManager == null)
        {
            _incidentDifficultyManager = this.GetComponent<IncidentDifficultyManager>();
        }

        var totalOfficersAvailable = OfficerController.TotalOfficers;
        var currentOfficersAvailable = OfficerController.GetAvailable();

        var newIncidents = _incidentDifficultyManager.GetNewIncidents(ongoingIncidents, MaxIncidents, totalOfficersAvailable, turn +1,
           currentOfficersAvailable, null);

        if (newIncidents != null)
        {
            foreach (var newIncident in newIncidents)
            {
                newIncident.turnToShow = turn;
                newIncident.turnToDevelop = turn + newIncident.turnsToAdd + 1;
                //our complete list of incidents
                incidents.Add(newIncident);

                NextIncident.Add(newIncident);
                m_IncidentQueue.AddToQueue(newIncident);
            }
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
    public void ShowIncidentWithCaseNumber(int caseNum)
    {
        //used in button driven incidents
        var turn = GameObject.Find("TurnManager").GetComponent<TurnManager>().turn;
        ShowIncident(turn, caseNum);
    }
    public void ShowIncident(int turn, int zCaseNumber = -1)
    {
        currentTurn = turn;

        if (NextIncident == null)
        {
            AddNewIncidents(incidents, turn + 1);
        }
        //CreateNewIncident(turn);

        var currentIncident = NextIncident[0];
        m_dialogBox.CurrentIncident = currentIncident;
        SatisfactionImpactGameObject.SetActive(false);
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
                    m_dialogBox.CurrentIncident = currentIncident;
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
        if (m_dialogBox == null)
        {
            m_dialogBox = GameObject.Find("IncidentDialog").GetComponent<DialogBox>();
        }
        currentIncident.Show(ref currentIncident, m_dialogBox);

        m_IncidentQueue.ToggleBackground(currentIncident.caseNumber);

        //make sure the current incident is not showing as new as we now know the player has seen it
        currentIncident.isNew = false;
    }
    public void CaseClosed(int impact, float transitionTime, bool expired = false)
    {
        if (expired)
        {
            if (impact >= 0)
                impact = (impact + 1) * -1;
            AddHappiness(impact);
        }
        else
        {
            AddHappiness(impact);
        }
        happiness = Mathf.Clamp(happiness, 0, 100);
       // m_satisfactionDisplay.SetSatisfactionDisplays(happiness);
        var ratingObjects = m_dialogBox.GetRatingObjects();

        foreach (var feedbackTransform in ratingObjects)
        {
            feedbackTransform.parent = GameObject.Find("Canvas").transform;
            StartCoroutine(m_satisfactionDisplay.TransitionTo(feedbackTransform, transitionTime, happiness));
        }


        _casesClosed++;
        _casesClosedThisTurn++;
    }
    public void EndTurn()
    {
        //punish the player for having cases open, stopping players from just ignoring all cases
        AddHappiness(_endTurnSatisfaction * -1);
        _endTurnSatisfaction = 0;
        happiness = Mathf.Clamp(happiness, 0, 100);
        m_satisfactionDisplay.SetSatisfactionDisplays(happiness);

        // reset the number of cases closed this turn
        _casesClosedThisTurn = 0;
    }
    public void ClearList()
    {
        incidents.Clear();
        NextIncident.Clear();
    }
    public void WaitPressed()
    {
        
#if SELECT_INCIDENTS
        var currentIncident = NextIncident[incidentShowingIndex];
        m_dialogBox.CurrentIncident = currentIncident;
#else
        var currentIncident = NextIncident[0];
        m_dialogBox.CurrentIncident = currentIncident;
#endif
        ScenarioTracker.AddDecision(currentIncident.scenarioNum.ToString(), currentIncident.index.ToString(), "Ignore", Location.CurrentLocation, (DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English"));

        m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().WaitPressed(ref currentIncident);
       // GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref currentIncident, true);
        m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.Waiting);
        //update our lists
        NextIncident[0] = currentIncident;
        for (var i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].caseNumber == currentIncident.caseNumber)
            {
                incidents[i] = currentIncident;
                break;
            }
        }

        AddIncidentHistory(m_dialogBox.CurrentIncident, IncidentHistoryElement.Decision.Ignore);


        ShowNext();
    }
    public void ResolvePressed()
    {
#if SELECT_INCIDENTS
        var currentIncident = NextIncident[incidentShowingIndex];
        m_dialogBox.CurrentIncident = currentIncident;
#else
        var currentIncident = NextIncident[0];
        m_dialogBox.CurrentIncident = currentIncident;
#endif
        ScenarioTracker.AddDecision(currentIncident.scenarioNum.ToString(), currentIncident.index.ToString(), "Officer", Location.CurrentLocation, (DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English"));

        m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
            
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().OfficerPressed(ref currentIncident);

        m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.OfficersSent);
        NextIncident[0] = currentIncident;

        for (var i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].caseNumber == currentIncident.caseNumber)
            {
                incidents[i] = currentIncident;
                break;
            }
        }

        AddIncidentHistory(m_dialogBox.CurrentIncident, IncidentHistoryElement.Decision.Officer);


        ShowNext();
    }

    public void CitizenHelpPressed()
    {
        
#if SELECT_INCIDENTS
        Incident currentIncident = NextIncident[incidentShowingIndex];
        m_dialogBox.CurrentIncident = currentIncident;
#else
        var currentIncident = NextIncident[0];
        m_dialogBox.CurrentIncident = currentIncident;
#endif
        ScenarioTracker.AddDecision(currentIncident.scenarioNum.ToString(), currentIncident.index.ToString(), "Citizen", Location.CurrentLocation, (DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English"));

        m_IncidentQueue.RemoveWarningIcon(currentIncident.caseNumber);
        m_IncidentQueue.ChangeCaseState(currentIncident.caseNumber, IncidentCase.State.CitizenRequest);
        //make sure the incident is updated next turn, we will handle the citizen request result when we next show the incident
        currentIncident.turnToShow++;
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().CitizenPressed(ref currentIncident);
        NextIncident[0] = currentIncident;
        for (var i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].caseNumber == currentIncident.caseNumber)
            {
                incidents[i] = currentIncident;
                break;
            }
        }

        AddIncidentHistory(m_dialogBox.CurrentIncident, IncidentHistoryElement.Decision.Citizen);


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
            var tmp = this.gameObject.GetComponent<TurnManager>();
            tmp.NextTurnButton.SetActive(true);
			tmp.EndTurnSatisfaction.SetActive(true);

            // Get our updated statistics
            var total = GetTotalCasesCount();
            var ignored = GetIgnoredCasesCount();
            var active = GetActiveCaseCount();
            var actionTaken = GetDeployedCaseCount();

            var casesClosed = GetTotalCasesClosed();
            var casesClosedThisTurn = GetTurnClosedCaseCount();

            _endTurnSatisfaction = GetEndTurnSatisfactionDeduction();

            GameObject.Find("GameInformationPanel").GetComponent<InformationPanel>().DisableAll();
            tmp.EndTurnSatisfaction.GetComponent<EndTurnSatisfaction>().SetText(total, casesClosed, casesClosedThisTurn, active, actionTaken, ignored);
            ShowSatisfactionImpact(-_endTurnSatisfaction);
        }
        else
            ShowIncident(currentTurn);
    }

    public void ShowSatisfactionImpact(int impact)
    {
        SatisfactionImpactGameObject.SetActive(true);
        var satisfactionText = "";
        satisfactionText += Localization.Get("BASIC_TEXT_SATISFACTION_IMPACT") + ": " + impact;
        SatisfactionImpactGameObject.GetComponentInChildren<Text>().text = satisfactionText;
    }
    public void CloseCase(int caseNumber, float transitionTime)
    {
        for (int i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].caseNumber == caseNumber)
            {
                //we have found the case to remove
                m_IncidentQueue.ChangeCaseState(caseNumber, IncidentCase.State.Resolved);
                m_IncidentQueue.RemoveFromQueue(incidents[i].caseNumber);

                CaseClosed(incidents[i].satisfactionImpact, transitionTime);

                incidents.RemoveAt(i);
                i--;


            }
        }
    }
    public bool IsGameOver()
    {
        return GetHappiness() < 10f;
    }
    public int GetHappiness()
    {
        return Mathf.RoundToInt(happiness);
    }

   
    /// <summary>
    /// Gets the total severity of all active cases
    /// </summary>
    /// <returns></returns>
    public int GetTotalSeverity()
    {
        var total = 0;
        foreach(var i in m_IncidentQueue.allCases)
        {
            if (i.severityNumber != 0 && i.gameObject.activeSelf && i.m_state == IncidentCase.State.Waiting)
            {
                total += i.severityNumber;
            }
        }
        return total;
    }
    /// <summary>
    /// The number of cases that the player has chosen to ignore this turn
    /// </summary>
    /// <returns></returns>
    public int GetIgnoredCasesCount()
    {
        var total = 0;
        foreach (var i in m_IncidentQueue.allCases)
        {
            if (i.m_state == IncidentCase.State.Waiting)
            {
                total += 1;
            }
        }
        return total;
    }

    /// <summary>
    /// The total number of cases that are currently active
    /// </summary>
    /// <returns></returns>
    public int GetActiveCaseCount()
    {
        var total = 0;
        foreach (var i in m_IncidentQueue.allCases)
        {
            if (i.m_state == IncidentCase.State.Waiting 
                || i.m_state == IncidentCase.State.CitizenRequest 
                || i.m_state == IncidentCase.State.OfficersSent)
            {
                total += 1;
            }
        }
        return total;
    }

    /// <summary>
    /// The number of cases that are active and have an action taken (that is not ignore)
    /// </summary>
    /// <returns></returns>
    public int GetDeployedCaseCount()
    {
        var total = 0;

        foreach (var incidentCase in m_IncidentQueue.allCases)
        {
            if (incidentCase.m_state == IncidentCase.State.OfficersSent ||
                incidentCase.m_state == IncidentCase.State.CitizenRequest)
            {
                total++;
            }
        }
        return total;
    }

    /// <summary>
    /// Gets the total number of cases closed
    /// </summary>
    /// <returns></returns>
    public int GetTotalCasesClosed()
    {
        return _casesClosed;
    }

    /// <summary>
    /// Gets a total number of cases closed this turn
    /// </summary>
    /// <returns></returns>
    public int GetTurnClosedCaseCount()
    {
        return _casesClosedThisTurn;
    }

    /// <summary>
    /// Gets the total number of cases that there have been
    /// </summary>
    /// <returns></returns>
    public int GetTotalCasesCount()
    {
        return _casesClosed + GetActiveCaseCount();
    }

    /// <summary>
    /// Gets the amount of satisfaction a player loses through ignored cases
    /// </summary>
    /// <returns></returns>
    public int GetEndTurnSatisfactionDeduction()
    {
        return GetTotalSeverity()*4;
    }

    public Color GetSeverityColor(int severity)
    {
        switch (severity)
        {
            case 1:
                return LowSeverity;
            case 2:
                return MidSeverity;
            case 3:
                return HighSeverity;
            default:
                return UnknownSeverity;
        }
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
            Severity = incident.severity,
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

    private void AddHappiness(int value)
    {
        happiness += value;
    }
}
