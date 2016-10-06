using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WarningBox : MonoBehaviour {

    ///This class will show and hide the warning box when called by the dialogBox class
    private bool _showingPopup;
    private Text _warningText;

    public IEnumerator ShowWarning(string message)
    {
        if (!_showingPopup)
        {
            if (_warningText == null)
            {
                _warningText = transform.FindChild("Text").GetComponent<Text>();
            }
            _warningText.text = message.ToUpper();

            AudioManager.Instance.ShowWarningMessage();

            _showingPopup = true;

            var anim = this.GetComponent<Animation>();
            anim.Play();
            yield return anim.clip.length;

            _showingPopup = false;
        }
    }
}
