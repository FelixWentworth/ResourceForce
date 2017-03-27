using UnityEngine;
using UnityEngine.UI;

public class VersionInformation : MonoBehaviour
{
    private Text _text;

    void Start()
    {
        _text = this.GetComponent<Text>();
        _text.text = Application.version;
    }
}
