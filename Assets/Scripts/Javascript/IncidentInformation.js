#pragma strict
//this class can be set up for each incident and will outline the behaviour for this
//the body of text that will display when the incident is selected
public var playerPrefName : String;
public var incidentText : String[];
public var turnsToResolve : int;
public var officersRequired : int;
public var severity : int;

//the number of officers working on incident
private var officers : int;
private var manager : GameObject;
private var myName : String;

public function SetUpIncidentPlayerPrefName(zName : String)
{
    playerPrefName = zName;
    myName = zName;
}

public function SetUpIncidentText(zIncident : String[])
{
	incidentText = zIncident;
}
public function SetUpIncidentTurns(zIncident : int)
{
	turnsToResolve = zIncident;
}
public function SetUpIncidentOfficers(zIncident : int)
{
	officersRequired = zIncident;
}
public function SetUpIncidentSeverity(zIncident : int)
{
	severity = zIncident;
}

public function SetName (zName : String)
{
	myName = zName;
}

public function TurnHasPassed(zTurns : int)
{
	//end of turn, update the information for this incident
	if (officers > 0)
	{
		//there is an officer working on the incident, lower the number of turns remainingq
		turnsToResolve-=zTurns;

		if(turnsToResolve<=0)
		{
			//release the officers involved in this issue
		    officers = 0;
		    //delete the information in the player prefs to enable this scenario to come back again
		    PlayerPrefs.DeleteKey(playerPrefName);

		}
	}
	else 
	{
		//increase the number of officers needed
		officersRequired++;
	}
	var id = PlayerPrefs.GetInt(myName + "DialogID");
	if (id == 0)
	{
		//player has ignored the incident so develop the incident
		PlayerPrefs.SetInt(myName + "DialogID", 4);
	}
}
public function AddOfficer(num : int)
{
	if (manager == null)
	    manager = GameObject.FindWithTag("GameController");
	manager.SendMessage("SendIncidentObjectForCallback", this.gameObject);
	manager.SendMessage("RemoveOfficers", num);
}
public function AddOfficerSuccess(num : int)
{
    officers += num;
}
