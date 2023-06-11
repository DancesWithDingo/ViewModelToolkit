using ViewModelToolkit;
using ViewModelToolkit.Dialogs;
using ViewModelToolkit.Services;
using ViewModelToolkitSample.Models;
using ViewModelToolkitSample.Services;

namespace ViewModelToolkitSample.ViewModels;

public class EditCustomerStep2PageViewModel : CustomerViewModelBase
{
    readonly IExceptionService _exceptionService;

    public EditCustomerStep2PageViewModel(IExceptionService exceptionService) {
        _exceptionService = exceptionService;
    }

    public override void Initialize(Customer item) {
        base.Initialize(item);
        IsLoyaltyPointsBarVisible = IsNewAccount;

        DialogManager.DisplayMode = SaveBarDisplayMode.SaveBarOnly;
        DialogManager.CancelButtonText = "Back";
        DialogManager.SaveButtonText = "Continue";
        DialogManager.SaveButtonCommand = ContinueCommand;
        DialogManager.ShouldCancelIgnoreIsDirty = true;
    }

    public override Customer Update() {
        var result = base.Update();
        result.AnniversaryDate = AnniversaryDate;
        result.LoyaltyPoints = LoyaltyPoints;
        return result;
    }

    public override bool Validate() => base.Validate(true);

    public bool IsExistingAccount => !IsNewAccount;
    public bool IsLoyaltyPointsBarVisible { get => _IsLoyaltyPointsBarVisible; set => Set(ref _IsLoyaltyPointsBarVisible, value); }
    bool _IsLoyaltyPointsBarVisible = true;

    public Command ContinueCommand => _ContinueCommand ??= new Command(async p => {
        try {
            if ( Validate() ) {
                Customer result = await NavigationService.GoToEditCustomerStep3PageAsync(Update());
                if ( !result.IsDefault() ) {
                    ExecuteCleanly(() => Initialize(result));
                    DialogManager.ExecuteDefaultSaveButtonCommand();
                }
            }
        } catch ( Exception ex ) {
            _exceptionService.HandleException(ex);
        }
    }, _ => !IsNewAccount || (IsDirty && IsValid));
    Command _ContinueCommand;

    public Command EditPointsCommand => _EditPointsCommand ??= new Command(async p => {
        string response = await AlertService.PromptForPointsAsync(LoyaltyPoints);
        if ( !response.IsDefault() ) {
            if ( int.TryParse(response, out int result) )
                LoyaltyPoints = result;
            else
                await AlertService.AlertInvalidLoyaltyPointsInputAsync(response);
        }
    });
    Command _EditPointsCommand;

    public Command RewardPointsCommand => _RewardPointsCommand ??= new Command(p => {
        if ( p is string st && int.TryParse(st, out int points) ) {
            LoyaltyPoints = points;
            IsLoyaltyPointsBarVisible = false;
        }
    }, _ => LoyaltyPoints == 0);
    Command _RewardPointsCommand;
}
