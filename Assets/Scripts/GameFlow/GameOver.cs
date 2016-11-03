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

        TurnsText.text = turns.ToString();
        CasesText.text = cases.ToString();

        ScoreText.text = totalScore.ToString();
        HighScoreText.text = highScore.ToString();
    }
}
