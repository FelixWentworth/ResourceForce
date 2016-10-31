using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class WarningBox : MonoBehaviour {

    ///This class will show and hide the warning box when called by the dialogBox class
    private bool _showingPopup;
    private Text _warningText;

    public IEnumerator ShowWarning(string message, float pauseTime)
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


            anim.Play("WarningPopup");
            yield return anim["WarningPopup"].clip.length;

            while (pauseTime >= 0f && !Input.GetMouseButtonDown(0))
            {
                pauseTime -= Time.deltaTime;
                yield return null;
            }

            anim.Play("WarningPopupExit");
            yield return anim["WarningPopupExit"].clip.length;

            _showingPopup = false;
        }
    }
}
