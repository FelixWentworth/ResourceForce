using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScenarioTracker : MonoBehaviour {

    /// <summary>
    /// This class keeps a log of scenarios and choices a player has made, this information will be sent with the report bug feature from the pause menu
    /// </summary>

    // The data for each decision we want to track
	public struct History
    {
        public string ScenarioId;
        public string ScenarioIndex;
        public string PlayerDecision;
        public string TestSite;
        public string Language;
    }

    private static List<History> _history = new List<History>();

    /// <summary>
    /// Add a new decision a player has made, should be called on option made button
    /// </summary>
    /// <param name="scenarioId"></param>
    /// <param name="scenarioIndex"></param>
    /// <param name="playerDecision"></param>
    /// <param name="testSite"></param>
    /// <param name="language"></param>
    public static void AddDecision(string scenarioId, string scenarioIndex, string playerDecision, string testSite, string language)
    {
        var element = NewHistoryElement(scenarioId, scenarioIndex, playerDecision, testSite, language);

        _history.Add(element);
    }

    /// <summary>
    /// Clear the saved history data, should be called at the beginning of a new game
    /// </summary>
    public static void ClearHistory()
    {
        _history = new List<History>();
    }

    #region Reporting Scenario Issues

    public static string GetScenarioHistory(string body, string currentScenarioId, string currentScenarioIndex, string currentTestSite, string currentLanguage)
    {
        // before we send the report we want to attach incident after body text

        // Ceate a new History Element
        var currentScenario = NewHistoryElement(currentScenarioId, currentScenarioIndex, "null", currentTestSite,
            currentLanguage);

        var history = GetHistory();
        history.Add(currentScenario);

        var scenarioData = GetJson(history);

        body += "\n\n------------------------------------------------------------------------------------------------------\n" 
            + "Scenario History" + "\n\n" + scenarioData;

        // Send the report
        return body;
    }

    

    #endregion

    /// <summary>
    /// Get list of history elements
    /// </summary>
    /// <param name="numElements">The number of elements that should be returned, from the most recent decision, leave empty for all elements</param>
    /// <returns></returns>
    private static List<History> GetHistory(int numElements = -1)
    {
        if (numElements == -1)
        {
            return _history;
        }
        return _history.Skip(Mathf.Max(0, _history.Count - numElements)).ToList();
    }

    /// <summary>
    /// Creates a new History element
    /// </summary>
    /// <param name="scenarioNum"></param>
    /// <param name="scenarioIndex"></param>
    /// <param name="playerDecision"></param>
    /// <param name="testSite"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    private static History NewHistoryElement(string scenarioId, string scenarioIndex, string playerDecision, string testSite, string language)
    {
        var newElement = new History
        {
            ScenarioId = scenarioId,
            ScenarioIndex = scenarioIndex,
            PlayerDecision = playerDecision,
            TestSite = testSite,
            Language = language
        };

        return newElement;
    }

    /// <summary>
    /// Get a specified list in json format
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private static string GetJson(List<History> list)
    {
        var jsonString = "[";
        foreach (var element in list)
        {
            jsonString += " " + JsonUtility.ToJson(element) + ",";
        }

        jsonString = jsonString.Substring(0, jsonString.Length - 1) + "]";

        return jsonString;
    }
}
