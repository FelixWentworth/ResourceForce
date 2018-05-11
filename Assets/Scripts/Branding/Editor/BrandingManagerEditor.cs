using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BrandingManager))]
public class BrandingManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var brandingManager = (BrandingManager)target;

        if (GUILayout.Button("Apply"))
        {
            brandingManager.Apply();
        }

        GUILayout.Space(20);

        DrawDefaultInspector();

        GUILayout.Space(20);

        if (GUILayout.Button("Apply"))
        {
            brandingManager.Apply();
        }
    }
}