using ViewModelToolkit.Dialogs;
using ViewModelToolkitSample.Models;
using ViewModelToolkitSample.Services;

namespace ViewModelToolkitSample.ViewModels;

public class EditCustomerStep1PageViewModel : CustomerViewModelBase
{
    const int MINIMUM_ACCOUNT_HOLDER_AGE = 18;

    readonly DateTime defaultPickerDateTime = DateTime.Today.AddYears(-MINIMUM_ACCOUNT_HOLDER_AGE);

    public EditCustomerStep1PageViewModel() {
        DialogManager = new(this);
        MinimumBirthDate = defaultPickerDateTime;
    }

    public override void Initialize(Customer item) {
        base.Initialize(item);

        BirthDate = item.BirthDate == default ? DateTime.Today : item.BirthDate;

        DialogManager.DisplayMode = SaveBarDisplayMode.SaveBarOnly;
        DialogManager.SaveButtonText = "Continue";
        DialogManager.SaveButtonCommand = ContinueCommand;
    }

    public override bool Validate() {
        var result = Update();

        FirstNameErrorText = result.FirstName == string.Empty ? FirstNameErrorText = "First name is required." : string.Empty;
        LastNameErrorText = result.LastName == string.Empty ? LastNameErrorText = "Last name is required." : string.Empty;

        bool noErrors =
            result.FirstName is not null && result.LastName is not null &&
            result.BirthDate != defaultPickerDateTime &&
            FirstNameErrorText + LastNameErrorText == string.Empty;

        return base.Validate(noErrors);
    }

    public DateTime MinimumBirthDate { get; init; }

    public Command ContinueCommand => _ContinueCommand ??= new Command(async p => {
        if (Validate() ) {
            Customer input = Update();
            Customer result = await NavigationService.GoToEditCustomerStep2PageAsync(input);
            if ( !result.IsDefault() ) {
                InitializeCleanly(() => Initialize(result));
                DialogManager.ExecuteDefaultSaveButtonCommand();
            }
        }
    }, _ => !IsNewAccount || (IsDirty && IsValid));
    Command _ContinueCommand;
}
