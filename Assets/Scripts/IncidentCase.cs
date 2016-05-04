using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IncidentCase : MonoBehaviour {

    public enum State { New, Waiting, OfficersSent, CitizenRequest, Resolved, Escalated, InProgress };
    public State m_state = State.New;

    public Image icon;
    public Text number;
    public Image severity;

    public GameObject warningIcon;

    public GameObject highlightObject;

    [HideInInspector]public int caseNumber;
    [HideInInspector]public int severityNumber;

    public void Setup(int zNumber, State zState = State.New, int zSeverity = 1)
    {
        number.text = zNumber.ToString();
        caseNumber = zNumber;
        m_state = zState;

        SetSeverity(zSeverity);
        SetIcon();
        highlightObject.SetActive(false);
    }
    public void SetSeverity(int zSeverity = 1)
    {
        float alpha = 0f;
        if (zSeverity == 2)
            alpha = 0.5f;
        else if (zSeverity == 3)
            alpha = 1.0f;
        severity.color = new Color(1f, 0f, 0f, alpha);
        severityNumber = zSeverity;
    }
	public void SetIcon()
    {
        switch (m_state)
        {
            case State.New:
                icon.sprite = Resources.Load<Sprite>("Sprites/New");
                break;
            case State.Waiting:
                icon.sprite = Resources.Load<Sprite>("Sprites/Wait");
                break;
            case State.OfficersSent:
                icon.sprite = Resources.Load<Sprite>("Sprites/Officer");
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
        if (forceDefault)
        {
            //by default the highlight should be disabled
            highlightObject.SetActive(false);
        }
        else
        {
            highlightObject.SetActive(true);
            UpdateWarning(false);
        }
    }
    public void UpdateWarning(bool activated)
    {
        warningIcon.SetActive(activated);
    }
}
