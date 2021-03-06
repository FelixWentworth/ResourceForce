﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayGen.Unity.Utilities.Localization;
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

    private string _caseNum;

    private const float TransitionTime = 0.75f;
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
        _citizenHelpButton = ButtonPanel.Find("CitizenHelpButton").gameObject;
        _sendOfficerButton = ButtonPanel.Find("SendOfficersButton").gameObject;
        _waitButton = ButtonPanel.Find("WaitButton").gameObject;
        _caseClosedButton = ButtonPanel.Find("CaseClosedButton").gameObject;
        _buttonFeedback = ButtonPanel.Find("FeedbackPanel").GetComponent<ButtonFeedback>();
        _buttonFeedback.gameObject.SetActive(false);
        _buttonFade = ButtonPanel.Find("ButtonFade").gameObject;
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
        var endCase = zIncident.IsEndCase();

        var history = _incidentManager.GetIncidentHistory(zIncident.Scenario.Id);

        var currentInformation = new IncidentHistoryElement()
        {
            Type = zIncident.IncidentContent.Title,
            Description = zIncident.IncidentContent.Description,
            Feedback = "",
            FeedbackRating = 0,
            Severity = zIncident.IncidentContent.Severity,
            PlayerDecision = IncidentHistoryElement.Decision.Ignore
        };

        IncidentInformationDisplay.Show(history, currentInformation, currentInformation.Severity);
        
        _caseNum = zIncident.Scenario.Id;

        OfficerButtonTurns.text = "x" + (zIncident.IncidentContent.TurnReq > 0 ? zIncident.IncidentContent.TurnReq.ToString() : "0");
        OfficerButtonRequired.text = "x" + (zIncident.IncidentContent.OfficerReq > 0 ? zIncident.IncidentContent.OfficerReq.ToString() : "0");
        RightButton.text = zIncident.IncidentContent.OfficerReq == 1 ? Localization.Get("BASIC_TEXT_SEND_ONE") : RightButton.text = Localization.Get("BASIC_TEXT_SEND_MANY");

        //wait for anim to finish
        yield return EmailAnim(-1f, "EmailShow");

        if (endCase)
        {
            // populate the button with feedback elements
            float satisfaction = zIncident.IncidentContent.SatisfactionImpact;

            var ratingPanel = _caseClosedButton.transform.Find("RatingPanel").transform;
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
        //yield return new WaitForSeconds(0.25f);
        //now set which buttons should be active
        if (endCase)
        {
            DisableButtons();
        }
        else
        {
            SetButtonActive(_waitButton, zIncident.GetChoiceContent("Ignore") != null);
            SetButtonActive(_sendOfficerButton, zIncident.GetChoiceContent("Officer") != null);
            SetButtonActive(_citizenHelpButton, zIncident.GetChoiceContent("Citizen") != null);
        }

        //_waitButton.SetActive(zIncident.GetChoiceContent("Ignore") != null);
        //_sendOfficerButton.SetActive(zIncident.GetChoiceContent("Officer") != null);
        //_citizenHelpButton.SetActive(zIncident.GetChoiceContent("Citizen") != null);

        _caseClosedButton.SetActive(endCase);
    }

    private void SetButtonActive(GameObject obj, bool active)
    {
        var button = obj.GetComponent<Button>();
        var canvasGroup = obj.GetComponent<CanvasGroup>();

        button.interactable = active;
        canvasGroup.alpha = active ? 1f : .5f;

        obj.SetActive(true);
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
		if (_buttonFade.activeSelf)
		{
			return;
		}
        _buttonFade.SetActive(true);

        var feedback = CurrentIncident.GetChoiceFeedback("Ignore");

        ShowImmediateFeedback(feedback.FeedbackRating, CurrentIncident.IncidentContent.Severity, _waitButton.transform);

        StartCoroutine(LeftButtonWithAnim(feedback));

        ButtonFeedbackManager.Instance.ShowFeedback(ButtonFeedbackManager.FeedbackType.Ignore, CurrentIncident.IncidentContent.Severity);
    }

    private IEnumerator LeftButtonWithAnim(ChoiceFeedback feedback)
    {

        if (feedback.FeedbackRating != -1)
        {
            yield return WarningBox.ShowWarning(feedback.Feedback, error: false);
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
		if (_buttonFade.activeSelf)
		{
			return;
		}
		if (OfficerController.m_officers.Count >= CurrentIncident.IncidentContent.OfficerReq)
        {
			//double check if we have enough officers otherwise the game will break
			_buttonFade.SetActive(true);

            var feedback = CurrentIncident.GetChoiceFeedback("Officer");

            StartCoroutine(RightButtonWithAnim(feedback));

            ShowImmediateFeedback(feedback.FeedbackRating, CurrentIncident.IncidentContent.Severity, _sendOfficerButton.transform);

            ButtonFeedbackManager.Instance.ShowFeedback(ButtonFeedbackManager.FeedbackType.Officer, CurrentIncident.IncidentContent.Severity);
        }
        else
        {
            StartCoroutine(WarningBox.ShowWarning(Localization.Get("BASIC_TEXT_NO_OFFICERS"), error: true, upperCase:true));
        }
    }

    private IEnumerator RightButtonWithAnim(ChoiceFeedback feedback)
    {
        if (OfficerController.m_officers.Count >= CurrentIncident.IncidentContent.OfficerReq)
        {
            OfficerController.RemoveOfficer(CurrentIncident.IncidentContent.OfficerReq, CurrentIncident.IncidentContent.TurnReq);
            if (feedback.FeedbackRating != -1)
            {
                yield return WarningBox.ShowWarning(feedback.Feedback, error: false);
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
		if (_buttonFade.activeSelf)
		{
			return;
		}
		_buttonFade.SetActive(true);

        var feedback = CurrentIncident.GetChoiceFeedback("Citizen");

        ShowImmediateFeedback(feedback.FeedbackRating, CurrentIncident.IncidentContent.Severity, _citizenHelpButton.transform);
        StartCoroutine(CitizenButtonWithAnim(feedback));
        ButtonFeedbackManager.Instance.ShowFeedback(ButtonFeedbackManager.FeedbackType.Citizen, CurrentIncident.IncidentContent.Severity);
    }

    private IEnumerator CitizenButtonWithAnim(ChoiceFeedback feedback)
    {
        if (feedback.FeedbackRating != -1)
        {
            yield return WarningBox.ShowWarning(feedback.Feedback, error: false);
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
        // Make sure that the feedback objects are also disbled
        ButtonFeedbackManager.Instance.ResetObjects();
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
