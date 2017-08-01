using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ContentRequest : MonoBehaviour
{

    [SerializeField] private string _hostName;
    [SerializeField] private string _port;

    [SerializeField] private string _fileName;

    private List<Scenario> _allScenarios = new List<Scenario>();
    private string _contentString;
    private bool _contentFound;

    private long _serialNumber
    {
        get
        {
            var serial = PlayerPrefs.GetString("SerialNumber");
            if (serial == "")
            {
                return 0;
            }
            else
            {
                return Convert.ToInt64(serial);
            }
        }
        set { }
    }

    private enum State
    {
        Initializing = 0,
        Retrieving,
        Updating,
        Saving,
        Finalizing
    };

    private State _currentState;

    private string _api
    {
        get { return "http://" + _hostName + ":" + _port + "/api"; }
    }

    private string _extension
    {
        get { return "/scenario/new/" + _serialNumber; }
    }

    IEnumerator Start()
    {
        Loading.LoadingSpinner.StartSpinner(Localization.Get("BASIC_TEXT_RETRIEVING_CONTENT"));
        yield return GetStreamingAssetsScenario();
        Loading.LoadingSpinner.StartSpinner(Localization.Get("BASIC_TEXT_CHECKING_NEW_CONTENT"));
        yield return FetchNewContent();
        if (_contentFound)
        {
            yield return Loading.LoadingSpinner.StopSpinner(Localization.Get("BASIC_TEXT_NEW_CONTENT"), 1.5f);
        }
        else
        {
            yield return Loading.LoadingSpinner.StopSpinner(Localization.Get("BASIC_TEXT_NO_CONTENT"), 1.5f);
        }
    }

    private IEnumerator FetchNewContent()
    {
        _currentState = State.Initializing;
        var www = new WWW(_api + _extension);

        _currentState = State.Retrieving;
        yield return www;

        if (string.IsNullOrEmpty(www.text))
        {
            Debug.Log("Unable to find any new content from authoring tool");
            _contentFound = false;
        }
        else
        {
            _contentFound = true;
            var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(www.text);

            _currentState = State.Updating;

            var contentString = _contentString;

            // Update any scenarios that may have been edited
            if (contentString != "")
            {
                var existingScenarios = JsonConvert.DeserializeObject<List<Scenario>>(contentString);

                contentString = UpdateExistingContent(existingScenarios, scenarios);

            }
            else
            {
                contentString = www.text;
            }

            _currentState = State.Saving;

            if (contentString != "")
            {
                StartCoroutine(WriteToFile(contentString));
            }
            // update the scenario number
            foreach (var scenario in scenarios)
            {
                if (scenario.SerialNumber > _serialNumber)
                {
                    // TODO Save serial number
                    _serialNumber = scenario.SerialNumber;
                }
            }

            PlayerPrefs.SetString("SerialNumber", _serialNumber.ToString());

            _currentState = State.Finalizing;
        }
    }

    private string UpdateExistingContent(List<Scenario> currentScenarios, List<Scenario> newScenarios)
    {
        if (newScenarios == null)
        {
            return "";
        }

        var allScenarios = currentScenarios;
        foreach (var newScenario in newScenarios)
        {
            var modified = currentScenarios.FirstOrDefault(s => s.Id == newScenario.Id);

            if (modified != null)
            {
                allScenarios.Remove(modified);
                modified.Content = newScenario.Content;
            }

            allScenarios.Add(newScenario);
        }

        return JsonConvert.SerializeObject(allScenarios);
    }

    private IEnumerator WriteToFile(string content)
    {
        var path = Application.streamingAssetsPath + "/" + _fileName;

        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WSAPlayerX86 ||
            Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.LinuxPlayer)
        {
            path = "file:///" + path;
        }

        var www = new WWW(path);

        yield return www;
        if (www.text != null)
        {
            var newtext = www.text + content;
            using (var sw = new StreamWriter(Application.streamingAssetsPath + "/" + _fileName))
            {
                sw.Write(newtext);
            }
        }
    }

    public List<Scenario> GetScenarios(string location, string language)
    {
        if (_allScenarios.Count == 0)
        {
            StartCoroutine(GetStreamingAssetsScenario());
            return null;
        }
        else
        {
            return _allScenarios.Where(s => s.Language == language && s.Location == location).ToList();
        }
    }


    private IEnumerator GetStreamingAssetsScenario()
    {
        var path = Application.streamingAssetsPath + "/" + _fileName;

        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WSAPlayerX86 ||
            Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.LinuxPlayer)
        {
            path = "file:///" + path;
        }

        var www = new WWW(path);

        yield return www;
        if (www.text != null)
        {
            _contentString = www.text;
            var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(www.text);
            _allScenarios = scenarios;
        }
    }
}
