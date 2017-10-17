using System.Collections.Generic;

public class IncidentHistory
{
    public readonly List<IncidentHistoryElement> IncidentHistoryElements;

    public IncidentHistory()
    {
        IncidentHistoryElements = new List<IncidentHistoryElement> ();
    }

    public void AddHistoryElement(IncidentHistoryElement incidentHistoryElement)
    {
        IncidentHistoryElements.Add(incidentHistoryElement);
    }
}
