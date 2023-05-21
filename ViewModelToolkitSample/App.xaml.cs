﻿using System.Diagnostics;
using ViewModelToolkit;
using ViewModelToolkit.Modals;
using ViewModelToolkit.Services;

namespace ViewModelToolkitSample;

public partial class App : Application
{
    public App() {
        InitializeComponent();

        // Allows setting an application-wide default for all modal pages:
        CoreNavigation.ConfigureDefaultButtonBarDisplayMode(SaveBarDisplayMode.Default);

        // Allows setting an application-wide default for details of the
        //   alert window presented when cancelling out of a dialog with changes:
        CoreNavigation.ConfigureDefaultCancelWhenDirtyAlertDetails(new AlertDetails(title: "Forget your changes?"));


        // Allows informing the engine to call into your dependency injection container:
        // CoreNavigation.ConfigureDependencyResolver(new MyCustomDependencyResolver());

        CoreNavigation.ConfigureExceptionHandler(new MyExceptionHander());

        var pg = new Views.MainPage();
        var vm = new ViewModels.MainPageViewModel();
        pg.BindingContext = vm;

        MainPage = new NavigationPage(pg);
    }

    public class MyCustomDependencyResolver : IDependencyResolver
    {
        public T Resolve<T>() {
            // Call into your own container here. The default implementation
            //   simply performs the following statement:
            return Activator.CreateInstance<T>();
        }
    }

    public class MyExceptionHander : IExceptionService
    {
        public void HandleException(Exception exception) {
            Debug.WriteLine($"MyExceptionHandler => exception:");
            Debug.WriteLine($"->  Message: {exception.Message}");
            Debug.WriteLine($"->  StackTrace: {exception.StackTrace}");
            Debug.WriteLine($"->  InnerException: {exception.InnerException}");
        }
    }
}
