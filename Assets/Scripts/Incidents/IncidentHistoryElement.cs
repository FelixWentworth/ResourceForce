using UnityEngine;
using System.Collections;

public class IncidentHistoryElement
{
    public string Type;
    public int FeedbackRating;
    public string Feedback;

    public enum Decision
    {
        Ignore,
        Officer,
        Citizen
    }

    public Decision PlayerDecision;
}
