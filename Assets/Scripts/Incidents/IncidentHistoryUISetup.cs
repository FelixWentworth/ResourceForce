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

    public RectTransform DropDownImage;

    void Awake()
    {
        Header = this.transform.Find("Header").GetComponent<RectTransform>();
    }

    public void Setup(string titleText, string descriptionText, Sprite icon, int listPoition, Color severity)
    {
        TitleText.text = titleText;
        DescriptionText.text = descriptionText;
        Icon.sprite = icon;
        ListPoition = listPoition;
        SetColor(severity);
    }

    public void Selected()
    {
        transform.parent.GetComponent<IncidentInformationDisplay>().ElementSelected(ListPoition);
    }

    private void SetColor(Color severity)
    {
        HeaderImageOverlay.color = severity;
        BorderImageOverlay.color = severity;
    }
}
