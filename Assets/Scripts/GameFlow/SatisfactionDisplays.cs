﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
public class SatisfactionDisplays : MonoBehaviour {

    public RectTransform myRect;

    public Image warningBG;

    public Color FadeGB;

    public Color BackgroundColor;

    private Transform _segmentParent;
    private Image _segment;

    private Text _satisfactionValue;

    private const float MaxValue = 100f;

    public void SetSatisfactionDisplays(float satisfaction)
    {
        //mySlider.anchoredPosition = new Vector2(myRect.rect.width - ((myRect.rect.width /MaxValue) * satisfaction),0f);
        warningBG.color = new Color(FadeGB.r, FadeGB.g, FadeGB.b, 1f - (satisfaction / MaxValue));
        AudioManager.Instance.SetBackgroundMusicBalance(satisfaction);

        SetSatisfactionText(Mathf.RoundToInt(satisfaction));

        StartCoroutine(SetSatisfactionFillAmount(1f, satisfaction, MaxValue));
    }

    public IEnumerator TransitionTo(Transform myTransform, float time, float value)
    {
        var deltaTime = 0f;
        var startPos = myTransform.position;

        while (deltaTime <= time)
        {
            myTransform.position = Vector3.Lerp(startPos, myRect.transform.position, deltaTime/time);
            deltaTime += Time.deltaTime;
            yield return null;
        }
        myTransform.position = myRect.transform.position;

        Destroy(myTransform.gameObject);
        StartCoroutine(SetSatisfactionFillAmount(0.5f, value, MaxValue));
        
    }

    private IEnumerator SetSatisfactionFillAmount(float time, float happiness, float maxHappiness)
    {
        var satisfactionBar = transform.FindChild("BarOverlay").GetComponent<Image>();

        var startValue = satisfactionBar.fillAmount;
        var endValue = happiness/maxHappiness;

        var deltaTime = 0f;
        while (deltaTime <= time)
        {
            var newFillAmount = Mathf.Lerp(startValue, endValue, deltaTime/time);

            satisfactionBar.fillAmount = newFillAmount;
            SetSatisfactionText(Mathf.RoundToInt(newFillAmount*100f));

            deltaTime += Time.deltaTime;
            yield return null;
        }

        satisfactionBar.fillAmount = endValue;
        SetSatisfactionText(Mathf.RoundToInt(endValue * 100f));
    }

    private void SetSatisfactionText(int satisfaction)
    {
        if (_satisfactionValue == null)
        {
            _satisfactionValue = transform.FindChild("SatisfactionValueText").GetComponent<Text>();
        }

        _satisfactionValue.text = satisfaction + "%";
    }
}
