using UnityEngine;
using System.Collections;

public class IncidentHistoryElement
{
    public string Type;
    public string Description;
    public int FeedbackRating;
    public string Feedback;
    public int Severity;

    public enum Decision
    {
        Ignore,
        Officer,
        Citizen
    }

    public Decision PlayerDecision;
}
