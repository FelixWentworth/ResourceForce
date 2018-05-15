using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System;

[CustomEditor(typeof(BrandingManager))]
public class BrandingManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var brandingManager = (BrandingManager)target;

        if (GUILayout.Button("Apply"))
        {
            Apply(brandingManager);
        }

        GUILayout.Space(20);

        DrawDefaultInspector();

        GUILayout.Space(20);

        if (GUILayout.Button("Apply"))
        {
            Apply(brandingManager);
        }
    }

    private void Apply(BrandingManager brandingManager)
    {
        brandingManager.Apply();
        ApplyLocalizations(brandingManager.LocalizationSource, brandingManager.LocalizationOutput, brandingManager.Config.LocalizationOverrides);
    }


    private void ApplyLocalizations(string inputPath, string outputPath, LocalizationOverride[] localizationOverrides)
    {
        var localizationOverrideDict = localizationOverrides.ToDictionary(
            lo => lo.Key,
            lo => new Dictionary<string, string>
            {
                { "en-gb", lo.EnGb },
                { "el", lo.El },
                { "es", lo.Es },
                { "nl", lo.Nl },
            });

        var converter = new ExcelToJsonConverter();
        converter.ConvertExcelFileToJson(inputPath, outputPath, localizationOverrideDict);
    }
}