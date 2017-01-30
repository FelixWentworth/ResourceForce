using UnityEngine;

public class IncidentQueue : MonoBehaviour {

    //create a list of gameobjects that we will use to keep track of incidents
    public IncidentCase[] allCases;

    private int cases = 0;
    void Awake()
    {
        for (int i=0; i<allCases.Length; i++)
        {
            //ensure that all of the cases are disabled at the start of the game
            allCases[i].gameObject.SetActive(false);
        }
    }
    public void MoveQueueUp(int index)
    {
        for (int i = index; i < cases-1; i++)
        {
            //move the data from the current case and move it up
            allCases[i].Setup(allCases[i+1].caseNumber, allCases[i+1].m_state, allCases[i+1].severityNumber);
            allCases[i].UpdateWarning(allCases[i + 1].warningIcon.activeSelf);
        }
    }
    public void AddToQueue(Incident zIncident)
    {
        //set up the data to show on the case
        allCases[cases].Setup(zIncident.caseNumber, IncidentCase.State.New, zIncident.severity);
        allCases[cases].gameObject.SetActive(true);
        ShowWarningIcon(zIncident.caseNumber);

        cases++;
    }
    public void RemoveFromQueue(int zCaseNumber)
    {
        for (int i=0; i< cases +1; i++)
        {
            //search for our case
            if (allCases[i].GetComponent<IncidentCase>().caseNumber == zCaseNumber)
            {
                //move the remaining cases up the queue
                //allCases[i].gameObject.SetActive(false);
                allCases[i].UpdateWarning(false);
                MoveQueueUp(i);
                break;
            }
        }
        cases--;
        //now deactivate all other cases
        for (int i = cases; i < allCases.Length; i++)
        {
            allCases[i].gameObject.SetActive(false);
        }
    }

   public void ToggleBackground(int caseNum)
    {
        for (int i = 0; i < cases; i++)
        {
            if (allCases[i].caseNumber == caseNum)
            {
                allCases[i].ToggleHighlight();
#if SELECT_INCIDENTS

#else
                allCases[i].warningIcon.SetActive(false);
#endif
            }
            else
            {
                allCases[i].ToggleHighlight(true);

            }
        }
    }
    public void RemoveAllHighlights()
    {
        //remove all highlight object from cases
        for (int i = 0; i< allCases.Length; i++)
        {
            allCases[i].ToggleHighlight(true);
        }
    }
    public void RemoveWarningIcon(int caseNum)
    {
        for (int i=0; i<cases; i++)
        {
            if (allCases[i].caseNumber == caseNum)
                allCases[i].UpdateWarning(false);
        }
    }
    public void UpdateSeverity(int caseNum, int zSeverity)
    {
        IncidentCase current = GetCaseFromNum(caseNum);
        current.severityNumber = zSeverity;
    }
    public void ChangeCaseState(int caseNum, IncidentCase.State newState)
    {
        IncidentCase current = GetCaseFromNum(caseNum);
        current.m_state = newState;
        //current.SetIcon();
    }
    public IncidentCase GetCaseFromNum(int n)
    {
        for (int i = 0; i < cases; i++)
        {
            if (allCases[i].caseNumber == n)
                return allCases[i];
        }
        return null;
    }
    public void ShowWarningIcon(int n)
    {
        for (int i=0; i< cases; i++)
        {
            if (allCases[i].caseNumber == n)
                allCases[i].UpdateWarning(true);
        }
    }

    public int GetActiveCases()
    {
        var count = 0;
        foreach (var c in allCases)
        {
            if (c.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }
}
