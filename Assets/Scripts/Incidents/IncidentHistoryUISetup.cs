using UnityEngine;
using UnityEngine.UI;

public class IncidentHistoryUISetup : MonoBehaviour
{
    public RectTransform Header { get; set; }

    public Text TitleText;
    public Text DescriptionText;
    public Image Icon;

    public int ListPoition;

    public Image HeaderImageOverlay;
    public Image BorderImageOverlay;

    void Awake()
    {
        Header = this.transform.FindChild("Header").GetComponent<RectTransform>();
    }

    public void Setup(string titleText, string descriptionText, Sprite icon, int listPoition, float backgroundAlpha)
    {
        TitleText.text = titleText;
        DescriptionText.text = descriptionText;
        Icon.sprite = icon;
        ListPoition = listPoition;
        SetColor(backgroundAlpha);
    }

    public void Selected()
    {
        transform.parent.GetComponent<IncidentInformationDisplay>().ElementSelected(ListPoition);
    }

    private void SetColor(float alpha)
    {
        HeaderImageOverlay.color = new Color(1f, 0f, 0f, alpha);
        BorderImageOverlay.color = new Color(1f, 0f, 0f, alpha);
    }
}
