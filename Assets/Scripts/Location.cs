using UnityEngine;
using System.Collections;

public class Location : MonoBehaviour {

    public enum Site { Preston = 0, Belfast = 1, Nicosia = 2, Groningen = 3, Valencia = 4 }
    public Site m_site;

    public string IncidentFilePath { get; private set; }

    public int numIncidents { get; private set; }

    public static string CurrentLocation { get; private set; }

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
                CurrentLocation = "Preston";
                return "_Preston";
            case Site.Belfast:
                numIncidents = 2;
                CurrentLocation = "Belfast";
                return "_Belfast";
            case Site.Nicosia:
                numIncidents = 10;
                CurrentLocation = "Nicosia";
                return "_Nicosia";
            case Site.Groningen:
                numIncidents = 10;
                CurrentLocation = "Groningen";
                return "_Groningen";
            case Site.Valencia:
                numIncidents = 20;
                CurrentLocation = "Valencia";
                return "_Valencia";
            default:
                CurrentLocation = "Preston";
                return "_Preston";
        }
        
    }
}
