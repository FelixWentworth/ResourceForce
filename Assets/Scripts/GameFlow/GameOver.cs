using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameAnalyticsSDK;

public class GameOver : MonoBehaviour
{
    public Text GameOverText;

    public Text TurnsLabel;
    public Text GoodCasesLabel;
    public Text BadCasesLabel;

    public Text TurnsText;
    public Text GoodCaseText;
    public Text BadCaseText;
    public Text ScoreText;
    public Text HighScoreText;

    private readonly int TurnsSurvivedMultiplier = 1;
    private readonly int CasesClosedWellMultiplier = 5;
    private readonly int CasesClosedBadlyMultiplier = -5;

    private readonly string TurnsLabelKey = "BASIC_TEXT_TURNS_SURVIVED";
    private readonly string GoodCasesLabelKey = "BASIC_TEXT_CASES_CLOSED_POSITIVE";
    private readonly string BadCasesLabelKey = "BASIC_TEXT_CASES_CLOSED_NEGATIVE";

    public void ShowGameOver(int turns, int cases, int casesClosedWell)
    {
        AudioManager.Instance.PlayGameOver();

        GameOverText.text = string.Format(Localization.Get("BASIC_TEXT_GAMEOVER_BODY"), turns);

        TurnsLabel.text = "";
        GoodCasesLabel.text = "";
        BadCasesLabel.text = "";


        var turnScore = turns*TurnsSurvivedMultiplier;
        var goodCaseScore = casesClosedWell*CasesClosedWellMultiplier;
        var badCaseScore = (cases - casesClosedWell)*CasesClosedBadlyMultiplier;

        var totalScore = turnScore + goodCaseScore + badCaseScore;

        var highScore = PlayerPrefs.GetInt("HighScore");

        if (totalScore > highScore)
        {
            highScore = totalScore;
            PlayerPrefs.SetInt("HighScore", highScore);

        }
        GameAnalytics.NewDesignEvent(Location.CurrentLocation + "_Score", totalScore);

        StartCoroutine(ShowScores(turns, casesClosedWell, (cases - casesClosedWell), turnScore, goodCaseScore, badCaseScore, totalScore, highScore));
    }

    private IEnumerator ShowScores(int turns, int goodCases, int badCases, int turnsScore, int goodCaseScore, int badCaseScore,  int score, int highScore)
    {
        TurnsText.text = "";
        GoodCaseText.text = "";
        BadCaseText.text = "";
        ScoreText.text = "";
        HighScoreText.text = "";
        // We want to display the scores one at a time to make the page more interesting
        yield return new WaitForSeconds(0.5f);

        TurnsLabel.text = Localization.Get(TurnsLabelKey) + ": x" + turns;
        StartCoroutine(IncrementTextNumber(ScoreText, 0, turnsScore));
        yield return IncrementTextNumber(TurnsText, 0, turnsScore);

        GoodCasesLabel.text = Localization.Get(GoodCasesLabelKey) + ": x" + goodCases;
        StartCoroutine(IncrementTextNumber(ScoreText, turnsScore, (turnsScore + goodCaseScore)));
        yield return IncrementTextNumber(GoodCaseText, 0, goodCaseScore);

        BadCasesLabel.text = Localization.Get(BadCasesLabelKey) + ": x" + badCases;
        StartCoroutine(IncrementTextNumber(ScoreText, (turnsScore + goodCaseScore), score));
        yield return IncrementTextNumber(BadCaseText, 0, badCaseScore);  

        //yield return IncrementTextNumber(ScoreText, 0, score);
        yield return IncrementTextNumber(HighScoreText, 0, highScore);
    }

    private IEnumerator IncrementTextNumber(Text text, int start, int end)
    {
        var num = start;
        var speed = 1.0f;

        var t = 0f;

        while (t <= speed)
        {
            t += speed*Time.deltaTime;
            num = (int)Mathf.Lerp(start, end, t/speed);
            text.text = num.ToString();
            

            yield return null;
        }
        text.text = end.ToString();
    }
}
