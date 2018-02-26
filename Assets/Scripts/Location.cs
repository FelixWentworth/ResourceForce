using UnityEngine;
using System.Collections;

public class Location : MonoBehaviour
{
	// Sites that are supported, custom is a specific city. If list is changed, be sure to change call to SetSite in MarketingManager.cs
    public enum Site { Preston = 0, Belfast = 1, Nicosia = 2, Groningen = 3, Valencia = 4, Custom = 5 }
    public static Site m_site;

    public static string CurrentLocation {
	    get
	    {
		    if (m_site == Site.Custom)
		    {
			    return MarketingManager.Instance.AppMetadata.Location;
		    }
		    return m_site.ToString();
	    }
	}

    public static int NumIncidents { get; set; }

    // Use this for initialization
	void Awake() {
        var siteNum = PlayerPrefs.GetInt("site");
	    SetSite(siteNum);
	}

    public void SetSite(int site)
    {
        if (m_site == (Site)site)
        {
            // No change
            return;
        }
	    m_site = (Site)site;
        PlayerPrefs.SetInt("site", site);
    }
}
