# ViewModelToolkit

ViewModelToolkit is a .NET MAUI toolkit for users of the Model-View-ViewModel (MVVM) pattern. The package includes ViewModelBase, DialogManager and CoreNavigation, three components that can make a programmer's job much easier while providing a (relatively) simple, clean and consise codebase.

In addition to providing the abstract `ViewModelBase` class, the additional components make it easier to navigate to your own pages *modally*, treating each navigation as an atomic awaitable task. Gone are the days tracking whether the user hit the cancel button, back button or the device hardware button. No matter how the dialog is closed, control returns to the statement *after* the navigation call, returning the result of the dialog.

## ViewModelBase

At its most basic, VMT provides three ViewModelBase classes:

1. `ViewModelBase`, used for simple navigation to another page,
2. `ViewModelBase<T>`, used for task-based modal and modeless navigation to another page with a strongly typed ViewModel.
3. `ModalViewModelBase<T>`, used for task-based modal navigation to another page, adding toolbar items and/or a button bar with Save and Cancel buttons.

### `ViewModelBase` class

At the root of the hierarchy is `ViewModelBase`, which provides the basic functionality. An `IsDirty` property is managed by a `Set<T>` function that handles the `INotifyPropertyChanged` calls and sets the `IsDirty` flag automatically.

Basic usage of `ViewModel.Set<T>` within a property setter is illustrated here:

```cs
public string FullName { get => _FullName; set => Set(ref _FullName, value); }
string _FullName;
```
The optional argument `setAction: Action<T>` allows specification of an action to be invoked when the property changes. In the following example, the `SetFullName` method will be executed whenever either of the `FirstName` or `LastName` properties changes.

```cs
public string FirstName { get => _FirstName; set => Set(ref _FirstName, value, SetFullName); }
string _FirstName;

public string LastName { get => _LastName; set => Set(ref _LastName, value, SetFullName); }    
string _LastName;

void SetFullName(string _) => FullName = $"{FirstName} {LastName}";
```
The optional argument `setIsDirty: bool` allows a property to disable the default behavior where `IsDirty` is set to true. The default value is `true`.

#### `CoreNavigation.NavigateToPage<TPage, TViewModel>`

The `CoreNavigation` class provides the static method `NavigateToPage<TPage, TViewModel>` to make it easier to instantiate and navigate to a page. To do this manually, we'd need the following commands:

```cs
var pg = new SimplePage();
var vm = new SimplePageViewModel();
vm.Initialize();
pg.BindingContext = vm;
App.Current.MainPage.Navigation.PushAsync(pg);
```

Or you can simply use `NavigateToPage()`, which does all of that under the covers:

```cs
CoreNavigation.NavigateToPage<SimplePage, SimplePageViewModel>();
```

Either method works, but the latter is more consise, and it will allow for dependency injection when used for ViewModel instantiation. See **Dependency Injection** later in this document.

To stay in compliance with the MVVM design pattern, best practices would mandate that navigation methods such as the above be placed in a static navigation service class. Under MVVM, the ViewModel should not know anything about the View class, so we would do something like the following:

```cs
public static class NavigationService
{
    public static void GoToSimplePage(string text) {
        CoreNavigation.NavigateToPage<SimplePage, SimplePageViewModel>();
    }
```

which would be called in the ViewModel as such:

```cs
GoToSimplePage("Hello world!");
```

### `ViewModelBase<T>` class

The abstract base class `ViewModelBase<T>` provides support for strongly-typed ViewModels. The `Source` property holds the value passed to the ViewModel in the `Initialize(T)` method.

An optional `Update()` virtual function can be overriden to return a typed object representing the current state of the ViewModel notification properties.
 
An optional `Validate()` virtual function can be overriden to provide validation logic for the ViewModel. 

Here's an example of a strongly typed ViewModel of type `int`.

```cs
public class CustomPageViewModel : ViewModelBase<int>
{
    public override void Initialize(int item) {
        base.Initialize(item);
        NumberString = string.Empty;
    }

    public override int Update() {
        return int.TryParse(NumberString?.Trim(), out int result) ? result : 0;
    }

    public override bool Validate() {
        NumberStringErrorText = string.Empty;

        if ( string.IsNullOrWhiteSpace(NumberString) ) {
            NumberStringErrorText = "This field is required";
        } else {
            var result = Update();
            if ( result < 1 || result > 10 )
                NumberStringErrorText = $"{NumberString} is not an integer between 1 and 10.";
        }

        bool hasError = !string.IsNullOrWhiteSpace(NumberStringErrorText);
        return base.Validate(!hasError);
    }

    public string NumberString { get => _NumberString; set => Set(ref _NumberString, value, shouldValidate: true); }
    string _NumberString;

    public string NumberStringErrorText { get => _NumberStringErrorText; set => Set(ref _NumberStringErrorText, value, setIsDirty: false); }
    string _NumberStringErrorText;
}
```
The `Update()` function manages parsing from string to integer, returning the value of the number entered. And the `Validate()` function evaluates the state of the ViewModel properties. Note that the call to `base.Validate(bool)` sets a boolean flag named `IsValid` that can be used to determine the current state, for example in the `CanExecute()` declaration of the `ICommand` interface.

