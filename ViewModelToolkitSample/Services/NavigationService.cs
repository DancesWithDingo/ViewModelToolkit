using ViewModelToolkit.Dialogs;
using ViewModelToolkit.Views;
using ViewModelToolkitSample.Models;
using ViewModelToolkitSample.ViewModels;
using ViewModelToolkitSample.Views;

namespace ViewModelToolkitSample.Services;

public static class NavigationService
{
    public static void GoToSimpleNavigationPage(string text) {
        CoreNavigation.NavigateToPage<string, SimpleNavigationPage, SimpleNavigationPageViewModel>(text);
    }


    public static async Task<int> GoToPickANumberPageAsync() =>
        await CoreNavigation.NavigateToModalPageAsync<int, PickANumberPage, PickANumberPageViewModel>
            (-1, nullResultHandling: NullResultHandling.ReturnInput);


    public static async Task<Customer> GoToEditCustomerStep1PageAsync(Customer customer) =>
        await CoreNavigation.NavigateToModalPageAsync<Customer, EditCustomerStep1Page, EditCustomerStep1PageViewModel>
            (customer);


    public static async Task<Customer> GoToEditCustomerStep2PageAsync(Customer customer) =>
        await CoreNavigation.NavigateToModalPageAsync<Customer, EditCustomerStep2Page, EditCustomerStep2PageViewModel>
            (customer,
             shouldSuppressReturnNavigationAnimation: p => !p.IsDefault());


    public static async Task<Customer> GoToEditCustomerStep3PageAsync(Customer customer) =>
        await CoreNavigation.NavigateToModalPageAsync<Customer, EditCustomerStep3Page, EditCustomerStep3PageViewModel>
            (customer,
             shouldSuppressReturnNavigationAnimation: p => !p.IsDefault());


    public static async Task<Transaction> GoToCustomFormPageAsync(Transaction transaction, Person person) {
        return await CoreNavigation.NavigateToModalPageAsync<Transaction, CustomFormPage, CustomFormPageViewModel>
            (transaction,
             initialization: (p, vm) => vm.Initialize(transaction, person),
             saveBarInjector: SaveBarInjector);

        static ISaveBarView SaveBarInjector(CustomFormPage page) {
            var saveBar = new CustomSaveBarView();

            saveBar.HelpButton.Command = new Command(async () => await page.DisplayAlert("Help", "This is a help page.", "Close"));

            if ( page.FindByName("SaveBarLayout") is Grid layout )
                layout.Add(saveBar);
            else
                throw new NullReferenceException("Could not find a Grid named \"SaveBarLayout\")");

            return saveBar;
        }
    }
}
