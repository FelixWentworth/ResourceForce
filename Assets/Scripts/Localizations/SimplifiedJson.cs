using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

public class SimplifiedJson : MonoBehaviour {

    //how the json data is stored
    /*
     * each scenario is broken down into different incidents each with their own index
     * 
     * [_INDEX_] : [ [_TYPE_] , [_NAME_] , [_WAIT_INDEX_] , [_OFFICER_INDEX_] , [_CITIZEN_INDEX] , [_OFFICERS_] , [_TURNS_REQUIRED_] , [_SEVERITY_] , [_SATISFACTION_IMPACT] ]
     * 
     * for easy look up the idexes are below
     * 
     * [_INDEX_] : [ type=0, name=1, waitIndex=2. officerIndex=3, citizenIndex=4, officers=5, turnsRequired=6, severity=7, satisfactionImpact=8 ]
     * 
     */

    public Location m_location;

    private TextAsset _myText;
    private TurnManager _manager;

    public static int Identifier = 1;

    void Awake()
    {
        _manager = this.GetComponent<TurnManager>();
    }

    // Use this for initialization
    public void CreateNewIncident(ref Incident zIncident, List<Incident> incidents = null ) {

        _myText = Resources.Load(m_location.IncidentFilePath) as TextAsset;

        var randIncident = Random.Range(1, m_location.numIncidents+1);

        // Should be hidden behind ALLOW_DUPLICATE_INCIDENTS flag in incident manager
        if (incidents != null)
        {
            // We have provided the list of incidents to make sure that the incident is not duplicated, now get a random incident index that is not currently in use
            var indexesInUse = incidents.Select(i => i.scenarioNum).ToList();
            while (indexesInUse.Contains(randIncident) && indexesInUse.Count() < m_location.numIncidents)
            {
                // Keep looking for an incident that is not used, and we have other incidents we can show
                randIncident = Random.Range(1, m_location.numIncidents + 1);
            }
        }

        zIncident = GetIncidentAtIndex(1, randIncident);
        
        //add the case number to help the player keep track of it throughout its life
        zIncident.caseNumber = Identifier;
        Identifier++;

    }
    private Incident GetIncidentAtIndex(int index, int scenarioNum, int caseNum = 0)
    {
        if (_myText == null)
            _myText = Resources.Load(m_location.IncidentFilePath) as TextAsset;
        var N = JSON.Parse(_myText.text);

        var tmp = new Incident();
        var name = "Scenario_" + scenarioNum;

        tmp.scenarioNum = scenarioNum;
        tmp.index = index;
        var i = index.ToString();


        //get the officers, turns to add from officers and severity values

        tmp.officer = int.Parse(N[name][i][5]);
        tmp.turnsToAdd = int.Parse(N[name][i][6]);
        tmp.severity = int.Parse(N[name][i][7]);

        //set the name
        tmp.incidentName = N[name][i][1];
        tmp.type = N[name][i][0];

        //set the indexes for the buttons
        var w = tmp.waitIndex = int.Parse(N[name][i][2]);
        var o = tmp.officerIndex = int.Parse(N[name][i][3]);
        var c = tmp.citizenIndex = int.Parse(N[name][i][4]);

        tmp.satisfactionImpact = int.Parse(N[name][i][8]);

        //this will count as being resolved when there are no buttons to tap on
        tmp.resolved = (w == -1 && o == -1 && c == -1);

        if (caseNum != 0)
        {
            //ongoing case
            tmp.caseNumber = caseNum;
        }

        tmp.feedbackRatingWait = int.Parse(N[name][i][9]);
        tmp.feedbackWait = N[name][i][10];

        tmp.feedbackRatingOfficer = int.Parse(N[name][i][11]);
        tmp.feedbackOfficer = N[name][i][12];

        tmp.feedbackRatingCitizen = int.Parse(N[name][i][13]);
        tmp.feedbackCitizen = N[name][i][14];

        return tmp;
    }
    public void WaitPressed(ref Incident zIncident)
    {
        var index = zIncident.waitIndex;
        var currentIndex = zIncident.index;
        var turnToDevelop = zIncident.turnToDevelop;
        if (index == -1)
        {
            //we should not be able to press the button
            return;
        }
        //we haave chosen to wait, so check the wait index
        zIncident = GetIncidentAtIndex(index, zIncident.scenarioNum, zIncident.caseNumber);
        zIncident.turnToShow = _manager.turn + 1;
        if (index != currentIndex)
        {
            //new incident
            zIncident.turnToDevelop = _manager.turn + 3;
        }
        else
            zIncident.turnToDevelop = turnToDevelop;
    }
    public void OfficerPressed(ref Incident zIncident)
    {
        var index = zIncident.officerIndex;
        var turnsToAdd = zIncident.turnsToAdd;
        if (index == -1)
            return;
        zIncident = GetIncidentAtIndex(index, zIncident.scenarioNum, zIncident.caseNumber);
        zIncident.turnToShow = _manager.turn + turnsToAdd;
        zIncident.turnToDevelop = _manager.turn + 3;
    }
    public void CitizenPressed(ref Incident zIncident)
    {
        var index = zIncident.citizenIndex;
        if (index == -1)
            return;
        zIncident = GetIncidentAtIndex(index, zIncident.scenarioNum, zIncident.caseNumber);
        zIncident.turnToShow = _manager.turn + 1;
        zIncident.turnToDevelop = _manager.turn + 3;
    }
}
