using UnityEngine;
using System.Collections;
using System.Timers;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {

    [HideInInspector]
    public Incident currentIncident;
    
    //the ok and wait text objects are enabled based on which button to show as they have a different layout
    public GameObject OkText;
    public Text satisfaction;
    public GameObject WaitText;

    public Text RightButton;

    public GameObject EmailPanel;

    private IncidentManager m_incidentManager;
    public OfficerController m_officerController;
    public TurnManager m_turnManager;
    public GameObject m_citizenHelpButton;
    public GameObject SendOfficerButton;
    public GameObject waitButton;
    public enum PopupType { Incident, CaseClosed };
    [HideInInspector]
    public PopupType popupType = PopupType.Incident;

    public WarningBox WarningBox;

    private int caseNum;

	private const int _CITIZEN_TIPS = 5;
	private const int _WAIT_TIPS = 5;
	private const int _OFFICER_TIPS = 2;
	private const int _POSITIVE_TIPS = 5;
	private int turnsRequired;
	private int severity;
    private string _tip = "";


    public IncidentInformationDisplay IncidentInformationDisplay;

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
        var endCase = (zIncident.waitIndex == -1 && zIncident.officerIndex == -1 && zIncident.citizenIndex == -1);

        var history = m_incidentManager.GetIncidentHistory(zIncident.caseNumber);

        var requirements = "";

        if (!endCase)
        {
            var officerPlural = zIncident.officer > 1;
            var turnPlural = zIncident.turnsToAdd > 1;

            if (!officerPlural && !turnPlural)
            {
                requirements = Localization.Get("BASIC_TEXT_REQUIREMENT_SINGULAR");
            }
            else if (officerPlural && turnPlural)
            {
                requirements = Localization.Get("BASIC_TEXT_REQUIREMENT_PLURAL");
                requirements = string.Format(requirements, zIncident.officer, zIncident.turnsToAdd);
            }
            else if (officerPlural && !turnPlural)
            {
                requirements = Localization.Get("BASIC_TEXT_REQUIREMENT_OFFICER_PLURAL");
                requirements = string.Format(requirements, zIncident.officer);
            }
            else
            {
                requirements = Localization.Get("BASIC_TEXT_REQUIREMENT_TURN_PLURAL");
                requirements = string.Format(requirements, zIncident.turnsToAdd);
            }
        }

        requirements = "<size=35><b>" + requirements + "</b></size>";
        var currentInformation = new IncidentHistoryElement()
        {
            Description = Localization.Get(zIncident.incidentName) + "\n\n" + requirements,
            Type = zIncident.type,
            Feedback = "",
            FeedbackRating = 0,
            PlayerDecision = IncidentHistoryElement.Decision.Ignore
        };

        IncidentInformationDisplay.Show(history, currentInformation, zIncident.severity);
        
        caseNum = zIncident.caseNumber;
        SetSeverity(zIncident.severity);
	    severity = zIncident.severity;

		if (!endCase)
        {
	        turnsRequired = zIncident.turnsToAdd;
			// The Popup Type
            popupType = PopupType.Incident;
            
        }
        else
        {
			// The Popup Type
            popupType = PopupType.CaseClosed;
        }
		// Our Buttons
		OkText.SetActive(endCase);
		var satisfactionText = Localization.Get("BASIC_TEXT_SATISFACTION");
		if (zIncident.satisfactionImpact >= 0)
		{
			satisfactionText = satisfactionText.Insert(0, "+");
		}
		satisfaction.text = string.Format(satisfactionText, zIncident.satisfactionImpact);
		WaitText.SetActive(!endCase);

        RightButton.text = zIncident.officer == 1 ? Localization.Get("BASIC_TEXT_SEND_ONE") : RightButton.text = Localization.Get("BASIC_TEXT_SEND_MANY");

        //wait for anim to finish
        //yield return EmailAnim(-1f, "EmailShow");
        yield return new WaitForSeconds(0.0f);

        //now set which buttons should be active
        waitButton.SetActive(zIncident.waitIndex != -1 || endCase);
        SendOfficerButton.SetActive(zIncident.officerIndex != -1);
        m_citizenHelpButton.SetActive(zIncident.citizenIndex != -1);
    }
    public IEnumerator EmailAnim(float speed, string name)
    {
        yield return new WaitForSeconds(0.0f);
        //play the anim at the speed specified
        //var anim = EmailPanel.GetComponent<Animation>();

        ////set up the speed ant time to make sure the aim plays in the correct direction
        //anim[name].speed = speed;
        //var length = anim[name].length;
        //anim[name].time = speed == -1f ? length : 0f;

        //anim.Play();
        //yield return new WaitForSeconds(length);
    }
    public void SetSeverity(int zSeverity=1)
    {
        //set the alpha of the overlay to fade between yellow and red
        var alpha = 0f;
        if (zSeverity == 2)
            alpha = 0.5f;
        else if (zSeverity == 3)
            alpha = 1.0f;
    }
    public void LeftButtonPressed()
    {
		// Check if the citizen option was available
	    var isCitizensAvailable = m_citizenHelpButton.activeSelf;
        DisableButtons();
        StartCoroutine(LeftButtonWithAnim(isCitizensAvailable));
        AudioManager.Instance.PressWaitButton();
    }
    IEnumerator LeftButtonWithAnim(bool citizensAvailable = false)
    {
	    if (citizensAvailable)
	    {
		    yield return ShowTip(_CITIZEN_TIPS, "TIPS_CITIZEN_");
	    }
	    else if (popupType == PopupType.Incident)
	    {
			// dont show any tips with the case closed information
			if (turnsRequired == 1)
			{
				yield return ShowTip(2, "TIPS_OFFICER_ONE_TURN_NEGATIVE_");
			}
			else if (severity == 3)
			{
				yield return ShowTip(2, "TIPS_OFFICER_HIGH_SEVERITY_NEGATIVE_");
			}
			else
			{
				yield return ShowTip(_WAIT_TIPS, "TIPS_WAIT_");
			}
		}

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

	private IEnumerator ShowTip(int max, string preText)
	{
		// Set the text to show on screen
		var num = Random.Range(1, max + 1);
		_tip = Localization.Get(preText + num);
        
	    yield return WarningBox.ShowWarning(_tip, 5f);
	}

    public string GetTip()
    {
        return _tip;
    }
    public void RightButtonPressed()
    {
        if (m_officerController.m_officers.Count >= currentIncident.officer)
        {
			var isCitizensAvailable = m_citizenHelpButton.activeSelf;
			//double check if we have enough officers otherwise the game will break
			DisableButtons();
            StartCoroutine(RightButtonWithAnim(isCitizensAvailable));
            AudioManager.Instance.PressOfficerButton();
        }
        else
        {
            StartCoroutine(WarningBox.ShowWarning(Localization.Get("BASIC_TEXT_NO_OFFICERS"), 2f));
        }
    }
    IEnumerator RightButtonWithAnim(bool citizensAvailable = false)
    {
		if (citizensAvailable)
		{
			yield return ShowTip(_CITIZEN_TIPS, "TIPS_CITIZEN_");
		}
		else
		{
			if (turnsRequired == 1 || severity == 3)
			{
				yield return ShowTip(_POSITIVE_TIPS, "TIPS_POSITIVE_");
			}
			else
			{
				yield return ShowTip(_OFFICER_TIPS, "TIPS_OFFICER_");
			}
		}
		//send officers to resolve issue
		yield return EmailAnim(1f, "EmailShow");
        m_incidentManager.ResolvePressed();
    }
    public void CitizenButtonPressed()
    {
        DisableButtons();
        StartCoroutine(CitizenButtonWithAnim());
        AudioManager.Instance.PressCitizenButton();
    }
    IEnumerator CitizenButtonWithAnim()
    {
	    yield return ShowTip(_POSITIVE_TIPS, "TIPS_POSITIVE_");
        //removing citizen help popup and instead setting the delay to one turn
        yield return EmailAnim(1f, "EmailShow");
        m_incidentManager.CitizenHelpPressed();
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
    }
}
