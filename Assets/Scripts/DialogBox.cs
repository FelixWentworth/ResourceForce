using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {

    [HideInInspector]
    public Incident currentIncident;
    
    public Text Body;
    public Text LeftButton;
    public Text RightButton;
    public Text OfficerReqText;

    public Text EmailNumber;
    public Image emailIconSeverityOverlay;

    public GameObject EmailPanel;

    private IncidentManager m_incidentManager;
    public OfficerController m_officerController;
    public GameObject m_citizenHelpButton;
    public GameObject SendOfficerButton;
    public GameObject waitButton;
    public enum PopupType { Incident, CaseClosed };
    public PopupType popupType = PopupType.Incident;

    private int caseNum;

    private bool citizenSuccess = false;

    void Start()
    {
        m_incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
    }
    public void DeactivateAll()
    {
        DisableButtons();
        Body.text = "";
        OfficerReqText.text = "";
    }
    public void Show(Incident zIncident)
    {
        //pass through the relevant info to the dialog box
        StartCoroutine(ShowIncident(zIncident.incidentName, zIncident.caseNumber, zIncident.officer, zIncident.turnsToAdd, zIncident.severity, zIncident.waitIndex, zIncident.officerIndex, zIncident.citizenIndex));
    }
    public IEnumerator ShowIncident(string zDescription, int zCaseNumber, int zOfficers, int zTurnsToSolve, int zSeverity, int zWaitIndex, int zOfficerIndex, int zCitizenIndex)
    {
        bool endCase = (zWaitIndex == -1 && zOfficerIndex == -1 && zCitizenIndex == -1);
        Body.text = Localization.Get(zDescription);
        caseNum = zCaseNumber;
        if (!endCase)
        {
            OfficerReqText.text = Localization.Get("BASIC_TEXT_OFFICERS_REQUIRED") + ": " + zOfficers + "\n" + Localization.Get("BASIC_TEXT_TURNS_REQUIRED") + ": " + zTurnsToSolve;
            popupType = PopupType.Incident;
        }
        else
        {
            OfficerReqText.text = "";
            popupType = PopupType.CaseClosed;
        }
        LeftButton.text = endCase ? Localization.Get("BASIC_TEXT_OK") : Localization.Get("BASIC_TEXT_WAIT");
        
        if (zOfficers == 1)
        {
            RightButton.text = Localization.Get("BASIC_TEXT_SEND_ONE");
        }
        else
        {
            RightButton.text = Localization.Get("BASIC_TEXT_SEND_MANY");
        }
        EmailNumber.text = zCaseNumber.ToString();
        SetSeverity(zSeverity);

        //wait for anim to finish
        yield return EmailAnim(-1f);

        //now set which buttons should be active
        waitButton.SetActive(zWaitIndex != -1 || endCase);
        SendOfficerButton.SetActive(zOfficerIndex != -1);
        m_citizenHelpButton.SetActive(zCitizenIndex != -1);
        if (m_citizenHelpButton.activeSelf)
        {
            Body.text += "\n\n" + Localization.Get("CITIZEN_HELP_TEXT");
        }
    }
    public IEnumerator EmailAnim(float speed)
    {
        Animation anim = EmailPanel.GetComponent<Animation>();
        anim["EmailShow"].speed = speed;
        float length = anim["EmailShow"].length;
        if (speed == -1f)
            anim["EmailShow"].time = length;
        else
            anim["EmailShow"].time = 0f;
        anim.Play();
        yield return new WaitForSeconds(length);
    }
    public void SetSeverity(int zSeverity=1)
    {
        float alpha = 0f;
        if (zSeverity == 2)
            alpha = 0.5f;
        else if (zSeverity == 3)
            alpha = 1.0f;
        emailIconSeverityOverlay.color = new Color(1f, 0f, 0f, alpha);
    }
    public void LeftButtonPressed()
    {
        DisableButtons();
        StartCoroutine(LeftButtonWithAnim());
    }
    IEnumerator LeftButtonWithAnim()
    {
        yield return EmailAnim(1f);
        //wait for more officers to become available
        switch (popupType)
        {
            case PopupType.CaseClosed:
                m_incidentManager.CloseCase(caseNum);
                break;
            case PopupType.Incident:
                m_incidentManager.WaitPressed();
                break;
        }
    }
    public void RightButtonPressed()
    {
        if (m_officerController.m_officers.Count >= currentIncident.officer)
        {
            //double check if we have enough officers otherwise the game will break
            DisableButtons();
            StartCoroutine(RightButtonWithAnim());
        }
    }
    IEnumerator RightButtonWithAnim()
    {
        //send officers to resolve issue
        yield return EmailAnim(1f);
        m_incidentManager.ResolvePressed();
    }
    public void CitizenButtonPressed()
    {
        DisableButtons();
        StartCoroutine(CitizenButtonWithAnim());
    }
    IEnumerator CitizenButtonWithAnim()
    {
        //removing citizen help popup and instead setting the delay to one turn
        yield return EmailAnim(1f);
        m_incidentManager.CitizenHelpPressed();
    }
    public void ClearDialogBox()
    {
        Body.text = "";
        OfficerReqText.text = "";
    }
    public void DisableButtons()
    {
        waitButton.SetActive(false);
        SendOfficerButton.SetActive(false);
        m_citizenHelpButton.SetActive(false);
    }
}
