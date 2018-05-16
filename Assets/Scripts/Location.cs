using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Location : MonoBehaviour
{
	// Sites that are supported, custom is a specific city. If list is changed, be sure to change call to SetSite in BrandingManager.cs
	public static string Region;

	public static int NumIncidents { get; set; }

	public static string CurrentLocation
	{
		get
		{
			if (BrandingManager.Instance.Config.Regions.Length == 1)
			{
				return BrandingManager.Instance.Config.Regions[0].Location;
			}
			return Region;
		}
	}


	// Use this for initialization
	void Awake()
	{
		var current = PlayerPrefs.GetString("Region");
		SetRegion(current);
	}

	public void SetRegion(string region)
	{
		if (Region == region)
		{
			// No change
			return;
		}
		Region = region;
		PlayerPrefs.SetString("Region", Region);
	}

	/// <summary>
	/// Default language list
	/// </summary>
	/// <returns></returns>
	public SystemLanguage[] GetLanguages()
	{
		return BrandingManager.Instance.Config.Regions[0].Languages;
	}

	public List<string> GetLocations()
	{
		return BrandingManager.Instance.Config.Regions.Select(l => l.Location).ToList();
	}

	/// <summary>
	/// Language list for specified location
	/// </summary>
	/// <param name="region"></param>
	/// <returns></returns>
	public SystemLanguage[] GetLanguages(string region)
	{
		return BrandingManager.Instance.Config.Regions.FirstOrDefault(r => r.Location == region).Languages;
	}
}