using UnityEngine;
using System.Collections;
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
}
