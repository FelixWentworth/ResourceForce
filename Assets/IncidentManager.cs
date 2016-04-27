﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IncidentManager : MonoBehaviour {

    public List<Incident> incidents = new List<Incident>();
    private SimplifiedJson jsonReader;

    Incident NextIncident;
	public void CreateNewIncident(int zTurn)
    {
        Incident newIncident = new Incident();
        
        //now get a random incident data from JSON file
        if (jsonReader == null)
            jsonReader = this.GetComponent<SimplifiedJson>();

        jsonReader.CreateNewIncident(ref newIncident);

        incidents.Add(newIncident);
        NextIncident = newIncident;
    }
    public bool IsIncidentWaitingToShow(int zTurn)
    {
        for (int i=0; i < incidents.Count; i++)
        {
            if (incidents[i].turnToShow <= zTurn)
            {
                //this incident wants to be shown on this turn
                NextIncident = incidents[i];
                return true;
            }
        }
        //no elements in our incident list need to be shown
        return false;
    }
    public void UpdateIncidents()
    {
        for (int i = 0; i < incidents.Count; i++)
        {
            //an incident has been resolved so remove it from our list
            if(incidents[i].resolved)
            {
                incidents.RemoveAt(i);
                i--;
            }
        }
    }
    public void ShowIncident(int turn)
    {
        if (NextIncident == null)
            CreateNewIncident(turn);
        NextIncident.Show(ref NextIncident);

        //make the incident null to make sure we dont show it again until its due
        //NextIncident = null;
    }
    public void ClearList()
    {
        incidents.Clear();
    }
    public void WaitPressed()
    {
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref NextIncident, true);
        NextIncident = null;
        this.gameObject.GetComponent<TurnManager>().NextTurn();
    }
    public void ResolvePressed()
    {
        GameObject.Find("TurnManager").GetComponent<SimplifiedJson>().DevelopIncident(ref NextIncident, false);
        GameObject.Find("OfficerManager").GetComponent<OfficerController>().RemoveOfficer(NextIncident.officer);
        NextIncident = null;
        this.gameObject.GetComponent<TurnManager>().NextTurn();
    }
}

public class Incident {
    public int index;
    public int officer;
    public string area;
    public string incidentName;
    public int turnToShow;
    public bool resolved;
    public int turnsToAdd;

    public int caseNumber;
    public bool developed;

    private TurnManager m_turnManager;

    public void Show(ref Incident zIncident)
    {
        //use the dialog box to show the current incident
        GameObject.Find("IncidentDialog").GetComponent<DialogBox>().ShowBox(incidentName, area, officer, caseNumber, developed);
    }
}
