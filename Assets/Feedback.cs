﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class Feedback : MonoBehaviour
{

    private string _feedbackSendLocation;

    private Button _cancelButton;
    private Button _sendButton;

    private UnityAction<string> _sendAction;

    private Text _feedbackText;

    public void Setup(UnityAction<string> action)
    {
        _cancelButton = transform.FindChild("Panel/FooterPanel/CancelButton").GetComponent<Button>();
        _sendButton = transform.FindChild("Panel/FooterPanel/SendButton").GetComponent<Button>();

        _feedbackText = transform.FindChild("Panel/BodyPanel/Feedback/Text").GetComponent<Text>();

        _feedbackText.text = "";

        _sendAction = action;

        _sendButton.onClick.AddListener(SendPressed);
        _cancelButton.onClick.AddListener(CancelPressed);
    }

    private void SendPressed()
    {
        var body = _feedbackText.text;

        _sendAction(body);
    }

    private void CancelPressed()
    {
        _feedbackText.text = "";

        this.gameObject.SetActive(false);
    }

   
}
