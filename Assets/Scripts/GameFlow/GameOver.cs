using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameAnalyticsSDK;
using PlayGen.Unity.Utilities.Localization;

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
	    var newHighScore = highScore;
        if (totalScore > highScore)
        {
            newHighScore = totalScore;
            PlayerPrefs.SetInt("HighScore", newHighScore);
        }

        GameAnalytics.NewDesignEvent(Location.CurrentLocation + "_TurnsSurvived", turns);
        GameAnalytics.NewDesignEvent(Location.CurrentLocation + "_Score", totalScore);

        StartCoroutine(ShowScores(turns, casesClosedWell, (cases - casesClosedWell), turnScore, goodCaseScore, badCaseScore, totalScore, highScore, newHighScore));
    }

    private IEnumerator ShowScores(int turns, int goodCases, int badCases, int turnsScore, int goodCaseScore, int badCaseScore,  int score, int highScore, int newHighScore)
    {
        TurnsText.text = "";
        GoodCaseText.text = "";
        BadCaseText.text = "";
        ScoreText.text = "0";
        HighScoreText.text = highScore.ToString();
        // We want to display the scores one at a time to make the page more interesting
        yield return new WaitForSeconds(0.5f);

        TurnsLabel.text = turns + " " + Localization.Get(TurnsLabelKey);
        //StartCoroutine(IncrementTextNumber(ScoreText, 0, turnsScore));
        yield return IncrementTextNumber(TurnsText, 0, turnsScore, 0, true);

        GoodCasesLabel.text = goodCases + " " + Localization.Get(GoodCasesLabelKey);
        //StartCoroutine(IncrementTextNumber(ScoreText, turnsScore, (turnsScore + goodCaseScore)));
        yield return IncrementTextNumber(GoodCaseText, 0, goodCaseScore, turnsScore, true);

        BadCasesLabel.text = badCases + " " + Localization.Get(BadCasesLabelKey);
        //StartCoroutine(IncrementTextNumber(ScoreText, (turnsScore + goodCaseScore), score));
        yield return IncrementTextNumber(BadCaseText, 0, badCaseScore, (turnsScore + goodCaseScore), true);  

        yield return IncrementTextNumber(HighScoreText, highScore, newHighScore, 0, false);
    }

    private IEnumerator IncrementTextNumber(Text text, int start, int end, int currentScore, bool incrementScore)
    {
        var num = start;
        var speed = 1.0f;

        var t = 0f;

        while (t <= speed)
        {
            t += speed*Time.deltaTime;
            num = (int)Mathf.Lerp(start, end, t/speed);
            text.text = num.ToString();

	        if (incrementScore)
	        {
		        ScoreText.text = (currentScore + num).ToString();
	        }
				
            yield return null;
        }
        text.text = end.ToString();
	    if (incrementScore)
	    {
		    ScoreText.text = (currentScore + num).ToString();
	    }

    }
}
