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

    [SerializeField] private string _filePath;

    private long _serialNumber = 10;

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

    void Start()
    {
        StartCoroutine(FetchNewContent());
    }

    private IEnumerator FetchNewContent()
    {
        _currentState = State.Initializing;
        var www = new WWW(_api + _extension);

        _currentState = State.Retrieving;
        yield return www;

        if (string.IsNullOrEmpty(www.text))
        {
            Debug.Log("Unable to find any new content");
        }
        else
        {
            var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(www.text);

            _currentState = State.Updating;
            // Get existing content
            var contentString = "";
            using (var stream = new StreamReader(_filePath))
            {
                contentString = stream.ReadToEnd();
            }

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

            WriteToFile(contentString);

            // update the scenario number
            foreach (var scenario in scenarios)
            {
                if (scenario.SerialNumber > _serialNumber)
                {
                    // TODO Save serial number
                    _serialNumber = scenario.SerialNumber;
                }
            }
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

    private void WriteToFile(string content)
    {
        if (content == "")
        {
            return;
        }
        using (var stream = new StreamWriter(_filePath, false))
        {
            stream.Write(content);
        }
    }

    public List<Scenario> GetScenarios(string location, string language)
    {
        using (var stream = new StreamReader(_filePath))
        {
            var scenarios = stream.ReadToEnd();

            var scenarioJson = JsonConvert.DeserializeObject<List<Scenario>>(scenarios);

            return scenarioJson.Where(s => s.Language == language && s.Location == location).ToList();
        }
    }
}
