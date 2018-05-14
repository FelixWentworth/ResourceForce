using System.Collections;
using System.Collections.Generic;
using PlayGen.Unity.Utilities.Localization;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class Feedback : MonoBehaviour
{
    private string _game = "New Game";
    public string _otherInfo = "Region";

    private string _feedbackSendLocation;

    private Button _cancelButton;
    private Button _sendButton;

    private UnityAction<string> _sendAction;

    private InputField _feedbackText;
    private InputField _emailText;

    private bool _sending;

    void OnEnable()
    {
        TouchScreenKeyboard.hideInput = true;
    }

    void OnDisable()
    {
        TouchScreenKeyboard.hideInput = false;
    }

    public void SetInformationText()
    {
        _game = "Resource Force";
        _otherInfo = Location.CurrentLocation + ", Language: " + Localization.SelectedLanguage.DisplayName;
    }

    public void Setup(UnityAction<string> action)
    {
        _cancelButton = transform.Find("Panel/FooterPanel/CancelButton").GetComponent<Button>();
        _sendButton = transform.Find("Panel/FooterPanel/SendButton").GetComponent<Button>();

        _feedbackText = transform.Find("Panel/BodyPanel/Feedback").GetComponent<InputField>();
        _emailText = transform.Find("Panel/BodyPanel/EmailEntry").GetComponent<InputField>();

        _feedbackText.text = "";
        _emailText.text = "";

        _sendAction = action;
        _sending = false;

        _sendButton.onClick.AddListener(SendPressed);
        _cancelButton.onClick.AddListener(CancelPressed);

        SetInformationText();
    }

    private void SendPressed()
    {
        if (_sending)
            return;

        // Stop the game spamming useless feedback, allow email to be provided if players want further information
        if (_feedbackText.text == "" && _emailText.text == "")
        {
            CancelPressed();
            return;
        }
        _sending = true;
        var body = _game + "\n" + _otherInfo + "\n\n" + _feedbackText.text;
        if (_emailText.text != "")
        {
            body += "\n\nFrom: " + _emailText.text + "\n\n";
        }


        Loading.Set(300, false);
        Loading.Start(Localization.Get("BASIC_TEXT_SENDING"));

        _sendAction(body);
    }

    private void CancelPressed()
    {
        _feedbackText.text = "";

        this.gameObject.SetActive(false);
    }
}
