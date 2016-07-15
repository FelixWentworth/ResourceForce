﻿using UnityEngine;
using System.Collections;

public class Location : MonoBehaviour {

    public enum Site { Preston=0, Belfast=1, Nicosia=2, Groningen=3, Valencia=4}
    public Site m_site;

    public string IncidentFilePath { get; private set; }

    public int numIncidents { get; private set; }

	// Use this for initialization
	void Awake () {
        int siteNum = PlayerPrefs.GetInt("site");
        SetFilePath((int)m_site);
    }
    public void SetFilePath(int siteIndex)
    {
        m_site = (Site)siteIndex;
        IncidentFilePath = "ScenarioInformation";
        IncidentFilePath += GetExtension();
        PlayerPrefs.SetInt("site", siteIndex);
        //player has set location so dont show the screen unless requested
        PlayerPrefs.SetInt("SetLocation", 1);
    }
    public string GetExtension()
    {
        switch (m_site)
        {
            case Site.Preston:
                numIncidents = 12;
                return "_Preston";
            case Site.Belfast:
                numIncidents = 2;
                return "_Belfast";
            case Site.Nicosia:
                numIncidents = 3;
                return "_Nicosia";
            case Site.Groningen:
                numIncidents = 2;
                return "_Groningen";
            case Site.Valencia:
                numIncidents = 7;
                return "_Valencia";
            default:
                return "_Preston";
        }
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
