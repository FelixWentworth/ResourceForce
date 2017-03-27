using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class VersionInformation : MonoBehaviour
{

    private Text _text;

    void Start()
    {
        _text = this.GetComponent<Text>();
        _text.text = PlayerSettings.bundleVersion;
    }
}
