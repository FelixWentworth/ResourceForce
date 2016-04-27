using UnityEngine;
using System.Collections;

public class DebugGUIScreen : MonoBehaviour {
	public GameObject[] Incidents;
	public GameManager m_GameManager;
	void Start()
	{
		PlayerPrefs.DeleteAll();
	}
	void OnGUI()
	{
		GUI.Label (new Rect (10, 400, 250, 100), "Turns: " +m_GameManager.turns.ToString ());
		GUI.Label(new Rect(410, 400, 250, 250), "Officers Available: "+m_GameManager.officers.ToString() + GetOfficerStatus());
		if (GUI.Button (new Rect(10, 10, 150, 100), "Case 1")) {
			Incidents [0].SendMessage ("ShowIncident");
		}
		if (GUI.Button (new Rect(160, 10, 150, 100), "Case 2")) {
			Incidents [1].SendMessage ("ShowIncident");
		}
		if (GUI.Button (new Rect(310, 10, 150, 100), "Case 3")) {
			Incidents [2].SendMessage ("ShowIncident");
		}
		if (GUI.Button (new Rect(460, 10, 150, 100), "Case 4")) {
			Incidents [3].SendMessage ("ShowIncident");
		}
		if (GUI.Button (new Rect(610, 10, 150, 100), "Case 5")) {
			Incidents [4].SendMessage ("ShowIncident");
		}
	}
    string GetOfficerStatus()
    {
        string status = "";

        OfficerController tempOfficerController = this.GetComponent<OfficerController>();
        int count = 1;
        for(int i = 0; i < tempOfficerController.m_officers.Count; i++, count++)
        {
            status += "\n Officer " + count + " Available"; 
        }
        for (int i = 0; i< tempOfficerController.m_officersInUse.Count; i++, count++)
        {
            status += "\n Officer " + count + " In Use";
        }

        return status;
    }
}
