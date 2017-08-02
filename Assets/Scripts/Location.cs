using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Persistence;

public class Location : MonoBehaviour
{
    public enum Site { Preston = 0, Belfast = 1, Nicosia = 2, Groningen = 3, Valencia = 4 }
    public static Site m_site;

    public static string CurrentLocation { get { return m_site.ToString(); } }

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
