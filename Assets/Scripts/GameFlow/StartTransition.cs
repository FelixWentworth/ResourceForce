using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartTransition : MonoBehaviour {

    public Text password;
    string passwordText;
    public Image background;
    private Color backgroundColor;
    private Color fadedBackgroundColor;

    public static bool overrideShowLocation;

    public GameObject SelectLocationScreen;

    public GameObject[] objectsToDisableOnStart;
    public Tutorial tut;

    void Awake()
    {
        //reset the start screen whilst taking note of the text that used to be used as the password
        passwordText = password.text;
        password.text = "";
        SetObjects(true);
        backgroundColor = background.color;
        fadedBackgroundColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0f);

        var a = PlayerPrefs.GetInt("2131qweqw23");

        bool newLogIn = PlayerPrefs.GetInt("SetLocation") == 0 || overrideShowLocation;
        overrideShowLocation = false;
        SelectLocationScreen.SetActive(newLogIn);
        this.gameObject.SetActive(!newLogIn);
    }

    public void StartGameTransition()
    {
        //start to transition from this screen to game screen, we will do this by filling in the password and then transition to the main game
        //StartCoroutine(ShowPassword());
        AudioManager.Instance.PlayNewGame();
        StartCoroutine(DelayToGame());
    }
    IEnumerator ShowPassword()
    {
        foreach (char c in passwordText)
        {
            password.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        //we have now filled the password text, so fade the background to show the main game
        FadeBackgrounds();
    }
    IEnumerator DelayToGame()
    {
        yield return new WaitForSeconds(0.3f);
        FadeBackgrounds();
    }
    private void FadeBackgrounds()
    {
        SetObjects(false);
        //now fade the background image
        //on completion notify the turn manager that the start transition is done
        if (PlayerPrefs.GetInt("NewPlayer") == 0)
        {
            //player is new so show tutorial
            tut.gameObject.SetActive(true);
            tut.ShowStep();
        }
        else
        {
            GameObject.Find("TurnManager").GetComponent<TurnManager>().StartGame();
        }
        
        StartCoroutine(FadeBackground());
        
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
