using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
public class SatisfactionDisplays : MonoBehaviour {

    public RectTransform myRect;
    public RectTransform mySlider;

    public Image warningBG;

    public Color FadeGB;

    private const float SlideTime = 0.6f;

    public void SetSatisfactionDisplays(float satisfaction)
    {
        // set the slider and the background image alpha based off of the satisfaction
        StartCoroutine(MoveSlider(new Vector2(myRect.rect.width - ((myRect.rect.width / 100f) * satisfaction), 0f)));
        //mySlider.anchoredPosition = new Vector2(myRect.rect.width - ((myRect.rect.width / 100f) * satisfaction),0f);
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

        StartCoroutine(MoveSlider(new Vector2(myRect.rect.width - ((myRect.rect.width/100f)*value), 0f)));
        warningBG.color = new Color(FadeGB.r, FadeGB.g, FadeGB.b, 1f - (value / 100f));
        AudioManager.Instance.SetBackgroundMusicBalance(value);

    }

    public void SetPulseAnim(bool danger)
    {
        var anim = mySlider.GetComponent<Animation>();
        if (danger)
        {
            anim.Play();
        }
        else
        {
            anim.Stop();
            mySlider.localScale = Vector3.one;
        }
    }

    private IEnumerator MoveSlider(Vector2 targetVector2)
    {
        var deltaTime = 0f;
        var startPos = mySlider.anchoredPosition;

        while (deltaTime <= SlideTime)
        {
            mySlider.anchoredPosition = Vector2.Lerp(startPos,targetVector2, deltaTime / SlideTime);
            deltaTime += Time.deltaTime;
            yield return null;
        }
        mySlider.anchoredPosition = targetVector2;
    }
}
