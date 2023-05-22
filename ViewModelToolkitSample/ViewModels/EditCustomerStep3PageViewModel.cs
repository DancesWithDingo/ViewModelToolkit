using ViewModelToolkit.Dialogs;
using ViewModelToolkitSample.Models;

namespace ViewModelToolkitSample.ViewModels;

public class EditCustomerStep3PageViewModel : CustomerViewModelBase
{
    public override void Initialize(Customer item) {
        base.Initialize(item);

        DialogManager.DisplayMode = SaveBarDisplayMode.SaveBarOnly;
        DialogManager.CancelButtonText = "Back";
        DialogManager.SaveButtonText = IsNewAccount ? "Add Account" : "Update Account";
        DialogManager.IsSaveButtonAlwaysEnabled = true;
        DialogManager.ShouldCancelIgnoreIsDirty = true;
    }
}
