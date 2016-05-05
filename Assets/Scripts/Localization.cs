using UnityEngine;
using System.Collections;
using SimpleJSON;


public class Localization : MonoBehaviour {

    public static string filePath = "incidentSpreadsheet";
    static TextAsset jsonTextAsset;
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
        for (int i = 0; N[i]!= null; i++)
        {
            if (N[i][0].ToString().Contains(key))
            {
                txt = N[i][1]; //TODO make the 1 match to the localisation of the device
            }
        }
        return txt;
    }
    public static string GetRandomStringForType(string type)
    {
        string txt = "";

        //get the text asset ready to search through
        jsonTextAsset = Resources.Load(filePath) as TextAsset;

        var N = JSON.Parse(jsonTextAsset.text);
        //now search through our file for our type
        int maxTexts = 0;
        for (int i = 0; N[i]!= null; i++)
        {
            if (N[i][0].ToString().Contains(type.ToUpper() + "_LENGTH"))
            {
                maxTexts = int.Parse(N[i][1]);
                break;
            }
        }

        //now set a random string to get from the max number
        int rand = UnityEngine.Random.Range(1, maxTexts + 1);

        txt = Get(type.ToUpper() + "_TEXT_" + rand, N);

        return txt;
    }
}
