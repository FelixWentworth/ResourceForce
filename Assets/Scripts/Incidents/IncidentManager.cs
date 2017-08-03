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
    protected float Happiness = 10f;
    protected float MaxHappiness = 20f;

    public DialogBox m_dialogBox;
    public SatisfactionDisplays m_satisfactionDisplay;

    public Color LowSeverity;
    public Color MidSeverity;
    public Color HighSeverity;
    public Color UnknownSeverity;

    public GameObject SatisfactionImpactGameObject;

    private Dictionary<string, IncidentHistory> _incidentHistories = new Dictionary<string, IncidentHistory>();

    private int _casesClosed;
    private int _casesClosedWell;
    private int _casesClosedThisTurn;

    private int _endTurnSatisfaction;

    private const int MaxIncidents = 7;
   
#if SELECT_INCIDENTS
    private int incidentShowingIndex = 1;
#endif
    void Start()
    {
        m_satisfactionDisplay.SetSatisfactionDisplays(Happiness, MaxHappiness);
        _casesClosed = 0;
        _casesClosedWell = 0;
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
       
        if (incidents.Count >= MaxIncidents)
        {
            return;
        }

        var location = Location.CurrentLocation;
        var language = DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English";

        var newIncident = new Incident();
#if ALLOW_DUPLICATE_INCIDENTS
        jsonReader.CreateNewScenario(location, language);
#else
        jsonReader.CreateNewScenario(location, language, incidents);
#endif
        newIncident.TurnToShow = zTurn;
        newIncident.TurnToDevelop = zTurn + newIncident.IncidentContent.TurnReq + 1;
        //our complete list of incidents
        incidents.Add(newIncident);
        //our list of incidents waiting to show this turn
        NextIncident.Add(newIncident);
        m_IncidentQueue.AddToQueue(newIncident);


    }

    public void AddNewIncidents(int turn)
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

        var newIncidents = _incidentDifficultyManager.GetNewIncidents(incidents, MaxIncidents, totalOfficersAvailable, turn +1,
           currentOfficersAvailable, null);
        if (newIncidents != null)
        {
            foreach (var newIncident in newIncidents)
            {
                newIncident.TurnToShow = turn;
                newIncident.TurnToDevelop = turn + newIncident.IncidentContent.TurnReq + 1;
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
            if (incidents[i].TurnToShow <= zTurn)
            {
                //this incident wants to be shown on this turn
                m_IncidentQueue.ShowWarningIcon(incidents[i].Scenario.Id);
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
                //status += Localization.Get("INCIDENT_CASE") + incidents[i].caseNumber + ": " + incidents[i].NameBeforeDeveloped + "\n";
                m_IncidentQueue.UpdateSeverity(incidents[i].Scenario.Id, incidents[i].IncidentContent.Severity);
            }
        }
        CaseStatus.text = status;
    }
    public void CheckExpiredIncidents(int turn)
    {
        for (int i=0; i<incidents.Count; i++)
        {
            if (incidents[i].TurnToDevelop < turn)
            {
                incidents[i].IncidentContent.SatisfactionImpact = -1 * incidents[i].IncidentContent.SatisfactionImpact;
            }
        }
    }
    public void ShowIncidentWithCaseNumber(string caseNum)
    {
        //used in button driven incidents
        var turn = GameObject.Find("TurnManager").GetComponent<TurnManager>().turn;
        ShowIncident(turn, caseNum);
    }
    public void ShowIncident(int turn, string caseNumber = "")
    {
        currentTurn = turn;
        if (NextIncident == null || NextIncident.Count == 0)
        {
            AddNewIncidents(turn + 1);
        }

        var currentIncident = NextIncident[0];
        if (m_dialogBox == null)
        {
            m_dialogBox = GameObject.Find("IncidentDialog").GetComponent<DialogBox>();
        }
        m_dialogBox.CurrentIncident = currentIncident;
        SatisfactionImpactGameObject.SetActive(false);
#if SELECT_INCIDENTS
        incidentShowingIndex = 0;
#endif
        if (caseNumber != "")
        {
            //we have a case to show
            bool incidentFound = false;
            for (int i = 0; i < NextIncident.Count; i++)
            {
                //find the case to show by caseNumber
                if (NextIncident[i].Scenario.Id == caseNumber)
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
        
        currentIncident.Show(ref currentIncident, m_dialogBox);

        m_IncidentQueue.ToggleBackground(currentIncident.Scenario.Id);

        //make sure the current incident is not showing as new as we now know the player has seen it
        currentIncident.IsNew = false;
    }
    public void CaseClosed(float impact, float transitionTime, bool expired = false)
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

        _casesClosedWell += impact > 0 ? 1 : 0;

        Happiness = Mathf.Clamp(Happiness, 0, MaxHappiness);
       // m_satisfactionDisplay.SetSatisfactionDisplays(happiness);
        var ratingObjects = m_dialogBox.GetRatingObjects();

        foreach (var feedbackTransform in ratingObjects)
        {
            feedbackTransform.SetParent(GameObject.Find("Canvas").transform);
            StartCoroutine(m_satisfactionDisplay.TransitionTo(feedbackTransform, transitionTime, Happiness));
        }


        _casesClosed++;
        _casesClosedThisTurn++;
    }
    public void EndTurn()
    {
        _endTurnSatisfaction = 0;
        Happiness = Mathf.Clamp(Happiness, 0, MaxHappiness);
        m_satisfactionDisplay.SetSatisfactionDisplays(Happiness, MaxHappiness);

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
        AddIncidentHistory(m_dialogBox.CurrentIncident, IncidentHistoryElement.Decision.Ignore);

#if SELECT_INCIDENTS
        var currentIncident = NextIncident[incidentShowingIndex];
        m_dialogBox.CurrentIncident = currentIncident;
#else
        var currentIncident = NextIncident[0];
        m_dialogBox.CurrentIncident = currentIncident;
#endif
        ScenarioTracker.AddDecision(currentIncident.Scenario.Id, currentIncident.IncidentContent.Title, "Ignore", Location.CurrentLocation, (DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English"));

        m_IncidentQueue.RemoveWarningIcon(currentIncident.Scenario.Id);
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().WaitPressed(ref currentIncident);
       // GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref currentIncident, true);
        m_IncidentQueue.ChangeCaseState(currentIncident.Scenario.Id, IncidentCase.State.Waiting);
        //update our lists
        NextIncident[0] = currentIncident;
        for (var i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].Scenario.Id == currentIncident.Scenario.Id)
            {
                incidents[i] = currentIncident;
                break;
            }
        }



        ShowNext();
    }
    public void ResolvePressed()
    {
        AddIncidentHistory(m_dialogBox.CurrentIncident, IncidentHistoryElement.Decision.Officer);


#if SELECT_INCIDENTS
        var currentIncident = NextIncident[incidentShowingIndex];
        m_dialogBox.CurrentIncident = currentIncident;
#else
        var currentIncident = NextIncident[0];
        m_dialogBox.CurrentIncident = currentIncident;
#endif
        ScenarioTracker.AddDecision(currentIncident.Scenario.Id, currentIncident.IncidentContent.Title, "Officer", Location.CurrentLocation, (DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English"));

        m_IncidentQueue.RemoveWarningIcon(currentIncident.Scenario.Id);
            
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().OfficerPressed(ref currentIncident);

        m_IncidentQueue.ChangeCaseState(currentIncident.Scenario.Id, IncidentCase.State.OfficersSent);
        NextIncident[0] = currentIncident;

        for (var i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].Scenario.Id == currentIncident.Scenario.Id)
            {
                incidents[i] = currentIncident;
                break;
            }
        }



        ShowNext();
    }

    public void CitizenHelpPressed()
    {
        AddIncidentHistory(m_dialogBox.CurrentIncident, IncidentHistoryElement.Decision.Citizen);

#if SELECT_INCIDENTS
        Incident currentIncident = NextIncident[incidentShowingIndex];
        m_dialogBox.CurrentIncident = currentIncident;
#else
        var currentIncident = NextIncident[0];
        m_dialogBox.CurrentIncident = currentIncident;
#endif
        ScenarioTracker.AddDecision(currentIncident.Scenario.Id, currentIncident.IncidentContent.Title, "Citizen", Location.CurrentLocation, (DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English"));

        m_IncidentQueue.RemoveWarningIcon(currentIncident.Scenario.Id);
        m_IncidentQueue.ChangeCaseState(currentIncident.Scenario.Id, IncidentCase.State.CitizenRequest);
        //make sure the incident is updated next turn, we will handle the citizen request result when we next show the incident
        currentIncident.TurnToShow++;
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().CitizenPressed(ref currentIncident);
        NextIncident[0] = currentIncident;
        for (var i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].Scenario.Id == currentIncident.Scenario.Id)
            {
                incidents[i] = currentIncident;
                break;
            }
        }



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

            GameObject.Find("GameInformationPanel").GetComponent<InformationPanel>().DisableAll();
            tmp.EndTurnSatisfaction.GetComponent<EndTurnSatisfaction>().SetText(total, casesClosed, casesClosedThisTurn, active, actionTaken, ignored);
            ShowSatisfactionImpact(_endTurnSatisfaction);
        }
        else
            ShowIncident(currentTurn);
    }

    public void ShowSatisfactionImpact(float impact, bool closeCase = false)
    {
        if (closeCase)
        {
            // Convert to actual value
            impact = Mathf.RoundToInt(impact * (100f / MaxHappiness));
        }
        SatisfactionImpactGameObject.SetActive(true);
        var satisfactionText = "";
        satisfactionText += Localization.Get("BASIC_TEXT_SATISFACTION_IMPACT") + ": ";
        satisfactionText += impact > 0 ? "+" + impact: impact.ToString();
        satisfactionText += "%";
        SatisfactionImpactGameObject.GetComponentInChildren<Text>().text = satisfactionText;
    }
    public void CloseCase(string Id, float transitionTime)
    {
        for (int i = 0; i < incidents.Count; i++)
        {
            if (incidents[i].Scenario.Id == Id)
            {
                //we have found the case to remove
                m_IncidentQueue.ChangeCaseState(Id, IncidentCase.State.Resolved);
                m_IncidentQueue.RemoveFromQueue(incidents[i].Scenario.Id);

                float impact = incidents[i].IncidentContent.SatisfactionImpact;
                if (impact > 0)
                {
                    impact = impact * 0.4f;
                }

                CaseClosed(impact, transitionTime);

                incidents.RemoveAt(i);
                i--;


            }
        }
    }

    /// <summary>
    /// Check if the happiness is below the end game threshold
    /// </summary>
    /// <returns>if the game is over</returns>
    public bool IsGameOver()
    {
        // Happiness is less than 10%
        return GetHappiness() < 10f;
    }

    /// <summary>
    /// Get the numerical value of happiness
    /// </summary>
    /// <returns>Current happiness value</returns>
    public float GetActualHappiness()
    {
        return Happiness;
    }

    /// <summary>
    /// Get the % value of happiness as shown in game
    /// </summary>
    /// <returns>Current happiness %</returns>
    public float GetHappiness()
    {
        return (Happiness / MaxHappiness) * 100f;
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
    /// Gets the total number of cases that the player has resolved positively
    /// </summary>
    /// <returns></returns>
    public int GetTotalCasesClosedWell()
    {
        return _casesClosedWell;
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
        var type = incident.IncidentContent.Title;
        var feedbackRating = 1;
        var feedback = "";

        var historyElement = new IncidentHistoryElement
        {
            Type = type,
            Description = incident.IncidentContent.Description + "\n\n" + "<color=yellow>" + m_dialogBox.GetTip() + "</color>",
            Feedback = feedback,
            FeedbackRating = feedbackRating,
            Severity = incident.IncidentContent.Severity,
            PlayerDecision = decision
        };

        UpdateHistory(incident, historyElement);
    }
    private void UpdateHistory(Incident incident, IncidentHistoryElement element)
    {
        var incidentHistory = new IncidentHistory();

        if (_incidentHistories.ContainsKey(incident.Scenario.Id))
        {
            _incidentHistories.TryGetValue(incident.Scenario.Id, out incidentHistory);
        }

        incidentHistory.IncidentHistoryElements.Add(element);
        _incidentHistories[incident.Scenario.Id] = incidentHistory;
    }

    public List<IncidentHistoryElement> GetIncidentHistory(string scenarioId)
    {
        IncidentHistory incidentHistory;

        return _incidentHistories.TryGetValue(scenarioId, out incidentHistory) ? 
            incidentHistory.IncidentHistoryElements 
            : 
            new List<IncidentHistoryElement>();
    }

    public void AddHappiness(float value)
    {
        Happiness += value;
        Happiness = Mathf.Clamp(Happiness, 0, MaxHappiness);
        _endTurnSatisfaction += Mathf.RoundToInt(value * (100f / MaxHappiness));
    }
}
