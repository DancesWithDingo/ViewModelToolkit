using ViewModelToolkit.Dialogs;
using ViewModelToolkit.ViewModels;
using ViewModelToolkitSample.Models;

namespace ViewModelToolkitSample.ViewModels;

public class CustomerViewModelBase : ViewModelBase<Customer>, IDialogSupport<Customer>
{
    public DialogManager<Customer> DialogManager { get; init; }

    readonly DateTime defaultPickerDateTime = new(1900, 1, 1);

    public CustomerViewModelBase() {
        DialogManager = new(this);
    }

    public override void Initialize(Customer item) {
        base.Initialize(item);

        IsNewAccount = item.AccountId == default;
        AccountIdText = IsNewAccount ? "(new account)" : item.AccountId.ToString();

        AccountId = item.AccountId;
        FirstName = item.FirstName;
        LastName = item.LastName;
        BirthDate = item.BirthDate == DateTime.MinValue ? defaultPickerDateTime : item.BirthDate;
        AnniversaryDate = item.AnniversaryDate;
        LoyaltyPoints = item.LoyaltyPoints;

        PageTitleText = $"{(IsNewAccount ? "Add new" : "Edit")} Customer";
    }

    public override Customer Update() {
        return new Customer {
            AccountId = AccountId,
            FirstName = FirstName?.Trim(),
            LastName = LastName?.Trim(),
            BirthDate = BirthDate,
            AnniversaryDate = AnniversaryDate == default ? DateTime.Now.Date : AnniversaryDate,
            LoyaltyPoints = int.TryParse(LoyaltyPointsText, out int r) ? r : 0,
        };
    }

    #region Properties

    public Guid AccountId { get; private set; }
    public string AccountIdText { get; set; }
    public bool IsNewAccount { get; private set; }
    public string PageTitleText { get; private set; }

    #endregion

    #region Notification Properties

    public DateTime AnniversaryDate { get => _AnniversaryDate; set => Set(ref _AnniversaryDate, value, shouldValidate: true); }
    DateTime _AnniversaryDate;

    public DateTime BirthDate { get => _BirthDate; set => Set(ref _BirthDate, value, shouldValidate: true); }
    DateTime _BirthDate;

    public string FirstName { get => _FirstName; set => Set(ref _FirstName, value, SetFullName, shouldValidate: true); }
    string _FirstName;

    public string FullName { get => _FullName; set => Set(ref _FullName, value); }
    string _FullName;

    public string LastName { get => _LastName; set => Set(ref _LastName, value, SetFullName, shouldValidate: true); }
    string _LastName;

    public int LoyaltyPoints { get => _LoyaltyPoints; set => Set(ref _LoyaltyPoints, value, SetLoyaltyPointsText); }
    void SetLoyaltyPointsText(int points) => LoyaltyPointsText = points.ToString();
    int _LoyaltyPoints;

    public string LoyaltyPointsText { get => _LoyaltyPointsText; set => Set(ref _LoyaltyPointsText, value); }
    string _LoyaltyPointsText;

    #endregion

    #region Locals

    void SetFullName(string _) => FullName = $"{this.Update().FullName}";

    #endregion
}
