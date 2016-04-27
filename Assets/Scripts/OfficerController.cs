using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OfficerController : MonoBehaviour {

    //we want to handle the number of officers in this script, so that we can keep track of what they are doing and make sure that they take 3 turns to become available
    
    public List<officer> m_officers = new List<officer>();
    public List<officer> m_officersInUse = new List<officer>();

    public Text officersText;
    public Text officerStatus;

    public int StartingOfficers = 5;
    public int GetAvailable()
    {
        return m_officers.Count;
    }
    // Use this for initialization
    void Awake () {
        for (int i = 0; i< StartingOfficers; i++)
        {
            officer temp = new officer();
            m_officers.Add(temp);
        }
        m_officersInUse = new List<officer>();
        officersText.text = "Officers: " + m_officers.Count;
        SetOfficerStatus();
    }
    public void RemoveOfficer(int num)
    {
        //before calling this we must check if we have enough officers available
        for (int i = 0; i< num; i++)
        {
            //move officer from available list to unavailable list
            officer removed = m_officers[0];
            m_officers.RemoveAt(0);
            removed.Use();
            m_officersInUse.Add(removed);
        }
        officersText.text = "Officers: " + m_officers.Count;
        SetOfficerStatus();
    }
    private void AddOfficer(officer zOfficer)
    {
        //move officer from available list to unavailable list
        officer removed = zOfficer;
        removed.Reset();
        m_officers.Add(removed);
        officersText.text = "Officers: " + m_officers.Count;
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
        officersText.text = "Officers: " + m_officers.Count;
        SetOfficerStatus();
    }
    private void SetOfficerStatus()
    {
        string status = "";
        int count = 1;
        for (int i = 0; i < m_officers.Count; i++, count++)
        {
            status += count + " - Available\n";
        }
        for (int i = 0; i< m_officersInUse.Count; i++, count++)
        {
            status += count + " - " + m_officersInUse[i].turnsTilAvailable + " turns until available\n";
        }
        officerStatus.text = status;
    }
}
public class officer
{
    public officer()
    {
        available = true;
        turnsTilAvailable = 0;
    }
    public bool available;
    public int turnsTilAvailable;

    public void Use()
    {
        available = false;
        turnsTilAvailable = 3;
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
