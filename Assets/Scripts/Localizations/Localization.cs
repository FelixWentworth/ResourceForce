using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;

public class Localization : MonoBehaviour {

    static Dictionary<string, string> localizationDict = new Dictionary<string, string>();

    public string Key;
    public bool toUpper;

    public static string filePath = "StringLocalizations";
    static TextAsset jsonTextAsset_Scenario;
    static TextAsset jsonTextAsset_Basic;
    public static int languageIndex = 1;

    private SystemLanguage defaultLanguage = SystemLanguage.English;

    void Awake()
    {
        UpdateTextFile();
    }
    public static void UpdateTextFile()
    {
        //below code not original part of localization package
        filePath = "StringLocalizations" + GameObject.Find("LocationMaster").GetComponent<Location>().GetExtension();
        //above code not original part of localization package
        languageIndex = GetLanguageIndex();
        ConvertJsonToDict();
    }
    static void ConvertJsonToDict()
    {
        jsonTextAsset_Scenario = Resources.Load(filePath) as TextAsset;
        jsonTextAsset_Basic = Resources.Load("StringLocalizations_BasicText") as TextAsset;
        var N = JSON.Parse(jsonTextAsset_Scenario.text);

        for (int i = 0; N[i] != null; i++)
        {
            //go through the list and add the strings to the dictionary
            string _key = N[i][0].ToString();
            _key = _key.Replace("\"", "");
            string _value = N[i][languageIndex].ToString();
            _value = _value.Replace("\"", "");
            localizationDict[_key] = _value;
        }

        var B = JSON.Parse(jsonTextAsset_Basic.text);

        for (int i = 0; B[i] != null; i++)
        {
            //go through the list and add the strings to the dictionary
            string _key = B[i][0].ToString();
            _key = _key.Replace("\"", "");
            string _value = B[i][languageIndex].ToString();
            _value = _value.Replace("\"", "");
            localizationDict[_key] = _value;
        }

        
    }
    void OnEnable()
    {
        Text _text = this.GetComponent<Text>();
        if (_text == null)
            Debug.LogError("Localization script could not find Text component attached to this gameObject: " + this.gameObject.name);
        _text.text = Get(Key);
        if (_text.text == "")
        {
            Debug.LogError("Could not find string with key: " + Key);
        }
        if (toUpper)
            _text.text = _text.text.ToUpper();
    }
    
    public static string Get(string key)
    {
        var txt = "";
        key = key.ToUpper();
        localizationDict.TryGetValue(key, out txt);
        
        //new line character in spreadsheet is *n*
        if (txt == null)
        {
            return key;
        }
        txt = txt.Replace("*n*", "\n");
        txt = txt.Replace("*2n*", "\n\n");
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
            return 1;
            /*
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
            }
            */
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
}
