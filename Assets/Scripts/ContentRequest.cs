﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContentRequest : MonoBehaviour
{
    public GameObject SelectLocationScreen;

    [SerializeField] private string _hostName;

    [SerializeField] private string _fileName;
	[SerializeField]private string _brandedFileName = "";
	private string FileName
	{
		get { return _brandedFileName == "" ? _fileName : _brandedFileName; }
	}

    [SerializeField] private string _resourcesFileName;
	[SerializeField] private string _brandedResourcesFileName = "";
	private string ResourcesFileName
	{
		get { return _brandedResourcesFileName == "" ? _resourcesFileName : _brandedResourcesFileName; }
	}

	private static List<Scenario> _allScenarios = new List<Scenario>();
    private string _contentString;
    private bool _contentFound;

	public GameObject CancelButton;

    private long _serialNumber
    {
        get
        {
            // ------------------------
            // TESTING 
            //return 0;
            // ------------------------
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
    }

    private enum State
    {
        Initializing = 0,
        Retrieving,
        Updating,
        Saving,
        Finalizing,
		Error
    };

    private State _currentState;

    private string _api
    {
        get { return _hostName + "/api"; }
    }

    private string _extension
    {
        get { return "/scenario/new/" + _serialNumber; }
    }

	public void SetUrl(string url)
	{
		_hostName = url;
	}

	public void SetFileName(string filename)
	{
		_brandedFileName = filename;
	}

	public void SetResourcesFileName(string filename)
	{
		_brandedResourcesFileName = filename;
	}

	IEnumerator Start()
    {
        if (SelectLocationScreen != null)
        {

            if (!SelectLocationScreen.activeSelf && Location.NumIncidents == 0)
            {
	            StartCoroutine(WaitToActivateCancel(10f));
                yield return GetContent();
            }
        }
    }

	private IEnumerator WaitToActivateCancel(float wait)
	{
		CancelButton.SetActive(false);
		yield return new WaitForSeconds(wait);
		if ((int) _currentState <= 1)
		{
			CancelButton.SetActive(true);
			CancelButton.GetComponentInChildren<Button>().onClick.AddListener(Cancel);
		}
	}

	public void Cancel()
	{
		StopCoroutine("GetContent");
		Failed();
		Loading.LoadingSpinner.StopSpinner("");
		var language = DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English";
		Location.NumIncidents = _allScenarios.Count(s => s.Location == Location.CurrentLocation && s.Language == language);
		Debug.Log(Location.NumIncidents + " Scenarios available");
	}

	public IEnumerator GetContent()
    {
        Loading.LoadingSpinner.StartSpinner(Localization.Get("BASIC_TEXT_RETRIEVING_CONTENT"));
        yield return GetSavedScenarios();
        Loading.LoadingSpinner.StartSpinner(Localization.Get("BASIC_TEXT_CHECKING_NEW_CONTENT"));
		yield return FetchNewContent();
		if (_contentFound)
		{
			yield return Loading.LoadingSpinner.StopSpinner(Localization.Get("BASIC_TEXT_NEW_CONTENT"), 1.5f);
		}
		else
		{
			Loading.LoadingSpinner.StopSpinner("");
		}
		// Set content number
		var language = DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English";
		Location.NumIncidents = _allScenarios.Count(s => s.Location == Location.CurrentLocation && s.Language == language);
		Debug.Log(Location.NumIncidents + " Scenarios available");
	}

    private IEnumerator FetchNewContent()
    {
        _currentState = State.Initializing;
        var www = new WWW(_api + _extension);

        _currentState = State.Retrieving;
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
        else if (string.IsNullOrEmpty(www.text) || www.text == "[]")
        {
            Failed();
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
                yield return WriteToFile(contentString);
            }
            // update the scenario number
            foreach (var scenario in scenarios)
            {
                if (scenario.SerialNumber > _serialNumber)
                {

                    // TODO Save serial number
                    PlayerPrefs.SetString("SerialNumber", scenario.SerialNumber.ToString());

                    Debug.Log("---" + scenario.SerialNumber);
                }
            }

            _currentState = State.Finalizing;
        }
    }

	private void Failed()
	{
		Debug.Log("Unable to find any new content from authoring tool");
		Debug.Log("Current Serial Number: " + _serialNumber);
		_contentFound = false;
		_currentState = State.Error;
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
            // check if the scenario currently exists and therefore has been modified
            var modified = currentScenarios.FirstOrDefault(s => s.Id == newScenario.Id);

            if (modified != null)
            {
                allScenarios.Remove(modified);
                modified.Content = newScenario.Content;
                if (!newScenario.Deleted)
                {
                    allScenarios.Add(modified);
                }
            }
            else
            {
                if (!newScenario.Deleted)
                {
                    allScenarios.Add(newScenario);
                }
            }
        }

        return JsonConvert.SerializeObject(allScenarios);
    }

    private IEnumerator WriteToFile(string content)
    {
        var path = Application.persistentDataPath + "/" + FileName;

        //if (Application.platform == RuntimePlatform.WindowsEditor ||
        //    Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WSAPlayerX86 ||
        //    Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.LinuxPlayer)
        //{
        //    path = "file:///" + path;
        //}
        //else if (Application.platform == RuntimePlatform.Android)
        //{
        //    path = "jar:file://" + path;
        //}

        var www = new WWW(path);

        yield return www;
        if (www.text != null)
        {
            var newtext = www.text + content;
            Debug.Log(path);
            
            File.WriteAllText(path, newtext);

            using (var sw = new StreamWriter(path))
            {
                sw.Write(newtext);
            }
        }
    }

    public List<Scenario> GetScenarios(string location, string language)
    {
	    if (_allScenarios.Count == 0)
	    {
			// Fall back to the basic scenarios
		    GetResourcesScenario();
	    }
	    return _allScenarios.Where(s => s.Language == language && (s.Location == location || s.Location == "Any")).ToList();
    }

    public void GetResourcesScenario()
    {
        var textAsset = Resources.Load(ResourcesFileName) as TextAsset;
        if (textAsset == null)
        {
            Debug.LogError("Resources file not found");
            return;
        }
        _contentString = textAsset.text;
        var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(textAsset.text);
        _allScenarios = scenarios;
    }

    private IEnumerator GetSavedScenarios()
    {
        var path = Application.persistentDataPath + "/" + FileName;

        //if (Application.platform == RuntimePlatform.WindowsEditor ||
        //    Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WSAPlayerX86 ||
        //    Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.LinuxPlayer)
        //{
            path = "file:///" + path;
        //}
        //else if (Application.platform == RuntimePlatform.Android)
        //{
        //    path = "jar:file://" + path;
        //}

        var www = new WWW(path);
        yield return www;
        if (string.IsNullOrEmpty(www.text))
        {
            // write the data from Resources to the persistent data file
            GetResourcesScenario();
            var contentString = JsonConvert.SerializeObject(_allScenarios);
            yield return WriteToFile(contentString);
        }
        else
        {
            _contentString = www.text;
            var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(www.text);
            _allScenarios = scenarios;
        }
    }
}
