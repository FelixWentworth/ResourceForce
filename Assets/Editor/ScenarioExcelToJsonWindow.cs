using UnityEditor;
using UnityEngine;

public class ScenarioExcelToJsonWindow : EditorWindow
{
    public static string InputPathPrefsName_Preston = "Scenario.Preston.InputPath";
    public static string OutputPathPrefsName_Preston = "Scenario.Preston.OutputPath";

    public static string InputPathPrefsName_Belfast = "Scenario.Belfast.InputPath";
    public static string OutputPathPrefsName_Belfast = "Scenario.Belfast.OutputPath";

    public static string InputPathPrefsName_Valencia = "Scenario.Valencia.InputPath";
    public static string OutputPathPrefsName_Valencia = "Scenario.Valencia.OutputPath";

    public static string InputPathPrefsName_Nicosia = "Scenario.Nicosia.InputPath";
    public static string OutputPathPrefsName_Nicosia = "Scenario.Nicosia.OutputPath";

    public static string InputPathPrefsName_Groningen = "Scenario.Groningen.InputPath";
    public static string OutputPathPrefsName_Groningen = "Scenario.Groningen.OutputPath";

    private string _inputPathPreston;
    private string _outputPathPreston;

    private string _inputPathBelfast;
    private string _outputPathBelfast;

    private string _inputPathValencia;
    private string _outputPathValencia;

    private string _inputPathNicosia;
    private string _outputPathNicosia;

    private string _inputPathGroningen;
    private string _outputPathGroningen;

    private ScenarioExcelToJsonConverter _excelProcessor;

    [MenuItem("Tools/Scenario Excel To Json Converter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ScenarioExcelToJsonWindow), true, "Scenario Excel To Json Converter", true);
    }

    public void OnEnable()
    {
        if (_excelProcessor == null)
        {
            _excelProcessor = new ScenarioExcelToJsonConverter();
        }

        LoadPaths();

    }

    public void OnDisable()
    {
        SavePaths();
    }

    void OnGUI()
    {
        GenerateGUIRow(ref _inputPathPreston, ref _outputPathPreston, ScenarioExcelToJsonConverter.ScenarioSite.Preston);
        GenerateGUIRow(ref _inputPathBelfast, ref _outputPathBelfast, ScenarioExcelToJsonConverter.ScenarioSite.Belfast);
        GenerateGUIRow(ref _inputPathValencia, ref _outputPathValencia, ScenarioExcelToJsonConverter.ScenarioSite.Valencia);
        GenerateGUIRow(ref _inputPathGroningen, ref _outputPathGroningen, ScenarioExcelToJsonConverter.ScenarioSite.Groningen);
        GenerateGUIRow(ref _inputPathNicosia, ref _outputPathNicosia, ScenarioExcelToJsonConverter.ScenarioSite.Nicosia);

        GUI.enabled = true;
    }

    private void LoadPaths()
    {
        _inputPathPreston = EditorPrefs.GetString(InputPathPrefsName_Preston, Application.dataPath);
        _outputPathPreston = EditorPrefs.GetString(OutputPathPrefsName_Preston, Application.dataPath);

        _inputPathBelfast = EditorPrefs.GetString(InputPathPrefsName_Belfast, Application.dataPath);
        _outputPathBelfast = EditorPrefs.GetString(OutputPathPrefsName_Belfast, Application.dataPath);

        _inputPathValencia = EditorPrefs.GetString(InputPathPrefsName_Valencia, Application.dataPath);
        _outputPathValencia = EditorPrefs.GetString(OutputPathPrefsName_Valencia, Application.dataPath);

        _inputPathNicosia = EditorPrefs.GetString(InputPathPrefsName_Nicosia, Application.dataPath);
        _outputPathNicosia = EditorPrefs.GetString(OutputPathPrefsName_Nicosia, Application.dataPath);

        _inputPathGroningen = EditorPrefs.GetString(InputPathPrefsName_Groningen, Application.dataPath);
        _outputPathGroningen = EditorPrefs.GetString(OutputPathPrefsName_Groningen, Application.dataPath);
    }

    private void SavePaths()
    {
        EditorPrefs.SetString(InputPathPrefsName_Preston, _inputPathPreston);
        EditorPrefs.SetString(OutputPathPrefsName_Preston, _outputPathPreston);

        EditorPrefs.SetString(InputPathPrefsName_Belfast, _inputPathBelfast);
        EditorPrefs.SetString(OutputPathPrefsName_Belfast, _outputPathBelfast);

        EditorPrefs.SetString(InputPathPrefsName_Valencia, _inputPathValencia);
        EditorPrefs.SetString(OutputPathPrefsName_Valencia, _outputPathValencia);

        EditorPrefs.SetString(InputPathPrefsName_Nicosia, _inputPathNicosia);
        EditorPrefs.SetString(OutputPathPrefsName_Nicosia, _outputPathNicosia);

        EditorPrefs.SetString(InputPathPrefsName_Groningen, _inputPathGroningen);
        EditorPrefs.SetString(OutputPathPrefsName_Groningen, _outputPathGroningen);
    }

    private void GenerateGUIRow(ref string inputPath, ref string outputPath, ScenarioExcelToJsonConverter.ScenarioSite site)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(site.ToString(), EditorStyles.boldLabel);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUIContent inputFolderContent = new GUIContent("Input Folder", "Select the folder where the excel files to be processed are located.");
        EditorGUIUtility.labelWidth = 120.0f;
        EditorGUILayout.TextField(inputFolderContent, inputPath, GUILayout.MinWidth(120));
        if (GUILayout.Button(new GUIContent("Select Folder"), GUILayout.MinWidth(80), GUILayout.MaxWidth(100)))
        {
            inputPath = EditorUtility.OpenFolderPanel("Select Folder with Excel Files", inputPath, Application.dataPath);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUIContent outputFolderContent = new GUIContent("Output Folder", "Select the folder where the converted json files should be saved.");
        EditorGUILayout.TextField(outputFolderContent, outputPath, GUILayout.MinWidth(120));
        if (GUILayout.Button(new GUIContent("Select Folder"), GUILayout.MinWidth(80), GUILayout.MaxWidth(100)))
        {
            outputPath = EditorUtility.OpenFolderPanel("Select Folder to save json files", outputPath, Application.dataPath);
        }

        GUILayout.EndHorizontal();


        if (string.IsNullOrEmpty(inputPath) || string.IsNullOrEmpty(outputPath))
        {
            GUI.enabled = false;
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Convert " + site + " Excel File"))
        {
            _excelProcessor.ConvertExcelFilesToJson(inputPath, outputPath, site);
        }

        GUILayout.EndHorizontal();
    }

}

[InitializeOnLoad]
public class ScenarioExcelToJsonAutoConverter
{
    /// <summary>
    /// Class attribute [InitializeOnLoad] triggers calling the static constructor on every refresh.
    /// </summary>
    static ScenarioExcelToJsonAutoConverter()
    {
        //string inputPath = EditorPrefs.GetString(ExcelToJsonConverterWindow.kExcelToJsonConverterInputPathPrefsName, Application.dataPath);
        //string outputPath = EditorPrefs.GetString(ExcelToJsonConverterWindow.kExcelToJsonConverterOuputPathPrefsName, Application.dataPath);

        //ScenarioExcelToJsonConverter excelProcessor = new ScenarioExcelToJsonConverter();
        //excelProcessor.ConvertExcelFilesToJson(inputPath, outputPath);
    }
}