The `Set<T>` override adds another optional parameter, `shouldValidate: bool`. When set to true, the `Validate()` function will be called each time the property is set. The default behavior does not validate, but the Validate() method can be called manually at any time after initialization for more precise control of the validation process.

#### `CoreNavigation.NavigateToPage<TResult, TPage, TViewModel>`

The `CoreNavigation` class provides the static function `TResult NavigateToPage<TResult, TPage, TViewModel>` to make it easier to instantiate and navigate to a page managing a data `Source` item. To do this manually, we'd need the following commands:

```cs
var pg = new ModalPage();
var vm = new ModalPageViewModel<Person>(); // where ModalPageViewModel derives from ViewModelBase<T>
var person = new Person { FirstName = "John", LastName = "Smith" }
vm.Initialize(person); // initialize the ViewModelViewModel with the Person object
pg.BindingContext = vm;
App.Current.MainPage.Navigation.PushModalAsync(pg);
Person result = await vm.DialogManager.ExecuteModalTaskAsync();

```

The call to `DialogManager.ExecuteModalTaskAsync()` will be discussed later in the **`DialogManager`** section. Instead of typing these seven mostly boilerplate lines of code, the simpler approach would be to use `NavigateToPage<Page, PageViewModel>(person)`:

```cs
var result = await CoreNavigation.NavigateToPage<ModalPage, ModalPageViewModel>(person);
```
This would be implemented within our `NavigationService` as:

```cs
public static class NavigationService
{
    public static async Task<Person> GoToPersonDialogPageAsync(Person person) {
        return await CoreNavigation.NavigateToModalPageAsync<Person, PersonDialogPage, PersonDialogPageViewModel>(person);
    }
```
That would be invoked in the ViewModel as in this example:

```cs
Person result = await NavigationService.GoToPersonDialogPageAsync(person);
```
As discussed above, control will suspend within the ViewModel until the modal dialog page is dismissed, at which time the result will be returned.

### `ModalViewModelBase<T>` class

The abstract base class `ModalViewModelBase<T>`, in addition to providing the fuctionality of the `ViewModelBase<T>` class, adds `DialogManager` support to the mix. The `DialogManager` class will be discussed in next section. This base class is provided as a convenience, as it adds and implements the `IDialogSupport<T>` interface, and its use removes the need to manually declare and initialize the `DialogManager<T>` ViewModel member. However if you need to implement ViewModel inheritance, you will need to utilize `ViewModelBase<T>` and implement `IDialogSupport<T>` yourself. See **ViewModel Inheritance** later in this document.


## `DialogManager` class

The purpose of the `DialogManager` class is to encapsulate all of the goodies we would expect from a modal dialog into an easy to reuse component. In addition to providing Save and Cancel buttons, there are bindable properties for each button's `Text`, `Command` and `CommandParameter` properties, as well as others to control visual aspects of the buttons. (And bindable properties mean the `DialogManager` component can participate in XAML-based data binding.)

The `DialogManager` class is instantiated at the top of the ViewModel by adding and implementing the `IDialogSupport<T>` interface as is shown here:

