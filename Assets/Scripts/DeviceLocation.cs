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

	public GameObject LangAndLocGameObject;
	public GameObject LangGameObject;

    public Button EnglishButton;
    public Button DutchButton;
    public Button GreekButton;
    public Button SpanishButton;

    public static bool shouldOverrideLanguage;
    public static SystemLanguage overrideLanguage = SystemLanguage.English;

    public WarningBox WarningBox;
    public Text TitleText;
    public RectTransform grid;

    private enum LanguageMapping { Preston = 1, Belfast = 2, Nicosia = 3, Groningen = 4, Valencia = 5 }
    private static readonly Dictionary<SystemLanguage, CultureInfo> LanguageCultureInfoMappings = new Dictionary<SystemLanguage, CultureInfo>
    {
        { SystemLanguage.English, new CultureInfo("en-gb") },
        { SystemLanguage.Dutch, new CultureInfo("nl") },
        { SystemLanguage.Spanish, new CultureInfo("es") },
        { SystemLanguage.Greek, new CultureInfo("el") }
    };

    private LocationConfig _config;
    private int _locationIndex;
    private GridLayoutGroup _gridLayout;

    private Dropdown _dropdown;

	private Transform _languageButtonPanel;

    void Start()
    {
        _dropdown = GetComponentInChildren<Dropdown>();
        _gridLayout = grid.GetComponent<GridLayoutGroup>();
        _config = this.GetComponent<LocationConfig>();
        _locationIndex = 0;

        SetButtonClicks();

		// By default the language and location object should be active
		LangAndLocGameObject.SetActive(true);
	    LangGameObject.SetActive(false);

	    if (!BrandingManager.Instance.UseManager)
	    {
		    UpdateLanguagesAvailable(-1);
	    }
    }

    private void SetButtonClicks()
    {
        EnglishButton.onClick.AddListener(EnglishSelected);
        DutchButton.onClick.AddListener(DutchSelected);
        GreekButton.onClick.AddListener(GreekSelected);
        SpanishButton.onClick.AddListener(SpanishSelected);
    }

    void Update()
    {
	    if (!BrandingManager.Instance.UseManager)
	    {
		    if (_dropdown.transform.childCount != 4)
		    {
			    SetToggleToButtons();
		    }
	    }
    }

    private void SetToggleToButtons()
    {
        var toggles = GetComponentsInChildren<Toggle>();
        foreach (var toggle in toggles)
        {
            if (toggle.gameObject.GetComponent<EventTrigger>() != null)
            {
                // We have already set up this trigger
                continue;
            }
            var trigger = toggle.gameObject.AddComponent<EventTrigger>();
            var clicked = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
            clicked.callback.AddListener((eventData) => OnToggleClicked(toggle.gameObject.name));
            trigger.triggers.Add(clicked);
        }
    }

    private void OnToggleClicked(string name)
    {
        var itemNumber = name.Substring(5, 1);

        var index = int.Parse(itemNumber);
        _dropdown.Hide();
        _dropdown.value = -1;
        _dropdown.value = index;
        SetLocation(index);
        
    }

    public void SetLocation(Dropdown dropdown)
    {
        SetLocation(dropdown.value);
    }

    private void SetLocation(int value)
    {
        UpdateLanguagesAvailable(value);
        loc.SetSite(value);
    }

	public void SetRequiredSelection(bool languageOnly)
	{
		LangAndLocGameObject.SetActive(!languageOnly);
		LangGameObject.SetActive(languageOnly);
	}

	public void SetLanguages(SystemLanguage[] supported)
	{
		if (supported.Length == 1)
		{
			// get language to be used
			var language = supported[0];
			shouldOverrideLanguage = true;
			overrideLanguage = language;
			LanguageSelectd();
			return;
		}
		SetButtonPanel();
		EnglishButton.gameObject.SetActive(supported.Any(l => l == SystemLanguage.English));
		DutchButton.gameObject.SetActive(supported.Any(l => l == SystemLanguage.Dutch));
		SpanishButton.gameObject.SetActive(supported.Any(l => l == SystemLanguage.Spanish));
		GreekButton.gameObject.SetActive(supported.Any(l => l == SystemLanguage.Greek));
	}

	private void SetButtonPanel()
	{
		var activeObj = LangAndLocGameObject.activeSelf ? LangAndLocGameObject : LangGameObject;
		Debug.Log(activeObj.name);
		_languageButtonPanel = activeObj.GetComponentInChildren<LayoutGroup>().transform;

		Debug.Log(_languageButtonPanel);

		var englishParent = EnglishButton.transform.parent.transform;
		var dutchParent = DutchButton.transform.parent.transform;
		var greekParent = GreekButton.transform.parent.transform;
		var spanishParent = SpanishButton.transform.parent.transform;

		englishParent.parent = _languageButtonPanel;
		dutchParent.parent = _languageButtonPanel;
		greekParent.parent = _languageButtonPanel;
		spanishParent.parent = _languageButtonPanel;
	}

    private void EnglishSelected()
    {
        shouldOverrideLanguage = true;
        overrideLanguage = SystemLanguage.English;
        LanguageSelectd();
    }

    private void SpanishSelected()
    {
        shouldOverrideLanguage = true;
        overrideLanguage = SystemLanguage.Spanish;
        LanguageSelectd();
    }

    private void DutchSelected()
    {
        shouldOverrideLanguage = true;
        overrideLanguage = SystemLanguage.Dutch;
        LanguageSelectd();
    }

    private void GreekSelected()
    {
        shouldOverrideLanguage = true;
        overrideLanguage = SystemLanguage.Greek;
        LanguageSelectd();
    }

    public void LanguageSelectd()
    {
        // Set num incidents to 0 to recalculate the number available on start
        Location.NumIncidents = 0;

        if (_locationIndex == 0 && !BrandingManager.Instance.UseManager)
        {
            // Notify the player to select a location
            StartCoroutine(WarningBox.ShowWarning(Localization.Get("WARNING_TEXT_LOCATION"), Color.yellow, true));
        } 
        else
        {
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
    }
    private void UpdateLanguagesAvailable(int locationSelected = 0)
    {
	    SetButtonPanel();
		var englishParent = EnglishButton.transform.parent.gameObject;
        var dutchParent = DutchButton.transform.parent.gameObject;
        var greekParent = GreekButton.transform.parent.gameObject;
        var spanishParent = SpanishButton.transform.parent.gameObject;

        if (locationSelected == -1)
        {
            englishParent.SetActive(false);
            dutchParent.SetActive(false);
            greekParent.SetActive(false);
            spanishParent.SetActive(false);

            TitleText.gameObject.SetActive(false);
            return;
        }

        _locationIndex = locationSelected + 1;
        switch ((LanguageMapping)_locationIndex)
        {
            case LanguageMapping.Preston:
                englishParent.SetActive(_config.English_Preston);
                dutchParent.SetActive(_config.Dutch_Preston);
                greekParent.SetActive(_config.Greek_Preston);
                spanishParent.SetActive(_config.Spanish_Preston);
                break;
            case LanguageMapping.Belfast:
                englishParent.SetActive(_config.English_Belfast);
                dutchParent.SetActive(_config.Dutch_Belfast);
                greekParent.SetActive(_config.Greek_Belfast);
                spanishParent.SetActive(_config.Spanish_Belfast);
                break;
            case LanguageMapping.Nicosia:
                englishParent.SetActive(_config.English_Nicosia);
                dutchParent.SetActive(_config.Dutch_Nicosia);
                greekParent.SetActive(_config.Greek_Nicosia);
                spanishParent.SetActive(_config.Spanish_Nicosia);
                break;
            case LanguageMapping.Groningen:
                englishParent.SetActive(_config.English_Groningen);
                dutchParent.SetActive(_config.Dutch_Groningen);
                greekParent.SetActive(_config.Greek_Groningen);
                spanishParent.SetActive(_config.Spanish_Groningen);
                break;
            case LanguageMapping.Valencia:
                englishParent.SetActive(_config.English_Valencia);
                dutchParent.SetActive(_config.Dutch_Valencia);
                greekParent.SetActive(_config.Greek_Valencia);
                spanishParent.SetActive(_config.Spanish_Valencia);
                break;
        }
        _gridLayout.cellSize = new Vector2(grid.rect.width/2f, grid.rect.height/2f);
        TitleText.gameObject.SetActive(true);
    }
}
