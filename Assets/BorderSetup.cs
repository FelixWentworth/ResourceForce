using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorderSetup : MonoBehaviour
{

	public Color PrimaryColor;
	public Color SecondaryColor;

	public Image[] PrimaryImages;
	public Image[] SecondaryImages;

	[Range(0f, 1f)] public float CanvasAlpha; 

	void Start()
	{
		GetComponentInChildren<CanvasGroup>().alpha = CanvasAlpha;
		foreach (var image in PrimaryImages)
		{
			image.color = PrimaryColor;
		}
		foreach (var image in SecondaryImages)
		{
			image.color = SecondaryColor;
		}
	}
}
