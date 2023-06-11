using ViewModelToolkit;
using ViewModelToolkit.Dialogs;
using ViewModelToolkit.Services;
using ViewModelToolkitSample.Models;
using ViewModelToolkitSample.Services;

namespace ViewModelToolkitSample.ViewModels;

public class EditCustomerStep1PageViewModel : CustomerViewModelBase
{
    const int MINIMUM_ACCOUNT_HOLDER_AGE = 18;

    readonly DateTime defaultPickerDateTime = DateTime.Today.AddYears(-MINIMUM_ACCOUNT_HOLDER_AGE);
    readonly IExceptionService _exceptionService;

    public EditCustomerStep1PageViewModel(IExceptionService exceptionService) {
        _exceptionService = exceptionService;
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

    public string FirstNameErrorText { get => _FirstNameErrorText; set => Set(ref _FirstNameErrorText, value, setIsDirty: false); }
    string _FirstNameErrorText;

    public string LastNameErrorText { get => _LastNameErrorText; set => Set(ref _LastNameErrorText, value, setIsDirty: false); }
    string _LastNameErrorText;

    public Command ContinueCommand => _ContinueCommand ??= new Command(async p => {
        try {
            if ( Validate() ) {
                Customer current = Update();
                Customer result = await NavigationService.GoToEditCustomerStep2PageAsync(current);
                if ( !result.IsDefault() ) {
                    // Reinitialize without changing the IsDirty flag:
                    ExecuteCleanly(() => Initialize(result));
                    DialogManager.ExecuteDefaultSaveButtonCommand();
                }
            }
        } catch ( Exception ex ) {
            _exceptionService.HandleException(ex);
        }
    }, _ => !IsNewAccount || (IsDirty && IsValid));
    Command _ContinueCommand;
}
