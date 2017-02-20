using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class Feedback : MonoBehaviour
{
    private string _game = "New Game";
    public string _otherInfo = "Location";

    private string _feedbackSendLocation;

    private Button _cancelButton;
    private Button _sendButton;

    private UnityAction<string> _sendAction;

    private InputField _feedbackText;

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
        _otherInfo = Location.CurrentLocation + ", Language: " + Localization.GetLanguage();
    }

    public void Setup(UnityAction<string> action)
    {
        _cancelButton = transform.FindChild("Panel/FooterPanel/CancelButton").GetComponent<Button>();
        _sendButton = transform.FindChild("Panel/FooterPanel/SendButton").GetComponent<Button>();

        _feedbackText = transform.FindChild("Panel/BodyPanel/Feedback").GetComponent<InputField>();

        _feedbackText.text = "";

        _sendAction = action;

        _sendButton.onClick.AddListener(SendPressed);
        _cancelButton.onClick.AddListener(CancelPressed);

        SetInformationText();
    }

    private void SendPressed()
    {
        var body = _game + "\n" + _otherInfo + "\n\n" + _feedbackText.text;

        Loading.Set(300, false);
        Loading.Start(Localization.Get("BASIC_TEXT_SENDING_FEEDBACK"));

        _sendAction(body);
    }

    private void CancelPressed()
    {
        _feedbackText.text = "";

        this.gameObject.SetActive(false);
    }
}
