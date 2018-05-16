using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsInitializer : MonoBehaviour {

	// Use this for initialization
#if !UNITY_EDITOR
	#if UNITY_ANDROID || UNITY_IPHONE
		void Start () {
			GameAnalytics.Initialize();		
		}
	#endif
#endif
}
