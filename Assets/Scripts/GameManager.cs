using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public int turns { get; private set; }
	public int officers { get; private set; }

    //HACK: static int to keep the incident number, this script will create ScenarioManager objects during the new turn phase and set its number then
    public static int NumberOfIncident = 0;

    private GameObject incidentCallback;

    private OfficerController m_OfficerController;
    void Start()
    {
        m_OfficerController = this.GetComponent<OfficerController>();
        officers = m_OfficerController.GetAvailable();
    }
	void Update()
	{
		if (Input.GetKeyUp (KeyCode.N))
			AddTurn (1);
	}
	public void AddTurn(int zTurns)
	{ 
		//turns will be added one at a time
		turns+=zTurns;
		//notify incidents a turn has passed
		gameObject.BroadcastMessage("TurnHasPassed", zTurns);
        m_OfficerController.EndTurn();

        //officer may be no longer be being used so update the number available
        officers = m_OfficerController.GetAvailable();
	}
	public void RemoveOfficers(int number)
	{
        if (officers < number)  //we dont have enough officers for the incident so can not send them
            return;
        //officer has been sent to deal with an incident and must now be removed from those available
        m_OfficerController.RemoveOfficer(number);
        officers = m_OfficerController.GetAvailable();
        incidentCallback.SendMessage("AddOfficerSuccess", number);
	}
    public void SendIncidentObjectForCallback(GameObject obj)
    {
        incidentCallback = obj;
    }
}
