using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
public class SatisfactionDisplays : MonoBehaviour {

    public RectTransform myRect;

    public Image warningBG;

    public Color FadeGB;

    public Color BackgroundColor;
    private Color _segmentColor;

    private Transform _segmentParent;
    private Image _segment;

    private List<Image> _segments;
    private int _numSegments = 20;

    private float _satisfaction = -1f;

    private Text _satisfactionValue;

    public void SetSatisfactionDisplays(float satisfaction)
    {
        //mySlider.anchoredPosition = new Vector2(myRect.rect.width - ((myRect.rect.width / 100f) * satisfaction),0f);
        warningBG.color = new Color(FadeGB.r, FadeGB.g, FadeGB.b, 1f - (satisfaction / 100f));
        AudioManager.Instance.SetBackgroundMusicBalance(satisfaction);

        SetSatisfactionText(Mathf.RoundToInt(satisfaction));

        _numSegments = 25;
        SetSegments(satisfaction, _numSegments);
    }

    public void SetSegments(float satisfaction, int numSegments = 20)
    {
        _segmentParent = transform.FindChild("SliderPanel/SegmentPanel").transform;

        _numSegments = numSegments;

        var _fadeOffset = 0f;
        var _fadeOffsetIncrement = 5f / _numSegments;
        _segmentColor = new Color(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, 0f);
        if (_segments == null || _segments.Count != _numSegments)
        {
            // make sure we take a copy of the segment before clearing children
            if (_segment == null)
            {
                _segment = transform.FindChild("SliderPanel/SegmentPanel/Segment").GetComponent<Image>();
            }


            _segments = new List<Image>(_numSegments) {_segment};
            // instantiate our segments
            for (var i = 1; i < _numSegments; i++)
            {
                var go = Instantiate(_segment.gameObject);
                go.transform.parent = _segmentParent;
                go.transform.localScale = Vector3.one;
                go.name = "Segment_" + i;
                go.GetComponent<Image>().color = _segmentColor;
                _segments.Add(go.GetComponent<Image>());
            }
        }

        if (_satisfaction == -1f)
        {
            _satisfaction = satisfaction;
        }
        var increment = _satisfaction >= satisfaction ? 1 : -1;
        var startValue = _satisfaction >= satisfaction ? 0 : _numSegments - 1;

        // disable segments
        var segmentsToDisable = _numSegments - ((satisfaction/100f)*_numSegments);
        for (var i = startValue; i < _numSegments && i >= 0; i += increment)
        {
            var toDisable = i <= segmentsToDisable;
            // Check the segment should be the color as the background - Faded out
            if (toDisable && _segments[i].color == _segmentColor) 
            {
                StartCoroutine(FadeColor(_fadeOffset, 1.0f, true, _segments[i]));
                _fadeOffset += _fadeOffsetIncrement;
            }
            else if (!toDisable)
            {
                if (_segments[i].color != _segmentColor)
                {
                    StartCoroutine(FadeColor(_fadeOffset, 1.0f, false, _segments[i]));
                    _fadeOffset += _fadeOffsetIncrement;
                }
                else
                {
                    _segments[i].color = _segmentColor;
                }
               
            }
        }

        SetSatisfactionText(Mathf.RoundToInt(satisfaction));
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

        SetSegments(value, _numSegments);

    }

    public IEnumerator FadeColor(float offset, float time, bool toBackground, Image image)
    {
        yield return new WaitForSeconds(offset);
        var normal = _segmentColor;
        var faded = BackgroundColor;

        var startColor = toBackground ? normal : faded;
        var endColor = toBackground ? faded : normal;

        var deltaTime = 0f;
        while (deltaTime < time)
        {
            image.color = Color.Lerp(startColor, endColor, deltaTime);
            deltaTime += Time.deltaTime;
            yield return null;
        }

        image.color = endColor;
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
