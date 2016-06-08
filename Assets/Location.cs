using UnityEngine;
using System.Collections;

public class Location : MonoBehaviour {

    public enum Site { Preston, Belfast, Nicosia, Groningen, Valencia}
    public Site m_site;

    public string IncidentFilePath { get; private set; }

    public int numIncidents { get; private set; }

	// Use this for initialization
	void Awake () {
        IncidentFilePath = "ScenarioInformation";
        IncidentFilePath += GetExtension();
    }
    public string GetExtension()
    {
        switch (m_site)
        {
            case Site.Preston:
                numIncidents = 2;
                return "_Preston";
            case Site.Belfast:
                numIncidents = 2;
                return "_Belfast";
            case Site.Nicosia:
                numIncidents = 2;
                return "_Nicosia";
            case Site.Groningen:
                numIncidents = 2;
                return "_Groningen";
            case Site.Valencia:
                numIncidents = 2;
                return "_Valencia";
            default:
                return "_Preston";
        }
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
