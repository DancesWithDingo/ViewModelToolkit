using ViewModelToolkit.ViewModels;
using ViewModelToolkitSample.Models;
using ViewModelToolkitSample.Services;

namespace ViewModelToolkitSample.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public override void Initialize() {
        base.Initialize();
    }

    public string SimpleNavigationDescription => GetSimpleNavigationDescription();
    public string PickANumberDescription => GetPickANumberDescription();

    #region Notification Properties

    public string CustomerEditorPageResultText { get => _CustomerEditorPageResultText; set => Set(ref _CustomerEditorPageResultText, value); }
    string _CustomerEditorPageResultText;

    public string ComplicatedPageResultText { get => _ComplicatedPageResultText; set => Set(ref _ComplicatedPageResultText, value); }
    string _ComplicatedPageResultText;

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
            AnniversaryDate = DateTime.Today.AddDays(-365 * 4 - 90),
            LoyaltyPoints = 12_490_980,
        };

        var result = await NavigationService.GoToEditCustomerStep1PageAsync(customer);
        CustomerEditorPageResultText = result is null
            ? "The dialog was cancelled."
            : GetResultText(result);

        if ( result?.AccountId == Guid.Empty )
            result.AccountId = Guid.NewGuid();


        static string GetResultText(Customer result) {
            return $"{(result.AccountId == Guid.Empty ? "New" : "Exisiting")} customer:{Environment.NewLine}{result.SortName} (DOB: {result.BirthDate:d}){Environment.NewLine}"
                 + $"Account opened: {(result.AnniversaryDate == DateTime.Today ? "Today" : result.AnniversaryDate.ToString("d"))}, points: {result.LoyaltyPoints:N0}";
        }
    });
    Command _CustomerEditorPageCommand;

    public Command ComplicatedPageCommand => _ComplicatedPageCommand ??= new Command(async _ => {
        var person = new Person {
            FirstName = "John",
            LastName = "Smith",
            BirthDate = DateTime.Now.AddYears(-40).AddDays(-1)
        };

        var transaction = new Transaction {
            TransactionDate = DateTime.Now.AddHours(-1.5),
            Amount = 2000
        };

        var result = await NavigationService.GoToComplicatedPageAsync(transaction, person);
        ComplicatedPageResultText = result is null
            ? "The dialog was cancelled."
            : $"Result => {person.FullName}: {result.Amount:C}, Description: \"{result.Description}\"";
    });
    Command _ComplicatedPageCommand;

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

    static string GetSimpleNavigationDescription() =>
        "This example shows a simple push to a receiving ContentPage. The receiving ViewModel derives from "
        + "ViewModelBase<string>, and the string value you enter will be passed to the second page Source "
        + "property via Initialize(string) method. Note that type of ViewModelBase<T> can be string, int "
        + $"or even classes.{Environment.NewLine + Environment.NewLine}This is a very simple example, "
        + "however it's important to notice how little code is involved in accomplishing this task.";

    static string GetPickANumberDescription() =>
        "This action represents a common but difficult task: that of prompting the user "
        + "with a simple dialog. The sample page ViewModel derives from ModalViewModelBase<int> "
        + "with a Source property of type int. When you enter a value, the Update() and Validate() "
        + "functions work together to parse the entered text value to an integer, while providing a "
        + "simple validation process.";
    #endregion
}
