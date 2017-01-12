using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class IncidentDifficultyManager : MonoBehaviour
{
    private const float MaxOfficerUsePercentage = 1.0f;
    private const int TurnsToMaxOfficer = 3;

    private SimplifiedJson _incidentLoader;

    /// <summary>
    /// Get the percentage of total officers that a player should have for this turn
    /// </summary>
    /// <param name="currentTurn"></param>
    /// <returns></returns>
    private float GetOfficerRequiredPercentage(int currentTurn)
    {
        if (currentTurn >= TurnsToMaxOfficer)
        {
            return MaxOfficerUsePercentage;
        }
        else
        {
            return MaxOfficerUsePercentage/(TurnsToMaxOfficer/(currentTurn - 1));
        }
    }

    /// <summary>
    /// Get the target number of officers that should be required this turn
    /// </summary>
    /// <param name="numOfficers">The total number of officers available</param>
    /// <param name="turnNumber">The current turn</param>
    /// <returns></returns>
    private int GetTargetOfficers(int numOfficers, int turnNumber)
    {
        return Mathf.RoundToInt(numOfficers* GetOfficerRequiredPercentage(turnNumber));
    }

    /// <summary>
    /// Get the number of officers that new incidents should require this turn
    /// </summary>
    /// <param name="numOfficers">The total number of officers available</param>
    /// <param name="turnNumber">The current turn</param>
    /// <param name="currentMostOfficersRequired">The most officers currently required by active incidents that need action this turn</param>
    /// <param name="currentLeastOfficersRequired">The least officers currently required by active incidents that need action this turn</param>
    /// <returns></returns>
    private int GetTotalOfficersRequiredForNewIncidents(int numOfficers, int turnNumber,
        int currentMostOfficersRequired, int currentLeastOfficersRequired)
    {
        var targetNumber = GetTargetOfficers(numOfficers, turnNumber);

        var officersRequired = GetOfficerRequiredPercentage(turnNumber);

        var incidentOfficersRequired = targetNumber -
                                       (currentLeastOfficersRequired +
                                        (currentMostOfficersRequired - currentLeastOfficersRequired*officersRequired));

        return Mathf.RoundToInt(incidentOfficersRequired);
    }


    /// <summary>
    /// Get the percentage chance to get an extra officer
    /// </summary>
    /// <param name="extraOfficersRequired">The number of extra officers the next turn will require</param>
    /// <param name="turnNumber">The current turn number</param>
    /// <returns></returns>
    private float GetChanceForExtraOfficer(int extraOfficersRequired, int turnNumber)
    {
        var percentageChance = (1f - GetOfficerRequiredPercentage(turnNumber));

        return percentageChance*extraOfficersRequired;
    }

    /// <summary>
    /// Get the most number of officers required for the active incidents if the user always chooses to send officers
    /// </summary>
    /// <param name="incidents">List of incidents that need an action this turn</param>
    /// <returns></returns>
    private int GetMostOfficersRequiredForThisTurn(List<Incident> incidents)
    {
        var officersRequired = 0;
        foreach (var incident in incidents)
        {
            if (incident.officerIndex != -1)
            {
                officersRequired += incident.officer;
            }
        }
        return officersRequired;
    }

    /// <summary>
    /// Get the least number of officers required for the active incidents if the user sends citizens when possible
    /// </summary>
    /// <param name="incidents">List of incidents that need an action this turn</param>
    /// <returns></returns>
    private int GetLeastOfficersRequiredForThisTurn(List<Incident> incidents)
    {
        var officersRequired = 0;
        foreach (var incident in incidents)
        {
            if (incident.citizenIndex == -1 && incident.officerIndex != -1)
            {
                officersRequired += incident.officer;
            }
        }
        return officersRequired;
    }

    /// <summary>
    /// Get a new incident that the number of officers required is less than or equal to the requirement limit
    /// </summary>
    /// <param name="officerRequirementLimit">Limit of number of officers required for sending to new incident</param>
    /// <returns></returns>
    private Incident GetNewIncident(int officerRequirementLimit)
    {
        var incident = new Incident();

        if (_incidentLoader == null)
        {
            _incidentLoader = GameObject.Find("TurnManager").GetComponent<SimplifiedJson>();
        }

        do
        {
            _incidentLoader.CreateNewIncident(ref incident);
        }
        while (incident.officerIndex == -1 || incident.officer > officerRequirementLimit);

        return incident;
    }

    /// <summary>
    /// Determine if the game should award the player more officers
    /// </summary>
    /// <param name="target">The target number of officers needed</param>
    /// <param name="available">The number of available officers</param>
    /// <param name="turnNumber">The current turn number</param>
    /// <returns></returns>
    private bool ShouldAwardMoreOfficers(int target, int available, int turnNumber)
    {
        var chanceOfNewOfficer = GetChanceForExtraOfficer(target - available, turnNumber);

        //now test if the user is successful
        var random = Random.Range(0f, 1f);

        return random > chanceOfNewOfficer;
    }

    /// <summary>
    /// Get a list of new incidents to show at the new turn stage
    /// </summary>
    /// <param name="currentIncidents">List of current incidents</param>
    /// <param name="totalOfficers">The total officers available</param>
    /// <param name="turnNumber">The current turn number</param>
    /// <param name="availableOfficers">The number of available officers</param>
    /// <param name="AddOfficersAction">An action to call if the player is awarded more officers</param>
    /// <returns></returns>
    public List<Incident> GetNewIncidents(List<Incident> currentIncidents, int maximumIncidentx, int totalOfficers, int turnNumber, int availableOfficers, UnityAction<int> AddOfficersAction)
    {
        var incidentList = new List<Incident>();

        var bestOfficerUsage = GetLeastOfficersRequiredForThisTurn(currentIncidents);
        var worstOfficerUsage = GetMostOfficersRequiredForThisTurn(currentIncidents);

        var newIncidentOfficerLimit = GetTotalOfficersRequiredForNewIncidents(totalOfficers, turnNumber,
            worstOfficerUsage, bestOfficerUsage);


        Debug.Log("Target officers for players to use this turn: " + newIncidentOfficerLimit);

        var currentIncidentsShowing = currentIncidents.Count;

        if (newIncidentOfficerLimit == 0)
        {
            return null;
        }
        else
        {
            while (newIncidentOfficerLimit > 0 ) 
            {
                var newIncident = GetNewIncident(newIncidentOfficerLimit);
                newIncidentOfficerLimit -= newIncident.officer;

                incidentList.Add(newIncident);
            }
        }

        //select the most officer intensive 
        incidentList.Sort((a, b) => a.officer.CompareTo(b.officer));

        var countNewIncidents = maximumIncidentx - currentIncidentsShowing;

        if (incidentList.Count > countNewIncidents)
        {
            incidentList = incidentList.Take(countNewIncidents).ToList();
        }

        //incidentList.Sort((a, b) => a.citizenIndex.CompareTo(b.citizenIndex));

        // Check the player has enough officers to complete the round
        var officersRequired = newIncidentOfficerLimit > availableOfficers;

        if (officersRequired)
        {
            if (ShouldAwardMoreOfficers(newIncidentOfficerLimit, availableOfficers, turnNumber))
            {
                AddOfficersAction(newIncidentOfficerLimit - availableOfficers);
            }
            
        }

        return incidentList;
    }
}
