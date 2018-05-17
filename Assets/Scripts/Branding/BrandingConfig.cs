using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BrandingConfig : ScriptableObject
{
    public Metadata Metadata;

    [Header("Regions")]
    public Region[] Regions;

    [Header("Localization Overrides")]
    public LocalizationOverride[] LocalizationOverrides;

    [Header("Element Images")]
    public ElementImage StartScreenLogo;
    public ElementImage GameLogo;
    public ElementImage CitizenButtonIcon;
    public ElementImage EndGameLinkIcon;
    public ElementImage ExternalLinkWebsite;
    public ElementImage ExternalLinkFacebook;
    public ElementImage ExternalLinkTwitter;
    public ElementImage LocationSelectLogo;
    public ElementImage TapScreenStartBackground;
    public ElementImage InGameMap;
    public ElementImage HomeScreenMap;

    [Header("External Links")]
    public ExternalLink HomeAppDownload;
    public ExternalLink EndAppDownload;
    public ExternalLink OptionsWebsite;
    public ExternalLink OptionsFacebook;
    public ExternalLink OptionsTwitter;

	[Header("Color Palette")]
	public List<ColorMapping> BrandingColors;
}

[Serializable]
public struct LocalizationOverride
{
    public string Key;
    public string EnGb;
    public string Nl;
    public string El;
    public string Es;
}

[Serializable]
public struct ExternalLink
{
    public string Url;
    public bool SendEvent;
    public string EventName;

    public void ApplyTo(ButtonLink applyTo)
    {
        applyTo.Url = Url;
        applyTo.EventName = EventName;
        applyTo.SendEvent = SendEvent;
    }
}

[Serializable]
public struct ElementImage
{
    public Sprite NewSprite;
    public Color Tint;

    public void ApplyTo(Image applyTo)
    {
        applyTo.sprite = NewSprite;
        applyTo.color = Tint;
    }
}

[Serializable]
public struct Metadata
{
    public string AppName;
    public string BundleId;
    public string Version;
    public string AuthoringToolUrl;
    public string FileName;
    public string ResourcesFileName;
    public Sprite[] SplashScreenLogos;
	public Sprite SplashScreenBackground;
}

[Serializable]
public class ColorMapping
{
	[HideInInspector] public string Name; // used for unity to name each element rather than naming as 'Element n'
	public BrandingManager.ColorTheme Theme;
	public Color Color;

	public ColorMapping(BrandingManager.ColorTheme theme)
	{
		Theme = theme;
		Name = theme.ToString();
	}
}

[Serializable]
public struct Region
{
	public string Location;
	public AvailableLanguage[] Languages;
}

[Serializable]
public struct AvailableLanguage
{
	[Tooltip("What the player sees in game")]
	public string LanguageText;
	public SystemLanguage Language;
}

