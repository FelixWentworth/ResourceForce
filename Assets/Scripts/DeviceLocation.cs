using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeviceLocation : MonoBehaviour {

    public Location loc;
    public GameObject startScreen;
    public GameObject DebugSelectLanguageGameObject;

    public static bool shouldOverrideIncidentLanguage = false;
    public static SystemLanguage overrideLanguage = SystemLanguage.English;

    void Awake()
    {
        DebugSelectLanguageGameObject.SetActive(false);
    }
    public void SetLocation(Dropdown dropdown)
    {
        int value = dropdown.value;

        // check a location has been set
        if (value == 0)
            return;

        loc.SetFilePath(dropdown.value - 1);
        if (value == 5) // Valencia selected
        {
            DebugSelectLanguageGameObject.SetActive(true);
        }
        else
        {

            startScreen.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    public void EnglishSelected()
    {
        shouldOverrideIncidentLanguage = false;
        overrideLanguage = SystemLanguage.English;
        startScreen.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void SpanishSelected()
    {
        shouldOverrideIncidentLanguage = true;
        overrideLanguage = SystemLanguage.Spanish;
        startScreen.SetActive(true);
        this.gameObject.SetActive(false);
    }

}
