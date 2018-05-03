using UnityEngine;
using UnityEngine.UI;

public class IncidentHistoryUISetup : MonoBehaviour
{
    public RectTransform Header { get; set; }

    public Text TitleText;
    public Text DescriptionText;
    public Image Icon;

    public int ListPosition;

    public Image HeaderImageOverlay;
    public Image BorderImageOverlay;

    public RectTransform DropDownImage;

    void Awake()
    {
        Header = this.transform.Find("Header").GetComponent<RectTransform>();
    }

    public void Setup(string titleText, string descriptionText, Sprite icon, int listPosition, Color severity)
    {
        TitleText.text = titleText;
        Icon.sprite = icon;
        ListPosition = listPosition;
        SetColor(severity);

		if (DescriptionText)
			DescriptionText.text = descriptionText;
	    if (DropDownImage)
			DropDownImage.gameObject.SetActive(ListPosition > 0);
	}

	public void Selected()
    {
        transform.parent.GetComponent<IncidentInformationDisplay>().ElementSelected(DropDownImage);
    }

    private void SetColor(Color severity)
    {
		if (HeaderImageOverlay)
			HeaderImageOverlay.color = severity;
		if (BorderImageOverlay)
			BorderImageOverlay.color = severity;
    }
}
