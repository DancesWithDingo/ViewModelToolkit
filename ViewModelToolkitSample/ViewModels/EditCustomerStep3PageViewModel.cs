using ViewModelToolkit.Modals;
using ViewModelToolkit.ViewModels;
using ViewModelToolkitSample.Models;
using ViewModelToolkitSample.ViewModels;

namespace ViewModelToolkitSample.ViewModels;

public class EditCustomerStep3PageViewModel : CustomerViewModelBase, IDialogSupport<Customer>
{
    public DialogManager<Customer> DialogManager { get; init; }

    public EditCustomerStep3PageViewModel() {
        DialogManager = new(this);
    }

    public override void Initialize(Customer item) {
        base.Initialize(item);

        PageTitleText = $"{(IsNewAccount ? "Add new" : "Edit")} Customer";

        DialogManager.DisplayMode = SaveBarDisplayMode.SaveBarOnly;
        DialogManager.CancelButtonText = "Back";
        DialogManager.SaveButtonText = IsNewAccount ? "Add Account" : "Update Account";
        DialogManager.IsSaveButtonAlwaysEnabled = true;
        DialogManager.ShouldCancelIgnoreIsDirty = true;
    }

    public string PageTitleText { get; private set; }
}
