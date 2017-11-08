using UnityEngine;

/// <summary>
/// Attach to a button object to open a specified link
/// </summary>
public class ButtonLink : MonoBehaviour
{
    public string Url;

	void Start ()
	{
	    gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Open);
	}

    private void Open()
    {
        Application.OpenURL(Url);
    }
}