```cs
public class ModalPageViewModel : ViewModelBase<Person>, IDialogSupport<Person>
{
    public DialogManager<Person> DialogManager { get; init; }

    public ModalPageViewModel() {
        DialogManager = new(this);
    }
    
    // implementation omitted
}

```
The `IDialogSupport<TResult>` interface requires a read/init `DialogManager<TResult>` property. Once declared, the `DialogManager` will collaborate with the View to present a default `SaveBarView` view. Page designers can add the default `SaveBarView` control (or any view implementing the 'ISaveBarView' interface). Or not, and one will be added automagically. Control freaks can even override the save bar` injection entirely with their own implementation of the `ISaveBarView` interface.


### The hidden `ToolbarManager` component

A dialog page would not be complete without providing a means for users to accept changes or cancel out entirely. But where on the screen those buttons should be depends on the device platform and idiom. Desktop computers traditionally have a button bar below the page content, while mobile devices have ToolbarItem buttons that are displayed in the navigation bar. Sometimes you want both. Or the other. Or neither. That's all configurable thanks to the `ToolbarManager` component.

You'll never directly deal with `ToolbarManager`, but buried under the `DialogManager` hood, it takes care of managing which Save and Cancel buttons are displayed, as well as the commands and text strings to display in the buttons.

By default, `ToolbarItem` views will be used for mobile devices and the internal `SaveBarView` will be used for desktop environments. The `DisplayMode` property allows the developer to configure what buttons the `ToolbarManager` will display any combination of  `ToolbarItem` controls in the navigation bar and `ISaveBarView`. The default can be configured application-wide by calling the `CoreNavigation.ConfigureDefaultButtonBarDisplayMode(SaveBarDisplayMode)` method.



### `ISaveBarView` implementation

During configuration, `DialogManager` searches for an instance of ISaveBarView within the `ContentPage` visual tree. If it cannot find one, it will create an instance of `ViewModelToolkit.Views.SaveBarView` and insert it in the page at the end of the first `Layout` derived control it finds. Depending on the page design, this may be unsuitable behavior.

The page designer can insert a `SaveBarView` in a ContentPage using XAML syntax:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:ViewModelToolkitSample.ViewModels"
             xmlns:vmtv="clr-namespace:ViewModelToolkit.Views;assembly=ViewModelToolkit"
             x:Class="ViewModelToolkitSample.Views.ModalNavigationPage"
             x:DataType="vm:ModalNavigationPageViewModel"
             Title="Modal Navigation Page">
    <ScrollView>
        <VerticalStackLayout>
            <!-- page content redacted -->
            
            <vmtv:SaveBarView SaveButtonText="Submit"/>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>

```
The `Text`, `Command`, `CommandParameter` and other properties on `SaveBarView` are **bindable properties**, which means you can use data binding to set the Save and Cancel button properties from the ViewModel. Although these properties can be set using the DialogManager property we've already seen declared in the ViewModel itself. More about that in the **ViewModel Configuration** section later in this document.

For the utmost in customizability, the developer can design their own save bar. They need only implement the `ISaveBarView` interface, which ensures the Save and Cancel button properties are declared. Note that these buttons don't need to be actual buttons: any control can be used provided it supports the commanding structure as described above. Even an image with a hit map could be used, and the `ISaveBarButtonView` interface is provided to simplify the task of developing the custom save bar buttons.

The ViewModelToolkitSample application contains an example of a custom save bar that is vertically oriented and includes an additional Help button. 

#### ISaveBarView Custom Injection

If the developer would like to provide an instance of an `ISaveBarView` implemented control at **runtime**, the `CoreNavigation.NavigateToModalPageAsync` function has an optional parameter, `saveBarInjector: Func<TPage, ISaveBarView>` to facilitate this. When provided, the injector function will provide the `ContentPage` instance. The developer should instantiate their own ISaveBarView class here and insert it into the visual tree, returning a reference to the custom save bar.

The ViewModelToolkitSample also contains an example of this usage in the function `NavigationService.GoToComplicatedPageAsync`.

## ViewModel Customization

Since "Save" and "Cancel" are not always the most suitable strings for the buttons, `DialogManager` allows the developer to customize the text and commands used on a per-ViewModel basis. While these changes can be declared in XAML as mentioned above, this allows customization to be made in the ViewModel (where some could argue it belongs in MVVM).

For example, a "wizard-style" multi-page dialog would need to provide a custom `SaveButtonCommand` to provide navigation to the next page of the wizard. Here's what that could look like:

```cs
public override void Initialize(Person item) {
    base.Initialize(item);
    // initialization redacted

    DialogManager.DisplayMode = SaveBarDisplayMode.BothButtonBarAndToolBar;
    DialogManager.SaveButtonText = "Continue";
    DialogManager.SaveButtonCommand = ContinueCommand;
}
```
where `ContinueCommand` is declared as:

```cs
public Command ContinueCommand => _ContinueCommand ??= new Command(async p => {
    if ( Validate() ) {
        Person result = await NavigationService.GoToMultipleStepTwoPageAsync(Update());
        if ( !result.IsDefault() ) {
            InitializeCleanly(() => Initialize(result));
            DialogManager.ExecuteDefaultSaveButtonCommand();
        }
    }
}, _ => IsDirty && IsValid);
Command _ContinueCommand;
```

This example can also be found in project ViewModelToolkitSample class `MultipleStepOnePageViewModel`. 


## ViewModel Inheritance

More complex applications can require higher levels of inheritence among ViewModels. 



```



















```

That example begins in our project's `NavigationService` class with the function:

```cs
public static async Task<Transaction> GoToComplicatedPageAsync(Transaction transaction, Person person) {
    return await CoreNavigation.NavigateToModalPageAsync<Transaction, ComplicatedPage, ComplicatedPageViewModel>
        (transaction,
         initialization: (p, vm) => vm.Initialize(transaction, person),
         saveBarInjector: SaveBarInjector);
}
```


