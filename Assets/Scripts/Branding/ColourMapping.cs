using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourMapping : MonoBehaviour {

	public BrandingManager.ColorTheme Theme;

	// Use this for initialization
	void Start () {
		Refresh();
	}

	public void Refresh()
	{
		var image = GetComponent<Image>();
		var text = GetComponent<Text>();
		if (image != null) image.color = BrandingManager.Instance.GetColor(Theme);
		if (text != null) text.color = BrandingManager.Instance.GetColor(Theme);
	}

}
