using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MarketingManager : MonoBehaviour
{

	/// <summary>
	/// Handles aspects of the game that can be altered for use as a standalone app
	/// </summary>

	public static MarketingManager Instance;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(this);
		}
		Instance = this;
		DontDestroyOnLoad(this);
	}

	[Tooltip("if the manager will be used to override elements")]
	public bool UseManager;

	[Serializable]
	public struct Metadata
	{
		public string AppName;
		public string BundleId;
		public string Version;
		public string AuthoringToolUrl;
		public string Location;
	}

	[Serializable]
	public struct ElementImages
	{
		[Tooltip("Name Not used, for ease of use only")]
		public string Name;
		public Image ImageElement;
		public Sprite NewSprite;
		public Color Tint;
	}

	[Serializable]
	public struct ExternalLinks
	{
		[Tooltip("Name Not used, for ease of use only")]
		public string Name;
		public ButtonLink Button;
		public string Url;
		public bool SendEvent;
		public string EventName;
	}

	public Metadata AppMetadata;

	public List<ElementImages> Elements;

	public List<ExternalLinks> Links;


	[Space(25)]

	public bool Apply;
	// Update is called once per frame
	void Update () {
		if (Apply)
		{
			Apply = false;
			foreach (var e in Elements)
			{
				e.ImageElement.sprite = e.NewSprite;
				e.ImageElement.color = e.Tint;
			}

			foreach (var link in Links)
			{
				link.Button.Url = link.Url;
				link.Button.SendEvent = link.SendEvent;
				link.Button.EventName = link.EventName;
			}

			SetMetadata();
		}

	}

	private void SetMetadata()
	{
		PlayerSettings.productName = AppMetadata.AppName;
		PlayerSettings.bundleVersion = AppMetadata.Version;
		PlayerSettings.applicationIdentifier = AppMetadata.BundleId;

		GameObject.Find("ContentManager").GetComponent<ContentRequest>().SetUrl(AppMetadata.AuthoringToolUrl);
	}
}
