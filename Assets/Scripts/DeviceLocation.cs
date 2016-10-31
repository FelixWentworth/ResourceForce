using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeviceLocation : MonoBehaviour {

    public Location loc;
    public GameObject startScreen;

    public Button EnglishButton;
    public Button DutchButton;
    public Button GreekButton;
    public Button SpanishButton;

    public static bool shouldOverrideLanguage = false;
    public static SystemLanguage overrideLanguage = SystemLanguage.English;

    public WarningBox WarningBox;
    public Text TitleText;
    public RectTransform grid;

    private enum LanguageMapping { Preston = 1, Belfast = 2, Nicosia = 3, Groningen = 4, Valencia = 5 }
    private LocationConfig _config;
    private Image[] _images;
    private int _locationIndex;
    private GridLayoutGroup _gridLayout;

    void Awake()
    {
        _gridLayout = grid.GetComponent<GridLayoutGroup>();
        _config = this.GetComponent<LocationConfig>();
        _locationIndex = 0;
        _images = new []{
            EnglishButton.transform.GetChild(0).GetComponent<Image>(),
            DutchButton.transform.GetChild(0).GetComponent<Image>(),
            GreekButton.transform.GetChild(0).GetComponent<Image>(),
            SpanishButton.transform.GetChild(0).GetComponent<Image>()
        };

        SetButtonClicks();

        UpdateLanguagesAvailable();
    }

    private void SetButtonClicks()
    {
        EnglishButton.onClick.AddListener(EnglishSelected);
        DutchButton.onClick.AddListener(DutchSelected);
        GreekButton.onClick.AddListener(GreekSelected);
        SpanishButton.onClick.AddListener(SpanishSelected);
    }

    public void SetLocation(Dropdown dropdown)
    {
        int value = dropdown.value;

        UpdateLanguagesAvailable(value);

        // check a location has been set
        if (value == 0)
            return;

        loc.SetFilePath(value-1);
        
    }

    private void EnglishSelected()
    {
        shouldOverrideLanguage = true;
        overrideLanguage = SystemLanguage.English;
        UpdateSelected(EnglishButton.transform);
    }

    private void SpanishSelected()
    {
        shouldOverrideLanguage = true;
        overrideLanguage = SystemLanguage.Spanish;
        UpdateSelected(SpanishButton.transform);
    }

    private void DutchSelected()
    {
        shouldOverrideLanguage = true;
        overrideLanguage = SystemLanguage.Dutch;
        UpdateSelected(DutchButton.transform);
    }

    private void GreekSelected()
    {
        shouldOverrideLanguage = true;
        overrideLanguage = SystemLanguage.Greek;
        UpdateSelected(GreekButton.transform);
    }

    public void OKSelected()
    {
        if (_locationIndex == 0)
        {
            // Notify the player to select a location
            StartCoroutine(WarningBox.ShowWarning(Localization.Get("WARNING_TEXT_LOCATION"), 2f));

        } 
        if (!IsAnyLanguageSelected())
        {
            // notify the player they need to select a language
            StartCoroutine(WarningBox.ShowWarning(Localization.Get("WARNING_TEXT_LANGUAGE"), 2f));
        }
        else
        {
            //player has successfully set the location, no need to show the popup on load anymore
            PlayerPrefs.SetInt("SetLocation", 1);

            //load the scene again to reload data
            SceneManager.LoadScene(0);
        }
    }
    private void UpdateLanguagesAvailable(int locationSelected = 0)
    {
        var englishParent = EnglishButton.transform.parent.gameObject;
        var dutchParent = DutchButton.transform.parent.gameObject;
        var greekParent = GreekButton.transform.parent.gameObject;
        var spanishParent = SpanishButton.transform.parent.gameObject;

        _locationIndex = locationSelected;
        foreach (var image in _images)
        {
            image.enabled = false;
        }
        if (locationSelected == 0)
        {
            // no language chosen, disable all language choices
            englishParent.SetActive(false);
            dutchParent.SetActive(false);
            greekParent.SetActive(false);
            spanishParent.SetActive(false);
            TitleText.gameObject.SetActive(false);
        }
        else
        {
            switch ((LanguageMapping)locationSelected)
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
            UpdateSelected(EnglishButton.transform);
            TitleText.gameObject.SetActive(true);
        }
    }

    private void UpdateSelected(Transform selectedButton)
    {
        foreach (var image in _images)
        {
            image.enabled = image.transform.parent == selectedButton;
        }
    }

    private bool IsAnyLanguageSelected()
    {
        // Check at least one of the buttons which is interactable has an active selected image
        var languageSelected = false;
        foreach (var image in _images)
        {
            languageSelected |= image.enabled && image.transform.parent.GetComponent<Button>().interactable;
        }
        return languageSelected;
    }
}
