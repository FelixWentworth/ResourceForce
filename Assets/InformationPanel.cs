using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PlayGen.Unity.Utilities.Localization;

public class InformationPanel : MonoBehaviour
{

    public GameObject InputBlocker;

    public GameObject SatifactionInfo;
    public GameObject IncidentInfo;
    public GameObject OfficerInfo;
    public GameObject TurnInfo;

    private Text _satisfactionText;
    private Text _incidenText;
    private Text _officerText;
    private Text _turnText;

    private IncidentManager _incidentManager;
    private OfficerController _officerController;
    private TurnManager _turnManager;

    private WarningBox _warningObject;

    private const string satisfaction = "TIPS_SATISFACTION";

    private const string incident = "TIPS_INCIDENT";

    private const string incidents = "TIPS_INCIDENTS";

    private const string officer = "TIPS_OFFICER";

    private const string officers = "TIPS_OFFICERS";

    private const string turn = "TIPS_TURN";

    private const string turns = "TIPS_TURNS";

    void Awake()
    {
        _satisfactionText = SatifactionInfo.GetComponentInChildren<Text>();
        _incidenText = IncidentInfo.GetComponentInChildren<Text>();
        _officerText = OfficerInfo.GetComponentInChildren<Text>();
        _turnText = TurnInfo.GetComponentInChildren<Text>();
        _warningObject =  GameObject.Find("WarningPopup").GetComponent<WarningBox>();

        DisableAll();
    }

    void Start()
    {
        _incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
        _turnManager = _incidentManager.gameObject.GetComponent<TurnManager>();
        _officerController = GameObject.Find("OfficerManager").GetComponent<OfficerController>();
    }
    public void DisableAll()
    {
        SatifactionInfo.SetActive(false);
        IncidentInfo.SetActive(false);
        OfficerInfo.SetActive(false);
        TurnInfo.SetActive(false);
        InputBlocker.SetActive(false);
    }

    public void SatifactionPressed()
    {
        if (InputBlocker.activeSelf || _warningObject.ShowingPopup)
        {
            DisableAll();
            return;
        }
        DisableAll();

        _satisfactionText.text = string.Format(Localization.Get(satisfaction), Mathf.RoundToInt(_incidentManager.GetHappiness()));

        InputBlocker.SetActive(true);
        SatifactionInfo.SetActive(true);
    }

    public void IncidentPressed()
    {
        if (InputBlocker.activeSelf || _warningObject.ShowingPopup)
        {
            DisableAll();
            return;
        }
        DisableAll();

        var num = _incidentManager.m_IncidentQueue.GetActiveCases();

        _incidenText.text = num == 1
            ? string.Format(Localization.Get(incident), num)
            : string.Format(Localization.Get(incidents), num);

        InputBlocker.SetActive(true);
        IncidentInfo.SetActive(true);
    }

    public void OfficerPressed()
    {
        if (InputBlocker.activeSelf || _warningObject.ShowingPopup)
        {
            DisableAll();
            return;
        }
        DisableAll();
        var num = _officerController.m_officers.Count;

        _officerText.text = num == 1
            ? string.Format(Localization.Get(officer), num)
            : string.Format(Localization.Get(officers), num);

        InputBlocker.SetActive(true);
        OfficerInfo.SetActive(true);
    }

    public void TurnPressed()
    {
        if (InputBlocker.activeSelf || _warningObject.ShowingPopup)
        {
            DisableAll();
            return;
        }
        DisableAll();
        var num = _turnManager.turn;

        _turnText.text = num == 1
            ? string.Format(Localization.Get(turn), num)
            : string.Format(Localization.Get(turns), num);

        InputBlocker.SetActive(true);
        TurnInfo.SetActive(true);
    }

}
