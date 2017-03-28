﻿using UnityEngine;
using System.Collections;

public class Location : MonoBehaviour {

    public enum Site { Preston = 0, Belfast = 1, Nicosia = 2, Groningen = 3, Valencia = 4 }
    public Site m_site;

    public string IncidentFilePath { get; private set; }

    public static int numIncidents { get; private set; }

    public static string CurrentLocation { get; private set; }

    public static bool UsesFullFeedback { get; private set; }

	// Use this for initialization
	void Awake() {
        int siteNum = PlayerPrefs.GetInt("site");
        SetFilePath(siteNum);
    }
    public void SetFilePath(int siteIndex)
    {
        m_site = (Site)siteIndex;
        IncidentFilePath = "ScenarioInformation";
        IncidentFilePath += GetExtension();
        PlayerPrefs.SetInt("site", siteIndex);
    }
    public string GetExtension()
    {
        switch (m_site)
        {
            case Site.Preston:
                numIncidents = 20;
                UsesFullFeedback = true;
                CurrentLocation = "Preston";
                return "_Preston";
            case Site.Belfast:
                numIncidents = 10;
                UsesFullFeedback = true;
                CurrentLocation = "Belfast";
                return "_Belfast";
            case Site.Nicosia:
                numIncidents = 10;
                UsesFullFeedback = true;
                CurrentLocation = "Nicosia";
                return "_Nicosia";
            case Site.Groningen:
                numIncidents = 10;
                UsesFullFeedback = true;
                CurrentLocation = "Groningen";
                return "_Groningen";
            case Site.Valencia:
                numIncidents = 20;
                UsesFullFeedback = true;
                CurrentLocation = "Valencia";
                return "_Valencia";
            default:
                numIncidents = 20;
                UsesFullFeedback = true;
                CurrentLocation = "Preston";
                return "_Preston";
        }
        
    }
}
