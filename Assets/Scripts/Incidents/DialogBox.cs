using UnityEngine;
using System.Collections;
using System.Timers;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {

    [HideInInspector]
    public Incident currentIncident;
    
    public Text Body;
	public Text Tips;
    //the ok and wait text objects are enabled based on which button to show as they have a different layout
    public GameObject OkText;
    public Text satisfaction;
    public GameObject WaitText;

    public GameObject officerRequiredInformation;

    public Text RightButton;
    public Text OfficerReqText;
    public Text TurnReqText;

    public Text EmailNumber;
    public Image emailIconSeverityOverlay;

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

    public GameObject Border_Incident;
    public GameObject Border_Resolution;
    public GameObject WarningIcon;
	[Header("Resolution")]
    public Image titleBackground;
	public Image OuterBorder;
	public Image InnerBorder;
    public Color ResolutionTint;

	private const float _TIP_TIME = 2.75f;
	private const float _TIP_TIME_QUICK = 0.6f;
	private const int _CITIZEN_TIPS = 5;
	private const int _WAIT_TIPS = 5;
	private const int _OFFICER_TIPS = 2;
	private const int _POSITIVE_TIPS = 5;
	private int turnsRequired;
	private int severity;

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

        var currentInformation = new IncidentHistoryElement()
        {
            Description = zIncident.incidentName,
            Type = zIncident.type,
            Feedback = "",
            FeedbackRating = 0,
            PlayerDecision = IncidentHistoryElement.Decision.Ignore
        };

        IncidentInformationDisplay.Show(history, currentInformation);

        //set the body of text with information
        Body.text = "<color=#00F3FFFF>" + Localization.Get("BASIC_TEXT_DESCRIPTION") + ": </color>";
        Body.text += Localization.Get(zIncident.incidentName);

	    Tips.text = "";
        //if this is the last time a player can ignore a case before it expires, show a warning that they will lose large satisfaction
        if (zIncident.turnToDevelop < m_turnManager.turn && !endCase) //check its not the end of a case as some cases can show as expiring when they take a long time to solve
        {
            Body.text += "\n\n" + Localization.Get("BASIC_TEXT_IGNORE_WARNING");
        }
        
        caseNum = zIncident.caseNumber;
        EmailNumber.text = caseNum.ToString();
        SetSeverity(zIncident.severity);
	    severity = zIncident.severity;

		if (!endCase)
        {
			// Our Text
            OfficerReqText.text = zIncident.officer.ToString();
            TurnReqText.text = zIncident.turnsToAdd.ToString();
	        turnsRequired = zIncident.turnsToAdd;
			// Our Game Objects
			WarningIcon.SetActive(true);
            Border_Incident.SetActive(true);
            Border_Resolution.SetActive(false);
			officerRequiredInformation.SetActive(true);
			// Our colours
			titleBackground.color = Color.white;
			InnerBorder.color = Color.white;
			OuterBorder.color = Color.white;
			// The Popup Type
            popupType = PopupType.Incident;
            
        }
        else
        {
			// Our Text
            OfficerReqText.text = "";
            TurnReqText.text = "";
			// Our GameObjects
            officerRequiredInformation.SetActive(false);
            WarningIcon.SetActive(false);
            Border_Incident.SetActive(false);
            Border_Resolution.SetActive(true);
			// Our Colours
            titleBackground.color = ResolutionTint;
			InnerBorder.color = ResolutionTint;
			OuterBorder.color = ResolutionTint;
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
        emailIconSeverityOverlay.color = new Color(1f, 0f, 0f, alpha);
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
		    yield return ShowTip(_CITIZEN_TIPS, "TIPS_CITIZEN_", _TIP_TIME);
	    }
	    else if (popupType == PopupType.Incident)
	    {
			// dont show any tips with the case closed information
			if (turnsRequired == 1)
			{
				yield return ShowTip(2, "TIPS_OFFICER_ONE_TURN_NEGATIVE_", _TIP_TIME);
			}
			else if (severity == 3)
			{
				yield return ShowTip(2, "TIPS_OFFICER_HIGH_SEVERITY_NEGATIVE_", _TIP_TIME);
			}
			else
			{
				yield return ShowTip(_WAIT_TIPS, "TIPS_WAIT_", _TIP_TIME);
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

	private IEnumerator ShowTip(int max, string preText, float waitTime)
	{
		// Set the text to show on screen
		var num = Random.Range(1, max + 1);
		var text = Localization.Get(preText + num);
		Tips.text = text;
		// Wait for certain time or if the player taps the screen
		var time = 0.0f;
		while (time < waitTime && !Input.GetMouseButtonDown(0))
		{
			time += Time.deltaTime;
			
			yield return null;
		}
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
            StartCoroutine(WarningBox.ShowWarning(Localization.Get("BASIC_TEXT_NO_OFFICERS")));
        }
    }
    IEnumerator RightButtonWithAnim(bool citizensAvailable = false)
    {
		if (citizensAvailable)
		{
			yield return ShowTip(_CITIZEN_TIPS, "TIPS_CITIZEN_", _TIP_TIME);
		}
		else
		{
			if (turnsRequired == 1 || severity == 3)
			{
				yield return ShowTip(_POSITIVE_TIPS, "TIPS_POSITIVE_", _TIP_TIME_QUICK);
			}
			else
			{
				yield return ShowTip(_OFFICER_TIPS, "TIPS_OFFICER_", _TIP_TIME);
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
	    yield return ShowTip(_POSITIVE_TIPS, "TIPS_POSITIVE_", _TIP_TIME_QUICK);
        //removing citizen help popup and instead setting the delay to one turn
        yield return EmailAnim(1f, "EmailShow");
        m_incidentManager.CitizenHelpPressed();
    }
    //methods for clearing data
    public void ClearDialogBox()
    {
        Body.text = "";
	    Tips.text = "";
        OfficerReqText.text = "";
        TurnReqText.text = "";
        WarningIcon.SetActive(false);
        Border_Incident.SetActive(false);
        Border_Resolution.SetActive(true);
        officerRequiredInformation.SetActive(false);
        titleBackground.color = Color.white;
		InnerBorder.color = Color.white;
		OuterBorder.color = Color.white;
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
