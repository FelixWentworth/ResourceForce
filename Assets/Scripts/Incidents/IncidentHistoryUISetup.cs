using UnityEngine;
using UnityEngine.UI;

public class IncidentHistoryUISetup : MonoBehaviour
{
    public RectTransform Header { get; set; }

    public Text TitleText;
    public Text DescriptionText;
    public Image Icon;

    public int ListPoition;

    void Awake()
    {
        Header = this.transform.FindChild("Header").GetComponent<RectTransform>();
    }

    public void Setup(string titleText, string descriptionText, Sprite icon, int listPoition)
    {
        TitleText.text = titleText;
        DescriptionText.text = descriptionText;
        Icon.sprite = icon;
        ListPoition = listPoition;
    }

    public void Selected()
    {
        transform.parent.GetComponent<IncidentInformationDisplay>().ElementSelected(ListPoition);
    }
}
