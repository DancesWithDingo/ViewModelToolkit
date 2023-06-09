using ViewModelToolkit.Dialogs;
using ViewModelToolkit.ViewModels;
using ViewModelToolkitSample.Models;

namespace ViewModelToolkitSample.ViewModels;

public class CustomFormPageViewModel : ModalViewModelBase<Transaction>
{
    // Note: this is not an override of Initialize due to the additional argument
    public void Initialize(Transaction item, Person person) {
        // But still call the base initializer:
        base.Initialize(item);

        Description = item.Description;
        Person = person;

        DialogManager.DisplayMode = SaveBarDisplayMode.SaveBarOnly;
        DialogManager.CancelWhenDirtyAlertDetails = new AlertDetails(
            "Are you certain?",
            "You seem unsure of yourself. Do you really want to cancel out of this dialog knowing that you will lose the changes you have made so far?",
            "No way", "Well, okay");
    }

    public override Transaction Update() {
        return new Transaction(Source.TransactionId) {
            TransactionDate = Source.TransactionDate,
            Amount = Source.Amount,
            Description = Description.Trim(),
        };
    }

    public override bool Validate() {
        DescriptionErrorText = string.IsNullOrEmpty(Description) ? "This field is required!" : string.Empty;

        bool noErrors = DescriptionErrorText == string.Empty;
        return base.Validate(noErrors);
    }

    public Person Person { get; private set; }

    public string Description { get => _Description; set => Set(ref _Description, value, shouldValidate: true); }
    string _Description;

    public string DescriptionErrorText { get => _DescriptionErrorText; set => Set(ref _DescriptionErrorText, value, setIsDirty: false); }
    string _DescriptionErrorText;
}
