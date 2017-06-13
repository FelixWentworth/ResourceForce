using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnSatisfaction : MonoBehaviour {

    private Text _totalCasesText;
    private Text _closedCasesText;
    private Text _ongoingCasesText;

    public void SetText(int totalCases, int closedCases, int casesClosedThisTurn, int ongoingCases, int actionTaken, int ignoredCases)
    {
        _totalCasesText = transform.Find("SatisfactionSubtitle").GetComponent<Text>();
        _closedCasesText = transform.Find("ClosedCases/ClosedCasesText").GetComponent<Text>();
        _ongoingCasesText = transform.Find("OngoingCases/OngoingCasesText").GetComponent<Text>();

        _totalCasesText.text = Localization.Get("BASIC_TEXT_CASES") + ": " + totalCases;

        _closedCasesText.text = "<b>" + Localization.Get("BASIC_TEXT_CLOSED") + ": " + closedCases + "</b>";
        _closedCasesText.text += "\n" + Localization.Get("BASIC_TEXT_THIS_TURN") + ": " + casesClosedThisTurn;

        _ongoingCasesText.text = "<b>" + Localization.Get("BASIC_TEXT_ACTIVE") + ": " + ongoingCases + "</b>" + 
            "\n" + Localization.Get("BASIC_TEXT_ACTION_TAKEN") + ": " + actionTaken +
            "\n" + Localization.Get("BASIC_TEXT_IGNORED") + ": " + ignoredCases; 
    }
}
