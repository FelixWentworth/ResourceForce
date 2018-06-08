	using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using PlayGen.Unity.Utilities.Localization;

public class DeviceLocation : MonoBehaviour {

    public Location loc;
    public GameObject startScreen;

	public GameObject LanguageButton;
	private List<GameObject> _languagesCreated = new List<GameObject>();

	public GameObject LangAndLocGameObject;
	public GameObject LangGameObject;

	[SerializeField] private GameObject _languageSelectPanel;

    public static bool shouldOverrideLanguage;
    public static SystemLanguage overrideLanguage = SystemLanguage.English;

    public WarningBox WarningBox;
    public Text TitleText;
    public RectTransform grid;

    private static readonly Dictionary<SystemLanguage, CultureInfo> LanguageCultureInfoMappings = new Dictionary<SystemLanguage, CultureInfo>
    {
        { SystemLanguage.English, new CultureInfo("en-gb") },
        { SystemLanguage.Dutch, new CultureInfo("nl") },
        { SystemLanguage.Spanish, new CultureInfo("es") },
        { SystemLanguage.Greek, new CultureInfo("el") }
    };

    private Dropdown _dropdown;

	private Transform _languageButtonPanel;

    void Start()
    {
	    ClearLanguageButtons();


		// By default the language and location object should be active
		LangAndLocGameObject.SetActive(true);
	    LangGameObject.SetActive(false);

	    var setLocation = loc.GetLocations().Count > 1;
	    if (setLocation)
	    {
		    _dropdown = GetComponentInChildren<Dropdown>();
		    UpdateLocationsAvailable(loc.GetLocations());
		    _languageSelectPanel.SetActive(true);
		}
	    else
	    {
		    _languageSelectPanel.SetActive(false);
		}
	}

	private void UpdateLocationsAvailable(List<string> locations)
	{
		_dropdown.ClearOptions();
		locations.Insert(0, Localization.Get("BASIC_TEXT_SELECT_LOCATION"));
		_dropdown.AddOptions(locations);
		_dropdown.value = 0;
	}

    public void SetLocation(Dropdown dropdown)
    {
        SetLocation(dropdown.options[dropdown.value].text);
    }

    private void SetLocation(string region)
    {
        loc.SetRegion(region);
	    SetButtonPanel(region, loc.GetLanguages(region));
	}

	public void SetRequiredSelection(bool languageOnly)
	{
		LangAndLocGameObject.SetActive(!languageOnly);
		LangGameObject.SetActive(languageOnly);
	}

	public void SetLanguages(string region, SystemLanguage[] supported)
	{
		if (supported.Length == 1)
		{
			// get language to be used
			var language = supported[0];
			shouldOverrideLanguage = true;
			overrideLanguage = language;
			LanguageSelected(language);
			return;
		}
		SetButtonPanel(region, supported);
	}

	private void SetButtonPanel(string region, SystemLanguage[] languages)
	{
		// first element of dropdown is a not a location optoon
		var showLanguages = _dropdown.value >= 1;
		
		_languageSelectPanel.SetActive(showLanguages);
		if (showLanguages)
		{
			var activeObj = LangAndLocGameObject.activeSelf ? LangAndLocGameObject : LangGameObject;

			_languageButtonPanel = activeObj.GetComponentInChildren<LayoutGroup>().transform;
			ClearLanguageButtons();

			foreach (var language in languages)
			{
				Debug.Log(language);
				var button = Instantiate(LanguageButton, _languageButtonPanel, false);
				button.GetComponentInChildren<Button>().onClick.AddListener(() => LanguageButtonPress(language));
				button.GetComponentInChildren<Text>().text = loc.GetLanguageText(region, language);
				_languagesCreated.Add(button);
			}
			LayoutRebuilder.MarkLayoutForRebuild(_languageButtonPanel.GetComponent<RectTransform>());
		}
	}

	private void LanguageButtonPress(SystemLanguage language)
	{
		shouldOverrideLanguage = true;
		overrideLanguage = language;
		LanguageSelected(language);
	}

  
    public void LanguageSelected(SystemLanguage language)
    {
        // Set num incidents to 0 to recalculate the number available on start
        Location.NumIncidents = 0;

        if (shouldOverrideLanguage)
        {
            var languageCultureInfo = LanguageCultureInfoMappings[overrideLanguage];
            Localization.UpdateLanguage(languageCultureInfo);
        }

        //player has successfully set the location, no need to show the popup on load anymore
        PlayerPrefs.SetInt("SetLocation", 1);

        PlayerPrefs.SetInt("LanguageOverride", shouldOverrideLanguage ? 1 : 0);
        PlayerPrefs.SetInt("LanguageOverrideChosen", (int)overrideLanguage);

        //load the scene again to reload data
        SceneManager.LoadScene(0);
    }

	private void ClearLanguageButtons()
	{
		foreach (var created in _languagesCreated)
		{
			DestroyImmediate(created);
		}
		_languagesCreated.Clear();
	}
    //private void UpdateLanguagesAvailable(SystemLanguage[] languages)
    //{
	   // SetButtonPanel(languages);

    //    //_locationIndex = locationSelected + 1;
        
    //    _gridLayout.cellSize = new Vector2(grid.rect.width/2f, grid.rect.height/2f);
    //    TitleText.gameObject.SetActive(true);
    //}
}
