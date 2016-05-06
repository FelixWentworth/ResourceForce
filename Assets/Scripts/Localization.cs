using UnityEngine;
using System.Collections;
using SimpleJSON;
using UnityEngine.UI;

public class Localization : MonoBehaviour {

    public string Key;
    public bool toUpper;
    void Start()
    {
        Text _text = this.GetComponent<Text>();
        if (_text == null)
            Debug.LogError("Localization script could not find Text component");
        _text.text = Get(Key, null);
        if (toUpper)
            _text.text = _text.text.ToUpper();
    }

    public static string filePath = "incidentSpreadsheet";
    static TextAsset jsonTextAsset;
    public static int languageIndex = 1;
    public static string Get(string key, JSONNode N)
    {
        string txt = "";
        key = key.ToUpper();
        ////get the text asset ready to search through
        if (N == null)
        {
            jsonTextAsset = Resources.Load(filePath) as TextAsset;

            N = JSON.Parse(jsonTextAsset.text);
        }
        //set the language index based on the system language, we can do this every time in case the language changes
        SetLanguageIndex();

        for (int i = 0; N[i]!= null; i++)
        {
            if (N[i][0].ToString().Contains(key))
            {
                txt = N[i][languageIndex]; //TODO make the 1 match to the localisation of the device
            }
        }
        return txt;
    }
    public static string GetRandomStringForType(string type)
    {
        //get the text asset ready to search through
        jsonTextAsset = Resources.Load(filePath) as TextAsset;

        var N = JSON.Parse(jsonTextAsset.text);
        //get language index 
        SetLanguageIndex();

        //now search through our file for our type
        int maxTexts = 0;
        for (int i = 0; N[i]!= null; i++)
        {
            if (N[i][0].ToString().Contains(type.ToUpper() + "_LENGTH"))
            {
                maxTexts = int.Parse(N[i][languageIndex]);
                break;
            }
        }

        //now set a random string to get from the max number
        int rand = UnityEngine.Random.Range(1, maxTexts + 1);

        return Get(type.ToUpper() + "_TEXT_" + rand, N);
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
