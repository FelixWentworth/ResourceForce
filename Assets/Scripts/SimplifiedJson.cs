using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SimplifiedJson : MonoBehaviour {

    string filePath = "ScenarioInformation";
    TextAsset myText;
    public static int identifier = 1;
    // Use this for initialization
    public void CreateNewIncident(ref Incident zIncident) {
        //select a new branching incident from our 

        myText = Resources.Load(filePath) as TextAsset;
        var N = JSON.Parse(myText.text);

        // get a random incident
        int randIncident = UnityEngine.Random.Range(1, 3);

        zIncident = GetIncidentAtIndex(1, randIncident);
        
        //add the case number to help the player keep track of it throughout its life
        zIncident.caseNumber = identifier;
        identifier++;

        ////get a random incident
        //int randIncident = UnityEngine.Random.Range(0, int.Parse(N["TotalIncidentTypes"]));
        //int randArea = UnityEngine.Random.Range(0, int.Parse(N["TotalAreaTypes"]));

        //zIncident.index = randIncident;
        //zIncident.officer = int.Parse(N["Incidents"][randIncident][1]);
        //zIncident.area = N["Areas"][randArea];
        //zIncident.incidentName = GetIncidentFromType(N["Incidents"][randIncident][0]);
        //zIncident.turnsToAdd = int.Parse(N["Incidents"][randIncident][3]);
        //zIncident.resolved = false;
        //zIncident.caseNumber = identifier;
        //zIncident.severity = int.Parse(N["Incidents"][randIncident][4]);
        //identifier++;
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
        zIncident.turnToShow += 1;
    }
    public void OfficerPressed(ref Incident zIncident)
    {
        int index = zIncident.officerIndex;
        if (index == -1)
            return;
        zIncident = GetIncidentAtIndex(index, zIncident.scenarioNum, zIncident.caseNumber);
        zIncident.turnToShow += zIncident.turnsToAdd;
    }
    public void CitizenPressed(ref Incident zIncident)
    {
        int index = zIncident.citizenIndex;
        if (index == -1)
            return;
        zIncident = GetIncidentAtIndex(index, zIncident.scenarioNum, zIncident.caseNumber);
        zIncident.turnToShow += 1;
    }
    //public void DevelopIncident(ref Incident zIncident, bool waiting)
    //{
    //    //we must now see if the incident can be developed
    //    if (myText == null)
    //        myText = Resources.Load(filePath) as TextAsset;
    //    var N = JSON.Parse(myText.text);

    //    //check the index of the developed index
    //    int index = int.Parse(N["Incidents"][zIncident.index][2]);

    //    if (index == -1)
    //    {
    //        TurnManager turn = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    //        if (turn.turn >= zIncident.turnToDevelop)
    //        {
    //            //the incident that does not develop will expire, for now we will resolve the case, TODO make this a unsuccessful resolution
    //            zIncident.resolved = true;
    //            zIncident.expired = true;
    //            zIncident.positiveResolution = false;
    //        }
    //        else if (waiting)
    //        {
    //            //case does not resolve if you do nothing and has no developments, instead we will delay the showing of this message
    //            zIncident.turnToShow += 1;
    //        }
    //        else
    //        {
    //            //we must resolve the issue
    //            zIncident.resolved = true;
    //            zIncident.positiveResolution = true;
    //            zIncident.turnToShow += zIncident.turnsToAdd;
    //        }
    //    }
    //    else
    //    {
    //        if (waiting)
    //        {
    //            //check to see if the current incident has been ignored enough to develop into a new incident
    //            TurnManager turn = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    //            if (zIncident.turnToDevelop >= turn.turn-1)
    //            {
    //                zIncident.turnToShow += 1;
    //            }
    //            else
    //            {
    //                //bad response so develop the situation
    //                zIncident.nameBeforeDeveloped = zIncident.incidentName;
    //                zIncident.incidentName = GetIncidentFromType(N["Incidents"][index][0]);
    //                zIncident.officer = int.Parse(N["Incidents"][index][1]);
    //                zIncident.turnToShow += 1;
    //                zIncident.turnsToAdd = int.Parse(N["Incidents"][index][3]);
    //                zIncident.turnToDevelop = turn.turn + 3;
    //                zIncident.severity = int.Parse(N["Incidents"][index][4]);
    //                zIncident.index = index;
    //                zIncident.developed = true;
    //            }

    //        }
    //        else
    //        {
    //            //good response so dont branch
    //            zIncident.resolved = true;
    //            zIncident.positiveResolution = true;
    //            zIncident.turnToShow += zIncident.turnsToAdd;
    //        }
    //    }
    //}
    public string GetIncidentFromType(string Type)
    {
        string description = "";

        description = Localization.GetRandomStringForType(Type);
        
        return description;
    }
}
