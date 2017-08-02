using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario
{
    public string Id;
    public long SerialNumber;
    public string Location;
    public string Language;
    public ScenarioWrapper Content;
}

public class ScenarioWrapper
{
    private string _type = "Content";
    public IncidentContent Scene;
}

public class IncidentContent
{
    private string _type = "scene";
    public string Title;
    public string Description;
    public int Severity;
    public int OfficerReq;
    public int TurnReq;
    public int SatisfactionImpact;
    public List<IncidentChoice> Choices;
}

public class ChoiceFeedback
{
    private string _type = "ChoiceAction";
    public string ChoiceType;
    public int FeedbackRating;
    public string Feedback;
}

public class IncidentChoice
{
    private string _type = "Choice";
    public ChoiceFeedback Choice;
    public IncidentContent Scene;
}