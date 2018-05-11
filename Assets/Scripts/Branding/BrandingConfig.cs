using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BrandingConfig : ScriptableObject
{
    [SerializeField] public Metadata Metadata;

    [SerializeField] public SystemLanguage[] Languages;

    [Header("Element Images")]
    [SerializeField] public ElementImage StartScreenLogo;
    [SerializeField] public ElementImage GameLogo;
    [SerializeField] public ElementImage CitizenButtonIcon;
    [SerializeField] public ElementImage EndGameLinkIcon;
    [SerializeField] public ElementImage ExternalLinkWebsite;
    [SerializeField] public ElementImage ExternalLinkFacebook;
    [SerializeField] public ElementImage ExternalLinkTwitter;
    [SerializeField] public ElementImage LocationSelectLogo;
    [SerializeField] public ElementImage TapScreenStartBackground;
    [SerializeField] public ElementImage InGameMap;
    [SerializeField] public ElementImage HomeScreenMap;

    [Header("External Links")]
    [SerializeField] public ExternalLink HomeAppDownload;
    [SerializeField] public ExternalLink EndAppDownload;
    [SerializeField] public ExternalLink OptionsWebsite;
    [SerializeField] public ExternalLink OptionsFacebook;
    [SerializeField] public ExternalLink OptionsTwitter;
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
    public string Location;
    public string FileName;
    public string ResourcesFileName;
    public Sprite[] SplashScreenLogos;
    public string PoliceForceName;
    public string PoliceForceStrapline;
}