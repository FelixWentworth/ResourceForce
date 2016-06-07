﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;

public class Localization : MonoBehaviour {

    static Dictionary<string, string> localizationDict = new Dictionary<string, string>();

    public string Key;
    public bool toUpper;

    public static string filePath = "StringLocalizations";
    static TextAsset jsonTextAsset;
    public static int languageIndex = 1;

    void Awake()
    {
        SetLanguageIndex();
        ConvertJsonToDict();
    }
    static void ConvertJsonToDict()
    {
        jsonTextAsset = Resources.Load(filePath) as TextAsset;

        var N = JSON.Parse(jsonTextAsset.text);

        for (int i = 0; N[i] != null; i++)
        {
            //go through the list and add the strings to the dictionary
            string _key = N[i][0].ToString();
            _key = _key.Replace("\"", "");
            string _value = N[i][languageIndex].ToString();
            _value = _value.Replace("\"", "");
            localizationDict[_key] = _value;
        }
    }
    void Start()
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
        string txt = "";
        key = key.ToUpper();

        localizationDict.TryGetValue(key, out txt);

        //new line character in spreadsheet is *n*
        if (txt == null)
            return key;
        txt = txt.Replace("*n*", "\n");
        txt = txt.Replace("*2n*", "\n\n");
        return txt;
    }
    public static void SetLanguageIndex()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                languageIndex = 1;
                break;
            case SystemLanguage.Dutch:
                languageIndex = 2;
                break;
            case SystemLanguage.Greek:
                languageIndex = 3;
                break;
            case SystemLanguage.Spanish:
                languageIndex = 4;
                break;
        }
    }

}