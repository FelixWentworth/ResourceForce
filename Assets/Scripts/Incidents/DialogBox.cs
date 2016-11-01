using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {

    [HideInInspector]
    public Incident CurrentIncident;
    
    //the ok and wait text objects are enabled based on which button to show as they have a different layout
    public Text RightButton;

    public GameObject EmailPanel;

    private IncidentManager _incidentManager;
    public OfficerController OfficerController;
    public TurnManager TurnManager;
    public Transform ButtonPanel;

    public Sprite PlusSprite;
    public Sprite MinusSprite;

    public WarningBox WarningBox;

    public IncidentInformationDisplay IncidentInformationDisplay;

    private int _caseNum;

    private const float TransitionTime = 0.75f;

    private const int CitizenTips = 5;
	private const int WaitTips = 5;
	private const int OfficerTips = 2;
	private const int PositiveTips = 5;
	private int _turnsRequired;
	private int _severity;
    private string _tip = "";

    private GameObject _citizenHelpButton;
    private GameObject _sendOfficerButton;
    private GameObject _waitButton;
    private GameObject _caseClosedButton;

    private List<Transform> _ratingTransforms;

    void Start()
    {
        _citizenHelpButton = ButtonPanel.FindChild("CitizenHelpButton").gameObject;
        _sendOfficerButton = ButtonPanel.FindChild("SendOfficersButton").gameObject;
        _waitButton = ButtonPanel.FindChild("WaitButton").gameObject;
        _caseClosedButton = ButtonPanel.FindChild("CaseClosedButton").gameObject;

        _incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
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

        var history = _incidentManager.GetIncidentHistory(zIncident.caseNumber);

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

        _turnsRequired = zIncident.turnsToAdd;

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
        
        _caseNum = zIncident.caseNumber;
        SetSeverity(zIncident.severity);
	    _severity = zIncident.severity;

        RightButton.text = zIncident.officer == 1 ? Localization.Get("BASIC_TEXT_SEND_ONE") : RightButton.text = Localization.Get("BASIC_TEXT_SEND_MANY");

        //wait for anim to finish
        yield return EmailAnim(-1f, "EmailShow");

        //now set which buttons should be active
        _waitButton.SetActive(zIncident.waitIndex != -1);
        _sendOfficerButton.SetActive(zIncident.officerIndex != -1);
        _citizenHelpButton.SetActive(zIncident.citizenIndex != -1);

        _caseClosedButton.SetActive(endCase);
        if (_caseClosedButton.activeSelf)
        {
            // populate the button with feedback elements
            var satisfaction = zIncident.satisfactionImpact;

            var ratingPanel = _caseClosedButton.transform.FindChild("RatingPanel").transform;
            DestroyChildren(ratingPanel);
            _ratingTransforms = new List<Transform>();

            // satisfaction ranges from +3 to -3
            var spriteToUse = satisfaction > 0 ? PlusSprite : MinusSprite;
            var increment = satisfaction > 0 ? 1 : -1;

            for (int i = 0; i != satisfaction; i += increment)
            {
                var go = new GameObject();
                go.transform.parent = ratingPanel.transform;
                var img = go.AddComponent<Image>();
                img.sprite = spriteToUse;
                img.preserveAspect = true;

                _ratingTransforms.Add(go.transform);
            }

        }
    }

    public List<Transform> GetRatingObjects()
    {
        return _ratingTransforms;
    }

    public IEnumerator EmailAnim(float speed, string name)
    {
        yield return new WaitForSeconds(0.0f);
        //play the anim at the speed specified
        var anim = EmailPanel.GetComponent<Animation>();

        //set up the speed ant time to make sure the aim plays in the correct direction
        anim[name].speed = speed;
        var length = anim[name].length;
        anim[name].time = speed == -1f ? length : 0f;

        anim.Play();
        yield return new WaitForSeconds(length);
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
	    var isCitizensAvailable = _citizenHelpButton.activeSelf;
        DisableButtons();
        StartCoroutine(LeftButtonWithAnim(isCitizensAvailable));
        AudioManager.Instance.PressWaitButton();
    }

    private IEnumerator LeftButtonWithAnim(bool citizensAvailable = false)
    {
	    if (citizensAvailable)
	    {
		    yield return ShowTip(CitizenTips, "TIPS_CITIZEN_");
	    }
	    else
        {
			// dont show any tips with the case closed information
			if (_turnsRequired == 1)
			{
				yield return ShowTip(2, "TIPS_OFFICER_ONE_TURN_NEGATIVE_");
			}
			else if (_severity == 3)
			{
				yield return ShowTip(2, "TIPS_OFFICER_HIGH_SEVERITY_NEGATIVE_");
			}
			else
			{
				yield return ShowTip(WaitTips, "TIPS_WAIT_");
			}
		}

		yield return EmailAnim(1f, "EmailShow");
        //wait for more officers to become available
        _incidentManager.WaitPressed();
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
        if (OfficerController.m_officers.Count >= CurrentIncident.officer)
        {
			var isCitizensAvailable = _citizenHelpButton.activeSelf;
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

    private IEnumerator RightButtonWithAnim(bool citizensAvailable = false)
    {
		if (citizensAvailable)
		{
			yield return ShowTip(CitizenTips, "TIPS_CITIZEN_");
		}
		else
		{
			if (_turnsRequired == 1 || _severity == 3)
			{
				yield return ShowTip(PositiveTips, "TIPS_POSITIVE_");
			}
			else
			{
				yield return ShowTip(OfficerTips, "TIPS_OFFICER_");
			}
		}
		//send officers to resolve issue
		yield return EmailAnim(1f, "EmailShow");
        _incidentManager.ResolvePressed();
    }

    public void OkButtonPressed()
    {
        StartCoroutine(OkButtonPressedWithAnim());
        AudioManager.Instance.PressCaseCloseButton();
    }

    private IEnumerator OkButtonPressedWithAnim()
    {
        
        _incidentManager.CloseCase(_caseNum, TransitionTime);
        //wait for the objects to move to the top of the screen
        yield return new WaitForSeconds(TransitionTime);

        DisableButtons();

        // animate our satisfaction objects to the satisfaction bar at the top of the screen
        yield return EmailAnim(1f, "EmailShow");

        _incidentManager.ShowNext();
    }

    public void CitizenButtonPressed()
    {
        DisableButtons();
        StartCoroutine(CitizenButtonWithAnim());
        AudioManager.Instance.PressCitizenButton();
    }

    private IEnumerator CitizenButtonWithAnim()
    {
	    yield return ShowTip(PositiveTips, "TIPS_POSITIVE_");
        //removing citizen help popup and instead setting the delay to one turn
        yield return EmailAnim(1f, "EmailShow");
        _incidentManager.CitizenHelpPressed();
    }
    public void DisableButtons()
    {
        _waitButton.SetActive(false);
        _caseClosedButton.SetActive(false);
        _sendOfficerButton.SetActive(false);
        _citizenHelpButton.SetActive(false);
    }
    public void DeactivateAll()
    {
        DisableButtons();
    }

    private void DestroyChildren(Transform targetTransform)
    {
        foreach (Transform t in targetTransform)
        {
            Destroy(t.gameObject);
        }
    }
}
