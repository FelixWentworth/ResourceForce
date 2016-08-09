//#define SELECT_INCIDENTS //used to allow players to select incidents

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IncidentCase : MonoBehaviour {

    public enum State { New, Waiting, OfficersSent, CitizenRequest, Resolved, Escalated, InProgress };
    public State m_state = State.New;

    public Image icon;
    public Text number;
    public Image severity;
    public Image highlightSeverity;

    public GameObject warningIcon;
    public GameObject newIncidentOverlay;

    public GameObject highlightObject;

    [HideInInspector]public int caseNumber;
    [HideInInspector]public int severityNumber;

    public void Setup(int zNumber, State zState = State.New, int zSeverity = 1, Image severityHighlight = null, bool isNew = false)
    {
        //set up the case to show the relevant info
        number.text = zNumber.ToString();
        caseNumber = zNumber;
        m_state = zState;

        SetSeverity(zSeverity);
        
        if (severityHighlight != null)
        {
            highlightSeverity.color = severityHighlight.color;
            if (isNew)
            {
                newIncidentOverlay.SetActive(true);
            }
        }
        
        SetIcon();
        highlightObject.SetActive(false);
    }
    public void SetSeverity(int zSeverity = 1)
    {
        //set the alpha of the severity overlay
        float alpha = 0f;
        if (zSeverity == 2)
            alpha = 0.5f;
        else if (zSeverity == 3)
            alpha = 1.0f;
        severity.color = new Color(1f, 0f, 0f, alpha);
        highlightSeverity.color = new Color(1f, 0f, 0f, alpha);
        severityNumber = zSeverity;
    }
	public void SetIcon()
    {
        //set the icon of each incident based on the desicion made
        switch (m_state)
        {
            case State.New:
                icon.sprite = Resources.Load<Sprite>("Sprites/New");
                break;
            case State.Waiting:
                icon.sprite = Resources.Load<Sprite>("Sprites/Wait");
                break;
            case State.OfficersSent:
                icon.sprite = Resources.Load<Sprite>("Sprites/Siren");
                break;
            case State.CitizenRequest:
                icon.sprite = Resources.Load<Sprite>("Sprites/Inspec2t");
                break;
            case State.Resolved:
                icon.sprite = Resources.Load<Sprite>("Sprites/Resolved");
                break;
            case State.Escalated:
                icon.sprite = Resources.Load<Sprite>("Sprites/Escalated");
                break;
            case State.InProgress:
                icon.sprite = Resources.Load<Sprite>("Sprites/InProgress");
                break;
        }
    }
    public void ToggleHighlight(bool forceDefault = false)
    {
        //by default the highligh should be disabled
        highlightObject.SetActive(!forceDefault);
        if (!forceDefault)
        {
            SetSeverity(severityNumber);
            DisableNewCase();
        }
    }
    public void UpdateWarning(bool activated)
    {
        warningIcon.SetActive(activated);
    }
    public void DisableNewCase()
    {

        newIncidentOverlay.SetActive(false);
    }

    public void Pressed(Text myText)
    {
        //can be called through incident button if we want to have selectable cases
#if SELECT_INCIDENTS
        GameObject.Find("TurnManager").GetComponent<IncidentManager>()._showIncident(myText);
#endif
    }
}
