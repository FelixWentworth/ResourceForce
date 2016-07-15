using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CreateScenarioWindow : EditorWindow
{
    enum Locations { Preston=0, Belfast=1, Nicosia=2, Valencia=3, Cyprus=4};
    Locations location = Locations.Preston;

    static int index = 1;
    string scenarioNum = "";

    List<scenarioStep> newScenario = new List<scenarioStep>();

    //add menu item names "Create Scenario" to the Tools Menu
    [MenuItem ("Tools/Create Scneario")]
    public static void ShowWindow()
    {
        //show existing window instance, if one doesnt exist, make one.
        index = 1;
        EditorWindow.GetWindow(typeof(CreateScenarioWindow));
    }

    void OnGUI()
    {
        if (newScenario.Count ==0)
        {
            newScenario.Add(new scenarioStep());
        }
        GUILayout.Label("Create a new Scenario", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        location = (Locations)EditorGUILayout.EnumPopup("Location", location);
        scenarioNum = EditorGUILayout.TextField("Scenario Number", scenarioNum);

        float width = Screen.width * 0.6f;
        float labelWidth = width * 0.15f;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Step ID", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Type", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Description Key", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Wait", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Send Officers", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Request Citizen", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Officers Required", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Turns Required", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Severity", GUILayout.Width(labelWidth));
        EditorGUILayout.LabelField("Satisfaction Impact", GUILayout.Width(labelWidth));
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < index; i++)
        {
            newScenario[i].stepId = (i + 1).ToString();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField((i+1).ToString(), GUILayout.Width(labelWidth));
            newScenario[i].type = EditorGUILayout.TextField(newScenario[i].type, GUILayout.Width(labelWidth));
            newScenario[i].descriptionKey = EditorGUILayout.TextField(newScenario[i].descriptionKey, GUILayout.Width(labelWidth));
            newScenario[i].waitIndex = EditorGUILayout.TextField(newScenario[i].waitIndex, GUILayout.Width(labelWidth));
            newScenario[i].officerIndex = EditorGUILayout.TextField(newScenario[i].officerIndex, GUILayout.Width(labelWidth));
            newScenario[i].citizenIndex = EditorGUILayout.TextField(newScenario[i].citizenIndex, GUILayout.Width(labelWidth));
            newScenario[i].officerRequired = EditorGUILayout.TextField(newScenario[i].officerRequired, GUILayout.Width(labelWidth));
            newScenario[i].turnsRequired = EditorGUILayout.TextField(newScenario[i].turnsRequired, GUILayout.Width(labelWidth));
            newScenario[i].severity = EditorGUILayout.TextField(newScenario[i].severity, GUILayout.Width(labelWidth));
            newScenario[i].satisfactionImpact = EditorGUILayout.TextField(newScenario[i].satisfactionImpact, GUILayout.Width(labelWidth));
            EditorGUILayout.EndHorizontal();
        }
        if (GUI.Button(new Rect(10, 20, 150, 20), "Add another row"))
        {
            newScenario.Add(new scenarioStep());
            index++;
        }
        if (GUI.Button(new Rect(170, 20, 150, 20), "Remove Last Row"))
        {
            if (index > 1)
            {
                index--;
                newScenario.RemoveAt(index);
            }
        }
        if (GUI.Button(new Rect(330, 20, 150, 20), "Log current scenario"))
        {
            string jsonText = "";
            if (int.Parse(scenarioNum) > 1)
                jsonText += ",\n";
            jsonText += "\"Scenario" + scenarioNum + "\": {\n";
            for (int i = 0; i < index; i++)
            {
                jsonText += "\"" + newScenario[i].stepId + "\", "
                    + "[\"" + newScenario[i].type + "\", "
                    + "\"" + newScenario[i].descriptionKey + "\", "
                     + int.Parse(newScenario[i].waitIndex) + "\", "
                      + int.Parse(newScenario[i].officerIndex) + "\", "
                       + int.Parse(newScenario[i].citizenIndex) + "\", "
                        + int.Parse(newScenario[i].officerRequired) + "\", "
                         + int.Parse(newScenario[i].turnsRequired) + "\", "
                          + int.Parse(newScenario[i].severity) + "\", "
                           + int.Parse(newScenario[i].satisfactionImpact) + " ]";
                if (i < index-1)
                {
                    jsonText += ",\n";
                }
            }
            jsonText += "\n}";
            Debug.Log(jsonText);

            WriteToFile(jsonText);
        }
    }

    public void WriteToFile(string text)
    {
        string filePath = Application.dataPath + "/Resources/ScenarioInformation_" + location.ToString();
        if (System.IO.File.Exists(filePath))
        {
            Debug.LogError(string.Format("File at {0} does not exist", filePath));
            return;
        }
        var sr = System.IO.File.CreateText(filePath);
        sr.WriteLine(text);
        sr.Close();
        Debug.Log(string.Format("Successfully wrote to file: {0}", filePath));
    }
}

public class scenarioStep
{
    public string stepId = "";
    public string type = "";
    public string descriptionKey = "";
    public string waitIndex = "";
    public string officerIndex = "";
    public string citizenIndex = "";
    public string officerRequired = "";
    public string turnsRequired = "";
    public string severity = "";
    public string satisfactionImpact = "";
}