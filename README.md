# ViewModelToolkit

ViewModelToolkit is a .NET MAUI toolkit for C# developers using the Model-View-ViewModel (MVVM) pattern. The package includes ViewModelBase, DialogManager and CoreNavigation, three components that can make a programmer's job much easier while providing a (relatively) simple, clean and consise codebase.

In addition to providing the abstract `ViewModelBase` class, the additional components make it easier to navigate to pages *modally*, treating each navigation as an atomic awaitable task. Gone are the days tracking whether the user hit the cancel button, back button or the device hardware button. No matter how the user closes the dialog, control returns to the statement *after* the navigation call, returning the result of the dialog.

## ViewModelBase

At its most basic, VMT provides three ViewModelBase base classes:

1. `ViewModelBase`, used for simple navigation to another page,
2. `ViewModelBase<T>`, used for simple navigation to another page with a strongly typed model object passed to the `Initialize` method,
3. `ModalViewModelBase<T>`, used for task-based modal navigation to another page, adding toolbar items and/or a button bar with Save and Cancel buttons.

### `ViewModelBase` class

At the root of the hierarchy is `ViewModelBase`, which provides the basic ViewModel lifecycle functionality. An `IsDirty` property is managed by the `Set<T>` function that handles the `INotifyPropertyChanged` calls and sets the `IsDirty` flag automatically.

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
The optional argument `setIsDirty: bool` allows a property to disable the default behavior where `IsDirty` is set to true automatically. The default value is `true`.

### Simple Navigation

The `CoreNavigation` class provides the static method `NavigateToPage<TView, TViewModel>` to make it easier to instantiate and navigate to another ContentPage. To do this manually, we'd need the following statements:

```cs
var pg = new SimplePage();
var vm = new SimplePageViewModel();
vm.Initialize();
pg.BindingContext = vm;
App.Current.MainPage.Navigation.PushAsync(pg);
```

Or you can simply use `NavigateToPage()` which does all of that under the hood:

```cs
CoreNavigation.NavigateToPage<SimplePage, SimplePageViewModel>();
```

