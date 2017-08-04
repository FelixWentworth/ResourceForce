using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OfficerIndicator : MonoBehaviour {
    private Text _availableText;
    private Image _icon;

    void Awake()
    {
        GetReferences();
    }

    void GetReferences()
    {
        _availableText = GetComponentInChildren<Text>();
        _icon = GetComponentInChildren<Image>();
    }
	public void UpdateText(string text = "")
    {
        GetReferences();
        _availableText.text = text;
    }

    public void UpdateColor(Color color)
    {
        GetReferences();
        _icon.color = color;
    }
}
