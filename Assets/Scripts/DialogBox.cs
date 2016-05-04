using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {

    Incident currentIncident;
    
    public Text Body;
    public Text LeftButton;
    public Text RightButton;

    public Button SendOfficerButton;

    public GameObject dialog;

    private IncidentManager m_incidentManager;
    public OfficerController m_officerController;
    public GameObject m_citizenHelpButton;

    public enum PopupType { Incident, Citizen, CaseClosed };
    public PopupType popupType = PopupType.Incident;

    private int caseNum;

    private bool citizenSuccess = false;

    void Start()
    {
        m_incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
    }
    public void ShowBox(string zName, string zArea, int zOfficers, int caseNumber, bool developed, bool showCitizen = false)
    {
        popupType = PopupType.Incident;
        Body.text = "";
        if (developed)
            Body.text += "DEVELOPED CASE\n";
        Body.text += string.Format("{0} in {1} area", zName, zArea);
        LeftButton.text = "Wait for more officers to become available";
        SendOfficerButton.gameObject.SetActive(m_officerController.m_officers.Count >= zOfficers);
 
        RightButton.text = string.Format("Send {0} officer{1}", zOfficers, zOfficers > 1 ? "s" : "");

        m_citizenHelpButton.SetActive(showCitizen);

        dialog.SetActive(true);
    }
    public void ShowCaseClosedBox(int zCaseNumber)
    {
        popupType = PopupType.CaseClosed;
        Body.text = "Arrests have been made";
        SendOfficerButton.gameObject.SetActive(false);
        LeftButton.text = "OK";
        caseNum = zCaseNumber;
    }
    public void ShowCitizenHelp(int zCaseNumber)
    {
        popupType = PopupType.Citizen;
        //now calculate if this was a success
        int rand = UnityEngine.Random.Range(1, 101);
        bool success = rand > 65;
        Body.text = success ? "Citizens Provide Evidence through the INSPEC2T app, 2 have been charged" : "Citizen fails to provide any evidence for the case";
        SendOfficerButton.gameObject.SetActive(!success);
        citizenSuccess = success;

        LeftButton.text = success ? "OK" : "Wait for more officers to become available";
        caseNum = zCaseNumber;
        m_citizenHelpButton.SetActive(false);
    }
    public void LeftButtonPressed()
    {
        //wait for more officers to become available
        switch(popupType)
        {
            case PopupType.CaseClosed:
                m_incidentManager.CloseCase(caseNum);
                break;
            case PopupType.Incident:
                m_incidentManager.WaitPressed();
                break;
            case PopupType.Citizen:
                if (citizenSuccess)
                {
                    //has the issue been resolved
                    m_incidentManager.CloseCase(caseNum);
                    citizenSuccess = false;
                }
                else
                {
                    //issue was not resolved now the player has chosen to wait
                    m_incidentManager.WaitPressed();
                }
                break;
        }
    }
    public void RightButtonPressed()
    {
        //send officers to resolve issue
        m_incidentManager.ResolvePressed();
    }
    public void CitizenButtonPressed()
    {
        //removing citizen help popup and instead setting the delay to one turn
        m_incidentManager.CitizenHelpPressed();
    }
    public void ClearDialogBox()
    {
        Body.text = "";
    }
    public void NoMoreIncidents()
    {
        Body.text = "No More Incidents to check this turn";
    }
}
