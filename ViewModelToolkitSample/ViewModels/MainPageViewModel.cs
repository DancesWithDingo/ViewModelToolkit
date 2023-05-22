using ViewModelToolkit.ViewModels;
using ViewModelToolkitSample.Models;
using ViewModelToolkitSample.Services;

namespace ViewModelToolkitSample.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public override void Initialize() {
        base.Initialize();
    }

#pragma warning disable CA1822 // Mark members as static
    public string CustomFormPageDescription => GetCustomFormDescription();
    public string CustomerEditorDescription => GetCustomerEditorDescription();
    public string PickANumberDescription => GetPickANumberDescription();
    public string SimpleNavigationDescription => GetSimpleNavigationDescription();
#pragma warning restore CA1822 // Mark members as static

    #region Notification Properties

    public string CustomerEditorPageResultText { get => _CustomerEditorPageResultText; set => Set(ref _CustomerEditorPageResultText, value); }
    string _CustomerEditorPageResultText;

    public string CustomFormPageResultText { get => _CustomFormPageResultText; set => Set(ref _CustomFormPageResultText, value); }
    string _CustomFormPageResultText;

    public string PickANumberResultText { get => _PickANumberResultText; set => Set(ref _PickANumberResultText, value); }
    string _PickANumberResultText;

    public string SimpleNavigationText { get => _SimpleNavigationText; set => Set(ref _SimpleNavigationText, value); }
    string _SimpleNavigationText;

    #endregion

    #region Commands

    public Command CustomerEditorPageCommand => _CustomerEditorPageCommand ??= new Command(async p => {
        bool isNew = p is string st && st == "new";

        Customer customer = isNew ? new() : new() {
            FirstName = "Testy",
            LastName = "McTestface",
            BirthDate = DateTime.Today.AddYears(-60),
            AccountId = Guid.NewGuid(),
            AnniversaryDate = DateTime.Today.AddDays(-365 * 4 - 94),
            LoyaltyPoints = 120_980,
        };

        var result = await NavigationService.GoToEditCustomerStep1PageAsync(customer);
        CustomerEditorPageResultText = result is null ? "The dialog was cancelled." : GetResultText(result);

        if ( result?.AccountId == Guid.Empty )
            result.AccountId = Guid.NewGuid();

        static string GetResultText(Customer result) {
            return $"{(result.AccountId == Guid.Empty ? "New" : "Exisiting")} customer:{Environment.NewLine}{result.SortName} (DOB: {result.BirthDate:d}){Environment.NewLine}"
                 + $"Account opened: {(result.AnniversaryDate == DateTime.Today ? "Today" : result.AnniversaryDate.ToString("d"))}, points: {result.LoyaltyPoints:N0}";
        }
    });
    Command _CustomerEditorPageCommand;

    public Command CustomFormPageCommand => _CustomFormPageCommand ??= new Command(async _ => {
        var person = new Person {
            FirstName = "John",
            LastName = "Smith",
            BirthDate = DateTime.Now.AddYears(-40).AddDays(-1)
        };

        var transaction = new Transaction {
            TransactionDate = DateTime.Now.AddHours(-1.5),
            Amount = 2000
        };

        var result = await NavigationService.GoToCustomFormPageAsync(transaction, person);
        CustomFormPageResultText = result is null ? "The dialog was cancelled." : $"Result => {person.FullName}: {result.Amount:C}, Description: \"{result.Description}\"";
    });
    Command _CustomFormPageCommand;

    public Command PickANumberCommand => _PickANumberCommand ??= new Command(async _ => {
        var result = await NavigationService.GoToPickANumberPageAsync();
        PickANumberResultText = result == -1 ? "The dialog was cancelled." : $"You chose {result}";
    });
    Command _PickANumberCommand;

    public Command SimpleNavigationCommand => _SimpleNavigationCommand ??= new Command(p => {
        if ( p is string st )
            NavigationService.GoToSimpleNavigationPage(st);
    }, p => !string.IsNullOrWhiteSpace((string)p));
    Command _SimpleNavigationCommand;

    #endregion

    #region Locals

    static string GetCustomFormDescription() =>
        "The final example shows how to make use of custom ViewModel initialization to pass additional "
        + "data to a ViewModel, in this case a Person object. The Source item for the ViewModel is of "
        + "type Transaction, and the Person is passed to a new virtual Initialize method. Additionally, "
        + "it shows how to create and inject a custom SaveBarView instance with vertical buttons and "
        + "an additional help button. (Note that this is merely demonstrating how to inject a view "
        + "into the page. In a real world application, the custom save bar would more likely be declared "
        + "in XAML.";

    static string GetCustomerEditorDescription() =>
        "This example is a three-page \"wizard\"-like dialog. It shows how to provide your own "
        + "command to provide the forward navigation required in a multiple-page dialog. "
        + "It also illustrates how to use ViewModel inheritance to simplify the design and reduce "
        + "the need for redundant code.";

    static string GetPickANumberDescription() =>
        "This example represents a common but difficult task: that of prompting the user "
        + "with a simple dialog. The sample page ViewModel derives from ModalViewModelBase<int> "
        + "with a Source property of type int. When you enter a value, the Update() and Validate() "
        + "functions work together to parse the entered text value to an integer, while providing a "
        + "simple validation process.";

    static string GetSimpleNavigationDescription() =>
        "This example shows a simple push to a receiving ContentPage. The receiving ViewModel derives from "
        + "ViewModelBase<string>, and the string value you enter will be passed to the second page Source "
        + "property via Initialize(string) method. Note that type of ViewModelBase<T> can be string, int "
        + $"or even class objects.";

    #endregion
}
