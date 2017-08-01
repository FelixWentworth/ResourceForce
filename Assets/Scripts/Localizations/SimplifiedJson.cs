﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

public class SimplifiedJson : MonoBehaviour {

    public Location m_location;

    private TextAsset _myText;
    private TurnManager _manager;

    public static int Identifier = 1;

    void Awake()
    {
        _manager = this.GetComponent<TurnManager>();
    }

    public Scenario CreateNewScenario(string location, string language, List<Incident> activeScenarios = null )
    {
        var scenarios = GameObject.Find("ContentManager").GetComponent<ContentRequest>().GetScenarios(location, language);

        var randScenario = Random.Range(0, scenarios.Count);

        if (activeScenarios != null)
        {
            var activeScenarioIds = activeScenarios.Select(s => s.Scenario.Id).ToList();

            while (activeScenarioIds.Contains(scenarios[randScenario].Id) && activeScenarioIds.Count < scenarios.Count)
            {
                randScenario = Random.Range(0, scenarios.Count);
            }
        }

        var scenario = scenarios[randScenario];

        return scenario;
    }

    public void WaitPressed(ref Incident incident)
    {
        GoToChoice(ref incident, "Wait");
    }
    public void OfficerPressed(ref Incident incident)
    {
        GoToChoice(ref incident, "Officer", incident.IncidentContent.OfficerReq);
    }
    public void CitizenPressed(ref Incident incident)
    {
        GoToChoice(ref incident, "Citizen");
    }

    private void GoToChoice(ref Incident incident, string choice, int turnsToAdd = 1)
    {

        var waitContent = incident.GetChoiceContent(choice);
        if (waitContent == null)
        {
            Debug.Log(choice + " option not available");
            return;
        }

        var turnToDevelop = incident.TurnToDevelop;

        incident.IncidentContent = waitContent;

        incident.TurnToShow = _manager.turn + turnsToAdd;
        incident.TurnToDevelop = turnToDevelop;
    }
}
