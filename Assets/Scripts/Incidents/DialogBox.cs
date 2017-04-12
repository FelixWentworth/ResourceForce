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
    public Text OfficerButtonTurns;
    public Text OfficerButtonRequired;

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
    private string _tip = "";

    private GameObject _citizenHelpButton;
    private GameObject _sendOfficerButton;
    private GameObject _waitButton;
    private GameObject _caseClosedButton;
    private ButtonFeedback _buttonFeedback;
    private GameObject _buttonFade;

    private List<Transform> _ratingTransforms;

    void Start()
    {        
        _citizenHelpButton = ButtonPanel.FindChild("CitizenHelpButton").gameObject;
        _sendOfficerButton = ButtonPanel.FindChild("SendOfficersButton").gameObject;
        _waitButton = ButtonPanel.FindChild("WaitButton").gameObject;
        _caseClosedButton = ButtonPanel.FindChild("CaseClosedButton").gameObject;
        _buttonFeedback = ButtonPanel.FindChild("FeedbackPanel").GetComponent<ButtonFeedback>();
        _buttonFeedback.gameObject.SetActive(false);
        _buttonFade = ButtonPanel.FindChild("ButtonFade").gameObject;
        _buttonFade.SetActive(false);

        DisableButtons();

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
        Debug.Log("Wait Index: " + zIncident.waitIndex + "\nOfficer Index: " +  zIncident.officerIndex + "\ncitizen Index: " + zIncident.citizenIndex + "\nScenario Index: " + zIncident.scenarioNum);
        var endCase = (zIncident.waitIndex == -1 && zIncident.officerIndex == -1 && zIncident.citizenIndex == -1);

        var history = _incidentManager.GetIncidentHistory(zIncident.caseNumber);

        //if (zIncident.waitIndex != -1 && zIncident.citizenIndex != -1)
        //{
        //    // there is both a wait and citizen option
        //    var citizenChance = 100f - _incidentManager.GetHappiness();
        //    var rand = UnityEngine.Random.Range(1, 101);

        //    Debug.Log("Chance: " + citizenChance + ", value: " + rand);

        //    if (rand > citizenChance)
        //    {
        //        zIncident.citizenIndex = -1;
        //    } 
        //}

        var currentInformation = new IncidentHistoryElement()
        {
            Description = Localization.Get(zIncident.incidentName),
            Type = endCase ? "SCENARIO_TITLE_RESOLUTION" : zIncident.type,
            Feedback = "",
            FeedbackRating = 0,
            Severity = zIncident.severity,
            PlayerDecision = IncidentHistoryElement.Decision.Ignore
        };

        IncidentInformationDisplay.Show(history, currentInformation, zIncident.severity);
        
        _caseNum = zIncident.caseNumber;

        OfficerButtonTurns.text = "x" + zIncident.turnsToAdd.ToString();
        OfficerButtonRequired.text = "x" + zIncident.officer.ToString();
        RightButton.text = zIncident.officer == 1 ? Localization.Get("BASIC_TEXT_SEND_ONE") : RightButton.text = Localization.Get("BASIC_TEXT_SEND_MANY");

        //wait for anim to finish
        yield return EmailAnim(-1f, "EmailShow");

        if (endCase)
        {
            // populate the button with feedback elements
            float satisfaction = zIncident.satisfactionImpact;

            var ratingPanel = _caseClosedButton.transform.FindChild("RatingPanel").transform;
            DestroyChildren(ratingPanel);
            _ratingTransforms = new List<Transform>();

            // satisfaction ranges from +3 to -3
            var spriteToUse = satisfaction > 0 ? PlusSprite : MinusSprite;
            var increment = satisfaction > 0 ? 1 : -1;

            for (int i = 0; i != satisfaction; i += increment)
            {
                var go = new GameObject();
                go.transform.SetParent(ratingPanel.transform);
                var img = go.AddComponent<Image>();
                img.sprite = spriteToUse;
                img.preserveAspect = true;

                _ratingTransforms.Add(go.transform);

                go.GetComponent<RectTransform>().localScale = Vector3.one;

                var ratingPanelHeight = ratingPanel.GetComponent<RectTransform>().rect.height;
                ratingPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(ratingPanelHeight, ratingPanelHeight);
            }

            if (satisfaction > 0)
            {
                satisfaction *= 0.4f;
            }
            _incidentManager.ShowSatisfactionImpact(satisfaction, true);

        }
        Debug.Log(endCase);
        //yield return new WaitForSeconds(0.25f);
        //now set which buttons should be active
        _waitButton.SetActive(zIncident.waitIndex != -1);
        _sendOfficerButton.SetActive(zIncident.officerIndex != -1);
        _citizenHelpButton.SetActive(zIncident.citizenIndex != -1);

        _caseClosedButton.SetActive(endCase);
        Debug.Log(endCase);
       

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
    public void LeftButtonPressed()
    {
		// Check if the citizen option was available
	    var isCitizensAvailable = _citizenHelpButton.activeSelf;

        _buttonFade.SetActive(true);

        ShowImmediateFeedback(CurrentIncident.feedbackRatingWait, CurrentIncident.severity, _waitButton.transform);

        StartCoroutine(LeftButtonWithAnim(isCitizensAvailable));
        AudioManager.Instance.PressWaitButton();
    }

    private IEnumerator LeftButtonWithAnim(bool citizensAvailable = false)
    {
        if (CurrentIncident.feedbackRatingWait != -1)
        {
            yield return WarningBox.ShowWarning(Localization.Get(CurrentIncident.feedbackWait), Color.cyan);
        }
        DisableButtons();


        yield return EmailAnim(1f, "EmailShow");
        //wait for more officers to become available
        _incidentManager.WaitPressed(); 
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
			_buttonFade.SetActive(true);
            StartCoroutine(RightButtonWithAnim(isCitizensAvailable));

            ShowImmediateFeedback(CurrentIncident.feedbackRatingOfficer, CurrentIncident.severity, _sendOfficerButton.transform);

            AudioManager.Instance.PressOfficerButton();
        }
        else
        {
            StartCoroutine(WarningBox.ShowWarning(Localization.Get("BASIC_TEXT_NO_OFFICERS"), Color.yellow, true));
        }
    }

    private IEnumerator RightButtonWithAnim(bool citizensAvailable = false)
    {
        if (OfficerController.m_officers.Count >= CurrentIncident.officer)
        {
            OfficerController.RemoveOfficer(CurrentIncident.officer, CurrentIncident.turnsToAdd);
            if (CurrentIncident.feedbackRatingOfficer != -1)
            {
                yield return WarningBox.ShowWarning(Localization.Get(CurrentIncident.feedbackOfficer), Color.cyan);
            }
            DisableButtons();
            //send officers to resolve issue
            yield return EmailAnim(1f, "EmailShow");
            _incidentManager.ResolvePressed();
        }
    }

    public void OkButtonPressed()
    {
        _caseClosedButton.GetComponent<Button>().interactable = false;

        StartCoroutine(OkButtonPressedWithAnim());
        AudioManager.Instance.PressCaseCloseButton();
    }

    private IEnumerator OkButtonPressedWithAnim()
    {
        
        _incidentManager.CloseCase(_caseNum, TransitionTime);
        //wait for the objects to move to the top of the screen
        yield return new WaitForSeconds(TransitionTime);

        DisableButtons();
        _caseClosedButton.GetComponent<Button>().interactable = true;
        // animate our satisfaction objects to the satisfaction bar at the top of the screen
        yield return EmailAnim(1f, "EmailShow");

        _incidentManager.ShowNext();
    }

    public void CitizenButtonPressed()
    {
        _buttonFade.SetActive(true);

        ShowImmediateFeedback(CurrentIncident.feedbackRatingCitizen, CurrentIncident.severity, _citizenHelpButton.transform);
        StartCoroutine(CitizenButtonWithAnim());
        AudioManager.Instance.PressCitizenButton();
    }

    private IEnumerator CitizenButtonWithAnim()
    {
        if (CurrentIncident.feedbackRatingCitizen != -1)
        {
            yield return WarningBox.ShowWarning(Localization.Get(CurrentIncident.feedbackCitizen), Color.cyan);
        }
        DisableButtons();
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

        _buttonFade.SetActive(false);
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

    private void ShowImmediateFeedback(int rating, int severity, Transform button)
    {
        _buttonFeedback.gameObject.SetActive(true);

        StartCoroutine(_buttonFeedback.ShowFeedback(rating, severity, button));
    }
}
