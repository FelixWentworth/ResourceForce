using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CitizenHelpPopup : MonoBehaviour {

    public IncidentManager incidentManager;
    bool success = false;
    int failChance = 65;
    public Text HelpOutcome;

    public GameObject tick, cross;
	// Use this for initialization
	void OnEnable () {
        //determine whether the ask for help was a success
        DetermineSuccess();
        //mow using the result set the text
        HelpOutcome.text = string.Format("Citizen {0} the investigation", (success ? "provides video footage that helps" : "withdraws statement from"));
        tick.SetActive(success);
        cross.SetActive(!success);
	}
	void DetermineSuccess()
    {
        int rand = UnityEngine.Random.Range(1, 101);
        success = rand > failChance;
    }
	

    public void OKPressed()
    {
        if (success)
        {
            //resolve the incidents
            incidentManager.ResolvePressed();
        }
        else
        {
            //develop the incident, this counts as a ignore step
            incidentManager.WaitPressed();
        }
        this.gameObject.SetActive(false);
    }
}
