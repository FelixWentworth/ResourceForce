using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
public class SatisfactionDisplays : MonoBehaviour {

    public RectTransform myRect;
    public RectTransform mySlider;

    public Image warningBG;

    public Color FadeGB;

    public void SetSatisfactionDisplays(float satisfaction)
    {
        // set the slider and the background image alpha based off of the satisfaction
        mySlider.anchoredPosition = new Vector2(myRect.rect.width - ((myRect.rect.width / 100f) * satisfaction),0f);
        warningBG.color = new Color(FadeGB.r, FadeGB.g, FadeGB.b, 1f - (satisfaction / 100f));
        AudioManager.Instance.SetBackgroundMusicBalance(satisfaction);

    }

    public IEnumerator TransitionTo(Transform myTransform, float time, float value)
    {
        var deltaTime = 0f;
        var startPos = myTransform.position;
        while (deltaTime <= time)
        {
            myTransform.position = Vector3.Lerp(startPos, mySlider.transform.position, deltaTime / time);
            deltaTime += Time.deltaTime;
            yield return null;
        }
        myTransform.position = mySlider.transform.position;

        Destroy(myTransform.gameObject);

        mySlider.anchoredPosition = new Vector2(myRect.rect.width - ((myRect.rect.width / 100f) * value), 0f);
        warningBG.color = new Color(FadeGB.r, FadeGB.g, FadeGB.b, 1f - (value / 100f));
        AudioManager.Instance.SetBackgroundMusicBalance(value);

    }
}
