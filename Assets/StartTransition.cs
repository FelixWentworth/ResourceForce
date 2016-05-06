using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartTransition : MonoBehaviour {

    public Text password;
    string passwordText;
    private Image background;
    private Color backgroundColor;
    private Color fadedBackgroundColor;

    public GameObject[] objectsToDisableOnStart;

    void Awake()
    {
        passwordText = password.text;
        password.text = "";
        SetObjects(true);
        background = this.GetComponent<Image>();
        backgroundColor = background.color;
        fadedBackgroundColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0f);
    }

    public void StartGameTransition()
    {
        //start to transition from this screen to game screen, we will do this by filling in the password and then transition to the main game
        StartCoroutine(ShowPassword());   
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

    private void FadeBackgrounds()
    {
        SetObjects(false);
        //now fade the background image
        //on completion notify the turn manager that the start transition is done
        GameObject.Find("TurnManager").GetComponent<TurnManager>().StartGame();
        StartCoroutine(FadeBackground());
        
    }
    IEnumerator FadeBackground()
    {
        while (background.color != fadedBackgroundColor)
        {
            background.color = Color.Lerp(background.color, fadedBackgroundColor, 2.5f * Time.deltaTime);
            yield return null;
        }
        
    }
    private void SetObjects(bool enabled)
    {
        for (int i = 0; i<objectsToDisableOnStart.Length; i++)
        {
            objectsToDisableOnStart[i].SetActive(enabled);
        }
    }
}
