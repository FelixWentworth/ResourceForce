using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SimplifiedJson : MonoBehaviour {

    string filePath = "IncidentBasicInfo";
    TextAsset myText;
    public static int identifier = 1;
    // Use this for initialization
    public void CreateNewIncident(ref Incident zIncident) {
        myText = Resources.Load(filePath) as TextAsset;
        var N = JSON.Parse(myText.text);

        //get a random incident
        int randIncident = UnityEngine.Random.Range(0, int.Parse(N["TotalIncidentTypes"]));
        int randArea = UnityEngine.Random.Range(0, int.Parse(N["TotalAreaTypes"]));

        zIncident.index = randIncident;
        zIncident.officer = int.Parse(N["Incidents"][randIncident][1]);
        zIncident.area = N["Areas"][randArea];
        zIncident.incidentName = GetIncidentFromType(N["Incidents"][randIncident][0]);
        zIncident.turnsToAdd = int.Parse(N["Incidents"][randIncident][3]);
        zIncident.resolved = false;
        zIncident.caseNumber = identifier;
        zIncident.severity = int.Parse(N["Incidents"][randIncident][4]);
        identifier++;
    }
    public void DevelopIncident(ref Incident zIncident, bool waiting)
    {
        //we must now see if the incident can be developed
        if (myText == null)
            myText = Resources.Load(filePath) as TextAsset;
        var N = JSON.Parse(myText.text);

        //check the index of the developed index
        int index = int.Parse(N["Incidents"][zIncident.index][2]);

        if (index == -1)
        {
            TurnManager turn = GameObject.Find("TurnManager").GetComponent<TurnManager>();
            if (turn.turn >= zIncident.turnToDevelop)
            {
                //the incident that does not develop will expire, for now we will resolve the case, TODO make this a unsuccessful resolution
                zIncident.resolved = true;
                zIncident.positiveResolution = false;
            }
            else if (waiting)
            {
                //case does not resolve if you do nothing and has no developments, instead we will delay the showing of this message
                zIncident.turnToShow += 1;
            }
            else
            {
                //we must resolve the issue
                zIncident.resolved = true;
                zIncident.positiveResolution = true;
                zIncident.turnToShow += zIncident.turnsToAdd;
            }
        }
        else
        {
            if (waiting)
            {
                //check to see if the current incident has been ignored enough to develop into a new incident
                TurnManager turn = GameObject.Find("TurnManager").GetComponent<TurnManager>();
                if (zIncident.turnToDevelop >= turn.turn-1)
                {
                    zIncident.turnToShow += 1;
                }
                else
                {
                    //bad response so develop the situation
                    zIncident.nameBeforeDeveloped = zIncident.incidentName;
                    zIncident.incidentName = GetIncidentFromType(N["Incidents"][index][0]);
                    zIncident.officer = int.Parse(N["Incidents"][index][1]);
                    zIncident.turnToShow += 1;
                    zIncident.turnsToAdd = int.Parse(N["Incidents"][index][3]);
                    zIncident.turnToDevelop = turn.turn + 3;
                    zIncident.severity = int.Parse(N["Incidents"][index][4]);
                    zIncident.index = index;
                    zIncident.developed = true;
                }

            }
            else
            {
                //good response so dont branch
                zIncident.resolved = true;
                zIncident.positiveResolution = true;
                zIncident.turnToShow += zIncident.turnsToAdd;
            }
        }
    }
    public string GetIncidentFromType(string Type)
    {
        string description = "";

        description = Localization.GetRandomStringForType(Type);
        
        return description;
    }
}
