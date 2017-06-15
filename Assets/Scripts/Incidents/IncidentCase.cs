using UnityEngine;
using UnityEngine.UI;

public class IncidentCase : MonoBehaviour {

    public enum State { New, Waiting, OfficersSent, CitizenRequest, Resolved, Escalated, InProgress };
    public State m_state = State.New;

    public Image CaseImage;
    public Image ActiveCaseImage;

    public GameObject warningIcon;

    public GameObject highlightObject;

    private IncidentManager _incidentManager;

    [HideInInspector]public string caseNumber;
    [HideInInspector]public int severityNumber;

    public void Setup(string id, State zState = State.New, int zSeverity = 1)
    {
        if (_incidentManager == null)
        {
            _incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
        }
        //set up the case to show the relevant info
        caseNumber = id;
        m_state = zState;
        severityNumber = zSeverity;

        CaseImage.color = m_state == State.New
            ? _incidentManager.GetSeverityColor(0)
            : _incidentManager.GetSeverityColor(severityNumber); 

        highlightObject.SetActive(false);
    }

    public void ToggleHighlight(bool highlighted = false)
    {
        //by default the highligh should be disabled
        highlightObject.SetActive(!highlighted);
        if (!highlighted)
        {
            CaseImage.color = _incidentManager.GetSeverityColor(severityNumber);
            ActiveCaseImage.color = _incidentManager.GetSeverityColor(severityNumber);
        }
    }
    public void UpdateWarning(bool activated)
    {
        warningIcon.SetActive(activated);
    }

    public void Pressed()
    {
        //can be called through incident button if we want to have selectable cases
#if SELECT_INCIDENTS
        GameObject.Find("TurnManager").GetComponent<IncidentManager>().ShowIncidentWithCaseNumber(caseNumber);
#endif
    }
}
