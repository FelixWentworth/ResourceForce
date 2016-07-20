using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class FontReplace : EditorWindow {

    private List<Font> _gameFonts = new List<Font>();

    [MenuItem("Tools/Replace Fonts")]
	public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FontReplace));
    }
    void OnGUI()
    {
        GUILayout.Label("Replace selected fonts", EditorStyles.boldLabel);
        GUILayout.Space(10f);
        //get all the fonts that exist in the project
        List<string> names = FontNames();
        for (int i=0;i<names.Count; i++)
        {
            //list all the fonts
            if (_gameFonts.Count == 0 || _gameFonts.Count < i + 1)
                _gameFonts.Add(new Font());
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(names[i]);
            if (GUILayout.Button("Replace With"))
            {
                ReplaceFont(names[i], _gameFonts[i]);
            }
            _gameFonts[i] = (Font)EditorGUILayout.ObjectField(_gameFonts[i], typeof(Font), true);
            //create a button to replace the selected font with the font dragged in
           
            EditorGUILayout.EndHorizontal();
        }
    }

    void ReplaceFont(string name, Font font)
    {
        //check the font has been set to a valid font
        if (font == null || font.fontNames.Length == 0)
            return;
        //now get all text types in the scene and change the ones with a matching font name to the new font
        Text[] allTexts = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Text>();
        foreach (Text t in allTexts)
        {
            if (t.font.ToString() == name)
            {
                t.font = font;
            }
        }
    }
    List<string> FontNames()
    {
        //return a string of font names
        List<string> fontNames = new List<string>();
        Text[] allTexts = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Text>();

        foreach(Text t in allTexts)
        {
            if (!fontNames.Contains(t.font.ToString()))
            {
                fontNames.Add(t.font.ToString());
            }
        }
        return fontNames;
    }
}
