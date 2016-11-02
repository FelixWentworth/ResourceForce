public class Incident
{
    public int scenarioNum;
    public int index;
    public int officer;
    public string incidentName;
    public string type;
    public int turnToShow;
    public int turnsToAdd;
    public int severity;
    public int caseNumber;
    public int waitIndex;
    public int officerIndex;
    public int citizenIndex;
    public int satisfactionImpact;

    public int feedbackRatingWait = 1;
    public string feedbackWait;

    public int feedbackRatingCitizen = 5;
    public string feedbackCitizen;

    public int feedbackRatingOfficer = 4;
    public string feedbackOfficer;

    //values which are not set during setup
    public int turnToDevelop;
    public string nameBeforeDeveloped;
    public bool developed;
    public bool resolved;
    public bool positiveResolution = false;
    public bool isNew = true;

    private TurnManager m_turnManager;
    private DialogBox m_dialogBox;

    public void Show(ref Incident zIncident, DialogBox dialogBox)
    {
        //use the dialog box to show the current incident
        dialogBox.Show(zIncident);
    }
}