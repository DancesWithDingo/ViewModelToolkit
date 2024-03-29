﻿using System.Diagnostics;
using ViewModelToolkit;
using ViewModelToolkit.Dialogs;
using ViewModelToolkit.Services;

namespace ViewModelToolkitSample;

public partial class App : Application
{
    public App() {
        InitializeComponent();

        // Allows setting an application-wide default for all modal pages:
        //CoreNavigation.ConfigureDefaultButtonBarDisplayMode(SaveBarDisplayMode.Default);

        // Allows setting an application-wide default for details of the
        //   alert window presented when cancelling out of a dialog with changes:
        CoreNavigation.ConfigureDefaultCancelWhenDirtyAlertDetails(new AlertDetails(title: "Forget your changes?", yesText: "Continue"));

        // Register the dependency injection container:
        CoreNavigation.ConfigureDependencyResolver(new IocResolver());

        CoreNavigation.ConfigureExceptionHandler(new MyExceptionHander());

        var pg = new Views.MainPage();
        var vm = new ViewModels.MainPageViewModel();
        pg.BindingContext = vm;

        MainPage = new NavigationPage(pg);
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
