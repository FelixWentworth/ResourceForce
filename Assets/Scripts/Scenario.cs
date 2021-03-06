﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Scenario
{
    public string Id;
    public long SerialNumber;
    public string Region;
    public string Language;
    public bool Deleted;
	public bool Enabled;
    public ScenarioWrapper Content;

    public bool IsRegionMatch(string matchRegion)
    {
        return Region == matchRegion || Region == "Any";
    }
}

[Serializable]
public class ScenarioWrapper
{
    private string _type = "Content";
    public IncidentContent Scene;
}

[Serializable]
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

[Serializable]
public class ChoiceFeedback
{
    private string _type = "ChoiceAction";
    public string ChoiceType;
    public int FeedbackRating;
    public string Feedback;
}

[Serializable]
public class IncidentChoice
{
    private string _type = "Choice";
    public ChoiceFeedback Choice;
    public IncidentContent Scene;
}