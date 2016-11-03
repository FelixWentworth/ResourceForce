using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOver : MonoBehaviour
{
    public Text GameOverText;

    public Text TurnsText;
    public Text CasesText;

    public Text ScoreText;
    public Text HighScoreText;

    public void ShowGameOver(int turns, int cases)
    {
        AudioManager.Instance.PlayGameOver();

        GameOverText.text = string.Format(Localization.Get("BASIC_TEXT_GAMEOVER_BODY"), turns);

        var totalScore = turns + cases;

        var highScore = PlayerPrefs.GetInt("HighScore");

        if (totalScore > highScore)
        {
            highScore = totalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        StartCoroutine(ShowScores(turns, cases, totalScore, highScore));
    }

    private IEnumerator ShowScores(int turns, int cases, int score, int highScore)
    {
        TurnsText.text = "";
        CasesText.text = "";
        ScoreText.text = "";
        HighScoreText.text = highScore.ToString();
        // We want to display the scores one at a time to make the page more interesting
        yield return new WaitForSeconds(0.5f);
        yield return IncrementTextNumber(TurnsText, 0, turns);
        yield return IncrementTextNumber(CasesText, 0, cases);
        yield return IncrementTextNumber(ScoreText, 0, score);
        //yield return IncrementTextNumber(HighScoreText, 0, highScore);
    }

    private IEnumerator IncrementTextNumber(Text text, int start, int end)
    {
        var num = start;
        var speed = 0.4f;

        var t = 0f;

        while (t < 1)
        {
            t += speed*Time.deltaTime;
            num = (int)Mathf.Lerp(start, end, t);
            text.text = num.ToString();
            

            yield return null;
        }
    }
}
