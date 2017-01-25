using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class WarningBox : MonoBehaviour
{

    public GameObject ScreenFade;

    ///This class will show and hide the warning box when called by the dialogBox class
    public bool ShowingPopup { get; private set; }
    private Text _warningText;
    private Image _borderImage;

    void Awake()
    {
        // make sure the fade is disabled
        ScreenFade.SetActive(false);
    }

    public IEnumerator ShowWarning(string message, Color color, bool upperCase = false)
    {
        SetColor(color);
        if (!ShowingPopup)
        {
            ScreenFade.SetActive(true);
            if (_warningText == null)
            {
                _warningText = transform.FindChild("Text").GetComponent<Text>();
            }

            _warningText.text = upperCase ? message.ToUpper() : message;

            AudioManager.Instance.ShowWarningMessage();

            ShowingPopup = true;

            var anim = this.GetComponent<Animation>();


            anim.Play("WarningPopup");
            yield return anim["WarningPopup"].clip.length;

            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }

            anim.Play("WarningPopupExit");
            yield return anim["WarningPopupExit"].clip.length;

            ShowingPopup = false;
            ScreenFade.SetActive(false);
        }
    }

    private void SetColor(Color color)
    {
        if (_borderImage == null)
        {
            _borderImage = transform.FindChild("Border").GetComponent<Image>();
        }
        if (_warningText == null)
        {
            _warningText = transform.FindChild("Text").GetComponent<Text>();
        }
        _warningText.color = color;
        _borderImage.color = color;
    }
}
