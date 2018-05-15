using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WarningBox : MonoBehaviour
{

    public GameObject ScreenFade;

    ///This class will show and hide the warning box when called by the dialogBox class
    public bool ShowingPopup { get; private set; }
    private Text _warningText;
	[SerializeField] private GameObject _messageBorder;
	[SerializeField] private GameObject _errorBorder;

	void Awake()
    {
        // make sure the fade is disabled
        ScreenFade.SetActive(false);
    }

    public void StartShowWarning(string message, bool error, bool upperCase = false)
    {
        StartCoroutine(ShowWarning(message, error, upperCase));
    }

    public IEnumerator ShowWarning(string message, bool error, bool upperCase = false)
	{
		_messageBorder.SetActive(!error);
		_errorBorder.SetActive(error);

        if (!ShowingPopup)
        {
            ScreenFade.SetActive(true);
            if (_warningText == null)
            {
                _warningText = transform.Find("Text").GetComponent<Text>();
            }

            _warningText.text = upperCase ? message.ToUpper() : message;
	        _warningText.color = error
		        ? BrandingManager.Instance.GetColor(BrandingManager.ColorTheme.ErrorText)
		        : BrandingManager.Instance.GetColor(BrandingManager.ColorTheme.StandardText);

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

            var tutorial = GameObject.Find("TurnManager").GetComponent<Tutorial>();
            if (tutorial != null)
            {
                tutorial.FeedbackDismissed();
            }
        }
    }
}
