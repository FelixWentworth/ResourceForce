﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class BrandingManager : MonoBehaviour
{

	/// <summary>
	/// Handles aspects of the game that can be altered for use as a standalone app
	/// </summary>

	public static BrandingManager Instance;

    [SerializeField] private BrandingConfig _brandingConfig;

    public BrandingConfig Config
    {
        get { return _brandingConfig; }
    }

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

	[Tooltip("if the manager will be used to override elements")]
	public bool UseBrandingObjects;
    
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
	[Serializable]
	public enum ColorTheme
	{
		Background,
		BasicBorder,
		MessagePrimary,
		MessageSecondary,
		ErrorPrimary,
		ErrorSecondary,
		StandardText,
		ErrorText
	}

	[Serializable]
	public struct ColorPalette
	{
		public ColorTheme Theme;
		public Color Color;
	}

	[Serializable]
	public class Colors
	{
		public List<ColorMapping> Themes = new List<ColorMapping>()
		{
			new ColorMapping(ColorTheme.Background),
			new ColorMapping(ColorTheme.BasicBorder),
			new ColorMapping(ColorTheme.MessagePrimary),
			new ColorMapping(ColorTheme.MessageSecondary),
			new ColorMapping(ColorTheme.ErrorPrimary),
			new ColorMapping(ColorTheme.ErrorSecondary),
			new ColorMapping(ColorTheme.StandardText),
			new ColorMapping(ColorTheme.ErrorText)
		};
	}

	// Branding colors outside of unity editor as it is needed at runtime
	[Header("Color Palette")]
	[SerializeField] private Colors _brandingColors;
	
#if UNITY_EDITOR
	[Header("Localization")]
    public string LocalizationSource;
    public string LocalizationOutput;

    [Header("Element Images")]
    [SerializeField] private Image _startScreenLogo;
    [SerializeField] private Image _gameLogo;
    [SerializeField] private Image _citizenButtonIcon;
    [SerializeField] private Image _endGameLinkIcon;
    [SerializeField] private Image _externalLinkWebsite;
    [SerializeField] private Image _externalLinkFacebook;
    [SerializeField] private Image _externalLinkTwitter;
    [SerializeField] private Image _locationSelectLogo;
    [SerializeField] private Image _tapScreenStartBackground;
    [SerializeField] private Image _inGameMap;
    [SerializeField] private Image _homeScreenMap;

    [Header("External Links")]
    [SerializeField] private ButtonLink _homeAppDownload;
    [SerializeField] private ButtonLink _endAppDownload;
    [SerializeField] private ButtonLink _optionsWebsite;
    [SerializeField] private ButtonLink _optionsFacebook;
    [SerializeField] private ButtonLink _optionsTwitter;

	[Space(10)]

	public List<BrandingObjects> ObjectsForBranding;

	public void Apply()
	{
	    SetImages();
		SetLinks();
		SetMetadata();
		SetBrandingObjects();
	}

    private void SetImages()
	{
	    Config.StartScreenLogo.ApplyTo(_startScreenLogo);

	    Config.GameLogo.ApplyTo(_gameLogo);
	    ApplyPrefab(_gameLogo);

        Config.CitizenButtonIcon.ApplyTo(_citizenButtonIcon);
	    Config.EndGameLinkIcon.ApplyTo(_endGameLinkIcon);
	    Config.ExternalLinkWebsite.ApplyTo(_externalLinkWebsite);
	    Config.ExternalLinkFacebook.ApplyTo(_externalLinkFacebook);
	    Config.ExternalLinkTwitter.ApplyTo(_externalLinkTwitter);
	    Config.LocationSelectLogo.ApplyTo(_locationSelectLogo);
	    Config.TapScreenStartBackground.ApplyTo(_tapScreenStartBackground);
        
	    Config.InGameMap.ApplyTo(_inGameMap);
	    ApplyPrefab(_inGameMap);

	    Config.HomeScreenMap.ApplyTo(_homeScreenMap);
    }

    private void ApplyPrefab(Component modified)
    {
        var prefabInstancea = PrefabUtility.FindPrefabRoot(modified.gameObject);
        var prefabMaster = PrefabUtility.GetPrefabParent(prefabInstancea);
        PrefabUtility.ReplacePrefab(prefabInstancea, prefabMaster);
    }

    private void SetLinks()
	{
        Config.HomeAppDownload.ApplyTo(_homeAppDownload);
	    Config.EndAppDownload.ApplyTo(_endAppDownload);
	    Config.OptionsWebsite.ApplyTo(_optionsWebsite);
	    Config.OptionsFacebook.ApplyTo(_optionsFacebook);
	    Config.OptionsTwitter.ApplyTo(_optionsTwitter);
	}

	private void SetMetadata()
	{
		PlayerSettings.productName = _brandingConfig.Metadata.AppName;
		PlayerSettings.bundleVersion = _brandingConfig.Metadata.Version;
		PlayerSettings.applicationIdentifier = _brandingConfig.Metadata.BundleId;
		var logos = new PlayerSettings.SplashScreenLogo[_brandingConfig.Metadata.SplashScreenLogos.Length];
		for (var i = 0; i < _brandingConfig.Metadata.SplashScreenLogos.Length; i++)
		{
			logos[i].logo = _brandingConfig.Metadata.SplashScreenLogos[i];
		}
		PlayerSettings.SplashScreen.logos = logos;
		PlayerSettings.SplashScreen.background = _brandingConfig.Metadata.SplashScreenBackground;

        GameObject.Find("ContentManager").GetComponent<ContentRequest>().SetUrl(_brandingConfig.Metadata.AuthoringToolUrl);
		GameObject.Find("ContentManager").GetComponent<ContentRequest>().SetFileName(_brandingConfig.Metadata.FileName);
		GameObject.Find("ContentManager").GetComponent<ContentRequest>().SetResourcesFileName(_brandingConfig.Metadata.ResourcesFileName);
    }

    private void SetBrandingObjects()
	{
		foreach (var branding in ObjectsForBranding)
		{
			var active = (branding.IsBrandingObject && UseBrandingObjects) || (!branding.IsBrandingObject && !UseBrandingObjects);
			
			branding.Obj.SetActive(active);
		}
	}

#endif
	public Color GetColor(ColorTheme theme)
	{
		// Check if config is overriding the color
		var overrideColor = Config.BrandingColors.FirstOrDefault(c => c.Theme == theme);
		return overrideColor != null ? overrideColor.Color : _brandingColors.Themes.Find(c => c.Theme == theme).Color;
	}
}
