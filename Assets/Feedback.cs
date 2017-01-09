using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Feedback : MonoBehaviour
{

    private Image _icon;
    private string _feedbackSendLocation;

    private Button _cancelButton;
    private Button _sendButton;

    private Text _feedbackText;

    private void Setup(Image icon, string feedbackSendLocation)
    {
        _icon = transform.FindChild("HeaderPanel/Icon").GetComponent<Image>();

        _cancelButton = transform.FindChild("FooterPanel/CancelButton").GetComponent<Button>();
        _sendButton = transform.FindChild("FooterPanel/SendButton").GetComponent<Button>();

        _feedbackText = transform.FindChild("BodyPanel/Feedback/Text").GetComponent<Text>();

        _icon.sprite = icon.sprite;
        _sendButton.onClick.AddListener(SendPressed);
        _cancelButton.onClick.AddListener(CancelPressed);
    }

    private void SendPressed()
    {
        var text = _feedbackText.text;
    }

    private void CancelPressed()
    {
        
    }
}
