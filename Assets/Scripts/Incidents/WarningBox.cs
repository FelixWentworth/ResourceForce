using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WarningBox : MonoBehaviour {

    ///This class will show and hide the warning box when called by the dialogBox class

    private float height;
    private bool showingPopup = false;
    private RectTransform myTransform;

    void Start()
    {
        //get our height as this is what we will offset by
        myTransform = this.GetComponent<RectTransform>();
        height = myTransform.rect.height;
    }
    public IEnumerator ShowWarning()
    {
        if (!showingPopup)
        {
            AudioManager.Instance.ShowWarningMessage();
            showingPopup = true;

            yield return ShowWarningPopup();
            showingPopup = false;
        }
    }

    IEnumerator ShowWarningPopup()
    {
        while (myTransform.anchoredPosition.y < height)
        {
            this.transform.position += Vector3.up * 120f * Time.deltaTime;
            yield return null;
        }
        while (myTransform.anchoredPosition.y > 0)
        {
            this.transform.position += Vector3.down * 120f * Time.deltaTime;
            yield return null;
        }
    }
}
