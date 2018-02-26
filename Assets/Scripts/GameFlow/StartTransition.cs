using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartTransition : MonoBehaviour {

    public Text password;
    public Image background;
    private Color backgroundColor;
    private Color fadedBackgroundColor;

    public static bool overrideShowLocation;

    public GameObject SelectLocationScreen;

    public GameObject[] objectsToDisableOnStart;
    public Tutorial tut;

    private bool _gameStarted;

    void Start()
    {

        //reset the start screen whilst taking note of the text that used to be used as the password
        SetObjects(true);
        _gameStarted = false;
        backgroundColor = background.color;
        fadedBackgroundColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0f);
		
	    bool newLogIn = PlayerPrefs.GetInt("SetLocation") == 0 || overrideShowLocation;
        overrideShowLocation = false;

		var deviceLocation = SelectLocationScreen.GetComponent<DeviceLocation>();
	    var useMarketingManager = BrandingManager.Instance.UseManager;

		deviceLocation.SetRequiredSelection(languageOnly: useMarketingManager);

	    if (useMarketingManager)
	    {
		    deviceLocation.SetLanguages(BrandingManager.Instance.Languages);
	    }

	    SelectLocationScreen.SetActive(newLogIn);

		this.gameObject.SetActive(!newLogIn);
    }

    public void StartGameTransition()
    {
        if (_gameStarted)
            return;
        // check that we are not going to show the tutorial
        _gameStarted = PlayerPrefs.GetInt("NewPlayer") == 1;
        //start to transition from this screen to game screen, we will do this by filling in the password and then transition to the main game
        AudioManager.Instance.PlayNewGame();
        StartCoroutine(DelayToGame());
    }
    IEnumerator DelayToGame()
    {
        yield return new WaitForSeconds(0.3f);
        FadeBackgrounds();
    }
    private void FadeBackgrounds()
    {
       
        //now fade the background image
        //on completion notify the turn manager that the start transition is done
        if (PlayerPrefs.GetInt("NewPlayer") == 0)
        {
            // TODO Load tutorial
            SceneManager.LoadScene(1);
            //player is new so show tutorial
            //tut.gameObject.SetActive(true);
            //tut.StartTutorial();
        }
        else
        {
            SetObjects(false);
            GameObject.Find("TurnManager").GetComponent<TurnManager>().StartGame();
            StartCoroutine(FadeBackground());
        }
    }
    IEnumerator FadeBackground()
    {
        float t = 0f;
        float duration = 3f;
        while (background.color != fadedBackgroundColor)
        {
            background.color = Color.Lerp(background.color, fadedBackgroundColor, t);
            if (t < 1)
            {
                t += Time.deltaTime/duration;
            }
            yield return null;
        }
        this.gameObject.SetActive(false);

    }
    private void SetObjects(bool enabled)
    {
        for (int i = 0; i<objectsToDisableOnStart.Length; i++)
        {
            objectsToDisableOnStart[i].SetActive(enabled);
        }
    }
}
