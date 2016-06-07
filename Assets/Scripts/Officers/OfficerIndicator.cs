using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OfficerIndicator : MonoBehaviour {
    public Text turnsTilAvailableText;
    
	public void UpdateText(string text = "")
    {
        turnsTilAvailableText.text = text;
    }
}
