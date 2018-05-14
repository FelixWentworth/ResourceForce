using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Obsolete("Replaced by PlayGen.Unity.Utilities.Localization.LocalizationText. Use that instead.")]
public class LegacyFormattedLocalization : MonoBehaviour
{
	public string Key;
	public string[] FormattedKeys;

	private string[] _localizedStrings;

	public bool ToUpper;

	void OnEnable() {
		_localizedStrings = new string[FormattedKeys.Length];
		for (var i = 0; i < FormattedKeys.Length; i++)
		{
			_localizedStrings[i] = PlayGen.Unity.Utilities.Localization.Localization.Get(FormattedKeys[i]);
		}

		var text = GetComponent<Text>();
		text.text = string.Format(PlayGen.Unity.Utilities.Localization.Localization.Get(Key), _localizedStrings);
		if (ToUpper)
		{
			text.text = text.text.ToUpper();
		}
	}
}
