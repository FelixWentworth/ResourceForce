using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SatisfactionSlider : MonoBehaviour {

    public GameObject slider;
    public Image warningBG;

    private Color initColor;
    private RectTransform myRect;
    private RectTransform mySlider;
	// Use this for initialization
	void Start () {
        Setup();
	}

    private void Setup()
    {
        myRect = this.gameObject.GetComponent<RectTransform>();
        mySlider = slider.GetComponent<RectTransform>();

        initColor = warningBG.color;
    }

    public void SetSlider(float happiness)
    {
        if (mySlider == null || myRect == null)
        {
            Setup();
        }
        mySlider.anchoredPosition = new Vector2(myRect.rect.width - ((myRect.rect.width / 100f) * happiness), 0f);
        warningBG.color = new Color(initColor.r, initColor.g, initColor.b, 1f - (happiness / 100f));
    }
}
