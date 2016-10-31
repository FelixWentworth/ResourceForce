using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OfficerController : MonoBehaviour {

    //we want to handle the number of officers in this script, so that we can keep track of what they are doing and make sure that they take 3 turns to become available
    
    public List<officer> m_officers = new List<officer>();
    public List<officer> m_officersInUse = new List<officer>();

    public Text officerStatus;

    public int StartingOfficers = 5;

    public OfficerIndicator[] officerIndicators;
    public RectTransform OfficerPanel;
    private float _offset = 0f;

    public Color AvailableOfficerColor;
    public Color InUseOfficerColor;

    public int GetAvailable()
    {
        return m_officers.Count;
    }
    // Use this for initialization
    void Awake () {
        for (int i = 0; i<officerIndicators.Length; i++)
        {
            officerIndicators[i].gameObject.SetActive(true);
            officerIndicators[i].UpdateText("");
            var rectTransform = officerIndicators[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = new Vector3(_offset, 0f, 0f);
            _offset += rectTransform.rect.width;
        }
        for (int i = 0; i< StartingOfficers; i++)
        {
            officer temp = new officer();
            m_officers.Add(temp);
        }
        m_officersInUse = new List<officer>();
        SetOfficerStatus();
        
    }
    public void RemoveOfficer(int num, int turnsAway)
    {
        //before calling this we must check if we have enough officers available
        for (int i = 0; i< num; i++)
        {
            //move officer from available list to unavailable list
            officer removed = m_officers[0];
            m_officers.RemoveAt(0);
            removed.Use(turnsAway);
            m_officersInUse.Add(removed);
        }
        SetOfficerStatus();
    }
    private void AddOfficer(officer zOfficer)
    {
        //move officer from available list to unavailable list
        officer removed = zOfficer;
        removed.Reset();
        m_officers.Add(removed);
        SetOfficerStatus();
    }
    public void EndTurn()
    {
        //turn has ended, reduce turnsTilAvailable
        for (int i = 0; i < m_officersInUse.Count; i++)
        {
            m_officersInUse[i].ReduceTurn();
            if (m_officersInUse[i].turnsTilAvailable <= 0)
            {
                //add the officer back to the list of available officers
                AddOfficer(m_officersInUse[i]);
                m_officersInUse.RemoveAt(i);
                i--; //reduce i as we removed one of the elements in the list
            }
        }
        SetOfficerStatus();
    }
    private void SetOfficerStatus()
    {
        var status = "";
        var count = 1;
        for (var i = 0; i < m_officers.Count; i++, count++)
        {
            status += count + " - " + Localization.Get("BASIC_TEXT_AVAILABLE") + "\n";
        }
        for (var i = 0; i< m_officersInUse.Count; i++, count++)
        {
            status += count + " - " + m_officersInUse[i].turnsTilAvailable + Localization.Get("BASIC_TEXT_TURNS_UNTIL_AVAILABLE") + "\n";
        }
        officerStatus.text = status;
        UpdateOfficerIndicators();
    }

    private void UpdateOfficerIndicators()
    {
        var officersInUse = m_officersInUse.Count;

        var inUseIndex = 0;

        var inUseOffset = OfficerPanel.rect.width;
        var availableOffset = 0f;

        for (var i = StartingOfficers - 1; i >= 0; i--)
        {
            if (officersInUse > 0)
            {
                officerIndicators[i].gameObject.SetActive(true);
                officerIndicators[i].UpdateText(m_officersInUse[inUseIndex].turnsTilAvailable.ToString());
                officerIndicators[i].UpdateColor(InUseOfficerColor);

                inUseOffset -= officerIndicators[i].GetComponent<RectTransform>().rect.width;
                StartCoroutine(TransitionToPosition(officerIndicators[i].GetComponent<RectTransform>(),
                    new Vector3(inUseOffset, 0f, 0f), 0.5f));

                officersInUse--;
                inUseIndex++;
            }
            else
            {
                officerIndicators[i].UpdateText("");
                officerIndicators[i].UpdateColor(AvailableOfficerColor);

                availableOffset = i * officerIndicators[i].GetComponent<RectTransform>().rect.width;
                StartCoroutine(TransitionToPosition(officerIndicators[i].GetComponent<RectTransform>(),
                    new Vector3(availableOffset, 0f, 0f), 0.5f));              
            }
        }
    }
    private IEnumerator TransitionToPosition(RectTransform transform, Vector3 position, float time)
    {
        var deltaTime = 0f;
        var startPos = transform.anchoredPosition3D;
        while (deltaTime <= time)
        {
            transform.anchoredPosition3D = Vector3.Lerp(startPos, position, deltaTime / time);
            deltaTime += Time.deltaTime;
            yield return null;
        }
        transform.anchoredPosition3D = position;
    }
}
public class officer
{
    public bool available;
    public int turnsTilAvailable;

    public officer()
    {
        available = true;
        turnsTilAvailable = 0;
    }
    public void Use(int turns = 3)
    {
        available = false;
        turnsTilAvailable = turns;
    }
    public void Reset()
    {
        available = true;
        turnsTilAvailable = 0;
    }
    public void ReduceTurn()
    {
        turnsTilAvailable -= 1;
    }
}
