using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OfficerIndicator : MonoBehaviour {
    private Text _availableText;
    private Image _icon;

    void Awake()
    {
        _availableText = this.transform.GetChild(1).GetComponent<Text>();
        _icon = this.transform.GetChild(0).GetComponent<Image>();
    }
	public void UpdateText(string text = "")
    {
        _availableText.text = text;
    }

    public void UpdateColor(Color color)
    {
        _icon.color = color;
    }
}
