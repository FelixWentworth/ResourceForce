using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {

    Incident currentIncident;
    
    public Text Body;
    public Text LeftButton;
    public Text RightButton;
    public Text LandingText;

    public GameObject EmailPanel;

    private IncidentManager m_incidentManager;
    public OfficerController m_officerController;
    public GameObject m_citizenHelpButton;
    public GameObject SendOfficerButton;
    public GameObject waitButton;
    public enum PopupType { Incident, Citizen, CaseClosed };
    public PopupType popupType = PopupType.Incident;

    private int caseNum;

    private bool citizenSuccess = false;

    void Start()
    {
        m_incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
    }
    public void DeactivateAll()
    {
        m_citizenHelpButton.gameObject.SetActive(false);
        SendOfficerButton.gameObject.SetActive(false);
        waitButton.SetActive(false);
        Body.text = "";
    }
    public void Show(string zName, string zArea, int zOfficers, int caseNumber, bool developed, int turnsToSolve, bool showCitizen = false)
    {
        //call this method so we can activate coroutines from incident class
        StartCoroutine(ShowBox(zName, zArea, zOfficers, caseNumber, developed, turnsToSolve, showCitizen));
    }
    public void Show(int zCaseNumber, bool zPositive)
    {
        StartCoroutine(ShowCaseClosedBox(zCaseNumber, zPositive));
    }
    public void Show(int zCaseNumber)
    {
        StartCoroutine(ShowCitizenHelp(zCaseNumber));
    }
    public IEnumerator ShowBox(string zName, string zArea, int zOfficers, int caseNumber, bool developed, int turnsToSolve, bool showCitizen = false)
    {
        popupType = PopupType.Incident;
        Body.text = "";
        if (developed)
            Body.text += Localization.Get("BASIC_TEXT_DEVELOPED") + "\n";
        Body.text += string.Format(zName, zArea);
        LeftButton.text = Localization.Get("BASIC_TEXT_WAIT");
        SendOfficerButton.gameObject.SetActive(m_officerController.m_officers.Count >= zOfficers);

        if (zOfficers == 1)
        {
            RightButton.text = string.Format(Localization.Get("BASIC_TEXT_SEND_ONE"), turnsToSolve);
        }
        else
        {
            RightButton.text = string.Format(Localization.Get("BASIC_TEXT_SEND_MANY"), zOfficers, turnsToSolve);
        }

        yield return EmailAnim(-1f);

        waitButton.SetActive(true);
        m_citizenHelpButton.SetActive(showCitizen);
    }
    public IEnumerator ShowCaseClosedBox(int zCaseNumber, bool positive = false)
    {
        popupType = PopupType.CaseClosed;
        Body.text = positive ? Localization.Get("BASIC_TEXT_ARREST_SUCCESS") : Localization.Get("BASIC_TEXT_ARREST_FAIL");
        SendOfficerButton.gameObject.SetActive(false);
        LeftButton.text = Localization.Get("BASIC_TEXT_OK");
        caseNum = zCaseNumber;

        yield return EmailAnim(-1f);

        waitButton.SetActive(true);
        m_citizenHelpButton.SetActive(false);
    }
    public IEnumerator ShowCitizenHelp(int zCaseNumber)
    {
        popupType = PopupType.Citizen;
        //now calculate if this was a success
        int rand = UnityEngine.Random.Range(1, 101);
        bool success = rand > 65;
        Body.text = success ? Localization.Get("BASIC_TEXT_CITIZEN_SUCCESS"): Localization.Get("BASIC_TEXT_CITIZEN_FAIL");
        SendOfficerButton.gameObject.SetActive(!success);
        citizenSuccess = success;
        if (citizenSuccess)
            currentIncident.positiveResolution = true;
        LeftButton.text = success ? Localization.Get("BASIC_TEXT_OK") : Localization.Get("BASIC_TEXT_WAIT");
        caseNum = zCaseNumber;

        yield return EmailAnim(-1f);

        waitButton.SetActive(true);
        m_citizenHelpButton.SetActive(false);
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
    public void LeftButtonPressed()
    {
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
                    citizenSuccess = false;
                }
                break;
        }
    }
    public void RightButtonPressed()
    {
        StartCoroutine(RightButtonWithAnim());        
    }
    IEnumerator RightButtonWithAnim()
    {
        //send officers to resolve issue
        yield return EmailAnim(1f);
        m_incidentManager.ResolvePressed();
    }
    public void CitizenButtonPressed()
    {
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
    }
    public void NoMoreIncidents()
    {
        LandingText.text = Localization.Get("BASIC_TEXT_NO_MORE_INCIDENTS");
        
    }
    public void NewTurn(int turnNum)
    {
        LandingText.text = Localization.Get("BASIC_TEXT_TURN") + ":" + turnNum;
    }
}
