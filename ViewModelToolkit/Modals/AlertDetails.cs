namespace ViewModelToolkit.Modals;

public class AlertDetails
{
    const string titleText = "Lose your changes?";
    const string descriptionText = "If you continue, you will lose any unsaved changes. Are you sure you want to continue?";

    public AlertDetails(string title = titleText, string description = descriptionText, string noText = "No", string yesText = "Yes") {
        Title = title;
        Description = description;
        NoText = noText;
        YesText = yesText;
    }

    public string Title { get; init; }
    public string Description { get; init; }
    public string YesText { get; init; }
    public string NoText { get; init; }
}
