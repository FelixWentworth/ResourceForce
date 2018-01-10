using GameAnalyticsSDK;
using UnityEngine;

/// <summary>
/// Attach to a button object to open a specified link
/// </summary>
public class ButtonLink : MonoBehaviour
{
    public string Url;
	public bool SendEvent;
	public string EventName;

	void Start ()
	{
	    gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Open);
	}

    private void Open()
    {
	    if (SendEvent)
	    {
			GameAnalytics.NewDesignEvent(EventName);
		}
        Application.OpenURL(Url);
    }
}
