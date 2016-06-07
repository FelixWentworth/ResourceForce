using UnityEngine;
using System.Collections;
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

    string filePath = "ScenarioInformation";
    TextAsset myText;
    TurnManager manager;
    public static int identifier = 1;

    void Awake()
    {
        manager = this.GetComponent<TurnManager>();
    }

    // Use this for initialization
    public void CreateNewIncident(ref Incident zIncident) {
        //select a new branching incident from our 

        myText = Resources.Load(filePath) as TextAsset;
        var N = JSON.Parse(myText.text);

        // get a random incident -> todo change this value to represent the number of cases available
        int randIncident = UnityEngine.Random.Range(1, 3);

        zIncident = GetIncidentAtIndex(1, randIncident);
        
        //add the case number to help the player keep track of it throughout its life
        zIncident.caseNumber = identifier;
        identifier++;
    }
    private Incident GetIncidentAtIndex(int index, int scenarioNum, int caseNum = 0)
    {
        if (myText == null)
            myText = Resources.Load(filePath) as TextAsset;
        var N = JSON.Parse(myText.text);

        Incident tmp = new Incident();
        string name = "Scenario" + scenarioNum;

        tmp.scenarioNum = scenarioNum;
        tmp.index = index;
        string ind = index.ToString();
        //get the officers, turns to add from officers and severity values

        tmp.officer = int.Parse(N[name][ind][5]);
        tmp.turnsToAdd = int.Parse(N[name][ind][6]);
        tmp.severity = int.Parse(N[name][ind][7]);

        //set the name
        tmp.incidentName = N[name][ind][1];
        tmp.type = N[name][ind][0];

        //set the indexes for the buttons
        int w = tmp.waitIndex = int.Parse(N[name][ind][2]);
        int o = tmp.officerIndex = int.Parse(N[name][ind][3]);
        int c = tmp.citizenIndex = int.Parse(N[name][ind][4]);

        tmp.satisfactionImpact = int.Parse(N[name][ind][8]);

        //this will count as being resolved when there are no buttons to tap on
        tmp.resolved = (w == -1 && o == -1 && c == -1);

        if (caseNum != 0)
        {
            //ongoing case
            tmp.caseNumber = caseNum;
        }

        return tmp;
    }
    public void WaitPressed(ref Incident zIncident)
    {
        int index = zIncident.waitIndex;
        if (index == -1)
        {
            //we should not be able to press the button
            return;
        }
        //we haave chosen to wait, so check the wait index
        zIncident = GetIncidentAtIndex(index, zIncident.scenarioNum, zIncident.caseNumber);
        zIncident.turnToShow = manager.turn + 1;
    }
    public void OfficerPressed(ref Incident zIncident)
    {
        int index = zIncident.officerIndex;
        int turnsToAdd = zIncident.turnsToAdd;
        if (index == -1)
            return;
        zIncident = GetIncidentAtIndex(index, zIncident.scenarioNum, zIncident.caseNumber);
        zIncident.turnToShow = manager.turn + turnsToAdd;
    }
    public void CitizenPressed(ref Incident zIncident)
    {
        int index = zIncident.citizenIndex;
        if (index == -1)
            return;
        zIncident = GetIncidentAtIndex(index, zIncident.scenarioNum, zIncident.caseNumber);
        zIncident.turnToShow = manager.turn + 1;
    }
}
