using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

public class IncidentFromJson : MonoBehaviour {
	void Start()
	{
		GetNewIncident ();
	}
	void GetNewIncident(int zSeverity = 1)
	{
		string filePath = "Incidents_Severity_" + zSeverity;
		TextAsset myText = Resources.Load (filePath) as TextAsset;
		var N = JSON.Parse (myText.text);

		//get a ranodm incident from our file
		int randomIncident = UnityEngine.Random.Range(0, int.Parse(N["NumberOfIncidents"])) + 1; 
		string incidentNumber = "Incident"+randomIncident;

		//get the info from the file
		int textArrayLength = int.Parse(N[incidentNumber]["NumberOfTexts"]);

		//now we have the length of the text array populate it
		string[] textArray = new string[textArrayLength];
		for (int i = 0; i < textArrayLength; i++) {
			textArray [i] = N [incidentNumber] ["IncidentText"] [i];
		}
		int turns = int.Parse (N [incidentNumber] ["Turns"]);
		int officers = int.Parse (N [incidentNumber] ["Officers"]);

        //now we have our data, send it to our object for initialisation of scenario
        GameManager.NumberOfIncident++;
        SendData("Incident " + GameManager.NumberOfIncident,textArray, turns, officers, zSeverity);
	}
	private void SendData(string zEntryName, string[] zTextArray, int zTurns, int zOfficers, int zSeverity)
	{
        this.gameObject.SendMessage("SetUpIncidentPlayerPrefName", zEntryName);
		this.gameObject.SendMessage ("SetUpIncidentText", zTextArray);
		this.gameObject.SendMessage ("SetUpIncidentTurns", zTurns);
		this.gameObject.SendMessage ("SetUpIncidentOfficers", zOfficers);
		this.gameObject.SendMessage ("SetUpIncidentSeverity", zSeverity);
	}
}