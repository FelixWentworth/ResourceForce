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

    void Start()
    {
        m_incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
    }
    public void ShowBox(string zName, string zArea, int zOfficers, int caseNumber, bool developed)
    {
        Body.text = "";
        if (developed)
            Body.text += "DEVELOPED CASE\n";
        Body.text += "Case " + caseNumber + "\n";
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



        dialog.SetActive(true);
    }
    public void LeftButtonPressed()
    {
        //wait for more officers to become available
        m_incidentManager.WaitPressed();
    }
    public void RightButtonPressed()
    {
        //send officers to resolve issue
        m_incidentManager.ResolvePressed();
    }
}
