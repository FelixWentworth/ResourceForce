using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InformationPanel : MonoBehaviour
{

    public GameObject InputBlocker;

    public GameObject SatifactionInfo;
    public GameObject IncidentInfo;
    public GameObject OfficerInfo;

    private Text _satisfactionText;
    private Text _incidenText;
    private Text _officerText;

    private IncidentManager _incidentManager;
    private OfficerController _officerController;

    private const string satisfaction = "TIPS_SATISFACTION";

    private const string incident = "TIPS_INCIDENT";

    private const string incidents = "TIPS_INCIDENTS";

    private const string officer = "TIPS_OFFICER";

    private const string officers = "TIPS_OFFICERS";

    void Awake()
    {
        _satisfactionText = SatifactionInfo.GetComponentInChildren<Text>();
        _incidenText = IncidentInfo.GetComponentInChildren<Text>();
        _officerText = OfficerInfo.GetComponentInChildren<Text>();

        DisableAll();
    }

    void Start()
    {
        _incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
        _officerController = GameObject.Find("OfficerManager").GetComponent<OfficerController>();
    }
    public void DisableAll()
    {
        SatifactionInfo.SetActive(false);
        IncidentInfo.SetActive(false);
        OfficerInfo.SetActive(false);
        InputBlocker.SetActive(false);
    }

    public void SatifactionPressed()
    {
        if (InputBlocker.activeSelf)
        {
            DisableAll();
            return;
        }
        DisableAll();

        _satisfactionText.text = string.Format(Localization.Get(satisfaction), _incidentManager.GetHappiness());

        InputBlocker.SetActive(true);
        SatifactionInfo.SetActive(true);
    }

    public void IncidentPressed()
    {
        if (InputBlocker.activeSelf)
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
        if (InputBlocker.activeSelf)
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

}
