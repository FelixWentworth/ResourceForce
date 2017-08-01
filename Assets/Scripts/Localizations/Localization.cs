using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;

public class Localization : MonoBehaviour {

    private static Dictionary<string, string> _localizationDict = new Dictionary<string, string>();
    private static Dictionary<string, string> _defaultLocalizationDict = new Dictionary<string, string>();
    public string Key;
    public bool toUpper;

    public static string filePath = "StringLocalizations";
    static TextAsset jsonTextAsset_Scenario;
    static TextAsset jsonTextAsset_Basic;
    public static int languageIndex = 1;

    void Awake()
    {
        if (_localizationDict.Count == 0 || languageIndex != GetLanguageIndex())
        {
            UpdateTextFile();
        }
        if (GetLanguageIndex() != 1) // we can default to english for now if english is not the current language
        {
            _defaultLocalizationDict = ConvertJsonToDict(1);
        }
        else
        {
            _defaultLocalizationDict = null;
        }
    }
    public static void UpdateTextFile()
    {
        //below code not original part of localization package
        filePath = "StringLocalizations" + GameObject.Find("LocationMaster").GetComponent<Location>().GetExtension();
        //above code not original part of localization package
        languageIndex = GetLanguageIndex();
        _localizationDict = ConvertJsonToDict(languageIndex);
    }
    static Dictionary<string, string> ConvertJsonToDict(int index)
    {
        var dict = new Dictionary<string, string>();

        jsonTextAsset_Basic = Resources.Load("StringLocalizations_BasicText") as TextAsset;

        var B = JSON.Parse(jsonTextAsset_Basic.text);

        for (int i = 0; B[i] != null; i++)
        {
            //go through the list and add the strings to the dictionary
            string _key = B[i][0].ToString();
            _key = _key.Replace("\"", "");
            string _value = B[i][languageIndex].ToString();
            _value = _value.Replace("\"", "");
            dict[_key] = _value;
        }

        return dict;
    }
    void OnEnable()
    {
        var text = this.GetComponent<Text>();
        if (text == null)
        {
            Debug.LogError("Localization script could not find Text component attached to this gameObject: " + gameObject.name);
            return;
        }

        text.text = Get(Key);

        if (text.text == "")
        {
            Debug.LogError("Could not find string with key: " + Key);

        }
        if (toUpper)
        {
            text.text = text.text.ToUpper();
        }
    }
    
    public static string Get(string key)
    {
        var txt = "";
        key = key.ToUpper();

        _localizationDict.TryGetValue(key, out txt);
        if (txt == null || txt == "XXXX")
        {
            var language = DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English";
            Debug.LogError("Could not find string with key: \"" + key + " \" in the selected language (" + language + ")\nMake sure the key is correct and does not have spacing at the start or end!");

            // Try to fall back to default library
            if (_defaultLocalizationDict != null)
            {
                _defaultLocalizationDict.TryGetValue(key, out txt);
                if (txt == null)
                {
                    Debug.LogError("Could not find string with key: \"" + key +
                                   " \" in the default language\nMake sure the key is correct and does not have spacing at the start or end!");
                    return key;
                }
            }
            else
            {
                return key;
            }
        }
        //typing \n in excel spreadsheet will format to \\n so we put it back here
        txt = txt.Replace(@"\\n", "\n");
        //same again for tabs
        txt = txt.Replace(@"\\t", "\t");
        return txt;
    }
    public static int GetLanguageIndex()
    {
        var savedOverrideChoice = PlayerPrefs.GetInt("LanguageOverride") == 1;
        var savedOverrideLanguage = PlayerPrefs.GetInt("LanguageOverrideChosen");

        DeviceLocation.shouldOverrideLanguage = savedOverrideChoice;
        DeviceLocation.overrideLanguage = (SystemLanguage)savedOverrideLanguage;

        //override to always use english for beta release 0.2
        if (!DeviceLocation.shouldOverrideLanguage)
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    return 1;
                case SystemLanguage.Dutch:
                    return 2;
                case SystemLanguage.Greek:
                    return 3;
                case SystemLanguage.Spanish:
                    return 4;
                default:
                    return 1;
            }
        }
        else
        {
            switch (DeviceLocation.overrideLanguage)
            {
                case SystemLanguage.English:
                    return 1;
                case SystemLanguage.Dutch:
                    return 2;
                case SystemLanguage.Greek:
                    return 3;
                case SystemLanguage.Spanish:
                    return 4;
                default:
                    return 1;
            }
        }        
    }
    public static string GetLanguageString()
    {
        switch (languageIndex)
        {
            case 1:
                return "English";
            case 2:
                return "Dutch";
            case 3:
                return "Greek";
            case 4:
                return "Spanish";
            default:
                return "English";
        }
    }
}
