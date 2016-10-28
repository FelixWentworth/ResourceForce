using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.VR;

public class AdvancedAnchor : MonoBehaviour {

    public enum AnchorAlignment
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }
    [Header("Top Target")]
    public RectTransform TopTarget;
    public AnchorAlignment TopAlignment;
    public float TopPadding;
    [Header("Bottom Target")]
    public RectTransform BottomTarget;
    public AnchorAlignment BottomAlignment;
    public float BottomPadding;
    [Header("Left Target")]
    public RectTransform LeftTarget;
    public AnchorAlignment LeftAlignment;
    public float LeftPadding;
    [Header("Right Target")]
    public RectTransform RightTarget;
    public AnchorAlignment RightAlignment;
    public float RightPadding;

    private RectTransform _transform;
    private CanvasScaler _canvas;


    void LateUpdate()
    {
        if (_transform == null)
        {
            _transform = this.GetComponent<RectTransform>();
            if (_transform == null)
            {
                Debug.LogError(string.Format("No RectTransform found on attached object: {0}", this.gameObject.name));
            }
        }
        if (_canvas == null)
        {
            // get the first canvas scaler from the parent of this object
            FindParentCanvas();
        }
        if (TopTarget == null && BottomTarget == null && LeftTarget == null && RightTarget == null)
        {
            return;
        }

        var width = GetPosition(RightTarget, RightAlignment).x - GetPosition(LeftTarget, LeftAlignment).x;
        var height = GetPosition(TopTarget, TopAlignment).y - GetPosition(BottomTarget, BottomAlignment).y;

        var screenResolution = new Vector2(Screen.width, Screen.height);
        var referenceResolution = _canvas.referenceResolution;

        var scale = referenceResolution.x/screenResolution.x;

        _transform.sizeDelta = new Vector2(width, height) * scale;
    }

    private void FindParentCanvas()
    {
        var t = this.transform;
        while (t.parent != null && _canvas == null)
        {
            _canvas = t.parent.gameObject.GetComponent<CanvasScaler>();
            t = t.parent;
        }
    }

    private Vector2 GetPosition(RectTransform target, AnchorAlignment alignment)
    {

        var rectCenter = target.TransformPoint(target.rect.center);

        var rectMin = target.TransformPoint(target.rect.min);
        var rectMax = target.TransformPoint(target.rect.max);

        var rectWidth = (rectMax.x - rectMin.x)/2f;
        var rectHeight = (rectMax.y - rectMin.y)/2f;
        switch (alignment)
        {
            case AnchorAlignment.TopLeft:
                return new Vector2(rectCenter.x - rectWidth, rectCenter.y + rectHeight);

            case AnchorAlignment.TopCenter:
                return new Vector2(rectCenter.x, rectCenter.y + rectHeight);

            case AnchorAlignment.TopRight:
                return new Vector2(rectCenter.x + rectWidth, rectCenter.y + rectHeight);

            case AnchorAlignment.MiddleLeft:
                return new Vector2(rectCenter.x - rectWidth, rectCenter.y);

            case AnchorAlignment.MiddleCenter:
                return rectCenter;

            case AnchorAlignment.MiddleRight:
                return new Vector2(rectCenter.x + rectWidth, rectCenter.y);

            case AnchorAlignment.BottomLeft:
                return new Vector2(rectCenter.x - rectWidth, rectCenter.y - rectHeight);

            case AnchorAlignment.BottomCenter:
                return new Vector2(rectCenter.x, rectCenter.y - rectHeight);

            case AnchorAlignment.BottomRight:
                return new Vector2(rectCenter.x + rectWidth, rectCenter.y - rectHeight);

        }
        return Vector2.zero;
    }
}
