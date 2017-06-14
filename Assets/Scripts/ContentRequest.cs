using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ContentRequest : MonoBehaviour
{

    public string HostName;
    public string Port;

    private int _serialNumber = 10;

    private string _api
    {
        get { return "http://" + HostName + ":" + Port + "/api"; }
    }

    private string _extension
    {
        get { return "/scenario/new/" + _serialNumber; }
    }

    void Start()
    {
        StartCoroutine(FetchNewContent());
    }

    private IEnumerator FetchNewContent()
    {
        var www = new WWW(_api + _extension);

        yield return www;

        if (www.text == null)
        {
            Debug.Log("Unable to find any new content");
        }

        // Remove object brackets []
        var body = www.text.Substring(1, www.text.Length - 2);

        var scenarios = JsonConvert.DeserializeObject<Scenario>(body);

        Debug.Log(scenarios);
    }
}
