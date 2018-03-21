using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BrandingManager : MonoBehaviour
{

	/// <summary>
	/// Handles aspects of the game that can be altered for use as a standalone app
	/// </summary>

	public static BrandingManager Instance;

	void Awake()
	{
		if (Application.isPlaying)
		{
			if (Instance != null)
			{
				Destroy(this);
			}
			Instance = this;
			DontDestroyOnLoad(this);
		}
	}	

	void Start()
	{
		if (UseManager)
		{
			// 5 == Custom
			GameObject.Find("LocationMaster").GetComponent<Location>().SetSite(5);
		}
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
		public Sprite[] SplashScreenLogos;
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

	[Serializable]
	public struct SupportedLanguages
	{
		public bool English;
		public bool Spanish;
		public bool Dutch;
		public bool Greek;
	}

	[Serializable]
	public struct BrandingObjects
	{
		[Tooltip("Name Not used, for ease of use only")]
		public string Name;
		public GameObject Obj;
		/// <summary>
		/// On apply, will check if using the manager then set the object with <param name="IsBrandingObject"></param> to be active
		/// </summary>
		public bool IsBrandingObject;
	}

	public Metadata AppMetadata;
	public SupportedLanguages Languages;
	public List<ElementImages> Elements;
	public List<ExternalLinks> Links;
	public List<BrandingObjects> ObjectsForBranding;

	[Space(25)]

	public bool Apply;
	// Update is called once per frame
	void Update () {
		if (Apply)
		{
			Apply = false;
		
			SetImages();
			SetLinks();
			SetMetadata();
			SetBrandingObjects();
		}

	}

	private void SetImages()
	{
		foreach (var e in Elements)
		{
			e.ImageElement.sprite = e.NewSprite;
			e.ImageElement.color = e.Tint;
		}
	}

	private void SetLinks()
	{
		foreach (var link in Links)
		{
			link.Button.Url = link.Url;
			link.Button.SendEvent = link.SendEvent;
			link.Button.EventName = link.EventName;
		}
	}

	private void SetMetadata()
	{
#if UNITY_EDITOR
		PlayerSettings.productName = AppMetadata.AppName;
		PlayerSettings.bundleVersion = AppMetadata.Version;
		PlayerSettings.applicationIdentifier = AppMetadata.BundleId;
		var logos = new PlayerSettings.SplashScreenLogo[AppMetadata.SplashScreenLogos.Length];
		for (var i = 0; i < AppMetadata.SplashScreenLogos.Length; i++)
		{
			logos[i].logo = AppMetadata.SplashScreenLogos[i];
		}
		PlayerSettings.SplashScreen.logos = logos;
#endif
		GameObject.Find("ContentManager").GetComponent<ContentRequest>().SetUrl(AppMetadata.AuthoringToolUrl);
	}

	private void SetBrandingObjects()
	{
		foreach (var branding in ObjectsForBranding)
		{
			var active = (branding.IsBrandingObject && UseManager) || (!branding.IsBrandingObject && !UseManager);
			
			branding.Obj.SetActive(active);
		}
	}
}
