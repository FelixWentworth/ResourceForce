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
        zIncident.incidentName = N["Incidents"][randIncident][0];
        zIncident.area = N["Areas"][randArea];
        zIncident.turnsToAdd = int.Parse(N["Incidents"][randIncident][3]);
        zIncident.resolved = false;
        zIncident.caseNumber = identifier;
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
            if (waiting)
            {
                //case does not resolve if you do nothing and has no developments, instead we will delay the showing of this message
                zIncident.turnToShow += 5;
            }
            else
            {
                //we must resolve the issue
                zIncident.resolved = true;
                this.GetComponent<TurnManager>().caseResolved = "Case " + zIncident.caseNumber + " Closed";
            }
        }
        else
        {
            if (waiting)
            {
                //bad response so develop the situation
                zIncident.nameBeforeDeveloped = zIncident.incidentName;
                zIncident.incidentName = N["Incidents"][index][0];
                zIncident.officer = int.Parse(N["Incidents"][index][1]);
                zIncident.turnToShow += zIncident.turnsToAdd;
                zIncident.turnsToAdd = int.Parse(N["Incidents"][index][3]);
                zIncident.index = index;
                zIncident.developed = true;
            }
            else
            {
                //good response so dont branch
                zIncident.resolved = true;
                this.GetComponent<TurnManager>().caseResolved = "Case " + zIncident.caseNumber + " Closed";
            }
        }
    }
}
