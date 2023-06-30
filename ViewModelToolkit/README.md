
# ViewModelToolkit

ViewModelToolkit is a set of .NET MAUI tools for C# developers using the Model-View-ViewModel (MVVM) pattern. The package includes ViewModelBase, DialogManager and CoreNavigation, three components that can make a programmer's job much easier while maintaining a simple, uniform and concise codebase.

In addition to providing the abstract `ViewModelBase` class, the additional components make it easier to navigate to pages *modally*, treating each navigation as an atomic awaitable task. Gone are the days tracking whether the user hit the cancel button, back button or the device hardware button. No matter how the user closes the dialog, control returns to the statement *after* the navigation call, returning the result of the dialog to the call site.

ViewModelToolkit provides three ViewModelBase base classes:

1. `ViewModelBase`, used for simple navigation to another page,
2. `ViewModelBase<T>`, used for simple navigation to another page with a strongly typed model object passed to the `Initialize` method,
3. `ModalViewModelBase<T>`, used for task-based modal navigation to another page, adding toolbar items and/or a button bar with Save and Cancel buttons.


## Documentation
Complete documentation is available in the GitHub repository: https://github.com/DancesWithDingo/ViewModelToolkit.git