Both methods get the job done, but the latter is more consise and will be more flexible for use of dependency injection in more complex scenarios. See [**Dependency Injection**](#dependency-injection) later in this document.

To stay in compliance with the MVVM design pattern, best practices would mandate that navigation methods such as the above be placed in a static navigation service class. Under MVVM, the ViewModel should not know anything about the View class, so we would do something like the following:

```cs
public static class NavigationService
{
    public static void GoToSimplePage(string text) =>
        CoreNavigation.NavigateToPage<SimplePage, SimplePageViewModel>();
```

which would be called in the ViewModel as such:

```cs
NavigationService.GoToSimplePage("Hello world!");
```

### `ViewModelBase<T>` class

The abstract base class `ViewModelBase<T>` provides support for strongly-typed ViewModels. The `Source` property holds the model value passed to the ViewModel in the `Initialize(T)` method. `Source` is intended to describe the data at the time of initialization, and as such should remain static throughout the ViewModel lifecycle.

The `Update` virtual function should be overriden to return a typed object representing the current state of the ViewModel notification properties.
 
An optional `Validate()` virtual function can be overriden to provide validation logic for the ViewModel.

Here's an example of a strongly typed ViewModel of type `int`.

```cs
public class CustomPageViewModel : ViewModelBase<int>
{
    public override void Initialize(int item) {
        base.Initialize(item); // Sets the Source property
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

        bool noErrors = string.IsNullOrWhiteSpace(NumberStringErrorText);
        return base.Validate(noErrors); // Sets the IsValid property
    }

    public string NumberString { get => _NumberString; set => Set(ref _NumberString, value, shouldValidate: true); }
    string _NumberString;

    public string NumberStringErrorText { get => _NumberStringErrorText; set => Set(ref _NumberStringErrorText, value, setIsDirty: false); }
    string _NumberStringErrorText;
}
```
When writing an `Initialize` method, keep in mind that it can be called more than once in a page's lifecycle. As such, it's important to ensure that the method cleans up any class-wide properties (such as observable collections, event handlers, etc.) before doing any initialization. And to repeat, the `Source` property is intented to represent the object passed in through `Initialize` and thus should not be changed unless reinitializing the ViewModel.

The `Update` function manages parsing from string to integer, returning the value of the number entered. And the `Validate` function evaluates the state of the ViewModel properties. Note that the call to `base.Validate(bool)` sets a boolean flag named `IsValid` that can be used to determine the current state, for example in the `CanExecute` declaration of the `ICommand` interface.

Within the NumberString property declaration, the `Set<T>` override adds another optional parameter, `shouldValidate: bool`. When set to true, the `Validate` function will be called each time the property is set. The default behavior does not validate, and the `Validate` method can be called manually at any time after initialization for more precise control of the validation process.

### Model-based Simple Navigation

The `CoreNavigation` class provides the static function `TModel NavigateToPage<TModel, TView, TViewModel>` to make it easier to instantiate and navigate to a page managing a data `Source` item. To do this manually, we'd need the following commands:

### Model-based Dialog Navigation

The `CoreNavigation` class provides the static function `TModel NavigateToModalPage` to make it easier to instantiate and navigate to a **modal** page managing a `Source` model item. Hand coding this would require the following commands:

```cs
var pg = new EditPersonPage();
var vm = new EditPersonPageViewModel(); // where EditPersonPageViewModel derives from ViewModelBase<Person>
var person = new Person { FirstName = "John", LastName = "Smith" }
vm.Initialize(person); // initialize the ViewModel with the Person object
pg.BindingContext = vm;
App.Current.MainPage.Navigation.PushModalAsync(pg);
Person result = await vm.DialogManager.ExecuteDialogTaskAsync();

```

The call to `DialogManager.ExecuteModalTaskAsync()` will be discussed later in the [**`DialogManager`**](#dialogmanager-class) section. Instead of typing these seven mostly boilerplate lines of code, the simpler approach would be to use `NavigateToModalPage`:

```cs
public static class NavigationService
{
    public static async Task<Person> GoToEditPersonPageAsync(Person person) =>
        await CoreNavigation.NavigateToModalPageAsync<Person, EditPersonPage, EditPersonPageViewModel>(person);
}
```
That would be invoked in the ViewModel as in this example:

```cs
Person result = await NavigationService.GoToEditPersonPageAsync(person);
```
Because the modal page is executed using a TaskCompletionSource, program control will suspend within the ViewModel until the modal dialog page is dismissed, at which time the result will be returned to the caller.

### `ModalViewModelBase<T>` class

The abstract base class `ModalViewModelBase<T>`, in addition to providing the fuctionality of the `ViewModelBase<T>` class, adds `DialogManager` support to the mix. The `DialogManager` class will be discussed in next section. This base class is provided as a convenience, as it adds and implements the `IDialogSupport<T>` interface, and its use removes the need to manually declare and initialize the `DialogManager<T>` ViewModel member. However if you need to implement ViewModel inheritance, you will need to utilize `ViewModelBase<T>` and implement `IDialogSupport<T>` manually. See [**ViewModel Inheritance**](#viewmodel-inheritance) later in this document.


## DialogManager class

The purpose of the `DialogManager` class is to encapsulate all of the goodness we would expect from a modal dialog into an easy to reuse component. In addition to providing Save and Cancel buttons, there are bindable properties for each button's `Text`, `Command` and `CommandParameter` properties, as well as others to control visual aspects of the buttons. (And bindable properties mean the `DialogManager` component can participate in XAML-based data binding.)

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
The `IDialogSupport<TModel>` interface requires a read/init `DialogManager<TModel>` property. Once declared, the `DialogManager` will collaborate with the View to present a default `SaveBarView` view. Page designers can add a `SaveBarView` control (or any view implementing the 'ISaveBarView' interface) to the page. If not provided, and one will be added automagically. Control freaks can even override the save bar injection entirely with their own implementation of the `ISaveBarView` interface.


### The hidden `ToolbarManager` component

A dialog page would not be complete without providing a means for users to accept changes or cancel out entirely. But where on the screen those buttons should be depends on the device platform and idiom. Desktop computers traditionally have a button bar below the page content, while mobile devices have ToolbarItem buttons that are displayed in the navigation bar. Sometimes you want both. Or the other. Or neither. That's all configurable thanks to the `ToolbarManager` component.

You'll never directly deal with `ToolbarManager`, but buried under the `DialogManager` hood, it takes care of managing which Save and Cancel buttons are displayed, as well as the commands and text strings to display in the buttons.

By default, `ToolbarItem` views will be used for mobile devices and the internal `SaveBarView` will be used for desktop environments. The `DisplayMode` property allows the developer to configure what buttons the `ToolbarManager` will display any combination of  `ToolbarItem` controls in the navigation bar and `ISaveBarView`. The default can be configured application-wide by calling this method near the top of the App.xaml.cs file:

```
CoreNavigation.ConfigureDefaultButtonBarDisplayMode(SaveBarDisplayMode.BothToolBarAndSaveBar);
```


### `ISaveBarView` implementation

At runtime, `DialogManager` searches for an instance of ISaveBarView within the `ContentPage` visual tree. If it cannot find one, it will create an instance of `ViewModelToolkit.Views.SaveBarView` and insert it in the page at the end of the first `Layout` derived control it finds. Depending on the page design, this may be unsuitable behavior.

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
        <Grid RowDefinitions="100,*,44">
            <!-- page content redacted -->
            
            <vmtv:SaveBarView Grid.Row="2" SaveButtonText="Submit"/>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>

```

For the most flexible solution, the developer can design their own save bar. They need only implement the `ISaveBarView` interface, which ensures the Save and Cancel button properties are declared. Note that these buttons don't need to be actual buttons: any control can be used provided it supports the commanding structure as described above. Even an image with a hit map could be used, and the `ISaveBarButtonView` interface is provided to simplify the task of developing the custom save bar buttons.

The ViewModelToolkitSample application contains an example of a custom save bar that is vertically oriented and includes an additional Help button. 

#### Custom `ISaveBarView` Injection

If the developer would like to provide an instance of an `ISaveBarView` implemented control at **runtime**, the `CoreNavigation.NavigateToModalPageAsync` function has an optional parameter, `saveBarInjector: Func<TView, ISaveBarView>` to facilitate this. The supplied function will receive the newly created `ContentPage` instance. The developer should instantiate their own ISaveBarView class here and insert it into the `ContentPage` visual tree, returning a reference to the custom save bar.

The ViewModelToolkitSample also contains an example of this usage in the `NavigationService.GoToCustomFormPageAsync` function.

## ViewModel Configuration

Since "Save" and "Cancel" are not always the most suitable strings for the buttons and default button actions often need to be replaced, `DialogManager` allows the developer to customize these features on a per-ViewModel basis. While these changes can be declared in XAML as mentioned above, it's often more appropriate for customization to be made in the ViewModel (where some could argue it belongs in MVVM).

For example, a "wizard-style" multi-page dialog would need to provide a custom navigation experience. Here's what that could look like:

```cs
public override void Initialize(Person item) {
    base.Initialize(item);
    // initialization redacted

    DialogManager.DisplayMode = SaveBarDisplayMode.BothButtonBarAndToolBar;
    DialogManager.SaveButtonCommand = ContinueCommand;
    DialogManager.SaveButtonText = "Continue";
    DialogManager.CancelButtonText = "Back";
}
```

The declaration of `ContinueCommand` is described the next section.


## ViewModel Inheritance

More complex applications can require higher levels of inheritence among ViewModels. This can easily be accomplished using `ViewModelBase<T>`. See the Customer Editor Dialog sample for an illustration. Each of the three pages of that dialog inherit from a common `CustomerViewModelBase` class, which contains a `Source` property of type `Customer`, the notification properties, a base `Initialize(T item)` method, an `Update()` override, and other members common to all three pages.

Each of the edit customer pages need only declare the specific notification properties and commands needed to provide the forward navigation.  Here's an example of how `ContinueCommand` could handle the navigation from page one to page two:

```cs
public Command ContinueCommand => _ContinueCommand ??= new Command(async p => {
    if ( Validate() ) {
        Customer current = Update();
        Customer result = await NavigationService.GoToEditCustomerStep2PageAsync(current);
        if ( result is not null ) {
            ExecuteCleanly(() =>
                Initialize(result));
            DialogManager.ExecuteDefaultSaveButtonCommand();
        }
    }
}, _ => !IsNewAccount || (IsDirty && IsValid));
Command _ContinueCommand;
```

After running validation on the ViewModel, the command navigates to page two of the dialog, passing the current values from the ViewModel properties. Upon successful return from page two, page one is reinitialized with the results (the `ExecuteCleanly()` method executes the provided action without modifying the IsDirty flag). Finally it calls the default SaveButtonCommand to handle returning and back navigation.

Speaking of that back navigation, consider our three page dialog: if the user is on page three and presses the Save button, we don't want the user have to watch the previous two modal pages animate off screen. But if the Cancel button is pressed, we do want that single backward animation. To that end, there is an optional parameter on the `NavigateToModalPageAsync` function called `shouldSuppressReturnNavigationAnimation` of type `Func<T, bool>`. If specified, the provided function will be executed to answer the question.

```cs
public static async Task<Customer> GoToEditCustomerStep2PageAsync(Customer customer) =>
    await CoreNavigation.NavigateToModalPageAsync<Customer, EditCustomerStep2Page, EditCustomerStep2PageViewModel>
        (customer,
         shouldSuppressReturnNavigationAnimation: p => !p.IsDefault());
```
In this snippet, the `GoToEditCustomerStep2PageAsync()` method is passed a function that returns false (don't suppress animation) if the Customer is null (the user cancelled) and true (don't animate)  otherwise, meaning do not animate when the user saves their changes.

The `IsDefault()` function is an extension method that determines whether a variable is set to the default value for that type (null for objects or value type defaults otherwise). This parameter is added to the navigation methods for both pages two and three, so that when the user finally presses the Save button on page three, return navigation is skipped going back to the home page.

## Dependency Injection

As developers, we strive to keep our code clean, nonrepetitive, and managable. We use patterns such as Model-View-ViewModel to separate user interface from business logic. Dependency Injection has become a valuable tool to this end, and ViewModelToolkit provides a hook to allow for usage of virtually any DI tool available for the .NET platform.

Under the hood, CoreNavigation functions `NavigateToPage` and `NavigateToModalPage` do their magic by creating instances of your page View and ViewModel classes for you. This creation is managed by abstracting out calls into a default dependency resolver that simply uses `Activator.CreateInstance<T>()`.

To configure CoreNavigation to use your own, either implement the `IDependencyResolver` interface on your DI container or on a helper class with access to your DI container. The interface requires a single function declared, `T Resolve<T>()`:

```cs
public class MyCustomDependencyResolver : IDependencyResolver
{
    public T Resolve<T>() where T : class {
        // Call into your own DI container here. The default implementation
        //   simply performs the following statement:
        // return Activator.CreateInstance<T>();
    }
}
```

Then configure CoreNavigation near the top of your App.xaml.cs constructor:

```cs
CoreNavigation.ConfigureDependencyResolver(new MyCustomDependencyResolver());
```


## Exception Handling

All good applications deal with the unexpected in a consistent manner. ViewModelToolkit provides a simple exception handling mechanism that can easily be expanded or integrated into an existing exception management system. Interface `IExceptionService` allows this expansion point. By default, `DefaultExceptionService` implements method `HandleException(Exception)` by writing the exception information to the debug window and rethrowing the exception. The developer can declare their own custom `IExceptionService`-implemented instance near the top of the App.xaml.cs file as follows:

```
CoreNavigation.ConfigureExceptionHandler(new MyExceptionHander());
```


