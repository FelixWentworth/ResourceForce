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
		GetComponent<Image>().color = BrandingManager.Instance.GetColor(Theme);
	}

}
