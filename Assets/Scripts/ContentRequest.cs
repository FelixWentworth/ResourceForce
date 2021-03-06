﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ContentRequest : MonoBehaviour
{
    public GameObject SelectLocationScreen;

    [SerializeField] private string _hostName;
    [SerializeField] private int _downloadTimeout;
    [SerializeField] private int _minimumScenariosRequired;

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

	public static List<Scenario> Scenarios { get; private set; }

	public GameObject CancelButton;

    private long _serialNumber
    {
        get
        {
            // ------------------------
            // TESTING 
            // return 0;
            // ------------------------
            var serial = PlayerPrefs.GetString("SerialNumber");
            return string.IsNullOrEmpty(serial) 
                ? 0 
                : Convert.ToInt64(serial);
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
            if (!SelectLocationScreen.activeSelf && Location.NumIncidents <= _minimumScenariosRequired)
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

	    Location.NumIncidents = Scenarios.Count;
	    Debug.Log(Location.NumIncidents + " Scenarios available");
    }

	public IEnumerator GetContent()
    {
        var loadedScenarios = new List<Scenario>();

        Loading.LoadingSpinner.StartSpinner(Localization.Get("BASIC_TEXT_RETRIEVING_CONTENT"));
        yield return GetSavedScenarios(loadedScenarios);

        Loading.LoadingSpinner.StartSpinner(Localization.Get("BASIC_TEXT_CHECKING_NEW_CONTENT"));
		yield return FetchNewContent(loadedScenarios);

        var filteredScenarios = FilterScenarios(loadedScenarios);

        if (filteredScenarios.Count(s => s.Enabled && !s.Deleted) >= _minimumScenariosRequired)
		{
			yield return Loading.LoadingSpinner.StopSpinner(Localization.Get("BASIC_TEXT_NEW_CONTENT"), 1.5f);
		}
		else
		{
			if (filteredScenarios.Any())
			{
				Debug.LogWarning("Not Enough Scenarios valid through web tool to play the game with custom content alone");
			}
			// No need to tell players nothing was found, quit quietly
			Debug.Log("Loading fallback content from Resources.");
            GetResourcesScenario(filteredScenarios);
		    
            Loading.LoadingSpinner.StopSpinner("");
        }

        // Set content number
        Location.NumIncidents = filteredScenarios.Count;
        Debug.Log(Location.NumIncidents + " Scenarios available");

        Scenarios = filteredScenarios;
    }

    private List<Scenario> FilterScenarios(List<Scenario> unfilteredScenarios)
    {
        var language = DeviceLocation.shouldOverrideLanguage ? DeviceLocation.overrideLanguage.ToString() : "English";
        var validScenarios = unfilteredScenarios.Where(s => s.IsRegionMatch(Location.CurrentLocation) && s.Language == language && s.Enabled).ToList();
        return validScenarios;
    }
    
    private IEnumerator FetchNewContent(List<Scenario> loadedScenarios)
    {
        _currentState = State.Initializing;
        var www = new WWW(_api + _extension);

        _currentState = State.Retrieving;

        var elapsedTime = 0f;
        while (!www.isDone && elapsedTime < _downloadTimeout)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!www.isDone)
        {
            www.Dispose();
            Debug.LogError("Request timed out.");
        }
        else if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
        else if (string.IsNullOrEmpty(www.text) || www.text == "[]")
        {
            Failed();
        }
        else
        {
            var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(www.text);

            _currentState = State.Updating;

            string allScenariosContent;

            // Update any scenarios that may have been edited
            if (loadedScenarios.Any())
            {
                allScenariosContent = UpdateExistingContent(loadedScenarios, scenarios);
            }
            else
            {
                allScenariosContent = www.text;
                loadedScenarios.AddRange(scenarios);
            }

            _currentState = State.Saving;

            if (!string.IsNullOrEmpty(allScenariosContent))
            {
                yield return WriteToFile(allScenariosContent);
            }

            // update the scenario number
            var maxSerial = scenarios.Max(s => s.SerialNumber);
            if (maxSerial > _serialNumber)
            {
                PlayerPrefs.SetString("SerialNumber", maxSerial.ToString());
                Debug.Log("Saved new Max serial number: " + maxSerial);
            }

            _currentState = State.Finalizing;
        }
    }

	private void Failed()
	{
		Debug.Log("Unable to find any new content from authoring tool");
		Debug.Log("Current Serial Number: " + _serialNumber);
		_currentState = State.Error;
	}

    private string UpdateExistingContent(List<Scenario> currentScenarios, List<Scenario> newScenarios)
    {
        if (newScenarios == null)
        {
            return "";
        }

        foreach (var newScenario in newScenarios)
        {
            // check if the scenario currently exists and therefore has been modified
            var modified = currentScenarios.FirstOrDefault(s => s.Id == newScenario.Id);

            if (modified != null)
            {
                currentScenarios.Remove(modified);
                modified.Content = newScenario.Content;
                if (!newScenario.Deleted)
                {
                    currentScenarios.Add(modified);
                }
            }
            else
            {
                if (!newScenario.Deleted)
                {
                    currentScenarios.Add(newScenario);
                }
            }
        }

        return JsonConvert.SerializeObject(currentScenarios);
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

    public void GetResourcesScenario()
    {
        GetResourcesScenario(new List<Scenario>());
    }

    public void GetResourcesScenario(List<Scenario> loadedScenarios)
    {
        var textAsset = Resources.Load(ResourcesFileName) as TextAsset;
        if (textAsset == null)
        {
            Debug.LogError("Resources file not found");
            return;
        }
        var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(textAsset.text);
	    var filteredScenarios = FilterScenarios(scenarios);
		loadedScenarios.AddRange(filteredScenarios);
    }

    private IEnumerator GetSavedScenarios(List<Scenario> loadedScenarios)
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
        if (!string.IsNullOrEmpty(www.text))
        {
            var scenarios = JsonConvert.DeserializeObject<List<Scenario>>(www.text);
            loadedScenarios.AddRange(scenarios);
        }
        else
        {
            Debug.Log("No saved scenarios to load.");
        }
    }
}
