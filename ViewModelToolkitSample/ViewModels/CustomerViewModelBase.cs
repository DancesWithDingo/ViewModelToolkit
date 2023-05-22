using ViewModelToolkit.ViewModels;
using ViewModelToolkitSample.Models;

namespace ViewModelToolkitSample.ViewModels;

public class CustomerViewModelBase : ViewModelBase<Customer>
{
    readonly DateTime defaultPickerDateTime = new(1900, 1, 1);

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

    public override bool Validate() {
        var result = Update();

        FirstNameErrorText = string.IsNullOrEmpty(result.FirstName) ? FirstNameErrorText = "First name is required." : string.Empty;
        LastNameErrorText = string.IsNullOrEmpty(result.LastName) ? LastNameErrorText = "Last name is required." : string.Empty;

        BirthDateErrorText = BirthDate == defaultPickerDateTime ? "Date of birth is required." : string.Empty;
        AnniversaryDateErrorText = BirthDate == defaultPickerDateTime ? "Anniverary date is required." : string.Empty;

        LoyaltyPointsErrorText = string.IsNullOrEmpty(LoyaltyPointsText)
            ? "This field is required!"
            : result.LoyaltyPoints < 0
                ? "Loyalty points must be 0 or greater!"
                : string.Empty;

        bool noErrors = FirstNameErrorText + LastNameErrorText + BirthDateErrorText
                      + AnniversaryDateErrorText + LoyaltyPointsErrorText == string.Empty;

        return base.Validate(noErrors);
    }

    #region Properties

    public Guid AccountId { get; private set; }
    public string AccountIdText { get; set; }
    public bool IsNewAccount { get; private set; }

    #endregion

    #region Notification Properties

    public DateTime AnniversaryDate { get => _AnniversaryDate; set => Set(ref _AnniversaryDate, value, shouldValidate: true); }
    DateTime _AnniversaryDate;

    public string AnniversaryDateErrorText { get => _AnniversaryDateErrorText; set => Set(ref _AnniversaryDateErrorText, value); }
    string _AnniversaryDateErrorText;

    public DateTime BirthDate { get => _BirthDate; set => Set(ref _BirthDate, value, shouldValidate: true); }
    DateTime _BirthDate;

    public string BirthDateErrorText { get => _BirthDateErrorText; set => Set(ref _BirthDateErrorText, value, setIsDirty: false); }
    string _BirthDateErrorText;

    public string FirstName { get => _FirstName; set => Set(ref _FirstName, value, SetFullName, shouldValidate: true); }
    string _FirstName;

    public string FirstNameErrorText { get => _FirstNameErrorText; set => Set(ref _FirstNameErrorText, value, setIsDirty: false); }
    string _FirstNameErrorText;

    public string FullName { get => _FullName; set => Set(ref _FullName, value); }
    string _FullName;

    public string LastName { get => _LastName; set => Set(ref _LastName, value, SetFullName, shouldValidate: true); }
    string _LastName;

    public string LastNameErrorText { get => _LastNameErrorText; set => Set(ref _LastNameErrorText, value, setIsDirty: false); }
    string _LastNameErrorText;

    public int LoyaltyPoints { get => _LoyaltyPoints; set => Set(ref _LoyaltyPoints, value, SetLoyaltyPointsText); }
    void SetLoyaltyPointsText(int points) => LoyaltyPointsText = points.ToString();
    int _LoyaltyPoints;

    public string LoyaltyPointsErrorText { get => _LoyaltyPointsErrorText; set => Set(ref _LoyaltyPointsErrorText, value, setIsDirty: false); }
    string _LoyaltyPointsErrorText;

    public string LoyaltyPointsText { get => _LoyaltyPointsText; set => Set(ref _LoyaltyPointsText, value); }
    string _LoyaltyPointsText;

    #endregion

    #region Locals

    void SetFullName(string _) => FullName = $"{this.Update().FullName}";

    #endregion
}
