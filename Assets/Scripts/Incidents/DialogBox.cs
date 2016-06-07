﻿using UnityEngine;
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

    void Start()
    {
        m_incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
    }
    public void Show(Incident zIncident)
    {
        //pass through the relevant info to the dialog box
        StartCoroutine(ShowIncident(zIncident));
    }
    public IEnumerator ShowIncident(Incident zIncident)
    {
        //check if this is a resolution, ie. no buttons will lead anywhere
        bool endCase = (zIncident.waitIndex == -1 && zIncident.officerIndex == -1 && zIncident.citizenIndex == -1);

        Body.text = "<color=#00F3FFFF>" + Localization.Get("BASIC_TEXT_TYPE") + ": </color>" + zIncident.type + "\n\n<color=#00F3FFFF>" + Localization.Get("BASIC_TEXT_DESCRIPTION") + ": </color>" + Localization.Get(zIncident.incidentName);
        if (m_citizenHelpButton.activeSelf)
            Body.text += "\n\n" + Localization.Get("CITIZEN_HELP_TEXT");

        caseNum = zIncident.caseNumber;
        EmailNumber.text = caseNum.ToString();
        SetSeverity(zIncident.severity);

        if (!endCase)
        {
            OfficerReqText.text = Localization.Get("BASIC_TEXT_OFFICERS_REQUIRED") + ": " + zIncident.officer + "\n" + Localization.Get("BASIC_TEXT_TURNS_REQUIRED") + ": " + zIncident.turnsToAdd;
            popupType = PopupType.Incident;
        }
        else
        {
            OfficerReqText.text = "";
            popupType = PopupType.CaseClosed;
        }
        //set the button text
        LeftButton.text = endCase ? Localization.Get("BASIC_TEXT_OK") : Localization.Get("BASIC_TEXT_WAIT");
        RightButton.text = zIncident.officer == 1 ? Localization.Get("BASIC_TEXT_SEND_ONE") : RightButton.text = Localization.Get("BASIC_TEXT_SEND_MANY");

        //wait for anim to finish
        yield return EmailAnim(-1f, "EmailShow");

        //now set which buttons should be active
        waitButton.SetActive(zIncident.waitIndex != -1 || endCase);
        SendOfficerButton.SetActive(zIncident.officerIndex != -1);
        m_citizenHelpButton.SetActive(zIncident.citizenIndex != -1);
    }
    public IEnumerator EmailAnim(float speed, string name)
    {
        //play the anim at the speed specified
        Animation anim = EmailPanel.GetComponent<Animation>();

        //set up the speed ant time to make sure the aim plays in the correct direction
        anim[name].speed = speed;
        float length = anim[name].length;
        anim[name].time = speed == -1f ? length : 0f;

        anim.Play();
        yield return new WaitForSeconds(length);
    }
    public void SetSeverity(int zSeverity=1)
    {
        //set the alpha of the overlay to fade between yellow and red
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
        yield return EmailAnim(1f, "EmailShow");
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
        yield return EmailAnim(1f, "EmailShow");
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
        yield return EmailAnim(1f, "EmailShow");
        m_incidentManager.CitizenHelpPressed();
    }
    //methods for clearing data
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
    public void DeactivateAll()
    {
        DisableButtons();
        ClearDialogBox();
    }
}