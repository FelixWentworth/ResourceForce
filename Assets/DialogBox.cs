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
    public GameObject m_citizenHelpPopup;
    public GameObject m_citizenHelpButton;

    public enum PopupType { Incident, CaseClosed };
    public PopupType popupType = PopupType.Incident;

    private int caseNum;

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
        Body.text += string.Format("{0} Reported at {1} area", zName, zArea);
        LeftButton.text = "Wait for more officers to become available";
        SendOfficerButton.interactable = (m_officerController.m_officers.Count >= zOfficers);
        if (!SendOfficerButton.interactable)
        {
            SendOfficerButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            SendOfficerButton.GetComponent<Image>().color = Color.white;
        }
        RightButton.text = string.Format("Send {0} officer{1}", zOfficers, zOfficers > 1 ? "s" : "");

        m_citizenHelpButton.SetActive(showCitizen);

        dialog.SetActive(true);
    }
    public void ShowCaseClosedBox(int zCaseNumber)
    {
        popupType = PopupType.CaseClosed;
        Body.text = "Arrests have been made";
        SendOfficerButton.interactable = false;
        LeftButton.text = "OK";
        caseNum = zCaseNumber;
    }
    public void LeftButtonPressed()
    {
        //wait for more officers to become available
        if (popupType == PopupType.Incident)
            m_incidentManager.WaitPressed();
        if (popupType == PopupType.CaseClosed)
        {
            m_incidentManager.CloseCase(caseNum);
            
        }
    }
    public void RightButtonPressed()
    {
        //send officers to resolve issue
        m_incidentManager.ResolvePressed();
    }
    public void CitizenButtonPressed()
    {
        m_citizenHelpPopup.SetActive(true);
    }
}
