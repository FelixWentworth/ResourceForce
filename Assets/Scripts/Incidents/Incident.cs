using System.Linq;

public class Incident
{
    public Scenario Scenario;
    public IncidentContent IncidentContent;

    public int TurnToShow;
    
    //values which are not set during setup
    public int TurnToDevelop;
    public string NameBeforeDeveloped;
    public bool Developed;
    public bool Resolved;
    public bool PositiveResolution = false;
    public bool IsNew = true;

    public void Show(ref Incident zIncident, DialogBox dialogBox)
    {
        //use the dialog box to show the current incident
        dialogBox.Show(zIncident);
    }

    public bool IsEndCase()
    {
        return IncidentContent.Choices.Count == 1 && IncidentContent.Choices[0].Choice == null;
    }

    public IncidentContent GetChoiceContent(string choice)
    {
        foreach (var incidentChoice in IncidentContent.Choices)
        {
            if (incidentChoice.Choice != null && incidentChoice.Choice.ChoiceType != null && incidentChoice.Choice.ChoiceType == choice)
            {
                return incidentChoice.Scene;
            }
        }

        return null;
    }

    public ChoiceFeedback GetChoiceFeedback(string choice)
    {
        var content = IncidentContent.Choices.FirstOrDefault(c => c.Choice != null && c.Choice.ChoiceType == choice);
        if (content == null)
        {
            return new ChoiceFeedback() {ChoiceType = null, Feedback = "", FeedbackRating = -1};
        }
        return content.Choice;
    }
}